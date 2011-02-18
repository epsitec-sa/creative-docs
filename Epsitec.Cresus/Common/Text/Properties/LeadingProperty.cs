//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La propriété LeadingProperty définit l'interligne (leading = bandes de
	/// plomb qui se rajoutaient entre les lignes de caractères), l'alignement
	/// sur une grille et les espacements avant/après un paragraphe.
	/// </summary>
	public class LeadingProperty : Property
	{
		public LeadingProperty() : this (double.NaN, SizeUnits.None, AlignMode.None)
		{
		}
		
		public LeadingProperty(double leading, SizeUnits leadingUnits, AlignMode alignMode)
		{
			this.leading       = leading;
			this.leadingUnits  = leadingUnits;
			this.alignMode     = alignMode;
			
			this.spaceBefore = double.NaN;
			this.spaceAfter  = double.NaN;
			
			this.spaceBeforeUnits = SizeUnits.None;
			this.spaceAfterUnits  = SizeUnits.None;
		}
		
		public LeadingProperty(double leading, SizeUnits leadingUnits, double spaceBefore, SizeUnits spaceBeforeUnits, double spaceAfter, SizeUnits spaceAfterUnits, AlignMode alignMode)
		{
			this.leading      = leading;
			this.spaceBefore  = spaceBefore;
			this.spaceAfter   = spaceAfter;
			
			this.leadingUnits      = leadingUnits;
			this.spaceBeforeUnits  = spaceBeforeUnits;
			this.spaceAfterUnits   = spaceAfterUnits;
			
			this.alignMode = alignMode;
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
				return this.spaceBefore;
			}
		}
		
		public double							SpaceAfter
		{
			get
			{
				return this.spaceAfter;
			}
		}
		
		
		public double							LeadingInPoints
		{
			get
			{
				if (UnitsTools.IsAbsoluteSize (this.leadingUnits))
				{
					return UnitsTools.ConvertToPoints (this.leading, this.leadingUnits);
				}
				
				throw new System.InvalidOperationException ();
			}
		}
		
		public double							SpaceBeforeInPoints
		{
			get
			{
				if (UnitsTools.IsAbsoluteSize (this.spaceBeforeUnits))
				{
					return UnitsTools.ConvertToPoints (this.spaceBefore, this.spaceBeforeUnits);
				}
				
				throw new System.InvalidOperationException ();
			}
		}
		
		public double							SpaceAfterInPoints
		{
			get
			{
				if (UnitsTools.IsAbsoluteSize (this.spaceAfterUnits))
				{
					return UnitsTools.ConvertToPoints (this.spaceAfter, this.spaceAfterUnits);
				}
				
				throw new System.InvalidOperationException ();
			}
		}
		
		
		public SizeUnits						LeadingUnits
		{
			get
			{
				return this.leadingUnits;
			}
		}
		
		public SizeUnits						SpaceBeforeUnits
		{
			get
			{
				return this.spaceBeforeUnits;
			}
		}
		
		public SizeUnits						SpaceAfterUnits
		{
			get
			{
				return this.spaceAfterUnits;
			}
		}
		
		
		public AlignMode						AlignMode
		{
			get
			{
				return this.alignMode;
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
				/**/				SerializerSupport.SerializeDouble (this.spaceBefore),
				/**/				SerializerSupport.SerializeDouble (this.spaceAfter),
				/**/				SerializerSupport.SerializeSizeUnits (this.leadingUnits),
				/**/				SerializerSupport.SerializeSizeUnits (this.spaceBeforeUnits),
				/**/				SerializerSupport.SerializeSizeUnits (this.spaceAfterUnits),
				/**/				SerializerSupport.SerializeEnum (this.alignMode));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 7);
			
			double    leading       = SerializerSupport.DeserializeDouble (args[0]);
			double    spaceBefore  = SerializerSupport.DeserializeDouble (args[1]);
			double    spaceAfter   = SerializerSupport.DeserializeDouble (args[2]);
			
			SizeUnits leadingUnits      = SerializerSupport.DeserializeSizeUnits (args[3]);
			SizeUnits spaceBeforeUnits  = SerializerSupport.DeserializeSizeUnits (args[4]);
			SizeUnits spaceAfterUnits   = SerializerSupport.DeserializeSizeUnits (args[5]);
			
			AlignMode alignMode = (AlignMode) SerializerSupport.DeserializeEnum (typeof (AlignMode), args[6]);
			
			this.leading      = leading;
			this.spaceBefore  = spaceBefore;
			this.spaceAfter   = spaceAfter;
			
			this.leadingUnits      = leadingUnits;
			this.spaceBeforeUnits  = spaceBeforeUnits;
			this.spaceAfterUnits   = spaceAfterUnits;
			
			this.alignMode   = alignMode;
		}
		
		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.LeadingProperty);
			
			LeadingProperty a = this;
			LeadingProperty b = property as LeadingProperty;
			LeadingProperty c = new LeadingProperty ();
			
			UnitsTools.Combine (a.leading,      a.leadingUnits,      b.leading,      b.leadingUnits,      out c.leading,      out c.leadingUnits);
			UnitsTools.Combine (a.spaceBefore, a.spaceBeforeUnits, b.spaceBefore, b.spaceBeforeUnits, out c.spaceBefore, out c.spaceBeforeUnits);
			UnitsTools.Combine (a.spaceAfter,  a.spaceAfterUnits,  b.spaceAfter,  b.spaceAfterUnits,  out c.spaceAfter,  out c.spaceAfterUnits);
			
			c.alignMode = b.alignMode == AlignMode.Undefined ? a.alignMode : b.alignMode;
			
			return c;
		}
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.leading);
			checksum.UpdateValue (this.spaceBefore);
			checksum.UpdateValue (this.spaceAfter);
			checksum.UpdateValue ((int) this.leadingUnits);
			checksum.UpdateValue ((int) this.spaceBeforeUnits);
			checksum.UpdateValue ((int) this.spaceAfterUnits);
			checksum.UpdateValue ((int) this.alignMode);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return LeadingProperty.CompareEqualContents (this, value as LeadingProperty);
		}
		
		
		private static bool CompareEqualContents(LeadingProperty a, LeadingProperty b)
		{
			return NumberSupport.Equal (a.leading,        b.leading)
				&& NumberSupport.Equal (a.spaceBefore, b.spaceBefore)
				&& NumberSupport.Equal (a.spaceAfter,  b.spaceAfter)
				&& a.leadingUnits      == b.leadingUnits
				&& a.spaceBeforeUnits == b.spaceBeforeUnits
				&& a.spaceAfterUnits  == b.spaceAfterUnits
				&& a.alignMode         == b.alignMode;
		}
		
		
		private double							leading;
		private SizeUnits						leadingUnits;
		
		private double							spaceBefore;
		private SizeUnits						spaceBeforeUnits;
		
		private double							spaceAfter;
		private SizeUnits						spaceAfterUnits;
		
		private AlignMode						alignMode;
	}
}
