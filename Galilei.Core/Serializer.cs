using System;
using System.Collections.Generic;
using System.IO;

namespace Galilei.Core
{
	abstract public class Serializer
	{
		protected XpcaProxy proxy;
		
		public Serializer(Node node)
		{
			proxy = new XpcaProxy(node);
		}
		
		public string Serialize()
		{
			return Serialize(typeof(PropertyAttribute));
		}
		
		abstract public string Serialize(Type typeAttr);
		abstract public void Deserialize(string data);

		protected void UpdateNode (Dictionary<string, object> properties)
		{
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

