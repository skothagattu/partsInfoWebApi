using Microsoft.AspNetCore.Mvc;
using PartsInfoWebApi.Interfaces;
using PartsInfoWebApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartsInfoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThreeLetterCodeController : ControllerBase
    {
        private readonly IThreeLetterCodeService _service;

        public ThreeLetterCodeController(IThreeLetterCodeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThreeLetterCodeDto>>> GetAll()
        {
            var result = await _service.GetAllSortedAsync();
            return Ok(result);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<ThreeLetterCodeDto>> GetByCode(string code)
        {
            var result = await _service.GetByIdAsync(code);
            if (result == null)
            {
                return NotFound();
            }
            await _service.SetPositionInformation(result); // Include position information
            return Ok(result);
        }

        [HttpGet("search/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<ThreeLetterCodeDto>>> Search(string searchTerm)
        {
            var result = await _service.SearchAsync(searchTerm);
            return Ok(result);
        }

        [HttpGet("first")]
        public async Task<ActionResult<ThreeLetterCodeDto>> GetFirst()
        {
            var result = await _service.GetFirstAsync();
            return Ok(result);
        }

        [HttpGet("last")]
        public async Task<ActionResult<ThreeLetterCodeDto>> GetLast()
        {
            var result = await _service.GetLastAsync();
            return Ok(result);
        }

        [HttpGet("next/{currentCode}")]
        public async Task<ActionResult<ThreeLetterCodeDto>> GetNext(string currentCode)
        {
            var result = await _service.GetNextAsync(currentCode);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("previous/{currentCode}")]
        public async Task<ActionResult<ThreeLetterCodeDto>> GetPrevious(string currentCode)
        {
            var result = await _service.GetPreviousAsync(currentCode);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] ThreeLetterCodeDto dto)
        {
            if (string.IsNullOrEmpty(dto.CODE) || string.IsNullOrEmpty(dto.TYPE) || string.IsNullOrEmpty(dto.COMPANY))
            {
                return BadRequest("Code, Type, and Company are required fields.");
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

        [HttpPut("{code}")]
        public async Task<ActionResult> Update(string code, ThreeLetterCodeDto dto)
        {
            if (code != dto.CODE)
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

        [HttpDelete("{code}")]
        public async Task<ActionResult> Delete(string code)
        {
            await _service.DeleteAsync(code);
            return NoContent();
        }
    }
}
