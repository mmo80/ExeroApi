using System;

namespace Exero.Api.Models
{
    public abstract class BaseId
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class User : BaseId
    {
        public string Email { get; set; }
        public bool Disabled { get; set; }
    }
}
