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

		public ButtonDisplayMode ButtonDisplayMode
		{
			get
			{
				return (ButtonDisplayMode) this.GetValue(Aspect.ButtonDisplayModeProperty);
			}
			set
			{
				this.SetValue(Aspect.ButtonDisplayModeProperty, value);
			}
		}


		protected override void InitializePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			if (this.ObjectModifier.HasButtonDisplayMode(this.DefaultWidget))
			{
				ButtonDisplayMode bdm = this.ObjectModifier.GetButtonDisplayMode(this.DefaultWidget);

				this.ButtonDisplayMode = bdm;
			}
		}


		private static void NotifyButtonDisplayModeChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Aspect that = (Aspect) o;
			ButtonDisplayMode bdm = (ButtonDisplayMode) newValue;

			if (that.IsNotSuspended)
			{
				that.SuspendChanges();

				try
				{
					foreach (Widget obj in that.Widgets)
					{
						that.ObjectModifier.SetButtonDisplayMode(obj, bdm);
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
			EnumType buttonDisplayModeEnumType = Res.Types.Widgets.ButtonDisplayMode;
			Aspect.ButtonDisplayModeProperty.DefaultMetadata.DefineNamedType(buttonDisplayModeEnumType);
			Aspect.ButtonDisplayModeProperty.DefaultMetadata.DefineCaptionId(Res.Captions.Aspect.ButtonDisplayMode.Druid);
		}


		public static readonly DependencyProperty ButtonDisplayModeProperty = DependencyProperty.Register("ButtonDisplayMode", typeof(ButtonDisplayMode), typeof(Aspect), new DependencyPropertyMetadata(ButtonDisplayMode.Automatic, Aspect.NotifyButtonDisplayModeChanged));
	}
}
