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
  public class AddressController : Controller
{
  readonly IAddressService _service = null;
  readonly ILogger<AddressController> _logger = null;
  #region constructor
  public AddressController(IAddressService service, ILogger<AddressController> logger)
  {
    _logger = logger;
    _service = service;
  }
  #endregion
  [HttpGet()]
      [ActionName("ReadAddresses")]
      [ProducesResponseType(200, Type = typeof(ResponsePageDto<IAddress>))]
      [ProducesResponseType(400, Type = typeof(string))]
      public async Task<IActionResult> ReadAddresses(string seeded = "true", string flat="true", string filter=null,string pageNr = "0", string pageSize="10")
    {
      try{
      bool seededArg = bool.Parse(seeded);
      bool flatArg = bool.Parse(flat);
      int pageNrArg = int.Parse(pageNr);
      int pageSizeArg = int.Parse(pageSize);

      if(!string.IsNullOrEmpty(filter) && !Regex.IsMatch(filter, @"^[a-zA-Z0-9åäöÅÄÖ\s-]*$"))
      {
        throw new ArgumentException($"Filter can only contain letters (a-ö), numbers (0-9), and spaces and -.");
      }
      _logger.LogInformation($"{nameof(ReadAddresses)}: {nameof(seededArg)}: {seededArg}, {nameof(flatArg)}: {flatArg}, " +
                            $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}");
      var resp = await _service.ReadAddressesAsync(seededArg, flatArg, filter?.Trim().ToLower(), pageNrArg, pageSizeArg);
      return Ok(resp);
      }
      catch(Exception ex)
      {
        _logger.LogError($"{nameof(ReadAddresses)}: {ex.Message}");
        return BadRequest(ex.Message);
      }
    } 
  [HttpGet()]
  [ActionName("ReadAddress")]
  [ProducesResponseType(200, Type = typeof(IAddress))]
  [ProducesResponseType(400, Type = typeof(string))]
  public async Task<IActionResult> ReadAddress(string id = null, string flat = "false")
  {
    try
    {
      var idArg = Guid.Parse(id);
      var flatArg = bool.Parse(flat);

      _logger.LogInformation($"{nameof(ReadAddress)}: {nameof(idArg)}: {idArg}, {nameof(flatArg)}: {flatArg}");
      
      var address   = await _service.ReadAddressAsync(idArg, flatArg);
      if(address == null)
      {
        throw new ArgumentException($"Address with id {idArg} not found.");
      }
      return Ok(address);
    }
    catch(Exception ex)
    {
      _logger.LogError($"{nameof(ReadAddress)}: {ex.Message}");
      return BadRequest(ex.Message);
    }
  }
}