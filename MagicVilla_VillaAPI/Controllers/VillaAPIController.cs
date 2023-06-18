using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/VilllAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _response;

        private readonly IMapper mapper;

        private readonly ILogger<VillaAPIController> logger;

        private readonly IVillaRepository _dbVilla;

        public VillaAPIController(IMapper _mapper, ILogger<VillaAPIController> logger, IVillaRepository dbVilla)
        {
            this.mapper = _mapper;
            this.logger = logger;
            this._dbVilla = dbVilla;
            this._response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                logger.LogInformation("Fetching All Villas");

                var villaList = await _dbVilla.GetAsyncList();

                _response.Result = mapper.Map<List<VillaDTO>>(villaList);

                _response.StatusCode = System.Net.HttpStatusCode.OK;

                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(p => p.ID == id);

                if (villa == null)
                {
                    return NotFound();
                }

                _response.Result = mapper.Map<VillaDTO>(villa);

                _response.StatusCode = System.Net.HttpStatusCode.OK;

                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createVillaDTO)
        {
            try
            {
                if (createVillaDTO == null)
                {
                    return BadRequest();
                }
                if (await _dbVilla.GetAsync(p => p.Name.ToLower() == createVillaDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa already Exist!");
                    return BadRequest(ModelState);
                }

                Villa model = mapper.Map<Villa>(createVillaDTO);

                await _dbVilla.CreateAsync(model);

                _response.Result = model;

                _response.StatusCode = System.Net.HttpStatusCode.OK;

                _response.IsSuccess = true;

                return CreatedAtRoute("GetVilla", new { id = model.ID }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(p => p.ID == id);

                if (villa == null) { return NotFound(); }

                await _dbVilla.RemoveAsync(villa);

                _response.StatusCode = System.Net.HttpStatusCode.NoContent;

                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaUpdateDTO)
        {
            try
            {
                if (villaUpdateDTO == null || villaUpdateDTO.ID != id)
                {
                    return BadRequest();
                }

                var villa = mapper.Map<Villa>(villaUpdateDTO);

                await _dbVilla.UpdateAsync(villa);

                _response.StatusCode = System.Net.HttpStatusCode.NoContent;

                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDto)
        {
            try
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

                _response.StatusCode = System.Net.HttpStatusCode.NoContent;

                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

    }
}
