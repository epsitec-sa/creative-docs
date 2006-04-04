//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using BundleAttribute  = Support.BundleAttribute;
	
	/// <summary>
	/// La classe Panel repr�sente un widget qui permet de grouper d'autres widgets
	/// tout en limitant la surface affich�e � une ouverture (aperture). Un Panel
	/// s'utilise en principe en association avec un Scrollable.
	/// </summary>
	public class Panel : AbstractGroup
	{
		public Panel()
		{
			this.aperture = Drawing.Rectangle.MaxValue;
		}
		
		public Panel(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		
		[Bundle]	public Drawing.Size			SurfaceSize
		{
			//	La taille de la surface indique une taille minimale id�ale, en-dessous de laquelle
			//	le contenu ne sera plus enti�rement visible. Scrollable utiliser cette information
			//	pour d�cider du moment � partir duquel afficher les ascenceurs.
			
			//	Par d�faut, la taille de la surface est z�ro, ce qui signifie qu'il n'y aura jamais
			//	d'ascenceurs montr�s, quelle que soit la taille du panel.
			
			get
			{
				return this.surface_size;
			}
			set
			{
				if (this.surface_size != value)
				{
					this.surface_size = value;
					this.OnSurfaceSizeChanged ();
				}
			}
		}
		
		[Bundle]	public bool					IsAutoFitting
		{
			get
			{
				return this.is_auto_fitting;
			}
			set
			{
				if (this.is_auto_fitting != value)
				{
					this.is_auto_fitting = value;
					
					this.OnIsAutoFittingChanged ();
					
					if (this.is_auto_fitting)
					{
						this.ForceLayout ();
					}
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
				this.SurfaceSize = new Drawing.Size (value, this.surface_size.Height);
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
				this.SurfaceSize = new Drawing.Size (this.surface_size.Width, value);
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
		
		
		protected override void UpdateMinMaxBasedOnDockedChildren(Widget[] children)
		{
			base.UpdateMinMaxBasedOnDockedChildren (children);
			
			if (this.is_auto_fitting)
			{
				this.SurfaceSize = this.RealMinSize;
			}
		}

		
		protected virtual void OnSurfaceSizeChanged()
		{
			if (this.SurfaceSizeChanged != null)
			{
				this.SurfaceSizeChanged (this);
			}
		}
		
		protected virtual void OnIsAutoFittingChanged()
		{
			if (this.IsAutoFittingChanged != null)
			{
				this.IsAutoFittingChanged (this);
			}
		}
		
		
		public event Support.EventHandler		SurfaceSizeChanged;
		public event Support.EventHandler		IsAutoFittingChanged;
		
		
		protected Drawing.Rectangle				aperture;
		protected Drawing.Size					surface_size;
		protected bool							is_auto_fitting;
	}
}
