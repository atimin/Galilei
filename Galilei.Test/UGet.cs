using NUnit.Framework;
using System;
using Galilei.Core;
using System.Net;

namespace Galilei
{
	[TestFixture()]
	public class UGet
	{
		private Galilei galilei;
		
		[TestFixtureSetUp]
		public void SetUp()
		{		
			System.IO.File.Copy("test_config.json.example", "test_config.json", true);
			galilei = new Galilei("test_config.json");
			galilei.Start();
		}
		
		[TestFixtureTearDown]
		public void TearDown()
		{
			galilei.Stop();
		}
		
		[Test()]
		public void GetNode()
		{
			Helper.Get("http://127.0.0.1:3001/node_1", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.AreEqual("application/json", response.ContentType);
				Assert.AreEqual("{\"type\":\"Node\"," +
					"\"name\":\"node_1\"," +
					"\"parent\":\"xpca://\"," +
					"\"children\":[\"xpca://node_1/node_2\"]}", 
					Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
				);
			});
		}
		
		[Test()]
		public void GetNodeJson()
		{
			Helper.Get("http://127.0.0.1:3001/node_1.json", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.AreEqual("application/json", response.ContentType);
				Assert.AreEqual("{\"type\":\"Node\"," +
					"\"name\":\"node_1\"," +
					"\"parent\":\"xpca://\"," +
					"\"children\":[\"xpca://node_1/node_2\"]}", 
					Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
				);
			});

		}
		
		[Test()]
		public void GetNodeXml()
		{
			Helper.Get("http://127.0.0.1:3001/node_1.xml", delegate(HttpWebResponse response) {
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
			});

		}
		
		[Test()]
		public void GetUnexpectedFormat()
		{
			Helper.Get("http://127.0.0.1:3001/node_1.xxx", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
			});
		}
		
		[Test()]
		public void GetUnexpectedNode()
		{
			Helper.Get("http://127.0.0.1:3001/node_xxx", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
			});
		}
	}
}

