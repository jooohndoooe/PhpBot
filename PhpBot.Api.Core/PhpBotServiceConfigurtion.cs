using Microsoft.Extensions.DependencyInjection;
using PhpBot.Api.Core.Auth.AccessService;
using PhpBot.Api.Core.Auth.TelegramAuthService;
using PhpBot.Api.Core.SftpClient;
using PhpBot.Api.Core.Uploading.SecretKeyService;
using PhpBot.Api.Core.Uploading.UploadService;
using PhpBot.Api.Core.UserManagement;

namespace PhpBot.Api.Core
{
    public static class PhpBotServiceConfigurtion
    {
        public static void AddPhpBotService(this IServiceCollection services)
        {
            services.AddScoped<IAccessService, AccessService>();
            services.AddScoped<ITelegramAuthService, TelegramAuthService>();
            services.AddScoped<ISftpClient, SshNetSftpClient>();
            services.AddScoped<ISecretKeyService, SecretKeyService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
        }
    }
}
