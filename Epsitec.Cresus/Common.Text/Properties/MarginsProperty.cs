//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe MarginsProperty définit les marges gauche/droite d'un bloc de
	/// texte, ainsi que sa justification (0 = aucune, 1 = 100%), son centrage
	/// (0 = aligné à gauche, 0.5 = centré, 1 = aligné à droite) et les réglages
	/// liés à la césure.
	/// </summary>
	public class MarginsProperty : Property
	{
		public MarginsProperty() : this (double.NaN, double.NaN, SizeUnits.None)
		{
		}
		
		public MarginsProperty(double left_margin, double right_margin, SizeUnits units)
		{
			this.left_margin_first_line  = left_margin;
			this.left_margin_body        = left_margin;
			this.right_margin_first_line = right_margin;
			this.right_margin_body       = right_margin;
			
			this.units = units;
			this.level = -1;
		}
		
		public MarginsProperty(double left_margin_first_line, double left_margin_body, double right_margin_first_line, double right_margin_body, SizeUnits units, double justification_body, double justification_last_line, double disposition, double break_fence_before, double break_fence_after, ThreeState enable_hyphenation) : this (left_margin_first_line, left_margin_body, right_margin_first_line, right_margin_body, units, justification_body, justification_last_line, disposition, break_fence_before, break_fence_after, enable_hyphenation, -1)
		{
		}
		
		public MarginsProperty(double left_margin_first_line, double left_margin_body, double right_margin_first_line, double right_margin_body, SizeUnits units, double justification_body, double justification_last_line, double disposition, double break_fence_before, double break_fence_after, ThreeState enable_hyphenation, int level)
		{
			this.left_margin_first_line  = left_margin_first_line;
			this.left_margin_body        = left_margin_body;
			this.right_margin_first_line = right_margin_first_line;
			this.right_margin_body       = right_margin_body;
			
			this.units = units;
			
			this.justification_body      = justification_body;
			this.justification_last_line = justification_last_line;
			this.disposition             = disposition;
			
			this.break_fence_before = break_fence_before;
			this.break_fence_after  = break_fence_after;
			this.enable_hyphenation = enable_hyphenation;
			
			this.level = level;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Margins;
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
		
		
		public double							LeftMarginFirstLine
		{
			get
			{
				return this.left_margin_first_line;
			}
		}
		
		public double							LeftMarginBody
		{
			get
			{
				return this.left_margin_body;
			}
		}
		
		public double							RightMarginFirstLine
		{
			get
			{
				return this.right_margin_first_line;
			}
		}
		
		public double							RightMarginBody
		{
			get
			{
				return this.right_margin_body;
			}
		}
		
		public SizeUnits						Units
		{
			get
			{
				return this.units;
			}
		}
		
		public double							JustificationBody
		{
			get
			{
				return this.justification_body;
			}
		}
		
		public double							JustificationLastLine
		{
			get
			{
				return this.justification_last_line;
			}
		}
		
		public double							Disposition
		{
			get
			{
				return this.disposition;
			}
		}
		
		public double							BreakFenceBefore
		{
			get
			{
				return this.break_fence_before;
			}
		}
		
		public double							BreakFenceAfter
		{
			get
			{
				return this.break_fence_after;
			}
		}
		
		public ThreeState						EnableHyphenation
		{
			get
			{
				return this.enable_hyphenation;
			}
		}
		
		public int								Level
		{
			get
			{
				return this.level;
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new MarginsProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.left_margin_first_line),
				/**/				SerializerSupport.SerializeDouble (this.left_margin_body),
				/**/				SerializerSupport.SerializeDouble (this.right_margin_first_line),
				/**/				SerializerSupport.SerializeDouble (this.right_margin_body),
				/**/				SerializerSupport.SerializeSizeUnits (this.units),
				/**/				SerializerSupport.SerializeDouble (this.justification_body),
				/**/				SerializerSupport.SerializeDouble (this.justification_last_line),
				/**/				SerializerSupport.SerializeDouble (this.disposition),
				/**/				SerializerSupport.SerializeDouble (this.break_fence_before),
				/**/				SerializerSupport.SerializeDouble (this.break_fence_after),
				/**/				SerializerSupport.SerializeThreeState (this.enable_hyphenation),
				/**/				SerializerSupport.SerializeInt (this.level));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 12);
			
			double     left_margin_first_line  = SerializerSupport.DeserializeDouble (args[0]);
			double     left_margin_body        = SerializerSupport.DeserializeDouble (args[1]);
			double     right_margin_first_line = SerializerSupport.DeserializeDouble (args[2]);
			double     right_margin_body       = SerializerSupport.DeserializeDouble (args[3]);
			SizeUnits  units                   = SerializerSupport.DeserializeSizeUnits (args[4]);
			double     justification_body      = SerializerSupport.DeserializeDouble (args[5]);
			double     justification_last_line = SerializerSupport.DeserializeDouble (args[6]);
			double     disposition             = SerializerSupport.DeserializeDouble (args[7]);
			double     break_fence_before      = SerializerSupport.DeserializeDouble (args[8]);
			double     break_fence_after       = SerializerSupport.DeserializeDouble (args[9]);
			ThreeState enable_hyphenation      = SerializerSupport.DeserializeThreeState (args[10]);
			int        level                   = SerializerSupport.DeserializeInt (args[11]);
			
			this.left_margin_first_line  = left_margin_first_line;
			this.left_margin_body        = left_margin_body;
			this.right_margin_first_line = right_margin_first_line;
			this.right_margin_body       = right_margin_body;
			this.units                   = units;
			this.justification_body      = justification_body;
			this.justification_last_line = justification_last_line;
			this.disposition             = disposition;
			this.break_fence_before      = break_fence_before;
			this.break_fence_after       = break_fence_after;
			this.enable_hyphenation      = enable_hyphenation;
			this.level                   = level;
		}
		
		
		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.MarginsProperty);
			
			MarginsProperty a = this;
			MarginsProperty b = property as MarginsProperty;
			MarginsProperty c = new MarginsProperty ();
			
			if (b.units != SizeUnits.None)
			{
				c.left_margin_first_line  = NumberSupport.Combine (UnitsTools.ConvertToSizeUnits (a.left_margin_first_line, a.units, b.units),  b.left_margin_first_line);
				c.left_margin_body        = NumberSupport.Combine (UnitsTools.ConvertToSizeUnits (a.left_margin_body, a.units, b.units),        b.left_margin_body);
				c.right_margin_first_line = NumberSupport.Combine (UnitsTools.ConvertToSizeUnits (a.right_margin_first_line, a.units, b.units), b.right_margin_first_line);
				c.right_margin_body       = NumberSupport.Combine (UnitsTools.ConvertToSizeUnits (a.right_margin_body, a.units, b.units),       b.right_margin_body);
				c.units                   = b.units;
			}
			else
			{
				c.left_margin_first_line  = a.left_margin_first_line;
				c.left_margin_body        = a.left_margin_body;
				c.right_margin_first_line = a.right_margin_first_line;
				c.right_margin_body       = a.right_margin_body;
				c.units                   = a.units;
			}
			
			c.justification_body      = NumberSupport.Combine (a.justification_body,      b.justification_body);
			c.justification_last_line = NumberSupport.Combine (a.justification_last_line, b.justification_last_line);
			c.disposition             = NumberSupport.Combine (a.disposition,             b.disposition);
			c.break_fence_before      = NumberSupport.Combine (a.break_fence_before,      b.break_fence_before);
			c.break_fence_after       = NumberSupport.Combine (a.break_fence_after,       b.break_fence_after);
			c.enable_hyphenation      = b.enable_hyphenation == ThreeState.Undefined ? a.enable_hyphenation : b.enable_hyphenation;
			c.level                   = b.level < 0 ? a.level : b.level;
			
			return c;
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.left_margin_first_line);
			checksum.UpdateValue (this.left_margin_body);
			checksum.UpdateValue (this.right_margin_first_line);
			checksum.UpdateValue (this.right_margin_body);
			checksum.UpdateValue ((int) this.units);
			checksum.UpdateValue (this.justification_body);
			checksum.UpdateValue (this.justification_last_line);
			checksum.UpdateValue (this.disposition);
			checksum.UpdateValue (this.break_fence_before);
			checksum.UpdateValue (this.break_fence_after);
			checksum.UpdateValue ((int) this.enable_hyphenation);
			checksum.UpdateValue (this.level);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return MarginsProperty.CompareEqualContents (this, value as MarginsProperty);
		}
		
		
		private static bool CompareEqualContents(MarginsProperty a, MarginsProperty b)
		{
			return NumberSupport.Equal (a.left_margin_first_line,  b.left_margin_first_line)
				&& NumberSupport.Equal (a.left_margin_body,        b.left_margin_body)
				&& NumberSupport.Equal (a.right_margin_first_line, b.right_margin_first_line)
				&& NumberSupport.Equal (a.right_margin_body,       b.right_margin_body)
				&& a.units == b.units
				&& NumberSupport.Equal (a.justification_body,      b.justification_body)
				&& NumberSupport.Equal (a.justification_last_line, b.justification_last_line)
				&& NumberSupport.Equal (a.disposition,             b.disposition)
				&& NumberSupport.Equal (a.break_fence_before,      b.break_fence_before)
				&& NumberSupport.Equal (a.break_fence_after,       b.break_fence_after)
				&& a.enable_hyphenation == b.enable_hyphenation
				&& a.level == b.level;
		}
		
		
		private double							left_margin_first_line;
		private double							left_margin_body;
		private double							right_margin_first_line;
		private double							right_margin_body;
		
		private double							break_fence_before;
		private double							break_fence_after;
		
		private SizeUnits						units;
		
		private ThreeState						enable_hyphenation;
		
		private double							justification_body;			//	0.0 = pas de justification, 1.0 = justification pleine
		private double							justification_last_line;
		private double							disposition;				//	0.0 = aligné à gauche, 0.5 = centré, 1.0 = aligné à droite
		
		private int								level;						//	-1 => pas de niveau spécifié
	}
}
