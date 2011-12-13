using NUnit.Framework;
using System;
using Galilei;
using Galilei.Core;
using System.Net;

namespace Galilei.Test
{
	[TestFixture()]
	public class UDelete
	{
		private Server srv;
		private Galilei galilei;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			srv = Helper.InitServer(3004);
			
			galilei = new Galilei(srv);
			galilei.Start();
		}
		
		[TestFixtureTearDown]
		public void TearDown()
		{
			galilei.Stop();
		}
		
		[Test()]
		public void DeleteNode()
		{
			HttpWebResponse response = Helper.Delete("http://127.0.0.1:3004/node_1");
			
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			
			response = Helper.Get("http://127.0.0.1:3004/node_1.json");			
			Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
		}
		
		
		[Test()]
		public void GetUnexpectedNode()
		{
			HttpWebResponse response = Helper.Delete("http://127.0.0.1:3004/node_xxx");
			
			Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
		}
	}
}

