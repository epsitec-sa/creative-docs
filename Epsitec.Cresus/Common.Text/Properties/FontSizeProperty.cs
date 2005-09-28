//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe FontSizeProperty d�crit une taille de fonte.
	/// </summary>
	public class FontSizeProperty : Property
	{
		public FontSizeProperty() : this (double.NaN, SizeUnits.None)
		{
		}
		
		public FontSizeProperty(double size, SizeUnits units)
		{
			this.size  = size;
			this.units = units;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.FontSize;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.Style;
			}
		}
		
		
		public double							Size
		{
			get
			{
				return this.size;
			}
		}
		
		public double							SizeInPoints
		{
			get
			{
				if (UnitsTools.IsAbsoluteSize (this.units))
				{
					return UnitsTools.ConvertToPoints (this.size, this.units);
				}
				
				throw new System.InvalidOperationException ();
			}
		}
		
		public SizeUnits						Units
		{
			get
			{
				return this.units;
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new FontSizeProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.size),
				/**/				SerializerSupport.SerializeSizeUnits (this.units));
		}

		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 2);
			
			double    size  = SerializerSupport.DeserializeDouble (args[0]);
			SizeUnits units = SerializerSupport.DeserializeSizeUnits (args[1]);
			
			this.size  = size;
			this.units = units;
		}

		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.FontSizeProperty);
			
			FontSizeProperty a = this;
			FontSizeProperty b = property as FontSizeProperty;
			FontSizeProperty c = new FontSizeProperty ();
			
			UnitsTools.Combine (a.size, a.units, b.size, b.units, out c.size, out c.units);
			
			c.DefineVersion (System.Math.Max (a.Version, b.Version));
			
			return c;
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.size);
			checksum.UpdateValue ((int) this.units);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return FontSizeProperty.CompareEqualContents (this, value as FontSizeProperty);
		}
		
		
		private static bool CompareEqualContents(FontSizeProperty a, FontSizeProperty b)
		{
			return NumberSupport.Equal (a.size, b.size)
				&& a.units == b.units;
		}
		
		
		private double							size;
		private SizeUnits						units;
	}
}
