using Microsoft.Extensions.Logging;
using DbContext;

namespace DbRepos;

public class CountryDbRepos
{
    private readonly ILogger<CountryDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public CountryDbRepos(ILogger<CountryDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
