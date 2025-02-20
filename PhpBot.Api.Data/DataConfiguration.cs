using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhpBot.Api.Data.Auth;
using PhpBot.Api.Data.Uploading;
using PhpBot.Api.Data.UserManagement;

namespace PhpBot.Api.Data
{
    public static class DataConfiguration
    {
        public static void AddDataConfiguration(this IServiceCollection services, string connectionString)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUploadRepository, UploadRepository>();
            services.AddScoped<IUserManagementRepository, UserManagementRepository>();

            services.AddDbContextPool<PhpBotDbContext>(options => options.UseNpgsql(connectionString));
        }
    }
}