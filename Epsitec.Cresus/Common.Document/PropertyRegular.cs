using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe PropertyRegular repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class PropertyRegular : AbstractProperty
	{
		public PropertyRegular(Document document) : base(document)
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

		// D�termine le nom de la propri�t� dans la liste (Lister).
		public string GetListName()
		{
			return string.Format("{0}", this.nbFaces);
		}

		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}


		// Nombre de poign�es.
		public override int TotalHandle(AbstractObject obj)
		{
			return 1;
		}

		// Indique si une poign�e est visible.
		public override bool IsHandleVisible(AbstractObject obj, int rank)
		{
			return this.star;
		}
		
		// Retourne la position d'une poign�e.
		public override Point GetHandlePosition(AbstractObject obj, int rank)
		{
			return Point.Scale(obj.Handle(1).Position, obj.Handle(0).Position, this.deep);
		}

		// Modifie la position d'une poign�e.
		public override void SetHandlePosition(AbstractObject obj, int rank, Point pos)
		{
			double d1 = Point.Distance(obj.Handle(1).Position, obj.Handle(0).Position);
			double d2 = Point.Distance(obj.Handle(1).Position, pos);
			if ( d1 == 0.0 )  this.Deep = 0.0;
			else              this.Deep = d2/d1;

			base.SetHandlePosition(obj, rank, pos);
		}
		
		
		// Effectue une copie de la propri�t�.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyRegular p = property as PropertyRegular;
			p.nbFaces = this.nbFaces;
			p.star    = this.star;
			p.deep    = this.deep;
		}

		// Compare deux propri�t�s.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyRegular p = property as PropertyRegular;
			if ( p.nbFaces != this.nbFaces )  return false;
			if ( p.star    != this.star    )  return false;
			if ( p.deep    != this.deep    )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override AbstractPanel CreatePanel(Document document)
		{
			return new PanelRegular(document);
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
		protected PropertyRegular(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.nbFaces = info.GetInt32("NbFaces");
			this.star = info.GetBoolean("Star");
			if ( this.star )
			{
				this.deep = info.GetDouble("Deep");
			}
			else
			{
				this.deep = 0.5;
			}
		}
		#endregion

	
		protected int					nbFaces;
		protected bool					star;
		protected double				deep;
	}
}
