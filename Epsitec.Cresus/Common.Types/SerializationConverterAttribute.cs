//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>TypeConverterAttribute</c> is used to specify which
	/// class to use in order to convert between a type and string
	/// (<see cref="T:ISerializationConverter"/>).
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Class | System.AttributeTargets.Struct, Inherited=false)]
	public class SerializationConverterAttribute : System.Attribute
	{
		public SerializationConverterAttribute(System.Type type)
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
		public ISerializationConverter			Converter
		{
			get
			{
				return System.Activator.CreateInstance (this.type) as ISerializationConverter;
			}
		}

		private System.Type type;
	}
}
