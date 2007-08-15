//	Copyright � 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Panneau principal, qui g�re les touches Tab et Return.
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
			//	Gestion des touches Tab et Return. On cherche le widget suivant (ou pr�c�dent si Shift)
			//	dans la parent�, d'apr�s les propri�t�s TabIndex (les trous ne sont pas admis).
			//	Si le widget est un TextFieldHexa utilis� dans un MemoryAccessor, on fait d�filer
			//	la m�moire lorsqu'on bute au d�but ou � la fin.
			if (message.MessageType == MessageType.KeyDown)
			{
				int offset = 0;

				if (message.KeyCode == KeyCode.Tab || message.KeyCode == KeyCode.Return)
				{
					offset = message.IsShiftPressed ? -1 : 1;
					message.Consumer = this;
				}

				if (message.KeyCode == KeyCode.ArrowDown)
				{
					offset = 1;
					message.Consumer = this;
				}

				if (message.KeyCode == KeyCode.ArrowUp)
				{
					offset = -1;
					message.Consumer = this;
				}

				if (offset != 0)
				{
					Widget widget = this.dolphinFocusedWidget;
					TextField field = widget as TextField;

					Code actualCode = field.Parent as Code;
					if (actualCode != null && actualCode.IsErrorMet)
					{
						return;
					}

					if (field != null)
					{
						Widget newWidget = this.SearchTabIndex(widget, widget.TabIndex+offset);
						if (newWidget == null)  // on bute sur une extr�mit� ?
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

							Code code = field.Parent as Code;
							if (code != null)
							{
								CodeAccessor ca = code.CodeAccessor;
								if (ca != null)
								{
									ca.FirstAddress = ca.NextFirstAddress(offset);

									newWidget = this.SearchTabIndex(widget, widget.TabIndex);
									if (newWidget != null)
									{
										this.SetDolphinFocus(newWidget);
									}
								}
							}
						}
						else  // widget suivant/pr�c�dent trouv� ?
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
