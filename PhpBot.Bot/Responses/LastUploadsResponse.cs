using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhpBot.Bot.Responses
{
    public class LastUploadsResponse
    {
        public List<Upload> Uploads { get; set; }
    }

    public class Upload
    {
        public int UserId { get; set; }
        public string AppName { get; set; }
        public string AppBundle { get; set; }
        public string SFTPHost { get; set; }
        public string SFTPLogin { get; set; }
        public string SFTPFilePath { get; set; }

        public DateTime UploadTime { get; set; }
        public string SecretKey { get; set; }
        public string SecretKeyParam { get; set; }
    }
}
