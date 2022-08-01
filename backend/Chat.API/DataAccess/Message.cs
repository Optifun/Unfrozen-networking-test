using System;
using System.Collections.Generic;

namespace Chat.API.DataAccess
{
    public partial class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTime Created { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
