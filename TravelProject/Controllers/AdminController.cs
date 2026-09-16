using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Runtime.Intrinsics.Arm;
using Newtonsoft.Json;

using Services;
using Configuration;
using Configuration.Options;
namespace TravelProject.Controllers
{
  [ApiController]
  [Route("api/[controller]/[action]")]
  public class AdminController: Controller
  {
        readonly ILogger<AdminController> _logger;
        readonly DbConnectionSetsOptions _dbSetOptions;
        readonly AesEncryptionOptions _aesOptions;
        readonly JwtOptions _jwtOptions;
        readonly VersionOptions _versionOptions;
        readonly IConfiguration _configuration;
        readonly Encryptions _encryptions = null;
        readonly DatabaseConnections _dbConnections = null;
        readonly IAdminService _service;

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

    [HttpGet]
    [ActionName("Seed")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(400, Type = typeof(string))]
    public async Task<IActionResult> Seed(int seedCount)
    {
      try
        { 
          _logger.LogInformation($"{nameof(Seed)}");
          await _service.SeedAsync(seedCount);
          return Ok($"Seeding {seedCount} items completed successfully.");
        }
      catch(Exception ex)
      {
        _logger.LogError($"{nameof(Seed)}: {ex.Message}");
        return BadRequest(ex.Message);
      }
    }

    [HttpGet()]
    [ActionName("ConnectionString")]
    [ProducesResponseType(200, Type = typeof(string))]
    public IActionResult ConnectionString()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("SqlServerDocker");
            _logger.LogInformation($"{nameof(ConnectionString)}:\n{JsonConvert.SerializeObject(connectionString)}");
            return Ok(connectionString);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(ConnectionString)}: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }

    public AdminController(
      ILogger<AdminController> logger,
      IConfiguration configuration,
      IOptions<DbConnectionSetsOptions> dbSetOptions,
      IOptions<AesEncryptionOptions> aesOptions,
      IOptions<JwtOptions> jwtOptions,
      IOptions<VersionOptions> versionOptions,
      Encryptions encryptions,
      DatabaseConnections dbConnections,
      IAdminService service)
    {
      _logger = logger;
      _configuration = configuration;
      _dbSetOptions = dbSetOptions.Value;
      _aesOptions = aesOptions.Value;
      _jwtOptions = jwtOptions.Value;
      _versionOptions = versionOptions.Value;
      _encryptions = encryptions;
      _dbConnections = dbConnections;
      _service = service;
    }
  }
}