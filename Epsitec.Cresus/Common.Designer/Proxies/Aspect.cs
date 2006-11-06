using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Proxies
{
	public class Aspect : Abstract
	{
		public Aspect(ProxyManager manager) : base (manager)
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

		public ButtonAspect ButtonAspect
		{
			get
			{
				return (ButtonAspect) this.GetValue(Aspect.ButtonAspectProperty);
			}
			set
			{
				this.SetValue(Aspect.ButtonAspectProperty, value);
			}
		}


		protected override void InitializePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			if (this.ObjectModifier.HasButtonAspect(this.DefaultWidget))
			{
				ButtonAspect aspect = this.ObjectModifier.GetButtonAspect(this.DefaultWidget);

				this.ButtonAspect = aspect;
			}
		}


		private static void NotifyButtonAspectChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Aspect that = (Aspect) o;
			ButtonAspect aspect = (ButtonAspect) newValue;

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


		public static readonly DependencyProperty ButtonAspectProperty = DependencyProperty.Register("ButtonAspect", typeof(ButtonAspect), typeof(Aspect), new DependencyPropertyMetadata(ButtonAspect.None, Aspect.NotifyButtonAspectChanged));
	}
}
