using GalaxyExplorer.DTO;
using GalaxyExplorer.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GalaxyExplorer.Service
{
    public class VoyagerService
        : IVoyagerService
    {
        private readonly GalaxyExplorerDbContext _dbContext;
        // Servisi kullanan uygulamanın DI Container Service Registery'si üzerinden gelecektir.
        // O anki opsiyonları ile birlikte gelir. SQL olur, Postgresql olur, Mongo olur bilemiyorum.
        // Entity modelin uygun düşen bir DbContext gelecektir.
        public VoyagerService(GalaxyExplorerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<GetVoyagersResponse> GetVoyagers(GetVoyagersRequest request)
        {
            var currentStartRow = (request.PageNumber - 1) * request.PageSize;
            var response = new GetVoyagersResponse
            {
                // Kolaylık olsun diye sonraki sayfa için de bir link bıraktım
                // Lakin başka kayıt yoksa birinci sayfaya da döndürebiliriz
                NextPage = $"api/voyager?PageNumber={request.PageNumber + 1}&PageSize={request.PageSize}&OnMission={request.OnMission}", 
                TotalVoyagers = await _dbContext.Voyagers.CountAsync(),
                TotalActiveVoyagers = await _dbContext.Voyagers.CountAsync(v => v.OnMission == true)
            };

            var voyagers = await _dbContext.Voyagers
                .Where(v => v.OnMission == request.OnMission)
                .Skip(currentStartRow)
                .Take(request.PageSize)
                .Select(v => new VoyagerResponse
                {
                    Name = v.Name,
                    Grade = v.Grade,
                    Detail = $"api/voyager/{v.VoyagerId}" // Bu Voyager'ın detaylarını görmek için bir sayfaya gitmek isterse diye
                })
                .ToListAsync();
            response.Voyagers = voyagers;

            return response;
        }
    }
}