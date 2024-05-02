﻿using Microsoft.AspNetCore.Identity;

namespace DoAnCoSo2.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string AvatarUrl { get; set; } = null!;
        public ICollection<Blog> Blogs { get; set; }
    }
}
