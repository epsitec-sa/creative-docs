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

		protected override void Initialise()
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

		// Effectue toutes les notifications après un changement.
		protected override void NotifyAfter(bool oplet)
		{
			System.Diagnostics.Debug.Assert(this.owners.Count == 1);
			Objects.Abstract obj = this.owners[0] as Objects.Abstract;  // objet calque
			this.document.Notifier.NotifyLayerChanged(obj);
			this.document.Notifier.NotifyArea();
			this.document.IsDirtySerialize = true;
		}

		// Indique si une impression complexe est nécessaire.
		public override bool IsComplexPrinting
		{
			get
			{
				if ( this.a != 0.0 )  return true;
				return false;
			}
		}

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return false; }
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(Abstract property)
		{
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

		// Compare deux propriétés.
		public override bool Compare(Abstract property)
		{
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

		// Crée le panneau permettant d'éditer la propriété.
		public override Panels.Abstract CreatePanel(Document document)
		{
			Panels.Abstract.StaticDocument = document;
			return new Panels.ModColor(document);
		}


		// Modifie une couleur.
		public void ModifyColor(ref Drawing.RichColor color)
		{
			if ( this.h == 0.0 &&
				 this.s == 0.0 &&
				 this.v == 0.0 &&
				 this.r == 0.0 &&
				 this.g == 0.0 &&
				 this.b == 0.0 &&
				 this.a == 0.0 &&
				 this.n == false )  return;

			Drawing.Color basic = color.Basic;

			if ( this.h != 0.0 )  // teinte ?
			{
				double a = basic.A;
				double h,s,v;
				basic.GetHSV(out h, out s, out v);
				h += this.h;
				basic = Drawing.Color.FromHSV(h,s,v);
				basic.A = a;
			}

			if ( this.s != 0.0 )  // saturation ?
			{
				double avg = (basic.R+basic.G+basic.B)/3.0;
				double factor = this.s+1.0;  // 0..2
				basic.R = avg+(basic.R-avg)*factor;
				basic.G = avg+(basic.G-avg)*factor;
				basic.B = avg+(basic.B-avg)*factor;
			}

			basic.R = basic.R+this.v+this.r;
			basic.G = basic.G+this.v+this.g;
			basic.B = basic.B+this.v+this.b;
			basic.A = basic.A+this.a;

			if ( this.n )  // négatif ?
			{
				basic.R = 1.0-basic.R;
				basic.G = 1.0-basic.G;
				basic.B = 1.0-basic.B;
			}

			basic = basic.ClipToRange();

			// Si une couleur Gray est devenue non grise, change l'espace de
			// couleur en RGB.
			if ( color.ColorSpace == ColorSpace.Gray )
			{
				if ( basic.R != basic.G || basic.G != basic.B || basic.B != basic.R )
				{
					color = RichColor.FromColor(basic);
					return;
				}
			}

			color.Basic = basic;
		}


		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
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

		// Constructeur qui désérialise la propriété.
		protected ModColor(SerializationInfo info, StreamingContext context) : base(info, context)
		{
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
