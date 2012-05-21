using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe ModColor représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class ModColor : Abstract
	{
		public ModColor(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialize()
		{
			base.Initialize ();
			this.h = 0.0;
			this.s = 0.0;
			this.v = 0.0;
			this.r = 0.0;
			this.g = 0.0;
			this.b = 0.0;
			this.a = 0.0;
			this.n = false;
		}

		public double H
		{
			get
			{
				return this.h;
			}
			
			set
			{
				if ( this.h != value )
				{
					this.NotifyBefore();
					this.h = value;
					this.NotifyAfter();
				}
			}
		}

		public double S
		{
			get
			{
				return this.s;
			}
			
			set
			{
				if ( this.s != value )
				{
					this.NotifyBefore();
					this.s = value;
					this.NotifyAfter();
				}
			}
		}

		public double V
		{
			get
			{
				return this.v;
			}
			
			set
			{
				if ( this.v != value )
				{
					this.NotifyBefore();
					this.v = value;
					this.NotifyAfter();
				}
			}
		}

		public double R
		{
			get
			{
				return this.r;
			}
			
			set
			{
				if ( this.r != value )
				{
					this.NotifyBefore();
					this.r = value;
					this.NotifyAfter();
				}
			}
		}

		public double G
		{
			get
			{
				return this.g;
			}
			
			set
			{
				if ( this.g != value )
				{
					this.NotifyBefore();
					this.g = value;
					this.NotifyAfter();
				}
			}
		}

		public double B
		{
			get
			{
				return this.b;
			}
			
			set
			{
				if ( this.b != value )
				{
					this.NotifyBefore();
					this.b = value;
					this.NotifyAfter();
				}
			}
		}

		public double A
		{
			get
			{
				return this.a;
			}
			
			set
			{
				if ( this.a != value )
				{
					this.NotifyBefore();
					this.a = value;
					this.NotifyAfter();
				}
			}
		}

		public bool N
		{
			get
			{
				return this.n;
			}
			
			set
			{
				if ( this.n != value )
				{
					this.NotifyBefore();
					this.n = value;
					this.NotifyAfter();
				}
			}
		}

		protected override void NotifyAfter(bool oplet)
		{
			//	Effectue toutes les notifications après un changement.
			System.Diagnostics.Debug.Assert(this.owners.Count == 1);
			Objects.Abstract obj = this.owners[0] as Objects.Abstract;  // objet calque
			this.document.Notifier.NotifyLayerChanged(obj);
			this.document.Notifier.NotifyArea();
			this.document.SetDirtySerialize(CacheBitmapChanging.Local);
		}

		public override bool IsComplexPrinting
		{
			//	Indique si une impression complexe est nécessaire.
			get
			{
				if ( this.a != 0.0 )  return true;
				return false;
			}
		}

		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propriété modifie la bbox de l'objet.
			get { return false; }
		}

		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			ModColor p = property as ModColor;
			p.h = this.h;
			p.s = this.s;
			p.v = this.v;
			p.r = this.r;
			p.g = this.g;
			p.b = this.b;
			p.a = this.a;
			p.n = this.n;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			ModColor p = property as ModColor;
			if ( p.h != this.h )  return false;
			if ( p.s != this.s )  return false;
			if ( p.v != this.v )  return false;
			if ( p.r != this.r )  return false;
			if ( p.g != this.g )  return false;
			if ( p.b != this.b )  return false;
			if ( p.a != this.a )  return false;
			if ( p.n != this.n )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.ModColor(document);
		}


		public Drawing.RichColor ModifyColor(Drawing.RichColor color)
		{
			//	Modifie une couleur.
			if (this.h == 0.0 &&
				 this.s == 0.0 &&
				 this.v == 0.0 &&
				 this.r == 0.0 &&
				 this.g == 0.0 &&
				 this.b == 0.0 &&
				 this.a == 0.0 &&
				 this.n == false)
			{
				return color;
			}

			Drawing.Color basic = color.Basic;

			if ( this.h != 0.0 )  // teinte ?
			{
				double a = basic.A;
				double h,s,v;
				basic.GetHsv(out h, out s, out v);
				h += this.h;
				basic = Drawing.Color.FromAlphaHsv(a, h,s,v);
			}

			if ( this.s != 0.0 )  // saturation ?
			{
				double avg = (basic.R+basic.G+basic.B)/3.0;
				double factor = this.s+1.0;  // 0..2
				basic = Drawing.Color.FromAlphaRgb(basic.A, avg+(basic.R-avg)*factor, avg+(basic.G-avg)*factor, avg+(basic.B-avg)*factor);
			}

			basic = Drawing.Color.FromAlphaRgb(basic.A+this.a, basic.R+this.v+this.r, basic.G+this.v+this.g, basic.B+this.v+this.b);

			if ( this.n )  // négatif ?
			{
				basic = Drawing.Color.FromAlphaRgb(basic.A, 1.0-basic.R, 1.0-basic.G, 1.0-basic.B);
			}

			basic = basic.ClipToRange();

			//	Si une couleur Gray est devenue non grise, change l'espace de
			//	couleur en RGB.
			if ( color.ColorSpace == ColorSpace.Gray )
			{
				if ( basic.R != basic.G || basic.G != basic.B || basic.B != basic.R )
				{
					color = RichColor.FromColor(basic);
					return color;
				}
			}

			color.Basic = basic;
			return color;
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			base.GetObjectData(info, context);

			info.AddValue("H", this.h);
			info.AddValue("S", this.s);
			info.AddValue("V", this.v);
			info.AddValue("R", this.r);
			info.AddValue("G", this.g);
			info.AddValue("B", this.b);
			info.AddValue("A", this.a);
			info.AddValue("N", this.n);
		}

		protected ModColor(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise la propriété.
			this.h = info.GetDouble("H");
			this.s = info.GetDouble("S");
			this.v = info.GetDouble("V");
			this.r = info.GetDouble("R");
			this.g = info.GetDouble("G");
			this.b = info.GetDouble("B");
			this.a = info.GetDouble("A");
			this.n = info.GetBoolean("N");
		}
		#endregion

	
		protected double			h;  // 0..360
		protected double			s;  // -1..1
		protected double			v;  // -1..1
		protected double			r;  // -1..1
		protected double			g;  // -1..1
		protected double			b;  // -1..1
		protected double			a;  // -1..1
		protected bool				n;  // negativ
	}
}
