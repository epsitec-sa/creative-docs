using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyModColor représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyModColor : AbstractProperty
	{
		public PropertyModColor()
		{
			this.h = 0.0;
			this.s = 0.0;
			this.v = 0.0;
			this.r = 0.0;
			this.g = 0.0;
			this.b = 0.0;
			this.a = 0.0;
			this.n = false;
		}

		[XmlAttribute]
		public double H
		{
			get { return this.h; }
			set { this.h = value; }
		}

		[XmlAttribute]
		public double S
		{
			get { return this.s; }
			set { this.s = value; }
		}

		[XmlAttribute]
		public double V
		{
			get { return this.v; }
			set { this.v = value; }
		}

		[XmlAttribute]
		public double R
		{
			get { return this.r; }
			set { this.r = value; }
		}

		[XmlAttribute]
		public double G
		{
			get { return this.g; }
			set { this.g = value; }
		}

		[XmlAttribute]
		public double B
		{
			get { return this.b; }
			set { this.b = value; }
		}

		[XmlAttribute]
		public double A
		{
			get { return this.a; }
			set { this.a = value; }
		}

		[XmlAttribute]
		public bool N
		{
			get { return this.n; }
			set { this.n = value; }
		}

		[XmlIgnore]
		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propriété modifie la bbox de l'objet.
			get { return false; }
		}

		public override void CopyTo(AbstractProperty property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			PropertyModColor p = property as PropertyModColor;
			p.H = this.h;
			p.S = this.s;
			p.V = this.v;
			p.R = this.r;
			p.G = this.g;
			p.B = this.b;
			p.A = this.a;
			p.N = this.n;
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			PropertyModColor p = property as PropertyModColor;
			if ( p.H != this.h )  return false;
			if ( p.S != this.s )  return false;
			if ( p.V != this.v )  return false;
			if ( p.R != this.r )  return false;
			if ( p.G != this.g )  return false;
			if ( p.B != this.b )  return false;
			if ( p.A != this.a )  return false;
			if ( p.N != this.n )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			return new PanelModColor(drawer);
		}


		public void ModifyColor(ref Drawing.Color color)
		{
			//	Modifie une couleur.
#if false
			if ( this.h != 0.0 || this.s != 0.0 )  // teinte ou saturation ?
			{
				double a = color.A;
				double h,s,v;
				color.GetHSV(out h, out s, out v);
				h += this.h;
				s += this.s;
				color = Drawing.Color.FromHSV(h,s,v);
				color.A = a;
			}
#else
			if ( this.h != 0.0 )  // teinte ?
			{
				double a = color.A;
				double h,s,v;
				color.GetHSV(out h, out s, out v);
				h += this.h;
				color = Drawing.Color.FromHSV(h,s,v);
				color.A = a;
			}

			if ( this.s != 0.0 )  // saturation ?
			{
				double avg = (color.R+color.G+color.B)/3.0;
				double factor = this.s+1.0;  // 0..2
				color.R = avg+(color.R-avg)*factor;
				color.G = avg+(color.G-avg)*factor;
				color.B = avg+(color.B-avg)*factor;
			}
#endif

			color.R = color.R+this.v+this.r;
			color.G = color.G+this.v+this.g;
			color.B = color.B+this.v+this.b;
			color.A = color.A+this.a;

			if ( this.n )  // négatif ?
			{
				color.R = 1.0-color.R;
				color.G = 1.0-color.G;
				color.B = 1.0-color.B;
			}

			color = color.ClipToRange();
		}


		protected double			h = 0.0;  // 0..360
		protected double			s = 0.0;  // -1..1
		protected double			v = 0.0;  // -1..1
		protected double			r = 0.0;  // -1..1
		protected double			g = 0.0;  // -1..1
		protected double			b = 0.0;  // -1..1
		protected double			a = 0.0;  // -1..1
		protected bool				n = false;  // negativ
	}
}
