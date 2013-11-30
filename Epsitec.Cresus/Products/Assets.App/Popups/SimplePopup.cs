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
		public SimplePopup()
		{
			this.items = new List<string> ();
			this.SelectedItem = -1;
		}


		public List<string>						Items
		{
			get
			{
				return this.items;
			}
		}

		public int								SelectedItem;


		protected override Size DialogSize
		{
			get
			{
				int dx = SimplePopup.margins*2 + this.RequiredWidth;
				int dy = SimplePopup.margins*2 + this.RequiredHeight;

				return new Size (dx, dy);
			}
		}

		public override void CreateUI()
		{
			int w = this.RequiredWidth;
			int y = SimplePopup.margins + this.RequiredHeight;

			for (int i=0; i<this.items.Count; i++)
			{
				var item = this.items[i];

				if (string.IsNullOrEmpty (item))
				{
					y -= SimplePopup.sepHeight;
					this.CreateSeparator (y, w);
				}
				else
				{
					y -= SimplePopup.itemHeight;
					this.CreateItem (i, y, w);
				}
			}
		}

		private void CreateSeparator(int y, int width)
		{
			y += SimplePopup.sepHeight/2;
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
				return this.items.Max
				(
					item => Helpers.Text.GetTextWidth (SimplePopup.GetTextWithGaps (item))
				)
				+ ColoredButton.HorizontalMargins * 2
				+ 3;  // visuellement, il est bon d'avoir un chouia d'espace en plus à droite
			}
		}

		private string GetTextWithGaps(int rank)
		{
			return SimplePopup.GetTextWithGaps (this.items[rank]);
		}

		private static string GetTextWithGaps(string text)
		{
			return string.Concat (SimplePopup.textGap, text, SimplePopup.textGap);
		}


		private int RequiredHeight
		{
			get
			{
				int sepCount  = this.items.Where (x => string.IsNullOrEmpty (x)).Count ();
				int itemCount = this.items.Count - sepCount;

				return itemCount*SimplePopup.itemHeight + sepCount*SimplePopup.sepHeight;
			}
		}


		#region Events handler
		private void OnItemClicked(int rank)
		{
			this.ItemClicked.Raise (this, rank);
		}

		public event EventHandler<int> ItemClicked;
		#endregion


		private static readonly int				margins		= 5;
		private static readonly int				itemHeight	= 20;
		private static readonly int				sepHeight	= 8;
		private static readonly string			textGap		= "  ";

		private readonly List<string>			items;
	}
}