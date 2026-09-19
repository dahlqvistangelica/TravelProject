using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

using Models;
using Models.DTO;
using Services;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using Models.Interfaces;

namespace TravelProject.Controllers;

  [ApiController]
  [Route("api/[controller]/[action]")]
  public class AttractionController : Controller
{
  readonly IAttractionService _service = null;
  readonly ILogger<AttractionController> _logger = null;
  #region constructor
  public AttractionController(IAttractionService service, ILogger<AttractionController> logger)
  {
    _logger = logger;
    _service = service;
  }
  #endregion

  [HttpGet()]
  [ActionName("Read")]
  [ProducesResponseType(200, Type = typeof(ResponsePageDto<IAttraction>))]
  [ProducesResponseType(400, Type = typeof(string))]
  public async Task<IActionResult> Read(string seeded = "true", string flat = "true", string filter = null, string pageNr = "0", string pageSize = "10")
  {
    try
    {
      bool seededArg = bool.Parse(seeded);
      bool flatArg = bool.Parse(flat);
      int pageNrArg = int.Parse(pageNr);
      int pageSizeArg = int.Parse(pageSize);

      if(!string.IsNullOrEmpty(filter) && !Regex.IsMatch(filter, @"^[a-zA-Z0-9\s]*$"))
      {
        throw new ArgumentException($"Filter can only contain letters (a-z), numbers (0-9), and spaces.");
      }
      _logger.LogInformation($"{nameof(Read)}: {nameof(seededArg)}: {seededArg}, {nameof(flatArg)}: {flatArg}, " +
                            $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}");
      var resp = await _service.ReadAttractionsAsync(seededArg, flatArg, filter?.Trim().ToLower(), pageNrArg, pageSizeArg);
      return Ok(resp);
    }
    catch(Exception ex)
    {
      _logger.LogError($"{nameof(Read)}: {ex.Message}");
      return BadRequest(ex.Message);
    }
  }
  [HttpGet()]
  [ActionName("ReadAttraction")]
  [ProducesResponseType(200, Type = typeof(IAttraction))]
  [ProducesResponseType(400, Type = typeof(string))]
  public async Task<IActionResult> ReadAttraction(string id = null, string flat = "false")
  {
    try
    {
      var idArg = Guid.Parse(id);
      var flatArg = bool.Parse(flat);

      _logger.LogInformation($"{nameof(ReadAttraction)}: {nameof(idArg)}: {idArg}, {nameof(flatArg)}: {flatArg}");
      
      var attraction   = await _service.ReadAttractionAsync(idArg, flatArg);
      if(attraction == null)
      {
        throw new ArgumentException($"Attraction with id {idArg} not found.");
      }
      return Ok(attraction);
    }
    catch(Exception ex)
    {
      _logger.LogError($"{nameof(ReadAttraction)}: {ex.Message}");
      return BadRequest(ex.Message);
    }
  }

  [HttpDelete()]
  [ActionName("DeleteAttraction")]
  [ProducesResponseType(200, Type = typeof(IAttraction))]
  [ProducesResponseType(400, Type = typeof(string))]
  public async Task<IActionResult> DeleteAttraction(string id)
  {
    try
    {
      var idArg = Guid.Parse(id);

      _logger.LogInformation($"{nameof(DeleteAttraction)}: {nameof(idArg)}: {idArg}");
      
      var attraction = await _service.DeleteAttractionAsync(idArg);
      if(attraction == null)
      {
        throw new ArgumentException($"Attraction with id {idArg} not found.");
      }
      return Ok(attraction);
    }
    catch(Exception ex)
    {
      _logger.LogError($"{nameof(DeleteAttraction)}: {ex.Message}");
      return BadRequest(ex.Message);
    }
  }
  [HttpGet()]
  [ActionName("ReadAttractionDto")]
  [ProducesResponseType(200, Type = typeof(AttractionCuDto))]
  [ProducesResponseType(400, Type = typeof(string))]
  [ProducesResponseType(404, Type = typeof(string))]
  public async Task<IActionResult> ReadAttractionDto(string id = null)
  {
    try
    {
      var idArg = Guid.Parse(id);

      _logger.LogInformation($"{nameof(ReadAttractionDto)}: {nameof(idArg)}: {idArg}");
      
      var attraction   = await _service.ReadAttractionAsync(idArg, true);
      if(attraction == null)
      {
        throw new ArgumentException($"Attraction with id {idArg} not found.");
      }
      return Ok(new ResponseItemDto<AttractionCuDto>()
      {
        #if DEBUG
        ConnectionString = attraction.ConnectionString,
        #endif
        Item = new AttractionCuDto(attraction.Item)
      });
    }
    catch(Exception ex)
    {
      _logger.LogError($"{nameof(ReadAttractionDto)}: {ex.Message}");
      return BadRequest(ex.Message);
    }
  }

  [HttpPut("{id}")]
  [ActionName("UpdateAttraction")]
  [ProducesResponseType(200, Type = typeof(IAttraction))]
  [ProducesResponseType(400, Type = typeof(string))]
  public async Task<IActionResult> UpdateAttraction(string id, [FromBody] AttractionCuDto item)
  {
    try
    {
      item.EnsureValidity();
      
      var idArg = Guid.Parse(id);
      
      _logger.LogInformation($"{nameof(UpdateAttraction)}: {nameof(idArg)}: {idArg}");
      if(item.AttractionId != idArg)
      {
        throw new ArgumentException($"AttractionId in the body ({item.AttractionId}) does not match the id in the URL ({idArg}).");
      }
      
      var attraction = await _service.UpdateAttractionAsync(item);
      _logger.LogInformation($"item {idArg} updated successfully.");
      return Ok(attraction);
    }
    catch(Exception ex)
    {
      _logger.LogError($"{nameof(UpdateAttraction)}: {ex.Message}");
      return BadRequest(ex.Message);
    }
  }
}

