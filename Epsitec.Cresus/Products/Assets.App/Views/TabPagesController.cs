//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class TabPagesController
	{
		public TabPagesController()
		{
			this.items = new List<string> ();
			this.selection = -1;
		}


		public List<string>						Items
		{
			get
			{
				return this.items;
			}
		}

		public int								Selection
		{
			get
			{
				return this.selection;
			}
			set
			{
				if (this.selection != value)
				{
					this.selection = value;
					this.UpdateUI ();
				}
			}
		}


		public void CreateUI(Widget parent)
		{
			this.frameBox = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = TabPagesController.height,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};
		}

		public void UpdateUI()
		{
			this.frameBox.Children.Clear ();

			for (int i=0; i<this.items.Count; i++)
			{
				this.CreateItems (i);
			}
		}


		private void CreateItems(int rank)
		{
			int width = Helpers.Text.GetTextWidth (this.items[rank]) + 20;

			var button = new ColoredButton
			{
				Parent           = this.frameBox,
				Text             = this.items[rank],
				ContentAlignment = ContentAlignment.MiddleCenter,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				AutoFocus        = false,
				Dock             = DockStyle.Left,
				PreferredSize    = new Size (width, TabPagesController.height),
				ActiveState      = rank == this.Selection ? ActiveState.Yes : ActiveState.No,
				NormalColor      = Color.Empty,
				SelectedColor    = ColorManager.EditBackgroundColor,
				HoverColor       = ColorManager.HoverColor,
			};

			button.Clicked += delegate
			{
				//?this.Selection = rank;
				this.OnItemClicked (rank);
			};
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


		private static readonly int height = 26;

		private readonly List<string>			items;
		private FrameBox						frameBox;
		private int								selection;
	}
}
