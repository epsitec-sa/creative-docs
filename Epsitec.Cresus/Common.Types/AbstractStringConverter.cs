//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe AbstractStringConverter fournit les supports de base pour une
	/// conversion de/vers le type String.
	/// </summary>
	public abstract class AbstractStringConverter : System.ComponentModel.TypeConverter
	{
		protected AbstractStringConverter()
		{
		}
		
		
		public abstract object ParseString(string value, System.Globalization.CultureInfo culture);
		public abstract string ToString(object value, System.Globalization.CultureInfo culture);
		
		public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
		{
			if (sourceType == typeof (string))
			{
				return true;
			}
				
			return base.CanConvertFrom (context, sourceType);
		}
			
		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
		{
			if (destinationType == typeof (string))
			{
				return true;
			}
				
			return base.CanConvertTo (context, destinationType);
		}
		
		public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			string text = value as string;
			
			if (text != null)
			{
				return this.ParseString (text, culture);
			}
			
			return base.ConvertFrom (context, culture, value);
		}
		
		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
		{
			if (destinationType == typeof (string))
			{
				return this.ToString (value, culture);
			}
			
			return base.ConvertTo (context, culture, value, destinationType);
		}
	}
}
