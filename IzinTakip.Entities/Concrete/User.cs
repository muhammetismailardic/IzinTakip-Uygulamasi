using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IzinTakip.Entities.Concrete
{
    public class User : IdentityUser
    {
        public User()
        {
            Employees = new HashSet<Employee>();
        }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Image { get; set; }
        public string Biography { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [NotMapped]
        public string Password { get; set; }
        [NotMapped]
        public bool RememberMe { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
