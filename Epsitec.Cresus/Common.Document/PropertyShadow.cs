using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe PropertyShadow représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class PropertyShadow : AbstractProperty
	{
		public PropertyShadow(Document document) : base(document)
		{
			this.color  = Color.FromARGB(0.0, 0.5, 0.5, 0.5);
			this.radius =  2.0;
			this.ox     =  1.0;
			this.oy     = -1.0;
		}

		// Couleur de l'ombre.
		public Color Color
		{
			get
			{
				return this.color;
			}
			
			set
			{
				if ( this.color != value )
				{
					this.NotifyBefore();
					this.color = value;
					this.NotifyAfter();
				}
			}
		}

		// Rayon de l'ombre.
		public double Radius
		{
			get
			{
				return this.radius;
			}
			
			set
			{
				if ( this.radius != value )
				{
					this.NotifyBefore();
					this.radius = value;
					this.NotifyAfter();
				}
			}
		}

		// Offset x de l'ombre.
		public double Ox
		{
			get
			{
				return this.ox;
			}
			
			set
			{
				if ( this.ox != value )
				{
					this.NotifyBefore();
					this.ox = value;
					this.NotifyAfter();
				}
			}
		}

		// Offset y de l'ombre.
		public double Oy
		{
			get
			{
				return this.oy;
			}
			
			set
			{
				if ( this.oy != value )
				{
					this.NotifyBefore();
					this.oy = value;
					this.NotifyAfter();
				}
			}
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyShadow p = property as PropertyShadow;
			p.color  = this.color;
			p.radius = this.radius;
			p.ox     = this.ox;
			p.oy     = this.oy;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyShadow p = property as PropertyShadow;
			if ( p.color  != this.color  )  return false;
			if ( p.radius != this.radius )  return false;
			if ( p.ox     != this.ox     )  return false;
			if ( p.oy     != this.oy     )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel(Document document)
		{
			return new PanelShadow(document);
		}


		// Effectue le rendu d'un chemin flou.
		public void Render(Graphics graphics, DrawingContext drawingContext, Path path)
		{
			if ( this.color.A == 0 )  return;

			Transform save = graphics.Transform;
			graphics.TranslateTransform(this.ox, this.oy);

			if ( this.radius == 0 )
			{
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(this.color);
			}
			else
			{
				graphics.SmoothRenderer.Color = this.color;
				graphics.SmoothRenderer.SetParameters(this.radius*drawingContext.ScaleX, this.radius*drawingContext.ScaleY);
				graphics.SmoothRenderer.AddPath(path);
			}

			graphics.Transform = save;
		}


		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Color", this.color);
			info.AddValue("Radius", this.radius);
			info.AddValue("Ox", this.ox);
			info.AddValue("Oy", this.oy);
		}

		// Constructeur qui désérialise la propriété.
		protected PropertyShadow(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.color = (Color) info.GetValue("Color", typeof(Color));
			this.radius = info.GetDouble("Radius");
			this.ox = info.GetDouble("Ox");
			this.oy = info.GetDouble("Oy");
		}
		#endregion

	
		protected Color					color;
		protected double				radius;
		protected double				ox;
		protected double				oy;
	}
}
