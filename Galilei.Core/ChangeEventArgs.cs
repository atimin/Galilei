using System;
namespace Galilei.Core
{
	public class ChangeEventArgs : EventArgs
	{
		private string propertyName;
		private object lastValue;
		private object newValue;
		
		public ChangeEventArgs (string propertyName, object lastValue, object newValue)
		{
			this.propertyName = propertyName;
			this.lastValue = lastValue;
			this.newValue = newValue;
		}
	

		public object LastValue 
		{
			get { return this.lastValue; }
		}

		public object NewValue 
		{
			get { return this.newValue;	}
		}

		public string PropertyName 
		{ 
			get { return this.propertyName;	}
		}
}
}

