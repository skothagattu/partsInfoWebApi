using AutoMapper;
using PartsInfoWebApi.Core.DTOs;
using PartsInfoWebApi.Core.Interfaces;
using PartsInfoWebApi.Core.Models;
using PartsInfoWebApi.Infrastructure.Repositories;
using PartsInfoWebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartsInfoWebApi.Services
{
    public class EcoLogService : Service<EcoLog, EcoLogDto>, IEcoLogService
    {
        private readonly IEcoLogRepository _repository;
        private readonly IMapper _mapper;

        public EcoLogService(IRepository<EcoLog> repository, IMapper mapper, IEcoLogRepository ecologRepository)
            : base(repository, mapper)
        {
            _repository = ecologRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EcoLogDto>> SearchAsync(string searchTerm)
        {
            var entities = await _repository.SearchAsync(searchTerm);
            var dtos = _mapper.Map<IEnumerable<EcoLogDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public async Task<EcoLogDto> GetFirstAsync()
        {
            var entity = await _repository.GetFirstAsync();
            var dto = _mapper.Map<EcoLogDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<EcoLogDto> GetLastAsync()
        {
            var entity = await _repository.GetLastAsync();
            var dto = _mapper.Map<EcoLogDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<EcoLogDto> GetNextAsync(int currentNO)
        {
            var entity = await _repository.GetNextAsync(currentNO);
            var dto = _mapper.Map<EcoLogDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<EcoLogDto> GetPreviousAsync(int currentNO)
        {
            var entity = await _repository.GetPreviousAsync(currentNO);
            var dto = _mapper.Map<EcoLogDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<IEnumerable<EcoLogDto>> GetAllSortedAsync()
        {
            var entities = await _repository.GetAllSortedAsync();
            var dtos = _mapper.Map<IEnumerable<EcoLogDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public override async Task AddAsync(EcoLogDto dto)
        {
            var entity = _mapper.Map<EcoLog>(dto);
            await _repository.AddAsync(entity);
        }

        public async Task<(bool success, List<string> changedColumns)> UpdateAsync(EcoLogDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.NO);
            if (entity == null)
            {
                return (false, new List<string>());
            }

            var changedColumns = new List<string>();

            if (entity.DESC != dto.DESC) { changedColumns.Add(nameof(dto.DESC)); entity.DESC = dto.DESC; }
            if (entity.MODEL != dto.MODEL) { changedColumns.Add(nameof(dto.MODEL)); entity.MODEL = dto.MODEL; }
            if (entity.ECR != dto.ECR) { changedColumns.Add(nameof(dto.ECR)); entity.ECR = dto.ECR; }
            if (entity.DATE_LOG != dto.DATE_LOG) { changedColumns.Add(nameof(dto.DATE_LOG)); entity.DATE_LOG = dto.DATE_LOG; }
            if (entity.NAME != dto.NAME) { changedColumns.Add(nameof(dto.NAME)); entity.NAME = dto.NAME; }
            if (entity.DATE_REL != dto.DATE_REL) { changedColumns.Add(nameof(dto.DATE_REL)); entity.DATE_REL = dto.DATE_REL; }

            await _repository.UpdateAsync(entity);

            return (true, changedColumns);
        }

        public async Task SetPositionInformation(IEnumerable<EcoLogDto> dtos)
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

        public async Task SetPositionInformation(EcoLogDto dto)
        {
            var allNumbers = (await _repository.GetAllSortedAsync()).ToList();
            int total = allNumbers.Count;

            var position = allNumbers.FindIndex(c => c.NO == dto.NO) + 1;
            dto.Position = position;
            dto.Total = total;
        }
    }
}
