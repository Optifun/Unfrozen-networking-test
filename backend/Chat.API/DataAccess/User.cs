using System;
using System.Collections.Generic;

namespace Chat.API.DataAccess
{
    public partial class User
    {
        public User()
        {
            Messages = new HashSet<Message>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public long Color { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
