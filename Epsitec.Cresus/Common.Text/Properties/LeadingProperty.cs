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
		public LeadingProperty()
		{
			this.value = double.NaN;
			this.units = SizeUnits.None;
			this.mode  = LeadingMode.Free;
		}
		
		public LeadingProperty(double value, SizeUnits units, LeadingMode mode) : this ()
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
				/**/				SerializerSupport.SerializeSizeUnits (this.units),
				/**/				SerializerSupport.SerializeEnum (this.mode));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 3);
			
			double      value = SerializerSupport.DeserializeDouble (args[0]);
			SizeUnits   units = SerializerSupport.DeserializeSizeUnits (args[1]);
			LeadingMode mode  = (LeadingMode) SerializerSupport.DeserializeEnum (typeof (LeadingMode), args[2]);
			
			this.value = value;
			this.units = units;
			this.mode  = mode;
		}
		
		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			Debug.Assert.IsTrue (property is Properties.LeadingProperty);
			
			LeadingProperty a = this;
			LeadingProperty b = property as LeadingProperty;
			LeadingProperty c = new LeadingProperty ();
			
			UnitsTools.Combine (a.value, a.units, b.value, b.units, out c.value, out c.units);
			
			c.mode = b.mode == LeadingMode.Undefined ? a.mode : b.mode;
			
			return c;
		}
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.value);
			checksum.UpdateValue ((int) this.units);
			checksum.UpdateValue ((int) this.mode);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return LeadingProperty.CompareEqualContents (this, value as LeadingProperty);
		}
		
		
		private static bool CompareEqualContents(LeadingProperty a, LeadingProperty b)
		{
			return NumberSupport.Equal (a.value,  b.value)
				&& a.units == b.units
				&& a.mode  == b.mode;
		}
		
		
		private double							value;
		private SizeUnits						units;
		private LeadingMode						mode;
	}
}
