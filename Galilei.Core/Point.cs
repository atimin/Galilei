using System;


namespace Galilei.Core
{
	public class Point : Node
	{
		private Engine engine;
		private object value;
		private DateTime timestamp;
		private Quality quality;
		
		public Point () : base("New point")
		{
			value = new object();
			timestamp = DateTime.Now;
			quality = Quality.Init;
		}
		
		[Config]
		public Engine Engine 
		{
			get { return engine; }
			set { 
				if (value != null){
					if (engine != null) {
						engine.Points.Remove(this);
					}
					engine = value; 
					engine.Points.Add(this);
				}
				else {
					engine.Points.Remove(this);
					engine = null;
				}
			}
		}
		
		[Property]
		public object Value {
			get { return GetValue(); }
			set { SetValue(value); }
		}
		
		[Property]
		public DateTime Timestamp {
			get { return timestamp; }
			set { SetValue(value); }
		}
		
		[Property]
		public Quality Quality {
			get { return quality; }
			set {SetValue(value); }
		}
		
		#region SetValue
		public virtual object GetValue()
		{
			return value;
		}
		
		public virtual void SetValue(object value, DateTime timestamp, Quality quality)
		{
			this.value = value;
			this.timestamp = timestamp;
			this.quality = quality;
		}
		
		public void SetValue(object value)
		{
			SetValue(value, DateTime.Now, Quality.Good);
		}
		
		public void SetValue(DateTime timestamp)
		{
			SetValue(value, timestamp, quality);
		}
		
		public void SetValue(Quality quality)
		{
			SetValue(value, DateTime.Now, quality);
		}
		#endregion
	}
}

