//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	/// <summary>
	/// Affiche des boutons ayant l'apparence d'onglets dans une zone horizontale, qui
	/// permettent de choisir le panneau sélectionné en dessous.
	/// </summary>
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
			int width = this.items[rank].GetTextWidth () + 20;

			var button = new ColoredButton
			{
				Parent           = this.frameBox,
				Text             = this.items[rank],
				ContentAlignment = ContentAlignment.MiddleCenter,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				AutoFocus        = false,
				Dock             = DockStyle.Left,
				PreferredSize    = new Size (width, TabPagesController.height),
				ActiveState      = Misc.GetActiveState (rank == this.Selection),
				NormalColor      = Color.Empty,
				SelectedColor    = ColorManager.EditBackgroundColor,
				HoverColor       = ColorManager.HoverColor,
			};

			button.Clicked += delegate
			{
				this.Selection = rank;
				this.OnItemClicked (rank);
			};
		}


		#region Events handler
		private void OnItemClicked(int rank)
		{
			this.ItemClicked.Raise (this, rank);
		}

		public event EventHandler<int> ItemClicked;
		#endregion


		private const int height = AbstractCommandToolbar.secondaryToolbarHeight;

		private readonly List<string>			items;
		private FrameBox						frameBox;
		private int								selection;
	}
}
