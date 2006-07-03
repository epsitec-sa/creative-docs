//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[System.AttributeUsage (System.AttributeTargets.Assembly,
		/**/				AllowMultiple=true)]
	
	public class DependencyClassAttribute : System.Attribute
	{
		public DependencyClassAttribute(System.Type type)
		{
			this.type = type;
		}
		
		public System.Type						Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}
		
		public System.Type						Converter
		{
			get
			{
				return this.converter;
			}
			set
			{
				this.converter = value;
			}
		}

		public static IEnumerable<System.Type> GetRegisteredTypes(System.Reflection.Assembly assembly)
		{
			foreach (DependencyClassAttribute attribute in assembly.GetCustomAttributes (typeof (DependencyClassAttribute), false))
			{
				yield return attribute.Type;
			}
		}

		public static IEnumerable<DependencyClassAttribute> GetConverterAttributes(System.Reflection.Assembly assembly)
		{
			foreach (DependencyClassAttribute attribute in assembly.GetCustomAttributes (typeof (DependencyClassAttribute), false))
			{
				if (attribute.Converter != null)
				{
					yield return attribute;
				}
			}
		}

		private System.Type						type;
		private System.Type						converter;
	}
}
