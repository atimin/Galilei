using NUnit.Framework;
using System;
using Galilei;
using Galilei.Core;

namespace Galilei.Test
{
	[TestFixture()]
	public class UConfig
	{
		private Server srv;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			srv = new Server();
			
			srv["/"] = new Node("node_1");
			srv["/node_1"] = new Node("node_2");
		}
		
		[Test()]
		public void TestLoadConfig ()
		{
			Server srv = new Server();
			Assert.AreEqual(this.srv["/node_1/node_2"], srv["/node_1/node_2"]);
		}
	}
}

