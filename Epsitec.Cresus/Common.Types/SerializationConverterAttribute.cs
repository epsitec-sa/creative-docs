//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>TypeConverterAttribute</c> is used to specify which
	/// class to use in order to convert between a type and string
	/// (<see cref="ISerializationConverter"/>).
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Class | System.AttributeTargets.Struct, Inherited=false)]
	public class SerializationConverterAttribute : System.Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SerializationConverterAttribute"/> class.
		/// </summary>
		/// <param name="type">The type of the converter class.</param>
		public SerializationConverterAttribute(System.Type type)
		{
			this.type = type;
		}

		/// <summary>
		/// Gets the type of the converter class.
		/// </summary>
		/// <value>The type of the converter class.</value>
		public System.Type						ConverterType
		{
			get
			{
				return this.type;
			}
		}

		/// <summary>
		/// Gets the converter instance. This is a shared instance which is only
		/// allocated once for every class.
		/// </summary>
		/// <value>The converter instance.</value>
		public ISerializationConverter			Converter
		{
			get
			{
				if (this.converter == null)
				{
					lock (this)
					{
						if (this.converter == null)
						{
							this.converter = System.Activator.CreateInstance (this.type) as ISerializationConverter;
						}
					}
				}
				
				return this.converter;
			}
		}

		private System.Type type;
		private ISerializationConverter converter;
	}
}
