using IzinTakip.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IzinTakip.Business.Abstract
{
    public interface IEmployeeSpecialLeaveService
    {
        Task CreateAsync(EmployeeSpecialLeave employee);
        Task UpdateAsync(EmployeeSpecialLeave employee);
        Task DeleteAsync(int Id);
        Task<ICollection<EmployeeSpecialLeave>> GetAllSpecialLeavebyEmployeeIdAsync(int empId);
        Task<EmployeeSpecialLeave> FindEmployeeSpecialLeaveByIdAsync(int? Id);
        Task<bool> Exist(int id);
    }
}
