using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	using XmlAttribute = System.Xml.Serialization.XmlAttributeAttribute;
	
	[System.Serializable]
	public struct Polar
	{
		public Polar(double r, double a)
		{
			this.r = r;
			this.a = a;
		}
		
		
		[XmlAttribute] public double			R
		{
			//	Distance à l'origine.
			get { return this.r; }
			set { this.r = value; }
		}
		
		[XmlAttribute] public double			A
		{
			//	Angle en degrés.
			get { return this.a; }
			set { this.a = value; }
		}
		
		
		public bool								IsZero
		{
			get { return this.r == 0 && this.a == 0; }
		}
		
		
		public static readonly Polar 			Zero;
		
		
		public override string ToString()
		{
			return System.String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0};{1}\u00B0", this.r, this.a);
		}
		
		
		public override bool Equals(object obj)
		{
			return (obj is Polar) && (this == (Polar) obj);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		
		public static Polar operator +(Polar a, Polar b)
		{
			return new Polar(a.r + b.r, a.a + b.a);
		}
		
		public static Polar operator -(Polar a, Polar b)
		{
			return new Polar(a.r - b.r, a.a - b.a);
		}
		
		public static Polar operator -(Polar a)
		{
			return new Polar(-a.r, a.a);
		}
		
		
		public static bool operator ==(Polar a, Polar b)
		{
			return (a.r == b.r) && (a.a == b.a);
		}
		
		public static bool operator !=(Polar a, Polar b)
		{
			return (a.r != b.r) || (a.a != b.a);
		}
		
		
		private double							r;
		private double							a;
	}
}
