using System;
using System.Collections.Generic;
using System.IO;

namespace Galilei.Core
{
	abstract public class Serializer
	{
		protected Accessor accessor;
		
		public Serializer(Type type)
		{
			accessor = new Accessor(type);
		}
		
		public string Serialize(Node target)
		{
			return Serialize(target, 
				typeof(PropertyAttribute)
			);
		}
		
		abstract public string Serialize(Node node, Type type);
		abstract public void Deserialize(string data, Node node);

		protected void UpdateNode (Node node, Dictionary<string, object> properties)
		{
			//Set properties
			foreach (KeyValuePair<string, object> property in properties) {
				object val = property.Value;
				if (val.ToString().IndexOf("xpca:/") > -1) {
					val = node.Root[val.ToString().Replace("xpca:/", "")];
				}
				accessor.SetValue(property.Key, node, val);
			}
		}
	}
}

