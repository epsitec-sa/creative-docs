using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyRegular représente une propriété d'un objet graphique.
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

		// Détermine le nom de la propriété dans la liste (Lister).
		public string GetListName()
		{
			return string.Format("{0}", this.nbFaces);
		}

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		[XmlIgnore]
		public override bool AlterBoundingBox
		{
			get { return true; }
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyRegular p = property as PropertyRegular;
			p.NbFaces = this.nbFaces;
			p.Star    = this.star;
			p.Deep    = this.deep;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyRegular p = property as PropertyRegular;
			if ( p.NbFaces != this.nbFaces )  return false;
			if ( p.Star    != this.star    )  return false;
			if ( p.Deep    != this.deep    )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelRegular();
		}


		protected int					nbFaces;
		protected bool					star;
		protected double				deep;
	}
}
