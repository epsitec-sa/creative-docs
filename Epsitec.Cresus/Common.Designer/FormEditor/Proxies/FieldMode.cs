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


		protected override void InitializePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			this.Separator = this.ObjectModifier.GetSeparator(this.DefaultWidget);
		}

		static FieldMode()
		{
			EnumType separatorEnumType = Res.Types.FieldDescription.SeparatorType;
			FieldMode.SeparatorProperty.DefaultMetadata.DefineNamedType(separatorEnumType);
			FieldMode.SeparatorProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Grid.ColumnMode.Id);
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


		public static readonly DependencyProperty SeparatorProperty = DependencyProperty.Register("Separator", typeof(FieldDescription.SeparatorType), typeof(FieldMode), new DependencyPropertyMetadata(FieldDescription.SeparatorType.Normal, FieldMode.NotifySeparatorChanged));
	}
}
