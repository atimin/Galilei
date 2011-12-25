using NUnit.Framework;
using System;
using Galilei.Core;

namespace Galilei
{
	[TestFixture()]
	public class UConfig
	{
		private Server srv;
		
		[SetUp]
		public void SetUp()
		{
			this.srv = new Server();
			
			this.srv["/"] = new Node("node_1");
			this.srv["/node_1"] = new Node("node_2");
			Configurator conf = new Configurator("test_config.json", this.srv);
			conf.Save();
		}
		
		[Test()]
		public void TestLoadConfig ()
		{
			Server srv = new Server();
			Configurator conf = new Configurator("test_config.json", srv);
			conf.Load();
			
			Assert.AreEqual(this.srv["/node_1/node_2"].FullName, srv["/node_1/node_2"].FullName);
		}
	}
}

