//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe TabProperty décrit une tabulation.
	/// </summary>
	public class TabProperty : Property
	{
		public TabProperty()
		{
			this.position     = 0.0;
			this.disposition  = 0.0;
			this.docking_mark = null;
		}
		
		public TabProperty(double position, double disposition, string docking_mark) : this ()
		{
			this.position     = position;
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
				return PropertyType.ExtraSetting;
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
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeDouble (this.position),
				/**/				SerializerSupport.SerializeDouble (this.disposition),
				/**/				SerializerSupport.SerializeString (this.docking_mark));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 3);
			
			this.position     = SerializerSupport.DeserializeDouble (args[0]);
			this.disposition  = SerializerSupport.DeserializeDouble (args[1]);
			this.docking_mark = SerializerSupport.DeserializeString (args[2]);
		}
		
		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.TabProperty);
			
			TabProperty a = this;
			TabProperty b = property as TabProperty;
			TabProperty c = new TabProperty ();
			
			c.position     = NumberSupport.Combine (a.position, b.position);
			c.disposition  = NumberSupport.Combine (a.disposition, b.disposition);
			c.docking_mark = b.docking_mark;
			
			return c;
		}
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.position);
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
				&& NumberSupport.Equal (a.disposition, b.disposition)
				&& a.docking_mark == b.docking_mark;
		}
		
		
		private double							position;
		private double							disposition;				//	0.0 = aligné à gauche, 0.5 = centré, 1.0 = aligné à droite
		private string							docking_mark;				//	"." = aligne sur le point décimal
	}
}
