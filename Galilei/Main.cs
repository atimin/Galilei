using System;
using System.Net;
using System.Web;

using Galilei.Core;

namespace Galilei
{
	class MainClass
	{
		public static void Main (string[] args)
		{	
			Server srv = new Server();
			
			srv.Start();			
			
			Console.ReadKey();
		}
	}
}
