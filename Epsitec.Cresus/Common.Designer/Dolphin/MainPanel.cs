using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dolphin
{
	/// <summary>
	/// Panneau principal, qui gère les touches Tab et Return.
	/// </summary>
	public class MainPanel : Panel
	{
		public MainPanel() : base()
		{
		}

		public MainPanel(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public void SetDolphinFocusedWidget(Widget widget)
		{
			//	Indique quel widget a le focus.
			this.dolphinFocusedWidget = widget;
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.MessageType == MessageType.KeyDown)
			{
				if (message.KeyCode == KeyCode.Tab || message.KeyCode == KeyCode.Return)
				{
					Widget widget = this.dolphinFocusedWidget;
					TextField field = widget as TextField;
					if (field != null)
					{
						int offset = message.IsShiftPressed ? -1 : 1;
						Widget newWidget = this.SearchTabIndex(widget, widget.TabIndex+offset);
						if (newWidget != null)
						{
							this.SetDolphinFocusedWidget(newWidget);
							newWidget.Focus();

							TextField newField = newWidget as TextField;
							if (newField != null)
							{
								newField.SelectAll();
							}
						}
					}
				}
			}
		}

		protected Widget SearchTabIndex(Widget widget, int tabIndex)
		{
			//	Cherche, parmi tous les parents d'un widget, un widget ayant un certain index.
			while (widget.Parent != null)
			{
				widget = widget.Parent;
				Widget search = this.SearchChildrenTabIndex(widget, tabIndex);
				if (search != null)
				{
					return search;
				}
			}

			return null;
		}

		protected Widget SearchChildrenTabIndex(Widget parent, int tabIndex)
		{
			//	Cherche, parmi tous les enfants d'un widget, un widget ayant un certain index.
			foreach (Widget widget in parent.Children)
			{
				if (widget.TabIndex == tabIndex)
				{
					return widget;
				}

				foreach (Widget children in widget.Children)
				{
					Widget search = this.SearchChildrenTabIndex(children, tabIndex);
					if (search != null)
					{
						return search;
					}
				}
			}

			return null;
		}


		protected Widget dolphinFocusedWidget;
	}
}
