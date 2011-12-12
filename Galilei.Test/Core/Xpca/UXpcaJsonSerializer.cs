using NUnit.Framework;
using System;
using System.IO;

namespace Galilei.Core.Test.Xpca
{
	
	[TestFixture]
	public class UXpcaJsonSerializer
	{
		private JsonSerializer serializer;
		private Root root;
		
		[SetUp]
		public void SetUp()
		{
			root = new Root();
			root["/"] = new Node("node_1");
			root["/node_1"] = new Node("node_2");
			
			serializer = new JsonSerializer(root["/node_1"]);
		}
		
		[Test]
		public void TestSerializeToJson()
		{
			
			Assert.AreEqual(
				"{\"type\":\"Node\"," +
				"\"name\":\"node_1\"," + 
				"\"parent\":\"xpca://\"," +
				"\"children\":[\"xpca://node_1/node_2\"]}",
				serializer.Serialize()
			);
		}
		
		[Test]
		public void TestSerializeToJsonOnlyConfig()
		{
			
			Assert.AreEqual(
				"{\"type\":\"Node\"," +
				"\"name\":\"node_1\"," +
				"\"parent\":\"xpca://\"}",
				serializer.Serialize(typeof(ConfigAttribute))
			);
		}

		[Test]
		public void TestDeserializeFromJson()
		{
			string data = "{\"name\":\"node_3\"," +
					"\"parent\":\"xpca://node_1\"}";
			
			root.Add("/", new Node("test"));
			
			Assert.IsNull(root["/node_1/node_3"]);
			serializer = new JsonSerializer(root["/test"]);
			serializer.Deserialize(data);
			Assert.IsNotNull(root["/node_1/node_3"]);
		}
	}
}