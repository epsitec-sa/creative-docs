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
		OnlyDetect,		// utile juste pour la détection
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


		public Type Type
		{
			//	Type de la forme.
			get
			{
				return this.type;
			}

			set
			{
				this.type = value;
			}
		}

		public Aspect Aspect
		{
			//	Aspect de la forme.
			get
			{
				return this.aspect;
			}

			set
			{
				this.aspect = value;
			}
		}

		public Path Path
		{
			//	Chemin associé à la forme.
			get
			{
				return this.path;
			}

			set
			{
				this.path = value;
			}
		}

		public bool IsMisc
		{
			//	Indique si la forme est ornementale.
			get
			{
				return this.isMisc;
			}

			set
			{
				this.isMisc = value;
			}
		}

		public bool IsVisible
		{
			//	Indique si la forme est visible.
			get
			{
				return this.isVisible;
			}

			set
			{
				this.isVisible = value;
			}
		}

		public bool IsLinkWithNext
		{
			//	Indique si la forme est liée à la suivante.
			get
			{
				return this.isLinkWithNext;
			}

			set
			{
				this.isLinkWithNext = value;
			}
		}

		public FillMode FillMode
		{
			//	Indique le mode de remplissage.
			get
			{
				return this.fillMode;
			}

			set
			{
				this.fillMode = value;
			}
		}

		public void SetTextObject(Objects.Abstract obj)
		{
			//	Spécifie l'objet texte.
			this.obj = obj;
			this.Type = Type.Text;
			this.IsVisible = true;
		}

		public void SetImageObject(Objects.Abstract obj)
		{
			//	Spécifie l'objet image.
			this.obj = obj;
			this.Type = Type.Image;
			this.IsVisible = true;
		}

		public Objects.Abstract Object
		{
			//	Donne l'objet texte.
			get
			{
				return this.obj;
			}
		}
		
		public void SetPropertySurface(IPaintPort port, Properties.Abstract surface)
		{
			//	Donne la propriété pour une surface.
			//	Type Properties.Gradient ou Properties.Font.
			this.propertySurface = surface;
			this.Type = Type.Surface;
			this.fillMode = FillMode.EvenOdd;  // mode par défaut pour les surfaces
			this.PropertyVisibility(port);
		}

		public void SetPropertyStroke(IPaintPort port, Properties.Line stroke, Properties.Gradient surface)
		{
			//	Donne les propriétés pour un trait.
			this.propertyStroke = stroke;
			this.propertySurface = surface;
			this.Type = Type.Stroke;
			this.fillMode = FillMode.NonZero;  // mode par défaut pour les traits
			this.PropertyVisibility(port);
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

		protected void PropertyVisibility(IPaintPort port)
		{
			//	Met la visibilité définie selon les propriétés.
			if ( this.Type == Type.Surface )
			{
				if ( this.propertySurface == null )
				{
					this.IsVisible = true;
				}
				else
				{
					this.IsVisible = this.propertySurface.IsVisible(port);
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
					this.IsVisible = this.propertyStroke.IsVisible(port) && this.propertySurface.IsVisible(port);
				}
			}
		}
		
		protected void PropertyVisibilityInvert(IPaintPort port)
		{
			//	Met la visibilité définie selon les propriétés.
			if ( this.Type == Type.Surface )
			{
				if ( this.propertySurface == null )
				{
					this.IsVisible = true;
				}
				else
				{
					this.IsVisible = !this.propertySurface.IsVisible(port);
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
					this.IsVisible = !this.propertyStroke.IsVisible(port) || !this.propertySurface.IsVisible(port);
				}
			}
		}
		

		public static void Hilited(IPaintPort port, Shape[] shapes)
		{
			//	Modifie une liste de formes pour mettre en évidence.
			foreach ( Shape shape in shapes )
			{
				if ( shape == null )  continue;
				if ( shape.Aspect == Aspect.InvisibleBox )  continue;
				if ( shape.Aspect == Aspect.OnlyDetect )  continue;

				if ( shape.Aspect == Aspect.Support )
				{
					shape.IsVisible = false;
					continue;
				}

				if ( shape.Type == Type.Surface )
				{
					shape.PropertyVisibility(port);
					shape.Aspect = Aspect.Hilited;
				}

				if ( shape.Type == Type.Stroke )
				{
					if ( shape.isMisc &&
						 (!shape.propertyStroke.IsVisible(port) ||
						  !shape.propertySurface.IsVisible(port) ) )
					{
						shape.IsVisible = false;
						continue;
					}

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
		
		public static void OverDashed(IPaintPort port, Shape[] shapes)
		{
			//	Modifie une liste de formes pour mettre en pointillé forcé.
			foreach ( Shape shape in shapes )
			{
				if ( shape == null )  continue;
				if ( shape.Aspect == Aspect.InvisibleBox )  continue;
				if ( shape.Aspect == Aspect.OnlyDetect )  continue;

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
					if ( shape.isMisc &&
						 (!shape.propertyStroke.IsVisible(port) ||
						  !shape.propertySurface.IsVisible(port) ) )
					{
						shape.IsVisible = false;
						continue;
					}

					shape.PropertyVisibilityInvert(port);
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
		protected bool							isMisc = false;
		protected bool							isVisible = true;
		protected bool							isLinkWithNext = false;
		protected Objects.Abstract				obj = null;
		protected Properties.Abstract			propertySurface = null;
		protected Properties.Line				propertyStroke = null;
		protected FillMode						fillMode = FillMode.NonZero;
	}
}
