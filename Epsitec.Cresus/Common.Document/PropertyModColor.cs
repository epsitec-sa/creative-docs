using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe PropertyModColor repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class PropertyModColor : AbstractProperty
	{
		public PropertyModColor(Document document) : base(document)
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

		// Effectue toutes les notifications apr�s un changement.
		protected override void NotifyAfter(bool oplet)
		{
			System.Diagnostics.Debug.Assert(this.owners.Count == 1);
			AbstractObject obj = this.owners[0] as AbstractObject;  // objet calque
			this.document.Notifier.NotifyLayerChanged(obj);
			this.document.Notifier.NotifyArea();
		}


		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return false; }
		}

		// Effectue une copie de la propri�t�.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyModColor p = property as PropertyModColor;
			p.h = this.h;
			p.s = this.s;
			p.v = this.v;
			p.r = this.r;
			p.g = this.g;
			p.b = this.b;
			p.a = this.a;
			p.n = this.n;
		}

		// Compare deux propri�t�s.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyModColor p = property as PropertyModColor;
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

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override AbstractPanel CreatePanel(Document document)
		{
			return new PanelModColor(document);
		}


		// Modifie une couleur.
		public void ModifyColor(ref Color color)
		{
#if false
			if ( this.h != 0.0 || this.s != 0.0 )  // teinte ou saturation ?
			{
				double a = color.A;
				double h,s,v;
				color.GetHSV(out h, out s, out v);
				h += this.h;
				s += this.s;
				color = Color.FromHSV(h,s,v);
				color.A = a;
			}
#else
			if ( this.h != 0.0 )  // teinte ?
			{
				double a = color.A;
				double h,s,v;
				color.GetHSV(out h, out s, out v);
				h += this.h;
				color = Color.FromHSV(h,s,v);
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

			if ( this.n )  // n�gatif ?
			{
				color.R = 1.0-color.R;
				color.G = 1.0-color.G;
				color.B = 1.0-color.B;
			}

			color = color.ClipToRange();
		}


		#region Serialization
		// S�rialise la propri�t�.
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

		// Constructeur qui d�s�rialise la propri�t�.
		protected PropertyModColor(SerializationInfo info, StreamingContext context) : base(info, context)
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
