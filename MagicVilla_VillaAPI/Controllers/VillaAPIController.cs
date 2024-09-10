using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ApiDBContext _Db;

        //private readonly ILogging _Logger;
        //As we are using DI so we don't need any modification for seri log implemenation here

        public VillaAPIController(ApiDBContext db)
        {
            //_Logger = logger;
            _Db = db;
        }

        //Uncomment if we want to use custom logging

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(_Db.VillaAPIs.ToList());
        }
        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id) {
            if(id == 0)
            {
                //_Logger.LogError("get villa error with Id" + id);
                return BadRequest();
            }
            var villaDetails= _Db.VillaAPIs.FirstOrDefault(x => x.Id == id);
            if(villaDetails == null)
            {
                return NotFound();
            }
            return Ok(villaDetails);
        }


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
            var villaNameforDB=_Db.VillaAPIs.FirstOrDefault(x => x.Name.ToLower() == villaDTO.Name.ToLower());
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
                //return StatusCode(StatusCodes.Status500InternalServerError);
            }
            //Converting VillaDTO object to VillaAPI for inserting in DB

            VillaAPI model= new VillaAPI()
            {
                Name=villaDTO.Name,
                Amenity=villaDTO.Amenity,
                Details=villaDTO.Details,
                ImageUrl = villaDTO.ImageUrl,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
                Id=villaDTO.Id
            };
            _Db.VillaAPIs.Add(model);
            _Db.SaveChanges();
            

            return CreatedAtRoute(nameof(GetVilla), new {id=villaDTO.Id},villaDTO);

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
            var dleteVilladetails = _Db.VillaAPIs.FirstOrDefault(x => x.Id == id);
            if (dleteVilladetails != null)
            {
                _Db.VillaAPIs.Remove(dleteVilladetails);
                _Db.SaveChanges();
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

            //EF Core will done below steps on the basis of ID automatically

            //var villaDetails= _Db.VillaAPIs.FirstOrDefault(y => y.Id == id);
            //if (villaDetails != null) { 
            //    villaDetails.Name = villaDTO.Name;
            //    villaDetails.Occupancy = villaDTO.Occupancy;
            //    villaDetails.Sqft = villaDTO.Sqft;
            //    return NoContent();
            //}
            VillaAPI model = new VillaAPI()
            {
                Name = villaDTO.Name,
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                ImageUrl = villaDTO.ImageUrl,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft,
                Id = villaDTO.Id
            };
            _Db.VillaAPIs.Update(model);
            _Db.SaveChanges();
            return NoContent();

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
                var patchDetails = _Db.VillaAPIs.AsNoTracking().FirstOrDefault(x => x.Id == id);
                //var patchDetails=_Db.VillaAPIs.FirstOrDefault(x=>x.Id == id);
                //On the above line EF Core is tracking the Id of VillaAPIs table, so if we try to update any property from this table
                //and just mark SaveChanges like below the changes will be updated and we don't have to write _DB.VillaAPIs.Update()

                //patchDetails.Name = "XYZ";
                //_Db.SaveChanges()

                VillaDTO modelDTO = new VillaDTO()
                {
                    Name = patchDetails.Name,
                    Amenity = patchDetails.Amenity,
                    Details = patchDetails.Details,
                    ImageUrl = patchDetails.ImageUrl,
                    Occupancy = patchDetails.Occupancy,
                    Rate = patchDetails.Rate,
                    Sqft = patchDetails.Sqft,
                    Id = patchDetails.Id
                };

                if (patchDetails != null) { 
                    patchDTO.ApplyTo(modelDTO, ModelState);

                    VillaAPI modelVilla = new VillaAPI()
                    {
                        Name = modelDTO.Name,
                        Amenity = modelDTO.Amenity,
                        Details = modelDTO.Details,
                        ImageUrl = modelDTO.ImageUrl,
                        Occupancy = modelDTO.Occupancy,
                        Rate = modelDTO.Rate,
                        Sqft = modelDTO.Sqft,
                        Id = modelDTO.Id
                    };

                    _Db.VillaAPIs.Update(modelVilla);
                    //Here also EF Core will get confused to which Id it has to update as it was tracking in line: 168
                    //So to avoid tracking we have to write AsNoTracking() to tell EF Core to not track the entity shown in line:167
                    _Db.SaveChanges();
                    if (!ModelState.IsValid)
                    {
                        return BadRequest();
                    }
                    return NoContent();
                }
            }
            return BadRequest();
        }
    }
}
