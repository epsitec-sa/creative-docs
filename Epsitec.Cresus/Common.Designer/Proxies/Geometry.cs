using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Proxies
{
	public class Geometry : Abstract
	{
		public Geometry(Widget widget, Viewers.Panels panel) : base(widget, panel)
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


		protected override void InitialisePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			if (this.objectModifier.HasMargins(this.widgets[0]))
			{
				Margins margins = this.objectModifier.GetMargins(this.widgets[0]);

				this.LeftMargin   = margins.Left;
				this.RightMargin  = margins.Right;
				this.TopMargin    = margins.Top;
				this.BottomMargin = margins.Bottom;
			}

			if (this.objectModifier.HasBounds(this.widgets[0]))
			{
				Rectangle bounds = this.objectModifier.GetBounds(this.widgets[0]);

				this.OriginX = bounds.Left;
				this.OriginY = bounds.Bottom;
				this.Width   = bounds.Width;
				this.Height  = bounds.Height;
			}
			else
			{
				if (this.objectModifier.HasWidth(this.widgets[0]))
				{
					this.Width = this.objectModifier.GetWidth(this.widgets[0]);
				}
				
				if (this.objectModifier.HasHeight(this.widgets[0]))
				{
					this.Height = this.objectModifier.GetHeight(this.widgets[0]);
				}
			}
		}

		private static void NotifyMarginsChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition de la marge (LeftMargin, RightMargin,
			//	etc.) pour permettre de mettre à jour les widgets connectés :
			Geometry that = (Geometry) o;
			Margins margins = new Margins(that.LeftMargin, that.RightMargin, that.TopMargin, that.BottomMargin);

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						that.objectModifier.SetMargins(obj, margins);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		private static void NotifyBoundsChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition de la boîte (OriginX, Width,
			//	etc.) pour permettre de mettre à jour les widgets connectés :
			Geometry that = (Geometry) o;
			Rectangle bounds = new Rectangle(that.OriginX, that.OriginY, that.Width, that.Height);

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						if (that.objectModifier.HasBounds(obj))
						{
							that.objectModifier.SetBounds(obj, bounds);
						}
						else
						{
							if (that.objectModifier.HasWidth(obj))
							{
								that.objectModifier.SetWidth(obj, bounds.Width);
							}

							if (that.objectModifier.HasHeight(obj))
							{
								that.objectModifier.SetHeight(obj, bounds.Height);
							}
						}
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}
		

		static Geometry()
		{
			Geometry.LeftMarginProperty.DefaultMetadata.DefineNamedType (ProxyManager.MarginNumericType);
			Geometry.RightMarginProperty.DefaultMetadata.DefineNamedType (ProxyManager.MarginNumericType);
			Geometry.TopMarginProperty.DefaultMetadata.DefineNamedType (ProxyManager.MarginNumericType);
			Geometry.BottomMarginProperty.DefaultMetadata.DefineNamedType (ProxyManager.MarginNumericType);
			
			Geometry.OriginXProperty.DefaultMetadata.DefineNamedType (ProxyManager.LocationNumericType);
			Geometry.OriginYProperty.DefaultMetadata.DefineNamedType (ProxyManager.LocationNumericType);
			Geometry.WidthProperty.DefaultMetadata.DefineNamedType (ProxyManager.SizeNumericType);
			Geometry.HeightProperty.DefaultMetadata.DefineNamedType (ProxyManager.SizeNumericType);

			Geometry.LeftMarginProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[1000]").ToLong());
			Geometry.RightMarginProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[1001]").ToLong());
			Geometry.TopMarginProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[1002]").ToLong());
			Geometry.BottomMarginProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[1003]").ToLong());

			Geometry.OriginXProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[1004]").ToLong());
			Geometry.OriginYProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[1005]").ToLong());
			Geometry.WidthProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[1006]").ToLong());
			Geometry.HeightProperty.DefaultMetadata.DefineCaptionId(new Support.Druid("[1007]").ToLong());
		}


		public static readonly DependencyProperty LeftMarginProperty	= DependencyProperty.Register("LeftMargin",   typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyMarginsChanged));
		public static readonly DependencyProperty RightMarginProperty	= DependencyProperty.Register("RightMargin",  typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyMarginsChanged));
		public static readonly DependencyProperty TopMarginProperty		= DependencyProperty.Register("TopMargin",    typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyMarginsChanged));
		public static readonly DependencyProperty BottomMarginProperty	= DependencyProperty.Register("BottomMargin", typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyMarginsChanged));

		public static readonly DependencyProperty OriginXProperty		= DependencyProperty.Register("OriginX",      typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyBoundsChanged));
		public static readonly DependencyProperty OriginYProperty		= DependencyProperty.Register("OriginY",      typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyBoundsChanged));
		public static readonly DependencyProperty WidthProperty			= DependencyProperty.Register("Width",        typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyBoundsChanged));
		public static readonly DependencyProperty HeightProperty		= DependencyProperty.Register("Height",       typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyBoundsChanged));
	}
}
