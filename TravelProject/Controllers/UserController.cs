using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Runtime.Intrinsics.Arm;
using Newtonsoft.Json;

using Services;
using Models.Interfaces;
using Models.DTO;
using Configuration;
using Configuration.Options;
using System.Text.RegularExpressions;
namespace TravelProject.Controllers
{
  [ApiController]
  [Route("api/[controller]/[action]")]
  public class UserController: Controller
  {
        readonly ILogger<UserController> _logger = null;
        readonly IUserService _service = null;

        public UserController(IUserService service, ILogger<UserController> logger)
        {
          _logger = logger;
          _service = service;
        }
  
      [HttpPost()]
      [ActionName("CreateUser")]
      [ProducesResponseType(200, Type = typeof(IUser))]
      [ProducesResponseType(400, Type = typeof(string))]
      public async Task<IActionResult> CreateUser([FromBody] UserCUdto item)
      {
        try
        {
          var resp = await _service.CreateUserAsync(item);
          return Ok(resp);
        }
        catch(Exception ex)
        {
          _logger.LogError($"{nameof(CreateUser)}: {ex.Message}");
          return BadRequest(ex.Message);
        }
      }
  
      [HttpPut("{id}")]
      [ActionName("UpdateUser")]
      [ProducesResponseType(200, Type = typeof(IUser))]
      [ProducesResponseType(400, Type = typeof(string))]
      public async Task<IActionResult> UpdateUser(string id, [FromBody] UserCUdto item)
      {
        try
        {
          item.EnsureValidity();

          var idArg = Guid.Parse(id);
          _logger.LogInformation($"{nameof(UpdateUser)}: {nameof(idArg)}: {idArg}, {nameof(item)}: {JsonConvert.SerializeObject(item)}");
          if(item.UserId != idArg)
          {
            throw new ArgumentException($"User ID in the URL ({idArg}) does not match the User ID in the request body ({item.UserId}).");
          }
         
          var resp = await _service.UpdateUserAsync(item);
          _logger.LogInformation($"User: {idArg} updated successfully.");
          return Ok(resp);
        }
        catch(Exception ex)
        {
          _logger.LogError($"{nameof(UpdateUser)}: {ex.Message}");
          return BadRequest($"Could not update {nameof(UpdateUser)}: {ex.Message}");
        }
      }
      
      [HttpGet()]
      [ActionName("ReadUserDto")]
      [ProducesResponseType(200, Type = typeof(UserCUdto))]
      [ProducesResponseType(400, Type = typeof(string))]
      [ProducesResponseType(404, Type = typeof(string))]
      public async Task<IActionResult> ReadUserDto(string id = null)
      {
        try
        {
          var idArg = Guid.Parse(id);

          _logger.LogInformation($"{nameof(ReadUserDto)}: {nameof(idArg)}: {idArg}");
          
          var item = await _service.ReadUserAsync(idArg, true);
          if(item == null)
          {
            throw new ArgumentException($"User with id {idArg} not found.");
          }
          return Ok(new ResponseItemDto<UserCUdto>()
          {
            #if DEBUG
            ConnectionString = item.ConnectionString,
            #endif
            Item = new UserCUdto(item.Item)
          });
        }
        catch(Exception ex)
        {
          _logger.LogError($"{nameof(ReadUserDto)}: {ex.Message}");
          return BadRequest(ex.Message);
        }
      }
      [HttpDelete("{id}")]
      [ActionName("DeleteUser")]
      [ProducesResponseType(200, Type = typeof(IUser))]
      [ProducesResponseType(400, Type = typeof(string))]
      public async Task<IActionResult> DeleteUser(string id)
    {
      try
      {
        var idArg = Guid.Parse(id);
        _logger.LogInformation($"{nameof(DeleteUser)}: {nameof(idArg)}: {idArg}");
        var item = await _service.DeleteUserAsync(idArg);
        if(item == null)
        {
          throw new ArgumentException($"User with id {idArg} not found.");
        }
        _logger.LogInformation($"User: {idArg} deleted successfully.");
        return Ok(item);
      }
      catch(Exception ex)
      {
        _logger.LogError($"{nameof(DeleteUser)}: {ex.Message}");
        return BadRequest($"Could not delete {nameof(DeleteUser)}: {ex.Message}");
      }
    }

      [HttpGet()]
      [ActionName("ReadUser")]
      [ProducesResponseType(200, Type = typeof(IUser))]
      [ProducesResponseType(400, Type = typeof(string))]
      [ProducesResponseType(404, Type = typeof(string))]
      public async Task<IActionResult> ReadUser(string id = null, string flat = "false")
      {
        try
        {
          var idArg = Guid.Parse(id);
          var flatArg = bool.Parse(flat);

          _logger.LogInformation($"{nameof(ReadUser)}: {nameof(idArg)}: {idArg}, {nameof(flatArg)}: {flatArg}");
          
          var item = await _service.ReadUserAsync(idArg, flatArg);
          if(item == null)
          {
            throw new ArgumentException($"User with id {idArg} not found.");
          }
          return Ok(item);
        }
        catch(Exception ex)
        {
          _logger.LogError($"{nameof(ReadUser)}: {ex.Message}");
          return BadRequest(ex.Message);
        }
      }

      [HttpGet()]
      [ActionName("ReadUsers")]
      [ProducesResponseType(200, Type = typeof(ResponsePageDto<IUser>))]
      [ProducesResponseType(400, Type = typeof(string))]
      public async Task<IActionResult> ReadUsers(string seeded = "true", string flat="true", string filter=null,string pageNr = "0", string pageSize="10")
    {
      try{
      bool seededArg = bool.Parse(seeded);
      bool flatArg = bool.Parse(flat);
      int pageNrArg = int.Parse(pageNr);
      int pageSizeArg = int.Parse(pageSize);

      if(!string.IsNullOrEmpty(filter) && !Regex.IsMatch(filter, @"^[a-zA-Z0-9\s]*$"))
      {
        throw new ArgumentException($"Filter can only contain letters (a-z), numbers (0-9), and spaces.");
      }
      _logger.LogInformation($"{nameof(ReadUsers)}: {nameof(seededArg)}: {seededArg}, {nameof(flatArg)}: {flatArg}, " +
                            $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}");
      var resp = await _service.ReadUsersAsync(seededArg, flatArg, filter?.Trim().ToLower(), pageNrArg, pageSizeArg);
      return Ok(resp);
      }
      catch(Exception ex)
      {
        _logger.LogError($"{nameof(ReadUsers)}: {ex.Message}");
        return BadRequest(ex.Message);
      }
    } 
}
}