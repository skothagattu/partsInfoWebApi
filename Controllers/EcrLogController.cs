using Microsoft.AspNetCore.Mvc;
using PartsInfoWebApi.Core.DTOs;
using PartsInfoWebApi.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartsInfoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EcrLogController : ControllerBase
    {
        private readonly IEcrLogService _service;

        public EcrLogController(IEcrLogService service)
        {
            _service = service;
        }

        [HttpGet("first")]
        public async Task<ActionResult<EcrLogDto>> GetFirst()
        {
            var result = await _service.GetFirstAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EcrLogDto>>> GetAll()
        {
            var result = await _service.GetAllSortedAsync();
            return Ok(result);
        }

        [HttpGet("{no}")]
        public async Task<ActionResult<EcrLogDto>> GetByNo(int no)
        {
            var result = await _service.GetByIdAsync(no);
            if (result == null)
            {
                return NotFound();
            }
            await _service.SetPositionInformation(result);
            return Ok(result);
        }

        [HttpGet("search/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<EcrLogDto>>> Search(string searchTerm)
        {
            var result = await _service.SearchAsync(searchTerm);
            return Ok(result);
        }

        [HttpGet("last")]
        public async Task<ActionResult<EcrLogDto>> GetLast()
        {
            var result = await _service.GetLastAsync();
            return Ok(result);
        }

        [HttpGet("next/{currentNO}")]
        public async Task<ActionResult<EcrLogDto>> GetNext(int currentNO)
        {
            var result = await _service.GetNextAsync(currentNO);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("previous/{currentNO}")]
        public async Task<ActionResult<EcrLogDto>> GetPrevious(int currentNO)
        {
            var result = await _service.GetPreviousAsync(currentNO);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] EcrLogDto dto)
        {
            if (dto.NO == 0 || string.IsNullOrEmpty(dto.DESC))
            {
                return BadRequest("NO and DESC are required fields.");
            }

            var existingEcrLog = await _service.GetByIdAsync(dto.NO);
            if (existingEcrLog != null)
            {
                return Conflict("NO already exists. Please create a unique NO.");
            }

            try
            {
                await _service.AddAsync(dto);
                return Ok("Record successfully created.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update/{no}")]
        public async Task<ActionResult> Update(int no, EcrLogDto dto)
        {
            if (no != dto.NO)
            {
                return BadRequest();
            }

            var result = await _service.UpdateAsync(dto);
            if (!result.success)
            {
                return NotFound("Record not found.");
            }

            return Ok($"Record updated. Changed columns: {string.Join(", ", result.changedColumns)}");
        }

        [HttpDelete("{no}")]
        public async Task<ActionResult> Delete(int no)
        {
            await _service.DeleteAsync(no);
            return NoContent();
        }
    }
}
