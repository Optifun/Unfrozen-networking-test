using System;
using System.Collections.Generic;

namespace Chat.API.DataAccess
{
    public partial class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public long Color { get; set; }
    }
}
