using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;

        private readonly IMapper mapper;

        private readonly ILogger<VillaNumberAPIController> logger;

        private readonly IVillaNumberRepository _dbVillaNumbers;

        public VillaNumberAPIController(IMapper _mapper, ILogger<VillaNumberAPIController> logger, IVillaNumberRepository dbVilla)
        {
            this.mapper = _mapper;
            this.logger = logger;
            this._dbVillaNumbers = dbVilla;
            this._response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                logger.LogInformation("Fetching All Villa Numbers");

                var villaList = await _dbVillaNumbers.GetAsyncList(includeProperties:"Villa");

                _response.Result = mapper.Map<List<VillaNumberDTO>>(villaList);

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

        [HttpGet("{villaNumber:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNumber)
        {
            try
            {
                if (villaNumber == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVillaNumbers.GetAsync(p => p.VillaNo == villaNumber);

                if (villa == null)
                {
                    return NotFound();
                }

                _response.Result = mapper.Map<VillaNumberDTO>(villa);

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
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createVillaDTO)
        {
            try
            {
                if (createVillaDTO == null)
                {
                    return BadRequest();
                }
                if (await _dbVillaNumbers.GetAsync(p => p.VillaNo == createVillaDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa Number already Exist!");

                    return BadRequest(ModelState);
                }

                VillaNumber model = mapper.Map<VillaNumber>(createVillaDTO);

                await _dbVillaNumbers.CreateAsync(model);

                _response.Result = model;

                _response.StatusCode = System.Net.HttpStatusCode.OK;

                _response.IsSuccess = true;

                return CreatedAtRoute("GetVillaNumber", new { villaNumber = model.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{villaNumber:int}", Name = "DeleteVillaNumber")]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int villaNumber)
        {
            try
            {
                if (villaNumber == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVillaNumbers.GetAsync(p => p.VillaNo == villaNumber);

                if (villa == null) { return NotFound(); }

                await _dbVillaNumbers.RemoveAsync(villa);

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

        [HttpPut("{villaNumber:int}", Name = "UpdateVillaNumber")]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNumber, [FromBody] VillaNumberUpdateDTO villaUpdateDTO)
        {
            try
            {
                if (villaUpdateDTO == null || villaUpdateDTO.VillaNo != villaNumber)
                {
                    return BadRequest();
                }

                var villa = mapper.Map<VillaNumber>(villaUpdateDTO);

                await _dbVillaNumbers.UpdateAsync(villa);

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

        [HttpPatch("{villaNumber:int}", Name = "UpdatePartialVillaNumber")]
        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int VillaNumber, JsonPatchDocument<VillaNumberUpdateDTO> patchDto)
        {
            try
            {
                if (patchDto == null || VillaNumber == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVillaNumbers.GetAsync(p => p.VillaNo == VillaNumber, false);

                if (villa == null)
                {
                    return NotFound();
                }

                var villaUpdateDTO = mapper.Map<VillaNumberUpdateDTO>(villa);

                patchDto.ApplyTo(villaUpdateDTO, ModelState);

                var model = mapper.Map<VillaNumber>(villaUpdateDTO);

                await _dbVillaNumbers.UpdateAsync(model);

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
