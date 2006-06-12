using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	public class Geometry : Abstract
	{
		public Geometry(Widget widget) : base (widget)
		{
		}

		public double LeftMargin
		{
			get
			{
				return (double) this.GetValue (Geometry.LeftMarginProperty);
			}
			set
			{
				this.SetValue (Geometry.LeftMarginProperty, value);
			}
		}

		public double RightMargin
		{
			get
			{
				return (double) this.GetValue (Geometry.RightMarginProperty);
			}
			set
			{
				this.SetValue (Geometry.RightMarginProperty, value);
			}
		}

		public double TopMargin
		{
			get
			{
				return (double) this.GetValue (Geometry.TopMarginProperty);
			}
			set
			{
				this.SetValue (Geometry.TopMarginProperty, value);
			}
		}

		public double BottomMargin
		{
			get
			{
				return (double) this.GetValue (Geometry.BottomMarginProperty);
			}
			set
			{
				this.SetValue (Geometry.BottomMarginProperty, value);
			}
		}

		protected override void InitialisePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			
			Drawing.Margins margins = (Drawing.Margins) this.GetWidgetProperty (Visual.MarginsProperty);

			this.LeftMargin   = margins.Left;
			this.RightMargin  = margins.Right;
			this.TopMargin    = margins.Top;
			this.BottomMargin = margins.Bottom;
		}

		private static void NotifyMarginChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition de la marge (LeftMargin, RightMargin,
			//	etc.) pour permettre de mettre à jour les widgets connectés :
			
			Geometry that = (Geometry) o;
			Drawing.Margins margins = new Drawing.Margins (that.LeftMargin, that.RightMargin, that.TopMargin, that.BottomMargin);
			
			//	Demande à Proxies.Abstract de mettre à jour la propriété qui
			//	définit les marges du ou des widget(s) sélectionné(s) :
			
			that.SetWidgetProperty (Visual.MarginsProperty, margins);
		}

		public static readonly DependencyProperty LeftMarginProperty	= DependencyProperty.Register ("LeftMargin", typeof (double), typeof (Geometry), new DependencyPropertyMetadata (0.0, Geometry.NotifyMarginChanged));
		public static readonly DependencyProperty RightMarginProperty	= DependencyProperty.Register ("RightMargin", typeof (double), typeof (Geometry), new DependencyPropertyMetadata (0.0, Geometry.NotifyMarginChanged));
		public static readonly DependencyProperty TopMarginProperty		= DependencyProperty.Register ("TopMargin", typeof (double), typeof (Geometry), new DependencyPropertyMetadata (0.0, Geometry.NotifyMarginChanged));
		public static readonly DependencyProperty BottomMarginProperty	= DependencyProperty.Register ("BottomMargin", typeof (double), typeof (Geometry), new DependencyPropertyMetadata (0.0, Geometry.NotifyMarginChanged));
	}
}
