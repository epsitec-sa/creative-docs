using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Shadow représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Shadow : Abstract
	{
		public Shadow(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			this.color  = Drawing.Color.FromARGB(0.0, 0.5, 0.5, 0.5);
			this.radius =  2.0;
			this.ox     =  1.0;
			this.oy     = -1.0;
		}

		// Couleur de l'ombre.
		public Drawing.Color Color
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
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Shadow p = property as Shadow;
			p.color  = this.color;
			p.radius = this.radius;
			p.ox     = this.ox;
			p.oy     = this.oy;
		}

		// Compare deux propriétés.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Shadow p = property as Shadow;
			if ( p.color  != this.color  )  return false;
			if ( p.radius != this.radius )  return false;
			if ( p.ox     != this.ox     )  return false;
			if ( p.oy     != this.oy     )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override Panels.Abstract CreatePanel(Document document)
		{
			return new Panels.Shadow(document);
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
		protected Shadow(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.color = (Drawing.Color) info.GetValue("Color", typeof(Drawing.Color));
			this.radius = info.GetDouble("Radius");
			this.ox = info.GetDouble("Ox");
			this.oy = info.GetDouble("Oy");
		}
		#endregion

	
		protected Drawing.Color			color;
		protected double				radius;
		protected double				ox;
		protected double				oy;
	}
}
