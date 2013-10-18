﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				+ 5;
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
		private static readonly string textGap = "    ";

		private readonly List<string> items;
	}
}