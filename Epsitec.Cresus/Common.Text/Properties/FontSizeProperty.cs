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
		
		public double							PointSize
		{
			get
			{
				if (this.units == FontSizeUnits.Points)
				{
					return this.size;
				}
				
				throw new System.InvalidOperationException ();
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
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.size),
				/**/				SerializerSupport.SerializeEnum (this.units));
		}

		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 2);
			
			double        size  = SerializerSupport.DeserializeDouble (args[0]);
			FontSizeUnits units = (FontSizeUnits) SerializerSupport.DeserializeEnum (typeof (FontSizeUnits), args[1]);
			
			this.size  = size;
			this.units = units;
		}

		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			Debug.Assert.IsTrue (property is Properties.FontSizeProperty);
			
			FontSizeProperty a = this;
			FontSizeProperty b = property as FontSizeProperty;
			FontSizeProperty c = new FontSizeProperty ();
			
			FontSizeUnitsTools.Combine (a.size, a.units, b.size, b.units, out c.size, out c.units);
			
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
		private FontSizeUnits					units;
	}
}
