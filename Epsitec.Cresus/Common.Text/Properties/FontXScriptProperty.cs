//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe FontXScriptProperty définit si un superscript ou un subscript
	/// est requis (exposant/indice en français).
	/// </summary>
	public class FontXScriptProperty : Property
	{
		public FontXScriptProperty()
		{
		}
		
		public FontXScriptProperty(double scale, double offset)
		{
			this.scale   = scale;
			this.offset  = offset;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.FontXScript;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.Style;
			}
		}
		
		public override CombinationMode			CombinationMode
		{
			get
			{
				return CombinationMode.Combine;
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
		
		
		public override Property EmptyClone()
		{
			return new FontXScriptProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.scale),
				/**/				SerializerSupport.SerializeDouble (this.offset));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 2);
			
			double scale  = SerializerSupport.DeserializeDouble (args[0]);
			double offset = SerializerSupport.DeserializeDouble (args[1]);
			
			this.scale  = scale;
			this.offset = offset;
		}
		
		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.FontXScriptProperty);
			
			FontXScriptProperty a = this;
			FontXScriptProperty b = property as FontXScriptProperty;
			
			//	La combinaison de deux propriétés XScript donne toujours la
			//	deuxième comme vainqueur.
			
			return b;
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.scale);
			checksum.UpdateValue (this.offset);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return FontXScriptProperty.CompareEqualContents (this, value as FontXScriptProperty);
		}
		
		
		private static bool CompareEqualContents(FontXScriptProperty a, FontXScriptProperty b)
		{
			return a.scale == b.scale
				&& a.offset == b.offset;
		}
		
		
		private double							scale;
		private double							offset;
	}
}
