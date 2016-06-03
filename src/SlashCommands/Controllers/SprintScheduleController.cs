﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog.Events;

namespace SlashCommands.Controllers
{
    [Route("api/[controller]")]
    public class SprintScheduleController : Controller
    {
        private DateTime _sprint1StartDate = new DateTime(2016, 4, 18);

        // GET: api/SprintSchedule
        [HttpGet]
        public IActionResult Get(int? sprintNum)
        {
            if (sprintNum == null)
            {
                sprintNum = GetCurrentSprint();
            }
            var dto = CalculateSprintSchedule(sprintNum.Value);
            return Ok(dto);
        }

        public async Task<IActionResult> Post()
        {
            var inCommand = Request.Form["command"][0];
            var inText = Request.Form["text"][0];
            var inResponseUrl = Request.Form["response_url"][0];
            var inUserId = Request.Form["user_id"][0];
            var inUserName = Request.Form["user_name"][0];

            int sprintNum;
            int.TryParse(inText, out sprintNum);
            if (sprintNum < 1)
            {
                sprintNum = GetCurrentSprint();
            }
            var dto = CalculateSprintSchedule(sprintNum);

            var pretext = $"*Sprint {dto.sprintNumber}*";

            var message = $"*Sprint {dto.sprintNumber}*\n" +
                          $"*Start*    {dto.sprintStartDate:MM/dd/yyyy}\n" +
                          $"*UAT*      {dto.sprintUatDate:MM/dd/yyyy}\n" +
                          $"*Prod*     {dto.sprintProdDate:MM/dd/yyyy}\n";

            var footer = $"{inCommand} triggered by <@{inUserId}>";

            var slackPost = new
            {
                response_type = "in_channel",
                username = "test",
                fallback = message,
                attachments = new[]
                {
                    new {
                        color = "#663399",
                        text = message,
                        footer = footer,
                        mrkdwn_in = new [] { "pretext", "text"}
                    }
                }
            };

            var slackPostJson = JsonConvert.SerializeObject(slackPost);

            using (var client = new HttpClient())
            {

                var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("payload", slackPostJson)
                    }
                );
                var response = client.PostAsync(inResponseUrl, content).Result;
                var responseString = await response.Content.ReadAsStringAsync();
                var logLevel = response.IsSuccessStatusCode ? LogEventLevel.Information : LogEventLevel.Warning;
                Serilog.Log.Write(logLevel, $"Slack returned code: {(int)response.StatusCode} {response.StatusCode}");
                Serilog.Log.Write(logLevel, responseString);
            }

            return Ok("command: " + inCommand);
        }

        private int GetCurrentSprint()
        {
            return 4;
        }

        private SprintScheuleDTO CalculateSprintSchedule(int sprintNumber)
        {
            var result = new SprintScheuleDTO();
            result.sprintNumber = sprintNumber;
            result.sprintStartDate = _sprint1StartDate.AddDays(14 * (sprintNumber - 1));
            result.sprintUatDate = result.sprintStartDate.AddDays(11);
            result.sprintProdDate = result.sprintStartDate.AddDays(20);

            return result;
        }

        private class SprintScheuleDTO
        {
            public int sprintNumber { get; set; }
            public DateTime sprintStartDate { get; set; }
            public DateTime sprintUatDate { get; set; }
            public DateTime sprintProdDate { get; set; }

        }
    }
}
