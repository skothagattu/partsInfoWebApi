using AutoMapper;
using PartsInfoWebApi.core.DTOs;
using PartsInfoWebApi.core.Models;
using PartsInfoWebApi.Core.DTOs;
using PartsInfoWebApi.Core.Models;
using PartsInfoWebApi.DTOs;

namespace PartsInfoWebApi
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<ThreeLetterCode, ThreeLetterCodeDto>().ReverseMap();
            CreateMap<SubLog, SubLogDto>().ReverseMap();
            CreateMap<D03numbers, D03numbersDto>().ReverseMap();
            CreateMap<DWGnumbers, DWGnumbersDto>().ReverseMap();
        }
    }
}
