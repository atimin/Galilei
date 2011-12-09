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
		
		
		public override void OnChange()
		{
			isChanged = true;
		}
		
		public override void OnConfig ()
		{
			isConfiged = true;
		}
	}
	
	[TestFixture]
	public class UXpcaAccessor
	{
		private Accessor accessor;
		private TestNode node;
		
		[SetUp]
		public void SetUp()
		{
			accessor = new Accessor(typeof(TestNode));
			node = new TestNode();
		}
		
		[Test]
		public void TestGetValue()
		{
			Assert.AreEqual(accessor.GetValue("name", node), node.Name);
		}
		
		[Test]
		public void TestSetValue()
		{
			accessor.SetValue("name", node, "new_name");
			Assert.AreEqual("new_name", node.Name);
		}
		
		[Test]
		public void TestSetValueWithOnChange()
		{
			Assert.IsFalse(node.isChanged);
			Assert.IsFalse(node.isConfiged);
			
			accessor.SetValue("prop", node, 1);
			
			Assert.AreEqual(1, node.Prop);
			Assert.IsTrue(node.isChanged);
			Assert.IsFalse(node.isConfiged);
		}
		
		[Test]
		public void TestSetValueWithOnConfig()
		{
			Assert.IsFalse(node.isChanged);
			Assert.IsFalse(node.isConfiged);
			
			accessor.SetValue("config", node, 1);
			
			Assert.AreEqual(1, node.Config);
			Assert.IsTrue(node.isChanged);
			Assert.IsTrue(node.isConfiged);
		}
	}
}