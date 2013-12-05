//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class NavigatorController
	{
		//	Le rang inclu l'onglet/bouton ainsi que la flèche/triangle
		//	sur la droite.
		//	
		//	    0         1         2
		//	<-------> <-------> <-------> 
		//	
		//	+-----+   +-----+   +-----+
		//	|     | > |     | > |     | >
		//	+-----+   +-----+   +-----+

		public NavigatorController()
		{
			this.items = new List<string> ();
		}


		public List<string>						Items
		{
			get
			{
				return this.items;
			}
		}

		public bool								HasLastArrow;
		public int								Selection;


		public void CreateUI(Widget parent)
		{
			this.frameBox = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = NavigatorController.height,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};
		}

		public void UpdateUI()
		{
			this.frameBox.Children.Clear ();

			for (int i=0; i<this.items.Count; i++)
			{
				this.CreateItems (i);

				if (i < this.items.Count-1 || this.HasLastArrow)
				{
					this.CreateArrow (i);
				}
			}
		}


		private void CreateItems(int rank)
		{
			int width = Text.GetTextWidth (this.items[rank]) + 20;

			var button = new ColoredButton
			{
				Parent           = this.frameBox,
				Text             = this.items[rank],
				ContentAlignment = ContentAlignment.MiddleCenter,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				AutoFocus        = false,
				Dock             = DockStyle.Left,
				PreferredSize    = new Size (width, NavigatorController.height),
				ActiveState      = Misc.GetActiveState (rank == this.Selection),
				NormalColor      = Color.Empty,
				SelectedColor    = ColorManager.EditBackgroundColor,
				HoverColor       = ColorManager.HoverColor,
			};

			button.Clicked += delegate
			{
				this.OnItemClicked (rank);
			};
		}

		private void CreateArrow(int rank)
		{
			var button = new GlyphButton
			{
				Parent           = this.frameBox,
				GlyphShape       = GlyphShape.TriangleRight,
				ButtonStyle      = ButtonStyle.ToolItem,
				AutoFocus        = false,
				Dock             = DockStyle.Left,
				PreferredSize    = new Size (NavigatorController.height, NavigatorController.height),
			};

			button.Clicked += delegate
			{
				this.OnArrowClicked (button, rank);
			};
		}


		#region Events handler
		private void OnItemClicked(int rank)
		{
			this.ItemClicked.Raise (this, rank);
		}

		public event EventHandler<int> ItemClicked;


		private void OnArrowClicked(Widget button, int rank)
		{
			this.ArrowClicked.Raise (this, button, rank);
		}

		public event EventHandler<Widget, int> ArrowClicked;
		#endregion


		private const int height = AbstractCommandToolbar.secondaryToolbarHeight;

		private readonly List<string>			items;
		private FrameBox						frameBox;
	}
}
