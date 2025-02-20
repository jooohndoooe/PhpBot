using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhpBot.Api.Data.Entities
{
    public class Upload
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
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
