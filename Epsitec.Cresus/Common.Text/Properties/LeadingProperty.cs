//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La propriété LeadingProperty définit l'interligne (leading = bandes de
	/// plomb qui se rajoutaient entre les lignes de caractères), l'alignement
	/// sur une grille et les espacements avant/après un paragraphe.
	/// </summary>
	public class LeadingProperty : BaseProperty
	{
		public LeadingProperty(): this (double.NaN, SizeUnits.None, LeadingMode.Free)
		{
		}
		
		public LeadingProperty(double value, SizeUnits units, LeadingMode mode)
		{
			this.value = value;
			this.units = units;
			this.mode  = mode;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Leading;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.Style;
			}
		}
		
		
		public double							Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (NumberSupport.Different (this.value, value))
				{
					this.value = value;
					this.Invalidate ();
				}
			}
		}
		
		public double							SpaceBefore
		{
			get
			{
				return this.space_before;
			}
			set
			{
				if (NumberSupport.Different (this.space_before, value))
				{
					this.space_before = value;
					this.Invalidate ();
				}
			}
		}
		
		public double							SpaceAfter
		{
			get
			{
				return this.space_after;
			}
			set
			{
				if (NumberSupport.Different (this.space_after, value))
				{
					this.space_after = value;
					this.Invalidate ();
				}
			}
		}
		
		public double							PointValue
		{
			get
			{
				if (UnitsTools.IsAbsoluteSize (this.units))
				{
					return UnitsTools.ConvertToPoints (this.value, this.units);
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
			set
			{
				if (this.units != value)
				{
					this.units = value;
					this.Invalidate ();
				}
			}
		}
		
		public LeadingMode						Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				if (this.mode != value)
				{
					this.mode = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.value),
				/**/				SerializerSupport.SerializeDouble (this.space_before),
				/**/				SerializerSupport.SerializeDouble (this.space_after),
				/**/				SerializerSupport.SerializeSizeUnits (this.units),
				/**/				SerializerSupport.SerializeEnum (this.mode));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 5);
			
			double      value        = SerializerSupport.DeserializeDouble (args[0]);
			double      space_before = SerializerSupport.DeserializeDouble (args[1]);
			double      space_after  = SerializerSupport.DeserializeDouble (args[2]);
			SizeUnits   units        = SerializerSupport.DeserializeSizeUnits (args[3]);
			LeadingMode mode         = (LeadingMode) SerializerSupport.DeserializeEnum (typeof (LeadingMode), args[4]);
			
			this.value        = value;
			this.space_before = space_before;
			this.space_after  = space_after;
			this.units        = units;
			this.mode         = mode;
		}
		
		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			Debug.Assert.IsTrue (property is Properties.LeadingProperty);
			
			LeadingProperty a = this;
			LeadingProperty b = property as LeadingProperty;
			LeadingProperty c = new LeadingProperty ();
			
			UnitsTools.Combine (a.value,        a.units, b.value,        b.units, out c.value,        out c.units);
			UnitsTools.Combine (a.space_before, a.units, b.space_before, b.units, out c.space_before, out c.units);
			UnitsTools.Combine (a.space_after,  a.units, b.space_after,  b.units, out c.space_after,  out c.units);
			
			c.mode = b.mode == LeadingMode.Undefined ? a.mode : b.mode;
			
			return c;
		}
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.value);
			checksum.UpdateValue (this.space_before);
			checksum.UpdateValue (this.space_after);
			checksum.UpdateValue ((int) this.units);
			checksum.UpdateValue ((int) this.mode);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return LeadingProperty.CompareEqualContents (this, value as LeadingProperty);
		}
		
		
		private static bool CompareEqualContents(LeadingProperty a, LeadingProperty b)
		{
			return NumberSupport.Equal (a.value,        b.value)
				&& NumberSupport.Equal (a.space_before, b.space_before)
				&& NumberSupport.Equal (a.space_after,  b.space_after)
				&& a.units == b.units
				&& a.mode  == b.mode;
		}
		
		
		private double							value;
		private double							space_before;
		private double							space_after;
		private SizeUnits						units;
		private LeadingMode						mode;
	}
}
