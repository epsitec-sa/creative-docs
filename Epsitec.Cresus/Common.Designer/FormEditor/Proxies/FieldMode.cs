using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.FormEngine;

namespace Epsitec.Common.Designer.FormEditor.Proxies
{
	public class FieldMode : Abstract
	{
		public FieldMode(ProxyManager manager) : base (manager)
		{
		}

		public override int Rank
		{
			//	Retourne le rang de ce proxy parmi la liste de tous les proxies.
			//	Plus le numéro est petit, plus le proxy apparaîtra haut dans la
			//	liste.
			get
			{
				return 100+2;
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
				return (FieldDescription.SeparatorType) this.GetValue(FieldMode.SeparatorProperty);
			}
			set
			{
				this.SetValue(FieldMode.SeparatorProperty, value);
			}
		}

		public FieldDescription.BackColorType BackColor
		{
			get
			{
				return (FieldDescription.BackColorType) this.GetValue(FieldMode.BackColorProperty);
			}
			set
			{
				this.SetValue(FieldMode.BackColorProperty, value);
			}
		}


		protected override void InitializePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			this.Separator = this.ObjectModifier.GetSeparator(this.DefaultWidget);
			this.BackColor = this.ObjectModifier.GetBackColor(this.DefaultWidget);
		}

		static FieldMode()
		{
			EnumType separatorEnumType = Res.Types.FieldDescription.SeparatorType;
			FieldMode.SeparatorProperty.DefaultMetadata.DefineNamedType(separatorEnumType);
			FieldMode.SeparatorProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.SeparatorType.Id);

			EnumType backColorEnumType = Res.Types.FieldDescription.BackColorType;
			FieldMode.BackColorProperty.DefaultMetadata.DefineNamedType(backColorEnumType);
			FieldMode.BackColorProperty.DefaultMetadata.DefineCaptionId(Res.Captions.FieldMode.BackColorType.Id);
		}


		private static void NotifySeparatorChanged(DependencyObject o, object oldValue, object newValue)
		{
			FieldDescription.SeparatorType value = (FieldDescription.SeparatorType) newValue;
			FieldMode that = (FieldMode) o;

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
			FieldMode that = (FieldMode) o;

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


		public static readonly DependencyProperty SeparatorProperty = DependencyProperty.Register("Separator", typeof(FieldDescription.SeparatorType), typeof(FieldMode), new DependencyPropertyMetadata(FieldDescription.SeparatorType.Normal, FieldMode.NotifySeparatorChanged));
		public static readonly DependencyProperty BackColorProperty = DependencyProperty.Register("BackColor", typeof(FieldDescription.BackColorType), typeof(FieldMode), new DependencyPropertyMetadata(FieldDescription.BackColorType.None, FieldMode.NotifyBackColorChanged));
	}
}
