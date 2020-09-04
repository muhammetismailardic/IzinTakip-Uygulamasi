using IzinTakip.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IzinTakip.Business.Abstract
{
    public interface IEmployeeService
    {
        Task CreateAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(int empId);
        Task<ICollection<Employee>> GetAllEmployeesAsync();
        Task<Employee> FindEmployeeByIdAsync(int? empId);
        Task<bool> Exist(int id);
    }
}
