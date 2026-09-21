using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using DbModels;
using Models.DTO;
using DbContext.Extensions;

namespace DbContext;

public class MainDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    #if DEBUG
    // remove password from connection string in debug mode
    // this is useful for debugging and logging purposes, but should not be used in production code
    public string dbConnection => System.Text.RegularExpressions.Regex.Replace(
        this.Database.GetConnectionString() ?? "", @"(pwd|password)=[^;]*;?", "",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
#endif
  public DbSet<AttractionDbM> Attractions { get; set; }
  public DbSet<CategoryDbM> Categories {get; set;}
  public DbSet<ReviewDbM> Reviews { get; set; }
  public DbSet<UserDbM> Users { get; set; }
  public DbSet<AddressDbM> Addresses {get; set;}
  public DbSet<CityDbM> Cities {get; set;}
  public DbSet<CountryDbM> Countries {get; set;}
  #region constructors
  public MainDbContext() {}
  public MainDbContext(DbContextOptions options) : base(options) {}
#endregion
#region Db-Views
public DbSet<GstUsrInfoDbDto> InfoDbView {get; set;}
public DbSet<GstUsrInfoAttractionsDto> InfoAttractionsView {get; set;}
public DbSet<GstUsrInfoUsersDto> InfoUsersView {get; set;}
public DbSet<GstUsrInfoCitiesDto> InfoCitiesView {get; set;}
#endregion
  protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GstUsrInfoDbDto>().ToView("vwInfoDb", "gstusr").HasNoKey();
        modelBuilder.Entity<GstUsrInfoAttractionsDto>().ToView("vwInfoAttractions", "gstusr").HasNoKey();
        modelBuilder.Entity<GstUsrInfoUsersDto>().ToView("vwInfoUsers", "gstusr").HasNoKey();
        modelBuilder.Entity<GstUsrInfoCitiesDto>().ToView("vwInfoCities", "gstusr").HasNoKey();
        base.OnModelCreating(modelBuilder);
    }


    #region Per-database sub-contexts used only for EF migrations

    public class SqlServerDbContext : MainDbContext
    {
        public SqlServerDbContext() { }
        public SqlServerDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
                        if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder = optionsBuilder.ConfigureForDesignTime(
                    (options, connectionString) => options.UseSqlServer(connectionString, options => options.EnableRetryOnFailure()));
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HaveColumnType("money");
            configurationBuilder.Properties<string>().HaveColumnType("varchar(200)");
            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
    #endregion
}