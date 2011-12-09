using System;
using System.Reflection;
using System.Collections.Generic;

namespace Galilei.Core
{
	public class Accessor
	{
		private Type type;
		private string typeName;
		private Dictionary<string, PropertyInfo> properties;
		

		public Accessor(Type type)
		{
			this.type = type;
			
			NodeAttribute nodeAttr =  Array.Find(type.GetCustomAttributes(false), delegate (object attr) {
				return attr is NodeAttribute;
			}) as NodeAttribute;
			
			if (nodeAttr != null) {
				typeName = type.Name;
			}
			else {
				throw new Exception("Type: " + type.FullName + " is not XPCA node");
			}
			
			// Getting XPCA properties from type
			
			
			List<PropertyInfo> props = new List<PropertyInfo>();
			

			GetProperties(type, props);
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
		
		/// <summary>
		/// Gets the value from XPCA property
		/// </summary>
		/// <returns>
		/// The value.
		/// </returns>
		/// <param name='name'>
		/// Name of XPCA property
		/// </param>
		/// <param name='target'>
		/// Target node.
		/// </param>
		public object GetValue(string name, Node target)
		{
			return properties[name].GetValue(target, null);
		}
		
		/// <summary>
		/// Sets the value of XPCA property
		/// </summary>
		/// <param name='name'>
		/// Name of XPCA property.
		/// </param>
		/// <param name='target'>
		/// Target node.
		/// </param>
		/// <param name='value'>
		/// Value.
		/// </param>
		public void SetValue(string name, Node target, object value)
		{
			PropertyInfo prop = properties[name];
			if (prop.GetValue(target, null) != value) {
				prop.SetValue(target, value, null);
				target.OnChange();
				
				if (GetAttribute(prop) is ConfigAttribute) {
					target.OnConfig();
				}				
			}
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
	}
}