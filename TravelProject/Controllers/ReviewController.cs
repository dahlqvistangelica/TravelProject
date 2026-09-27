using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

using Services;
using Configuration;
using Configuration.Options;
using Models.DTO;
using Models.Interfaces;

namespace TravelProject.Controllers
{
  [ApiController]
  [Route("api/[controller]/[action]")]
  public class ReviewController: Controller
  {
    readonly ILogger<ReviewController> _logger;
    readonly IReviewService _service;
    public ReviewController(ILogger<ReviewController> logger, IReviewService service)
    {
      _logger = logger;
      _service = service;
    }
    
    [HttpGet()]
    [ActionName("ReadReview")]
    [ProducesResponseType(200, Type = typeof(IReview))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(404, Type = typeof(string))]
    public async Task<IActionResult> ReadReview(string id, string flat = "true")
    {
      try
      {
        var idArg = Guid.Parse(id);
        bool flatArg = bool.Parse(flat);
        _logger.LogInformation($"{nameof(ReadReview)}: {nameof(idArg)}: {idArg}, {nameof(flatArg)}: {flatArg}");
        var resp = await _service.ReadReviewAsync(idArg, flatArg);
        if(resp == null)
        {
          throw new ArgumentException($"Review with id {id} not found");
        }
        return Ok(resp);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"{nameof(ReadReview)}: {ex.Message}");
        return BadRequest(ex.Message);
      }
    }

    [HttpPost()]
    [ActionName("CreateReview")]
    [ProducesResponseType(200, Type = typeof(IReview))]
    [ProducesResponseType(400, Type = typeof(string))]
    public async Task<IActionResult> CreateReview([FromBody] ReviewCuDto itemDto)
    {
      try
      {
        itemDto.EnsureValidity();
        _logger.LogInformation($"{nameof(CreateReview)}");
        var resp = await _service.CreateReviewAsync(itemDto);
        _logger.LogInformation($"Review with id {resp.Item.ReviewId} created successfully.");
        return Ok(resp);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"{nameof(CreateReview)}: {ex.Message}");
        return BadRequest(ex.Message);
      }
    }
    
    [HttpDelete("{id}")]
    [ActionName("DeleteReview")]
    [ProducesResponseType(200, Type = typeof(IReview))]
    [ProducesResponseType(400, Type = typeof(string))]
    [ProducesResponseType(404, Type = typeof(string))]
    public async Task<IActionResult> DeleteReview(string id)
    {
      try
      {
        var idArg = Guid.Parse(id);

        _logger.LogInformation($"{nameof(DeleteReview)}: {nameof(idArg)}: {idArg}");
        
        var resp = await _service.DeleteReviewAsync(idArg);
        if(resp == null)
        {
          throw new ArgumentException($"Review with id {id} not found");
        }
        _logger.LogInformation($"Review with id {idArg} deleted successfully.");
        return Ok(resp);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"{nameof(DeleteReview)}: {ex.Message}");
        return BadRequest(ex.Message);
      }
    }
  }
}