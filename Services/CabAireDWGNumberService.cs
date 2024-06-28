using AutoMapper;
using PartsInfoWebApi.core.Models;
using PartsInfoWebApi.Core.DTOs;
using PartsInfoWebApi.Core.Interfaces;
using PartsInfoWebApi.Core.Models;
using PartsInfoWebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartsInfoWebApi.Services
{
    public class CabAireDWGNumberService : Service<CabAireDWGNumber, CabAireDWGNumberDto>, ICabAireDWGNumberService
    {
        private readonly ICabAireDWGNumberRepository _repository;
        private readonly IMapper _mapper;

        public CabAireDWGNumberService(IRepository<CabAireDWGNumber> repository, IMapper mapper, ICabAireDWGNumberRepository cabAireDWGNumberRepository)
            : base(repository, mapper)
        {
            _repository = cabAireDWGNumberRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CabAireDWGNumberDto>> SearchAsync(string searchTerm)
        {
            var entities = await _repository.SearchAsync(searchTerm);
            var dtos = _mapper.Map<IEnumerable<CabAireDWGNumberDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public async Task<CabAireDWGNumberDto> GetFirstAsync()
        {
            var entity = await _repository.GetFirstAsync();
            var dto = _mapper.Map<CabAireDWGNumberDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<CabAireDWGNumberDto> GetLastAsync()
        {
            var entity = await _repository.GetLastAsync();
            var dto = _mapper.Map<CabAireDWGNumberDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<CabAireDWGNumberDto> GetNextAsync(int currentNO)
        {
            var entity = await _repository.GetNextAsync(currentNO);
            var dto = _mapper.Map<CabAireDWGNumberDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<CabAireDWGNumberDto> GetPreviousAsync(int currentNO)
        {
            var entity = await _repository.GetPreviousAsync(currentNO);
            var dto = _mapper.Map<CabAireDWGNumberDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<IEnumerable<CabAireDWGNumberDto>> GetAllSortedAsync()
        {
            var entities = await _repository.GetAllSortedAsync();
            var dtos = _mapper.Map<IEnumerable<CabAireDWGNumberDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public override async Task AddAsync(CabAireDWGNumberDto dto)
        {
            var entity = _mapper.Map<CabAireDWGNumber>(dto);
            await _repository.AddAsync(entity);
        }

        public async Task<(bool success, List<string> changedColumns)> UpdateAsync(CabAireDWGNumberDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.NO);
            if (entity == null)
            {
                return (false, new List<string>());
            }

            var changedColumns = new List<string>();

            if (entity.PREFIX != dto.PREFIX) { changedColumns.Add(nameof(dto.PREFIX)); entity.PREFIX = dto.PREFIX; }
            if (entity.DESC != dto.DESC) { changedColumns.Add(nameof(dto.DESC)); entity.DESC = dto.DESC; }
            if (entity.MODEL != dto.MODEL) { changedColumns.Add(nameof(dto.MODEL)); entity.MODEL = dto.MODEL; }
            if (entity.ORIG != dto.ORIG) { changedColumns.Add(nameof(dto.ORIG)); entity.ORIG = dto.ORIG; }
            if (entity.DATE != dto.DATE) { changedColumns.Add(nameof(dto.DATE)); entity.DATE = dto.DATE; }

            await _repository.UpdateAsync(entity);

            return (true, changedColumns);
        }


        public async Task SetPositionInformation(IEnumerable<CabAireDWGNumberDto> dtos)
        {
            var allNumbers = (await _repository.GetAllSortedAsync()).ToList();
            int total = allNumbers.Count;

            foreach (var dto in dtos)
            {
                var position = allNumbers.FindIndex(c => c.NO == dto.NO) + 1;
                dto.Position = position;
                dto.Total = total;
            }
        }

        public async Task SetPositionInformation(CabAireDWGNumberDto dto)
        {
            var allNumbers = (await _repository.GetAllSortedAsync()).ToList();
            int total = allNumbers.Count;

            var position = allNumbers.FindIndex(c => c.NO == dto.NO) + 1;
            dto.Position = position;
            dto.Total = total;
        }
    }
}
