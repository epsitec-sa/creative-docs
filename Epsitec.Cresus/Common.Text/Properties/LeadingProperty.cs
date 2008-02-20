//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La propri�t� LeadingProperty d�finit l'interligne (leading = bandes de
	/// plomb qui se rajoutaient entre les lignes de caract�res), l'alignement
	/// sur une grille et les espacements avant/apr�s un paragraphe.
	/// </summary>
	public class LeadingProperty : Property
	{
		public LeadingProperty() : this (double.NaN, SizeUnits.None, AlignMode.None)
		{
		}
		
		public LeadingProperty(double leading, SizeUnits leading_units, AlignMode align_mode)
		{
			this.leading       = leading;
			this.leading_units = leading_units;
			this.align_mode    = align_mode;
			
			this.space_before = double.NaN;
			this.space_after  = double.NaN;
			
			this.space_before_units = SizeUnits.None;
			this.space_after_units  = SizeUnits.None;
		}
		
		public LeadingProperty(double leading, SizeUnits leading_units, double space_before, SizeUnits space_before_units, double space_after, SizeUnits space_after_units, AlignMode align_mode)
		{
			this.leading      = leading;
			this.space_before = space_before;
			this.space_after  = space_after;
			
			this.leading_units      = leading_units;
			this.space_before_units = space_before_units;
			this.space_after_units  = space_after_units;
			
			this.align_mode = align_mode;
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
				return PropertyType.CoreSetting;
			}
		}
		
		public override bool					RequiresUniformParagraph
		{
			get
			{
				return true;
			}
		}
		
		
		public double							Leading
		{
			get
			{
				return this.leading;
			}
		}
		
		public double							SpaceBefore
		{
			get
			{
				return this.space_before;
			}
		}
		
		public double							SpaceAfter
		{
			get
			{
				return this.space_after;
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
		}
		
		public SizeUnits						SpaceBeforeUnits
		{
			get
			{
				return this.space_before_units;
			}
		}
		
		public SizeUnits						SpaceAfterUnits
		{
			get
			{
				return this.space_after_units;
			}
		}
		
		
		public AlignMode						AlignMode
		{
			get
			{
				return this.align_mode;
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new LeadingProperty ();
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
				/**/				SerializerSupport.SerializeEnum (this.align_mode));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 7);
			
			double    leading       = SerializerSupport.DeserializeDouble (args[0]);
			double    space_before  = SerializerSupport.DeserializeDouble (args[1]);
			double    space_after   = SerializerSupport.DeserializeDouble (args[2]);
			
			SizeUnits leading_units      = SerializerSupport.DeserializeSizeUnits (args[3]);
			SizeUnits space_before_units = SerializerSupport.DeserializeSizeUnits (args[4]);
			SizeUnits space_after_units  = SerializerSupport.DeserializeSizeUnits (args[5]);
			
			AlignMode align_mode = (AlignMode) SerializerSupport.DeserializeEnum (typeof (AlignMode), args[6]);
			
			this.leading      = leading;
			this.space_before = space_before;
			this.space_after  = space_after;
			
			this.leading_units      = leading_units;
			this.space_before_units = space_before_units;
			this.space_after_units  = space_after_units;
			
			this.align_mode   = align_mode;
		}
		
		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.LeadingProperty);
			
			LeadingProperty a = this;
			LeadingProperty b = property as LeadingProperty;
			LeadingProperty c = new LeadingProperty ();
			
			UnitsTools.Combine (a.leading,      a.leading_units,      b.leading,      b.leading_units,      out c.leading,      out c.leading_units);
			UnitsTools.Combine (a.space_before, a.space_before_units, b.space_before, b.space_before_units, out c.space_before, out c.space_before_units);
			UnitsTools.Combine (a.space_after,  a.space_after_units,  b.space_after,  b.space_after_units,  out c.space_after,  out c.space_after_units);
			
			c.align_mode = b.align_mode == AlignMode.Undefined ? a.align_mode : b.align_mode;
			
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
			checksum.UpdateValue ((int) this.align_mode);
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
				&& a.align_mode         == b.align_mode;
		}
		
		
		private double							leading;
		private SizeUnits						leading_units;
		
		private double							space_before;
		private SizeUnits						space_before_units;
		
		private double							space_after;
		private SizeUnits						space_after_units;
		
		private AlignMode						align_mode;
	}
}
