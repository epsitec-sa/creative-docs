using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.FormEngine;

namespace Epsitec.Common.Designer.FormEditor.Proxies
{
	public class Style : Abstract
	{
		public Style(ProxyManager manager) : base (manager)
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


		public FieldDescription.BackColorType BackColor
		{
			get
			{
				return (FieldDescription.BackColorType) this.GetValue(Style.BackColorProperty);
			}
			set
			{
				this.SetValue(Style.BackColorProperty, value);
			}
		}

		public FieldDescription.FontColorType LabelFontColor
		{
			get
			{
				return (FieldDescription.FontColorType) this.GetValue(Style.LabelFontColorProperty);
			}
			set
			{
				this.SetValue(Style.LabelFontColorProperty, value);
			}
		}

		public FieldDescription.FontColorType FieldFontColor
		{
			get
			{
				return (FieldDescription.FontColorType) this.GetValue(Style.FieldFontColorProperty);
			}
			set
			{
				this.SetValue(Style.FieldFontColorProperty, value);
			}
		}

		public FieldDescription.FontFaceType LabelFontFace
		{
			get
			{
				return (FieldDescription.FontFaceType) this.GetValue(Style.LabelFontFaceProperty);
			}
			set
			{
				this.SetValue(Style.LabelFontFaceProperty, value);
			}
		}

		public FieldDescription.FontFaceType FieldFontFace
		{
			get
			{
				return (FieldDescription.FontFaceType) this.GetValue(Style.FieldFontFaceProperty);
			}
			set
			{
				this.SetValue(Style.FieldFontFaceProperty, value);
			}
		}

		public FieldDescription.FontStyleType LabelFontStyle
		{
			get
			{
				return (FieldDescription.FontStyleType) this.GetValue(Style.LabelFontStyleProperty);
			}
			set
			{
				this.SetValue(Style.LabelFontStyleProperty, value);
			}
		}

		public FieldDescription.FontStyleType FieldFontStyle
		{
			get
			{
				return (FieldDescription.FontStyleType) this.GetValue(Style.FieldFontStyleProperty);
			}
			set
			{
				this.SetValue(Style.FieldFontStyleProperty, value);
			}
		}

		public FieldDescription.FontSizeType LabelFontSize
		{
			get
			{
				return (FieldDescription.FontSizeType) this.GetValue(Style.LabelFontSizeProperty);
			}
			set
			{
				this.SetValue(Style.LabelFontSizeProperty, value);
			}
		}

		public FieldDescription.FontSizeType FieldFontSize
		{
			get
			{
				return (FieldDescription.FontSizeType) this.GetValue(Style.FieldFontSizeProperty);
			}
			set
			{
				this.SetValue(Style.FieldFontSizeProperty, value);
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

			if (this.ObjectModifier.IsTitle(this.DefaultWidget) ||
				this.ObjectModifier.IsCommand(this.DefaultWidget))
			{
				this.LabelFontColor = this.ObjectModifier.GetLabelFontColor(this.DefaultWidget);
				this.LabelFontFace  = this.ObjectModifier.GetLabelFontFace(this.DefaultWidget);
				this.LabelFontStyle = this.ObjectModifier.GetLabelFontStyle(this.DefaultWidget);
				this.LabelFontSize  = this.ObjectModifier.GetLabelFontSize(this.DefaultWidget);
			}
		}

		static Style()
		{
			EnumType backColorEnumType = Res.Types.FieldDescription.BackColorType;
			Style.BackColorProperty.DefaultMetadata.DefineNamedType(backColorEnumType);
			Style.BackColorProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.BackColorType.Id);

			EnumType fontColorEnumType = Res.Types.FieldDescription.FontColorType;
			Style.LabelFontColorProperty.DefaultMetadata.DefineNamedType(fontColorEnumType);
			Style.LabelFontColorProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.LabelFontColorType.Id);
			Style.FieldFontColorProperty.DefaultMetadata.DefineNamedType(fontColorEnumType);
			Style.FieldFontColorProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.FieldFontColorType.Id);

			EnumType fontFaceEnumType = Res.Types.FieldDescription.FontFaceType;
			Style.LabelFontFaceProperty.DefaultMetadata.DefineNamedType(fontFaceEnumType);
			Style.LabelFontFaceProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.LabelFontFaceType.Id);
			Style.FieldFontFaceProperty.DefaultMetadata.DefineNamedType(fontFaceEnumType);
			Style.FieldFontFaceProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.FieldFontFaceType.Id);

			EnumType fontStyleEnumType = Res.Types.FieldDescription.FontStyleType;
			Style.LabelFontStyleProperty.DefaultMetadata.DefineNamedType(fontStyleEnumType);
			Style.LabelFontStyleProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.LabelFontStyleType.Id);
			Style.FieldFontStyleProperty.DefaultMetadata.DefineNamedType(fontStyleEnumType);
			Style.FieldFontStyleProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.FieldFontStyleType.Id);

			EnumType fontSizeEnumType = Res.Types.FieldDescription.FontSizeType;
			Style.LabelFontSizeProperty.DefaultMetadata.DefineNamedType(fontSizeEnumType);
			Style.LabelFontSizeProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.LabelFontSizeType.Id);
			Style.FieldFontSizeProperty.DefaultMetadata.DefineNamedType(fontSizeEnumType);
			Style.FieldFontSizeProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.FieldFontSizeType.Id);
		}


		private static void NotifyBackColorChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.BackColorType value = (FieldDescription.BackColorType) newValue;
			Style that = (Style) o;

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
			Style that = (Style) o;

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
			Style that = (Style) o;

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
			Style that = (Style) o;

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
			Style that = (Style) o;

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
			Style that = (Style) o;

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
			Style that = (Style) o;

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
			Style that = (Style) o;

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
			Style that = (Style) o;

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


		public static readonly DependencyProperty BackColorProperty       = DependencyProperty.Register("BackColor",       typeof(FieldDescription.BackColorType),  typeof(Style), new DependencyPropertyMetadata(FieldDescription.BackColorType.None,    Style.NotifyBackColorChanged));
		public static readonly DependencyProperty LabelFontColorProperty  = DependencyProperty.Register("LabelFontColor",  typeof(FieldDescription.FontColorType),  typeof(Style), new DependencyPropertyMetadata(FieldDescription.FontColorType.Default, Style.NotifyLabelFontColorChanged));
		public static readonly DependencyProperty FieldFontColorProperty  = DependencyProperty.Register("FieldFontColor",  typeof(FieldDescription.FontColorType),  typeof(Style), new DependencyPropertyMetadata(FieldDescription.FontColorType.Default, Style.NotifyFieldFontColorChanged));
		public static readonly DependencyProperty LabelFontFaceProperty   = DependencyProperty.Register("LabelFontFace",   typeof(FieldDescription.FontFaceType),   typeof(Style), new DependencyPropertyMetadata(FieldDescription.FontFaceType.Default,  Style.NotifyLabelFontFaceChanged));
		public static readonly DependencyProperty FieldFontFaceProperty   = DependencyProperty.Register("FieldFontFace",   typeof(FieldDescription.FontFaceType),   typeof(Style), new DependencyPropertyMetadata(FieldDescription.FontFaceType.Default,  Style.NotifyFieldFontFaceChanged));
		public static readonly DependencyProperty LabelFontStyleProperty  = DependencyProperty.Register("LabelFontStyle",  typeof(FieldDescription.FontStyleType),  typeof(Style), new DependencyPropertyMetadata(FieldDescription.FontStyleType.Normal,  Style.NotifyLabelFontStyleChanged));
		public static readonly DependencyProperty FieldFontStyleProperty  = DependencyProperty.Register("FieldFontStyle",  typeof(FieldDescription.FontStyleType),  typeof(Style), new DependencyPropertyMetadata(FieldDescription.FontStyleType.Normal,  Style.NotifyFieldFontStyleChanged));
		public static readonly DependencyProperty LabelFontSizeProperty   = DependencyProperty.Register("LabelFontSize",   typeof(FieldDescription.FontSizeType),   typeof(Style), new DependencyPropertyMetadata(FieldDescription.FontSizeType.Normal,   Style.NotifyLabelFontSizeChanged));
		public static readonly DependencyProperty FieldFontSizeProperty   = DependencyProperty.Register("FieldFontSize",   typeof(FieldDescription.FontSizeType),   typeof(Style), new DependencyPropertyMetadata(FieldDescription.FontSizeType.Normal,   Style.NotifyFieldFontSizeChanged));
	}
}
