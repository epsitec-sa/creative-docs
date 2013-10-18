//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class NavigatorController
	{
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

				if (i < this.items.Count-1)
				{
					this.CreateArrow ();
				}
			}
		}


		private void CreateItems(int rank)
		{
			int width = this.GetButtonWidth (this.items[rank]);

			var button = new ColoredButton
			{
				Parent           = this.frameBox,
				Text             = this.items[rank],
				ContentAlignment = ContentAlignment.MiddleCenter,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				AutoFocus        = false,
				Dock             = DockStyle.Left,
				PreferredSize    = new Size (width, NavigatorController.height),
				ActiveState      = rank == this.Selection ? ActiveState.Yes : ActiveState.No,
				NormalColor      = Color.Empty,
				SelectedColor    = ColorManager.EditBackgroundColor,
				HoverColor       = ColorManager.SelectionColor,
			};

			button.Clicked += delegate
			{
				this.OnItemClicked (rank);
			};
		}

		private int GetButtonWidth(string text)
		{
			var width = new TextGeometry (0, 0, 1000, 100, text, Font.DefaultFont, Font.DefaultFontSize, ContentAlignment.MiddleLeft).Width;
			return (int) width + 20;
		}

		private void CreateArrow()
		{
			new GlyphButton
			{
				Parent           = this.frameBox,
				GlyphShape       = GlyphShape.TriangleRight,
				ButtonStyle      = ButtonStyle.ToolItem,
				AutoFocus        = false,
				Dock             = DockStyle.Left,
				PreferredSize    = new Size (NavigatorController.height, NavigatorController.height),
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

		private readonly List<string> items;
		private FrameBox frameBox;
	}
}
