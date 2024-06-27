using Microsoft.AspNetCore.Mvc;
using PartsInfoWebApi.core.DTOs;
using PartsInfoWebApi.core.Interfaces;
using Serilog;

namespace PartsInfoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubLogController : ControllerBase
    {
        private readonly ISubLogService _service;

        public SubLogController(ISubLogService service)
        {
            _service = service;
        }

        [HttpGet("first")]
        public async Task<ActionResult<SubLogDto>> GetFirst()
        {
            var result = await _service.GetFirstAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubLogDto>>> GetAll()
        {
            var result = await _service.GetAllSortedAsync();
            return Ok(result);
        }

        [HttpGet("{no}")]
        public async Task<ActionResult<SubLogDto>> GetByNo(string no)
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
        public async Task<ActionResult<IEnumerable<SubLogDto>>> Search(string searchTerm)
        {
            var result = await _service.SearchAsync(searchTerm);
            return Ok(result);
        }

        [HttpGet("last")]
        public async Task<ActionResult<SubLogDto>> GetLast()
        {
            var result = await _service.GetLastAsync();
            return Ok(result);
        }

        [HttpGet("next/{currentNO}")]
        public async Task<ActionResult<SubLogDto>> GetNext(string currentNO)
        {
            var result = await _service.GetNextAsync(currentNO);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("previous/{currentNO}")]
        public async Task<ActionResult<SubLogDto>> GetPrevious(string currentNO)
        {
            var result = await _service.GetPreviousAsync(currentNO);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] SubLogDto dto)
        {
            if (string.IsNullOrEmpty(dto.NO) || string.IsNullOrEmpty(dto.PART_NO) || string.IsNullOrEmpty(dto.DESC))
            {
                return BadRequest("NO, PART NO, and DESC are required fields.");
            }

            var existingSubLog = await _service.GetByIdAsync(dto.NO);
            if (existingSubLog != null)
            {
                return Conflict("NO already exists. Please create a unique NO.");
            }

            try
            {
                await _service.AddAsync(dto);
                return Ok("Record successfully created.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("Update/{no}")]
        public async Task<ActionResult> Update(string no, SubLogDto dto)
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
        public async Task<ActionResult> Delete(string no)
        {
            await _service.DeleteAsync(no);
            return NoContent();
        }
    }

}
