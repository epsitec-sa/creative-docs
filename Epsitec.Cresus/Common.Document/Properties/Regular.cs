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

		public override string SampleText
		{
			//	Donne le petit texte pour les �chantillons.
			get
			{
				string star = this.star ? " *" : "";
				return string.Concat(Res.Strings.Property.Regular.Short.Faces, this.nbFaces.ToString(), star);
			}
		}

		public override void PutStyleResume(System.Text.StringBuilder builder)
		{
			//	Construit le texte r�sum� d'un style pour une propri�t�.
			this.PutStyleResumePrefix(builder);
			builder.Append(this.SampleText);
			this.PutStyleResumePostfix(builder);
		}

		public static string GetName(bool type)
		{
			//	Retourne le nom d'un type donn�.
			if ( type )  return Res.Strings.Property.Regular.Star;
			else         return Res.Strings.Property.Regular.Norm;
		}

		public static string GetIconText(bool type)
		{
			//	Retourne l'ic�ne pour un type donn�.
			if ( type )  return "RegularStar";
			else         return "RegularNorm";
		}


		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propri�t� modifie la bbox de l'objet.
			get { return true; }
		}


		public override int TotalHandle(Objects.Abstract obj)
		{
			//	Nombre de poign�es.
			return 1;
		}

		public override bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			//	Indique si une poign�e est visible.
			if ( !this.document.Modifier.IsPropertiesExtended(this.type) )
			{
				return false;
			}

			return this.star;
		}
		
		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			//	Retourne la position d'une poign�e.
			return Point.Scale(obj.Handle(1).Position, obj.Handle(0).Position, this.deep);
		}

		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			//	Modifie la position d'une poign�e.
			double d1 = Point.Distance(obj.Handle(1).Position, obj.Handle(0).Position);
			double d2 = Point.Distance(obj.Handle(1).Position, pos);
			if ( d1 == 0.0 )  this.Deep = 0.0;
			else              this.Deep = d2/d1;

			base.SetHandlePosition(obj, rank, pos);
		}
		
		
		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propri�t�.
			base.CopyTo(property);
			Regular p = property as Regular;
			p.nbFaces = this.nbFaces;
			p.star    = this.star;
			p.deep    = this.deep;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propri�t�s.
			if ( !base.Compare(property) )  return false;

			Regular p = property as Regular;
			if ( p.nbFaces != this.nbFaces )  return false;
			if ( p.star    != this.star    )  return false;
			if ( p.deep    != this.deep    )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Cr�e le panneau permettant d'�diter la propri�t�.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Regular(document);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise la propri�t�.
			base.GetObjectData(info, context);

			info.AddValue("NbFaces", this.nbFaces);
			info.AddValue("Star", this.star);
			if ( this.star )
			{
				info.AddValue("Deep", this.deep);
			}
		}

		protected Regular(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui d�s�rialise la propri�t�.
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
