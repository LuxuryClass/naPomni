using System.ComponentModel.DataAnnotations;

namespace Reminders.Core.Models
{
    public class TelegramBotState
    {
        [Key]
        public int Id { get; set; }
        public long? UpdatesOffset { get; set; }
    }
}
