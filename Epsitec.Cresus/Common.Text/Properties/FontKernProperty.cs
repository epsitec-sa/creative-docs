//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe FontKernProperty modifie la largeur d'un caract�re (cr�nage
	/// manuel).
	/// </summary>
	public class FontKernProperty : Property
	{
		public FontKernProperty() : this (0, SizeUnits.Points)
		{
		}
		
		public FontKernProperty(double offset, SizeUnits units)
		{
			this.offset = offset;
			this.units  = units;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.FontKern;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.LocalSetting;
			}
		}
		
		
		public double							Offset
		{
			get
			{
				return this.offset;
			}
		}
		
		public SizeUnits						Units
		{
			get
			{
				return this.units;
			}
		}
		
		
		public double GetOffsetInPoints(double font_size_in_points)
		{
			if (UnitsTools.IsAbsoluteSize (this.units))
			{
				return UnitsTools.ConvertToPoints (this.offset, this.units);
			}
			if (UnitsTools.IsRelativeSize (this.units))
			{
				return UnitsTools.ConvertToPoints (this.offset, this.units) + font_size_in_points;
			}
			if (UnitsTools.IsScale (this.units))
			{
				return UnitsTools.ConvertToScale (this.offset, this.units) * font_size_in_points;
			}
			
			throw new System.InvalidOperationException ();
		}
		
		
		public override Property EmptyClone()
		{
			return new FontKernProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.offset),
				/**/				SerializerSupport.SerializeSizeUnits (this.units));
		}

		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 2);
			
			double    offset = SerializerSupport.DeserializeDouble (args[0]);
			SizeUnits units  = SerializerSupport.DeserializeSizeUnits (args[1]);
			
			this.offset = offset;
			this.units  = units;
		}

		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.FontKernProperty);
			
			FontKernProperty a = this;
			FontKernProperty b = property as FontKernProperty;
			FontKernProperty c = new FontKernProperty ();
			
			UnitsTools.Combine (a.offset, a.units, b.offset, b.units, out c.offset, out c.units);
			
			c.DefineVersion (System.Math.Max (a.Version, b.Version));
			
			return c;
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.offset);
			checksum.UpdateValue ((int) this.units);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return FontKernProperty.CompareEqualContents (this, value as FontKernProperty);
		}
		
		
		private static bool CompareEqualContents(FontKernProperty a, FontKernProperty b)
		{
			return NumberSupport.Equal (a.offset, b.offset)
				&& a.units == b.units;
		}
		
		
		private double							offset;
		private SizeUnits						units;
	}
}
