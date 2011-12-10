using System;

using Galilei.Core;

namespace Galilei.Simulator
{
	[Node]
	public class RandomPoint : Point
	{
		private double maxValue;
		private double minValue;
		
		public RandomPoint () : base()
		{
			minValue = 0.0;
			maxValue = 100.0;
			SetValue(0.0, DateTime.Now, Quality.Init);
		}
		
		[Config]
		public double MaxValue 
		{
			get { return maxValue; }
			set { maxValue = value; }
		}
		
		[Config]
		public double MinValue 
		{
			get { return minValue; }
			set { minValue = value; }
		}
		
		public override void SetValue (object value, DateTime timestamp, Quality quality)
		{
			double raw = (double)value;
			
			if (raw >= minValue && raw <= maxValue) {
				base.SetValue (raw, timestamp, quality);
			} 
			else {
				throw new Exception();
			}
		}
		
		public override object GetValue ()
		{
			return base.GetValue ();
		}
	}
}

