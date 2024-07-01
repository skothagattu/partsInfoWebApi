using Microsoft.AspNetCore.Mvc;
using PartsInfoWebApi.Core.DTOs;
using PartsInfoWebApi.Core.Interfaces;
using Serilog;

namespace PartsInfoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class D03numberController : ControllerBase
    {
        private readonly ID03numberService _service;

        public D03numberController(ID03numberService service)
        {
            _service = service;
        }

        [HttpGet("first")]
        public async Task<ActionResult<D03numbersDto>> GetFirst()
        {
            var result = await _service.GetFirstAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<D03numbersDto>>> GetAll()
        {
            var result = await _service.GetAllSortedAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<D03numbersDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            await _service.SetPositionInformation(result);
            return Ok(result);
        }

        [HttpGet("search/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<D03numbersDto>>> Search(string searchTerm)
        {
            var result = await _service.SearchAsync(searchTerm);
            return Ok(result);
        }

        [HttpGet("last")]
        public async Task<ActionResult<D03numbersDto>> GetLast()
        {
            var result = await _service.GetLastAsync();
            return Ok(result);
        }

        [HttpGet("next/{currentID}")]
        public async Task<ActionResult<D03numbersDto>> GetNext(int currentID)
        {
            var result = await _service.GetNextAsync(currentID);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("previous/{currentID}")]
        public async Task<ActionResult<D03numbersDto>> GetPrevious(int currentID)
        {
            var result = await _service.GetPreviousAsync(currentID);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPost("createOrUpdate")]
        public async Task<ActionResult> CreateOrUpdate([FromBody] D03numbersDto dto)
        {
            if (dto.ID == 0 || string.IsNullOrEmpty(dto.DESCRIPTION))
            {
                return BadRequest("ID and DESCRIPTION are required fields.");
            }

            try
            {
                var (success, _, error) = await _service.AddOrUpdateAsync(dto);
                if (!success)
                {
                    return Conflict(error);
                }
                return Ok("Record successfully created or updated.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] D03numbersDto dto)
        {
            if (dto.ID == 0 || string.IsNullOrEmpty(dto.DESCRIPTION))
            {
                return BadRequest("ID and DESCRIPTION are required fields.");
            }

            var existingD03number = await _service.GetByIdAsync(dto.ID);
            if (existingD03number != null)
            {
                return Conflict("ID already exists. Please create a unique ID.");
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

        [HttpPut("update/{id}")]
        public async Task<ActionResult> Update(int id, D03numbersDto dto)
        {
            if (id != dto.ID)
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
