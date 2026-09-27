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
  /// <summary>
  /// Reads a paginated list of attractions from the database based on the provided parameters.
  /// The method allows filtering by seeded status and various search filters, and supports both flat and detailed representations of the attraction data.
  /// </summary>
  /// <param name="seeded"></param>
  /// <param name="flat"></param>
  /// <param name="filterName"></param>
  /// <param name="filterDesc"></param>
  /// <param name="filterPlace"></param>
  /// <param name="filterCat"></param>
  /// <param name="pageNr"></param>
  /// <param name="pageSize"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  [HttpGet()]
  [ActionName("ReadAllAttractions")]
  [ProducesResponseType(200, Type = typeof(ResponsePageDto<IAttraction>))]
  [ProducesResponseType(400, Type = typeof(string))]
  [ProducesResponseType(404, Type = typeof(string))]
  public async Task<IActionResult> ReadAttractions(string seeded = "true", string flat = "true", string filterName = null, string filterDesc = null, string filterPlace = null, string filterCat = null, string pageNr = "0", string pageSize = "10")
  {
    try
    {
      bool seededArg = bool.Parse(seeded);
      bool flatArg = bool.Parse(flat);
      int pageNrArg = int.Parse(pageNr);
      int pageSizeArg = int.Parse(pageSize);

      if(!string.IsNullOrEmpty(filterName) && !Regex.IsMatch(filterName, @"^[a-zA-Z0-9\s]*$"))
      {
        throw new ArgumentException($"Name can only contain letters (a-z), numbers (0-9), and spaces.");
      }
      if(!string.IsNullOrEmpty(filterDesc) && !Regex.IsMatch(filterDesc, @"^[a-zA-Z0-9\s]*$"))
      {
        throw new ArgumentException($"Description can only contain letters (a-z), numbers (0-9), and spaces.");
      }
      if(!string.IsNullOrEmpty(filterPlace) && !Regex.IsMatch(filterPlace, @"^[a-zA-Z0-9\s]*$"))
      {
        throw new ArgumentException($"Place can only contain letters (a-z), numbers (0-9), and spaces.");
      }
      if(!string.IsNullOrEmpty(filterCat) && !Regex.IsMatch(filterCat, @"^[a-zA-Z0-9\s]*$"))
      {
        throw new ArgumentException($"Category can only contain letters (a-z), numbers (0-9), and spaces.");
      }
      _logger.LogInformation($"{nameof(ReadAttractions)}: {nameof(seededArg)}: {seededArg}, {nameof(flatArg)}: {flatArg}, " +
                            $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}");
      var resp = await _service.ReadAttractionsAsync(seededArg, flatArg, filterName, filterDesc, filterPlace, filterCat, pageNrArg, pageSizeArg);
      return Ok(resp);
    }
    catch(Exception ex)
    {
      _logger.LogError($"{nameof(ReadAttractions)}: {ex.Message}");
      return BadRequest(ex.Message);
    }
  }

  /// <summary>
  /// Reads a single attraction from the database based on the provided ID and representation type (flat or detailed).
  /// If the attraction is not found, an ArgumentException is thrown.
  /// </summary>
  /// <param name="id"></param>
  /// <param name="flat"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
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

  /// <summary>
  /// Reads a paginated list of attractions that do not have any associated reviews from the database based on the provided parameters.
  /// The method allows filtering by seeded status and supports both flat and detailed representations of the attraction
  /// </summary>
  /// <param name="seeded"></param>
  /// <param name="flat"></param>
  /// <param name="pageNr"></param>
  /// <param name="pageSize"></param>
  /// <returns></returns>
  [HttpGet()]
  [ActionName("ReadAttractionsWithoutReviews")]
  [ProducesResponseType(200, Type = typeof(ResponsePageDto<IAttraction>))]
  [ProducesResponseType(400, Type = typeof(string))]
  public async Task<IActionResult> ReadAttractionsWithoutReviews(string seeded = "true", string flat = "true", string pageNr = "0", string pageSize = "10")
  {
    try
    {
      var seededArg = bool.Parse(seeded);
      var flatArg = bool.Parse(flat);
      var pageNrArg = int.Parse(pageNr);
      var pageSizeArg = int.Parse(pageSize);

      _logger.LogInformation($"{nameof(ReadAttractionsWithoutReviews)}: {nameof(seededArg)}: {seededArg}, {nameof(flatArg)}: {flatArg}, " +
                            $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}");

      var resp = await _service.AttractionsWithoutReviewsAsync(seededArg, flatArg, pageNrArg, pageSizeArg);
      return Ok(resp);
    }
    catch(Exception ex)
    {
      _logger.LogError($"{nameof(ReadAttractionsWithoutReviews)}: {ex.Message}");
      return BadRequest(ex.Message);
    }
  }

  /// <summary>
  /// Deletes a single attraction from the database based on the provided ID. If the attraction is not found, an ArgumentException is thrown.
  /// The method returns the deleted attraction data.
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
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
  /// <summary>
  /// Reads a single attraction from the database based on the provided ID and returns it as an AttractionCuDto object.
  /// If the attraction is not found, an ArgumentException is thrown.
  /// </summary>
  /// <param name="id"></param>
  /// <param name="flat"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  [HttpGet()]
  [ActionName("ReadAttractionDto")]
  [ProducesResponseType(200, Type = typeof(AttractionCuDto))]
  [ProducesResponseType(400, Type = typeof(string))]
  [ProducesResponseType(404, Type = typeof(string))]
  public async Task<IActionResult> ReadAttractionDto(string id = null, string flat = "false")
  {
    try
    {
      var idArg = Guid.Parse(id);
      var flatArg = bool.Parse(flat);

      _logger.LogInformation($"{nameof(ReadAttractionDto)}: {nameof(idArg)}: {idArg}");
      
      var attraction = await _service.ReadAttractionAsync(idArg, flatArg);
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

  /// <summary>
  /// Updates an existing attraction in the database based on the provided AttractionCuDto.
  /// If the attraction does not exist, an ArgumentException is thrown.
  /// </summary>
  /// <param name="id"></param>
  /// <param name="item"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
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
  /// <summary>
  /// Creates a new attraction in the database based on the provided AttractionCuDto.
  /// If the attraction already exists, an ArgumentException is thrown.
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  [HttpPost()]
  [ActionName("CreateAttraction")]
  [ProducesResponseType(200, Type = typeof(IAttraction))]
  [ProducesResponseType(400, Type = typeof(string))]
  public async Task<IActionResult> CreateAttraction([FromBody] AttractionCuDto item)
  {
    try
    {
      var resp = await _service.CreateAttractionAsync(item);
      return Ok(resp);
    }
    catch(Exception ex)
    {
      _logger.LogError($"{nameof(CreateAttraction)}: {ex.Message}");
          return BadRequest(ex.Message);
    }
  }
}

