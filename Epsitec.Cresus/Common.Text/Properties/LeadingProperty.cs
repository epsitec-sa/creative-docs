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
		public LeadingProperty() : this (double.NaN, SizeUnits.None, LeadingMode.Free)
		{
		}
		
		public LeadingProperty(double leading, SizeUnits leading_units, LeadingMode leading_mode)
		{
			this.leading       = leading;
			this.leading_units = leading_units;
			this.leading_mode  = leading_mode;
			
			this.space_before = double.NaN;
			this.space_after  = double.NaN;
			
			this.space_before_units = SizeUnits.None;
			this.space_after_units  = SizeUnits.None;
		}
		
		public LeadingProperty(double leading, SizeUnits leading_units, double space_before, SizeUnits space_before_units, double space_after, SizeUnits space_after_units, LeadingMode leading_mode)
		{
			this.leading      = leading;
			this.space_before = space_before;
			this.space_after  = space_after;
			
			this.leading_units      = leading_units;
			this.space_before_units = space_before_units;
			this.space_after_units  = space_after_units;
			
			this.leading_mode = leading_mode;
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
		
		
		public double							Leading
		{
			get
			{
				return this.leading;
			}
			set
			{
				if (NumberSupport.Different (this.leading, value))
				{
					this.leading = value;
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
		
		
		public double							LeadingInPoints
		{
			get
			{
				if (UnitsTools.IsAbsoluteSize (this.leading_units))
				{
					return UnitsTools.ConvertToPoints (this.leading, this.leading_units);
				}
				
				throw new System.InvalidOperationException ();
			}
		}
		
		public double							SpaceBeforeInPoints
		{
			get
			{
				if (UnitsTools.IsAbsoluteSize (this.space_before_units))
				{
					return UnitsTools.ConvertToPoints (this.space_before, this.space_before_units);
				}
				
				throw new System.InvalidOperationException ();
			}
		}
		
		public double							SpaceAfterInPoints
		{
			get
			{
				if (UnitsTools.IsAbsoluteSize (this.space_after_units))
				{
					return UnitsTools.ConvertToPoints (this.space_after, this.space_after_units);
				}
				
				throw new System.InvalidOperationException ();
			}
		}
		
		
		public SizeUnits						LeadingUnits
		{
			get
			{
				return this.leading_units;
			}
			set
			{
				if (this.leading_units != value)
				{
					this.leading_units = value;
					this.Invalidate ();
				}
			}
		}
		
		public SizeUnits						SpaceBeforeUnits
		{
			get
			{
				return this.space_before_units;
			}
			set
			{
				if (this.space_before_units != value)
				{
					this.space_before_units = value;
					this.Invalidate ();
				}
			}
		}
		
		public SizeUnits						SpaceAfterUnits
		{
			get
			{
				return this.space_after_units;
			}
			set
			{
				if (this.space_after_units != value)
				{
					this.space_after_units = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public LeadingMode						LeadingMode
		{
			get
			{
				return this.leading_mode;
			}
			set
			{
				if (this.leading_mode != value)
				{
					this.leading_mode = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.leading),
				/**/				SerializerSupport.SerializeDouble (this.space_before),
				/**/				SerializerSupport.SerializeDouble (this.space_after),
				/**/				SerializerSupport.SerializeSizeUnits (this.leading_units),
				/**/				SerializerSupport.SerializeSizeUnits (this.space_before_units),
				/**/				SerializerSupport.SerializeSizeUnits (this.space_after_units),
				/**/				SerializerSupport.SerializeEnum (this.leading_mode));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 7);
			
			double      leading       = SerializerSupport.DeserializeDouble (args[0]);
			double      space_before  = SerializerSupport.DeserializeDouble (args[1]);
			double      space_after   = SerializerSupport.DeserializeDouble (args[2]);
			
			SizeUnits   leading_units      = SerializerSupport.DeserializeSizeUnits (args[3]);
			SizeUnits   space_before_units = SerializerSupport.DeserializeSizeUnits (args[4]);
			SizeUnits   space_after_units  = SerializerSupport.DeserializeSizeUnits (args[5]);
			
			LeadingMode leading_mode = (LeadingMode) SerializerSupport.DeserializeEnum (typeof (LeadingMode), args[6]);
			
			this.leading       = leading;
			this.space_before  = space_before;
			this.space_after   = space_after;
			
			this.leading_units      = leading_units;
			this.space_before_units = space_before_units;
			this.space_after_units  = space_after_units;
			
			this.leading_mode  = leading_mode;
		}
		
		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			Debug.Assert.IsTrue (property is Properties.LeadingProperty);
			
			LeadingProperty a = this;
			LeadingProperty b = property as LeadingProperty;
			LeadingProperty c = new LeadingProperty ();
			
			UnitsTools.Combine (a.leading,      a.leading_units,      b.leading,      b.leading_units,      out c.leading,      out c.leading_units);
			UnitsTools.Combine (a.space_before, a.space_before_units, b.space_before, b.space_before_units, out c.space_before, out c.space_before_units);
			UnitsTools.Combine (a.space_after,  a.space_after_units,  b.space_after,  b.space_after_units,  out c.space_after,  out c.space_after_units);
			
			c.leading_mode = b.leading_mode == LeadingMode.Undefined ? a.leading_mode : b.leading_mode;
			
			return c;
		}
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.leading);
			checksum.UpdateValue (this.space_before);
			checksum.UpdateValue (this.space_after);
			checksum.UpdateValue ((int) this.leading_units);
			checksum.UpdateValue ((int) this.space_before_units);
			checksum.UpdateValue ((int) this.space_after_units);
			checksum.UpdateValue ((int) this.leading_mode);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return LeadingProperty.CompareEqualContents (this, value as LeadingProperty);
		}
		
		
		private static bool CompareEqualContents(LeadingProperty a, LeadingProperty b)
		{
			return NumberSupport.Equal (a.leading,        b.leading)
				&& NumberSupport.Equal (a.space_before, b.space_before)
				&& NumberSupport.Equal (a.space_after,  b.space_after)
				&& a.leading_units      == b.leading_units
				&& a.space_before_units == b.space_before_units
				&& a.space_after_units  == b.space_after_units
				&& a.leading_mode       == b.leading_mode;
		}
		
		
		private double							leading;
		private SizeUnits						leading_units;
		
		private double							space_before;
		private SizeUnits						space_before_units;
		
		private double							space_after;
		private SizeUnits						space_after_units;
		
		private LeadingMode						leading_mode;
	}
}
