//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// Summary description for MarginsProperty.
	/// </summary>
	public class MarginsProperty : BaseProperty
	{
		public MarginsProperty()
		{
			this.left_margin_first_line = double.NaN;
			this.left_margin_body       = double.NaN;
			
			this.right_margin_first_line = double.NaN;
			this.right_margin_body       = double.NaN;
			
			this.justification_body      = 0.0;
			this.justification_last_line = 0.0;
			this.disposition             = 0.0;
		}
		
		public MarginsProperty(double left_margin, double right_margin) : this ()
		{
			this.left_margin_first_line = left_margin;
			this.left_margin_body       = left_margin;
			
			this.right_margin_first_line = right_margin;
			this.right_margin_body       = right_margin;
		}
		
		public MarginsProperty(double left_margin_first_line, double left_margin_body, double right_margin_first_line, double right_margin_body, double justification_body, double justification_last_line, double disposition) : this ()
		{
			this.left_margin_first_line  = left_margin_first_line;
			this.left_margin_body        = left_margin_body;
			this.right_margin_first_line = right_margin_first_line;
			this.right_margin_body       = right_margin_body;
			
			this.justification_body      = justification_body;
			this.justification_last_line = justification_last_line;
			this.disposition             = disposition;
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
				return PropertyType.Style;
			}
		}
		
		
		public double							LeftMarginFirstLine
		{
			get
			{
				return this.left_margin_first_line;
			}
			set
			{
				if (NumberSupport.Different (this.left_margin_first_line, value))
				{
					this.left_margin_first_line = value;
					this.Invalidate ();
				}
			}
		}
		
		public double							LeftMarginBody
		{
			get
			{
				return this.left_margin_body;
			}
			set
			{
				if (NumberSupport.Different (this.left_margin_body, value))
				{
					this.left_margin_body = value;
					this.Invalidate ();
				}
			}
		}
		
		public double							RightMarginFirstLine
		{
			get
			{
				return this.right_margin_first_line;
			}
			set
			{
				if (NumberSupport.Different (this.right_margin_first_line, value))
				{
					this.right_margin_first_line = value;
					this.Invalidate ();
				}
			}
		}
		
		public double							RightMarginBody
		{
			get
			{
				return this.right_margin_body;
			}
			set
			{
				if (NumberSupport.Different (this.right_margin_body, value))
				{
					this.right_margin_body = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public double							JustificationBody
		{
			get
			{
				return this.justification_body;
			}
			set
			{
				if (NumberSupport.Different (this.justification_body, value))
				{
					this.justification_body = value;
					this.Invalidate ();
				}
			}
		}
		
		public double							JustificationLastLine
		{
			get
			{
				return this.justification_last_line;
			}
			set
			{
				if (NumberSupport.Different (this.justification_last_line, value))
				{
					this.justification_last_line = value;
					this.Invalidate ();
				}
			}
		}
		
		public double							Disposition
		{
			get
			{
				return this.disposition;
			}
			set
			{
				if (NumberSupport.Different (this.disposition, value))
				{
					this.disposition = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.left_margin_first_line),
				/**/				SerializerSupport.SerializeDouble (this.left_margin_body),
				/**/				SerializerSupport.SerializeDouble (this.right_margin_first_line),
				/**/				SerializerSupport.SerializeDouble (this.right_margin_body),
				/**/				SerializerSupport.SerializeDouble (this.justification_body),
				/**/				SerializerSupport.SerializeDouble (this.justification_last_line),
				/**/				SerializerSupport.SerializeDouble (this.disposition));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 7);
			
			this.left_margin_first_line  = SerializerSupport.DeserializeDouble (args[0]);
			this.left_margin_body        = SerializerSupport.DeserializeDouble (args[1]);
			this.right_margin_first_line = SerializerSupport.DeserializeDouble (args[2]);
			this.right_margin_body       = SerializerSupport.DeserializeDouble (args[3]);
			this.justification_body      = SerializerSupport.DeserializeDouble (args[4]);
			this.justification_last_line = SerializerSupport.DeserializeDouble (args[5]);
			this.disposition             = SerializerSupport.DeserializeDouble (args[6]);
		}
		
		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			Debug.Assert.IsTrue (property is Properties.MarginsProperty);
			
			MarginsProperty a = this;
			MarginsProperty b = property as MarginsProperty;
			MarginsProperty c = new MarginsProperty ();
			
			c.left_margin_first_line  = NumberSupport.Combine (a.left_margin_first_line,  b.left_margin_first_line);
			c.left_margin_body        = NumberSupport.Combine (a.left_margin_body,        b.left_margin_body);
			c.right_margin_first_line = NumberSupport.Combine (a.right_margin_first_line, b.right_margin_first_line);
			c.right_margin_body       = NumberSupport.Combine (a.right_margin_body,       b.right_margin_body);
			c.justification_body      = NumberSupport.Combine (a.justification_body,      b.justification_body);
			c.justification_last_line = NumberSupport.Combine (a.justification_last_line, b.justification_last_line);
			c.disposition             = NumberSupport.Combine (a.disposition,             b.disposition);
			
			return c;
		}
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.left_margin_first_line);
			checksum.UpdateValue (this.left_margin_body);
			checksum.UpdateValue (this.right_margin_first_line);
			checksum.UpdateValue (this.right_margin_body);
			checksum.UpdateValue (this.justification_body);
			checksum.UpdateValue (this.justification_last_line);
			checksum.UpdateValue (this.disposition);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return MarginsProperty.CompareEqualContents (this, value as MarginsProperty);
		}
		
		
		private static bool CompareEqualContents(MarginsProperty a, MarginsProperty b)
		{
			if ((a.left_margin_first_line  == b.left_margin_first_line) &&
				(a.left_margin_body        == b.left_margin_body) &&
				(a.right_margin_first_line == b.right_margin_first_line) &&
				(a.right_margin_body       == b.right_margin_body) &&
				(a.justification_body      == b.justification_body) &&
				(a.justification_last_line == b.justification_last_line) &&
				(a.disposition             == b.disposition))
			{
				return true;
			}
			
			return false;
		}
		
		
		private double							left_margin_first_line;
		private double							left_margin_body;
		private double							right_margin_first_line;
		private double							right_margin_body;
		
		private double							justification_body;			//	0.0 = pas de justification, 1.0 = justification pleine
		private double							justification_last_line;
		private double							disposition;				//	0.0 = aligné à gauche, 0.5 = centré, 1.0 = aligné à droite
	}
}
