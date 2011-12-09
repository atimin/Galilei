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
			
			serializer = new JsonSerializer(typeof(Node));
		}
		
		[Test]
		public void TestSerializeToJson()
		{
			
			Assert.AreEqual(
				"{\"type\":\"Node\"," +
				"\"name\":\"node_1\"," + 
				"\"parent\":\"xpca://\"," +
				"\"children\":[\"xpca://node_1/node_2\"]}",
				serializer.Serialize(root["/node_1"])
			);
		}
		
		[Test]
		public void TestSerializeToJsonOnlyConfig()
		{
			
			Assert.AreEqual(
				"{\"type\":\"Node\"," +
				"\"name\":\"node_1\"," +
				"\"parent\":\"xpca://\"}",
				serializer.Serialize(root["/node_1"], typeof(ConfigAttribute))
			);
		}

		[Test]
		public void TestDeserializeFromJson()
		{
			string data = "{\"name\":\"node_3\"," +
					"\"parent\":\"xpca://node_1\"}";
			
			root.Add("/", new Node("test"));
			
			Assert.IsNull(root["/node_1/node_3"]);
			serializer.Deserialize(data, root["/test"]);
			Assert.IsNotNull(root["/node_1/node_3"]);
		}
	}
}