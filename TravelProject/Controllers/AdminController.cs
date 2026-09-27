using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Runtime.Intrinsics.Arm;
using Newtonsoft.Json;

using Services;
using Configuration;
using Configuration.Options;
using Models.DTO;
namespace TravelProject.Controllers
{
  [ApiController]
  [Route("api/[controller]/[action]")]
  public class AdminController: Controller
  {
        readonly ILogger<AdminController> _logger;
        readonly VersionOptions _versionOptions;
        readonly DatabaseConnections _dbConnections = null;
        readonly IAdminService _service;
        
    /// <summary>
    /// Retrieves the version information of the application, including the current version and any relevant metadata.
    /// The method returns a VersionOptions object encapsulated in an IActionResult.
    /// </summary>
    [HttpGet]
    [ActionName("Environment")]
    [ProducesResponseType(200, Type = typeof(DatabaseConnections.SetupInformation))]
    public IActionResult Environment()
    {
      try
      {
        var info = _dbConnections.SetupInfo;
        _logger.LogInformation($"{nameof(Environment)}:\n{JsonConvert.SerializeObject(info)}");
        return Ok(info);
      }
      catch(Exception ex)
      {
        _logger.LogError(ex, "Error retrieving environment information");
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Retrieves the version information of the application, including the current version and any relevant metadata.
    /// The method returns a VersionOptions object encapsulated in an IActionResult.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ActionName("Version")]
    [ProducesResponseType(typeof(VersionOptions), 200)]
    public IActionResult Version()
    {
      try
      {
        return Ok(_versionOptions);
      }
      catch(Exception ex)
      {
        _logger.LogError(ex, "Error retrieving version information");
        return BadRequest(ex.Message);
      }
    }
    /// <summary>
    /// Removes all seeded data from the database and then reseeds it with new data generated from a seed source file.
    /// The method ensures that the number of attractions does not exceed the number of available addresses to avoid duplicates. It returns a summary of the database state after reseeding.
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    [ActionName("RobustSeeding")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(400, Type = typeof(string))]
    public async Task<IActionResult> RobustSeeding()
    {
      try
        { 
          _logger.LogInformation($"{nameof(RobustSeeding)}");
          var result = await _service.RobustSeedingAsync();
          return Ok(result);
        }
      catch(Exception ex)
      {
        _logger.LogError(ex, nameof(RobustSeeding));
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Removes all seeded data from the database.
    /// </summary>
    /// <param name="seeded"></param>
    /// <returns></returns>
    [HttpGet()]
    [ActionName("RemoveSeed")]
    [ProducesResponseType(200, Type = typeof(GstUsrInfoAllDto))]
    [ProducesResponseType(400, Type = typeof(string))]  
    public async Task<IActionResult> RemoveSeed(string seeded="true")
    {
      try
        {
          bool seededArg = bool.Parse(seeded); 
          _logger.LogInformation($"{nameof(RemoveSeed)}: {nameof(seededArg)}: {seededArg}");
          var result = await _service.RemoveSeedAsync(seededArg);
          return Ok(result);
        }
      catch(Exception ex)
      {
        _logger.LogError($"{nameof(RemoveSeed)}: {ex.Message}");
        return BadRequest(ex.Message);
      }
    }
    /// <summary>
    /// Retrieves comprehensive database information, including details about attractions, users, cities, and countries.
    /// The method returns a GstUsrInfoAllDto object encapsulated in a ResponseItemDto.
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    [ActionName("DbInfo")]
    [ProducesResponseType(200, Type = typeof(GstUsrInfoAllDto))]
    [ProducesResponseType(400, Type = typeof(string))]
    public async Task<IActionResult> DbInfo()
    {
      try
        {
          _logger.LogInformation($"{nameof(DbInfo)}");
          var result = await _service.DbInfoAsync();
          return Ok(result);
        }
      catch(Exception ex)
      {
        _logger.LogError($"{nameof(DbInfo)}: {ex.Message}");
        return BadRequest(ex.Message);
      }
    }


    public AdminController(
      ILogger<AdminController> logger,
      IOptions<VersionOptions> versionOptions,
      DatabaseConnections dbConnections,
      IAdminService service)
    {
      _logger = logger;
      _versionOptions = versionOptions.Value;
      _dbConnections = dbConnections;
      _service = service;
    }
  }
}