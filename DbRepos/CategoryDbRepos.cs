using Microsoft.Extensions.Logging;
using DbContext;

namespace DbRepos;

public class CategoryDbRepos
{
    private readonly ILogger<CategoryDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public CategoryDbRepos(ILogger<CategoryDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
