using NUnit.Framework;
using System;
using Galilei.Core;
using System.Net;

namespace Galilei
{
	[TestFixture()]
	public class UDelete
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
		public void DeleteNode()
		{
			Helper.Delete("http://127.0.0.1:3001/node_1", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			});
			
			Helper.Get("http://127.0.0.1:3001/node_1.json", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
			});
		}
		
		
		[Test()]
		public void GetUnexpectedNode()
		{
			Helper.Delete("http://127.0.0.1:3001/node_xxx", delegate(HttpWebResponse response) {
				Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
			});
		}
	}
}

