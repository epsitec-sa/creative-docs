using System.Xml.Serialization;

namespace Epsitec.Common.NiceIcon
{
	public enum HandleType
	{
		Primary,		// poignée principale
		Secondary,		// poignée secondaire
		Starting,		// poignée de départ
		Ending,			// poignée d'arrivée
		Hide,			// poignée invisible
	}

	public enum HandleConstrainType
	{
		Symetric,		// symétrique
		Smooth,			// lisse
		Corner,			// anguleux
	}

	/// <summary>
	/// La classe Handle représente une poignée d'un objet graphique.
	/// </summary>
	public class Handle
	{
		public Handle()
		{
		}

		// Position de la poignée.
		public Drawing.Point Position
		{
			get
			{
				return this.position;
			}

			set
			{
				this.position = value;
			}
		}

		// Type de la poignée.
		[XmlAttribute]
		public HandleType Type
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

		// Type de la poignée.
		[XmlAttribute]
		public HandleConstrainType ConstrainType
		{
			get
			{
				return this.constrainType;
			}

			set
			{
				this.constrainType = value;
			}
		}

		// Etat "sélectionné" de la poignée.
		[XmlIgnore]
		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}

			set
			{
				this.isSelected = value;
			}
		}


		// Copie la poignée courante dans une poignée destination.
		public void CopyTo(Handle dst)
		{
			dst.Position      = this.Position;
			dst.Type          = this.Type;
			dst.ConstrainType = this.ConstrainType;
			dst.IsSelected    = this.IsSelected;
		}


		// Détecte si la souris est dans la poignée.
		public bool Detect(Drawing.Point pos)
		{
			if ( this.type == HandleType.Hide )  return false;
			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Left   = this.position.X - this.handleSize/2 - 3.0/this.scaleX;
			rect.Right  = this.position.X + this.handleSize/2 + 1.0/this.scaleX;
			rect.Bottom = this.position.Y - this.handleSize/2 - 3.0/this.scaleY;
			rect.Top    = this.position.Y + this.handleSize/2 + 1.0/this.scaleY;
			return rect.Contains(pos);
		}


		// Dessine la poignée.
		public void Draw(Drawing.Graphics graphics, IconContext iconContext)
		{
			if ( !this.isSelected )  return;

			this.scaleX = iconContext.ScaleX;
			this.scaleY = iconContext.ScaleY;
			this.handleSize = iconContext.HandleSize;

			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Left   = this.position.X-this.handleSize/2;
			rect.Right  = this.position.X+this.handleSize/2;
			rect.Bottom = this.position.Y-this.handleSize/2;
			rect.Top    = this.position.Y+this.handleSize/2;
			graphics.Align(ref rect);

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.scaleX;

			if ( this.type == HandleType.Primary )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromRGB(1,0,0));

				rect.Inflate(-0.5/this.scaleX, -0.5/this.scaleY);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromRGB(0,0,0));
			}

			if ( this.type == HandleType.Secondary )
			{
				rect.Inflate(-2/this.scaleX, -2/this.scaleY);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromRGB(1,0,0));

				rect.Inflate(-0.5/this.scaleX, -0.5/this.scaleY);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromRGB(0,0,0));
			}

			if ( this.type == HandleType.Starting )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromRGB(0,1,0));

				rect.Inflate(-0.5/this.scaleX, -0.5/this.scaleY);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromRGB(0,0,0));
			}

			if ( this.type == HandleType.Ending )
			{
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromRGB(0,1,0));

				rect.Inflate(-0.5/this.scaleX, -0.5/this.scaleY);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromRGB(0,0,0));

				rect.Inflate(2/this.scaleX, 2/this.scaleY);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromRGB(1,0,0));
				rect.Inflate(1/this.scaleX, 1/this.scaleY);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(Drawing.Color.FromRGB(1,0,0));
			}

			graphics.LineWidth = initialWidth;
		}


		protected bool					isSelected = false;
		protected double				handleSize;
		protected double				scaleX;
		protected double				scaleY;

		protected HandleType			type = HandleType.Primary;
		protected HandleConstrainType	constrainType = HandleConstrainType.Symetric;

		[XmlAttribute]
		protected Drawing.Point			position = new Drawing.Point(0, 0);
	}
}
