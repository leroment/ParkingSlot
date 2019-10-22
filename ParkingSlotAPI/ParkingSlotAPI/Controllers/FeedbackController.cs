using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using AutoMapper;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Models;
using ParkingSlotAPI.Repository;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace ParkingSlotAPI.Controllers
{
    [Authorize(Roles = Role.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;

        public FeedbackController(IFeedbackRepository feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }

        //[AllowAnonymous]
        [HttpGet]
        public IActionResult GetFeedbacks()
        {
            var feedbacksFromRepo = _feedbackRepository.GetFeedbacks();

            if (feedbacksFromRepo == null)
            {
                return NotFound();
            }

            var feedbacks = _mapper.Map<IEnumerable<FeedbackDto>>(feedbacksFromRepo);

            return Ok(feedbacks);
        }

        [HttpGet("{id}", Name = "GetFeedback")]
        public IActionResult GetFeedback(Guid id)
        {
            var feedbackFromRepo = _feedbackRepository.GetFeedback(id);

            if (feedbackFromRepo == null)
            {
                return NotFound();
            }

            var feedback = _mapper.Map<FeedbackDto>(feedbackFromRepo);

            return Ok(feedback);
        }

        [HttpPost]
        public IActionResult AddFeedback([FromBody] FeedbackForCreationDto feedback)
        {
            if (feedback == null)
            {
                return BadRequest();
            }

            var feedbackEntity = _mapper.Map<Feedback>(feedback);

            _feedbackRepository.AddFeedback(feedbackEntity);
            if (!_feedbackRepository.Save())
            {
                throw new Exception("Adding a feedback failed on save.");
            }

            var feedbackToReturn = _mapper.Map<FeedbackDto>(feedbackEntity);

            return CreatedAtRoute("GetFeedback", new { id = feedbackToReturn.Id}, feedbackToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteFeedback(Guid id)
        {
            var feedbackFromRepo = _feedbackRepository.GetFeedback(id);
            if (feedbackFromRepo == null)
            {
                return NotFound();
            }

            _feedbackRepository.DeleteFeedback(feedbackFromRepo);
            if (!_feedbackRepository.Save())
            {
                throw new Exception($"Deleting feedback {id} failed on save");
            }

            return NoContent();
        }
        /*[HttpPut("{id}")]
        public IActionResult UpdatedFeedback(Guid id, [FromBody] FeedbackForUpdateDto feedback)
        {
            if(feedback == null)
            {
                return BadRequest();
            }

            var feedbackFromRepo = _feedbackRepository.GetFeedback(id);
            if (feedbackFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(feedback, feedbackFromRepo);

            _feedbackRepository.UpdatedFeedback(feedbackFromRepo);

            if(!_feedbackRepository.Save())
            {
                throw new Exception($"Updating feedback {id} failed on save");
            }

            return NoContent();
        }*/

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatedFeedback(Guid id, [FromBody]JsonPatchDocument<FeedbackForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var feedbackFromRepo = _feedbackRepository.GetFeedback(id);

            if (feedbackFromRepo == null)
            {
                return NotFound();
            }

            var feedbackToPatch = _mapper.Map<FeedbackForUpdateDto>(feedbackFromRepo);
            patchDoc.ApplyTo(feedbackToPatch);

            _mapper.Map(feedbackToPatch, feedbackFromRepo);

            _feedbackRepository.UpdatedFeedback(feedbackFromRepo);

            if(!_feedbackRepository.Save())
            {
                throw new Exception($"Patching feedback {id} failed on save.");
            }

            return NoContent();
        }
    }
}

