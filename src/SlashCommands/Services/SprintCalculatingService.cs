using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlashCommands.DTOs;

namespace SlashCommands.Services
{
    public class SprintCalculatingService
    {
        private DateTime _sprint1StartDate = new DateTime(2016, 4, 18);
        private int daysInSprint = 14;
        private int daysBeforeUat = 11;
        private int daysBeforeProd = 20;

        private int GetSprintForDate(DateTime targetDate)
        {
            var daysSinceSprint1 = targetDate.Subtract(_sprint1StartDate).TotalDays;
            var sprintNum = (daysSinceSprint1 / daysInSprint) + 1;
            return (int)sprintNum;
        }

        public SprintScheuleDTO CalculateSprintSchedule(int sprintNumber)
        {
            var result = new SprintScheuleDTO();
            result.sprintNumber = sprintNumber;
            result.sprintStartDate = _sprint1StartDate.AddDays(daysInSprint * (sprintNumber - 1));
            result.sprintUatDate = result.sprintStartDate.AddDays(daysBeforeUat);
            result.sprintProdDate = result.sprintStartDate.AddDays(daysBeforeProd);

            return result;
        }

        public SprintDateScheuleDTO CalculateSprintDateSchedule(DateTime targetDate)
        {
            var result = new SprintDateScheuleDTO();
            result.targetDate = targetDate;

            var daysSinceSprint1 = targetDate.Subtract(_sprint1StartDate).TotalDays;

            result.qaSprintNum = (int)((daysSinceSprint1 / daysInSprint) + 1);
            result.uatSprintNum = (int)(((daysSinceSprint1 - daysBeforeUat) / daysInSprint) + 1);
            result.prodSprintNum = (int)(((daysSinceSprint1 - daysBeforeProd) / daysInSprint) + 1);

            result.qaSprintSince = CalculateSprintSchedule(result.qaSprintNum).sprintStartDate;
            result.uatSprintSince = CalculateSprintSchedule(result.uatSprintNum).sprintUatDate;
            result.prodSprintSince = CalculateSprintSchedule(result.prodSprintNum).sprintProdDate;

            return result;
        }
    }
}
