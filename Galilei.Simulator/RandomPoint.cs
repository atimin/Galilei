using System;

using Galilei.Core;

namespace Galilei.Simulator
{
	[Node]
	public class RandomPoint : Point
	{
		private int maxValue;
		private int minValue;
		
		public RandomPoint () : base()
		{
			minValue = 0;
			maxValue = 100;
			SetValue(0, DateTime.Now, Quality.Init);
		}
		
		[Config]
		public int MaxValue 
		{
			get { return maxValue; }
			set { maxValue = value; }
		}
		
		[Config]
		public int MinValue 
		{
			get { return minValue; }
			set { minValue = value; }
		}
		
		public override void SetValue (object value, DateTime timestamp, Quality quality)
		{
			int raw = (int)value;
			
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

