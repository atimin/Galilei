using System;
namespace Galilei.Core
{
	public class XpcaPathError :XpcaError
	{
		public XpcaPathError (string path) : base("Node has not found by `" + path + "`")
		{
		}
	}
}

