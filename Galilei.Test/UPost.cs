using NUnit.Framework;
using System;

using System.Net;

namespace Galilei
{
	[TestFixture()]
	public class UPost
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
		public void PostNode()
		{
			Helper.Post("http://127.0.0.1:3001/node_1/new_node", "", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
			});
				
			Helper.Get("http://127.0.0.1:3001/node_1/new_node", delegate(HttpWebResponse response) {
				
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.AreEqual("application/json", response.ContentType);
				Assert.AreEqual("{\"type\":\"Node\"," +
					"\"name\":\"new_node\"," +
					"\"parent\":\"xpca://node_1\"," +
					"\"children\":[]}", 
					Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
				);
			});
		}
		
		[Test()]
		public void RenameNode()
		{
			Helper.Post("http://127.0.0.1:3001/node_1/node_2", "name=new_name", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			});
			
			Helper.Get("http://127.0.0.1:3001/node_1/new_name", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.AreEqual("application/json", response.ContentType);
				Assert.AreEqual("{\"type\":\"Node\"," +
					"\"name\":\"new_name\"," +
					"\"parent\":\"xpca://node_1\"," +
					"\"children\":[]}", 
					Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
				);
			});
		}
		
		[Test()]
		public void ReplaceNode()
		{
			Helper.Post("http://127.0.0.1:3001/node_1/new_name", "parent=xpca://", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			});
			
			Helper.Get("http://127.0.0.1:3001/new_name", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.AreEqual("application/json", response.ContentType);
				Assert.AreEqual("{\"type\":\"Node\"," +
					"\"name\":\"new_name\"," +
					"\"parent\":\"xpca://\"," +
					"\"children\":[]}", 
					Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
				);
			});
		}
		
		[Test()]
		public void PostToUnexpectedPath()
		{
			Helper.Post("http://127.0.0.1:3001/node_xxx/new_node", "", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
			});
		}
		
		[Test()]
		public void PostWithTypes()
		{
			Helper.Post("http://127.0.0.1:3001/test_node_1", 
				"type=TestNode" 
				+ "&floatAttr=1.99" 
			    + "&intAttr=99999" 
			    + "&stringAttr=Hello World" 
			    + "&boolAttr=true", 
			    delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
			});
			
			Helper.Get("http://127.0.0.1:3001/test_node_1", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				Assert.AreEqual("application/json", response.ContentType);
				Assert.AreEqual("{\"type\":\"TestNode\"," +
					"\"name\":\"test_node_1\"," +
					"\"parent\":\"xpca://\"," +
					"\"children\":[]," +
					"\"floatAttr\":1.99," +
					"\"intAttr\":99999," +
					"\"stringAttr\":\"Hello World\"," +
					"\"boolAttr\":true}", 
					Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
				);
			});
		}
		
		[Test()]
		public void PostNotSupportTypes()
		{
			Helper.Post("http://127.0.0.1:3001/node_1/test_node_1", 
				"type=xxx", 
			    delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
				Assert.AreEqual("Type `xxx` is not XPCA type", response.StatusDescription);		
			});
		}
	}
}

