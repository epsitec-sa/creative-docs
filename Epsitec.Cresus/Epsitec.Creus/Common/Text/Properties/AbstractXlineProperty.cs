//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public AbstractXlineProperty(double position, SizeUnits positionUnits, double thickness, SizeUnits thicknessUnits, string drawClass, string drawStyle)
		{
			this.positionUnits   = positionUnits;
			this.position        = positionUnits == SizeUnits.None ? double.NaN : position;
			this.thicknessUnits  = thicknessUnits;
			this.thickness       = thicknessUnits == SizeUnits.None ? double.NaN : thickness;
			
			this.drawClass  = drawClass;
			this.drawStyle  = drawStyle;
			
			System.Diagnostics.Debug.Assert (UnitsTools.IsAbsoluteSize (this.positionUnits)  || this.positionUnits == SizeUnits.None);
			System.Diagnostics.Debug.Assert (UnitsTools.IsAbsoluteSize (this.thicknessUnits) || this.thicknessUnits == SizeUnits.None);
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
				return CombinationMode.Combine;
			}
		}
		
		
		public bool								IsVisible
		{
			get
			{
				return (this.isDisabled == false) && (this.thickness != 0) && (double.IsNaN (this.thickness) == false);
			}
		}
		
		public bool								IsDisabled
		{
			get
			{
				return this.isDisabled;
			}
		}
		
		public bool								IsEmpty
		{
			get
			{
				return (this.drawClass == null) && (this.drawStyle == null);
			}
		}
		
		
		public SizeUnits						PositionUnits
		{
			get
			{
				return this.positionUnits;
			}
		}
		
		public SizeUnits						ThicknessUnits
		{
			get
			{
				return this.thicknessUnits;
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
				return this.drawClass;
			}
		}
		
		public string							DrawStyle
		{
			get
			{
				return this.drawStyle;
			}
		}
		
		
		public static System.Collections.IComparer	Comparer
		{
			get
			{
				return new AbstractXlineComparer ();
			}
		}
		
		
		public double GetPositionInPoints(double fontSizeInPoints)
		{
			if (UnitsTools.IsAbsoluteSize (this.positionUnits))
			{
				return UnitsTools.ConvertToPoints (this.position, this.positionUnits);
			}
			if (UnitsTools.IsScale (this.positionUnits))
			{
				return UnitsTools.ConvertToScale (this.position, this.positionUnits) * fontSizeInPoints;
			}
			
			throw new System.InvalidOperationException ();
		}
		
		public double GetThiknessInPoints(double fontSizeInPoints)
		{
			if (UnitsTools.IsAbsoluteSize (this.thicknessUnits))
			{
				return UnitsTools.ConvertToPoints (this.thickness, this.thicknessUnits);
			}
			if (UnitsTools.IsScale (this.thicknessUnits))
			{
				return UnitsTools.ConvertToScale (this.thickness, this.thicknessUnits) * fontSizeInPoints;
			}
			
			throw new System.InvalidOperationException ();
		}
		
		
		public static void RemoveInvisible(ref AbstractXlineProperty[] properties)
		{
			int count = 0;
			
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i].IsVisible)
				{
					count++;
				}
			}
			
			if (count < properties.Length)
			{
				//	Il y a des définitions de soulignements invisibles. Il faut
				//	recopier le tableau en les supprimant :
				
				AbstractXlineProperty[] copy  = new AbstractXlineProperty[count];
				int                     index = 0;
				
				for (int i = 0; (i < properties.Length) && (index < copy.Length); i++)
				{
					if (properties[i].IsVisible)
					{
						copy[index] = properties[i];
						index++;
					}
				}
				
				properties = copy;
			}
		}
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeBoolean (this.isDisabled),
				/**/				SerializerSupport.SerializeSizeUnits (this.positionUnits),
				/**/				SerializerSupport.SerializeSizeUnits (this.thicknessUnits),
				/**/				SerializerSupport.SerializeDouble (this.position),
				/**/				SerializerSupport.SerializeDouble (this.thickness),
				/**/				SerializerSupport.SerializeString (this.drawClass),
				/**/				SerializerSupport.SerializeString (this.drawStyle));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			System.Diagnostics.Debug.Assert (args.Length == 7);
			
			bool      isDisabled      = SerializerSupport.DeserializeBoolean (args[0]);
			SizeUnits positionUnits   = SerializerSupport.DeserializeSizeUnits (args[1]);
			SizeUnits thicknessUnits  = SerializerSupport.DeserializeSizeUnits (args[2]);
			double    position        = SerializerSupport.DeserializeDouble (args[3]);
			double    thickness       = SerializerSupport.DeserializeDouble (args[4]);
			string    drawClass       = SerializerSupport.DeserializeString (args[5]);
			string    drawStyle       = SerializerSupport.DeserializeString (args[6]);
			
			this.isDisabled = isDisabled;
			
			this.positionUnits  = positionUnits;
			this.thicknessUnits = thicknessUnits;
			
			this.position   = position;
			this.thickness  = thickness;
			
			this.drawClass = drawClass;
			this.drawStyle = drawStyle;
		}
		
		public override Property GetCombination(Property property)
		{
			AbstractXlineProperty a = this;
			AbstractXlineProperty b = property as AbstractXlineProperty;
			
			if (b.isDisabled)
			{
				//	Cas spécial: la deuxième définition indique qu'il faut désacriver
				//	le soulignement. On construit une définition conforme à la pre-
				//	mière, avec juste l'information qu'elle est désactivée :
				
				AbstractXlineProperty c = this.EmptyClone () as AbstractXlineProperty;
				
				c.isDisabled      = true;
				c.positionUnits   = a.positionUnits;
				c.thicknessUnits  = a.thicknessUnits;
				c.position        = a.position;
				c.thickness       = a.thickness;
				c.drawClass       = a.drawClass;
				c.drawStyle       = a.drawStyle;
				
				return c;
			}
			else
			{
				//	Cas normal: la deuxième définition le remporte sur la première.
				
				return b;
			}
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.isDisabled);
			checksum.UpdateValue ((int) this.positionUnits);
			checksum.UpdateValue ((int) this.thicknessUnits);
			checksum.UpdateValue (this.position);
			checksum.UpdateValue (this.thickness);
			checksum.UpdateValue (this.drawClass);
			checksum.UpdateValue (this.drawStyle);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return AbstractXlineProperty.CompareEqualContents (this, value as AbstractXlineProperty);
		}
		
		
		protected void Disable()
		{
			this.isDisabled = true;
		}
		
		
		private static bool CompareEqualContents(AbstractXlineProperty a, AbstractXlineProperty b)
		{
			return a.WellKnownType == b.WellKnownType
				&& a.isDisabled == b.isDisabled
				&& a.positionUnits == b.positionUnits
				&& a.thicknessUnits == b.thicknessUnits
				&& NumberSupport.Equal (a.position, b.position)
				&& NumberSupport.Equal (a.thickness, b.thickness)
				&& a.drawClass == b.drawClass
				&& a.drawStyle == b.drawStyle;
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
					result = string.Compare (px.drawClass, py.drawClass);
					
					if (result == 0)
					{
						result = string.Compare (px.drawStyle, py.drawStyle);
						
						if (result == 0)
						{
							double xv = px.positionUnits == SizeUnits.None ? double.NaN : UnitsTools.ConvertToSizeUnits (px.position, px.positionUnits, SizeUnits.Points);
							double yv = py.positionUnits == SizeUnits.None ? double.NaN : UnitsTools.ConvertToSizeUnits (py.position, py.positionUnits, SizeUnits.Points);
							
							result = NumberSupport.Compare (xv, yv);
							
							if (result == 0)
							{
								xv = px.thicknessUnits == SizeUnits.None ? double.NaN : UnitsTools.ConvertToSizeUnits (px.thickness, px.thicknessUnits, SizeUnits.Points);
								yv = py.thicknessUnits == SizeUnits.None ? double.NaN : UnitsTools.ConvertToSizeUnits (py.thickness, py.thicknessUnits, SizeUnits.Points);
							
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
		
		private bool							isDisabled;
		
		private SizeUnits						positionUnits;
		private SizeUnits						thicknessUnits;
		
		private double							position;
		private double							thickness;
		
		private string							drawClass;
		private string							drawStyle;
	}
}
