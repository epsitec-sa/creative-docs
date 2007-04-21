using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Regular représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Regular : Abstract
	{
		public Regular(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialize()
		{
			base.Initialize ();
			this.nbFaces = 6;
			this.star    = false;
			this.flower  = false;
			this.deep    = new Polar( 0.50,   0.0);
			this.e1      = new Polar(-0.05,  20.0);
			this.e2      = new Polar(-0.05, -20.0);
			this.i1      = new Polar( 0.60,  10.0);
			this.i2      = new Polar( 0.60, -10.0);
		}

		public int NbFaces
		{
			get
			{
				return this.nbFaces;
			}
			
			set
			{
				if ( this.nbFaces != value )
				{
					this.NotifyBefore();
					this.nbFaces = value;
					this.NotifyAfter();
				}
			}
		}

		public bool Star
		{
			get
			{
				return this.star;
			}

			set
			{
				if ( this.star != value )
				{
					this.NotifyBefore();
					this.star = value;
					this.NotifyAfter();
				}
			}
		}

		public bool Flower
		{
			get
			{
				return this.flower;
			}

			set
			{
				if ( this.flower != value )
				{
					this.NotifyBefore();
					this.flower = value;
					this.NotifyAfter();
				}
			}
		}

		public Polar Deep
		{
			//	Profondeur des branches de l'étoile.
			get
			{
				return this.deep;
			}

			set
			{
				value.R = System.Math.Max(0.0, value.R);
				value.R = System.Math.Min(1.0, value.R);

				value.A = System.Math.Max(-90.0, value.A);
				value.A = System.Math.Min(90.0, value.A);

				if ( this.deep != value )
				{
					this.NotifyBefore();
					this.deep = value;
					this.NotifyAfter();
				}
			}
		}

		public Polar E1
		{
			//	Torsion des branches de l'étoile.
			get
			{
				return this.e1;
			}

			set
			{
				value.R = System.Math.Max(-1.0, value.R);
				value.R = System.Math.Min(1.0, value.R);

				value.A = System.Math.Max(-90.0, value.A);
				value.A = System.Math.Min(90.0, value.A);

				if ( this.e1 != value )
				{
					this.NotifyBefore();
					this.e1 = value;
					this.NotifyAfter();
				}
			}
		}

		public Polar E2
		{
			//	Torsion des branches de l'étoile.
			get
			{
				return this.e2;
			}

			set
			{
				value.R = System.Math.Max(-1.0, value.R);
				value.R = System.Math.Min(1.0, value.R);

				value.A = System.Math.Max(-90.0, value.A);
				value.A = System.Math.Min(90.0, value.A);

				if ( this.e2 != value )
				{
					this.NotifyBefore();
					this.e2 = value;
					this.NotifyAfter();
				}
			}
		}

		public Polar I1
		{
			//	Torsion des branches de l'étoile.
			get
			{
				return this.i1;
			}

			set
			{
				value.R = System.Math.Max(-1.0, value.R);
				value.R = System.Math.Min(1.0, value.R);

				value.A = System.Math.Max(-90.0, value.A);
				value.A = System.Math.Min(90.0, value.A);

				if ( this.i1 != value )
				{
					this.NotifyBefore();
					this.i1 = value;
					this.NotifyAfter();
				}
			}
		}

		public Polar I2
		{
			//	Torsion des branches de l'étoile.
			get
			{
				return this.i2;
			}

			set
			{
				value.R = System.Math.Max(-1.0, value.R);
				value.R = System.Math.Min(1.0, value.R);

				value.A = System.Math.Max(-90.0, value.A);
				value.A = System.Math.Min(90.0, value.A);

				if ( this.i2 != value )
				{
					this.NotifyBefore();
					this.i2 = value;
					this.NotifyAfter();
				}
			}
		}

		public override string SampleText
		{
			//	Donne le petit texte pour les échantillons.
			get
			{
				string star = this.star ? " *" : "";
				return string.Concat(Res.Strings.Property.Regular.Short.Faces, this.nbFaces.ToString(), star);
			}
		}

		public override void PutStyleBrief(System.Text.StringBuilder builder)
		{
			//	Construit le texte résumé d'un style pour une propriété.
			this.PutStyleBriefPrefix(builder);
			builder.Append(this.SampleText);
			this.PutStyleBriefPostfix(builder);
		}

		public static string GetName(string type)
		{
			//	Retourne le nom d'un type donné.
			switch (type)
			{
				case "Star":    return Res.Strings.Property.Regular.Star;
				case "Flower":  return Res.Strings.Property.Regular.Flower;
				default:        return Res.Strings.Property.Regular.Norm;
			}
		}

		public static string GetIconText(string type)
		{
			//	Retourne l'icône pour un type donné.
			switch (type)
			{
				case "Star":    return "RegularStar";
				case "Flower":  return "RegularFlower";
				default:        return "RegularNorm";
			}
		}


		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propriété modifie la bbox de l'objet.
			get { return true; }
		}


		public override int TotalHandle(Objects.Abstract obj)
		{
			//	Nombre de poignées.
			return 5;
		}

		public override bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			//	Indique si une poignée est visible.
			if ( !this.document.Modifier.IsPropertiesExtended(this.type) )
			{
				return false;
			}

			if (this.flower)
			{
				return true;
			}
			else if (this.star)
			{
				return rank <= 0;
			}
			else
			{
				return false;
			}
		}
		
		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			//	Retourne la position d'une poignée.
			Objects.Regular reg = obj as Objects.Regular;
			Point pos = Point.Zero;

			if (rank == 0)
			{
				return reg.PolarToPoint(1-this.deep.R, this.deep.A);
			}

			if (rank == 1)
			{
				double a = 360.0/(this.nbFaces*2)+this.deep.A;
				return reg.PolarToPoint(1-this.deep.R*this.e1.R, a*this.e1.R+this.e1.A);
			}

			if (rank == 2)
			{
				double a = 360.0/(this.nbFaces*2)+this.deep.A;
				return reg.PolarToPoint(1-this.deep.R*this.i1.R, a*this.i1.R+this.i1.A);
			}

			if (rank == 3)
			{
				double a = -360.0/(this.nbFaces*2)+this.deep.A;
				return reg.PolarToPoint(1-this.deep.R*this.e2.R, a*this.e2.R+this.e2.A);
			}

			if (rank == 4)
			{
				double a = -360.0/(this.nbFaces*2)+this.deep.A;
				return reg.PolarToPoint(1-this.deep.R*this.i2.R, a*this.i2.R+this.i2.A);
			}

			return pos;
		}

		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			//	Modifie la position d'une poignée.
			Objects.Regular reg = obj as Objects.Regular;

			if (rank == 0)
			{
				Polar p = reg.PointToPolar(pos);
				this.Deep = new Polar(1-p.R, p.A);
			}

			if (rank == 1)
			{
				Polar p = reg.PointToPolar(pos);
				double scale, angle;
				if (this.deep.R == 0)
				{
					scale = 0;
				}
				else
				{
					scale = (1-p.R)/this.deep.R;
				}

				double a = 360.0/(this.nbFaces*2)+this.deep.A;
				angle = p.A-(a*this.e1.R);

				this.E1 = new Polar(scale, angle);
			}

			if (rank == 2)
			{
				Polar p = reg.PointToPolar(pos);
				double scale, angle;
				if (this.deep.R == 0)
				{
					scale = 0;
				}
				else
				{
					scale = (1-p.R)/this.deep.R;
				}

				double a = 360.0/(this.nbFaces*2)+this.deep.A;
				angle = p.A-(a*this.i1.R);

				this.I1 = new Polar(scale, angle);
			}

			if (rank == 3)
			{
				Polar p = reg.PointToPolar(pos);
				double scale, angle;
				if (this.deep.R == 0)
				{
					scale = 0;
				}
				else
				{
					scale = (1-p.R)/this.deep.R;
				}

				double a = -360.0/(this.nbFaces*2)+this.deep.A;
				angle = p.A-(a*this.e2.R);

				this.E2 = new Polar(scale, angle);
			}

			if (rank == 4)
			{
				Polar p = reg.PointToPolar(pos);
				double scale, angle;
				if (this.deep.R == 0)
				{
					scale = 0;
				}
				else
				{
					scale = (1-p.R)/this.deep.R;
				}

				double a = -360.0/(this.nbFaces*2)+this.deep.A;
				angle = p.A-(a*this.i2.R);

				this.I2 = new Polar(scale, angle);
			}

			base.SetHandlePosition(obj, rank, pos);
		}
		
		
		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			Regular p = property as Regular;
			p.nbFaces = this.nbFaces;
			p.star    = this.star;
			p.flower  = this.flower;
			p.deep    = this.deep;
			p.e1      = this.e1;
			p.e2      = this.e2;
			p.i1      = this.i1;
			p.i2      = this.i2;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Regular p = property as Regular;
			if ( p.nbFaces != this.nbFaces )  return false;
			if ( p.star    != this.star    )  return false;
			if ( p.flower  != this.flower  )  return false;
			if ( p.deep    != this.deep    )  return false;
			if ( p.e1      != this.e1      )  return false;
			if ( p.e2      != this.e2      )  return false;
			if ( p.i1      != this.i1      )  return false;
			if ( p.i2      != this.i2      )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Regular(document);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			base.GetObjectData(info, context);

			info.AddValue("NbFaces", this.nbFaces);
			info.AddValue("Star", this.star);
			info.AddValue("Flower", this.flower);
			if (this.star || this.flower)
			{
				info.AddValue("Deep", this.deep);
				info.AddValue("E1", this.e1);
				info.AddValue("E2", this.e2);
				info.AddValue("I1", this.i1);
				info.AddValue("I2", this.i2);
			}
		}

		protected Regular(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise la propriété.
			this.nbFaces = info.GetInt32("NbFaces");
			this.star = info.GetBoolean("Star");

			if (this.document.IsRevisionGreaterOrEqual(2, 0, 16))
			{
				this.flower = info.GetBoolean("Flower");
				if (this.star || this.flower)
				{
					this.deep = (Polar) info.GetValue("Deep", typeof(Polar));
					this.e1 = (Polar) info.GetValue("E1", typeof(Polar));
					this.e2 = (Polar) info.GetValue("E2", typeof(Polar));
					this.i1 = (Polar) info.GetValue("I1", typeof(Polar));
					this.i2 = (Polar) info.GetValue("I2", typeof(Polar));
				}
			}
			else
			{
				this.flower = false;
				if (this.star)
				{
					this.deep = new Polar(info.GetDouble("Deep"), 0.0);
				}
			}
		}
		#endregion

	
		protected int					nbFaces;
		protected bool					star;
		protected bool					flower;
		protected Polar					deep;
		protected Polar					e1;
		protected Polar					e2;
		protected Polar					i1;
		protected Polar					i2;
	}
}
