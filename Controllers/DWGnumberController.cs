﻿using Microsoft.AspNetCore.Mvc;
using PartsInfoWebApi.core.DTOs;
using PartsInfoWebApi.Core.DTOs;
using PartsInfoWebApi.Core.Interfaces;
using Serilog;

namespace PartsInfoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DWGnumberController : ControllerBase
    {
        private readonly IDWGnumberService _service;

        public DWGnumberController(IDWGnumberService service)
        {
            _service = service;
        }

        [HttpGet("first")]
        public async Task<ActionResult<DWGnumbersDto>> GetFirst()
        {
            var result = await _service.GetFirstAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DWGnumbersDto>>> GetAll()
        {
            var result = await _service.GetAllSortedAsync();
            return Ok(result);
        }

        [HttpGet("{no}")]
        public async Task<ActionResult<DWGnumbersDto>> GetByNo(string no)
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
        public async Task<ActionResult<IEnumerable<DWGnumbersDto>>> Search(string searchTerm)
        {
            var result = await _service.SearchAsync(searchTerm);
            return Ok(result);
        }

        [HttpGet("last")]
        public async Task<ActionResult<DWGnumbersDto>> GetLast()
        {
            var result = await _service.GetLastAsync();
            return Ok(result);
        }

        [HttpGet("next/{currentNO}")]
        public async Task<ActionResult<DWGnumbersDto>> GetNext(string currentNO)
        {
            var result = await _service.GetNextAsync(currentNO);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("previous/{currentNO}")]
        public async Task<ActionResult<DWGnumbersDto>> GetPrevious(string currentNO)
        {
            var result = await _service.GetPreviousAsync(currentNO);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] DWGnumbersDto dto)
        {
            if (string.IsNullOrEmpty(dto.NO) || string.IsNullOrEmpty(dto.DESC))
            {
                return BadRequest("NO and DESC are required fields.");
            }

            var existingD03number = await _service.GetByIdAsync(dto.NO);
            if (existingD03number != null)
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

        [HttpPut("update/{no}")]
        public async Task<ActionResult> Update(string no, DWGnumbersDto dto)
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