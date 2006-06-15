using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Proxies
{
	public class Geometry : Abstract
	{
		public Geometry(Widget widget) : base(widget)
		{
		}

		public override int Rank
		{
			//	Retourne le rang de ce proxy parmi la liste de tous les proxies.
			//	Plus le num�ro est petit, plus le proxy appara�tra haut dans la
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

		public override double DataColumnWidth
		{
			get
			{
				return 50;
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


		protected override void InitialisePropertyValues()
		{
			//	Cette m�thode est appel�e par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propri�t�s du widget s�lectionn�
			//	pour pouvoir ensuite travailler dessus :
			Margins margins = (Margins) this.GetWidgetProperty(Visual.MarginsProperty);

			this.LeftMargin   = margins.Left;
			this.RightMargin  = margins.Right;
			this.TopMargin    = margins.Top;
			this.BottomMargin = margins.Bottom;
		}

		private static void NotifyChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition de la marge (LeftMargin, RightMargin,
			//	etc.) pour permettre de mettre � jour les widgets connect�s :
			Geometry that = (Geometry) o;
			Margins margins = new Margins(that.LeftMargin, that.RightMargin, that.TopMargin, that.BottomMargin);
			
			//	Demande � Proxies.Abstract de mettre � jour la propri�t� qui
			//	d�finit les marges du ou des widget(s) s�lectionn�(s) :
			that.SetWidgetProperty(Visual.MarginsProperty, margins);
		}


		public static readonly DependencyProperty LeftMarginProperty	= DependencyProperty.Register("LeftMargin",   typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyChanged));
		public static readonly DependencyProperty RightMarginProperty	= DependencyProperty.Register("RightMargin",  typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyChanged));
		public static readonly DependencyProperty TopMarginProperty		= DependencyProperty.Register("TopMargin",    typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyChanged));
		public static readonly DependencyProperty BottomMarginProperty	= DependencyProperty.Register("BottomMargin", typeof(double), typeof(Geometry), new DependencyPropertyMetadata(0.0, Geometry.NotifyChanged));
	}
}
