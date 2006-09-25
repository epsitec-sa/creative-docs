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
			//	Plus le numéro est petit, plus le proxy apparaîtra haut dans la
			//	liste.
			get
			{
				return 1;
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

		public double MinWidth
		{
			get
			{
				return (double) this.GetValue(Geometry.MinWidthProperty);
			}
			set
			{
				this.SetValue(Geometry.MinWidthProperty, value);
			}
		}

		public double MaxWidth
		{
			get
			{
				return (double) this.GetValue(Geometry.MaxWidthProperty);
			}
			set
			{
				this.SetValue(Geometry.MaxWidthProperty, value);
			}
		}

		public double MinHeight
		{
			get
			{
				return (double) this.GetValue(Geometry.MinHeightProperty);
			}
			set
			{
				this.SetValue(Geometry.MinHeightProperty, value);
			}
		}

		public double MaxHeight
		{
			get
			{
				return (double) this.GetValue(Geometry.MaxHeightProperty);
			}
			set
			{
				this.SetValue(Geometry.MaxHeightProperty, value);
			}
		}


		protected override void InitializePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			if (this.ObjectModifier.HasMargins(this.DefaultWidget))
			{
				Margins margins = this.ObjectModifier.GetMargins(this.DefaultWidget);

				this.LeftMargin   = margins.Left;
				this.RightMargin  = margins.Right;
				this.TopMargin    = margins.Top;
				this.BottomMargin = margins.Bottom;
			}

			if (this.ObjectModifier.HasPreferredBounds(this.DefaultWidget))
			{
				Rectangle bounds = this.ObjectModifier.GetPreferredBounds(this.DefaultWidget);

				if (this.ObjectModifier.HasPreferredBounds(this.DefaultWidget, ObjectModifier.BoundsMode.OriginX))
				{
					this.OriginX = bounds.Left;
				}

				if (this.ObjectModifier.HasPreferredBounds(this.DefaultWidget, ObjectModifier.BoundsMode.OriginY))
				{
					this.OriginY = bounds.Bottom;
				}

				if (this.ObjectModifier.HasPreferredBounds(this.DefaultWidget, ObjectModifier.BoundsMode.Width))
				{
					this.Width = bounds.Width;
				}

				if (this.ObjectModifier.HasPreferredBounds(this.DefaultWidget, ObjectModifier.BoundsMode.Height))
				{
					this.Height = bounds.Height;
				}
			}
			else
			{
				if (this.ObjectModifier.HasPreferredWidth(this.DefaultWidget))
				{
					this.Width = this.ObjectModifier.GetPreferredWidth(this.DefaultWidget);
				}

				if (this.ObjectModifier.HasPreferredHeight(this.DefaultWidget))
				{
					this.Height = this.ObjectModifier.GetPreferredHeight(this.DefaultWidget);
				}
			}

			if (true)
			{
				this.MinWidth = this.ObjectModifier.GetMinWidth(this.DefaultWidget);
				this.MaxWidth = this.ObjectModifier.GetMaxWidth(this.DefaultWidget);
				this.MinHeight = this.ObjectModifier.GetMinHeight(this.DefaultWidget);
				this.MaxHeight = this.ObjectModifier.GetMaxHeight(this.DefaultWidget);
			}
		}

		private void NotifyMarginsChanged(Margins margins)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition de la marge (LeftMargin, RightMargin,
			//	etc.) pour permettre de mettre à jour les widgets connectés :
			
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
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition de la boîte (OriginX, Width,
			//	etc.) pour permettre de mettre à jour les widgets connectés :

			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					foreach (Widget obj in this.Widgets)
					{
						if (this.ObjectModifier.HasPreferredBounds(obj))
						{
							this.ObjectModifier.SetPreferredBounds(obj, bounds);
						}
						else
						{
							if (this.ObjectModifier.HasPreferredWidth(obj))
							{
								this.ObjectModifier.SetPreferredWidth(obj, bounds.Width);
							}

							if (this.ObjectModifier.HasPreferredHeight(obj))
							{
								this.ObjectModifier.SetPreferredHeight(obj, bounds.Height);
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

			Geometry.MinWidthProperty.DefaultMetadata.DefineNamedType(ProxyManager.SizeNumericType);
			Geometry.MaxWidthProperty.DefaultMetadata.DefineNamedType(ProxyManager.SizeNumericType);
			Geometry.MinHeightProperty.DefaultMetadata.DefineNamedType(ProxyManager.SizeNumericType);
			Geometry.MaxHeightProperty.DefaultMetadata.DefineNamedType(ProxyManager.SizeNumericType);

			Geometry.LeftMarginProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.LeftMargin.Druid);
			Geometry.RightMarginProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.RightMargin.Druid);
			Geometry.TopMarginProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.TopMargin.Druid);
			Geometry.BottomMarginProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.BottomMargin.Druid);

			Geometry.OriginXProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.OriginX.Druid);
			Geometry.OriginYProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.OriginY.Druid);
			Geometry.WidthProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.Width.Druid);
			Geometry.HeightProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.Height.Druid);

			Geometry.MinWidthProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.MinWidth.Druid);
			Geometry.MaxWidthProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.MaxWidth.Druid);
			Geometry.MinHeightProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.MinHeight.Druid);
			Geometry.MaxHeightProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Geometry.MaxHeight.Druid);
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

		private static void NotifyMinWidthChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Geometry that = (Geometry) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetMinWidth(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyMaxWidthChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Geometry that = (Geometry) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetMaxWidth(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyMinHeightChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Geometry that = (Geometry) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetMinHeight(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyMaxHeightChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Geometry that = (Geometry) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetMaxHeight(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}


		public static readonly DependencyProperty LeftMarginProperty	= DependencyProperty.Register("LeftMargin",   typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyLeftMarginChanged));
		public static readonly DependencyProperty RightMarginProperty	= DependencyProperty.Register("RightMargin",  typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyRightMarginChanged));
		public static readonly DependencyProperty TopMarginProperty		= DependencyProperty.Register("TopMargin",    typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyTopMarginChanged));
		public static readonly DependencyProperty BottomMarginProperty	= DependencyProperty.Register("BottomMargin", typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyBottomMarginChanged));

		public static readonly DependencyProperty OriginXProperty		= DependencyProperty.Register("OriginX",      typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyOriginXChanged));
		public static readonly DependencyProperty OriginYProperty		= DependencyProperty.Register("OriginY",      typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyOriginYChanged));
		public static readonly DependencyProperty WidthProperty			= DependencyProperty.Register("Width",        typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyWidthChanged));
		public static readonly DependencyProperty HeightProperty		= DependencyProperty.Register("Height",       typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyHeightChanged));

		public static readonly DependencyProperty MinWidthProperty		= DependencyProperty.Register("MinWidth",     typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyMinWidthChanged));
		public static readonly DependencyProperty MaxWidthProperty		= DependencyProperty.Register("MaxWidth",     typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyMaxWidthChanged));
		public static readonly DependencyProperty MinHeightProperty		= DependencyProperty.Register("MinHeight",    typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyMinHeightChanged));
		public static readonly DependencyProperty MaxHeightProperty		= DependencyProperty.Register("MaxHeight",    typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyMaxHeightChanged));
	}
}
