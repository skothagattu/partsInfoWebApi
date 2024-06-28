using AutoMapper;
using PartsInfoWebApi.Core.DTOs;
using PartsInfoWebApi.Core.Interfaces;
using PartsInfoWebApi.Core.Models;
using PartsInfoWebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartsInfoWebApi.Services
{
    public class EcrLogService : Service<EcrLog, EcrLogDto>, IEcrLogService
    {
        private readonly IEcrLogRepository _repository;
        private readonly IMapper _mapper;

        public EcrLogService(IRepository<EcrLog> repository, IMapper mapper, IEcrLogRepository ecrLogRepository)
            : base(repository, mapper)
        {
            _repository = ecrLogRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EcrLogDto>> SearchAsync(string searchTerm)
        {
            var entities = await _repository.SearchAsync(searchTerm);
            var dtos = _mapper.Map<IEnumerable<EcrLogDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public async Task<EcrLogDto> GetFirstAsync()
        {
            var entity = await _repository.GetFirstAsync();
            var dto = _mapper.Map<EcrLogDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<EcrLogDto> GetLastAsync()
        {
            var entity = await _repository.GetLastAsync();
            var dto = _mapper.Map<EcrLogDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<EcrLogDto> GetNextAsync(int currentNO)
        {
            var entity = await _repository.GetNextAsync(currentNO);
            var dto = _mapper.Map<EcrLogDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<EcrLogDto> GetPreviousAsync(int currentNO)
        {
            var entity = await _repository.GetPreviousAsync(currentNO);
            var dto = _mapper.Map<EcrLogDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<IEnumerable<EcrLogDto>> GetAllSortedAsync()
        {
            var entities = await _repository.GetAllSortedAsync();
            var dtos = _mapper.Map<IEnumerable<EcrLogDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public override async Task AddAsync(EcrLogDto dto)
        {
            var entity = _mapper.Map<EcrLog>(dto);
            await _repository.AddAsync(entity);
        }

        public async Task<(bool success, List<string> changedColumns)> UpdateAsync(EcrLogDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.NO);
            if (entity == null)
            {
                return (false, new List<string>());
            }

            var changedColumns = new List<string>();

            if (entity.DESC != dto.DESC) { changedColumns.Add(nameof(dto.DESC)); entity.DESC = dto.DESC; }
            if (entity.MODEL != dto.MODEL) { changedColumns.Add(nameof(dto.MODEL)); entity.MODEL = dto.MODEL; }
            if (entity.DATE_LOG != dto.DATE_LOG) { changedColumns.Add(nameof(dto.DATE_LOG)); entity.DATE_LOG = dto.DATE_LOG; }
            if (entity.NAME != dto.NAME) { changedColumns.Add(nameof(dto.NAME)); entity.NAME = dto.NAME; }
            if (entity.ECO != dto.ECO) { changedColumns.Add(nameof(dto.ECO)); entity.ECO = dto.ECO; }
            if (entity.DATE_REL != dto.DATE_REL) { changedColumns.Add(nameof(dto.DATE_REL)); entity.DATE_REL = dto.DATE_REL; }

            await _repository.UpdateAsync(entity);

            return (true, changedColumns);
        }

        public async Task SetPositionInformation(IEnumerable<EcrLogDto> dtos)
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

        public async Task SetPositionInformation(EcrLogDto dto)
        {
            var allNumbers = (await _repository.GetAllSortedAsync()).ToList();
            int total = allNumbers.Count;

            var position = allNumbers.FindIndex(c => c.NO == dto.NO) + 1;
            dto.Position = position;
            dto.Total = total;
        }
    }
}
