using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Proxies
{
	public class Geometry : Abstract
	{
		public Geometry(ProxyManager manager) : base (manager)
		{
		}

		public override int Rank
		{
			//	Retourne le rang de ce proxy parmi la liste de tous les proxies.
			//	Plus le num�ro est petit, plus le proxy appara�tra haut dans la
			//	liste.
			get
			{
				return 3;
			}
		}

		public override string IconName
		{
			get
			{
				return "PropertyGeometry";
			}
		}

		public double LeftMargin
		{
			get
			{
				return (double) this.GetValue(Geometry.LeftMarginProperty);
			}
			set
			{
				this.SetValue(Geometry.LeftMarginProperty, value);
			}
		}

		public double RightMargin
		{
			get
			{
				return (double) this.GetValue(Geometry.RightMarginProperty);
			}
			set
			{
				this.SetValue(Geometry.RightMarginProperty, value);
			}
		}

		public double TopMargin
		{
			get
			{
				return (double) this.GetValue(Geometry.TopMarginProperty);
			}
			set
			{
				this.SetValue(Geometry.TopMarginProperty, value);
			}
		}

		public double BottomMargin
		{
			get
			{
				return (double) this.GetValue(Geometry.BottomMarginProperty);
			}
			set
			{
				this.SetValue(Geometry.BottomMarginProperty, value);
			}
		}

		public double OriginX
		{
			get
			{
				return (double) this.GetValue(Geometry.OriginXProperty);
			}
			set
			{
				this.SetValue(Geometry.OriginXProperty, value);
			}
		}

		public double OriginY
		{
			get
			{
				return (double) this.GetValue(Geometry.OriginYProperty);
			}
			set
			{
				this.SetValue(Geometry.OriginYProperty, value);
			}
		}

		public double Width
		{
			get
			{
				return (double) this.GetValue(Geometry.WidthProperty);
			}
			set
			{
				this.SetValue(Geometry.WidthProperty, value);
			}
		}

		public double Height
		{
			get
			{
				return (double) this.GetValue(Geometry.HeightProperty);
			}
			set
			{
				this.SetValue(Geometry.HeightProperty, value);
			}
		}


		protected override void InitializePropertyValues()
		{
			//	Cette m�thode est appel�e par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propri�t�s du widget s�lectionn�
			//	pour pouvoir ensuite travailler dessus :
			if (this.ObjectModifier.HasMargins(this.DefaultWidget))
			{
				Margins margins = this.ObjectModifier.GetMargins(this.DefaultWidget);

				this.LeftMargin   = margins.Left;
				this.RightMargin  = margins.Right;
				this.TopMargin    = margins.Top;
				this.BottomMargin = margins.Bottom;
			}

			if (this.ObjectModifier.HasBounds(this.DefaultWidget))
			{
				Rectangle bounds = this.ObjectModifier.GetBounds(this.DefaultWidget);

				if (this.ObjectModifier.HasBounds(this.DefaultWidget, ObjectModifier.BoundsMode.OriginX))
				{
					this.OriginX = bounds.Left;
				}

				if (this.ObjectModifier.HasBounds(this.DefaultWidget, ObjectModifier.BoundsMode.OriginY))
				{
					this.OriginY = bounds.Bottom;
				}

				if (this.ObjectModifier.HasBounds(this.DefaultWidget, ObjectModifier.BoundsMode.Width))
				{
					this.Width = bounds.Width;
				}

				if (this.ObjectModifier.HasBounds(this.DefaultWidget, ObjectModifier.BoundsMode.Height))
				{
					this.Height = bounds.Height;
				}
			}
			else
			{
				if (this.ObjectModifier.HasWidth(this.DefaultWidget))
				{
					this.Width = this.ObjectModifier.GetWidth(this.DefaultWidget);
				}

				if (this.ObjectModifier.HasHeight(this.DefaultWidget))
				{
					this.Height = this.ObjectModifier.GetHeight(this.DefaultWidget);
				}
			}
		}

		private void NotifyMarginsChanged(Margins margins)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition de la marge (LeftMargin, RightMargin,
			//	etc.) pour permettre de mettre � jour les widgets connect�s :
			
			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					foreach (Widget obj in this.Widgets)
					{
						this.ObjectModifier.SetMargins(obj, margins);
					}
				}
				finally
				{
					this.ResumeChanges();
				}
			}
		}

		private void NotifyBoundsChanged(Rectangle bounds)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition de la bo�te (OriginX, Width,
			//	etc.) pour permettre de mettre � jour les widgets connect�s :

			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					foreach (Widget obj in this.Widgets)
					{
						if (this.ObjectModifier.HasBounds(obj))
						{
							this.ObjectModifier.SetBounds(obj, bounds);
						}
						else
						{
							if (this.ObjectModifier.HasWidth(obj))
							{
								this.ObjectModifier.SetWidth(obj, bounds.Width);
							}

							if (this.ObjectModifier.HasHeight(obj))
							{
								this.ObjectModifier.SetHeight(obj, bounds.Height);
							}
						}
					}
				}
				finally
				{
					this.ResumeChanges();
				}
			}
		}
		

		static Geometry()
		{
			Geometry.LeftMarginProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			Geometry.RightMarginProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			Geometry.TopMarginProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			Geometry.BottomMarginProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			
			Geometry.OriginXProperty.DefaultMetadata.DefineNamedType(ProxyManager.LocationNumericType);
			Geometry.OriginYProperty.DefaultMetadata.DefineNamedType(ProxyManager.LocationNumericType);
			Geometry.WidthProperty.DefaultMetadata.DefineNamedType(ProxyManager.SizeNumericType);
			Geometry.HeightProperty.DefaultMetadata.DefineNamedType(ProxyManager.SizeNumericType);

			Geometry.LeftMarginProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.LeftMargin.Id);
			Geometry.RightMarginProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.RightMargin.Id);
			Geometry.TopMarginProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.TopMargin.Id);
			Geometry.BottomMarginProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.BottomMargin.Id);

			Geometry.OriginXProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.OriginX.Id);
			Geometry.OriginYProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.OriginY.Id);
			Geometry.WidthProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.Width.Id);
			Geometry.HeightProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.Height.Id);
		}


		private static void NotifyLeftMarginChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Geometry that = (Geometry) o;
			Margins margins = new Margins(value, that.RightMargin, that.TopMargin, that.BottomMargin);
			that.NotifyMarginsChanged(margins);
		}

		private static void NotifyRightMarginChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Geometry that = (Geometry) o;
			Margins margins = new Margins(that.LeftMargin, value, that.TopMargin, that.BottomMargin);
			that.NotifyMarginsChanged(margins);
		}

		private static void NotifyTopMarginChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Geometry that = (Geometry) o;
			Margins margins = new Margins(that.LeftMargin, that.RightMargin, value, that.BottomMargin);
			that.NotifyMarginsChanged(margins);
		}

		private static void NotifyBottomMarginChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Geometry that = (Geometry) o;
			Margins margins = new Margins(that.LeftMargin, that.RightMargin, that.TopMargin, value);
			that.NotifyMarginsChanged(margins);
		}

		private static void NotifyOriginXChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Geometry that = (Geometry) o;
			Rectangle bounds = new Rectangle(value, that.OriginY, that.Width, that.Height);
			that.NotifyBoundsChanged(bounds);
		}

		private static void NotifyOriginYChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Geometry that = (Geometry) o;
			Rectangle bounds = new Rectangle(that.OriginX, value, that.Width, that.Height);
			that.NotifyBoundsChanged(bounds);
		}

		private static void NotifyWidthChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Geometry that = (Geometry) o;
			Rectangle bounds = new Rectangle(that.OriginX, that.OriginY, value, that.Height);
			that.NotifyBoundsChanged(bounds);
		}

		private static void NotifyHeightChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Geometry that = (Geometry) o;
			Rectangle bounds = new Rectangle(that.OriginX, that.OriginY, that.Width, value);
			that.NotifyBoundsChanged(bounds);
		}


		public static readonly DependencyProperty LeftMarginProperty	= DependencyProperty.Register("LeftMargin",   typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyLeftMarginChanged));
		public static readonly DependencyProperty RightMarginProperty	= DependencyProperty.Register("RightMargin",  typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyRightMarginChanged));
		public static readonly DependencyProperty TopMarginProperty		= DependencyProperty.Register("TopMargin",    typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyTopMarginChanged));
		public static readonly DependencyProperty BottomMarginProperty	= DependencyProperty.Register("BottomMargin", typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyBottomMarginChanged));

		public static readonly DependencyProperty OriginXProperty		= DependencyProperty.Register("OriginX",      typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyOriginXChanged));
		public static readonly DependencyProperty OriginYProperty		= DependencyProperty.Register("OriginY",      typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyOriginYChanged));
		public static readonly DependencyProperty WidthProperty			= DependencyProperty.Register("Width",        typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyWidthChanged));
		public static readonly DependencyProperty HeightProperty		= DependencyProperty.Register("Height",       typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyHeightChanged));
	}
}
