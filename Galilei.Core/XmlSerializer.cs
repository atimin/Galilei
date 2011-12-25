using System;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Galilei.Core
{
	public class XmlSerializer : Serializer
	{
		public XmlSerializer(Type type) : base(type)
		{
		}
		
		public override string Serialize(Node node, Type typeAttr)
		{
			XpcaProxy proxy = new XpcaProxy(node);
			StringWriter sw = new StringWriter();
			
			using(XmlWriter xmlWriter = new XmlTextWriter(sw))
			{
				xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
				xmlWriter.WriteStartElement("root");
				foreach (KeyValuePair<string, PropertyInfo> property in proxy.GetPropertiesFor(typeAttr)) {
					
					object value = proxy[property.Key];
					if (value != null) {	
						xmlWriter.WriteStartElement(property.Key);	
						if(value is IEnumerable<object>) {								
							foreach (object obj in (value as IEnumerable<object>)) {
								xmlWriter.WriteStartElement("item");
								XmlWriteValue(xmlWriter, obj);
								xmlWriter.WriteEndElement();
							}
						
						}
						else {
							XmlWriteValue(xmlWriter, value);
						}
						xmlWriter.WriteEndElement();
					}
				}
				xmlWriter.WriteEndElement();
			}
			
			return sw.ToString();
		}
				
		public override void Deserialize(Node node, string data)
		{
			TextReader tr = new StringReader(data);
			Dictionary<string, object> properties = new Dictionary<string, object>();
			
			using(XmlReader xmlReader = new XmlTextReader(tr))
			{	
				xmlReader.ReadStartElement("root");
				do {
					if (xmlReader.NodeType == XmlNodeType.Element) {
						string name = xmlReader.Name;
						if (xmlReader.Read()) {
							switch (xmlReader.NodeType) {
							case XmlNodeType.Text:
								properties.Add(name, xmlReader.ReadContentAs(xmlReader.ValueType, null));
								break;
							case XmlNodeType.Element:
								List<object> objs = new List<object>();
								while(xmlReader.Read() && xmlReader.NodeType != XmlNodeType.EndElement) {
									if (xmlReader.Name == "item")
										objs.Add(xmlReader.ReadElementContentAs(xmlReader.ValueType, null));
								}
								properties.Add(name, objs.ToArray());
								break;
							}
							
						}
						
					}
				} while(xmlReader.Read());								
			}
			
			UpdateNode (node, properties);
		}
		
				
		private void XmlWriteValue(XmlWriter xmlWriter, object value)
		{
			switch (value.GetType().Name) {
			case "Char": 
				xmlWriter.WriteValue((char)value);
				break;
			case "Byte": 
				xmlWriter.WriteValue((byte)value);
				break;
			case "Int16":
				xmlWriter.WriteValue((short)value);
				break;
			case "Uint16":
				xmlWriter.WriteValue((ushort)value);
				break;
			case "DateTime":
				xmlWriter.WriteValue((DateTime)value);
				break;
			case "DateTimeOffset":
				xmlWriter.WriteValue((DateTimeOffset)value);
				break;
			case "SByte":
				xmlWriter.WriteValue((sbyte)value);
				break;
			case "Decimal":
				xmlWriter.WriteValue((decimal)value);
				break;
			case "UInt32":
				xmlWriter.WriteValue((uint)value);
				break;
			case "UInt64":
			case "Int64":
				xmlWriter.WriteValue((long)value);
				break;
			case "String":
				xmlWriter.WriteValue((string)value);
				break;
			case "Int32":
				xmlWriter.WriteValue((int)value);
				break;
			case "Double":
				xmlWriter.WriteValue((double)value);
				break;
			case "Boolean":
				xmlWriter.WriteValue((bool)value);
				break;
			case "Single":
				xmlWriter.WriteValue((float)value);
				break;
			default:
				if (value is Node) {
					xmlWriter.WriteValue("xpca:/" + ((Node)value).FullName);
				} 
				else {
					xmlWriter.WriteValue(value.ToString());
				}
			break;
			}
		}
	}
}

