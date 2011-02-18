//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[System.AttributeUsage (System.AttributeTargets.Assembly,
		/* */				AllowMultiple=true)]
	
	public class DependencyConverterAttribute : System.Attribute
	{
		public DependencyConverterAttribute(System.Type type)
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

		/// <summary>
		/// Gets or sets the converter associated with the type.
		/// </summary>
		/// <value>The type of the converter to use for serialization conversions.</value>
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

		public static IEnumerable<DependencyConverterAttribute> GetConverterAttributes(System.Reflection.Assembly assembly)
		{
			foreach (DependencyConverterAttribute attribute in assembly.GetCustomAttributes (typeof (DependencyConverterAttribute), false))
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
