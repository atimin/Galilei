using NUnit.Framework;
using System;
using Galilei.Core;

namespace Galilei.Core.Test.Xpca
{
	[Node]
	public class TestNode : Node
	{
		public bool isConfiged;
		public bool isChanged;
		private int prop;
		private int config;
		
		[Property]
		public int Prop {
			get { return prop; }
			set { prop = value; }
		}
		
		[Config]
		public int Config {
			get { return config; }
			set { config = value; }
		}
	}
	
	[TestFixture]
	public class UXpcaAccessor
	{
		private XpcaProxy proxy;
		private TestNode node;
		
		[SetUp]
		public void SetUp()
		{
			node = new TestNode();
			
			proxy = new XpcaProxy(node);
			proxy.ConfigChange += new EventHandler<ChangeEventArgs>(OnConfig);
		}
		
		[Test]
		public void TestGetValue()
		{
			Assert.AreEqual(proxy["name"], node.Name);
		}
		
		[Test]
		public void TestSetValue()
		{
			proxy["name"] = "new_name";
			Assert.AreEqual("new_name", node.Name);
		}
		
//		[Test]
//		public void TestSetValueWithOnChange()
//		{
//			Assert.IsFalse(node.isChanged);
//			Assert.IsFalse(node.isConfiged);
//			
//			proxy["prop"] = 1;
//			
//			Assert.AreEqual(1, node.Prop);
//			Assert.IsTrue(node.isChanged);
//			Assert.IsFalse(node.isConfiged);
//		}
		
		[Test]
		public void TestSetValueWithOnConfig()
		{
//			Assert.IsFalse(node.isChanged);
			Assert.IsFalse(node.isConfiged);
			
			proxy["config"] = 1;
			
			Assert.AreEqual(1, node.Config);
//			Assert.IsTrue(node.isChanged);
			Assert.IsTrue(node.isConfiged);
		}
		
		private void OnConfig(object sender, ChangeEventArgs e)
		{
			XpcaProxy proxy = sender as XpcaProxy;
			TestNode node = proxy.Node as TestNode;
			
			node.isConfiged = true;
		}
	}
}