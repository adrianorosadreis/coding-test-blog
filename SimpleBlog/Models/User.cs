using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public List<Post> Posts { get; set; } = new List<Post>();
    }
}