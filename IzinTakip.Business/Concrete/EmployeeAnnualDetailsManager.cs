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
    public class EmployeeAnnualDetailsManager : IEmployeeAnnualDetailsService
    {
        private IEmployeeAnnualDetailsDal _employeeAnnualDetailsDal;

        public EmployeeAnnualDetailsManager(IEmployeeAnnualDetailsDal employeeAnnualDetailsDal)
        {
            _employeeAnnualDetailsDal = employeeAnnualDetailsDal;
        }
        public async Task CreateAsync(EmployeeAnnualDetails employeeAnnualDetails)
        {
            try
            {
                await _employeeAnnualDetailsDal.CreateAsync(employeeAnnualDetails);
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
                await _employeeAnnualDetailsDal.DeleteAsync(new EmployeeAnnualDetails { Id = empId });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> Exist(int id)
        {
            var result = await _employeeAnnualDetailsDal.GetWithInludesAsync(x => x.Id == id);

            if (result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<EmployeeAnnualDetails> FindEmployeeAnnualDetailsByIdAsync(int? empId)
        {
            try
            {
                return await _employeeAnnualDetailsDal.GetWithInludesAsync(id => id.Id == empId, c => c.Include(con => con.Employees));
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ICollection<EmployeeAnnualDetails>> GetAllEmployeeAnnualDetailsAsync()
        {
            try
            {
                return await _employeeAnnualDetailsDal.GetListWithIncludesAsync(null);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ICollection<EmployeeAnnualDetails>> GetAllEmployeeAnnualDetailsByIdAsync(int? empId)
        {
            try
            {
                return await _employeeAnnualDetailsDal.GetListWithIncludesAsync(x=> x.EmployeesId == empId);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateAsync(EmployeeAnnualDetails employee)
        {
            try
            {
                await _employeeAnnualDetailsDal.UpdateAsync(employee);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
