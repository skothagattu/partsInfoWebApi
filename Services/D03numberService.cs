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
    public class D03numberService : Service<D03numbers, D03numbersDto>, ID03numberService
    {
        private readonly ID03numberRepository _repository;
        private readonly IMapper _mapper;

        public D03numberService(IRepository<D03numbers> repository, IMapper mapper, ID03numberRepository d03numberRepository)
            : base(repository, mapper)
        {
            _repository = d03numberRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<D03numbersDto>> SearchAsync(string searchTerm)
        {
            var entities = await _repository.SearchAsync(searchTerm);
            var dtos = _mapper.Map<IEnumerable<D03numbersDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public async Task<D03numbersDto> GetFirstAsync()
        {
            var entity = await _repository.GetFirstAsync();
            var dto = _mapper.Map<D03numbersDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<D03numbersDto> GetLastAsync()
        {
            var entity = await _repository.GetLastAsync();
            var dto = _mapper.Map<D03numbersDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<D03numbersDto> GetNextAsync(string currentID)
        {
            var entity = await _repository.GetNextAsync(currentID);
            var dto = _mapper.Map<D03numbersDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<D03numbersDto> GetPreviousAsync(string currentID)
        {
            var entity = await _repository.GetPreviousAsync(currentID);
            var dto = _mapper.Map<D03numbersDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<IEnumerable<D03numbersDto>> GetAllSortedAsync()
        {
            var entities = await _repository.GetAllSortedAsync();
            var dtos = _mapper.Map<IEnumerable<D03numbersDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public override async Task AddAsync(D03numbersDto dto)
        {
            var entity = _mapper.Map<D03numbers>(dto);
            await _repository.AddAsync(entity);
        }

        public async Task<(bool success, List<string> changedColumns)> UpdateAsync(D03numbersDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.ID);
            if (entity == null)
            {
                return (false, new List<string>());
            }

            var changedColumns = new List<string>();

            if (entity.DESCRIPTION != dto.DESCRIPTION) { changedColumns.Add(nameof(dto.DESCRIPTION)); entity.DESCRIPTION = dto.DESCRIPTION; }
            if (entity.BL_NUMBER != dto.BL_NUMBER) { changedColumns.Add(nameof(dto.BL_NUMBER)); entity.BL_NUMBER = dto.BL_NUMBER; }
            if (entity.PANEL_DWG != dto.PANEL_DWG) { changedColumns.Add(nameof(dto.PANEL_DWG)); entity.PANEL_DWG = dto.PANEL_DWG; }
            if (entity.WHO != dto.WHO) { changedColumns.Add(nameof(dto.WHO)); entity.WHO = dto.WHO; }
            if (entity.START_DATE != dto.START_DATE) { changedColumns.Add(nameof(dto.START_DATE)); entity.START_DATE = dto.START_DATE; }
            if (entity.MODEL != dto.MODEL) { changedColumns.Add(nameof(dto.MODEL)); entity.MODEL = dto.MODEL; }

            await _repository.UpdateAsync(entity);

            return (true, changedColumns);
        }

        public async Task SetPositionInformation(IEnumerable<D03numbersDto> dtos)
        {
            var allNumbers = (await _repository.GetAllSortedAsync()).ToList();
            int total = allNumbers.Count;

            foreach (var dto in dtos)
            {
                var position = allNumbers.FindIndex(c => c.ID == dto.ID) + 1;
                dto.Position = position;
                dto.Total = total;
            }
        }

        public async Task SetPositionInformation(D03numbersDto dto)
        {
            var allNumbers = (await _repository.GetAllSortedAsync()).ToList();
            int total = allNumbers.Count;

            var position = allNumbers.FindIndex(c => c.ID == dto.ID) + 1;
            dto.Position = position;
            dto.Total = total;
        }
    }
}
