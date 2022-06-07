using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Notifications.DAL.Models.Telegram
{
    public class BaseEntity
    {
        [JsonIgnore]
        public long Id { get; set; }
        [JsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

