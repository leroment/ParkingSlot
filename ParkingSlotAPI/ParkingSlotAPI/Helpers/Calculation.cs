using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Helpers
{
    public class Calculation
    {
        private DateTime dateTime;
        private int duration;


        public Calculation(DateTime dt, int duration)
        {
            this.dateTime = dt;
            this.duration = duration;

        }

        public int parkingRate(DateTime dt)
        {

            return (int)dateTime.DayOfWeek;

        }
        public DateTime getDateTime()
        {
            return this.dateTime;
        }
        public void setDateTime(DateTime dt)
        {
            this.dateTime = dt;
        }
        public int getDuration()
        {
            return this.duration;
        }
        public void setDuration(int duration)
        {
            this.duration = duration;

        }



        public double calculatePrice(double rate, double min)
        {

            double result = 0;
            double finalDuration = this.duration / min;
            if (finalDuration < 1)
            {
                result = rate;
            }
            else
            {
                int getWholeNumber = (int)finalDuration;
                if ((finalDuration - (double)getWholeNumber) > 0)
                {
                    result += rate;
                }
                result += getWholeNumber * rate;
            }
            return Math.Round(result, 2, MidpointRounding.ToEven);


        }
    }
}
