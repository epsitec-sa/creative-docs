//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public MarginsProperty(double leftMargin, double rightMargin, SizeUnits units)
		{
			System.Diagnostics.Debug.Assert (UnitsTools.IsScale (units) == false);
			
			this.leftMarginFirstLine   = leftMargin;
			this.leftMarginBody        = leftMargin;
			this.rightMarginFirstLine  = rightMargin;
			this.rightMarginBody       = rightMargin;
			
			this.units = units;
			this.level = -1;
			this.levelAttribute = null;
		}
		
		public MarginsProperty(double leftMarginFirstLine, double leftMarginBody, double rightMarginFirstLine, double rightMarginBody, SizeUnits units, double justificationBody, double justificationLastLine, double disposition, double breakFenceBefore, double breakFenceAfter, ThreeState enableHyphenation) : this (leftMarginFirstLine, leftMarginBody, rightMarginFirstLine, rightMarginBody, units, justificationBody, justificationLastLine, disposition, breakFenceBefore, breakFenceAfter, enableHyphenation, -1, null)
		{
		}
		
		public MarginsProperty(double leftMarginFirstLine, double leftMarginBody, double rightMarginFirstLine, double rightMarginBody, SizeUnits units, double justificationBody, double justificationLastLine, double disposition, double breakFenceBefore, double breakFenceAfter, ThreeState enableHyphenation, int level, string levelAttribute)
		{
			System.Diagnostics.Debug.Assert (UnitsTools.IsScale (units) == false);
			
			this.leftMarginFirstLine   = leftMarginFirstLine;
			this.leftMarginBody        = leftMarginBody;
			this.rightMarginFirstLine  = rightMarginFirstLine;
			this.rightMarginBody       = rightMarginBody;
			
			this.units = units;
			
			this.justificationBody       = justificationBody;
			this.justificationLastLine   = justificationLastLine;
			this.disposition              = disposition;
			
			this.breakFenceBefore  = breakFenceBefore;
			this.breakFenceAfter   = breakFenceAfter;
			this.enableHyphenation = enableHyphenation;
			
			this.level = level;
			this.levelAttribute = levelAttribute;
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
				return this.leftMarginFirstLine;
			}
		}
		
		public double							LeftMarginBody
		{
			get
			{
				return this.leftMarginBody;
			}
		}
		
		public double							RightMarginFirstLine
		{
			get
			{
				return this.rightMarginFirstLine;
			}
		}
		
		public double							RightMarginBody
		{
			get
			{
				return this.rightMarginBody;
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
				return this.justificationBody;
			}
		}
		
		public double							JustificationLastLine
		{
			get
			{
				return this.justificationLastLine;
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
				return this.breakFenceBefore;
			}
		}
		
		public double							BreakFenceAfter
		{
			get
			{
				return this.breakFenceAfter;
			}
		}
		
		public ThreeState						EnableHyphenation
		{
			get
			{
				return this.enableHyphenation;
			}
		}
		
		public int								Level
		{
			get
			{
				return this.level;
			}
		}
		
		public string							LevelAttribute
		{
			get
			{
				return this.levelAttribute;
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new MarginsProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.leftMarginFirstLine),
				/**/				SerializerSupport.SerializeDouble (this.leftMarginBody),
				/**/				SerializerSupport.SerializeDouble (this.rightMarginFirstLine),
				/**/				SerializerSupport.SerializeDouble (this.rightMarginBody),
				/**/				SerializerSupport.SerializeSizeUnits (this.units),
				/**/				SerializerSupport.SerializeDouble (this.justificationBody),
				/**/				SerializerSupport.SerializeDouble (this.justificationLastLine),
				/**/				SerializerSupport.SerializeDouble (this.disposition),
				/**/				SerializerSupport.SerializeDouble (this.breakFenceBefore),
				/**/				SerializerSupport.SerializeDouble (this.breakFenceAfter),
				/**/				SerializerSupport.SerializeThreeState (this.enableHyphenation),
				/**/				SerializerSupport.SerializeInt (this.level),
				/**/				SerializerSupport.SerializeString (this.levelAttribute));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue ((args.Length == 12) || (args.Length == 13));
			
			double     leftMarginFirstLine   = SerializerSupport.DeserializeDouble (args[0]);
			double     leftMarginBody        = SerializerSupport.DeserializeDouble (args[1]);
			double     rightMarginFirstLine  = SerializerSupport.DeserializeDouble (args[2]);
			double     rightMarginBody       = SerializerSupport.DeserializeDouble (args[3]);
			SizeUnits  units                 = SerializerSupport.DeserializeSizeUnits (args[4]);
			double     justificationBody     = SerializerSupport.DeserializeDouble (args[5]);
			double     justificationLastLine = SerializerSupport.DeserializeDouble (args[6]);
			double     disposition           = SerializerSupport.DeserializeDouble (args[7]);
			double     breakFenceBefore      = SerializerSupport.DeserializeDouble (args[8]);
			double     breakFenceAfter       = SerializerSupport.DeserializeDouble (args[9]);
			ThreeState enableHyphenation     = SerializerSupport.DeserializeThreeState (args[10]);
			int        level                 = SerializerSupport.DeserializeInt (args[11]);
			string     levelAttribute        = (args.Length < 13) ? null : SerializerSupport.DeserializeString (args[12]);
			
			this.leftMarginFirstLine   = leftMarginFirstLine;
			this.leftMarginBody        = leftMarginBody;
			this.rightMarginFirstLine  = rightMarginFirstLine;
			this.rightMarginBody       = rightMarginBody;
			this.units                 = units;
			this.justificationBody     = justificationBody;
			this.justificationLastLine = justificationLastLine;
			this.disposition           = disposition;
			this.breakFenceBefore      = breakFenceBefore;
			this.breakFenceAfter       = breakFenceAfter;
			this.enableHyphenation     = enableHyphenation;
			this.level                 = level;
			this.levelAttribute        = levelAttribute;
		}
		
		
		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.MarginsProperty);
			
			MarginsProperty a = this;
			MarginsProperty b = property as MarginsProperty;
			MarginsProperty c = new MarginsProperty ();
			
			if (b.units != SizeUnits.None)
			{
				c.leftMarginFirstLine   = NumberSupport.Combine (UnitsTools.ConvertToSizeUnits (a.leftMarginFirstLine, a.units, b.units),  b.leftMarginFirstLine);
				c.leftMarginBody        = NumberSupport.Combine (UnitsTools.ConvertToSizeUnits (a.leftMarginBody, a.units, b.units),        b.leftMarginBody);
				c.rightMarginFirstLine  = NumberSupport.Combine (UnitsTools.ConvertToSizeUnits (a.rightMarginFirstLine, a.units, b.units), b.rightMarginFirstLine);
				c.rightMarginBody       = NumberSupport.Combine (UnitsTools.ConvertToSizeUnits (a.rightMarginBody, a.units, b.units),       b.rightMarginBody);
				c.units                 = b.units;
			}
			else
			{
				c.leftMarginFirstLine   = a.leftMarginFirstLine;
				c.leftMarginBody        = a.leftMarginBody;
				c.rightMarginFirstLine  = a.rightMarginFirstLine;
				c.rightMarginBody       = a.rightMarginBody;
				c.units                 = a.units;
			}
			
			c.justificationBody      = NumberSupport.Combine (a.justificationBody,      b.justificationBody);
			c.justificationLastLine  = NumberSupport.Combine (a.justificationLastLine, b.justificationLastLine);
			c.disposition            = NumberSupport.Combine (a.disposition,             b.disposition);
			c.breakFenceBefore       = NumberSupport.Combine (a.breakFenceBefore,      b.breakFenceBefore);
			c.breakFenceAfter        = NumberSupport.Combine (a.breakFenceAfter,       b.breakFenceAfter);
			c.enableHyphenation      = b.enableHyphenation == ThreeState.Undefined ? a.enableHyphenation : b.enableHyphenation;
			c.level                  = b.level < 0 ? a.level : b.level;
			c.levelAttribute         = b.levelAttribute == null ? a.levelAttribute : b.levelAttribute;
			
			return c;
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.leftMarginFirstLine);
			checksum.UpdateValue (this.leftMarginBody);
			checksum.UpdateValue (this.rightMarginFirstLine);
			checksum.UpdateValue (this.rightMarginBody);
			checksum.UpdateValue ((int) this.units);
			checksum.UpdateValue (this.justificationBody);
			checksum.UpdateValue (this.justificationLastLine);
			checksum.UpdateValue (this.disposition);
			checksum.UpdateValue (this.breakFenceBefore);
			checksum.UpdateValue (this.breakFenceAfter);
			checksum.UpdateValue ((int) this.enableHyphenation);
			checksum.UpdateValue (this.level);
			checksum.UpdateValue (this.levelAttribute);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return MarginsProperty.CompareEqualContents (this, value as MarginsProperty);
		}
		
		
		private static bool CompareEqualContents(MarginsProperty a, MarginsProperty b)
		{
			return NumberSupport.Equal (a.leftMarginFirstLine,  b.leftMarginFirstLine)
				&& NumberSupport.Equal (a.leftMarginBody,        b.leftMarginBody)
				&& NumberSupport.Equal (a.rightMarginFirstLine, b.rightMarginFirstLine)
				&& NumberSupport.Equal (a.rightMarginBody,       b.rightMarginBody)
				&& a.units == b.units
				&& NumberSupport.Equal (a.justificationBody,      b.justificationBody)
				&& NumberSupport.Equal (a.justificationLastLine, b.justificationLastLine)
				&& NumberSupport.Equal (a.disposition,             b.disposition)
				&& NumberSupport.Equal (a.breakFenceBefore,      b.breakFenceBefore)
				&& NumberSupport.Equal (a.breakFenceAfter,       b.breakFenceAfter)
				&& a.enableHyphenation == b.enableHyphenation
				&& a.level == b.level
				&& a.levelAttribute == b.levelAttribute;
		}
		
		
		private double							leftMarginFirstLine;
		private double							leftMarginBody;
		private double							rightMarginFirstLine;
		private double							rightMarginBody;
		
		private double							breakFenceBefore;
		private double							breakFenceAfter;
		
		private SizeUnits						units;
		
		private ThreeState						enableHyphenation;
		
		private double							justificationBody;			//	0.0 = pas de justification, 1.0 = justification pleine
		private double							justificationLastLine;
		private double							disposition;				//	0.0 = aligné à gauche, 0.5 = centré, 1.0 = aligné à droite
		
		private int								level;						//	-1 => pas de niveau spécifié
		private string							levelAttribute;			//	null => pas d'indentations particulières
	}
}
