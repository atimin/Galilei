using System;

namespace Galilei.Core
{
	/// <summary>
	/// Xpca attribute. 
	/// Mark field for XPCA serialization
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ConfigAttribute : PropertyAttribute
	{
		
		public ConfigAttribute() : base()
		{
		}
		
		public ConfigAttribute(string description) : base(description)
		{
		}
	}
}

