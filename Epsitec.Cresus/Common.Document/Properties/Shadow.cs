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

		protected override void Initialize()
		{
			base.Initialize ();
			this.color  = Drawing.RichColor.FromAlphaRgb(0.0, 0.5, 0.5, 0.5);
			this.radius =  2.0;
			this.ox     =  1.0;
			this.oy     = -1.0;
		}

		public Drawing.RichColor Color
		{
			//	Couleur de l'ombre.
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

		public double Radius
		{
			//	Rayon de l'ombre.
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

		public double Ox
		{
			//	Offset x de l'ombre.
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

		public double Oy
		{
			//	Offset y de l'ombre.
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

		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			Shadow p = property as Shadow;
			p.color  = this.color;
			p.radius = this.radius;
			p.ox     = this.ox;
			p.oy     = this.oy;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Shadow p = property as Shadow;
			if ( p.color  != this.color  )  return false;
			if ( p.radius != this.radius )  return false;
			if ( p.ox     != this.ox     )  return false;
			if ( p.oy     != this.oy     )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Shadow(document);
		}


		public void Render(Graphics graphics, DrawingContext drawingContext, Path path)
		{
			//	Effectue le rendu d'un chemin flou.
			if ( this.color.A == 0 )  return;

			Transform save = graphics.Transform;
			graphics.TranslateTransform(this.ox, this.oy);

			if ( this.radius == 0 )
			{
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(this.color.Basic);
			}
			else
			{
				graphics.SmoothRenderer.Color = this.color.Basic;
				graphics.SmoothRenderer.SetParameters(this.radius*drawingContext.ScaleX, this.radius*drawingContext.ScaleY);
				graphics.SmoothRenderer.AddPath(path);
			}

			graphics.Transform = save;
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			base.GetObjectData(info, context);

			info.AddValue("Color", this.color);
			info.AddValue("Radius", this.radius);
			info.AddValue("Ox", this.ox);
			info.AddValue("Oy", this.oy);
		}

		protected Shadow(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise la propriété.
			if ( this.document.IsRevisionGreaterOrEqual(1,0,22) )
			{
				this.color = (Drawing.RichColor) info.GetValue("Color", typeof(Drawing.RichColor));
			}
			else
			{
				Drawing.Color c = (Drawing.Color) info.GetValue("Color", typeof(Drawing.Color));
				this.color = new RichColor(c);
			}

			this.radius = info.GetDouble("Radius");
			this.ox = info.GetDouble("Ox");
			this.oy = info.GetDouble("Oy");
		}
		#endregion

	
		protected Drawing.RichColor		color;
		protected double				radius;
		protected double				ox;
		protected double				oy;
	}
}
