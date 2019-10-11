using System;
using System.Collections.Generic;
using System.Linq;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Database;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Repository
{
    public interface IFeedbackRepository
    {
	    IEnumerable<Feedback> GetFeedbacks();
        Feedback GetFeedback(Guid feedbackId);
        void AddFeedback(Feedback feedback);
        void DeleteFeedback(Feedback feedback);
        bool Save();
    }

    public class FeedbackRepository : IFeedbackRepository
    {
        private ParkingContext _context;
        public FeedbackRepository(ParkingContext context)
        {
            _context = context;
        }

        public IEnumerable<Feedback> GetFeedbacks()
        {
            return _context.Feedbacks.OrderBy(a => a.Id);
        }

        public Feedback GetFeedback(Guid feedbackId)
        {
            return _context.Feedbacks.FirstOrDefault(a => a.Id == feedbackId);
        }

        public void AddFeedback(Feedback feedback)
        {
            feedback.Id = Guid.NewGuid();
            _context.Feedbacks.Add(feedback);
        }

        public void DeleteFeedback(Feedback feedback)
        {
            _context.Feedbacks.Remove(feedback);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

