using System;
using System.Reflection;
using System.Collections.Generic;

namespace Galilei.Core
{
	public class XpcaProxy
	{
		private Node node;
		private string typeName;
		private Dictionary<string, PropertyInfo> properties;
		
		public event EventHandler<ChangeEventArgs> ConfigChange;

		public XpcaProxy(Node node)
		{
			Catch(node);
		}
		
		public void Catch(Node node)
		{
			this.node = node;
			
			NodeAttribute nodeAttr =  Array.Find(node.GetType().GetCustomAttributes(false), delegate (object attr) {
				return attr is NodeAttribute;
			}) as NodeAttribute;
			
			if (nodeAttr != null) {
				typeName = node.GetType().Name;
			}
			else {
				throw new XpcaTypeError(node.GetType().Name);
			}
			
			// Getting XPCA properties from type
			
			
			List<PropertyInfo> props = new List<PropertyInfo>();
			
	
			GetProperties(node.GetType(), props);
			props.RemoveAll(delegate(PropertyInfo p){
				return GetAttribute(p) == null;
			});
			
			properties = new Dictionary<string, PropertyInfo>();
			foreach (PropertyInfo property in props) {
				properties.Add(
					property.Name.Substring(0,1).ToLower() + property.Name.Substring(1),
					property
				);			
			}
		}

		public Node Node 
		{
			get { return this.node;	}
		}

		public object GetValue(string propertyName)
		{
			return properties[propertyName].GetValue(node, null);
		}

		public void SetValue(string propertyName, object value)
		{
			PropertyInfo prop = properties[propertyName];
			object lastValue = prop.GetValue(node, null);
			if (lastValue != value) {
				prop.SetValue(node, value, null);
				
				if (GetAttribute(prop) is ConfigAttribute) {
					if (ConfigChange != null) {
						OnConfig(propertyName, lastValue, value);
					}
				}				
			}
		}
		
		public object this [string propertyName] 
		{
			get { return GetValue(propertyName) ; }
			set { SetValue(propertyName, value); }
		}
		
		/// <summary>
		/// Gets the XPCA properties.
		/// </summary>
		/// <value>
		/// The XPCA properties.
		/// </value>
		public Dictionary<string, PropertyInfo> Properties
		{
			get { return properties; }
		}
		
		
		public string Type {
			get { return typeName; }
		}
		
		public PropertyAttribute GetAttribute(PropertyInfo property)
		{
			return Array.Find(property.GetCustomAttributes(true), delegate(object attr) {
				return attr is PropertyAttribute;
			}) as PropertyAttribute;
		}
		
		public Dictionary<string, PropertyInfo> GetPropertiesFor(Type typeAttr)
		{
			Dictionary<string, PropertyInfo> filtredProps = new Dictionary<string, PropertyInfo>();
			foreach (KeyValuePair<string, PropertyInfo> property in properties) {
				if (typeAttr.IsAssignableFrom( GetAttribute(property.Value).GetType())) {
					filtredProps.Add(property.Key, property.Value);
				}
			}
			return filtredProps;
		}
		
		private void GetProperties(Type type, List<PropertyInfo> props)
		{
			if (type != typeof(Node)){
				GetProperties(type.BaseType, props);
			}
			
			props.AddRange(type.GetProperties(
				BindingFlags.DeclaredOnly 
				| BindingFlags.Public
				| BindingFlags.Instance
			));
		}
		
		private void OnConfig(string propertyName, object lastValue, object newValue)
		{	
			EventHandler<ChangeEventArgs> handler = ConfigChange;
			
			if (handler != null) {
				ChangeEventArgs e = new ChangeEventArgs(propertyName, lastValue, newValue);
				handler(this, e);
			}
		}
	}
}