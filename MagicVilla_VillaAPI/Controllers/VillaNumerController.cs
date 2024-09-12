using AutoMapper;
using Azure;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaNumerController : Controller
    {
        private readonly IVillaNumberRepository _Db;
        private readonly IVillaRepository _villaRepository;
        private IMapper _Mapper;
        protected APIResponse _APIResponse;

        public VillaNumerController(IVillaNumberRepository villaNumberRepository, IMapper mapper, IVillaRepository villaRepository)
        {
            _Db = villaNumberRepository;
            _Mapper = mapper;
            _APIResponse = new APIResponse();
            this._villaRepository = villaRepository;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber()
        {
            try
            {
                IEnumerable<VillaNumber> lstVillaNumber = await _Db.GetAllAsync();
                _APIResponse.Result = _Mapper.Map<List<VillaNumberDTO>>(lstVillaNumber);
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
        [HttpGet("{villaNo:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillabyNumber(int villaNo)
        {
            try
            {
                if (villaNo == 0)
                {
                    _APIResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    return BadRequest(_APIResponse);
                }
                var villanumberDB = await _Db.GetAsync(x => x.VillaNo == villaNo);
                if (villanumberDB == null) {
                    _APIResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return NotFound(_APIResponse);
                }
                _APIResponse.Result=_Mapper.Map<VillaNumberDTO>(villanumberDB);
                _APIResponse.StatusCode=System.Net.HttpStatusCode.OK;
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber(VillaNumberCreateDTO villaNumberCreateDTO)
        {
            try
            {
                if(villaNumberCreateDTO!=null)
                {
                    var villaNumberDetails =await _Db.GetAsync(x => x.VillaNo == villaNumberCreateDTO.VillaNo);
                    if (villaNumberDetails == null)
                    {
                        if(await _villaRepository.GetAsync(x=>x.Id==villaNumberCreateDTO.VillaId)!=null)
                        {
                            VillaNumber villaNumber = _Mapper.Map<VillaNumber>(villaNumberCreateDTO);
                            await _Db.CreateAsync(villaNumber);
                            _APIResponse.Result = _Mapper.Map<VillaNumberDTO>(villaNumber);
                            _APIResponse.StatusCode = System.Net.HttpStatusCode.Created;
                            return CreatedAtRoute(nameof(GetVillabyNumber), new { id = villaNumber.VillaNo }, villaNumber);
                        }
                        else
                        {
                            ModelState.AddModelError("CustomError", "Villa ID is invalid");
                            return BadRequest(ModelState);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("CustomError", "Villa Number already exists");
                        return BadRequest(ModelState);
                    }
                }
                
            }
            catch (Exception ex)
            {
                _APIResponse.IsSuccess = false;
                _APIResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _APIResponse;
        }
        [HttpDelete("{villaNumber:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> DeleteVillabyNumber(int villaNumber)
        {
            try
            {
                if(villaNumber == 0)
                {
                    _APIResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    return BadRequest(_APIResponse);
                }
                var villanumberDetails =await _Db.GetAsync(x => x.VillaNo == villaNumber);
                if(villanumberDetails!=null)
                {
                    await _Db.RemoveAsync(villanumberDetails);

                    
                    _APIResponse.StatusCode=System.Net.HttpStatusCode.NoContent;
                    _APIResponse.IsSuccess=true;
                    return Ok(_APIResponse);
                }
            }
            catch(Exception ex) 
            {
                _APIResponse.IsSuccess=false;
                _APIResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _APIResponse;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo,[FromBody] VillaNumberUpdateDTO villaNumberUpdateDTO)
        {
            try
            {
                if(villaNumberUpdateDTO == null)
                {
                    _APIResponse.StatusCode=System.Net.HttpStatusCode.BadRequest;
                    return BadRequest(_APIResponse);
                }
                if (await _villaRepository.GetAsync(x => x.Id == villaNumberUpdateDTO.VillaId) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa ID is invalid");
                    return BadRequest(ModelState);
                }
                    VillaNumber villaNumber = _Mapper.Map<VillaNumber>(villaNumberUpdateDTO);
                await _Db.UpdateAsync(villaNumber);
                _APIResponse.StatusCode = System.Net.HttpStatusCode.NoContent;
                _APIResponse.IsSuccess=true;
                return Ok(_APIResponse);
            }
            catch (Exception ex) { 
                _APIResponse.IsSuccess = false;
                _APIResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _APIResponse;
        }

        [HttpPatch("{villaNo:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVillaNumber(int villaNo,JsonPatchDocument<VillaNumberUpdateDTO> patchVillaNumber)
        {
            try
            {
                if(patchVillaNumber != null && villaNo!=0)
                {
                    var villaNumberPatchDetails =await _Db.GetAsync(x => x.VillaNo == villaNo,tracked:false);
                    VillaNumberUpdateDTO numberUpdateDTO = _Mapper.Map<VillaNumberUpdateDTO>(villaNumberPatchDetails);


                    if (villaNumberPatchDetails != null)
                        patchVillaNumber.ApplyTo(numberUpdateDTO,ModelState);

                    VillaNumber modelVillaNumber=_Mapper.Map<VillaNumber>(numberUpdateDTO);

                    await _Db.UpdateAsync(modelVillaNumber);

                    if(!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _APIResponse.IsSuccess = false;
                _APIResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return BadRequest();
        }
    }
}
