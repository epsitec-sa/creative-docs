//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Menu composé d'une icône suivie d'un texte. Ce composant est utilisé pour les
	/// menus contextuels actionnés avec le bouton de droite de la souris.
	/// </summary>
	public class MenuPopup : AbstractPopup
	{
		public MenuPopup(AbstractCommandToolbar toolbar)
		{
			this.toolbar = toolbar;

			this.commands = new List<ToolbarCommand> ();
			this.actions = new Dictionary<ToolbarCommand, System.Action> ();
		}


		public void AddSeparator()
		{
			this.commands.Add (ToolbarCommand.Unknown);
		}

		public void AddItem(ToolbarCommand command, System.Action action)
		{
			var state = this.toolbar.GetCommandState (command);

			if (state == ToolbarCommandState.Enable)
			{
				this.commands.Add (command);
				this.actions.Add (command, action);
			}
		}


		protected override Size DialogSize
		{
			get
			{
				int dx = MenuPopup.margins*2 + this.RequiredWidth;
				int dy = MenuPopup.margins*2 + this.RequiredHeight;

				return new Size (dx, dy);
			}
		}

		public override void CreateUI()
		{
			int h = MenuPopup.margins + this.RequiredHeight;
			int w = this.RequiredWidth;
			int i = 0;

			foreach (int y in this.PosY)
			{
				var command = this.commands[i];

				if (command == ToolbarCommand.Unknown)
				{
					this.CreateSeparator (h+y, w);
				}
				else
				{
					this.CreateItem (i, h+y, w);
				}

				i++;
			}
		}

		private void CreateSeparator(int y, int width)
		{
			int dx = width + MenuPopup.margins*2;

			new FrameBox
			{
				Parent        = this.mainFrameBox,
				Anchor        = AnchorStyles.BottomLeft,
				PreferredSize = new Size (dx, 1),
				Margins       = new Margins (0, 0, 0, y),
				BackColor     = ColorManager.PopupBorderColor,
			};
		}

		private void CreateItem(int rank, int y, int width)
		{
			int x = MenuPopup.margins;
			int dx = width;
			int dy = MenuPopup.itemHeight;

			var frame = new FrameBox
			{
				Parent        = this.mainFrameBox,
				Anchor        = AnchorStyles.BottomLeft,
				PreferredSize = new Size (dx, dy),
				Margins       = new Margins (x, 0, 0, y),
			};

			var desc = this.toolbar.GetCommandDescription (this.commands[rank]);

			var icon = new IconButton
			{
				Parent        = frame,
				IconUri       = Misc.GetResourceIconUri (desc.Icon),
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (dy, dy),
			};

			var text = new ColoredButton
			{
				Parent           = frame,
				Text             = this.GetTextWithGaps (rank),
				ContentAlignment = ContentAlignment.MiddleLeft,
				AutoFocus        = false,
				NormalColor      = Color.Empty,
				Dock             = DockStyle.Fill,
				PreferredHeight  = dy,
			};

			frame.Clicked += delegate
			{
				this.DoAction (rank);
			};

			icon.Clicked += delegate
			{
				this.DoAction (rank);
			};

			text.Clicked += delegate
			{
				this.DoAction (rank);
			};
		}

		private void DoAction(int rank)
		{
			this.ClosePopup ();

			var command = this.commands[rank];
			this.actions[command] ();  // effectue l'action
		}

		private int RequiredWidth
		{
			//	Calcule la largeur nécessaire en fonction de l'ensemble des textes.
			get
			{
				return this.commands.Max
				(
					command => MenuPopup.GetTextWithGaps (this.toolbar.GetCommandDescription (command).Tooltip).GetTextWidth ()
				)
				+ MenuPopup.itemHeight
				+ ColoredButton.horizontalMargins * 2
				+ 3;  // visuellement, il est bon d'avoir un chouia d'espace en plus à droite
			}
		}

		private string GetTextWithGaps(int rank)
		{
			var desc = this.toolbar.GetCommandDescription (this.commands[rank]);
			return MenuPopup.GetTextWithGaps (desc.Tooltip);
		}

		private static string GetTextWithGaps(string text)
		{
			return string.Concat (MenuPopup.textGap, text, MenuPopup.textGap);
		}


		private int RequiredHeight
		{
			get
			{
				return -this.PosY.Last ();
			}
		}

		private IEnumerable<int> PosY
		{
			get
			{
				int y = 0;
				bool separator = false;

				for (int i=0; i<this.commands.Count; i++)
				{
					var command = this.commands[i];

					if (command == ToolbarCommand.Unknown)
					{
						if (separator)  // compact ?
						{
							y += MenuPopup.sepHeight/2 - 2;
							yield return y;
							y -= MenuPopup.sepHeight/2;
						}
						else
						{
							y -= MenuPopup.sepHeight/2;
							yield return y;
							y -= MenuPopup.sepHeight/2;
						}

						separator = true;
					}
					else
					{
						y -= MenuPopup.itemHeight;
						yield return y;
						separator = false;
					}
				}
			}
		}


		private const int							margins		= 5;
		private const int							itemHeight	= 26;
		private const int							sepHeight	= 8;
		private const string						textGap		= "  ";

		private readonly AbstractCommandToolbar		toolbar;
		private readonly List<ToolbarCommand>		commands;
		private readonly Dictionary<ToolbarCommand, System.Action> actions;
	}
}