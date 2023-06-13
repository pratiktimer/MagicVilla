using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
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

        private readonly IVillaRepository _dbVilla;

        public VillaAPIController(IMapper _mapper,ILogger<VillaAPIController> logger, IVillaRepository dbVilla)
        {
            this.mapper = _mapper;
            this.logger = logger;
            this._dbVilla = dbVilla;
        }
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            logger.LogInformation("Fetching All Villas");

            var villaList = await _dbVilla.GetAsyncList(p => p.ID > 0);

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
            var villa = await _dbVilla.GetAsync(p => p.ID == id);
           
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
            if(await _dbVilla.GetAsync(p => p.Name.ToLower() == createVillaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exist!");
                return BadRequest(ModelState);
            }

            Villa model = mapper.Map<Villa>(createVillaDTO);

            await _dbVilla.CreateAsync(model);
          
            return CreatedAtRoute("GetVilla",new { id = model.ID}, model);

        }

        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.GetAsync(p => p.ID == id);

            if(villa == null) { return NotFound(); }

            await _dbVilla.RemoveAsync(villa);
          
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

             await _dbVilla.UpdateAsync(villa);

             return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public async Task<IActionResult> UpdatePartialVilla(int id,JsonPatchDocument<VillaUpdateDTO> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.GetAsync(p => p.ID == id, false);

            if (villa == null)
            {
                return NotFound();
            }

            var villaUpdateDTO = mapper.Map<VillaUpdateDTO>(villa);

            patchDto.ApplyTo(villaUpdateDTO, ModelState);

            var model = mapper.Map<Villa>(villaUpdateDTO);

            await _dbVilla.UpdateAsync(model);

            return NoContent();
        }

    }
}
