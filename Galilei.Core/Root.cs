using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace Galilei.Core
{
	/// <summary>
	/// Root of XPCA tree
	/// </summary>
	/// <exception cref='NotImplementedException'>
	/// Is thrown when a requested operation is not implemented for a given type.
	/// </exception>
	//
	public class Root : Node
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Galilei.Core.Root"/> class.
		/// </summary>
		public Root() : base("/")
		{
			root = this;
		}
		
		public void Add(string path, Node node)
		{
			node.Parent = Get(path);
		}
				
		public void Remove(string path)
		{
			Get(path).Parent = null;
		}
		
		public Node Get(string path) 
		{
			Queue<string> split_path = new Queue<string>(path.Split('/'));				
			Node node = this;
		
			// Search nodes by path
			while(split_path.Count > 0) {
				string name = split_path.Dequeue();
				
				if (name.Length > 0) {
					Node nextNode = null;
					foreach(Node child in node.Children) {
						if(child.Name == name) {
							nextNode = child;
							break;
						}
					}
					node = nextNode;
					if (node == null) {
						break;
					}
				}
			}
			
			return node;
		}
		
		public Node this [string path] {
			get {
				return Get(path);
			}
			set {
				if (value != null) {
					Add(path, value);					
				}
				else {
					Remove(path);
				}
			}
		}
	}
}