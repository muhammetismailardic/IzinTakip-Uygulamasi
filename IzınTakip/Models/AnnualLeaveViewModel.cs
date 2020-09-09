using IzinTakip.Entities.Concrete;
using IzinTakip.UI.Shared;
using System;
using System.Collections.Generic;

namespace IzinTakip.UI
{
    public class AnnualLeaveViewModel
    {
        public int CurrentUsedDate { get; set; }
        public List<HolidayCostum> PublicHolidays { get; set; }
        public EmployeeAnnualDetails EmployeeAnnualDetails { get; set; }
        public EmployeeSpecialLeave EmployeeSpecialLeaves { get; set; }
    }
}