using AutoMapper;
using PartsInfoWebApi.core.DTOs;
using PartsInfoWebApi.Core.DTOs;
using PartsInfoWebApi.Core.Interfaces;
using PartsInfoWebApi.Core.Models;
using PartsInfoWebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartsInfoWebApi.Services
{
    public class DWGnumberService : Service<DWGnumbers, DWGnumbersDto>, IDWGnumberService
    {
        private readonly IDWGnumberRepository _repository;
        private readonly IMapper _mapper;

        public DWGnumberService(IRepository<DWGnumbers> repository, IMapper mapper, IDWGnumberRepository dwgnumberRepository)
            : base(repository, mapper)
        {
            _repository = dwgnumberRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DWGnumbersDto>> SearchAsync(string searchTerm)
        {
            var entities = await _repository.SearchAsync(searchTerm);
            var dtos = _mapper.Map<IEnumerable<DWGnumbersDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public async Task<DWGnumbersDto> GetFirstAsync()
        {
            var entity = await _repository.GetFirstAsync();
            var dto = _mapper.Map<DWGnumbersDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<DWGnumbersDto> GetLastAsync()
        {
            var entity = await _repository.GetLastAsync();
            var dto = _mapper.Map<DWGnumbersDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<DWGnumbersDto> GetNextAsync(string currentNO)
        {
            var entity = await _repository.GetNextAsync(currentNO);
            var dto = _mapper.Map<DWGnumbersDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<DWGnumbersDto> GetPreviousAsync(string currentNO)
        {
            var entity = await _repository.GetPreviousAsync(currentNO);
            var dto = _mapper.Map<DWGnumbersDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<IEnumerable<DWGnumbersDto>> GetAllSortedAsync()
        {
            var entities = await _repository.GetAllSortedAsync();
            var dtos = _mapper.Map<IEnumerable<DWGnumbersDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public override async Task AddAsync(DWGnumbersDto dto)
        {
            var entity = _mapper.Map<DWGnumbers>(dto);
            await _repository.AddAsync(entity);
        }

        public async Task<(bool success, List<string> changedColumns)> UpdateAsync(DWGnumbersDto dto)
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

        public async Task SetPositionInformation(IEnumerable<DWGnumbersDto> dtos)
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

        public async Task SetPositionInformation(DWGnumbersDto dto)
        {
            var allNumbers = (await _repository.GetAllSortedAsync()).ToList();
            int total = allNumbers.Count;

            var position = allNumbers.FindIndex(c => c.NO == dto.NO) + 1;
            dto.Position = position;
            dto.Total = total;
        }
    }
}
