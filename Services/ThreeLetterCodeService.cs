using AutoMapper;
using PartsInfoWebApi.core.Models;
using PartsInfoWebApi.DTOs;
using PartsInfoWebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartsInfoWebApi.Services
{
    public class ThreeLetterCodeService : Service<ThreeLetterCode, ThreeLetterCodeDto>, IThreeLetterCodeService
    {
        private readonly IThreeLetterCodeRepository _repository;
        private readonly IMapper _mapper;

        public ThreeLetterCodeService(IRepository<ThreeLetterCode> repository, IMapper mapper, IThreeLetterCodeRepository threeLetterCodeRepository)
            : base(repository, mapper)
        {
            _repository = threeLetterCodeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ThreeLetterCodeDto>> SearchAsync(string searchTerm)
        {
            var entities = await _repository.SearchAsync(searchTerm);
            var dtos = _mapper.Map<IEnumerable<ThreeLetterCodeDto>>(entities);
            await SetPositionInformation(dtos);
            return dtos;
        }

        public async Task<ThreeLetterCodeDto> GetFirstAsync()
        {
            var entity = await _repository.GetFirstAsync();
            var dto = _mapper.Map<ThreeLetterCodeDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<ThreeLetterCodeDto> GetLastAsync()
        {
            var entity = await _repository.GetLastAsync();
            var dto = _mapper.Map<ThreeLetterCodeDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<ThreeLetterCodeDto> GetNextAsync(string currentCode)
        {
            var entity = await _repository.GetNextAsync(currentCode);
            var dto = _mapper.Map<ThreeLetterCodeDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<ThreeLetterCodeDto> GetPreviousAsync(string currentCode)
        {
            var entity = await _repository.GetPreviousAsync(currentCode);
            var dto = _mapper.Map<ThreeLetterCodeDto>(entity);
            await SetPositionInformation(dto);
            return dto;
        }

        public async Task<IEnumerable<ThreeLetterCodeDto>> GetAllSortedAsync()
        {
            var entities = await _repository.GetAllSortedAsync();
            return _mapper.Map<IEnumerable<ThreeLetterCodeDto>>(entities);
        }

        public override async Task AddAsync(ThreeLetterCodeDto dto)
        {
            var entity = _mapper.Map<ThreeLetterCode>(dto);
            await _repository.AddAsync(entity);
        }

        public async Task<(bool success, List<string> changedColumns)> UpdateAsync(ThreeLetterCodeDto dto)
        {
            var existingEntity = await _repository.GetByIdAsync(dto.CODE);
            if (existingEntity == null)
            {
                return (false, null);
            }

            var changedColumns = new List<string>();

            if (existingEntity.TYPE != dto.TYPE) { changedColumns.Add(nameof(dto.TYPE)); existingEntity.TYPE = dto.TYPE; }
            if (existingEntity.COMPANY != dto.COMPANY) { changedColumns.Add(nameof(dto.COMPANY)); existingEntity.COMPANY = dto.COMPANY; }
            if (existingEntity.ADDRESS1 != dto.ADDRESS1) { changedColumns.Add(nameof(dto.ADDRESS1)); existingEntity.ADDRESS1 = dto.ADDRESS1; }
            if (existingEntity.ADDRESS2 != dto.ADDRESS2) { changedColumns.Add(nameof(dto.ADDRESS2)); existingEntity.ADDRESS2 = dto.ADDRESS2; }
            if (existingEntity.CITY_STATE_ZIP != dto.CITY_STATE_ZIP) { changedColumns.Add(nameof(dto.CITY_STATE_ZIP)); existingEntity.CITY_STATE_ZIP = dto.CITY_STATE_ZIP; }
            if (existingEntity.CONTACT1 != dto.CONTACT1) { changedColumns.Add(nameof(dto.CONTACT1)); existingEntity.CONTACT1 = dto.CONTACT1; }
            if (existingEntity.PHONE1 != dto.PHONE1) { changedColumns.Add(nameof(dto.PHONE1)); existingEntity.PHONE1 = dto.PHONE1; }
            if (existingEntity.EXT1 != dto.EXT1) { changedColumns.Add(nameof(dto.EXT1)); existingEntity.EXT1 = dto.EXT1; }
            if (existingEntity.CONTACT2 != dto.CONTACT2) { changedColumns.Add(nameof(dto.CONTACT2)); existingEntity.CONTACT2 = dto.CONTACT2; }
            if (existingEntity.PHONE2 != dto.PHONE2) { changedColumns.Add(nameof(dto.PHONE2)); existingEntity.PHONE2 = dto.PHONE2; }
            if (existingEntity.EXT2 != dto.EXT2) { changedColumns.Add(nameof(dto.EXT2)); existingEntity.EXT2 = dto.EXT2; }
            if (existingEntity.FAX != dto.FAX) { changedColumns.Add(nameof(dto.FAX)); existingEntity.FAX = dto.FAX; }
            if (existingEntity.TERMS != dto.TERMS) { changedColumns.Add(nameof(dto.TERMS)); existingEntity.TERMS = dto.TERMS; }
            if (existingEntity.FOB != dto.FOB) { changedColumns.Add(nameof(dto.FOB)); existingEntity.FOB = dto.FOB; }
            if (existingEntity.NOTES != dto.NOTES) { changedColumns.Add(nameof(dto.NOTES)); existingEntity.NOTES = dto.NOTES; }

            await _repository.UpdateAsync(existingEntity);

            return (true, changedColumns);
        }

        public async Task SetPositionInformation(IEnumerable<ThreeLetterCodeDto> dtos)
        {
            var allCodes = (await _repository.GetAllSortedAsync()).ToList();
            int total = allCodes.Count;

            foreach (var dto in dtos)
            {
                var position = allCodes.FindIndex(c => c.CODE == dto.CODE) + 1;
                dto.Position = position;
                dto.Total = total;
            }
        }

        public async Task SetPositionInformation(ThreeLetterCodeDto dto)
        {
            var allCodes = (await _repository.GetAllSortedAsync()).ToList();
            int total = allCodes.Count;

            var position = allCodes.FindIndex(c => c.CODE == dto.CODE) + 1;
            dto.Position = position;
            dto.Total = total;
        }
    }
}
