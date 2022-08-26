using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagment.Models;
using EmployeeManagment.ViewModels;
using Microsoft.AspNetCore.Authorization;
//using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace EmployeeManagment.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IWebHostEnvironment _webHostEnvironment;

    private readonly IEmployeeRepository _employeeRepository;

    public HomeController(ILogger<HomeController> logger, IEmployeeRepository employeeRepository, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _logger.LogDebug(1, "Nlog injected into Home Controller");
        _employeeRepository = employeeRepository;
        //_hostingEnvironment = hostingEnvironment;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: /<controller>/
    [AllowAnonymous]
    public ViewResult Index()
    {
        _logger.LogInformation("Hello, this ts index");
        var model = _employeeRepository.GetAllEmployes();
        return View(model);
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    public ViewResult Details(int? id)
    {

        Employee employee = _employeeRepository.GetEmployee(id.Value);
        if(employee == null)
        {
            Response.StatusCode = 404;
            return View("EmployeeNotFound", id);
        }

        HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
        {
            Employee = _employeeRepository.GetEmployee(id ?? 1),
            PageTitle = "Employee details"
        };

        return View(homeDetailsViewModel);
    }
    [HttpGet]
    public ViewResult Create()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Create(EmployeeCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            string uniqueFileName = ProcessUploadedFile(model);
            Employee employee = new Employee
            {
                Name = model.Name,
                Email = model.Email,
                Department = model.Department,
                PhotoPath = uniqueFileName
            };

            _employeeRepository.Add(employee);
            return RedirectToAction("details", new { id = employee.Id });
        }

        return View();
    }

    [HttpGet]
    public ViewResult Edit(int id)
    {
        Employee employee = _employeeRepository.GetEmployee(id);
        EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel

        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            Department = employee.Department,
            ExistingPhotoPath = employee.PhotoPath
        };

        return View(employeeEditViewModel);
    }

    [HttpPost]
    public IActionResult Edit(EmployeeEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            Employee employee = _employeeRepository.GetEmployee(model.Id);
            employee.Name = model.Name;
            employee.Email = model.Email;
            employee.Department = model.Department;

            if (model.Photo != null)
            {
                if(model.ExistingPhotoPath != null)
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                    System.IO.File.Delete(filePath);
                }
                employee.PhotoPath = ProcessUploadedFile(model);
            }

            
            _employeeRepository.Update(employee);
            return RedirectToAction("index");
        }

        return View();
    }

    private string ProcessUploadedFile(EmployeeCreateViewModel model)
    {
        string uniqueFileName = null;
        if (model.Photo != null)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using(var fileStream = new FileStream(filePath, FileMode.Create))
            model.Photo.CopyTo(fileStream);
        }

        return uniqueFileName;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}



