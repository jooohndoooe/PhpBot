namespace PhpBot.Api.Core.SftpClient;

public interface ISftpClient
{
    Task<SftpUploadResult> Upload(SftpConnectionParameters connection, string filePath, byte[] content);
}