﻿using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TicketingApp.Dtos;
using TicketingApp.Models;
using TicketingApp.Service;

namespace TicketingApp.Controllers
{
    [Route("api/trains")]
    [ApiController]
    public class TrainController : ControllerBase
    {
        private readonly TrainService _trainService;

        public TrainController(TrainService trainService)
        {
            _trainService = trainService;
        }

        [HttpPost]
        [Route("add")]
        public ActionResult CreateTrain(TrainSchedule trainSchedule)
        {
            bool created = _trainService.CreateTrain(trainSchedule);
            if (created)
            {
                return Ok("Train Schedule created successfully.");
            }
            return BadRequest("Train Schedule creation failed due to validation constraints.");
        }

        [HttpPut("update/{id}")]
        public ActionResult UpdateTrainSchedule(string id, TrainSchedule updatedSchedule)
        {
            bool updated = _trainService.UpdateTrainSchedule(id, updatedSchedule);
            if (updated)
            {
                return Ok("Train Schedule updated successfully.");
            }
            return BadRequest("Train Schedule update failed due to reservation not found.");
        }

        [HttpDelete("cancel/{id}")]
        public ActionResult CancelTrainSchedule(string id)
        {
            bool canceled = _trainService.CancelTrainSchedule(id);
            if (canceled)
            {
                return Ok("Train Schedule canceled successfully.");
            }
            return BadRequest("Train Schedule cancellation failed due to validation constraints or reservation not found.");
        }

        [HttpGet]
        [Route("getAll")]
        public ActionResult<List<TrainSchedule>> GetAllTrains()
        {
            List<TrainSchedule> trainSchedules = _trainService.GetAllOngoingTrains();
            return Ok(trainSchedules);
        }

        [HttpGet("{id}")]
        public ActionResult<List<TrainSchedule>> GetTrainById(string id)
        {
            List<TrainSchedule> trainSchedules = _trainService.GetTrainById(id);
            if (trainSchedules.Count != 0)
            {
                return Ok(trainSchedules);
            }
            return BadRequest("Train Schedule not found.");
        }

        [HttpGet]
        [Route("search")]
        public ActionResult<List<TrainSearchResult>> SearchTrains(
        [FromQuery] string fromStationName,
        [FromQuery] string toStationName,
        [FromQuery] string date,
        [FromQuery] int minAvailableSeatCount = 1)
        {
            if (DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                if (parsedDate > DateTime.Now.Date)
                {
                    List<TrainSearchResult> matchingTrains = _trainService.SearchTrains(fromStationName, toStationName, date, minAvailableSeatCount);

                    if (matchingTrains.Count > 0)
                    {
                        return Ok(matchingTrains);
                    }

                    return BadRequest("No matching trains found.");
                }
            }

            return BadRequest("Booking must be made at least one day before the booking date.");
        }

        [HttpGet]
        [Route("history/getAll")]
        public ActionResult<List<TrainSchedule>> GetAllTrainHistory()
        {
            List<TrainSchedule> trainSchedules = _trainService.GetAllTrainHistory();
            return Ok(trainSchedules);
        }

    }
}
