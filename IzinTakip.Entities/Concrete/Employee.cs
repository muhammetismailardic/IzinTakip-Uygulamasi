using IzinTakip.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IzinTakip.Entities.Concrete
{
    public class Employee : IEntity
    {
        public Employee()
        {
            EmployeeAnnualDetails = new HashSet<EmployeeAnnualDetails>();
            EmployeeSpecialLeaves = new HashSet<EmployeeSpecialLeave>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please specify name")]
        [Display(Name = "Employee Name")]
        public string Name { get; set; }
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please specify Surname")]
        [Display(Name = "SurName")]
        public string Surname { get; set; }
        public string Position { get; set; }
        public string PhoneNum { get; set; }
        public string Image { get; set; }

        [Display(Name = "Profile Picture")]
        [NotMapped]
        public IFormFile ProfileImage { get; set; }
        public string YearWorked { get; set; }
        public string DayWorked { get; set; }
        public int TotalAnnualRight { get; set; }


        // This are dymanmic Entiries
        public int YearlyAnnualRightCount { get; set; }
        public int AvailableTotalLeaves { get; set; }
        public int UsedTotalLeaves { get; set; }

        public DateTime RecruitmentDate { get; set; }
        public bool IsOnLeave { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime LeaveStartDate { get; set; }
        public DateTime LeaveEndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; }
        public ICollection<EmployeeAnnualDetails> EmployeeAnnualDetails { get; set; }
        public ICollection<EmployeeSpecialLeave> EmployeeSpecialLeaves { get; set; }
    }
}
