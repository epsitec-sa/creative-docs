//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe UnderlineProperty permet de régler les détails relatifs au
	/// soulignement du texte, mais aussi à d'autres décorations (biffé,
	/// encadré, surligné, etc.)
	/// </summary>
	public class UnderlineProperty : BaseProperty
	{
		public UnderlineProperty()
		{
		}
		
		public UnderlineProperty(double position, SizeUnits position_units, double thickness, SizeUnits thickness_units, string line_style)
		{
			this.position_units  = position_units;
			this.thickness_units = thickness_units;
			
			this.position   = this.position;
			this.thickness  = this.thickness;
			this.line_style = line_style;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Underline;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.ExtraSetting;
			}
		}
		
		
		public SizeUnits						PositionUnits
		{
			get
			{
				return this.position_units;
			}
		}
		
		public SizeUnits						ThicknessUnits
		{
			get
			{
				return this.thickness_units;
			}
		}
		
		public double							Position
		{
			get
			{
				return this.position;
			}
		}
		
		public double							Thickness
		{
			get
			{
				return this.thickness;
			}
		}
		
		public string							LineStyle
		{
			get
			{
				return this.line_style;
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeSizeUnits (this.position_units),
				/**/				SerializerSupport.SerializeSizeUnits (this.thickness_units),
				/**/				SerializerSupport.SerializeDouble (this.position),
				/**/				SerializerSupport.SerializeDouble (this.thickness),
				/**/				SerializerSupport.SerializeString (this.line_style));
		}

		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 5);
			
			SizeUnits position_units  = SerializerSupport.DeserializeSizeUnits (args[0]);
			SizeUnits thickness_units = SerializerSupport.DeserializeSizeUnits (args[1]);
			double    position        = SerializerSupport.DeserializeDouble (args[2]);
			double    thickness       = SerializerSupport.DeserializeDouble (args[3]);
			string    line_style      = SerializerSupport.DeserializeString (args[4]);
			
			this.position_units  = position_units;
			this.thickness_units = thickness_units;
			
			this.position   = position;
			this.thickness  = thickness;
			
			this.line_style = line_style;
		}
		
		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue ((int) this.position_units);
			checksum.UpdateValue ((int) this.thickness_units);
			checksum.UpdateValue (this.position);
			checksum.UpdateValue (this.thickness);
			checksum.UpdateValue (this.line_style);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return UnderlineProperty.CompareEqualContents (this, value as UnderlineProperty);
		}
		
		
		private static bool CompareEqualContents(UnderlineProperty a, UnderlineProperty b)
		{
			return a.position_units  == b.position_units
				&& a.thickness_units == b.thickness_units
				&& a.position   == b.position
				&& a.thickness  == b.thickness
				&& a.line_style == b.line_style;
		}
		
		
		
		private SizeUnits						position_units;
		private SizeUnits						thickness_units;
		
		private double							position;
		private double							thickness;
		
		private string							line_style;
	}
}
