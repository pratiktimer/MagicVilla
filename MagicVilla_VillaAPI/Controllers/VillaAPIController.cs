using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/[VilllAPI]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Villa> GetVillas()
        {
            return new List<Villa>()
            {
                new Villa() { ID = 1, Name = "Pol View" },
                new Villa() { ID = 2, Name = "Ocean Paradise" },
                new Villa() { ID = 3, Name = "Sunset Retreat" },
                new Villa() { ID = 4, Name = "Tropical Haven" },
                new Villa() { ID = 5, Name = "Island Oasis" },
                new Villa() { ID = 6, Name = "Seaside Serenity" },
                new Villa() { ID = 7, Name = "Palm Villa" },
                new Villa() { ID = 8, Name = "Vista Del Mar" },
                new Villa() { ID = 9, Name = "Azure Dream" },
                new Villa() { ID = 10, Name = "Harmony Hills" },
                // Add more villa objects with fake data as needed
            };
        }
    }
}
