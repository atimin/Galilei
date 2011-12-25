using NUnit.Framework;
using System;
using Galilei.Core;
using System.Net;

namespace Galilei
{
	[TestFixture()]
	public class UPut
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
		public void RenameNode()
		{
			Helper.Put("http://127.0.0.1:3001/node_1/node_2", "name=new_name", delegate(HttpWebResponse response) {
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
			Helper.Put("http://127.0.0.1:3001/node_1/new_name", "parent=xpca://", delegate(HttpWebResponse response) {
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
		public void PutToUnexpectedPath()
		{
			Helper.Put("http://127.0.0.1:3001/node_xxx/new_node", "", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
			});
		}
		
		[Test()]
		public void PutToUnexpectedNode()
		{
			Helper.Put("http://127.0.0.1:3001/node_1/new_node", "", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
			});
		}
	}
}

