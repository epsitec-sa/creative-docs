//	Copyright � 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Viewport repr�sente un widget qui permet de grouper d'autres widgets
	/// tout en limitant la surface affich�e � une ouverture (aperture). Un Viewport
	/// s'utilise en principe en association avec un Scrollable.
	/// </summary>
	public class Viewport : AbstractGroup
	{
		public Viewport()
		{
			this.aperture = Drawing.Rectangle.MaxValue;
		}
		
		public Viewport(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public Drawing.Size						SurfaceSize
		{
			//	La taille de la surface indique une taille minimale id�ale, en-dessous de laquelle
			//	le contenu ne sera plus enti�rement visible. Scrollable utiliser cette information
			//	pour d�cider du moment � partir duquel afficher les ascenceurs.
			
			//	Par d�faut, la taille de la surface est z�ro, ce qui signifie qu'il n'y aura jamais
			//	d'ascenceurs montr�s, quelle que soit la taille du panel.
			
			get
			{
				return this.surfaceSize;
			}
			set
			{
				if (this.surfaceSize != value)
				{
					this.surfaceSize = value;
					this.OnSurfaceSizeChanged ();
				}
			}
		}
		
		public double							SurfaceWidth
		{
			get
			{
				return this.SurfaceSize.Width;
			}
			set
			{
				this.SurfaceSize = new Drawing.Size (value, this.surfaceSize.Height);
			}
		}
		
		public double							SurfaceHeight
		{
			get
			{
				return this.SurfaceSize.Height;
			}
			set
			{
				this.SurfaceSize = new Drawing.Size (this.surfaceSize.Width, value);
			}
		}

		public Drawing.Rectangle				Aperture
		{
			//	L'ouverture, si elle est d�finie pour un panel, permet de d�finir quelle
			//	partie est actuellement visible, relativement au syst�me de coordonn�es
			//	client du panel.

			//	Pour indiquer que le panel complet est visible, il faut assigner � Aperture
			//	la valeur Drawing.Rectangle.MaxValue.

			get
			{
				return this.aperture;
			}

			set
			{
				if (this.aperture != value)
				{
					this.aperture = value;
					this.Invalidate ();
				}
			}
		}

		public bool								IsAutoFitting
		{
			get
			{
				return this.isAutoFitting;
			}
			set
			{
				if (this.isAutoFitting != value)
				{
					this.isAutoFitting = value;

					this.OnIsAutoFittingChanged ();

					if (this.isAutoFitting)
					{
						//-						this.ForceLayout ();
					}
				}

			}
		}
		
		
		public override bool HitTest(Drawing.Point point)
		{
			if (this.aperture == Drawing.Rectangle.MaxValue)
			{
				return base.HitTest (point);
			}
			else
			{
				Drawing.Rectangle rect = this.aperture;
				rect.Offset (this.ActualLocation);
				return rect.Contains (point);
			}
		}

		public override Drawing.Rectangle GetClipBounds()
		{
			Drawing.Rectangle clip = base.GetClipBounds ();
			
			if (this.aperture == Drawing.Rectangle.MaxValue)
			{
				//	Si aucune ouverture n'a �t� d�finie, elle a une taille infinie et on
				//	peut donc utiliser la r�gion de clipping retourn�e par la super-classe.
			}
			else
			{
				//	Une ouverture limite la surface visible; il faut donc r�aliser
				//	l'intersection de la r�gion compl�te avec l'ouverture.
				
				clip = Drawing.Rectangle.Intersection (clip, this.aperture);
			}
			
			return clip;
		}


		protected override void MeasureMinMax(ref Drawing.Size min, ref Drawing.Size max)
		{
			base.MeasureMinMax (ref min, ref max);
		}

		protected override void  ArrangeOverride(Epsitec.Common.Widgets.Layouts.LayoutContext context)
		{
 			base.ArrangeOverride(context);
			
			if (this.isAutoFitting)
			{
				Layouts.LayoutMeasure measureDx = Layouts.LayoutMeasure.GetWidth (this);
				Layouts.LayoutMeasure measureDy = Layouts.LayoutMeasure.GetHeight (this);
				
				if ((measureDx != null) &&
					(measureDy != null) &&
					(double.IsNaN (measureDx.Min) == false) &&
					(double.IsNaN (measureDy.Min) == false))
				{
					this.SurfaceSize = new Drawing.Size (measureDx.Min, measureDy.Min);
				}
			}
		}
		
		protected virtual void OnSurfaceSizeChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("SurfaceSizeChanged");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual void OnIsAutoFittingChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("IsAutoFittingChanged");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		
		public event EventHandler				SurfaceSizeChanged
		{
			add
			{
				this.AddUserEventHandler("SurfaceSizeChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("SurfaceSizeChanged", value);
			}
		}

		public event EventHandler				IsAutoFittingChanged
		{
			add
			{
				this.AddUserEventHandler("IsAutoFittingChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("IsAutoFittingChanged", value);
			}
		}

		
		
		protected Drawing.Rectangle				aperture;
		protected Drawing.Size					surfaceSize;
		protected bool							isAutoFitting;
	}
}
