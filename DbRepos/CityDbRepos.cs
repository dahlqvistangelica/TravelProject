using Microsoft.Extensions.Logging;
using DbContext;

namespace DbRepos;

public class CityDbRepos
{
    private readonly ILogger<CityDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public CityDbRepos(ILogger<CityDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
