//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe AbstractXlineProperty sert de base aux propriétés "souligné",
	/// "biffé", "surligné", etc.
	/// </summary>
	public abstract class AbstractXlineProperty : Property
	{
		public AbstractXlineProperty()
		{
		}
		
		public AbstractXlineProperty(double position, SizeUnits position_units, double thickness, SizeUnits thickness_units, string draw_class, string draw_style)
		{
			this.position_units  = position_units;
			this.position        = position;
			this.thickness_units = thickness_units;
			this.thickness       = thickness;
			
			this.draw_class = draw_class;
			this.draw_style = draw_style;
			
			System.Diagnostics.Debug.Assert (UnitsTools.IsAbsoluteSize (this.position_units)  || this.position_units == SizeUnits.None);
			System.Diagnostics.Debug.Assert (UnitsTools.IsAbsoluteSize (this.thickness_units) || this.thickness_units == SizeUnits.None);
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
		
		public string							DrawClass
		{
			get
			{
				return this.draw_class;
			}
		}
		
		public string							DrawStyle
		{
			get
			{
				return this.draw_style;
			}
		}
		
		
		public static System.Collections.IComparer	Comparer
		{
			get
			{
				return new AbstractXlineComparer ();
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeSizeUnits (this.position_units),
				/**/				SerializerSupport.SerializeSizeUnits (this.thickness_units),
				/**/				SerializerSupport.SerializeDouble (this.position),
				/**/				SerializerSupport.SerializeDouble (this.thickness),
				/**/				SerializerSupport.SerializeString (this.draw_class),
				/**/				SerializerSupport.SerializeString (this.draw_style));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			System.Diagnostics.Debug.Assert (args.Length == 6);
			
			SizeUnits position_units  = SerializerSupport.DeserializeSizeUnits (args[0]);
			SizeUnits thickness_units = SerializerSupport.DeserializeSizeUnits (args[1]);
			double    position        = SerializerSupport.DeserializeDouble (args[2]);
			double    thickness       = SerializerSupport.DeserializeDouble (args[3]);
			string    draw_class      = SerializerSupport.DeserializeString (args[4]);
			string    draw_style      = SerializerSupport.DeserializeString (args[5]);
			
			this.position_units  = position_units;
			this.thickness_units = thickness_units;
			
			this.position   = position;
			this.thickness  = thickness;
			
			this.draw_class = draw_class;
			this.draw_style = draw_style;
		}
		
		public override Property GetCombination(Property property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue ((int) this.position_units);
			checksum.UpdateValue ((int) this.thickness_units);
			checksum.UpdateValue (this.position);
			checksum.UpdateValue (this.thickness);
			checksum.UpdateValue (this.draw_class);
			checksum.UpdateValue (this.draw_style);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return AbstractXlineProperty.CompareEqualContents (this, value as AbstractXlineProperty);
		}
		
		
		private static bool CompareEqualContents(AbstractXlineProperty a, AbstractXlineProperty b)
		{
			return a.WellKnownType == b.WellKnownType
				&& a.position_units == b.position_units
				&& NumberSupport.Equal (a.position, b.position)
				&& a.thickness_units == b.thickness_units
				&& NumberSupport.Equal (a.thickness, b.thickness)
				&& a.draw_class == b.draw_class
				&& a.draw_style == b.draw_style;
		}
		
		
		#region AbstractXlineComparer Class
		private class AbstractXlineComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				Properties.AbstractXlineProperty px = x as Properties.AbstractXlineProperty;
				Properties.AbstractXlineProperty py = y as Properties.AbstractXlineProperty;
				
				int result;
				
				if (px.WellKnownType == py.WellKnownType)
				{
					result = string.Compare (px.draw_class, py.draw_class);
					
					if (result == 0)
					{
						result = string.Compare (px.draw_style, py.draw_style);
						
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
				}
				else
				{
					result = px.WellKnownType < py.WellKnownType ? -1 : 1;
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
		
		private string							draw_class;
		private string							draw_style;
	}
}
