using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IzinTakip.Business.Abstract;
using IzinTakip.Entities.Concrete;
using IzinTakip.UI.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IzinTakip.UI.Controllers
{
    [Authorize]
    public class EmployeeDetailsController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeAnnualDetailsService _employeeAnnualDetailsService;
        public WorkingDays calworkingDays = new WorkingDays();

        public EmployeeDetailsController(IEmployeeAnnualDetailsService employeeAnnualDetailsService, IEmployeeService employeeService)
        {
            _employeeAnnualDetailsService = employeeAnnualDetailsService;
            _employeeService = employeeService;
        }
        public async Task<IActionResult> Index(int empId)
        {
            var empAnnualDetails = await _employeeAnnualDetailsService.GetAllEmployeeAnnualDetailsByIdAsync(empId);

            if (empAnnualDetails.Count != 0)
            {
                foreach (var item in empAnnualDetails)
                {
                    if (item.StartDate <= DateTime.Now && item.EndDate >= DateTime.Now)
                    {
                        item.IsOnLeave = true;
                    }
                    else { item.IsOnLeave = false; }

                    await _employeeAnnualDetailsService.UpdateAsync(item);
                }
            }

            return View(empAnnualDetails);
        }

        [HttpGet]
        public IActionResult AddLeave(int empId)
        {
            ViewBag.empId = empId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddLeave(int Id, EmployeeAnnualDetails employeeAnnualRights, int empId)
        {
            if (Id != employeeAnnualRights.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var employeeDetails = await _employeeService.FindEmployeeByIdAsync(empId);
                var allDataByEmp = await _employeeAnnualDetailsService.GetAllEmployeeAnnualDetailsByIdAsync(empId);
                int currentUsedDate = calworkingDays.HolidaysAsync(employeeAnnualRights.StartDate, employeeAnnualRights.EndDate);

                employeeAnnualRights.EmployeesId = empId;
                employeeAnnualRights.AnnualRights = employeeDetails.YearlyAnnualRightCount;


                employeeAnnualRights.StartDate = employeeAnnualRights.StartDate;
                employeeAnnualRights.EndDate = employeeAnnualRights.EndDate.AddHours(23).AddMinutes(59);

                employeeAnnualRights.CreatedAt = DateTime.Now;
                employeeAnnualRights.UpdatedAt = DateTime.Now;

                employeeAnnualRights.LeftDate = employeeDetails.TotalAnnualRight - allDataByEmp.Sum(usedDate => usedDate.Used);

                if (calworkingDays.PublicHolidayDates.Count != 0)
                {
                    var test = new AnnualLeaveViewModel()
                    {
                        CurrentUsedDate = currentUsedDate,
                        PublicHolidays = calworkingDays.PublicHolidayDates,
                        EmployeeAnnualDetails = employeeAnnualRights,
                    };
                    await _employeeAnnualDetailsService.CreateAsync(employeeAnnualRights);

                    return View("HasPublicHoliday",test);
                }
                else
                {
                    employeeAnnualRights.LeftDate -= currentUsedDate;
                    employeeAnnualRights.Used = currentUsedDate;
                }

                await _employeeAnnualDetailsService.CreateAsync(employeeAnnualRights);
                return RedirectToAction(nameof(Index), new { empId = empId });
            }

            return View(employeeAnnualRights);
        }

        [HttpPost]
        public async Task<IActionResult> AddLeaveWithPublicHoliday(AnnualLeaveViewModel annualLeaveViewModel, int leftDate, int Used)
        {
            if (ModelState.IsValid)
            {
                int counter = 0;
                foreach (var item in annualLeaveViewModel.PublicHolidays)
                {
                    if (item.IsChecked == true)
                    {
                        counter++;
                    }
                }
                int totalUsedDate = annualLeaveViewModel.CurrentUsedDate - counter;

                annualLeaveViewModel.EmployeeAnnualDetails.Used = totalUsedDate;
                annualLeaveViewModel.EmployeeAnnualDetails.LeftDate -= totalUsedDate;
            }

            await _employeeAnnualDetailsService.UpdateAsync(annualLeaveViewModel.EmployeeAnnualDetails);
            return RedirectToAction(nameof(Index), new { empId = annualLeaveViewModel.EmployeeAnnualDetails.EmployeesId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var employeeDetails = await _employeeAnnualDetailsService.FindEmployeeAnnualDetailsByIdAsync(id);
            if (employeeDetails == null)
            {
                return NotFound();
            }
            return View(employeeDetails);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, EmployeeAnnualDetails employeeDetails)
        {
            if (Id != employeeDetails.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    int currentUsedDate = calworkingDays.HolidaysAsync(employeeDetails.StartDate, employeeDetails.EndDate);

                    employeeDetails.LeftDate -= (currentUsedDate - employeeDetails.Used);
                    employeeDetails.Used = currentUsedDate;
                    employeeDetails.UpdatedAt = DateTime.Now;

                    employeeDetails.StartDate = employeeDetails.StartDate;
                    employeeDetails.EndDate = employeeDetails.EndDate.AddHours(23).AddMinutes(59);

                    await _employeeAnnualDetailsService.UpdateAsync(employeeDetails);
                    return RedirectToAction(nameof(Index), new { empId = employeeDetails.EmployeesId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EmployeeDetailsExists(employeeDetails.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(employeeDetails);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeDetails = await _employeeAnnualDetailsService.FindEmployeeAnnualDetailsByIdAsync(id);
            if (employeeDetails == null)
            {
                return NotFound();
            }

            return View(employeeDetails);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var findDetails = await _employeeAnnualDetailsService.FindEmployeeAnnualDetailsByIdAsync(id);

            var getAllDetailsbyEmp = (await _employeeAnnualDetailsService.GetAllEmployeeAnnualDetailsByIdAsync(findDetails.EmployeesId)).LastOrDefault();

            if (getAllDetailsbyEmp != null)
            {
                getAllDetailsbyEmp.LeftDate += findDetails.Used;

                await _employeeAnnualDetailsService.UpdateAsync(getAllDetailsbyEmp);
            }

            await _employeeAnnualDetailsService.DeleteAsync(id);

            return RedirectToAction(nameof(Index), new { empId = findDetails.EmployeesId });
        }

        private async Task<bool> EmployeeDetailsExists(int id)
        {
            return await _employeeAnnualDetailsService.Exist(id);
        }
    }
}
