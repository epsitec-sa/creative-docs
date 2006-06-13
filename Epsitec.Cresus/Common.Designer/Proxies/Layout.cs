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


		protected override void InitialisePropertyValues()
		{
			//	Cette m�thode est appel�e par Proxies.Abstract quand on connecte
			//	le premier widget avec le proxy.
			
			//	Recopie localement les diverses propri�t�s du widget s�lectionn�
			//	pour pouvoir ensuite travailler dessus :
			Widgets.Layouts.LayoutMode layout = (Widgets.Layouts.LayoutMode) this.GetWidgetProperty(AbstractGroup.ChildrenLayoutModeProperty);
			this.LayoutMode = layout;
		}

		private static void NotifyLayoutChanged(DependencyObject o, object oldValue, object newValue)
		{
			//	Cette m�thode est appel�e � la suite de la modification d'une de
			//	nos propri�t�s de d�finition pour permettre de mettre � jour les
			//	widgets connect�s :
			Layout that = (Layout) o;
			Widgets.Layouts.LayoutMode layout = that.LayoutMode;
			
			//	Demande � Proxies.Abstract de mettre � jour la propri�t� qui
			//	d�finit le layout du ou des widget(s) s�lectionn�(s) :
			that.SetWidgetProperty(AbstractGroup.ChildrenLayoutModeProperty, layout);
		}


		public static readonly DependencyProperty LayoutModeProperty = DependencyProperty.Register("LayoutMode", typeof(Widgets.Layouts.LayoutMode), typeof(Layout), new DependencyPropertyMetadata(0.0, Layout.NotifyLayoutChanged));
	}
}
