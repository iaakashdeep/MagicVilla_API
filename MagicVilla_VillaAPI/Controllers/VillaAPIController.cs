using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.villaList);
        }
        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id) {
            if(id == 0)
            {
                return BadRequest();
            }
            var villaDetails= VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            if(villaDetails == null)
            {
                return NotFound();
            }
            return Ok(villaDetails);
        }

        //Actions require a unique method/path combination for Swagger/OpenAPI 3.0. Use ConflictingActionsResolver as a workaround
        //Above error will be shown because there are 2 httpget requests so we have to define id in one action method

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla(VillaDTO villaDTO)
        {
            //if (!ModelState.IsValid) { 
            //    return BadRequest(ModelState);
            //}

            //To check villa name is not already existing
            var villaNameforDB=VillaStore.villaList.FirstOrDefault(x => x.Name.ToLower() == villaDTO.Name.ToLower());
            if(villaNameforDB!=null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exists!");
                //CustomError-> is the key and it should be unique
                return BadRequest();
            }
            if (villaDTO == null)
            {
                return BadRequest();
            }
            if (villaDTO.Id >0) {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDTO.Id = VillaStore.villaList.OrderByDescending(x=>x.Id).First().Id+1;
            VillaStore.villaList.Add(villaDTO);

            return CreatedAtRoute(nameof(GetVilla), new {id=villaDTO.Id},villaDTO);

            //CreatedAtRoute-> The POST /api/CreteVilla action creates a new product and then returns a 201 created response with a location header pointing to 
            //GET /api/GetVilla, this also sens the created object in the body section of response

            //Condition on line: 44 will only execute when there is no [ApiController] attached at controller level if that is there you don't
            //need to add this condition explicitly

            //if the condition on line: 44 is still there then before coming to this condition the Model State error will be checked because
            //we have [ApiController] attached
        }
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var dleteVilladetails = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            if (dleteVilladetails != null)
            {
                VillaStore.villaList.Remove(dleteVilladetails);
                return NoContent();
            }
            return BadRequest();
            
        }

        //With IActionResult you don't need to explicitly define the return type as we did by using ActionResult also there are some
        //return types that only supports IActionResult like; NoContent
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id,VillaDTO villaDTO)
        {
            if (villaDTO == null && id != villaDTO.Id) { 
                return BadRequest();
            }
            var villaDetails=VillaStore.villaList.FirstOrDefault(y => y.Id == id);
            if (villaDetails != null) { 
                villaDetails.Name = villaDTO.Name;
                villaDetails.Occupancy = villaDTO.Occupancy;
                villaDetails.Sqft = villaDTO.Sqft;
                return NoContent();
            }
            return BadRequest();

        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdatePartialVilla(int id,JsonPatchDocument<VillaDTO> patchDTO)
        {
            //When we are working with PATCH we receive JsonPatchDocument having the object that needs to be updated, we have to define the type of
            //JsonPatchDocument like JsonPatchDocument<VillaDTO>
            if (id != 0 && patchDTO != null)
            {
                var patchDetails=VillaStore.villaList.FirstOrDefault(x=>x.Id == id);
                if (patchDetails != null) { 
                    patchDTO.ApplyTo(patchDetails,ModelState);   //ModelState-> used to handle any error
                    //ApplyTo is used to apply the changes 
                    if (!ModelState.IsValid)
                    {
                        return BadRequest();
                    }
                    return NoContent();
                }
            }
            return BadRequest();

            ///Sample Request Body of Patch:
            ///[
    //        {
    //            "operationType": 0,
    //"path": "string",     ->property that needs to be updated 
    //"op": "replace",   -> Opearation which we need to perform in update case here replace will come
    //"from": "string",
    //"value": "string"     -> Updated value of the property
    //        }
//]
        }
    }
}
