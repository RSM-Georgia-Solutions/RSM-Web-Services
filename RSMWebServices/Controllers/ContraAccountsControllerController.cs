﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CorrectContraAccountLogicDLL;
using Hangfire;
using Microsoft.Extensions.Configuration;
using RSMWebServices.Helpers;
using static System.Configuration.ConfigurationSettings;

namespace RSMWebServices.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ContraAccountsControllerController : ControllerBase
    {

        private readonly AppSettings _appSettings;
        public ContraAccountsControllerController(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        [ActionName("CorrectionByPeriod")]
        [HttpPost]
        public IActionResult CorrectionByPeriod(CorrectionJournalEntriesParams entriesParams)
        {
            CorrectionLogic logic = new CorrectionLogic(_appSettings.SqlServerHostName, 10, _appSettings.SapUserName, _appSettings.SapPassword, _appSettings.SqlServerCatalog);

            logic.CorrectionJournalEntries(entriesParams);
            logic.CorrectionJournalEntriesSecondLogic(entriesParams);
            return Ok();
        }
        [ActionName("CorrectionByTranId")]
        [HttpPost]
        public IActionResult CorrectionById(string tranId)
        {
            CorrectionLogic logic = new CorrectionLogic(_appSettings.SqlServerHostName, 10, _appSettings.SapUserName, _appSettings.SapPassword, _appSettings.SqlServerCatalog);
            logic.CorrectionJournalEntries(tranId);
            logic.CorrectionJournalEntriesSecondLogic(tranId, 120);
            return Ok();
        }

        [ActionName("CorrectionByPeriodRecuring")]
        [HttpPost]
        public IActionResult CorrectionByPeriodRecuring(CorrectionJournalEntriesParams entriesParams, string recuringId, int frequency, int hours, int minutes)
        {
            AddRecuringJob(entriesParams,
                recuringId,
                frequency,
                hours,
                minutes);
            return Ok();
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public void AddRecuringJob(CorrectionJournalEntriesParams entriesParams, string recuringId, int frequency, int hours, int minutes)
        {
            RecurringJob.AddOrUpdate(recuringId,
                () => CorrectionByPeriod(entriesParams),
                $"{minutes + 1} {hours} */{frequency} * *",
                TimeZoneInfo.FindSystemTimeZoneById("Georgian Standard Time"));
        }

    }
}