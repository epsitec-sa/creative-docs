//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe FontXscriptProperty définit si un superscript ou un subscript
	/// est requis (exposant/indice en français).
	/// </summary>
	public class FontXscriptProperty : Property
	{
		public FontXscriptProperty()
		{
		}
		
		public FontXscriptProperty(double scale, double offset)
		{
			this.scale   = scale;
			this.offset  = offset;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.FontXscript;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.CoreSetting;
			}
		}
		
		public override CombinationMode			CombinationMode
		{
			get
			{
				return CombinationMode.Combine;
			}
		}
		
		
		public bool								IsDisabled
		{
			get
			{
				return this.isDisabled;
			}
		}
		
		private bool							IsEmpty
		{
			get
			{
				return double.IsNaN (this.scale) && double.IsNaN (this.offset);
			}
		}
		
		
		public double							Scale
		{
			get
			{
				return this.scale;
			}
		}
		
		public double							Offset
		{
			get
			{
				return this.offset;
			}
		}
		
		
		public static FontXscriptProperty		DisableOverride
		{
			get
			{
				FontXscriptProperty value = new FontXscriptProperty ();
				
				value.isDisabled = true;
				value.scale       = double.NaN;
				value.offset      = double.NaN;
				
				return value;
			}
		}
		
		public override Property EmptyClone()
		{
			return new FontXscriptProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeBoolean (this.isDisabled),
				/**/				SerializerSupport.SerializeDouble (this.scale),
				/**/				SerializerSupport.SerializeDouble (this.offset));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 3);
			
			bool   isDisabled  = SerializerSupport.DeserializeBoolean (args[0]);
			double scale       = SerializerSupport.DeserializeDouble (args[1]);
			double offset      = SerializerSupport.DeserializeDouble (args[2]);
			
			this.isDisabled  = isDisabled;
			this.scale       = scale;
			this.offset      = offset;
		}
		
		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.FontXscriptProperty);
			
			FontXscriptProperty a = this;
			FontXscriptProperty b = property as FontXscriptProperty;
			
			if (b.isDisabled)
			{
				FontXscriptProperty c = new FontXscriptProperty ();
				
				c.isDisabled  = true;
				c.scale       = a.scale;
				c.offset      = a.offset;
				
				return c;
			}
			else
			{
				//	La combinaison de deux propriétés XScript normales donne
				//	toujours la deuxième comme vainqueur.
				
				return b;
			}
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.isDisabled);
			checksum.UpdateValue (this.scale);
			checksum.UpdateValue (this.offset);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return FontXscriptProperty.CompareEqualContents (this, value as FontXscriptProperty);
		}
		
		
		private static bool CompareEqualContents(FontXscriptProperty a, FontXscriptProperty b)
		{
			return a.isDisabled == b.isDisabled
				&& NumberSupport.Equal (a.scale, b.scale)
				&& NumberSupport.Equal (a.offset, b.offset);
		}
		
		
		private bool							isDisabled;
		private double							scale;
		private double							offset;
	}
}
