using System;
using System.Collections.Generic;

namespace Chat.API.DataAccess
{
    public partial class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public Guid UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
