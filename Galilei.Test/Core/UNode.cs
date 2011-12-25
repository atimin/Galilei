using System;
using NUnit.Framework;

namespace Galilei.Core
{
	[TestFixture]
	public class UNode
	{
		private Node node;
		private Node parentNode1;
		private Node parentNode2;
		
		[SetUp]
		public void SetUp()
		{
			parentNode1 = new Node("parentNode1");
			parentNode2 = new Node("parentNode2");
			node = new Node("node");
			node.Parent = parentNode1;
		}
		[Test]
		public void TestName ()
		{
			Assert.AreEqual("node", node.Name);	
		}
		
		[Test]
		public void TestParent1()
		{
			Assert.AreEqual(node.Parent, parentNode1);
		}
		
		[Test]
		public void TestParent2()
		{
			node.Parent = parentNode2;
			Assert.AreEqual(node.Parent, parentNode2);
			Assert.AreEqual(parentNode1.Children, new Node[0] {});
			Assert.AreEqual(parentNode2.Children, new Node[1] { node });
		}
		
		
		[Test]
		public void TestChildren()
		{
			Node[] children = { node };
			Assert.AreEqual(children, parentNode1.Children);
		}
		
		[Test]
		public void TestFullName()
		{
			Assert.AreEqual("parentNode1/node", node.FullName);
		}
	}
}

