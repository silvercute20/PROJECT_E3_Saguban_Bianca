using auth_api.Models; 
using System.Collections.Generic;

namespace auth_api.Data
{
    public static class UserStore
    {
        public static List<User> Users { get; set; } = new List<User>();
    }
}