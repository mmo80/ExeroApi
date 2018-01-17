using System;

namespace Exero.Api.Models
{
    public class Category : BaseId
    {
        public string Name { get; set; }
        public string Note { get; set; }
    }
}
