using AutoMapper;
using PartsInfoWebApi.core.DTOs;
using PartsInfoWebApi.core.Interfaces;
using PartsInfoWebApi.core.Models;
using PartsInfoWebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartsInfoWebApi.Services
{
    public class SubLogService : Service<SubLog, SubLogDto>, ISubLogService
    {
        private readonly ISubLogRepository _repository;
        private readonly IMapper _mapper;

        public SubLogService(IRepository<SubLog> repository, IMapper mapper, ISubLogRepository subLogRepository)
            : base(repository, mapper)
        {
            _repository = subLogRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubLogDto>> SearchAsync(string searchTerm)
        {
            var entities = await _repository.SearchAsync(searchTerm);
            var dtos = _mapper.Map<IEnumerable<SubLogDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public async Task<SubLogDto> GetFirstAsync()
        {
            var entity = await _repository.GetFirstAsync();
            var dto = _mapper.Map<SubLogDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<SubLogDto> GetLastAsync()
        {
            var entity = await _repository.GetLastAsync();
            var dto = _mapper.Map<SubLogDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<SubLogDto> GetNextAsync(string currentNO)
        {
            var entity = await _repository.GetNextAsync(currentNO);
            var dto = _mapper.Map<SubLogDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<SubLogDto> GetPreviousAsync(string currentNO)
        {
            var entity = await _repository.GetPreviousAsync(currentNO);
            var dto = _mapper.Map<SubLogDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<IEnumerable<SubLogDto>> GetAllSortedAsync()
        {
            var entities = await _repository.GetAllSortedAsync();
            var dtos = _mapper.Map<IEnumerable<SubLogDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public override async Task AddAsync(SubLogDto dto)
        {
            var entity = _mapper.Map<SubLog>(dto);
            await _repository.AddAsync(entity);
        }

        public async Task<(bool success, List<string> changedColumns)> UpdateAsync(SubLogDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.NO);
            if (entity == null)
            {
                return (false, new List<string>());
            }

            var changedColumns = new List<string>();

            if (entity.PART_NO != dto.PART_NO) { changedColumns.Add(nameof(dto.PART_NO)); entity.PART_NO = dto.PART_NO; }
            if (entity.DESC != dto.DESC) { changedColumns.Add(nameof(dto.DESC)); entity.DESC = dto.DESC; }
            if (entity.REQ_BY != dto.REQ_BY) { changedColumns.Add(nameof(dto.REQ_BY)); entity.REQ_BY = dto.REQ_BY; }
            if (entity.REQ_DATE != dto.REQ_DATE) { changedColumns.Add(nameof(dto.REQ_DATE)); entity.REQ_DATE = dto.REQ_DATE; }
            if (entity.ASSIGN != dto.ASSIGN) { changedColumns.Add(nameof(dto.ASSIGN)); entity.ASSIGN = dto.ASSIGN; }
            if (entity.ACCEPT != dto.ACCEPT) { changedColumns.Add(nameof(dto.ACCEPT)); entity.ACCEPT = dto.ACCEPT; }
            if (entity.REJECT != dto.REJECT) { changedColumns.Add(nameof(dto.REJECT)); entity.REJECT = dto.REJECT; }
            if (entity.DATE != dto.DATE) { changedColumns.Add(nameof(dto.DATE)); entity.DATE = dto.DATE; }

            await _repository.UpdateAsync(entity);

            return (true, changedColumns);
        }

        public async Task SetPositionInformation(IEnumerable<SubLogDto> dtos)
        {
            var allLogs = (await _repository.GetAllSortedAsync()).ToList();
            int total = allLogs.Count;

            foreach (var dto in dtos)
            {
                var position = allLogs.FindIndex(c => c.NO == dto.NO) + 1;
                dto.Position = position;
                dto.Total = total;
            }
        }

        public async Task SetPositionInformation(SubLogDto dto)
        {
            var allLogs = (await _repository.GetAllSortedAsync()).ToList();
            int total = allLogs.Count;

            var position = allLogs.FindIndex(c => c.NO == dto.NO) + 1;
            dto.Position = position;
            dto.Total = total;
        }
    }
}
