using IzinTakip.Business.Abstract;
using IzinTakip.Entities.Concrete;
using IzinTakip.UI.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IzinTakip.UI.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeAnnualDetailsService _employeeAnnualDetailsService;
        private readonly IEmployeeSpecialLeaveService _employeeSpecialLeave;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private FileExtentions fileExtentions;

        public EmployeeController(IEmployeeService employeeService, IWebHostEnvironment webHostEnvironment,
                                  IEmployeeAnnualDetailsService employeeAnnualDetailsService, IEmployeeSpecialLeaveService employeeSpecialLeave)
        {
            _employeeService = employeeService;
            _webHostEnvironment = webHostEnvironment;
            _employeeAnnualDetailsService = employeeAnnualDetailsService;
            _employeeSpecialLeave = employeeSpecialLeave;

            fileExtentions = new FileExtentions(_webHostEnvironment);
        }
        public async Task<IActionResult> Index()
        {
            var empList = (await _employeeService.GetAllEmployeesAsync()).Where(x => x.IsActive == true).ToList();

            foreach (var item in empList)
            {
                var empSpecialLeave = await _employeeSpecialLeave.GetAllSpecialLeavebyEmployeeIdAsync(item.Id);

                var recDate = item.RecruitmentDate.Year;
                int currentDate = DateTime.Now.Year;
                int value = currentDate - recDate;

                if (value < 1)
                {
                    item.YearlyAnnualRightCount = 0;
                }
                else if (value >= 1 && value <= 5)
                {
                    item.YearlyAnnualRightCount = 12;
                }
                else if (value >= 6 && value <= 15)
                {
                    item.YearlyAnnualRightCount = 18;
                }
                else if (value > 15)
                {
                    item.YearlyAnnualRightCount = 24;
                }

                var updateEmployee = await _employeeService.FindEmployeeByIdAsync(item.Id);
                updateEmployee.IsOnLeave = false;

                // Finding yearly annual count.
                if (updateEmployee.YearlyAnnualRightCount != item.YearlyAnnualRightCount)
                {
                    updateEmployee.YearlyAnnualRightCount = item.YearlyAnnualRightCount;
                }

                // Find all available total leaves.
                int sum = 0;
                for (int i = 1; i <= value; i++)
                {
                    if (value == 0)
                    {
                        sum = 0;
                    }
                    else if (i >= 1 && i <= 5)
                    {
                        sum += 12;
                    }
                    else if (i >= 6 && i <= 15)
                    {
                        sum += 18;
                    }
                    else if (i > 15)
                    {
                        sum += 24;
                    }
                }
                updateEmployee.TotalAnnualRight = sum;

                // If there is special leave it will add to total leave count.
                if (empSpecialLeave.Count != 0)
                {
                    // Farklı bi yerde kullanılacak
                    //updateEmployee.TotalAnnualRight += empSpecialLeave.Sum(x => x.Count);
                    //updateEmployee.UsedTotalLeaves = empSpecialLeave.Sum(x => x.Count);

                    // Displays whether user active or not

                    foreach (var spcLeaveStatus in empSpecialLeave)
                    {
                        if (spcLeaveStatus.StartDate <= DateTime.Now && spcLeaveStatus.EndDate >= DateTime.Now)
                        {
                            spcLeaveStatus.IsOnLeave = true;
                        }
                        else { spcLeaveStatus.IsOnLeave = false; }

                        updateEmployee.IsOnLeave = spcLeaveStatus.IsOnLeave == true ? updateEmployee.IsOnLeave = true : updateEmployee.IsOnLeave = false;
                    }

                    //var IsTrue = empSpecialLeave.Where(x => x.IsOnLeave == true)
                    //                          .FirstOrDefault();
                    //if (IsTrue != null && IsTrue.IsOnLeave) { updateEmployee.IsOnLeave = true; }
                }
                else { updateEmployee.UsedTotalLeaves = 0; }

                // Displays whether user active or not
                if (item.EmployeeAnnualDetails.Count != 0)
                {
                    //var IsEmpDetailTrue = item.EmployeeAnnualDetails
                    //                               .Where(x => x.IsOnLeave == true)
                    //                               .FirstOrDefault();
                    //if (IsEmpDetailTrue != null && IsEmpDetailTrue.IsOnLeave) { updateEmployee.IsOnLeave = true; }

                    foreach (var empAnnuelDetail in item.EmployeeAnnualDetails)
                    {
                        if (empAnnuelDetail.StartDate <= DateTime.Now && empAnnuelDetail.EndDate >= DateTime.Now)
                        {
                            empAnnuelDetail.IsOnLeave = true;
                        }
                        else { empAnnuelDetail.IsOnLeave = false; }

                        updateEmployee.IsOnLeave = empAnnuelDetail.IsOnLeave == true ? updateEmployee.IsOnLeave = true : updateEmployee.IsOnLeave = false;
                    }
                }

                //Burası değiştirildi
                updateEmployee.UsedTotalLeaves = item.EmployeeAnnualDetails.Sum(totalUsed => totalUsed.Used);
                updateEmployee.AvailableTotalLeaves = updateEmployee.TotalAnnualRight - updateEmployee.UsedTotalLeaves;

                //Adding total Leaves and Used counts to Index table
                await _employeeService.UpdateAsync(updateEmployee);
            }
            // it will return the lates status after updates.
            return View((await _employeeService.GetAllEmployeesAsync()).Where(x => x.IsActive == true).OrderBy(x => x.Name).ToList());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeById = await _employeeService.FindEmployeeByIdAsync(id);

            if (employeeById == null)
            {
                return NotFound();
            }

            return View(employeeById);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                //string uniqueFileName = fileExtentions.UploadedFile(employee.ProfileImage, "employee");
                employee.CreatedAt = DateTime.Now;
                employee.UpdatedAt = DateTime.Now;
                //employee.Image = uniqueFileName;
                employee.UserId = "bc68af64-5675-4a5b-b6b2-92b2fd282cbf";

                var recDate = employee.RecruitmentDate.Year;
                int currentDate = employee.CreatedAt.Year;
                int value = currentDate - recDate;

                // Calculates the the annual rights based on recruitment date and current date.
                if (value < 1)
                {
                    employee.YearlyAnnualRightCount = 0;
                }
                else if (value >= 1 && value <= 6)
                {
                    employee.YearlyAnnualRightCount = 12;
                }
                else if (value >= 7 && value <= 10)
                {
                    employee.YearlyAnnualRightCount = 18;
                }
                else
                {
                    employee.YearlyAnnualRightCount = 24;
                }

                await _employeeService.CreateAsync(employee);
                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var employee = await _employeeService.FindEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, Employee employee)
        {
            if (Id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //string uniqueFileName;

                employee.UpdatedAt = DateTime.Now;

                //if (employee.ProfileImage != null)
                //{
                //    uniqueFileName = fileExtentions.UploadedFile(employee.ProfileImage, "employee");
                //    employee.Image = uniqueFileName;

                //    //Old Image Delete operation goes here
                //    var employeeImage = (await _employeeService.FindEmployeeByIdAsync(Id)).Image;
                //    var directory = fileExtentions._rootImageDirectory + "/employee/" + employeeImage;
                //    fileExtentions.DeleteFile(directory);
                //}
                try
                {
                    await _employeeService.UpdateAsync(employee);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.FindEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var Image = (await _employeeService.FindEmployeeByIdAsync(id)).Image;

            //if (Image != null)
            //{
            //    //Old Image Delete operation goes here
            //    var directory = fileExtentions._rootImageDirectory + "/employee/" + Image;
            //    fileExtentions.DeleteFile(directory);
            //}

            //var findDetails = await _employeeAnnualDetailsService.GetAllEmployeeAnnualDetailsByIdAsync(id);

            //foreach (var item in findDetails)
            //{
            //    await _employeeAnnualDetailsService.DeleteAsync(item.Id);
            //}

            var emp = await _employeeService.FindEmployeeByIdAsync(id);
            emp.IsActive = false;

            await _employeeService.UpdateAsync(emp);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EmployeeExists(int id)
        {
            return await _employeeService.Exist(id);
        }
    }
}
