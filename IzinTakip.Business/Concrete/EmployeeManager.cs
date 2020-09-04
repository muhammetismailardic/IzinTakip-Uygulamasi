using IzinTakip.Business.Abstract;
using IzinTakip.DataAccess.Abstract;
using IzinTakip.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IzinTakip.Business.Concrete
{
    public class EmployeeManager : IEmployeeService
    {

        private IEmployeeDal _employeeDal;
        public EmployeeManager(IEmployeeDal employeeDal)
        {
            _employeeDal = employeeDal;
        }

        public async Task CreateAsync(Employee employee)
        {
            try
            {
                await _employeeDal.CreateAsync(employee);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task DeleteAsync(int empId)
        {
            try
            {
                await _employeeDal.DeleteAsync(new Employee { Id = empId });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> Exist(int id)
        {
            var result = await _employeeDal.GetWithInludesAsync(x => x.Id == id);

            if (result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<Employee> FindEmployeeByIdAsync(int? empId)
        {
            try
            {
                return await _employeeDal.GetWithInludesAsync(id => id.Id == empId, c => c.Include(con => con.User));
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ICollection<Employee>> GetAllEmployeesAsync()
        {
            try
            {
                return await _employeeDal.GetListWithIncludesAsync
                            (null,
                             c => c.Include(con => con.User),
                             c => c.Include(con => con.EmployeeAnnualDetails),
                             c => c.Include(con => con.EmployeeSpecialLeaves));
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateAsync(Employee employee)
        {
            try
            {
                await _employeeDal.UpdateAsync(employee);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
