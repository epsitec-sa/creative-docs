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
		
		public UnderlineProperty(double position, SizeUnits position_units, double thickness, SizeUnits thickness_units, string line_class, string line_style)
		{
			this.position_units  = position_units;
			this.position        = position;
			this.thickness_units = thickness_units;
			this.thickness       = thickness;
			
			this.line_class = line_class;
			this.line_style = line_style;
			
			System.Diagnostics.Debug.Assert (UnitsTools.IsAbsoluteSize (this.position_units)  || this.position_units == SizeUnits.None);
			System.Diagnostics.Debug.Assert (UnitsTools.IsAbsoluteSize (this.thickness_units) || this.thickness_units == SizeUnits.None);
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
		
		public override CombinationMode			CombinationMode
		{
			get
			{
				return CombinationMode.Accumulate;
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
		
		public string							LineClass
		{
			get
			{
				return this.line_class;
			}
		}
		
		public string							LineStyle
		{
			get
			{
				return this.line_style;
			}
		}
		
		
		public static System.Collections.IComparer	Comparer
		{
			get
			{
				return new UnderlineComparer ();
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeSizeUnits (this.position_units),
				/**/				SerializerSupport.SerializeSizeUnits (this.thickness_units),
				/**/				SerializerSupport.SerializeDouble (this.position),
				/**/				SerializerSupport.SerializeDouble (this.thickness),
				/**/				SerializerSupport.SerializeString (this.line_class),
				/**/				SerializerSupport.SerializeString (this.line_style));
		}

		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 6);
			
			SizeUnits position_units  = SerializerSupport.DeserializeSizeUnits (args[0]);
			SizeUnits thickness_units = SerializerSupport.DeserializeSizeUnits (args[1]);
			double    position        = SerializerSupport.DeserializeDouble (args[2]);
			double    thickness       = SerializerSupport.DeserializeDouble (args[3]);
			string    line_class      = SerializerSupport.DeserializeString (args[4]);
			string    line_style      = SerializerSupport.DeserializeString (args[5]);
			
			this.position_units  = position_units;
			this.thickness_units = thickness_units;
			
			this.position   = position;
			this.thickness  = thickness;
			
			this.line_class = line_class;
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
			checksum.UpdateValue (this.line_class);
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
				&& a.line_class == b.line_class
				&& a.line_style == b.line_style;
		}
		
		
		#region UnderlineComparer Class
		private class UnderlineComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				Properties.UnderlineProperty px = x as Properties.UnderlineProperty;
				Properties.UnderlineProperty py = y as Properties.UnderlineProperty;
				
				int result;
				
				result = string.Compare (px.line_class, py.line_class);
				
				if (result == 0)
				{
					result = string.Compare (px.line_style, py.line_style);
					
					if (result == 0)
					{
						double xv = UnitsTools.ConvertToSizeUnits (px.position, px.position_units, SizeUnits.Points);
						double yv = UnitsTools.ConvertToSizeUnits (py.position, py.position_units, SizeUnits.Points);
						
						result = NumberSupport.Compare (xv, yv);
						
						if (result == 0)
						{
							xv = UnitsTools.ConvertToSizeUnits (px.thickness, px.thickness_units, SizeUnits.Points);
							yv = UnitsTools.ConvertToSizeUnits (py.thickness, py.thickness_units, SizeUnits.Points);
						
							result = NumberSupport.Compare (xv, yv);
						}
					}
				}
				
				return result;
			}
			#endregion
		}
		#endregion
		
		
		private SizeUnits						position_units;
		private double							position;
		
		private SizeUnits						thickness_units;
		private double							thickness;
		
		private string							line_class;
		private string							line_style;
	}
}
