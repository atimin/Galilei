using System;
using System.IO;
using System.Reflection;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Galilei.Core;

namespace Galilei
{
	public class Configurator
	{
		private string configPath;
		private Server srv;
		
		public Configurator (string configPath, Server srv)
		{
			this.configPath = configPath;
			this.srv = srv;
		}
		
		public void Save()
		{
			StreamWriter file = File.CreateText(configPath);
			using (JsonTextWriter jsonWriter = new JsonTextWriter(file))
			{
				jsonWriter.WriteStartObject();
				Save(srv, jsonWriter);
				jsonWriter.WriteEndObject();
				jsonWriter.Close();
			}
			file.Close();
		}
		
		public void Load()
		{
			StreamReader file = File.OpenText(configPath);
			using(JsonTextReader jsonReader = new JsonTextReader(file))
			{
				while(jsonReader.Read()) {
					if (jsonReader.TokenType == JsonToken.PropertyName) {
						string fullName = (string)jsonReader.Value;
						
						if (jsonReader.Read() 
						    && jsonReader.TokenType == JsonToken.StartObject) {
						
							NameValueCollection parms = new NameValueCollection();
							while(jsonReader.Read()
							      && jsonReader.TokenType != JsonToken.EndObject) {
						
								if (jsonReader.TokenType == JsonToken.PropertyName) {
									string key = (string)jsonReader.Value;
									
									if (jsonReader.Read()) {
										string value = (String)jsonReader.Value;
										parms.Add(key, value);
									}
									
								}
							}
							
							TreeBuilder builder = new TreeBuilder(srv);
							try {
								builder.Update(fullName, parms);
							} 
							catch (XpcaPathError) {
								builder.Build(fullName, parms);
							}
						}
					}
				}
			}
			file.Close();
		}
		
		private void Save(Node node, JsonTextWriter jsonWriter)
		{
			jsonWriter.Formatting = Formatting.Indented;
			jsonWriter.WritePropertyName(node.FullName);
			jsonWriter.WriteStartObject();
			XpcaProxy proxy = new XpcaProxy(node);
		
			foreach (KeyValuePair<string, PropertyInfo> property in proxy.GetPropertiesFor(typeof(ConfigAttribute))) {
				
				object value = proxy[property.Key];
					
				if (value != null ) {
					jsonWriter.WritePropertyName(property.Key);
					if(value is Node) {
						jsonWriter.WriteValue("xpca:/" + ((Node)value).FullName);
					}
					else {							
						jsonWriter.WriteValue(value.ToString());
					}
				}
			}
			
			jsonWriter.WriteEndObject();
			
			foreach(Node ch in node.Children) {
				Save(ch, jsonWriter);
			}
		}
	}
}

