using IzinTakip.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IzinTakip.Business.Abstract
{
    public interface IEmployeeAnnualDetailsService
    {
        Task CreateAsync(EmployeeAnnualDetails employeeAnnualDetails);
        Task UpdateAsync(EmployeeAnnualDetails employee);
        Task DeleteAsync(int empId);
        Task<ICollection<EmployeeAnnualDetails>> GetAllEmployeeAnnualDetailsAsync();
        Task<ICollection<EmployeeAnnualDetails>> GetAllEmployeeAnnualDetailsByIdAsync(int? empId);
        Task<EmployeeAnnualDetails> FindEmployeeAnnualDetailsByIdAsync(int? empId);
        Task<bool> Exist(int id);
    }
}
