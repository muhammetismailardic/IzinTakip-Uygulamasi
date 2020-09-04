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
    public class EmployeeSpecialLeaveManager : IEmployeeSpecialLeaveService
    {
        private IEmployeeSpecialLeaveDal _employeeSpecialLeaveDal;

        public EmployeeSpecialLeaveManager(IEmployeeSpecialLeaveDal employeeSpecialLeaveDal)
        {
            _employeeSpecialLeaveDal = employeeSpecialLeaveDal;
        }

        public async Task CreateAsync(EmployeeSpecialLeave employeeSpecialLeave)
        {
            try
            {
                await _employeeSpecialLeaveDal.CreateAsync(employeeSpecialLeave);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task DeleteAsync(int Id)
        {
            try
            {
                await _employeeSpecialLeaveDal.DeleteAsync(new EmployeeSpecialLeave { Id = Id });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateAsync(EmployeeSpecialLeave employeeSpecialLeave)
        {
            try
            {
                await _employeeSpecialLeaveDal.UpdateAsync(employeeSpecialLeave);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Exist(int id)
        {
            var result = await _employeeSpecialLeaveDal.GetWithInludesAsync(x => x.Id == id);

            if (result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<ICollection<EmployeeSpecialLeave>> GetAllSpecialLeavebyEmployeeIdAsync(int empId)
        {
            try
            {
                return await _employeeSpecialLeaveDal.GetListWithIncludesAsync
                            (x => x.EmployeeId == empId, ı => ı.Include(con => con.Employee));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<EmployeeSpecialLeave> FindEmployeeSpecialLeaveByIdAsync(int? Id)
        {
            try
            {
                return await _employeeSpecialLeaveDal.GetWithInludesAsync
                            (x => x.Id == Id, ı => ı.Include(con => con.Employee));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
