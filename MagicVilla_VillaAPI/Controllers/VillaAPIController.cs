using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly IVillaRepository _Db;

        private readonly IMapper _Mapper;       //For Automapper dependency

        protected APIResponse _APIResponse;

        public VillaAPIController(IVillaRepository db,IMapper mapper)
        {
            //_Logger = logger;
            _Db = db;
            _Mapper = mapper;
            this._APIResponse=new APIResponse();
        }

        //Uncomment if we want to use custom logging

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                IEnumerable<VillaAPI> lstVilla = await _Db.GetAllAsync();
                _APIResponse.Result = _Mapper.Map<List<VillaDTO>>(lstVilla);
                _APIResponse.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.IsSuccess = false;
                _APIResponse.ErrorMessages=new List<string> {ex.ToString()};
            }
            return _APIResponse;

            //_Mapper.Map<Source to which mapping needs to be done>(object to send tosource)
        }
        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id) {
            try
            {
                if (id == 0)
                {
                    //_Logger.LogError("get villa error with Id" + id);
                    return BadRequest();
                }
                var villaDetails = await _Db.GetAsync(u => u.Id == id);
                if (villaDetails == null)
                {
                    return NotFound();
                }
                _APIResponse.Result = _Mapper.Map<VillaDTO>(villaDetails);
                _APIResponse.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_APIResponse);
            }
            
            catch (Exception ex)
            {
                _APIResponse.IsSuccess = false;
                _APIResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _APIResponse;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla(VillaCreateDTO villaCreateDTO)
        {
            try
            {
                //if (!ModelState.IsValid) { 
                //    return BadRequest(ModelState);
                //}

                //To check villa name is not already existing
                var villaNameforDB = await _Db.GetAsync(x => x.Name.ToLower() == villaCreateDTO.Name.ToLower());
                if (villaNameforDB != null)
                {
                    ModelState.AddModelError("CustomError", "Villa already Exists!");
                    //CustomError-> is the key and it should be unique
                    return BadRequest();
                }
                if (villaCreateDTO == null)
                {
                    return BadRequest();
                }

                //Converting VillaDTO object to VillaAPI for inserting in DB

                //VillaAPI model= new VillaAPI()
                //{
                //    Name= villaCreateDTO.Name,
                //    Amenity=villaCreateDTO.Amenity,
                //    Details=villaCreateDTO.Details,
                //    ImageUrl = villaCreateDTO.ImageUrl,
                //    Occupancy = villaCreateDTO.Occupancy,
                //    Rate = villaCreateDTO.Rate,
                //    Sqft = villaCreateDTO.Sqft
                //};

                VillaAPI model = _Mapper.Map<VillaAPI>(villaCreateDTO);

                await _Db.CreateAsync(model);
                _APIResponse.Result = _Mapper.Map<VillaDTO>(model);
                _APIResponse.StatusCode = System.Net.HttpStatusCode.Created;


                return CreatedAtRoute(nameof(GetVilla), new { id = model.Id }, _APIResponse);

                //CreatedAt-> Sends the 201 Created status along with Locationheader pointing to newly created resource
                //Here after the resource is created it is routing to GetVilla Api
            }

            catch (Exception ex)
            {
                _APIResponse.IsSuccess = false;
                _APIResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _APIResponse;

        }
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var dleteVilladetails = await _Db.GetAsync(x => x.Id == id);
                if (dleteVilladetails != null)
                {
                    _Db.RemoveAsync(dleteVilladetails);


                    _APIResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                    _APIResponse.IsSuccess = true;

                    return Ok(_APIResponse);
                }
            }
            
            catch (Exception ex)
            {
                _APIResponse.IsSuccess = false;
                _APIResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _APIResponse;

        }

        //With IActionResult you don't need to explicitly define the return type as we did by using ActionResult also there are some
        //return types that only supports IActionResult like; NoContent
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id,VillaUpdateDTO villaUpdateDTO)
        {
            try
            {
                if (villaUpdateDTO == null && id != villaUpdateDTO.Id)
                {
                    _APIResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    return BadRequest(_APIResponse);
                }

                //EF Core will done below steps on the basis of ID automatically

                //var villaDetails= _Db.VillaAPIs.FirstOrDefault(y => y.Id == id);
                //if (villaDetails != null) { 
                //    villaDetails.Name = villaDTO.Name;
                //    villaDetails.Occupancy = villaDTO.Occupancy;
                //    villaDetails.Sqft = villaDTO.Sqft;
                //    return NoContent();
                //}


                //VillaAPI model = new VillaAPI()
                //{
                //    Name = villaUpdateDTO.Name,
                //    Amenity = villaUpdateDTO.Amenity,
                //    Details = villaUpdateDTO.Details,
                //    ImageUrl = villaUpdateDTO.ImageUrl,
                //    Occupancy = villaUpdateDTO.Occupancy,
                //    Rate = villaUpdateDTO.Rate,
                //    Sqft = villaUpdateDTO.Sqft,
                //    Id = villaUpdateDTO.Id
                //};

                VillaAPI model = _Mapper.Map<VillaAPI>(villaUpdateDTO);

                await _Db.UpdateAsync(model);
                _APIResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                _APIResponse.IsSuccess = true;

                return Ok(_APIResponse);
            }
            catch (Exception ex)
            {
                _APIResponse.IsSuccess = false;
                _APIResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _APIResponse;

        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVilla(int id,JsonPatchDocument<VillaUpdateDTO> patchUpdateDTO)
        {
            //When we are working with PATCH we receive JsonPatchDocument having the object that needs to be updated, we have to define the type of
            //JsonPatchDocument like JsonPatchDocument<VillaDTO>
            if (id != 0 && patchUpdateDTO != null)
            {
                var patchDetails =await _Db.GetAsync(x => x.Id == id,tracked:false);
                //var patchDetails=_Db.VillaAPIs.FirstOrDefault(x=>x.Id == id);
                //On the above line EF Core is tracking the Id of VillaAPIs table, so if we try to update any property from this table
                //and just mark SaveChanges like below the changes will be updated and we don't have to write _DB.VillaAPIs.Update()

                //patchDetails.Name = "XYZ";
                //_Db.SaveChanges()

                //VillaUpdateDTO modelDTO = new VillaUpdateDTO()
                //{
                //    Name = patchDetails.Name,
                //    Amenity = patchDetails.Amenity,
                //    Details = patchDetails.Details,
                //    ImageUrl = patchDetails.ImageUrl,
                //    Occupancy = patchDetails.Occupancy,
                //    Rate = patchDetails.Rate,
                //    Sqft = patchDetails.Sqft,
                //    Id = patchDetails.Id
                //};

                VillaUpdateDTO modelDTO = _Mapper.Map<VillaUpdateDTO>(patchDetails);

                if (patchDetails != null) {
                    patchUpdateDTO.ApplyTo(modelDTO, ModelState);       //ApplyTo->It will send the Json object to DTO and ModelState if there is any error

                    //VillaAPI modelVilla = new VillaAPI()
                    //{
                    //    Name = modelDTO.Name,
                    //    Amenity = modelDTO.Amenity,
                    //    Details = modelDTO.Details,
                    //    ImageUrl = modelDTO.ImageUrl,
                    //    Occupancy = modelDTO.Occupancy,
                    //    Rate = modelDTO.Rate,
                    //    Sqft = modelDTO.Sqft,
                    //    Id = modelDTO.Id
                    //};

                    VillaAPI modelVilla=_Mapper.Map<VillaAPI>(modelDTO);

                    _Db.UpdateAsync(modelVilla);
                    //Here also EF Core will get confused to which Id it has to update as it was tracking in line: 168
                    //So to avoid tracking we have to write AsNoTracking() to tell EF Core to not track the entity shown in line:167
                    
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
