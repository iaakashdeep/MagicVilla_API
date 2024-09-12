using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI.Mapping
{
    public class MappingConfig:Profile
    {
        /// <summary>
        /// Profile-> this class is the base which provides mapping features from automapper
        /// </summary>
        public MappingConfig()
        {
            CreateMap<VillaAPI,VillaDTO>();
            CreateMap<VillaDTO,VillaAPI>();
            //CreateMap<Source, target> -> This method will automatically map the properties if the name of those properties are same in
            ////both source and target classes like; Details in VillaDTO map to Details in VillaAPI and so on
            /////We have written 2 times because we have to map VillaApi to VillaDTO and vice versa

            CreateMap<VillaAPI,VillaCreateDTO>().ReverseMap();
            CreateMap<VillaAPI,VillaUpdateDTO>().ReverseMap();

            //ReverseMap-> this function helps to map the properties in 2 ways instead of writing again as we have done in line; 12
            //we can make use of this function.

            //Mapper has some other features as well in which we can map the properties whose name are not matching in source and target


            CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberUpdateDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberCreateDTO>().ReverseMap();
        }
    }
}
