//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class SimplePopup : AbstractPopup
	{
		public SimplePopup()
		{
			this.items = new List<string> ();
			this.Selected = -1;
		}


		public List<string>						Items
		{
			get
			{
				return this.items;
			}
		}

		public int								Selected;


		protected override Size DialogSize
		{
			get
			{
				int dx = SimplePopup.margins*2 + this.RequiredWidth;
				int dy = SimplePopup.margins*2 + SimplePopup.itemHeight*this.items.Count;

				return new Size (dx, dy);
			}
		}

		protected override void CreateUI()
		{
			int w = this.RequiredWidth;

			for (int i=0; i<this.items.Count; i++)
			{
				this.CreateItem (i, w);
			}
		}

		private void CreateItem(int rank, int width)
		{
			int i = this.items.Count - rank - 1;

			int x = SimplePopup.margins;
			int y = SimplePopup.margins + SimplePopup.itemHeight*i;
			int dx = width;
			int dy = SimplePopup.itemHeight;

			var button = this.CreateItem (x, y, dx, dy, this.GetText (rank));

			if (rank == this.Selected)
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
			get
			{
				int width = 0;

				for (int i=0; i<this.items.Count; i++)
				{
					int bw = SimplePopup.GetButtonWidth (this.GetText (i));
					width = System.Math.Max (bw, width);
				}

				return width;
			}
		}

		private static int GetButtonWidth(string text)
		{
			var width = new TextGeometry (0, 0, 1000, 100, text, Font.DefaultFont, Font.DefaultFontSize, ContentAlignment.MiddleLeft).Width;
			return (int) width + 10;
		}

		private string GetText(int rank)
		{
			return "  " + this.items[rank];
		}


		#region Events handler
		private void OnItemClicked(int rank)
		{
			if (this.ItemClicked != null)
			{
				this.ItemClicked (this, rank);
			}
		}

		public delegate void ItemClickedEventHandler(object sender, int rank);
		public event ItemClickedEventHandler ItemClicked;
		#endregion


		private static readonly int margins    = 1;
		private static readonly int itemHeight = 20;

		private readonly List<string> items;
	}
}