using System;
using System.Collections.Generic;
using System.IO;

namespace Galilei.Core
{
	abstract public class Serializer
	{
		protected XpcaProxy proxy;
		protected Type type;
		
		public Serializer(Type type)
		{
			
		}
		
		public string Serialize(Node node)
		{
			return Serialize(node, typeof(PropertyAttribute));
		}
		
		abstract public string Serialize(Node node, Type typeAttr);
		abstract public void Deserialize(Node node, string data);

		protected void UpdateNode (Node node, Dictionary<string, object> properties)
		{
			proxy = new XpcaProxy(node);
			//Set properties
			foreach (KeyValuePair<string, object> property in properties) {
				object value = property.Value;
				if (value.ToString().IndexOf("xpca:/") > -1) {
					value = proxy.Node.Root[value.ToString().Replace("xpca:/", "")];
				}
				proxy[property.Key] = value;
			}
		}
	}
}

