using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyShadow représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyShadow : AbstractProperty
	{
		public PropertyShadow()
		{
			this.color  = Drawing.Color.FromARGB(0.0, 0.5, 0.5, 0.5);
			this.radius =  2.0;
			this.ox     =  1.0;
			this.oy     = -1.0;
		}

		// Couleur de l'ombre.
		public Drawing.Color Color
		{
			get { return this.color; }
			set { this.color = value; }
		}

		// Rayon de l'ombre.
		[XmlAttribute]
		public double Radius
		{
			get { return this.radius; }
			set { this.radius = value; }
		}

		// Offset x de l'ombre.
		[XmlAttribute]
		public double Ox
		{
			get { return this.ox; }
			set { this.ox = value; }
		}

		// Offset y de l'ombre.
		[XmlAttribute]
		public double Oy
		{
			get { return this.oy; }
			set { this.oy = value; }
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyShadow p = property as PropertyShadow;
			p.Color  = this.color;
			p.Radius = this.radius;
			p.Ox     = this.ox;
			p.Oy     = this.oy;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyShadow p = property as PropertyShadow;
			if ( p.Color  != this.color  )  return false;
			if ( p.Radius != this.radius )  return false;
			if ( p.Ox     != this.ox     )  return false;
			if ( p.Oy     != this.oy     )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelShadow();
		}


		// Effectue le rendu d'un chemin flou.
		public void Render(Drawing.Graphics graphics, IconContext iconContext, Drawing.Path path)
		{
			if ( this.color.A == 0 )  return;

			Drawing.Transform save = graphics.SaveTransform();
			graphics.TranslateTransform(this.ox, this.oy);

			if ( this.radius == 0 )
			{
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(this.color);
			}
			else
			{
				graphics.SmoothRenderer.Color = this.color;
				graphics.SmoothRenderer.SetParameters(this.radius*iconContext.ScaleX, this.radius*iconContext.ScaleY);
				graphics.SmoothRenderer.AddPath(path);
			}

			graphics.Transform = save;
		}


		protected Drawing.Color			color;
		protected double				radius;
		protected double				ox;
		protected double				oy;
	}
}
