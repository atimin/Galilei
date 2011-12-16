using System;
namespace Galilei.Core
{
	public class XpcaTypeError : XpcaError
	{
		public XpcaTypeError (string typeName) : base("Type `" + typeName + "` is not XPCA type")
		{
		}
	}
}

