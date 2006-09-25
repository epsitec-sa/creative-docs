using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Proxies
{
	public class Padding : Abstract
	{
		public Padding(ProxyManager manager) : base (manager)
		{
		}

		public override int Rank
		{
			//	Retourne le rang de ce proxy parmi la liste de tous les proxies.
			//	Plus le numéro est petit, plus le proxy apparaîtra haut dans la
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


		protected override void InitializePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			if (this.ObjectModifier.HasPadding(this.DefaultWidget))
			{
				Margins padding = this.ObjectModifier.GetPadding(this.DefaultWidget);

				this.LeftPadding   = padding.Left;
				this.RightPadding  = padding.Right;
				this.TopPadding    = padding.Top;
				this.BottomPadding = padding.Bottom;
			}
		}

		private void NotifyPaddingChanged(Margins padding)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition de la marge (LeftPadding, RightPadding,
			//	etc.) pour permettre de mettre à jour les widgets connectés :

			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					foreach (Widget obj in this.Widgets)
					{
						this.ObjectModifier.SetPadding(obj, padding);
					}
				}
				finally
				{
					this.ResumeChanges();
				}
			}
		}

		private static void NotifyLeftPaddingChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Padding that = (Padding) o;
			Margins padding = new Margins(value, that.RightPadding, that.TopPadding, that.BottomPadding);
			that.NotifyPaddingChanged(padding);
		}

		private static void NotifyRightPaddingChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Padding that = (Padding) o;
			Margins padding = new Margins(that.LeftPadding, value, that.TopPadding, that.BottomPadding);
			that.NotifyPaddingChanged(padding);
		}

		private static void NotifyTopPaddingChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Padding that = (Padding) o;
			Margins padding = new Margins(that.LeftPadding, that.RightPadding, value, that.BottomPadding);
			that.NotifyPaddingChanged(padding);
		}

		private static void NotifyBottomPaddingChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Padding that = (Padding) o;
			Margins padding = new Margins(that.LeftPadding, that.RightPadding, that.TopPadding, value);
			that.NotifyPaddingChanged(padding);
		}


		static Padding()
		{
			Padding.LeftPaddingProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			Padding.RightPaddingProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			Padding.TopPaddingProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			Padding.BottomPaddingProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);

			Padding.LeftPaddingProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Padding.Left.Druid);
			Padding.RightPaddingProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Padding.Right.Druid);
			Padding.TopPaddingProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Padding.Top.Druid);
			Padding.BottomPaddingProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Padding.Bottom.Druid);
		}


		public static readonly DependencyProperty LeftPaddingProperty	= DependencyProperty.Register("LeftPadding",   typeof(double), typeof(Padding), new DependencyPropertyMetadata(0.0, Padding.NotifyLeftPaddingChanged));
		public static readonly DependencyProperty RightPaddingProperty	= DependencyProperty.Register("RightPadding",  typeof(double), typeof(Padding), new DependencyPropertyMetadata(0.0, Padding.NotifyRightPaddingChanged));
		public static readonly DependencyProperty TopPaddingProperty    = DependencyProperty.Register("TopPadding",    typeof(double), typeof(Padding), new DependencyPropertyMetadata(0.0, Padding.NotifyTopPaddingChanged));
		public static readonly DependencyProperty BottomPaddingProperty = DependencyProperty.Register("BottomPadding", typeof(double), typeof(Padding), new DependencyPropertyMetadata(0.0, Padding.NotifyBottomPaddingChanged));
	}
}
