using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	using XmlAttribute = System.Xml.Serialization.XmlAttributeAttribute;

	public enum Type
	{
		Undefined,		// indéfini
		Surface,		// surface
		Stroke,			// trait
		Text,			// texte
		Image,			// image bitmap
	}

	public enum Aspect
	{
		Normal,			// forme normale
		Hilited,		// forme mise en évidence par un survol de la souris
		OverDashed,		// forme pointillée lorsque le trait n'existe pas
		Support,		// trait de support des points secondaires d'une courbe de Bézier
		Additional,		// objet additionnel pour le polygone quelconque
		InvisibleBox,	// utile juste pour calculer la bbox et pour la détection
	}

	/// <summary>
	/// La classe Shape contient une forme (surface ou trait).
	/// </summary>
	[System.Serializable]
	public class Shape
	{
		public Shape()
		{
		}


		// Type de la forme.
		public Type Type
		{
			get
			{
				return this.type;
			}

			set
			{
				this.type = value;
			}
		}

		// Aspect de la forme.
		public Aspect Aspect
		{
			get
			{
				return this.aspect;
			}

			set
			{
				this.aspect = value;
			}
		}

		// Chemin associé à la forme.
		public Path Path
		{
			get
			{
				return this.path;
			}

			set
			{
				this.path = value;
			}
		}

		// Indique si la forme est visible.
		public bool IsVisible
		{
			get
			{
				return this.isVisible;
			}

			set
			{
				this.isVisible = value;
			}
		}

		// Spécifie l'objet texte.
		public void SetTextObject(Objects.Abstract obj)
		{
			this.obj = obj;
			this.Type = Type.Text;
			this.IsVisible = true;
		}

		// Spécifie l'objet image.
		public void SetImageObject(Objects.Abstract obj)
		{
			this.obj = obj;
			this.Type = Type.Image;
			this.IsVisible = true;
		}

		// Donne l'objet texte.
		public Objects.Abstract Object
		{
			get
			{
				return this.obj;
			}
		}
		
		// Donne la propriété pour une surface.
		// Type Properties.Gradient ou Properties.Font.
		public void SetPropertySurface(Properties.Abstract surface)
		{
			this.propertySurface = surface;
			this.Type = Type.Surface;
			this.PropertyVisibility();
		}

		// Donne les propriétés pour un trait.
		public void SetPropertyStroke(Properties.Line stroke, Properties.Gradient surface)
		{
			this.propertyStroke = stroke;
			this.propertySurface = surface;
			this.Type = Type.Stroke;
			this.PropertyVisibility();
		}

		public Properties.Abstract PropertySurface
		{
			get
			{
				return this.propertySurface;
			}
		}
		
		public Properties.Line PropertyStroke
		{
			get
			{
				return this.propertyStroke;
			}
		}

		// Met la visibilité définie selon les propriétés.
		protected void PropertyVisibility()
		{
			if ( this.Type == Type.Surface )
			{
				if ( this.propertySurface == null )
				{
					this.IsVisible = true;
				}
				else
				{
					this.IsVisible = this.propertySurface.IsVisible;
				}
			}

			if ( this.Type == Type.Stroke )
			{
				if ( this.propertyStroke == null || this.propertySurface == null )
				{
					this.IsVisible = true;
				}
				else
				{
					this.IsVisible = this.propertyStroke.IsVisible && this.propertySurface.IsVisible;
				}
			}
		}
		
		// Met la visibilité définie selon les propriétés.
		protected void PropertyVisibilityInvert()
		{
			if ( this.Type == Type.Surface )
			{
				if ( this.propertySurface == null )
				{
					this.IsVisible = true;
				}
				else
				{
					this.IsVisible = !this.propertySurface.IsVisible;
				}
			}

			if ( this.Type == Type.Stroke )
			{
				if ( this.propertyStroke == null || this.propertySurface == null )
				{
					this.IsVisible = true;
				}
				else
				{
					this.IsVisible = !this.propertyStroke.IsVisible || !this.propertySurface.IsVisible;
				}
			}
		}
		

		// Modifie une liste de formes pour mettre en évidence.
		public static void Hilited(Shape[] shapes)
		{
			foreach ( Shape shape in shapes )
			{
				if ( shape == null )  continue;
				if ( shape.Aspect == Aspect.InvisibleBox )  continue;

				if ( shape.Aspect == Aspect.Support )
				{
					shape.IsVisible = false;
					continue;
				}

				if ( shape.Type == Type.Surface )
				{
					shape.PropertyVisibility();
					shape.Aspect = Aspect.Hilited;
				}

				if ( shape.Type == Type.Stroke )
				{
					shape.IsVisible = true;
					shape.Aspect = Aspect.Hilited;
				}

				if ( shape.Type == Type.Text )
				{
					shape.IsVisible = false;
				}

				if ( shape.Type == Type.Image )
				{
					shape.IsVisible = false;
				}
			}
		}
		
		// Modifie une liste de formes pour mettre en pointillé forcé.
		public static void OverDashed(Shape[] shapes)
		{
			foreach ( Shape shape in shapes )
			{
				if ( shape == null )  continue;
				if ( shape.Aspect == Aspect.InvisibleBox )  continue;

				if ( shape.Aspect == Aspect.Support )
				{
					shape.IsVisible = false;
					continue;
				}

				if ( shape.Type == Type.Surface )
				{
					shape.IsVisible = false;
				}

				if ( shape.Type == Type.Stroke )
				{
					shape.PropertyVisibilityInvert();
					shape.Aspect = Aspect.OverDashed;
				}

				if ( shape.Type == Type.Text )
				{
					shape.IsVisible = false;
				}

				if ( shape.Type == Type.Image )
				{
					shape.IsVisible = false;
				}
			}
		}


		protected Type							type = Type.Undefined;
		protected Aspect						aspect = Aspect.Normal;
		protected Path							path = null;
		protected bool							isVisible = true;
		protected Objects.Abstract				obj = null;
		protected Properties.Abstract			propertySurface = null;
		protected Properties.Line				propertyStroke = null;
	}
}
