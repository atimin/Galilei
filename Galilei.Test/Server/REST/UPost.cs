using NUnit.Framework;
using System;
using Galilei;
using Galilei.Core;
using System.Net;

namespace Galilei.Test
{
	[TestFixture()]
	public class UPost
	{
		private Server srv;
		private Galilei galilei;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			srv = Helper.InitServer(3002);
	
			galilei = new Galilei(srv);
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
			HttpWebResponse response = Helper.Post("http://127.0.0.1:3002/node_1/new_node", "");
			
			Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
			
			response = Helper.Get("http://127.0.0.1:3002/node_1/new_node");
			
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("application/json", response.ContentType);
			Assert.AreEqual("{\"type\":\"Node\"," +
				"\"name\":\"new_node\"," +
				"\"parent\":\"xpca://node_1\"," +
				"\"children\":[]}", 
				Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
			);
		}
		
		[Test()]
		public void RenameNode()
		{
			HttpWebResponse response = Helper.Post("http://127.0.0.1:3002/node_1/node_2", "name=new_name");
			
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			
			response = Helper.Get("http://127.0.0.1:3002/node_1/new_name");
			
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("application/json", response.ContentType);
			Assert.AreEqual("{\"type\":\"Node\"," +
				"\"name\":\"new_name\"," +
				"\"parent\":\"xpca://node_1\"," +
				"\"children\":[]}", 
				Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
			);
		}
		
		[Test()]
		public void ReplaceNode()
		{
			HttpWebResponse response = Helper.Post("http://127.0.0.1:3002/node_1/new_name", "parent=xpca://");
			
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			
			response = Helper.Get("http://127.0.0.1:3002/new_name");
			
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("application/json", response.ContentType);
			Assert.AreEqual("{\"type\":\"Node\"," +
				"\"name\":\"new_name\"," +
				"\"parent\":\"xpca://\"," +
				"\"children\":[]}", 
				Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
			);
		}
		
		[Test()]
		public void PostToUnexpectedPath()
		{
			HttpWebResponse response = Helper.Post("http://127.0.0.1:3002/node_xxx/new_node", "");
			
			Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
		}
		
		[Test()]
		public void PostWithTypes()
		{
			HttpWebResponse response = Helper.Post("http://127.0.0.1:3002/test_node_1", 
				"type=TestNode" 
				+ "&floatAttr=1.99" 
			    + "&intAttr=99999" 
			    + "&stringAttr=Hello World" 
			    + "&boolAttr=true"
				);
			
			Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
			
			response = Helper.Get("http://127.0.0.1:3002/test_node_1");
			
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual("application/json", response.ContentType);
			Assert.AreEqual("{\"type\":\"TestNode\"," +
				"\"name\":\"test_node_1\"," +
				"\"parent\":\"xpca://test_node\"," +
				"\"children\":[]," +
				"\"floatAttr\":1.99," +
				"\"intAttr\":99999," +
				"\"stringAttr\":\"Hello World\"," +
				"\"boolAttr\":true}", 
				Helper.ReadFromStream(response.GetResponseStream(), response.ContentLength)
			);
		}
		
		[Test()]
		public void PostNotSupportTypes()
		{
			HttpWebResponse response = Helper.Post("http://127.0.0.1:3002/test_node/test_node_1", 
				"type=xxx");
			
			Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
			Assert.AreEqual("Type `xxx` is not supported", response.StatusDescription);			                                    			                                       			                                      
		}
	}
}

