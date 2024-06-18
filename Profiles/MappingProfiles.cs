using AutoMapper;
using PartsInfoWebApi.DTOs;
using PartsInfoWebApi.Models;

namespace PartsInfoWebApi
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<ThreeLetterCode, ThreeLetterCodeDto>().ReverseMap();
        }
    }
}
