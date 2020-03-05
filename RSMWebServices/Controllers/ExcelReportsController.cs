using System;
using Microsoft.AspNetCore.Mvc;
using RSMWebServices.Helpers;
using ExcelHelper;
using Hangfire;

namespace RSMWebServices.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExcelReportsController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        public ExcelReportsController(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        [ActionName("CreateReport")]
        [HttpPost]
        public IActionResult CreateReport()
        {
            Helper helper = new Helper();
            helper.RefreshFile(_appSettings.SourceLocationForExcelFile);
            helper.GetAndSave(_appSettings.SourceLocationForExcelFile, _appSettings.TargetLocationForExcelFile);
            return Ok();
        }
        [ActionName("CreateReportFromPath")]
        [HttpPost]
        public IActionResult CreateReport(string sourceLocation, string targetLocation)
        {
            Helper helper = new Helper();
            helper.RefreshFile(sourceLocation);
            helper.GetAndSave(sourceLocation, targetLocation);
            return Ok();
        }
        [ActionName("CreateRecuringReport")]
        [HttpPost]
        public IActionResult CreateRecuringReport(string recuringId, int frequency, int hours, int minutes)
        {
            AddRecuringJob(
                recuringId,
                frequency,
                hours,
                minutes);
            return Ok();
        }
        [ActionName("CreateRecuringReportFromPath")]
        [HttpPost]
        public IActionResult CreateRecuringReport(string recuringId, int frequency, int hours, int minutes, string sourceLocation, string targetLocation)
        {
            AddRecuringJob(sourceLocation, targetLocation, recuringId,
                frequency,
                hours,
                minutes);
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void AddRecuringJob(string recuringId, int frequency, int hours, int minutes)
        {
            RecurringJob.AddOrUpdate(recuringId,
                () => CreateReport(),
                $"{minutes + 1} {hours} */{frequency} * *",
                TimeZoneInfo.FindSystemTimeZoneById("Georgian Standard Time"));
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public void AddRecuringJob(string sourceLocation, string targetLocation, string recuringId, int frequency, int hours, int minutes)
        {
            RecurringJob.AddOrUpdate(recuringId,
                () => CreateReport(sourceLocation, targetLocation),
                $"{minutes + 1} {hours} */{frequency} * *",
                TimeZoneInfo.FindSystemTimeZoneById("Georgian Standard Time"));
        }
    }
}