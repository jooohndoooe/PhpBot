using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace PhpBot.Api.Core.SftpClient;

public class SshNetSftpClient : ISftpClient
{
    public SshNetSftpClient(ILogger<SshNetSftpClient> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    public async Task<SftpUploadResult> Upload(SftpConnectionParameters connection, string filePath, byte[] content)
    {
        try
        {
            _logger.LogInformation("Trying to upload data to {host}/{path} by {login}", connection.Host, filePath, connection.Login);

            using var sftpClient = new Renci.SshNet.SftpClient(connection.Host, connection.Login, connection.Password);
            await sftpClient.ConnectAsync(CancellationToken.None);

            _logger.LogDebug("Connection established");

            using var stream = new MemoryStream(content);

            var completion = new TaskCompletionSource<ulong>();
            sftpClient.UploadFile(stream, filePath, true, size => { completion.SetResult(size); });

            var size = await completion.Task;

            if (size != (ulong) content.Length)
            {
                _logger.LogError("Error uploading file: uploaded size doesn't match actual content size");
                
                return new SftpUploadResult
                       {
                           Success = false, Error = $"Uploaded file size {size} doesn't match provided content size {content.Length}"
                       };
            }

            _logger.LogInformation("Upload completed successfully");
            return new SftpUploadResult {Success = true};
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading to SFTP");
            
            return new SftpUploadResult {Success = false, Error = ex.Message};
        }
    }

    private readonly ILogger _logger;
}