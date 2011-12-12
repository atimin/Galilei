using System;
using System.Collections.Generic;

namespace Galilei.Core
{
	
	/// <summary>
	/// Elementary object for XPCA
	/// </summary>
	[Node("Elementary object for XPCA")]
	public class Node
	{
		protected string name;
		protected List<Node> children = new List<Node>();
		protected Node parent;
		protected Root root;
		
		[Config("Name of node")]
		public string Name 
		{
			get { return name; }
			set { name = value; }
		}
		
		[Config("Parent node")]
		public Node Parent 
		{
			get { return parent; }
			set 
			{ 
				if (value != null){
					if (parent != null) {
						parent.children.Remove(this);
					}
					parent = value; 
					parent.children.Add(this);
					root = parent.root;
				}
				else {
					parent.children.Remove(this);
					parent = null;
					root = null;
				}
			}
		}		
		
		[Property("Collection of children nodes")]
		public Node[] Children 
		{
			get { return children.ToArray(); }
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Galilei.Core.Node"/> class.
		/// </summary>
		public Node() : this("New node") {}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Galilei.Core.Node"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		public Node(string name)
		{
			this.name = name;
		}
		
		public Root Root {
			get { return root; }
		}
		
		public string FullName
		{
			get
			{
				string fullName;
				if (parent == null)
					fullName = name;
				else {
					if (parent is Root)
						fullName = parent.FullName + this.name;
					else 
						fullName = parent.FullName + "/" + this.name;
				}
				
				return fullName;
			}
		}
	}
}