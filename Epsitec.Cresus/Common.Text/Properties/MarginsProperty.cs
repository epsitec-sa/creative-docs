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
			this.right_margin           = double.NaN;
		}
		
		public MarginsProperty(double left_margin_first_line, double left_margin_body, double right_margin1)
		{
			this.left_margin_first_line = left_margin_first_line;
			this.left_margin_body       = left_margin_body;
			this.right_margin           = right_margin;
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
		
		public double							RightMargin
		{
			get
			{
				return this.right_margin;
			}
			set
			{
				if (NumberSupport.Different (this.right_margin, value))
				{
					this.right_margin = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.left_margin_first_line),
				/**/				SerializerSupport.SerializeDouble (this.left_margin_body),
				/**/				SerializerSupport.SerializeDouble (this.right_margin));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 3);
			
			this.left_margin_first_line = SerializerSupport.DeserializeDouble (args[0]);
			this.left_margin_body       = SerializerSupport.DeserializeDouble (args[1]);
			this.right_margin           = SerializerSupport.DeserializeDouble (args[2]);
		}
		
		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			Debug.Assert.IsTrue (property is Properties.MarginsProperty);
			
			MarginsProperty a = this;
			MarginsProperty b = property as MarginsProperty;
			MarginsProperty c = new MarginsProperty ();
			
			c.left_margin_first_line = NumberSupport.Combine (a.left_margin_first_line, b.left_margin_first_line);
			c.left_margin_body       = NumberSupport.Combine (a.left_margin_body,       b.left_margin_body);
			c.right_margin           = NumberSupport.Combine (a.right_margin,           b.right_margin);
			
			return c;
		}
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.left_margin_first_line);
			checksum.UpdateValue (this.left_margin_body);
			checksum.UpdateValue (this.right_margin);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return MarginsProperty.CompareEqualContents (this, value as MarginsProperty);
		}
		
		
		private static bool CompareEqualContents(MarginsProperty a, MarginsProperty b)
		{
			if ((a.left_margin_first_line == b.left_margin_first_line) &&
				(a.left_margin_body       == b.left_margin_body) &&
				(a.right_margin           == b.right_margin))
			{
				return true;
			}
			
			return false;
		}
		
		
		private double							left_margin_first_line;
		private double							left_margin_body;
		private double							right_margin;
	}
}
