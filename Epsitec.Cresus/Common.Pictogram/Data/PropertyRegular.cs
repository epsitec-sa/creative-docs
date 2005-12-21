using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyRegular repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	public class PropertyRegular : AbstractProperty
	{
		public PropertyRegular()
		{
			this.nbFaces = 6;
			this.star    = false;
			this.deep    = 0.5;
		}

		[XmlAttribute]
		public int NbFaces
		{
			get { return this.nbFaces; }
			set { this.nbFaces = value; }
		}

		[XmlAttribute]
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
					this.star = value;
					this.OnChanged();
				}
			}
		}

		[XmlAttribute]
		public double Deep
		{
			get
			{
				return this.deep;
			}

			set
			{
				if ( this.deep != value )
				{
					this.deep = value;
					this.OnChanged();
				}
			}
		}

		public string GetListName()
		{
			//	D�termine le nom de la propri�t� dans la liste (Lister).
			return string.Format("{0}", this.nbFaces);
		}

		[XmlIgnore]
		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propri�t� modifie la bbox de l'objet.
			get { return true; }
		}

		public override void CopyTo(AbstractProperty property)
		{
			//	Effectue une copie de la propri�t�.
			base.CopyTo(property);
			PropertyRegular p = property as PropertyRegular;
			p.NbFaces = this.nbFaces;
			p.Star    = this.star;
			p.Deep    = this.deep;
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propri�t�s.
			if ( !base.Compare(property) )  return false;

			PropertyRegular p = property as PropertyRegular;
			if ( p.NbFaces != this.nbFaces )  return false;
			if ( p.Star    != this.star    )  return false;
			if ( p.Deep    != this.deep    )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Cr�e le panneau permettant d'�diter la propri�t�.
			return new PanelRegular(drawer);
		}


		protected int					nbFaces;
		protected bool					star;
		protected double				deep;
	}
}
