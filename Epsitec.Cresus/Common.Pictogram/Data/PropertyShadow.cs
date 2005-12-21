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

		public Drawing.Color Color
		{
			//	Couleur de l'ombre.
			get { return this.color; }
			set { this.color = value; }
		}

		[XmlAttribute]
		public double Radius
		{
			//	Rayon de l'ombre.
			get { return this.radius; }
			set { this.radius = value; }
		}

		[XmlAttribute]
		public double Ox
		{
			//	Offset x de l'ombre.
			get { return this.ox; }
			set { this.ox = value; }
		}

		[XmlAttribute]
		public double Oy
		{
			//	Offset y de l'ombre.
			get { return this.oy; }
			set { this.oy = value; }
		}

		public override void CopyTo(AbstractProperty property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			PropertyShadow p = property as PropertyShadow;
			p.Color  = this.color;
			p.Radius = this.radius;
			p.Ox     = this.ox;
			p.Oy     = this.oy;
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			PropertyShadow p = property as PropertyShadow;
			if ( p.Color  != this.color  )  return false;
			if ( p.Radius != this.radius )  return false;
			if ( p.Ox     != this.ox     )  return false;
			if ( p.Oy     != this.oy     )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			return new PanelShadow(drawer);
		}


		public void Render(Drawing.Graphics graphics, IconContext iconContext, Drawing.Path path)
		{
			//	Effectue le rendu d'un chemin flou.
			if ( this.color.A == 0 )  return;

			Drawing.Transform save = graphics.Transform;
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
