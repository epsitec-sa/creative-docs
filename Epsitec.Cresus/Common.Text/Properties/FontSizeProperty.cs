//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// Summary description for FontSizeProperty.
	/// </summary>
	public class FontSizeProperty : BaseProperty
	{
		public FontSizeProperty()
		{
		}
		
		public FontSizeProperty(double size, FontSizeUnits units)
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
			set
			{
				if (this.size != value)
				{
					this.size = value;
					this.Invalidate ();
				}
			}
		}
		
		public FontSizeUnits					Units
		{
			get
			{
				return this.units;
			}
			set
			{
				if (this.units != value)
				{
					this.units = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			//	TODO: ...
		}

		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			//	TODO: ...
		}

		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			Debug.Assert.IsTrue (property is Properties.FontSizeProperty);
			
			FontSizeProperty a = this;
			FontSizeProperty b = property as FontSizeProperty;
			FontSizeProperty c = new FontSizeProperty ();
			
			switch (b.Units)
			{
				case FontSizeUnits.Percent:				//	xxx * Percent --> xxx
					c.Units = a.Units;
					c.Size  = a.Size * b.Size;
					break;
				
				case FontSizeUnits.Points:				//	Points --> Points (écrase)
					c.Units = b.Units;
					c.Size  = b.Size;
					break;
				
				case FontSizeUnits.DeltaPoints:			//	Points + DeltaPoints --> Points, sinon erreur
					if (a.Units != FontSizeUnits.Points)
					{
						throw new System.InvalidOperationException ("Invalid units combination.");
					}
					c.Units = FontSizeUnits.Points;
					c.Size  = a.Size + b.Size;
					break;
				
				default:
					throw new System.InvalidOperationException ("Unsupported units.");
			}
			
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
			return a.size == b.size
				&& a.units == b.units;
		}
		
		
		private double							size;
		private FontSizeUnits					units;
	}
	
	public enum FontSizeUnits : byte
	{
		Points,
		DeltaPoints,
		Percent,
	}
}
