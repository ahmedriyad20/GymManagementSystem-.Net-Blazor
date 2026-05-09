using GymManagementSystem.EntityFrameworkCore;
using GymManagementSystem.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Services.AddSwaggerGen();

// Register DbContext
builder.Services.AddDbContext<GymManagementSystemDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

#region Register Identity, Repositories, and Services

//REGISTER ApplicationUser and IdentityRole with the DI Container
// Register Identity
builder.Services.AddApplicationIdentity();

// Register repositories and services using extension methods
builder.Services.AddApplicationRepositories();
builder.Services.AddApplicationServices();

#endregion

#region Configure CORS to allow requests from any origin
builder.Services.AddCors(options =>
{
    // Policy 1: Allow everything (for development)
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    // Policy 2: Restrict to specific origin (for production)
    options.AddPolicy("ProductionPolicy", builder =>
    {
        builder.WithOrigins("https://myapp.com", "https://www.myapp.com")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });

    // Policy 3: Read-only access
    options.AddPolicy("ReadOnly", builder =>
    {
        builder.AllowAnyOrigin()
               .WithMethods("GET")
               .AllowAnyHeader();
    });
});
#endregion

#region Configure JSON options to handle enum serialization as strings
builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
#endregion

#region override default authentication middleware to validate token not cookies
builder.Services.AddAuthentication(options =>
{
    //Check JWT token header
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    //Change the default redirect url when [Authorize] attribute activated (instead of /Account/Login => /api/Account/Login)
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    //And this option is for all and any other service that don't have a DefaultScheme
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => //verified key
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;  //If you have a Https certification  

    var jwtSecurityKey = builder.Configuration["JWT:SecurityKey"]
        ?? throw new InvalidOperationException("JWT:SecurityKey is not configured in appsettings.json. Please add a valid security key.");

    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:IssuerIP"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:AudienceIP"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecurityKey))
    };
});
#endregion

#region Swagger Setting to enable adding token
builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ASP.NET 10 Web API",
        Description = "Lion Gym Project"
    });
    // To Enable authorization using Swagger (JWT)
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    });
    swagger.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
