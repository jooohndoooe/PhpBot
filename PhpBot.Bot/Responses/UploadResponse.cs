namespace PhpBot.Bot.Responses
{
    public class UploadResponse
    {
        public bool Success { get; set; }
        public string ErrorMesage { get; set; }
        public string SecretKey { get; set; }
        public string SecretKeyParam { get; set; }
    }
}
