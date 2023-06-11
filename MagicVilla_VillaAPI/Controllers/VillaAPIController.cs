using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/VilllAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly IMapper mapper;

        private readonly ILogger<VillaAPIController> logger;

        private readonly ApplicationDbContext dbContext;

        public VillaAPIController(IMapper _mapper,ILogger<VillaAPIController> logger, ApplicationDbContext dbContext)
        {
            this.mapper = _mapper;
            this.logger = logger;
            this.dbContext = dbContext;
        }
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            logger.LogInformation("Fetching All Villas");

            var villaList = await dbContext.Villas.ToListAsync();

            return Ok(mapper.Map<List<VillaDTO>>(villaList));
        }
        
        [HttpGet("{id:int}", Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = await dbContext.Villas.FirstOrDefaultAsync(p => p.ID == id);
           
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<VillaDTO>(villa));
        }

        [HttpPost]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createVillaDTO)
        {
            if (createVillaDTO == null)
            {
                return BadRequest();
            }
            if(dbContext.Villas.FirstOrDefaultAsync(p => p.Name.ToLower() == createVillaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exist!");
                return BadRequest(ModelState);
            }


            Villa model = mapper.Map<Villa>(createVillaDTO);

            await dbContext.Villas.AddAsync(model);
            await dbContext.SaveChangesAsync();


            return CreatedAtRoute("GetVilla",new { id = model.ID}, model);

        }

        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = await dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(p => p.ID == id);

            if(villa == null) { return NotFound(); }

            dbContext.Villas.Remove(villa);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaUpdateDTO)
        {
            if(villaUpdateDTO == null || villaUpdateDTO.ID != id)
            {
                return BadRequest();
            }

             var villa = mapper.Map<Villa>(villaUpdateDTO);

             dbContext.Villas.Update(villa);
             await dbContext.SaveChangesAsync();
             return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public async Task<IActionResult> UpdatePartialVilla(int id,JsonPatchDocument<VillaUpdateDTO> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(p => p.ID == id);

            if (villa == null)
            {
                return NotFound();
            }

            var villaUpdateDTO = mapper.Map<VillaUpdateDTO>(villa);

            patchDto.ApplyTo(villaUpdateDTO, ModelState);

            var model = mapper.Map<Villa>(villa);

            dbContext.Villas.Update(model);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

    }
}
