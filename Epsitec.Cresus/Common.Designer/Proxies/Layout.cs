using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Proxies
{
	public class Layout : Abstract
	{
		public Layout(Widget widget) : base(widget)
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
				return "PropertyLayout";
			}
		}

		public Widgets.Layouts.LayoutMode LayoutMode
		{
			get
			{
				return (Widgets.Layouts.LayoutMode) this.GetValue(Layout.LayoutModeProperty);
			}
			set
			{
				this.SetValue(Layout.LayoutModeProperty, value);
			}
		}

		public ContainerLayoutMode ContainerMode
		{
			get
			{
				return (ContainerLayoutMode) this.GetValue(Layout.ContainerModeProperty);
			}
			set
			{
				this.SetValue(Layout.ContainerModeProperty, value);
			}
		}


		protected override void InitialisePropertyValues()
		{
			//	Cette méthode est appelée par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propriétés du widget sélectionné
			//	pour pouvoir ensuite travailler dessus :
			Widgets.Layouts.LayoutMode layout = (Widgets.Layouts.LayoutMode) this.GetWidgetProperty(AbstractGroup.ChildrenLayoutModeProperty);
			ContainerLayoutMode container = (ContainerLayoutMode) this.GetWidgetProperty(Visual.ContainerLayoutModeProperty);

			this.LayoutMode = layout;
			this.ContainerMode = container;
		}

		private static void NotifyChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette méthode est appelée à la suite de la modification d'une de
			//	nos propriétés de définition pour permettre de mettre à jour les
			//	widgets connectés :
			Layout that = (Layout) o;
			Widgets.Layouts.LayoutMode layout = that.LayoutMode;
			ContainerLayoutMode container = that.ContainerMode;
			
			//	Demande à Proxies.Abstract de mettre à jour la propriété qui
			//	définit le layout du ou des widget(s) sélectionné(s) :
			that.SetWidgetProperty(AbstractGroup.ChildrenLayoutModeProperty, layout);
			that.SetWidgetProperty(Visual.ContainerLayoutModeProperty, container);
		}


		public static readonly DependencyProperty LayoutModeProperty    = DependencyProperty.Register("LayoutMode",    typeof(Widgets.Layouts.LayoutMode), typeof(Layout), new DependencyPropertyMetadata(Widgets.Layouts.LayoutMode.None, Layout.NotifyChanged));
		public static readonly DependencyProperty ContainerModeProperty = DependencyProperty.Register("ContainerMode", typeof(ContainerLayoutMode),        typeof(Layout), new DependencyPropertyMetadata(ContainerLayoutMode.None,        Layout.NotifyChanged));
	}
}
