using Configuration;
using Configuration.Extensions;
using Configuration.Options;
using DbContext.Extensions;
using DbRepos;
using Models;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder 
    =>
    {
        builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddEndpointsApiExplorer();

builder.Configuration.AddSecrets(builder.Environment);
builder.Services.AddEncryptions(builder.Configuration);
builder.Services.AddDatabaseConnections(builder.Configuration);
builder.Services.AddVersionInfo();
builder.Services.AddInMemoryLogger();
builder.Services.AddUserBasedDbContext();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title="Angelica Dahlqvist Travel Api",
        #if DEBUG
        Version = "v2.0 DEBUG",
        #else
        Version = "v2.0",
        #endif
        Description = "This is an API created as an assigment during a .Net education"
                    + $"<br>DataSet: {builder.Configuration["DatabaseConnections:UseDataSetWithTag"]}"
            + $"<br>DefaultDataUser: {builder.Configuration["DatabaseConnections:DefaultDataUser"]}"
    });
});

builder.Services.AddInMemoryLogger();
builder.Services.AddScoped<AdminDbRepos>();
builder.Services.AddScoped<AttractionDbRepos>();
builder.Services.AddScoped<ReviewDbRepos>();
builder.Services.AddScoped<UserDbRepos>();
builder.Services.AddScoped<AddressDbRepos>();

builder.Services.AddScoped<IAttractionService, AttractionServiceDb>();
builder.Services.AddScoped<IReviewService, ReviewServiceDb>();
builder.Services.AddScoped<IUserService, UserServiceDb>();
builder.Services.AddScoped<IAdminService, AdminServiceDb>();
builder.Services.AddScoped<IAddressService, AddressServiceDb>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Travel Api v2.0");
});

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();
app.MapControllers();

app.Run();
