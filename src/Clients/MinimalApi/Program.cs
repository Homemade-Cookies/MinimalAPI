using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApi;
using MinimalApi.Contracts;
using MinimalApi.Contracts.DataModel;

AsyncLocal<SessionContext> _sessionContext = new();

AsyncLocal<HttpContext> _httpContext = new();

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MinimalApi", Version = "v1" });
});

builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

builder.Services.AddDbContext<MinimalApiContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("MinimalApiContext"))
        );

var requireConfirmedAccount = builder.Configuration.GetValue<bool>("RequireConfirmedAccount");
builder.Services.AddIdentityCore<UserAccount>(options => options.SignIn.RequireConfirmedAccount = requireConfirmedAccount)
                .AddEntityFrameworkStores<MinimalApiContext>()
                .AddPasswordValidator<PasswordValidator<UserAccount>>()
                .AddApiEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MinimalApi v1"));
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.Use((context, next) =>
{
    _sessionContext.Value = new SessionContext() { SessionId = Guid.NewGuid() };
    _httpContext.Value = context;
    return next(context);
});

app.MapIdentityApi<UserAccount>();

// Validate user is authenticated
app.MapGet("/Who", (ClaimsPrincipal user) => $"Hello {user.Identity!.Name}")
    .RequireAuthorization();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<MinimalApiContext>();
dbContext.Database.EnsureCreated();

app.Run();