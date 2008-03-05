using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.FormEngine;

namespace Epsitec.Common.Designer.FormEditor.Proxies
{
	public class Mode : Abstract
	{
		public Mode(ProxyManager manager) : base (manager)
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
				return 100+1;
			}
		}

		public override string IconName
		{
			get
			{
				return "PropertyAspect";
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


		public FieldDescription.SeparatorType SeparatorBottom
		{
			get
			{
				return (FieldDescription.SeparatorType) this.GetValue(Mode.SeparatorBottomProperty);
			}
			set
			{
				this.SetValue(Mode.SeparatorBottomProperty, value);
			}
		}

		public FieldDescription.BoxPaddingType BoxPadding
		{
			get
			{
				return (FieldDescription.BoxPaddingType) this.GetValue(Mode.BoxPaddingProperty);
			}
			set
			{
				this.SetValue(Mode.BoxPaddingProperty, value);
			}
		}

		public FieldDescription.BackColorType BackColor
		{
			get
			{
				return (FieldDescription.BackColorType) this.GetValue(Mode.BackColorProperty);
			}
			set
			{
				this.SetValue(Mode.BackColorProperty, value);
			}
		}

		public FieldDescription.FontColorType LabelFontColor
		{
			get
			{
				return (FieldDescription.FontColorType) this.GetValue(Mode.LabelFontColorProperty);
			}
			set
			{
				this.SetValue(Mode.LabelFontColorProperty, value);
			}
		}

		public FieldDescription.FontColorType FieldFontColor
		{
			get
			{
				return (FieldDescription.FontColorType) this.GetValue(Mode.FieldFontColorProperty);
			}
			set
			{
				this.SetValue(Mode.FieldFontColorProperty, value);
			}
		}

		public FieldDescription.FontFaceType LabelFontFace
		{
			get
			{
				return (FieldDescription.FontFaceType) this.GetValue(Mode.LabelFontFaceProperty);
			}
			set
			{
				this.SetValue(Mode.LabelFontFaceProperty, value);
			}
		}

		public FieldDescription.FontFaceType FieldFontFace
		{
			get
			{
				return (FieldDescription.FontFaceType) this.GetValue(Mode.FieldFontFaceProperty);
			}
			set
			{
				this.SetValue(Mode.FieldFontFaceProperty, value);
			}
		}

		public FieldDescription.FontStyleType LabelFontStyle
		{
			get
			{
				return (FieldDescription.FontStyleType) this.GetValue(Mode.LabelFontStyleProperty);
			}
			set
			{
				this.SetValue(Mode.LabelFontStyleProperty, value);
			}
		}

		public FieldDescription.FontStyleType FieldFontStyle
		{
			get
			{
				return (FieldDescription.FontStyleType) this.GetValue(Mode.FieldFontStyleProperty);
			}
			set
			{
				this.SetValue(Mode.FieldFontStyleProperty, value);
			}
		}

		public FieldDescription.FontSizeType LabelFontSize
		{
			get
			{
				return (FieldDescription.FontSizeType) this.GetValue(Mode.LabelFontSizeProperty);
			}
			set
			{
				this.SetValue(Mode.LabelFontSizeProperty, value);
			}
		}

		public FieldDescription.FontSizeType FieldFontSize
		{
			get
			{
				return (FieldDescription.FontSizeType) this.GetValue(Mode.FieldFontSizeProperty);
			}
			set
			{
				this.SetValue(Mode.FieldFontSizeProperty, value);
			}
		}

		public FrameState BoxFrameState
		{
			get
			{
				return (FrameState) this.GetValue(Mode.BoxFrameStateProperty);
			}
			set
			{
				this.SetValue(Mode.BoxFrameStateProperty, value);
			}
		}

		public double BoxFrameWidth
		{
			get
			{
				return (double) this.GetValue(Mode.BoxFrameWidthProperty);
			}
			set
			{
				this.SetValue(Mode.BoxFrameWidthProperty, value);
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
				this.BackColor = this.ObjectModifier.GetBackColor(this.DefaultWidget);
			}

			if (this.ObjectModifier.IsField(this.DefaultWidget) ||
				this.ObjectModifier.IsBox(this.DefaultWidget))
			{
				this.LabelFontColor = this.ObjectModifier.GetLabelFontColor(this.DefaultWidget);
				this.FieldFontColor = this.ObjectModifier.GetFieldFontColor(this.DefaultWidget);
				this.LabelFontFace  = this.ObjectModifier.GetLabelFontFace(this.DefaultWidget);
				this.FieldFontFace  = this.ObjectModifier.GetFieldFontFace(this.DefaultWidget);
				this.LabelFontStyle = this.ObjectModifier.GetLabelFontStyle(this.DefaultWidget);
				this.FieldFontStyle = this.ObjectModifier.GetFieldFontStyle(this.DefaultWidget);
				this.LabelFontSize  = this.ObjectModifier.GetLabelFontSize(this.DefaultWidget);
				this.FieldFontSize  = this.ObjectModifier.GetFieldFontSize(this.DefaultWidget);
			}

			if (this.ObjectModifier.IsTitle(this.DefaultWidget))
			{
				this.LabelFontColor = this.ObjectModifier.GetLabelFontColor(this.DefaultWidget);
				this.LabelFontFace  = this.ObjectModifier.GetLabelFontFace(this.DefaultWidget);
				this.LabelFontStyle = this.ObjectModifier.GetLabelFontStyle(this.DefaultWidget);
			}
		}

		static Mode()
		{
			EnumType separatorBottomEnumType = Res.Types.FieldDescription.SeparatorType;
			Mode.SeparatorBottomProperty.DefaultMetadata.DefineNamedType(separatorBottomEnumType);
			Mode.SeparatorBottomProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.SeparatorBottomType.Id);

			EnumType boxPaddingBottomEnumType = Res.Types.FieldDescription.BoxPaddingType;
			Mode.BoxPaddingProperty.DefaultMetadata.DefineNamedType(boxPaddingBottomEnumType);
			Mode.BoxPaddingProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.BoxPaddingType.Id);

			EnumType boxFrameStateEnumType = Res.Types.FieldDescription.FrameState;
			Mode.BoxFrameStateProperty.DefaultMetadata.DefineNamedType(boxFrameStateEnumType);
			Mode.BoxFrameStateProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.BoxFrameState.Id);

			Mode.BoxFrameWidthProperty.DefaultMetadata.DefineNamedType(ProxyManager.WidthNumericType);
			Mode.BoxFrameWidthProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.BoxFrameWidth.Id);

			EnumType backColorEnumType = Res.Types.FieldDescription.BackColorType;
			Mode.BackColorProperty.DefaultMetadata.DefineNamedType(backColorEnumType);
			Mode.BackColorProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.BackColorType.Id);

			EnumType fontColorEnumType = Res.Types.FieldDescription.FontColorType;
			Mode.LabelFontColorProperty.DefaultMetadata.DefineNamedType(fontColorEnumType);
			Mode.LabelFontColorProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.LabelFontColorType.Id);
			Mode.FieldFontColorProperty.DefaultMetadata.DefineNamedType(fontColorEnumType);
			Mode.FieldFontColorProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.FieldFontColorType.Id);

			EnumType fontFaceEnumType = Res.Types.FieldDescription.FontFaceType;
			Mode.LabelFontFaceProperty.DefaultMetadata.DefineNamedType(fontFaceEnumType);
			Mode.LabelFontFaceProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.LabelFontFaceType.Id);
			Mode.FieldFontFaceProperty.DefaultMetadata.DefineNamedType(fontFaceEnumType);
			Mode.FieldFontFaceProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.FieldFontFaceType.Id);

			EnumType fontStyleEnumType = Res.Types.FieldDescription.FontStyleType;
			Mode.LabelFontStyleProperty.DefaultMetadata.DefineNamedType(fontStyleEnumType);
			Mode.LabelFontStyleProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.LabelFontStyleType.Id);
			Mode.FieldFontStyleProperty.DefaultMetadata.DefineNamedType(fontStyleEnumType);
			Mode.FieldFontStyleProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.FieldFontStyleType.Id);

			EnumType fontSizeEnumType = Res.Types.FieldDescription.FontSizeType;
			Mode.LabelFontSizeProperty.DefaultMetadata.DefineNamedType(fontSizeEnumType);
			Mode.LabelFontSizeProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.LabelFontSizeType.Id);
			Mode.FieldFontSizeProperty.DefaultMetadata.DefineNamedType(fontSizeEnumType);
			Mode.FieldFontSizeProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.FieldFontSizeType.Id);
		}


		private static void NotifySeparatorBottomChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.SeparatorType value = (FieldDescription.SeparatorType) newValue;
			Mode that = (Mode) o;

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
			Mode that = (Mode) o;

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

		private static void NotifyBackColorChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.BackColorType value = (FieldDescription.BackColorType) newValue;
			Mode that = (Mode) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetBackColor(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyLabelFontColorChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.FontColorType value = (FieldDescription.FontColorType) newValue;
			Mode that = (Mode) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetLabelFontColor(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyFieldFontColorChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.FontColorType value = (FieldDescription.FontColorType) newValue;
			Mode that = (Mode) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetFieldFontColor(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyLabelFontFaceChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.FontFaceType value = (FieldDescription.FontFaceType) newValue;
			Mode that = (Mode) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetLabelFontFace(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyFieldFontFaceChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.FontFaceType value = (FieldDescription.FontFaceType) newValue;
			Mode that = (Mode) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetFieldFontFace(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyLabelFontStyleChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.FontStyleType value = (FieldDescription.FontStyleType) newValue;
			Mode that = (Mode) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetLabelFontStyle(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyFieldFontStyleChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.FontStyleType value = (FieldDescription.FontStyleType) newValue;
			Mode that = (Mode) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetFieldFontStyle(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyLabelFontSizeChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.FontSizeType value = (FieldDescription.FontSizeType) newValue;
			Mode that = (Mode) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetLabelFontSize(obj, value);
					}
				}
				finally
				{
					that.ResumeChanges();
					that.RegenerateProxiesAndForm();
				}
			}
		}

		private static void NotifyFieldFontSizeChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.FontSizeType value = (FieldDescription.FontSizeType) newValue;
			Mode that = (Mode) o;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetFieldFontSize(obj, value);
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
			Mode that = (Mode) o;

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
			Mode that = (Mode) o;

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


		public static readonly DependencyProperty SeparatorBottomProperty = DependencyProperty.Register("SeparatorBottom", typeof(FieldDescription.SeparatorType),  typeof(Mode), new DependencyPropertyMetadata(FieldDescription.SeparatorType.Normal,  Mode.NotifySeparatorBottomChanged));
		public static readonly DependencyProperty BoxPaddingProperty      = DependencyProperty.Register("BoxPadding",      typeof(FieldDescription.BoxPaddingType), typeof(Mode), new DependencyPropertyMetadata(FieldDescription.BoxPaddingType.Normal, Mode.NotifyBoxPaddingChanged));
		public static readonly DependencyProperty BackColorProperty       = DependencyProperty.Register("BackColor",       typeof(FieldDescription.BackColorType),  typeof(Mode), new DependencyPropertyMetadata(FieldDescription.BackColorType.None,    Mode.NotifyBackColorChanged));
		public static readonly DependencyProperty LabelFontColorProperty  = DependencyProperty.Register("LabelFontColor",  typeof(FieldDescription.FontColorType),  typeof(Mode), new DependencyPropertyMetadata(FieldDescription.FontColorType.Default, Mode.NotifyLabelFontColorChanged));
		public static readonly DependencyProperty FieldFontColorProperty  = DependencyProperty.Register("FieldFontColor",  typeof(FieldDescription.FontColorType),  typeof(Mode), new DependencyPropertyMetadata(FieldDescription.FontColorType.Default, Mode.NotifyFieldFontColorChanged));
		public static readonly DependencyProperty LabelFontFaceProperty   = DependencyProperty.Register("LabelFontFace",   typeof(FieldDescription.FontFaceType),   typeof(Mode), new DependencyPropertyMetadata(FieldDescription.FontFaceType.Default,  Mode.NotifyLabelFontFaceChanged));
		public static readonly DependencyProperty FieldFontFaceProperty   = DependencyProperty.Register("FieldFontFace",   typeof(FieldDescription.FontFaceType),   typeof(Mode), new DependencyPropertyMetadata(FieldDescription.FontFaceType.Default,  Mode.NotifyFieldFontFaceChanged));
		public static readonly DependencyProperty LabelFontStyleProperty  = DependencyProperty.Register("LabelFontStyle",  typeof(FieldDescription.FontStyleType),  typeof(Mode), new DependencyPropertyMetadata(FieldDescription.FontStyleType.Normal,  Mode.NotifyLabelFontStyleChanged));
		public static readonly DependencyProperty FieldFontStyleProperty  = DependencyProperty.Register("FieldFontStyle",  typeof(FieldDescription.FontStyleType),  typeof(Mode), new DependencyPropertyMetadata(FieldDescription.FontStyleType.Normal,  Mode.NotifyFieldFontStyleChanged));
		public static readonly DependencyProperty LabelFontSizeProperty   = DependencyProperty.Register("LabelFontSize",   typeof(FieldDescription.FontSizeType),   typeof(Mode), new DependencyPropertyMetadata(FieldDescription.FontSizeType.Normal,   Mode.NotifyLabelFontSizeChanged));
		public static readonly DependencyProperty FieldFontSizeProperty   = DependencyProperty.Register("FieldFontSize",   typeof(FieldDescription.FontSizeType),   typeof(Mode), new DependencyPropertyMetadata(FieldDescription.FontSizeType.Normal,   Mode.NotifyFieldFontSizeChanged));
		public static readonly DependencyProperty BoxFrameStateProperty   = DependencyProperty.Register("BoxFrameState",   typeof(FrameState),                      typeof(Mode), new DependencyPropertyMetadata(FrameState.None,                        Mode.NotifyBoxFrameStateChanged));
		public static readonly DependencyProperty BoxFrameWidthProperty   = DependencyProperty.Register("BoxFrameWidth",   typeof(double),                          typeof(Mode), new DependencyPropertyMetadata(1.0,                                    Mode.NotifyBoxFrameWidthChanged));
	}
}
