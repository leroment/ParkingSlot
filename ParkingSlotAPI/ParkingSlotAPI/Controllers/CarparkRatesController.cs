using Microsoft.AspNetCore.Mvc;
using ParkingSlotAPI.PublicAPI;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarparkRatesController : ControllerBase
    {
        private readonly IFetchPublicAPI _fetchPublicAPI;

        public CarparkRatesController(IFetchPublicAPI fetchPublicAPI)
        {
            _fetchPublicAPI = fetchPublicAPI;
        }

        [HttpGet]
        public IActionResult GetCarparkRatesForCarpark()
        {
            
            return Ok();
        }
    }
}