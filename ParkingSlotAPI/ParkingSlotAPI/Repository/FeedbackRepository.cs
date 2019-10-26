using System;
using System.Collections.Generic;
using System.Linq;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Database;
using System.Threading.Tasks;
using ParkingSlotAPI.Helpers;

namespace ParkingSlotAPI.Repository
{
    public interface IFeedbackRepository
    {
	    IEnumerable<Feedback> GetFeedbacks();
        Feedback GetFeedback(Guid feedbackId);
        void AddFeedback(Feedback feedback);
        void DeleteFeedback(Feedback feedback);
        void UpdatedFeedback(Feedback feedback);
        bool Save();
    }

    public class FeedbackRepository : IFeedbackRepository
    {
        private ParkingContext _context;
        private IUserRepository _userRepository;
        public FeedbackRepository(ParkingContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public IEnumerable<Feedback> GetFeedbacks()
        {
            return _context.Feedbacks.OrderBy(a => a.Id);
        }

        public Feedback GetFeedback(Guid feedbackId)
        {
            return _context.Feedbacks.FirstOrDefault(a => a.Id == feedbackId);
        }

        private bool FeedbackExists(Guid userId, Guid feedbackId)
        {
            return _context.Feedbacks.Any(a => a.Id == userId && a.Id == feedbackId);
        }

        public void AddFeedback(Guid userId, Feedback feedback)
        {
            var user = _userRepository.GetUser(userId);

            if (user != null)
            {
                if (FeedbackExists(userId, feedback.Id))
                {
                    throw new AppException($"This feedback {feedback.Id} already exists for user {userId}!");
                }
                else
                {
                    feedback.Id = Guid.NewGuid();
                    feedback.IsResolved = false;
                    user.
                }
            }
        }

        public void DeleteFeedback(Feedback feedback)
        {
            _context.Feedbacks.Remove(feedback);
        }

        public void UpdatedFeedback(Feedback feedback)
        {
            // no coding
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

