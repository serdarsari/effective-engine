using GalaxyExplorer.DTO;
using GalaxyExplorer.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GalaxyExplorer.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        // DI Container'a kayıtlı IMissionService uyarlaması kimse o gelecek
        private readonly IMissionService _missionService;
        public MissionController(IMissionService missionService)
        {
            _missionService = missionService;
        }
        [HttpPost]
        public async Task<IActionResult> StartAsync([FromBody] MissionStartRequest request) // JSON Body'den request nesnesini alsın
        {
            if (!ModelState.IsValid)
                return BadRequest(); // Model validasyon kurallarında ihlal olursa

            // Servis metodunu çağıralım
            var startResult = await _missionService.StartMissionAsync(request);
            if (startResult.Success) // Sonuç başarılı ise HTTP OK
                return Ok(startResult.Message);
            else
                return BadRequest(startResult.Message); // Değilse HTTP Bad Request
        }

        [HttpPut("{missionId}")]
        public async Task<IActionResult> CompletedAsync(int missionId){     //Route ile missionId parametresini alalım.
            var completedResult = await _missionService.UpdateCompletedMissionAsync(missionId);    //Servisdeki ilgili metodu çağıralım.
            if(completedResult.Success)
                return Ok(completedResult.Message);
            else
                return BadRequest(completedResult.Message);
        }
    }
}
