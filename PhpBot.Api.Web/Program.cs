using PhpBot.Api.Core;
using PhpBot.Api.Core.Auth.AccessService;
using PhpBot.Api.Web.Middleware;
using PhpBot.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddJsonConsole(opts => { opts.IncludeScopes = true; });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddDataConfiguration(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddPhpBotService();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUser>(
    provider =>
    {
        var accsessor = provider.GetRequiredService<IHttpContextAccessor>();
        var accessService = provider.GetRequiredService<IAccessService>();
        var httpContext = accsessor.HttpContext;
        var authorizationHeader = httpContext.Request.Headers["X-TelegramUsername"];
        var currentUser = accessService.GetCurrentUser(authorizationHeader).Result;
        return currentUser;
    }
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PhpBotDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHealthChecks("/health");
app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();