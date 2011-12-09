using NUnit.Framework;
using System;

namespace Galilei.Core.Test
{
	[TestFixture]
	public class URoot
	{
		private Root root;
		private Node node_1;
		private Node node_2;
		
		[SetUp]
		public void SetUp()
		{
			root = new Root();
			node_1 = new Node("node_1");
			root.Add("/", node_1);
			node_2 = new Node("node_2");
			root.Add("/node_1", node_2);
		}
		
		[Test]
		public void TestName()
		{
			Assert.AreEqual(root.Name, "/");
		}
		
		[Test]
		public void TestGet1()
		{
			Assert.AreEqual(root, root.Get("/"));
		}
		
		[Test]
		public void TestGet2()
		{
			Assert.AreEqual(node_1, root.Get("/node_1"));
		}
		
		[Test]
		public void TestGet3()
		{
			Assert.AreEqual(node_2, root.Get("/node_1/node_2"));
		}
		
		public void TestGet4()
		{
			Assert.IsNull(root.Get("/node_1/node_2/node_3"));
		}
		
		
		[Test]
		public void TestRemove()
		{
			root.Remove("/node_1/node_2");
			Assert.IsNull(root.Get("/node_1/node_2"));
		}
		
		[Test]
		public void TestAdd()
		{
			Node node_3 = new Node("node_3");
			root.Add("/node_1", node_3);
			Assert.AreEqual(node_3, root.Get ("/node_1/node_3"));
		}
		
		[Test] 
		public void TestIndexer()
		{
			Node node_3 = new Node("node_3");
			root["/node_1"] = node_3;
			Assert.AreEqual(node_3, root["/node_1/node_3"]);
			root["/node_1/node_3"] = null;
			Assert.IsNull(root["/node_1/node_3"]);
		}
		
		[Test]
		public void TestRoot()
		{
			Assert.AreEqual(root, root.Root);
			Assert.AreEqual(root, node_1.Root);
			Assert.AreEqual(root, node_2.Root);
		}
	}
}