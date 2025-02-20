using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhpBot.Api.Data.Entities
{
    public class TelegramUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TelegramUsername { get; set; }
        public int UserId { get; set; }
    }
}
