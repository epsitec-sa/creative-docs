//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup tout simple contenant juste une liste de textes. Il peut s'utiliser
	/// comme un menu-combo. La largeur s'adapte en fonction des textes contenus.
	/// </summary>
	public class SimplePopup : AbstractPopup
	{
		private SimplePopup()
		{
			this.items = new List<string> ();
			this.SelectedItem = -1;
		}


		private string                          Title;

		private List<string>					Items
		{
			get
			{
				return this.items;
			}
		}

		private int								SelectedItem;


		protected override Size					DialogSize
		{
			get
			{
				int dx = SimplePopup.margins*2 + this.RequiredWidth;
				int dy = SimplePopup.margins*2 + this.RequiredHeight;

				return new Size (dx, dy);
			}
		}

		protected override void CreateUI()
		{
			int h = SimplePopup.margins + this.RequiredHeight;
			int w = this.RequiredWidth;

			if (!string.IsNullOrEmpty (this.Title))  // y a-t-il un titre ?
			{
				h -= SimplePopup.itemHeight;
				this.CreateTitle (h, w);

				h -= SimplePopup.sepHeight;
				this.CreateSeparator (h, w);
			}

			int i = 0;
			foreach (int y in this.PosY)
			{
				var item = this.items[i];

				if (string.IsNullOrEmpty (item))
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

		private void CreateTitle(int y, int width)
		{
			int x = ColoredButton.horizontalMargins + SimplePopup.margins;
			int dx = width;
			int dy = SimplePopup.itemHeight;

			new StaticText
			{
				Parent        = this.mainFrameBox,
				Anchor        = AnchorStyles.BottomLeft,
				PreferredSize = new Size (dx, dy),
				Margins       = new Margins (x, 0, 0, y),
				Text          = this.GetTextWithGaps (-1),
			};
		}

		private void CreateSeparator(int y, int width)
		{
			int dx = width + SimplePopup.margins*2;

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
			int x = SimplePopup.margins;
			int dx = width;
			int dy = SimplePopup.itemHeight;

			var button = this.CreateItem (x, y, dx, dy, this.GetTextWithGaps (rank));

			if (rank == this.SelectedItem)
			{
				button.ActiveState = ActiveState.Yes;
			}

			button.Clicked += delegate
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
				return System.Math.Max (this.GetTextWithGaps (-1).GetTextWidth (), this.TextsWidth)
					+ ColoredButton.horizontalMargins * 2
					+ 3;  // visuellement, il est bon d'avoir un chouia d'espace en plus à droite
			}
		}

		private int TextsWidth
		{
			get
			{
				return this.items.Max (item => SimplePopup.GetTextWithGaps (item).GetTextWidth ());
			}
		}

		private string GetTextWithGaps(int rank)
		{
			if (rank == -1)
			{
				return SimplePopup.GetTextWithGaps (this.Title);
			}
			else
			{
				return SimplePopup.GetTextWithGaps (this.items[rank]);
			}
		}

		private static string GetTextWithGaps(string text)
		{
			return string.Concat (SimplePopup.textGap, text, SimplePopup.textGap);
		}


		private int RequiredHeight
		{
			get
			{
				return this.TitleHeight - this.PosY.Last ();
			}
		}

		private int TitleHeight
		{
		get
			{
				if (string.IsNullOrEmpty (this.Title))
				{
					return 0;
				}
				else
				{
					return SimplePopup.itemHeight + SimplePopup.sepHeight;
				}
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

					if (string.IsNullOrEmpty (item))
					{
						if (separator)  // compact ?
						{
							y += SimplePopup.sepHeight/2 - 2;
							yield return y;
							y -= SimplePopup.sepHeight/2;
						}
						else
						{
							y -= SimplePopup.sepHeight/2;
							yield return y;
							y -= SimplePopup.sepHeight/2;
						}

						separator = true;
					}
					else
					{
						y -= SimplePopup.itemHeight;
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

		private event EventHandler<int> ItemClicked;
		#endregion


		#region Helpers
		public static void Show(Widget target, IEnumerable<string> items, int selectedItem, string title, System.Action<int> action, bool leftOrRight = true)
		{
			//	Affiche le Popup.
			var popup = new SimplePopup ()
			{
				SelectedItem = selectedItem,
				Title        = title,
			};

			foreach (var item in items)
			{
				popup.Items.Add (item);
			}

			popup.Create (target, leftOrRight);

			popup.ItemClicked += delegate (object sender, int rank)
			{
				action (rank);
			};
		}
		#endregion


		private const int						margins		= 2;
		private const int						itemHeight	= 20;
		private const int						sepHeight	= 8;
		private const string					textGap		= "  ";

		private readonly List<string>			items;
	}
}