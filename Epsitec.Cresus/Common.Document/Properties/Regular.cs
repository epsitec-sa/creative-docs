using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Regular repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Regular : Abstract
	{
		public Regular(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			this.nbFaces = 6;
			this.star    = false;
			this.deep    = 0.5;
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

		public double Deep
		{
			get
			{
				return this.deep;
			}

			set
			{
				value = System.Math.Max(0.0, value);
				value = System.Math.Min(1.0, value);

				if ( this.deep != value )
				{
					this.NotifyBefore();
					this.deep = value;
					this.NotifyAfter();
				}
			}
		}

		// Donne le petit texte pour les �chantillons.
		public override string SampleText
		{
			get
			{
				string text = string.Format("{0}", this.nbFaces);
				if ( this.star )  text += " *";
				return text;
			}
		}

		// Retourne le nom d'un type donn�.
		public static string GetName(bool type)
		{
			if ( type )  return Res.Strings.Property.Regular.Star;
			else         return Res.Strings.Property.Regular.Norm;
		}

		// Retourne l'ic�ne pour un type donn�.
		public static string GetIconText(bool type)
		{
			if ( type )  return "RegularStar";
			else         return "RegularNorm";
		}


		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}


		// Nombre de poign�es.
		public override int TotalHandle(Objects.Abstract obj)
		{
			return 1;
		}

		// Indique si une poign�e est visible.
		public override bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			if ( !this.document.Modifier.IsPropertiesExtended(this.type) )
			{
				return false;
			}

			return this.star;
		}
		
		// Retourne la position d'une poign�e.
		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			return Point.Scale(obj.Handle(1).Position, obj.Handle(0).Position, this.deep);
		}

		// Modifie la position d'une poign�e.
		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			double d1 = Point.Distance(obj.Handle(1).Position, obj.Handle(0).Position);
			double d2 = Point.Distance(obj.Handle(1).Position, pos);
			if ( d1 == 0.0 )  this.Deep = 0.0;
			else              this.Deep = d2/d1;

			base.SetHandlePosition(obj, rank, pos);
		}
		
		
		// Effectue une copie de la propri�t�.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Regular p = property as Regular;
			p.nbFaces = this.nbFaces;
			p.star    = this.star;
			p.deep    = this.deep;
		}

		// Compare deux propri�t�s.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Regular p = property as Regular;
			if ( p.nbFaces != this.nbFaces )  return false;
			if ( p.star    != this.star    )  return false;
			if ( p.deep    != this.deep    )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override Panels.Abstract CreatePanel(Document document)
		{
			Panels.Abstract.StaticDocument = document;
			return new Panels.Regular(document);
		}


		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("NbFaces", this.nbFaces);
			info.AddValue("Star", this.star);
			if ( this.star )
			{
				info.AddValue("Deep", this.deep);
			}
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected Regular(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.nbFaces = info.GetInt32("NbFaces");
			this.star = info.GetBoolean("Star");
			if ( this.star )
			{
				this.deep = info.GetDouble("Deep");
			}
		}
		#endregion

	
		protected int					nbFaces;
		protected bool					star;
		protected double				deep;
	}
}
