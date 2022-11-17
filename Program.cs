
using System.Text;
using System.Text.Json.Serialization;
using LearnHttpContext.Helpers.Jwt;
using LearnHttpContext.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(sp => 
    {
        var config = sp.GetService<IConfiguration>()!;
        var dbName = config["MongoDbSettings:Database"];
        var connString = config["MongoDbSettings:ConnectionString"];

        var mongoClient = new MongoClient(connString);
        return mongoClient.GetDatabase(dbName);
    });
    
builder.Services.AddAuthentication(opt => 
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt => 
        {
            opt.RequireHttpsMetadata = false;
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
            };
        });
        
builder.Services.AddSingleton<IJwtGenerator, JwtGenerator>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IRoleService, RoleService>();

builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
