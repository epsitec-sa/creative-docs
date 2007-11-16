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
			//	Plus le num�ro est petit, plus le proxy appara�tra haut dans la
			//	liste.
			//	Les Proxies de PanelEditor sont num�rot�s � partir de 0.
			//	Ceux de FormEditor sont num�rot�s � partir de 100.
			//	Ceci �vite les collisions dans PanelsContext.IsExtendedProxies !
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


		public FieldDescription.SeparatorType Separator
		{
			get
			{
				return (FieldDescription.SeparatorType) this.GetValue(Mode.SeparatorProperty);
			}
			set
			{
				this.SetValue(Mode.SeparatorProperty, value);
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


		protected override void InitializePropertyValues()
		{
			//	Cette m�thode est appel�e par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			if (this.ObjectModifier.IsField(this.DefaultWidget))
			{
				this.Separator = this.ObjectModifier.GetSeparator(this.DefaultWidget);
			}

			if (this.ObjectModifier.IsField(this.DefaultWidget) ||
				this.ObjectModifier.IsBox(this.DefaultWidget)   )
			{
				this.BackColor = this.ObjectModifier.GetBackColor(this.DefaultWidget);
			}
		}

		static Mode()
		{
			EnumType separatorEnumType = Res.Types.FieldDescription.SeparatorType;
			Mode.SeparatorProperty.DefaultMetadata.DefineNamedType(separatorEnumType);
			Mode.SeparatorProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.SeparatorType.Id);

			EnumType backColorEnumType = Res.Types.FieldDescription.BackColorType;
			Mode.BackColorProperty.DefaultMetadata.DefineNamedType(backColorEnumType);
			Mode.BackColorProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.BackColorType.Id);
		}


		private static void NotifySeparatorChanged(DependencyObject o, object oldValue, object newValue)
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
						that.ObjectModifier.SetSeparator(obj, value);
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


		public static readonly DependencyProperty SeparatorProperty = DependencyProperty.Register("Separator", typeof(FieldDescription.SeparatorType), typeof(Mode), new DependencyPropertyMetadata(FieldDescription.SeparatorType.Normal, Mode.NotifySeparatorChanged));
		public static readonly DependencyProperty BackColorProperty = DependencyProperty.Register("BackColor", typeof(FieldDescription.BackColorType), typeof(Mode), new DependencyPropertyMetadata(FieldDescription.BackColorType.None, Mode.NotifyBackColorChanged));
	}
}
