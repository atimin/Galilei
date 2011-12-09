using System;

namespace Galilei.Core
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NodeAttribute : System.Attribute
	{
		private string description;
		public string Description {
			get { return description; }
		}
		
		public NodeAttribute() : this("")
		{
		}
		
		public NodeAttribute(string description)
		{
			this.description = description;
		}
	}
}

