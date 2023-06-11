using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PexelsDotNetSDK.Api;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/VilllAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogger<VillaAPIController> logger;

        private readonly ApplicationDbContext dbContext;

        public VillaAPIController(ILogger<VillaAPIController> logger, ApplicationDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }
        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            logger.LogInformation("Fetching All Villas");

            return Ok(dbContext.Villas.ToList());
        }
        
        [HttpGet("{id:int}", Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(200, Type = typeof(VillaDTO))]
        // [ProducesResponseType(404)]
        // [ProducesResponseType(400)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = dbContext.Villas.FirstOrDefault(p => p.ID == id);
           
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        public ActionResult<VillaDTO> CreateVilla([FromBody] Villa villa)
        {
           /// if (ModelState.IsValid)
           /// {
           ///     return BadRequest(ModelState);
           /// }
            if (villa == null)
            {
                return BadRequest();
            }
            if(dbContext.Villas.Any(p => p.Name.ToLower() == villa.Name.ToLower()))
            {
                ModelState.AddModelError("CustomError", "Villa already Exist!");
                return BadRequest(ModelState);
            }

            if(villa.ID == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villa.ID = dbContext.Villas.OrderByDescending(p => p.ID).FirstOrDefault().ID + 1;
           
            dbContext.Villas.Add(villa);
           
            return CreatedAtRoute("GetVilla",new { id = villa.ID}, villa);

        }

        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        public IActionResult DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = dbContext.Villas.FirstOrDefault(p => p.ID == id);

            if(villa == null) { return NotFound(); }

            dbContext.Villas.Remove(villa);

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDto)
        {
            if(villaDto == null || villaDto.ID != id)
            {
                return BadRequest();
            }
            var villa = dbContext.Villas.FirstOrDefault(p => p.ID == id);
            villa.Name = villaDto.Name;
            villa.Sqft = villaDto.Sqft;
            villa.Occupancy = villaDto.Occupancy;
            return NoContent();
        }


        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public IActionResult UpdatePartialVilla(int id,JsonPatchDocument<Villa> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = dbContext.Villas.FirstOrDefault(p => p.ID == id);

            if (villa == null)
            {
                return NotFound();
            }
            patchDto.ApplyTo(villa, ModelState);
            return NoContent();
        }

        [HttpPost("GenerateVillas", Name = "GenerateVillas")]
        public async Task<IActionResult> GenerateVillas()
        {
            var pexelsClient = new PexelsClient("API_Key");

            var result = await pexelsClient.SearchPhotosAsync("villa", pageSize: 80);

            for (int i = 0; i <= 79; i++)
            {
                Villa villa = new Villa
                {
                  
                    Name = $"Villa {i}",
                    Details = $"Details for Villa {i}",
                    Rate = GenerateRandomRate(),
                    ImageUrl = result.photos[i].url,
                    Amenity = $"Amenity for Villa {i}",
                    CreatedDate = GenerateRandomDate(),
                    UpdatedDate = GenerateRandomDate(),
                    Occupancy = GenerateRandomOccupancy(),
                    Sqft = GenerateRandomSqft()
                };

                dbContext.Villas.Add(villa);
            }

            await dbContext.SaveChangesAsync();

            return Ok("Villas generated successfully.");
        }
        // Generate a random rate between 1 and 10
        private static double GenerateRandomRate()
        {
            Random random = new Random();
            return random.Next(1, 11);
        }


        // Generate a random date between January 1, 2020 and December 31, 2022
        private static DateTime GenerateRandomDate()
        {
            Random random = new Random();
            DateTime start = new DateTime(2020, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(random.Next(range));
        }

        // Generate a random occupancy between 1 and 10
        private static int GenerateRandomOccupancy()
        {
            Random random = new Random();
            return random.Next(1, 11);
        }

        // Generate a random square footage between 1000 and 5000
        private static int GenerateRandomSqft()
        {
            Random random = new Random();
            return random.Next(1000, 5001);
        }

    }
}
