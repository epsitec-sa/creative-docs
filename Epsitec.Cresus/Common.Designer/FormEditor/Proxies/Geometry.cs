using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.FormEngine;

namespace Epsitec.Common.Designer.FormEditor.Proxies
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
			//	Les Proxies de PanelEditor sont numérotés à partir de 0.
			//	Ceux de FormEditor sont numérotés à partir de 100.
			//	Ceci évite les collisions dans PanelsContext.IsExtendedProxies !
			get
			{
				return 100+2;
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
				return 22*4+1;
			}
		}

		public override double RowsSpacing
		{
			get
			{
				return 3;
			}
		}


		public int ColumnsRequired
		{
			get
			{
				return (int) this.GetValue(Geometry.ColumnsRequiredProperty);
			}
			set
			{
				this.SetValue(Geometry.ColumnsRequiredProperty, value);
			}
		}

		public int RowsRequired
		{
			get
			{
				return (int) this.GetValue(Geometry.RowsRequiredProperty);
			}
			set
			{
				this.SetValue(Geometry.RowsRequiredProperty, value);
			}
		}

		public FieldDescription.SeparatorType SeparatorBottom
		{
			get
			{
				return (FieldDescription.SeparatorType) this.GetValue(Geometry.SeparatorBottomProperty);
			}
			set
			{
				this.SetValue(Geometry.SeparatorBottomProperty, value);
			}
		}

		public FieldDescription.BoxPaddingType BoxPadding
		{
			get
			{
				return (FieldDescription.BoxPaddingType) this.GetValue(Geometry.BoxPaddingProperty);
			}
			set
			{
				this.SetValue(Geometry.BoxPaddingProperty, value);
			}
		}

		public FrameState BoxFrameState
		{
			get
			{
				return (FrameState) this.GetValue(Geometry.BoxFrameStateProperty);
			}
			set
			{
				this.SetValue(Geometry.BoxFrameStateProperty, value);
			}
		}

		public double BoxFrameWidth
		{
			get
			{
				return (double) this.GetValue(Geometry.BoxFrameWidthProperty);
			}
			set
			{
				this.SetValue(Geometry.BoxFrameWidthProperty, value);
			}
		}


		protected override void InitializePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			if (this.ObjectModifier.IsReadonly)
			{
				return;
			}

			if (this.ObjectModifier.IsField(this.DefaultWidget) ||
				this.ObjectModifier.IsBox(this.DefaultWidget))
			{
				this.SeparatorBottom = this.ObjectModifier.GetSeparatorBottom(this.DefaultWidget);
			}

			if (this.ObjectModifier.IsBox(this.DefaultWidget))
			{
				this.BoxPadding = this.ObjectModifier.GetBoxPadding(this.DefaultWidget);
			}

			if (this.ObjectModifier.IsBox(this.DefaultWidget))
			{
				this.BoxFrameState = this.ObjectModifier.GetBoxFrameState(this.DefaultWidget);
				this.BoxFrameWidth = this.ObjectModifier.GetBoxFrameWidth(this.DefaultWidget);
			}

			if (this.ObjectModifier.IsField(this.DefaultWidget) ||
				this.ObjectModifier.IsBox(this.DefaultWidget) ||
				this.ObjectModifier.IsGlue(this.DefaultWidget))
			{
				this.ColumnsRequired = this.ObjectModifier.GetColumnsRequired(this.DefaultWidget);
			}

			if (this.ObjectModifier.IsField(this.DefaultWidget) ||
				this.ObjectModifier.IsBox(this.DefaultWidget))
			{
				this.RowsRequired = this.ObjectModifier.GetRowsRequired(this.DefaultWidget);
			}
		}

		static Geometry()
		{
			EnumType separatorBottomEnumType = Res.Types.FieldDescription.SeparatorType;
			Geometry.SeparatorBottomProperty.DefaultMetadata.DefineNamedType(separatorBottomEnumType);
			Geometry.SeparatorBottomProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.SeparatorBottomType.Id);

			EnumType boxPaddingBottomEnumType = Res.Types.FieldDescription.BoxPaddingType;
			Geometry.BoxPaddingProperty.DefaultMetadata.DefineNamedType(boxPaddingBottomEnumType);
			Geometry.BoxPaddingProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.BoxPaddingType.Id);

			EnumType boxFrameStateEnumType = Res.Types.FieldDescription.FrameState;
			Geometry.BoxFrameStateProperty.DefaultMetadata.DefineNamedType(boxFrameStateEnumType);
			Geometry.BoxFrameStateProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.BoxFrameState.Id);

			Geometry.BoxFrameWidthProperty.DefaultMetadata.DefineNamedType(ProxyManager.WidthNumericType);
			Geometry.BoxFrameWidthProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.BoxFrameWidth.Id);

			Geometry.ColumnsRequiredProperty.DefaultMetadata.DefineNamedType(ProxyManager.ColumnsRequiredNumericType);
			Geometry.ColumnsRequiredProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldGeometry.ColumnsRequired.Id);

			Geometry.RowsRequiredProperty.DefaultMetadata.DefineNamedType(ProxyManager.RowsRequiredNumericType);
			Geometry.RowsRequiredProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldGeometry.RowsRequired.Id);
		}


		private static void NotifySeparatorBottomChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.SeparatorType value = (FieldDescription.SeparatorType) newValue;
			Geometry that = (Geometry) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetSeparatorBottom(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyBoxPaddingChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.BoxPaddingType value = (FieldDescription.BoxPaddingType) newValue;
			Geometry that = (Geometry) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetBoxPadding(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyBoxFrameStateChanged(DependencyObject o, object oldValue, object newValue)
		{
			FrameState value = (FrameState) newValue;
			Geometry that = (Geometry) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetBoxFrameState(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyBoxFrameWidthChanged(DependencyObject o, object oldValue, object newValue)
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
						that.ObjectModifier.SetBoxFrameWidth(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyColumnsRequiredChanged(DependencyObject o, object oldValue, object newValue)
		{
			int value = (int) newValue;
			Geometry that = (Geometry) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetColumnsRequired(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyRowsRequiredChanged(DependencyObject o, object oldValue, object newValue)
		{
			int value = (int) newValue;
			Geometry that = (Geometry) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetRowsRequired(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}


		public static readonly DependencyProperty SeparatorBottomProperty = DependencyProperty.Register("SeparatorBottom", typeof(FieldDescription.SeparatorType),  typeof(Geometry), new DependencyPropertyMetadata(FieldDescription.SeparatorType.Normal,  Geometry.NotifySeparatorBottomChanged));
		public static readonly DependencyProperty BoxPaddingProperty      = DependencyProperty.Register("BoxPadding",      typeof(FieldDescription.BoxPaddingType), typeof(Geometry), new DependencyPropertyMetadata(FieldDescription.BoxPaddingType.Normal, Geometry.NotifyBoxPaddingChanged));
		public static readonly DependencyProperty BoxFrameStateProperty   = DependencyProperty.Register("BoxFrameState",   typeof(FrameState),                      typeof(Geometry), new DependencyPropertyMetadata(FrameState.None,                        Geometry.NotifyBoxFrameStateChanged));
		public static readonly DependencyProperty BoxFrameWidthProperty   = DependencyProperty.Register("BoxFrameWidth",   typeof(double),                          typeof(Geometry), new DependencyPropertyMetadata(1.0,                                    Geometry.NotifyBoxFrameWidthChanged));
		public static readonly DependencyProperty ColumnsRequiredProperty = DependencyProperty.Register("ColumnsRequired", typeof(int),                             typeof(Geometry), new DependencyPropertyMetadata(0,                                      Geometry.NotifyColumnsRequiredChanged));
		public static readonly DependencyProperty RowsRequiredProperty    = DependencyProperty.Register("RowsRequired",    typeof(int),                             typeof(Geometry), new DependencyPropertyMetadata(0,                                      Geometry.NotifyRowsRequiredChanged));
	}
}
