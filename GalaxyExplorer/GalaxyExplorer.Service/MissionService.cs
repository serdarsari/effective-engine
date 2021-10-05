using GalaxyExplorer.DTO;
using GalaxyExplorer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GalaxyExplorer.Service
{
    public class MissionService
        : IMissionService
    {
        private readonly GalaxyExplorerDbContext _dbContext;
        // Servisi kullanan uygulamanın DI Container Service Registery'si üzerinden gelecektir.
        // O anki opsiyonları ile birlikte gelir. SQL olur, Postgresql olur, Mongo olur bilemiyorum.
        // Entity modelin uygun düşen bir DbContext gelecektir.
        public MissionService(GalaxyExplorerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<MissionStartResponse> StartMissionAsync(MissionStartRequest request)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(); // Transaction başlatalım
            try
            {
                // Mürettebat sayısı uygun olup aktif görevde olmayan bir gemi bulmalıyız. Aday havuzunu çekelim.
                var crewCount = request.Voyagers.Count;
                var candidates = _dbContext.Spaceships.Where(s => s.MaxCrewCount >= crewCount && s.OnMission == false).ToList();
                if (candidates.Count > 0)
                {
                    Random rnd = new();
                    var candidateId = rnd.Next(0, candidates.Count);
                    var ship = candidates[candidateId]; // Index değerine göre rastgele bir tanesini alalım

                    ship.OnMission = true;
                    await _dbContext.SaveChangesAsync(); // Gemiyi görevde durumuna alalım

                    // Görev nesnesini oluşturalım
                    Mission mission = new()
                    {
                        Name = request.Name,
                        PlannedDuration = request.PlannedDuration,
                        SpaceshipId = ship.SpaceshipId, // Gemi ile ilişkilendirdik
                        StartDate = DateTime.Now,
                        IsCompleted = false,        //Görev yeni başlayacağı için IsCompleted parametresini false verelim.
                    };
                    await _dbContext.Missions.AddAsync(mission);
                    await _dbContext.SaveChangesAsync(); // Görev nesnesini db'ye yollayalım

                    List<Voyager> voyagersFound = new List<Voyager>();   
                    string voyagerIDsNotFound = "";
                    foreach (var rv in request.Voyagers)
                    {
                        var voyager = await _dbContext.Voyagers.SingleOrDefaultAsync(v => v.VoyagerId == rv.VoyagerId);   //request ile gelen, göreve yollanacak voyager'ların ID'lerinin veritabanında olup olmadığını kontrol edelim.
                        if (voyager is not null)
                        {
                            voyagersFound.Add(voyager);
                        }
                        else
                        {
                            voyagerIDsNotFound += rv.VoyagerId.ToString() + ",";
                        }
                    }

                    if (voyagerIDsNotFound.Length > 0)  //request ile gelen voyager id'lerinin herhangi biri bile veritabanında yoksa işlemleri iptal et ve hata döndür.
                    {
                        await transaction.RollbackAsync();

                        return new MissionStartResponse
                        {
                            Success = false,
                            Message = $"Görev başlatma başarısız! Şu ID(lere) sahip mürettebat(lar) bulunamadı : {voyagerIDsNotFound}"
                        };
                    }

                    foreach (var v in voyagersFound)    //Eğer girilen tüm voyager id'ler geçerliyse, göreve yollamak için OnMission özelliklerini true yap.
                    {
                        v.OnMission = true;
                    }
                    _dbContext.Voyagers.UpdateRange(voyagersFound);
                    await _dbContext.SaveChangesAsync();         //Voyager ile ilgili yapılanları veritabanına işle.

                    var missionVoyagers = new List<MissionVoyager>();
                    foreach (var v in voyagersFound)
                    {
                        MissionVoyager missionVoyager = new()
                        {
                            MissionId = mission.MissionId,          //Mission tablosundan MissionId,
                            VoyagerId = v.VoyagerId                 //Voyager tablosundan VoyagerId bilgilerini alıp, birbirleriyle ilişkili olan MissionVoyager tablosuna eklemek için hazırla.
                        };
                        missionVoyagers.Add(missionVoyager);
                    }
                    await _dbContext.MissionVoyagers.AddRangeAsync(missionVoyagers);    //MissionVoyager tablosuna bunları ekle.
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync(); // Transaction'ı commit edelim

                    return new MissionStartResponse
                    {
                        Success = true,
                        Message = $"Görev başlatıldı.",
                    };
                }
                else // Müsait veya uygun gemi yoksa burda durmamızın anlamı yok
                {
                    await transaction.RollbackAsync();

                    return new MissionStartResponse
                    {
                        Success = false,
                        Message = "Şu anda görev için müsait gemi yok"
                    };
                }
            }
            catch (Exception exp)
            {
                await transaction.RollbackAsync();
                return new MissionStartResponse
                {
                    Success = false,
                    Message = $"Sistem Hatası:{exp.Message}"
                };
            }
        }

        public async Task<MissionCompletedResponse> UpdateCompletedMissionAsync(int missionId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var mission = await _dbContext.Missions.SingleOrDefaultAsync(m => m.MissionId == missionId && m.IsCompleted == false);   //Görevi(Mission) db'den çekelim. Görevi tamamlama işlemi için görevin önceden tamamlanmamış olması gerekiyor. Bu yüzden IsCompleted alanının false olması zorunluluğunu ekleyelim.
                if (mission is not null)
                {
                    mission.IsCompleted = true;      //İlgili görevin IsCompleted alanını true yaparak tamamlandı durumuna getirelim.
                    await _dbContext.SaveChangesAsync();

                    var missionShip = await _dbContext.Spaceships.SingleOrDefaultAsync(s => s.SpaceshipId == mission.SpaceshipId && s.OnMission == true);   //Görevdeki gemiyi(Spaceship) db'den çekelim.
                    if (missionShip is not null)
                    {
                        missionShip.OnMission = false;        //Görev gemisinin görevden geldiğini belirtmek için OnMission parametresini false olarak değiştirelim.
                        await _dbContext.SaveChangesAsync();      //Değişiklikleri kaydedelim.

                        var missionVoyagers = await _dbContext.MissionVoyagers.Where(mv => mv.MissionId == mission.MissionId).ToListAsync();   //MissionVoyagers tablosundan, ilgili görevle ilişkilendirilen voyager'ları getirelim.
                        if (missionVoyagers.Count > 0)
                        {
                            List<Voyager> voyagers = new List<Voyager>();
                            foreach (var mv in missionVoyagers)
                            {
                                voyagers.Add(await _dbContext.Voyagers.SingleOrDefaultAsync(v => v.VoyagerId == mv.VoyagerId));   //Voyagers tablosundan ilgili görevdeki voyager'ları getirelim. 
                            }

                            foreach (var v in voyagers)
                            {
                                v.OnMission = false;     //Getirilen voyager'ların OnMission parametresini false durumuna getirerek, görevde olmadıklarını(görevden döndüklerini) belirtelim.
                            }
                            _dbContext.Voyagers.UpdateRange(voyagers);
                            await _dbContext.SaveChangesAsync();

                            await transaction.CommitAsync();      //transaction'ı commit edelim.

                            return new MissionCompletedResponse
                            {
                                Success = true,
                                Message = $"{mission.Name} adlı görev başarıyla tamamlandı. Gemi(Spaceship) ve mürettebat(Voyager) başarıyla döndü."
                            };
                        }
                        else
                        {
                            await transaction.RollbackAsync();

                            return new MissionCompletedResponse
                            {
                                Success = false,
                                Message = $"{mission.Name} adlı görevde, mürettebat(Voyager) bilgisi bulunamadı."
                            };
                        }
                    }
                    else
                    {
                        await transaction.RollbackAsync();

                        return new MissionCompletedResponse
                        {
                            Success = false,
                            Message = $"{mission.Name} adlı görevde, gemi(Spaceship) bilgisi bulunamadı."
                        };
                    }
                }
                else    //Görev bulunamazsa, tamamlanamaz zaten. Direkt hata döndürelim.
                {
                    await transaction.RollbackAsync();

                    return new MissionCompletedResponse
                    {
                        Success = false,
                        Message = "Daha önce tamamlanmış bir görevi sonlandırmaya çalışıyor veya yanlış bir ID bilgisi girmiş olabilirsiniz. Görev sonlandırma işlemi başarısız."
                    };
                }
            }
            catch (Exception exp)
            {
                await transaction.RollbackAsync();
                return new MissionCompletedResponse
                {
                    Success = false,
                    Message = $"Sistem Hatası: {exp.Message}"
                };
            }
        }
    }
}
