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
			Galilei galilei = new Galilei("config.json");
			
			galilei.Start();			
			
			Console.ReadKey();
		}
	}
}
