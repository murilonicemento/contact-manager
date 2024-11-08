using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace ContactManager.Controllers;

[Route("[controller]")]
public class CountriesController : Controller
{
    private readonly ICountriesService _countriesService;

    public CountriesController(ICountriesService countriesService)
    {
        _countriesService = countriesService;
    }


    [Route("UploadFromExcel")]
    public IActionResult UploadFromExcel()
    {
        return View();
    }

    [Route("")]
    public async Task<IActionResult> UploadFromExcel(IFormFile? excelFile)
    {
        if (excelFile is null || excelFile.Length == 0)
        {
            ViewBag.ErrorMessage = "Please Select a excel file.";

            return View();
        }

        if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ViewBag.ErrorMessage = "Unsupported file. 'xlsx' file is expected";

            return View();
        }

        int countriesFromExcelFile = await _countriesService.UploadCountriesFromExcelFile(excelFile);

        ViewBag.Message = $"{countriesFromExcelFile} Countries Upload";

        return View();
    }
}