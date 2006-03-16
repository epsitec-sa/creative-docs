//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	[System.AttributeUsage (System.AttributeTargets.Class, Inherited=false)]
	public class TypeConverterAttribute : System.Attribute
	{
		public TypeConverterAttribute(System.Type type)
		{
			this.type = type;
		}

		public System.Type						ConverterType
		{
			get
			{
				return this.type;
			}
		}
		public ITypeConverter					Converter
		{
			get
			{
				return System.Activator.CreateInstance (this.type) as ITypeConverter;
			}
		}

		private System.Type type;
	}
}
