using IzinTakip.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IzinTakip.Entities.Concrete
{
    public class EmployeeAnnualDetails : IEntity
    {
        public int Id { get; set; }
        public int EmployeesId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AnnualRights { get; set; }
        public int Used { get; set; }
        public int LeftDate { get; set; }
        public bool IsOnLeave { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Employee Employees { get; set; }
    }
}
