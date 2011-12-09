using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace Galilei.Core
{
	public class JsonSerializer : Serializer
	{
		public JsonSerializer(Type type) : base(type)
		{
		}
				
		public override string Serialize(Node node, Type type)
		{
			StringWriter sw = new StringWriter();
			using(JsonWriter jsonWriter = new JsonTextWriter(sw))
			{
				jsonWriter.Formatting = Newtonsoft.Json.Formatting.None;
				jsonWriter.WriteStartObject();
				jsonWriter.WritePropertyName("type");
				jsonWriter.WriteValue(accessor.Type);
				
				foreach (KeyValuePair<string, PropertyInfo> property in accessor.Properties) {
					if (!type.IsAssignableFrom(accessor.GetAttribute(property.Value).GetType()))
						continue;
					
					object val = property.Value.GetValue(node, null);
					
					if (val != null) {
						jsonWriter.WritePropertyName(property.Key);
						if(val is IEnumerable<object>) {
							jsonWriter.WriteStartArray();
							
							foreach (object obj in (val as IEnumerable<object>)) {
								JsonWriteValue(jsonWriter, obj);
							}
							
							jsonWriter.WriteEndArray();
						}
						else {							
							JsonWriteValue(jsonWriter, val);
						}
					}
				}
				jsonWriter.WriteEndObject();
			}
			
			return sw.ToString();
		}			
		
		public override void Deserialize(string data, Node node)
		{
			TextReader tr = new StringReader(data);
			Dictionary<string, object> properties = new Dictionary<string, object>();
			
			using(JsonTextReader jsonReader = new JsonTextReader(tr))
			{				
				while (jsonReader.Read()){
					if (jsonReader.TokenType == JsonToken.PropertyName) {
						string name = jsonReader.Value.ToString();
						if (jsonReader.Read()) {
							switch (jsonReader.TokenType) {
							case JsonToken.String:
							case JsonToken.Boolean:
							case JsonToken.Date:
							case JsonToken.Integer:
								properties.Add(name, jsonReader.Value);
								break;
							case JsonToken.StartArray:
								List<object> objs = new List<object>();
								while(jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray) {
									objs.Add(jsonReader.Value);
								}
								properties.Add(name, objs.ToArray());
								break;
							}
							
						}
						
					}
				}
			}
			
			UpdateNode (node, properties);
		}
		
		private void JsonWriteValue(JsonWriter jsonWriter, object value)
		{
			switch (value.GetType().Name) {
			case "Char": 
				jsonWriter.WriteValue((char)value);
				break;
			case "Byte": 
				jsonWriter.WriteValue((byte)value);
				break;
			case "Int16":
				jsonWriter.WriteValue((short)value);
				break;
			case "Uint16":
				jsonWriter.WriteValue((ushort)value);
				break;
			case "DateTime":
				jsonWriter.WriteValue((DateTime)value);
				break;
			case "DateTimeOffset":
				jsonWriter.WriteValue((DateTimeOffset)value);
				break;
			case "SByte":
				jsonWriter.WriteValue((sbyte)value);
				break;
			case "Decimal":
				jsonWriter.WriteValue((decimal)value);
				break;
			case "UInt32":
				jsonWriter.WriteValue((uint)value);
				break;
			case "Int64":
				jsonWriter.WriteValue((long)value);
				break;
			case "String":
				jsonWriter.WriteValue((string)value);
				break;
			case "Int32":
				jsonWriter.WriteValue((int)value);
				break;
			case "Double":
				jsonWriter.WriteValue((double)value);
				break;
			case "Boolean":
				jsonWriter.WriteValue((bool)value);
				break;
			case "UInt64":
				jsonWriter.WriteValue((ulong)value);
				break;
			case "Single":
				jsonWriter.WriteValue((float)value);
				break;
			default:
				if (value is Node) {
					jsonWriter.WriteValue("xpca:/" + ((Node)value).FullName);
				} 
				else {
					jsonWriter.WriteValue(value.ToString());
				}
			break;
			}
		}	
	}
}

