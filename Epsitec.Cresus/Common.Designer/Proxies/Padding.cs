using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Proxies
{
	public class Padding : Abstract
	{
		public Padding(Widget widget, ObjectModifier objectModifier) : base(widget, objectModifier)
		{
		}

		public override int Rank
		{
			//	Retourne le rang de ce proxy parmi la liste de tous les proxies.
			//	Plus le num�ro est petit, plus le proxy appara�tra haut dans la
			//	liste.
			get
			{
				return 2;
			}
		}

		public override string IconName
		{
			get
			{
				return "PropertyPadding";
			}
		}

		public override double DataColumnWidth
		{
			get
			{
				return 50;
			}
		}

		public double LeftPadding
		{
			get
			{
				return (double) this.GetValue(Padding.LeftPaddingProperty);
			}
			set
			{
				this.SetValue(Padding.LeftPaddingProperty, value);
			}
		}

		public double RightPadding
		{
			get
			{
				return (double) this.GetValue(Padding.RightPaddingProperty);
			}
			set
			{
				this.SetValue(Padding.RightPaddingProperty, value);
			}
		}

		public double TopPadding
		{
			get
			{
				return (double) this.GetValue(Padding.TopPaddingProperty);
			}
			set
			{
				this.SetValue(Padding.TopPaddingProperty, value);
			}
		}

		public double BottomPadding
		{
			get
			{
				return (double) this.GetValue(Padding.BottomPaddingProperty);
			}
			set
			{
				this.SetValue(Padding.BottomPaddingProperty, value);
			}
		}


		protected override void InitialisePropertyValues()
		{
			//	Cette m�thode est appel�e par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propri�t�s du widget s�lectionn�
			//	pour pouvoir ensuite travailler dessus :
			if (this.objectModifier.HasPadding(this.widgets[0]))
			{
				Margins padding = this.objectModifier.GetPadding(this.widgets[0]);

				this.LeftPadding   = padding.Left;
				this.RightPadding  = padding.Right;
				this.TopPadding    = padding.Top;
				this.BottomPadding = padding.Bottom;
			}
		}

		private static void NotifyPaddingChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition de la marge (LeftPadding, RightPadding,
			//	etc.) pour permettre de mettre � jour les widgets connect�s :
			Padding that = (Padding) o;
			Margins padding = new Margins(that.LeftPadding, that.RightPadding, that.TopPadding, that.BottomPadding);

			if (that.suspendChanges == 0)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.widgets)
					{
						that.objectModifier.SetPadding(obj, padding);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}


		public static readonly DependencyProperty LeftPaddingProperty	= DependencyProperty.Register("LeftPadding",   typeof(double), typeof(Padding), new DependencyPropertyMetadata(0.0, Padding.NotifyPaddingChanged));
		public static readonly DependencyProperty RightPaddingProperty	= DependencyProperty.Register("RightPadding",  typeof(double), typeof(Padding), new DependencyPropertyMetadata(0.0, Padding.NotifyPaddingChanged));
		public static readonly DependencyProperty TopPaddingProperty    = DependencyProperty.Register("TopPadding",    typeof(double), typeof(Padding), new DependencyPropertyMetadata(0.0, Padding.NotifyPaddingChanged));
		public static readonly DependencyProperty BottomPaddingProperty = DependencyProperty.Register("BottomPadding", typeof(double), typeof(Padding), new DependencyPropertyMetadata(0.0, Padding.NotifyPaddingChanged));
	}
}
