namespace PhpBot.Api.Core.Uploading.UploadService
{
    public class UploadResult
    {
        public bool Success { get; set; }
        public string ErrorMesage { get; set; }
        public string SecretKey { get; set; }
        public string SecretKeyParam { get; set; }
    }
}
