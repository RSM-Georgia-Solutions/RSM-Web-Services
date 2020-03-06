﻿using System;
using Microsoft.AspNetCore.Mvc;
using CorrectContraAccountLogicDLL;
using Hangfire;
using RSMWebServices.Helpers;

namespace RSMWebServices.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ContraAccountsController : ControllerBase
    {

        private readonly AppSettings _appSettings;
        public ContraAccountsController(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        [ActionName("CorrectionByPeriod")]
        [HttpPost]
        public IActionResult CorrectionByPeriod(CorrectionJournalEntriesParams entriesParams)
        {
            try
            {
                CorrectionLogic logic = new CorrectionLogic(_appSettings.SqlServerHostName,
                    int.Parse(_appSettings.DbServerType),
                    _appSettings.SapUserName,
                    _appSettings.SapPassword,
                    _appSettings.SqlServerCatalog);

                logic.CorrectionJournalEntries(entriesParams);
                logic.CorrectionJournalEntriesSecondLogic(entriesParams);
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e.Message);
            }
            return Accepted();
        }
        [ActionName("CorrectionByTranId")]
        [HttpPost]
        public IActionResult CorrectionById(string tranId)
        {
            try
            {
                CorrectionLogic logic = new CorrectionLogic(_appSettings.SqlServerHostName, int.Parse(_appSettings.DbServerType), _appSettings.SapUserName, _appSettings.SapPassword, _appSettings.SqlServerCatalog);
                logic.CorrectionJournalEntries(tranId);
                logic.CorrectionJournalEntriesSecondLogic(tranId, 120);
            }
            catch (Exception e)
            {
                return UnprocessableEntity(e.Message);
            }
            return Accepted();
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