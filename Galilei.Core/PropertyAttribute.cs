using System;

namespace Galilei.Core
{
	/// <summary>
	/// Xpca attribute. 
	/// Mark field for XPCA serialization
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyAttribute : System.Attribute
	{
		private string description;
		public string Description {
			get { return description; }
		}
		
		public PropertyAttribute() : this("")
		{
		}
				
		public PropertyAttribute(string description)
		{
			this.description = description;
		}
	}
}

