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
	public class MenuPopup : AbstractPopup
	{
		public MenuPopup()
		{
			this.items = new List<CommandCustomization> ();
		}


		public int AddItem()
		{
			//	Ajoute un séparateur horizontal.
			return this.AddItem (CommandCustomization.Empty);
		}

		public int AddItem(CommandCustomization command, ToolbarCommandState state = ToolbarCommandState.Enable)
		{
			if (state == ToolbarCommandState.Enable)
			{
				int rank = this.items.Count;
				this.items.Add (command);
				return rank;
			}
			else
			{
				return -1;
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
				var item = this.items[i];

				if (item.IsEmpty)
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

			var icon = new IconButton
			{
				Parent        = frame,
				IconUri       = Misc.GetResourceIconUri (this.items[rank].Icon),
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
				this.ClosePopup ();
				this.OnItemClicked (rank);
			};

			icon.Clicked += delegate
			{
				this.ClosePopup ();
				this.OnItemClicked (rank);
			};

			text.Clicked += delegate
			{
				this.ClosePopup ();
				this.OnItemClicked (rank);
			};
		}

		private int RequiredWidth
		{
			//	Calcule la largeur nécessaire en fonction de l'ensemble des textes.
			get
			{
				return this.items.Max
				(
					item => MenuPopup.GetTextWithGaps (item.Tooltip).GetTextWidth ()
				)
				+ MenuPopup.itemHeight
				+ ColoredButton.horizontalMargins * 2
				+ 3;  // visuellement, il est bon d'avoir un chouia d'espace en plus à droite
			}
		}

		private string GetTextWithGaps(int rank)
		{
			return MenuPopup.GetTextWithGaps (this.items[rank].Tooltip);
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

				for (int i=0; i<this.items.Count; i++)
				{
					var item = this.items[i];

					if (item.IsEmpty)
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



		#region Events handler
		private void OnItemClicked(int rank)
		{
			this.ItemClicked.Raise (this, rank);
		}

		public event EventHandler<int> ItemClicked;
		#endregion


		private const int						margins		= 5;
		private const int						itemHeight	= 26;
		private const int						sepHeight	= 8;
		private const string					textGap		= "  ";

		private readonly List<CommandCustomization> items;
	}
}