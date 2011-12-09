using NUnit.Framework;
using System;
using System.IO;


namespace Galilei.Core.Test.Xpca
{
	
	[TestFixture]
	public class UXpcaXmlSerializer
	{
		private Serializer serializer;
		private Root root;
		
		[SetUp]
		public void SetUp()
		{
			root = new Root();
			root.Add("/", new Node("node_1"));
			root.Add("/node_1", new Node("node_2"));
			
			serializer = new XmlSerializer(typeof(Node));
		}
		
		
		[Test]
		public void TestSerializeToXML()
		{
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
				serializer.Serialize(root["/node_1"])
			);
		}
		
		[Test]
		public void TestSerializeToXMLOnlyConfig()
		{
			Assert.AreEqual(
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + 
				"<root>" +
					"<type>Node</type>" +
					"<name>node_1</name>" +
					"<parent>xpca://</parent>" +
				"</root>",
				serializer.Serialize(root["/node_1"], typeof(ConfigAttribute))
			);
		}
		
		
		[Test]
		public void TestDeserializeFromXML()
		{
			string data = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + 
				"<root>" +
					"<name>node_3</name>" +
					"<parent>xpca://node_1</parent>" +
				"</root>";
	
			
			root.Add("/", new Node("test"));
			
			Assert.IsNull(root["/node_1/node_3"]);
			serializer.Deserialize(data, root["/test"]);
			Assert.IsNotNull(root["/node_1/node_3"]);
		}
	}
}