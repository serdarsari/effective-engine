using GalaxyExplorer.DTO;
using GalaxyExplorer.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GalaxyExplorer.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoyagerController : ControllerBase
    {
        // DI Container'a kayıtlı IMissionService uyarlaması kimse o gelecek
        private readonly IVoyagerService _voyagerService;
        public VoyagerController(IVoyagerService voyagerService)
        {
            _voyagerService = voyagerService;
        }
        [HttpGet]
        public async Task<IActionResult> GetVoyagers([FromQuery] GetVoyagersRequest request) // Parametreleri QueryString üzerinden almayı tercih ettim
        {
            var voyagers = await _voyagerService.GetVoyagers(request);
            return Ok(voyagers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoyager([FromBody] CreateVoyagerRequest request){        //Yeni bir mürettebat üyesi eklemek için
            if (!ModelState.IsValid)
                return BadRequest(); // Model validasyon kurallarında ihlal olursa
                
            var createVoyagerResult = await _voyagerService.CreateVoyager(request);     //Servisdeki metodu çağıralım.
            if (createVoyagerResult.Success)
                return Ok(createVoyagerResult.Message);
            else
                return BadRequest(createVoyagerResult.Message);
        }
    }
}
