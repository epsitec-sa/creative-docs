using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.PanelEditor.Proxies
{
	public class Aspect : Abstract
	{
		public Aspect(ProxyManager manager) : base (manager)
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

		public ButtonClass ButtonAspect
		{
			get
			{
				return (ButtonClass) this.GetValue(Aspect.ButtonAspectProperty);
			}
			set
			{
				this.SetValue(Aspect.ButtonAspectProperty, value);
			}
		}


		protected override void InitializePropertyValues()
		{
			//	Cette m�thode est appel�e par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propri�t�s du widget s�lectionn�
			//	pour pouvoir ensuite travailler dessus :
			if (this.ObjectModifier.HasButtonAspect(this.DefaultWidget))
			{
				ButtonClass aspect = this.ObjectModifier.GetButtonAspect(this.DefaultWidget);

				this.ButtonAspect = aspect;
			}
		}


		private static void NotifyButtonAspectChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition pour permettre de mettre � jour les
			//	widgets connect�s :
			Aspect that = (Aspect) o;
			ButtonClass aspect = (ButtonClass) newValue;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetButtonAspect(obj, aspect);
					}
				}
				finally
				{
					that.ResumeChanges();
				}
			}
		}

		
		static Aspect()
		{
			EnumType ButtonAspectEnumType = Res.Types.Widgets.ButtonAspect;
			Aspect.ButtonAspectProperty.DefaultMetadata.DefineNamedType(ButtonAspectEnumType);
			Aspect.ButtonAspectProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Aspect.ButtonAspect.Id);
		}


		public static readonly DependencyProperty ButtonAspectProperty = DependencyProperty.Register("ButtonAspect", typeof(ButtonClass), typeof(Aspect), new DependencyPropertyMetadata(ButtonClass.None, Aspect.NotifyButtonAspectChanged));
	}
}
