using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IzinTakip.Business.Abstract;
using IzinTakip.Entities.Concrete;
using IzinTakip.UI.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IzinTakip.UI.Controllers
{
    [Authorize]
    public class SpecialLeaveController : Controller
    {
        IEmployeeSpecialLeaveService _employeeSpecialLeave;
        public WorkingDays calworkingDays = new WorkingDays();
        public SpecialLeaveController(IEmployeeSpecialLeaveService employeeSpecialLeave)
        {
            _employeeSpecialLeave = employeeSpecialLeave;
        }
        public async Task<IActionResult> Index(int empId)
        {
            var empSpecialLeaveDetails = await _employeeSpecialLeave.GetAllSpecialLeavebyEmployeeIdAsync(empId);

            if (empSpecialLeaveDetails.Count != 0)
            {
                foreach (var item in empSpecialLeaveDetails)
                {
                    if (item.StartDate <= DateTime.Now && item.EndDate >= DateTime.Now)
                    {
                        item.IsOnLeave = true;
                    }
                    else { item.IsOnLeave = false; }
                    await _employeeSpecialLeave.UpdateAsync(item);
                }
            }
            return View(empSpecialLeaveDetails.OrderBy(x=> x.StartDate).ToList());
        }

        [HttpGet]
        public IActionResult Create(int empId)
        {
            ViewBag.employeeId = empId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeSpecialLeave employeeSpecialLeave)
        {
            if (ModelState.IsValid)
            {
                // If selected date is more than one day
                int currentUsedDate = 0;
                var result = employeeSpecialLeave.EndDate.Date.Subtract(employeeSpecialLeave.StartDate.Date);
                if (result.Days > 0)
                {
                    currentUsedDate = calworkingDays.HolidaysAsync(employeeSpecialLeave.StartDate, employeeSpecialLeave.EndDate);
                }

                employeeSpecialLeave.StartDate = employeeSpecialLeave.StartDate;
                employeeSpecialLeave.EndDate = employeeSpecialLeave.EndDate;//employeeSpecialLeave.EndDate.AddHours(23).AddMinutes(59);
                employeeSpecialLeave.CreatedAt = DateTime.Now;
                employeeSpecialLeave.UpdatedAt = DateTime.Now;

                if (calworkingDays.PublicHolidayDates.Count != 0)
                {
                    var createEmpSpecialLeaveWithPubHolidays = new AnnualLeaveViewModel()
                    {
                        CurrentUsedDate = currentUsedDate,
                        PublicHolidays = calworkingDays.PublicHolidayDates,
                        EmployeeSpecialLeaves = employeeSpecialLeave,
                    };

                    return View("HasPublicHoliday", createEmpSpecialLeaveWithPubHolidays);
                }
                else
                {
                    employeeSpecialLeave.Count = currentUsedDate;

                    if (employeeSpecialLeave.Count >= 2)
                    {
                        employeeSpecialLeave.Hours = "*";
                    }
                    else
                    {
                        var getHours = employeeSpecialLeave.EndDate - employeeSpecialLeave.StartDate;
                        employeeSpecialLeave.Hours = getHours.ToString();//String.Format("{0}", getHours.TotalHours, "hours");
                    }

                }

                await _employeeSpecialLeave.CreateAsync(employeeSpecialLeave);
                return RedirectToAction("Index", "SpecialLeave", new { empId = employeeSpecialLeave.EmployeeId });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateWithPublicHolidays(AnnualLeaveViewModel annualLeaveViewModel)
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
                //Adding Dates
                annualLeaveViewModel.EmployeeSpecialLeaves.Count = annualLeaveViewModel.CurrentUsedDate - counter;

                //Adding Hours
                if (annualLeaveViewModel.EmployeeSpecialLeaves.Count >= 2)
                {
                    annualLeaveViewModel.EmployeeSpecialLeaves.Hours = "*";
                }
                else
                {
                    var getHours = annualLeaveViewModel.EmployeeSpecialLeaves.EndDate - annualLeaveViewModel.EmployeeSpecialLeaves.StartDate;
                    annualLeaveViewModel.EmployeeSpecialLeaves.Hours = String.Format("{0}", getHours.TotalHours, "hours");
                }

                await _employeeSpecialLeave.CreateAsync(annualLeaveViewModel.EmployeeSpecialLeaves);

                return RedirectToAction("Index", "SpecialLeave", new { empId = annualLeaveViewModel.EmployeeSpecialLeaves.EmployeeId });
            }

            return RedirectToAction("Index", "SpecialLeave", new { empId = annualLeaveViewModel.EmployeeSpecialLeaves.EmployeeId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employeeSpecialPer = await _employeeSpecialLeave.FindEmployeeSpecialLeaveByIdAsync(id);
            if (employeeSpecialPer == null)
            {
                return NotFound();
            }
            return View(employeeSpecialPer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int Id, EmployeeSpecialLeave employeeSpecialLeave)
        {
            if (Id != employeeSpecialLeave.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // If selected date is more than one day
                    int currentUsedDate = 0;
                    var result = employeeSpecialLeave.EndDate.Date.Subtract(employeeSpecialLeave.StartDate.Date);
                    if (result.Days > 0)
                    {
                        currentUsedDate = calworkingDays.HolidaysAsync(employeeSpecialLeave.StartDate, employeeSpecialLeave.EndDate);
                    }

                    employeeSpecialLeave.StartDate = employeeSpecialLeave.StartDate;
                    employeeSpecialLeave.EndDate = employeeSpecialLeave.EndDate;
                    employeeSpecialLeave.Count = currentUsedDate;
                    employeeSpecialLeave.UpdatedAt = DateTime.Now;

                    if (calworkingDays.PublicHolidayDates.Count != 0)
                    {
                        var createEmpSpecialLeaveWithPubHolidays = new AnnualLeaveViewModel()
                        {
                            CurrentUsedDate = currentUsedDate,
                            PublicHolidays = calworkingDays.PublicHolidayDates,
                            EmployeeSpecialLeaves = employeeSpecialLeave,
                        };

                        return View("UpdateHasPublicHoliday", createEmpSpecialLeaveWithPubHolidays);
                    }
                    else
                    {
                        employeeSpecialLeave.Count = currentUsedDate;

                        //Adding hours back
                        if (employeeSpecialLeave.Count >= 2)
                        {
                            employeeSpecialLeave.Hours = "*";
                        }
                        else
                        {
                            var getHours = employeeSpecialLeave.EndDate - employeeSpecialLeave.StartDate;
                            employeeSpecialLeave.Hours = getHours.ToString();//String.Format("{0}", getHours.TotalHours, "hours");
                        }
                    }

                    await _employeeSpecialLeave.UpdateAsync(employeeSpecialLeave);
                    return RedirectToAction("Index", "SpecialLeave", new { empId = employeeSpecialLeave.EmployeeId });
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return View(employeeSpecialLeave);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWithPublicHoliday(AnnualLeaveViewModel annualLeaveViewModel)
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
                annualLeaveViewModel.EmployeeSpecialLeaves.Count = annualLeaveViewModel.CurrentUsedDate - counter;

                //Adding hours back
                if (annualLeaveViewModel.EmployeeSpecialLeaves.Count >= 2)
                {
                    annualLeaveViewModel.EmployeeSpecialLeaves.Hours = "*";
                }
                else
                {
                    var getHours = annualLeaveViewModel.EmployeeSpecialLeaves.EndDate - annualLeaveViewModel.EmployeeSpecialLeaves.StartDate;
                    annualLeaveViewModel.EmployeeSpecialLeaves.Hours = String.Format("{0}", getHours.TotalHours, "hours");
                }

                await _employeeSpecialLeave.UpdateAsync(annualLeaveViewModel.EmployeeSpecialLeaves);

                return RedirectToAction("Index", "SpecialLeave", new { empId = annualLeaveViewModel.EmployeeSpecialLeaves.EmployeeId });
            }

            return RedirectToAction("Edit", "SpecialLeave", new { empId = annualLeaveViewModel.EmployeeSpecialLeaves.EmployeeId });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeSpecialLeave = await _employeeSpecialLeave.FindEmployeeSpecialLeaveByIdAsync(id);
            if (employeeSpecialLeave == null)
            {
                return NotFound();
            }

            return View(employeeSpecialLeave);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emp = await _employeeSpecialLeave.FindEmployeeSpecialLeaveByIdAsync(id);

            //if (Image != null)
            //{
            //    //Old Image Delete operation goes here
            //    var directory = fileExtentions._rootImageDirectory + "/employee/" + Image;
            //    fileExtentions.DeleteFile(directory);
            //}

            await _employeeSpecialLeave.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { empId = emp.EmployeeId });
        }
    }
}
