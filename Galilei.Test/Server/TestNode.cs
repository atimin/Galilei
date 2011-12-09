using System;

using Galilei.Core;

namespace Galilei.Test
{
	[Node]
	public class TestNode : Node
	{
		private float floatAttr;
		private int intAttr;
		private string stringAttr;
		private bool boolAttr;
		
		public TestNode() : this("test_node") {}
		
		public TestNode (string name) : base(name)
		{
			floatAttr = 0.0f;
			intAttr = 0;
			stringAttr = "";
			boolAttr = false;
		}
		
		[Property]
		public float FloatAttr
		{
			get { return floatAttr; }
			set { floatAttr = value; }
		}
		
		[Property]
		public int IntAttr
		{
			get { return intAttr; }
			set { intAttr = value; }
		}
		
		[Property]
		public string StringAttr
		{
			get { return stringAttr; }
			set { stringAttr = value; }
		}
		
		[Property]
		public bool BoolAttr
		{
			get { return boolAttr; }
			set { boolAttr = value; }
		}
	}
}


		