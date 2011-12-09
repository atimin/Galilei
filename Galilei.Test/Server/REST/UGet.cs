using NUnit.Framework;
using System;
using Galilei;
using Galilei.Core;
using System.Net;

namespace Galilei.Test
{
	[TestFixture()]
	public class UGet
	{
		private Server srv;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			srv = new Server();
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
		public void GetNode()
		{
			HttpWebResponse response = Helper.Get("http://127.0.0.1:3001/node_1");
			
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("application/json", response.ContentType);
			Assert.AreEqual("{\"type\":\"Node\"," +
				"\"name\":\"node_1\"," +
				"\"parent\":\"xpca://\"," +
				"\"children\":[\"xpca://node_1/node_2\"]}", 
				Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
			);
		}
		
		[Test()]
		public void GetNodeJson()
		{
			HttpWebResponse response = Helper.Get("http://127.0.0.1:3001/node_1.json");
			
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("application/json", response.ContentType);
			Assert.AreEqual("{\"type\":\"Node\"," +
				"\"name\":\"node_1\"," +
				"\"parent\":\"xpca://\"," +
				"\"children\":[\"xpca://node_1/node_2\"]}", 
				Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
			);

		}
		
		[Test()]
		public void GetNodeXml()
		{
			HttpWebResponse response = Helper.Get("http://127.0.0.1:3001/node_1.xml");
			
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("application/xml", response.ContentType);
			Assert.AreEqual(
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + 
				"<root>" +
					"<type>Node</type>" +
					"<name>node_1</name>" +
					"<parent>xpca://</parent>" +
					"<children>" +
						"<item>xpca://node_1/node_2</item>" +
					"</children>" +
				"</root>",
				Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
			);

		}
		
		[Test()]
		public void GetUnexpectedFormat()
		{
			HttpWebResponse response = Helper.Get("http://127.0.0.1:3001/node_1.xxx");
			
			Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
		}
		
		[Test()]
		public void GetUnexpectedNode()
		{
			HttpWebResponse response = Helper.Get("http://127.0.0.1:3001/node_xxx");
			
			Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
		}
	}
}

