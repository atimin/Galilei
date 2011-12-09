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
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			srv = new Server();
			srv.Port= 3004;
			Node node_1 = new Node("node_1");
			Node node_2 = new Node("node_2");
			
			node_1.Parent = srv;
			node_2.Parent = node_1;
	
			srv.Start();
		}
		
		[TestFixtureTearDown]
		public void TearDown()
		{
			srv.Stop();
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

