using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI
{
	public class Calculation
	{

		private DateTime dateTime;
		private int duration;
		private int priceRate;

		private int carRate, motorcyleRate, heavyVehicleRate;
		private int weekdayMin, weekdayRate, saturdayMin, saturdayRate, PHMin, PHRate;

		public Calculation(DateTime dt, int duration, int priceRate, int weekdayMin, int weekdayRate, int saturdayMin, int saturdayRate, int PHMin, int PHRate)
		{
			this.dateTime = dt;
			this.duration = duration;
			this.priceRate = priceRate;
			this.weekdayMin = weekdayMin;
			this.weekdayRate = weekdayRate;
			this.saturdayMin = saturdayMin;
			this.saturdayRate = saturdayRate;
			this.PHMin = PHMin;
			this.PHRate = PHRate;

		}

		public int parkingRate(DateTime dt)
		{

			int value = (int)dateTime.DayOfWeek;
			int result;
			if (value > 0 && value < 6)
			{
				result = getWeekdayRate();
			}
			else if (value == 6)
			{
				result = getSaturdayRate();
			}
			else
			{
				result = getPHRate();
			}
			return result;
		}
		public DateTime getDateTIme()
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
		public int getPriceRate()
		{
			return this.priceRate;
		}
		public void setPriceRate(int newprice)
		{
			this.priceRate = newprice;
		}

		public int getWeekdayMin()
		{
			return this.weekdayMin;
		}
		public void setWeekdayMin(int newValue)
		{
			this.weekdayMin = newValue;
		}
		public int getWeekdayRate()
		{
			return this.weekdayRate;
		}
		public void setWeekdayRate(int newValue)
		{
			this.weekdayRate = newValue;

		}
		public int getSaturdayMin()
		{
			return this.saturdayMin;

		}
		public void setSaturdayMin(int newValue)
		{
			this.saturdayMin = newValue;
		}
		public int getSaturdayRate()
		{
			return this.saturdayRate;
		}
		public void setSaturdayRate(int newValue)
		{
			this.saturdayRate = newValue;
		}
		public int getPHMin()
		{
			return this.PHMin;
		}
		public void setPHMin(int newValue)
		{
			this.PHMin = newValue;
		}
		public int getPHRate()
		{
			return this.PHRate;
		}
		public void setPHRate(int newValue)
		{
			this.PHRate = newValue;
		}


		public double calculationPrice(DateTime dt)
		{


	
				return 	duration* parkingRate(dt);

		
		}
			
	}
}
