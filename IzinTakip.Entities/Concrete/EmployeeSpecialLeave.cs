using IzinTakip.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IzinTakip.Entities.Concrete
{
    public class EmployeeSpecialLeave : IEntity
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Text { get; set; }
        public int Count { get; set; }
        public string Hours { get; set; } = "-";
        public bool IsOnLeave { get; set; } = false;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Employee Employee { get; set; }
    }
}
