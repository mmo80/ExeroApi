using System;

namespace Exero.Api.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public bool Disabled { get; set; }
    }
}
