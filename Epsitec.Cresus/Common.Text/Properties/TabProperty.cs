//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe TabProperty décrit une tabulation.
	/// </summary>
	public class TabProperty : Property
	{
		public TabProperty() : this (0, SizeUnits.Points, 0, null)
		{
		}
		
		public TabProperty(double position, double disposition, string docking_mark) : this (position, SizeUnits.Points, disposition, docking_mark)
		{
		}
		
		public TabProperty(double position, SizeUnits units, double disposition, string docking_mark)
		{
			System.Diagnostics.Debug.Assert (double.IsNaN (position) == false);
			System.Diagnostics.Debug.Assert (UnitsTools.IsAbsoluteSize (units));
			
			this.position     = position;
			this.units        = units;
			this.disposition  = disposition;
			this.docking_mark = docking_mark;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Tab;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.LocalSetting;
			}
		}
		
		public override PropertyAffinity		PropertyAffinity
		{
			get
			{
				return PropertyAffinity.Symbol;
			}
		}

		public override CombinationMode			CombinationMode
		{
			get
			{
				return CombinationMode.Invalid;
			}
		}

		
		public double							Position
		{
			get
			{
				return this.position;
			}
			set
			{
				if (NumberSupport.Different (this.position, value))
				{
					this.position = value;
					this.Invalidate ();
				}
			}
		}
		
		public double							PositionInPoints
		{
			get
			{
				return UnitsTools.ConvertToPoints (this.position, this.units);
			}
		}
		
		public SizeUnits						Units
		{
			get
			{
				return this.units;
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
		
		public string							DockingMark
		{
			get
			{
				return this.docking_mark;
			}
			set
			{
				if (this.docking_mark != value)
				{
					this.docking_mark = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new TabProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.position),
				/**/				SerializerSupport.SerializeSizeUnits (this.units),
				/**/				SerializerSupport.SerializeDouble (this.disposition),
				/**/				SerializerSupport.SerializeString (this.docking_mark));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 4);
			
			double    position     = SerializerSupport.DeserializeDouble (args[0]);
			SizeUnits units        = SerializerSupport.DeserializeSizeUnits (args[1]);
			double    disposition  = SerializerSupport.DeserializeDouble (args[2]);
			string    docking_mark = SerializerSupport.DeserializeString (args[3]);
			
			this.position     = position;
			this.units        = units;
			this.disposition  = disposition;
			this.docking_mark = docking_mark;
		}
		
		
		public override Property GetCombination(Property property)
		{
			throw new System.InvalidOperationException ();
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.position);
			checksum.UpdateValue ((int) this.units);
			checksum.UpdateValue (this.disposition);
			checksum.UpdateValue (this.docking_mark);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return TabProperty.CompareEqualContents (this, value as TabProperty);
		}
		
		
		private static bool CompareEqualContents(TabProperty a, TabProperty b)
		{
			return NumberSupport.Equal (a.position, b.position)
				&& a.units == b.units
				&& NumberSupport.Equal (a.disposition, b.disposition)
				&& a.docking_mark == b.docking_mark;
		}
		
		
		private double							position;
		private SizeUnits						units;
		private double							disposition;				//	0.0 = aligné à gauche, 0.5 = centré, 1.0 = aligné à droite
		private string							docking_mark;				//	"." = aligne sur le point décimal
	}
}
