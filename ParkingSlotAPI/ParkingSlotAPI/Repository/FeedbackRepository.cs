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
        void SaveFeedback(Feedback feedback);
        void DeleteFeedback(Feedback feedback);
        bool Save();
    }

    public class FeedbackRepository : IFeedbackRepository
    {
        private FeedbackContext _context;
        public FeedbackRepository(FeedbackContext context)
        {
            context = _context;
        }
    }

    public IEnumerable<Feedback> GetFeedbacks()
    {
        return _context.Feedbacks.OrderBy(a => a.Feedbacks);
    }

    public Feedback GetFeedback(Guid feedbackId)
    {
        return _context.Feedbacks.FirstOrDefault(a => a.Id == feedbackId);
    }

    public void SaveFeedback(Feedback feedback)
    {
        feedback = Guid.NewGuid();
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

