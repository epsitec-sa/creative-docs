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
			//	Gestion des touches Tab et Return. On cherche le widget suivant (ou précédent si Shift)
			//	dans la parenté, d'après les propriétés TabIndex (les trous ne sont pas admis).
			//	Si le widget est un TextFieldHexa utilisé dans un MemoryAccessor, on fait défiler
			//	la mémoire lorsqu'on bute au début ou à la fin.
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
						if (newWidget == null)  // on bute sur une extrémité ?
						{
							TextFieldHexa hexa = field.Parent as TextFieldHexa;
							if (hexa != null)
							{
								MemoryAccessor ma = hexa.MemoryAccessor;
								if (ma != null)
								{
									ma.FirstAddress = ma.FirstAddress+offset;

									newWidget = this.SearchTabIndex(widget, widget.TabIndex);
									if (newWidget != null)
									{
										this.SetDolphinFocus(newWidget);
									}
								}
							}
						}
						else  // widget suivant/précédent trouvé ?
						{
							this.SetDolphinFocus(newWidget);
						}
					}
				}
			}
		}

		protected void SetDolphinFocus(Widget widget)
		{
			//	Met le focus dans un widget.
			this.SetDolphinFocusedWidget(widget);
			widget.Focus();

			TextField field = widget as TextField;
			if (field != null)
			{
				field.SelectAll();
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
