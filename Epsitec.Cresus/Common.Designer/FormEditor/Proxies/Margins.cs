using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.FormEngine;

namespace Epsitec.Common.Designer.FormEditor.Proxies
{
	public class Margins : Abstract
	{
		public Margins(ProxyManager manager) : base (manager)
		{
		}

		public override int Rank
		{
			//	Retourne le rang de ce proxy parmi la liste de tous les proxies.
			//	Plus le numéro est petit, plus le proxy apparaîtra haut dans la
			//	liste.
			//	Les Proxies de PanelEditor sont numérotés à partir de 0.
			//	Ceux de FormEditor sont numérotés à partir de 100.
			//	Ceci évite les collisions dans PanelsContext.IsExtendedProxies !
			get
			{
				return 100+3;
			}
		}

		public override string IconName
		{
			get
			{
				return "PropertyContent";
			}
		}


		public double ContainerMarginLeft
		{
			get
			{
				return (double) this.GetValue(Margins.ContainerMarginLeftProperty);
			}
			set
			{
				this.SetValue(Margins.ContainerMarginLeftProperty, value);
			}
		}

		public double ContainerMarginRight
		{
			get
			{
				return (double) this.GetValue(Margins.ContainerMarginRightProperty);
			}
			set
			{
				this.SetValue(Margins.ContainerMarginRightProperty, value);
			}
		}

		public double ContainerMarginTop
		{
			get
			{
				return (double) this.GetValue(Margins.ContainerMarginTopProperty);
			}
			set
			{
				this.SetValue(Margins.ContainerMarginTopProperty, value);
			}
		}

		public double ContainerMarginBottom
		{
			get
			{
				return (double) this.GetValue(Margins.ContainerMarginBottomProperty);
			}
			set
			{
				this.SetValue(Margins.ContainerMarginBottomProperty, value);
			}
		}


		public double ContainerPaddingLeft
		{
			get
			{
				return (double) this.GetValue(Margins.ContainerPaddingLeftProperty);
			}
			set
			{
				this.SetValue(Margins.ContainerPaddingLeftProperty, value);
			}
		}

		public double ContainerPaddingRight
		{
			get
			{
				return (double) this.GetValue(Margins.ContainerPaddingRightProperty);
			}
			set
			{
				this.SetValue(Margins.ContainerPaddingRightProperty, value);
			}
		}

		public double ContainerPaddingTop
		{
			get
			{
				return (double) this.GetValue(Margins.ContainerPaddingTopProperty);
			}
			set
			{
				this.SetValue(Margins.ContainerPaddingTopProperty, value);
			}
		}

		public double ContainerPaddingBottom
		{
			get
			{
				return (double) this.GetValue(Margins.ContainerPaddingBottomProperty);
			}
			set
			{
				this.SetValue(Margins.ContainerPaddingBottomProperty, value);
			}
		}


		protected override void InitializePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			if (this.ObjectModifier.IsBox(this.DefaultWidget))
			{
				Drawing.Margins margins = this.ObjectModifier.GetContainerMargins(this.DefaultWidget);
				this.ContainerMarginLeft   = margins.Left;
				this.ContainerMarginRight  = margins.Right;
				this.ContainerMarginTop    = margins.Top;
				this.ContainerMarginBottom = margins.Bottom;

				Drawing.Margins padding = this.ObjectModifier.GetContainerPadding(this.DefaultWidget);
				this.ContainerPaddingLeft   = padding.Left;
				this.ContainerPaddingRight  = padding.Right;
				this.ContainerPaddingTop    = padding.Top;
				this.ContainerPaddingBottom = padding.Bottom;
			}
		}

		static Margins()
		{
			Margins.ContainerMarginLeftProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			Margins.ContainerMarginRightProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			Margins.ContainerMarginTopProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			Margins.ContainerMarginBottomProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);

			Margins.ContainerMarginLeftProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMargins.MarginLeft.Id);
			Margins.ContainerMarginRightProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMargins.MarginRight.Id);
			Margins.ContainerMarginTopProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMargins.MarginTop.Id);
			Margins.ContainerMarginBottomProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMargins.MarginBottom.Id);

			Margins.ContainerPaddingLeftProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			Margins.ContainerPaddingRightProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			Margins.ContainerPaddingTopProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);
			Margins.ContainerPaddingBottomProperty.DefaultMetadata.DefineNamedType(ProxyManager.MarginNumericType);

			Margins.ContainerPaddingLeftProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMargins.PaddingLeft.Id);
			Margins.ContainerPaddingRightProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMargins.PaddingRight.Id);
			Margins.ContainerPaddingTopProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMargins.PaddingTop.Id);
			Margins.ContainerPaddingBottomProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMargins.PaddingBottom.Id);
		}


		private void NotifyContainerMarginsChanged(Drawing.Margins margins)
		{
			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					foreach (Widget obj in this.Widgets)
					{
						this.ObjectModifier.SetContainerMargins(obj, margins);
					}
				}
				finally
				{
					this.ResumeChanges();
					this.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyContainerMarginLeftChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Margins that = (Margins) o;
			Drawing.Margins margins = new Drawing.Margins(value, that.ContainerMarginRight, that.ContainerMarginTop, that.ContainerMarginBottom);
			that.NotifyContainerMarginsChanged(margins);
		}

		private static void NotifyContainerMarginRightChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Margins that = (Margins) o;
			Drawing.Margins margins = new Drawing.Margins(that.ContainerMarginLeft, value, that.ContainerMarginTop, that.ContainerMarginBottom);
			that.NotifyContainerMarginsChanged(margins);
		}

		private static void NotifyContainerMarginTopChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Margins that = (Margins) o;
			Drawing.Margins margins = new Drawing.Margins(that.ContainerMarginLeft, that.ContainerMarginRight, value, that.ContainerMarginBottom);
			that.NotifyContainerMarginsChanged(margins);
		}

		private static void NotifyContainerMarginBottomChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Margins that = (Margins) o;
			Drawing.Margins margins = new Drawing.Margins(that.ContainerMarginLeft, that.ContainerMarginRight, that.ContainerMarginTop, value);
			that.NotifyContainerMarginsChanged(margins);
		}

		
		private void NotifyContainerPaddingChanged(Drawing.Margins padding)
		{
			if (this.IsNotSuspended)
			{
				this.SuspendChanges();

				try
				{
					foreach (Widget obj in this.Widgets)
					{
						this.ObjectModifier.SetContainerPadding(obj, padding);
					}
				}
				finally
				{
					this.ResumeChanges();
					this.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyContainerPaddingLeftChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Margins that = (Margins) o;
			Drawing.Margins padding = new Drawing.Margins(value, that.ContainerPaddingRight, that.ContainerPaddingTop, that.ContainerPaddingBottom);
			that.NotifyContainerPaddingChanged(padding);
		}

		private static void NotifyContainerPaddingRightChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Margins that = (Margins) o;
			Drawing.Margins padding = new Drawing.Margins(that.ContainerPaddingLeft, value, that.ContainerPaddingTop, that.ContainerPaddingBottom);
			that.NotifyContainerPaddingChanged(padding);
		}

		private static void NotifyContainerPaddingTopChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Margins that = (Margins) o;
			Drawing.Margins padding = new Drawing.Margins(that.ContainerPaddingLeft, that.ContainerPaddingRight, value, that.ContainerPaddingBottom);
			that.NotifyContainerPaddingChanged(padding);
		}

		private static void NotifyContainerPaddingBottomChanged(DependencyObject o, object oldValue, object newValue)
		{
			double value = (double) newValue;
			Margins that = (Margins) o;
			Drawing.Margins padding = new Drawing.Margins(that.ContainerPaddingLeft, that.ContainerPaddingRight, that.ContainerPaddingTop, value);
			that.NotifyContainerPaddingChanged(padding);
		}



		public static readonly DependencyProperty ContainerMarginLeftProperty   = DependencyProperty.Register("ContainerMarginLeft",   typeof(double), typeof(Margins), new DependencyPropertyMetadata(0.0, Margins.NotifyContainerMarginLeftChanged));
		public static readonly DependencyProperty ContainerMarginRightProperty  = DependencyProperty.Register("ContainerMarginRight",  typeof(double), typeof(Margins), new DependencyPropertyMetadata(0.0, Margins.NotifyContainerMarginRightChanged));
		public static readonly DependencyProperty ContainerMarginTopProperty    = DependencyProperty.Register("ContainerMarginTop",    typeof(double), typeof(Margins), new DependencyPropertyMetadata(0.0, Margins.NotifyContainerMarginTopChanged));
		public static readonly DependencyProperty ContainerMarginBottomProperty = DependencyProperty.Register("ContainerMarginBottom", typeof(double), typeof(Margins), new DependencyPropertyMetadata(0.0, Margins.NotifyContainerMarginBottomChanged));

		public static readonly DependencyProperty ContainerPaddingLeftProperty   = DependencyProperty.Register("ContainerPaddingLeft",   typeof(double), typeof(Margins), new DependencyPropertyMetadata(0.0, Margins.NotifyContainerPaddingLeftChanged));
		public static readonly DependencyProperty ContainerPaddingRightProperty  = DependencyProperty.Register("ContainerPaddingRight",  typeof(double), typeof(Margins), new DependencyPropertyMetadata(0.0, Margins.NotifyContainerPaddingRightChanged));
		public static readonly DependencyProperty ContainerPaddingTopProperty    = DependencyProperty.Register("ContainerPaddingTop",    typeof(double), typeof(Margins), new DependencyPropertyMetadata(0.0, Margins.NotifyContainerPaddingTopChanged));
		public static readonly DependencyProperty ContainerPaddingBottomProperty = DependencyProperty.Register("ContainerPaddingBottom", typeof(double), typeof(Margins), new DependencyPropertyMetadata(0.0, Margins.NotifyContainerPaddingBottomChanged));
	}
}
