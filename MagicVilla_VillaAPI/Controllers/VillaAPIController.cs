﻿using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/VilllAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.villaList);
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
            var villa = VillaStore.villaList.FirstOrDefault(p => p.ID == id);
           
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villa)
        {
           /// if (ModelState.IsValid)
           /// {
           ///     return BadRequest(ModelState);
           /// }
            if (villa == null)
            {
                return BadRequest();
            }
            if(VillaStore.villaList.Any(p => p.Name.ToLower() == villa.Name.ToLower()))
            {
                ModelState.AddModelError("CustomError", "Villa already Exist!");
                return BadRequest(ModelState);
            }

            if(villa.ID == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villa.ID = VillaStore.villaList.OrderByDescending(p => p.ID).FirstOrDefault().ID + 1;
           
            VillaStore.villaList.Add(villa);
           
            return CreatedAtRoute("GetVilla",new { id = villa.ID}, villa);

        }

        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        public IActionResult DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(p => p.ID == id);

            if(villa == null) { return NotFound(); }

            VillaStore.villaList.Remove(villa);

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDto)
        {
            if(villaDto == null || villaDto.ID != id)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(p => p.ID == id);
            villa.Name = villaDto.Name;
            villa.Sqft = villaDto.Sqft;
            villa.Occupancy = villaDto.Occupancy;
            return NoContent();
        }


        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public IActionResult UpdatePartialVilla(int id,JsonPatchDocument<VillaDTO> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(p => p.ID == id);

            if (villa == null)
            {
                return NotFound();
            }
            patchDto.ApplyTo(villa, ModelState);
            return NoContent();
        }

    }
}
