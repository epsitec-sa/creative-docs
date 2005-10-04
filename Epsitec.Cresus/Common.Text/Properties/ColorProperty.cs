//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe ColorProperty définit la couleur à appliquer au corps
	/// du texte.
	/// </summary>
	public class ColorProperty : Property
	{
		public ColorProperty()
		{
		}
		
		public ColorProperty(Drawing.Color text_color)
		{
			this.text_color = text_color;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Color;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.ExtraSetting;
			}
		}
		
		
		public Drawing.Color					TextColor
		{
			get
			{
				return this.text_color;
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new ColorProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (Drawing.Color.ToString (this.text_color, System.Globalization.CultureInfo.InvariantCulture)));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			Drawing.Color text_color = Drawing.Color.Parse (SerializerSupport.DeserializeString (args[0]), System.Globalization.CultureInfo.InvariantCulture);
			
			this.text_color = text_color;
		}

		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.ColorProperty);
			
			ColorProperty a = this;
			ColorProperty b = property as ColorProperty;
			ColorProperty c = new ColorProperty ();
			
			c.text_color = b.text_color.IsEmpty ? a.text_color : b.text_color;
			
			c.DefineVersion (System.Math.Max (a.Version, b.Version));
			
			return c;
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.text_color.R);
			checksum.UpdateValue (this.text_color.G);
			checksum.UpdateValue (this.text_color.B);
			checksum.UpdateValue (this.text_color.A);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return ColorProperty.CompareEqualContents (this, value as ColorProperty);
		}
		
		
		private static bool CompareEqualContents(ColorProperty a, ColorProperty b)
		{
			return a.text_color == b.text_color;
		}
		
		
		private Drawing.Color					text_color;
	}
}
