//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe FontOffsetProperty décrit un décalage vertical de la ligne
	/// de base d'un caractère.
	/// </summary>
	public class FontOffsetProperty : Property
	{
		public FontOffsetProperty() : this (0, SizeUnits.Points)
		{
		}
		
		public FontOffsetProperty(double offset, SizeUnits units)
		{
			this.offset = offset;
			this.units  = units;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.FontOffset;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.LocalSetting;
			}
		}
		
		
		public double							Offset
		{
			get
			{
				return this.offset;
			}
		}
		
		public SizeUnits						Units
		{
			get
			{
				return this.units;
			}
		}
		
		
		public double GetOffsetInPoints(double fontSizeInPoints)
		{
			if (UnitsTools.IsAbsoluteSize (this.units))
			{
				return UnitsTools.ConvertToPoints (this.offset, this.units);
			}
			if (UnitsTools.IsRelativeSize (this.units))
			{
				return UnitsTools.ConvertToPoints (this.offset, this.units) + fontSizeInPoints;
			}
			if (UnitsTools.IsScale (this.units))
			{
				return UnitsTools.ConvertToScale (this.offset, this.units) * fontSizeInPoints;
			}
			
			throw new System.InvalidOperationException ();
		}
		
		
		public override Property EmptyClone()
		{
			return new FontOffsetProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.offset),
				/**/				SerializerSupport.SerializeSizeUnits (this.units));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 2);
			
			double    offset = SerializerSupport.DeserializeDouble (args[0]);
			SizeUnits units  = SerializerSupport.DeserializeSizeUnits (args[1]);
			
			this.offset = offset;
			this.units  = units;
		}

		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.FontOffsetProperty);
			
			FontOffsetProperty a = this;
			FontOffsetProperty b = property as FontOffsetProperty;
			FontOffsetProperty c = new FontOffsetProperty ();
			
			UnitsTools.Combine (a.offset, a.units, b.offset, b.units, out c.offset, out c.units);
			
			return c;
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.offset);
			checksum.UpdateValue ((int) this.units);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return FontOffsetProperty.CompareEqualContents (this, value as FontOffsetProperty);
		}
		
		
		private static bool CompareEqualContents(FontOffsetProperty a, FontOffsetProperty b)
		{
			return NumberSupport.Equal (a.offset, b.offset)
				&& a.units == b.units;
		}
		
		
		private double							offset;
		private SizeUnits						units;
	}
}
