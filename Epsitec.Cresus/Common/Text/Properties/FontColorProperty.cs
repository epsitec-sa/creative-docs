//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe FontColorProperty définit la couleur à appliquer au corps
	/// du texte.
	/// </summary>
	public class FontColorProperty : Property
	{
		public FontColorProperty()
		{
		}
		
		public FontColorProperty(string textColor)
		{
			this.textColor = textColor;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.FontColor;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.ExtraSetting;
			}
		}
		
		
		public string							TextColor
		{
			get
			{
				return this.textColor;
			}
		}
		
		
		
		public override Property EmptyClone()
		{
			return new FontColorProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.textColor));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 1);
			
			string textColor = SerializerSupport.DeserializeString (args[0]);
			
			this.textColor = textColor;
		}

		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.FontColorProperty);
			
			FontColorProperty a = this;
			FontColorProperty b = property as FontColorProperty;
			FontColorProperty c = new FontColorProperty ();
			
			c.textColor = (b.textColor == null) ? a.textColor : b.textColor;
			
			return c;
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.textColor);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return FontColorProperty.CompareEqualContents (this, value as FontColorProperty);
		}
		
		
		private static bool CompareEqualContents(FontColorProperty a, FontColorProperty b)
		{
			return a.textColor == b.textColor;
		}
		
		
		private string							textColor;
	}
}
