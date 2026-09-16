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
}

