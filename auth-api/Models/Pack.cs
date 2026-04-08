using System;

namespace auth_api.Models
{
    public class Pack
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool Published { get; set; } = false;
    }
}