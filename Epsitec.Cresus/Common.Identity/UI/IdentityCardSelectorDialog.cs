//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Identity;
using Epsitec.Common.Identity.UI;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.Identity.UI
{
	public class IdentityCardSelectorDialog : Dialogs.AbstractMessageDialog
	{
		public IdentityCardSelectorDialog()
			: this (IdentityRepository.Default.IdentityCards)
		{
		}

		public IdentityCardSelectorDialog(IList<IdentityCard> identities)
		{
			this.collectionView = new CollectionView (identities as System.Collections.IList);
			this.factory = new ItemViewFactory ();
		}

		public IdentityCard ActiveIdentityCard
		{
			get
			{
				return this.collectionView.CurrentItem as IdentityCard;
			}
			set
			{
				this.collectionView.MoveCurrentTo (value);
			}
		}

		protected override void CreateWindow()
		{
			this.window = new Window ();

			this.window.Root.Children.Add (this.CreateContents ());

			this.window.Text = "Sélection de l'identité";
			this.window.Name = "Dialog";
			this.window.PreventAutoClose = true;
			this.window.ClientSize = new Size (100, 320);
			this.window.MakeSecondaryWindow ();
			this.window.Root.MinSize = new Size (100, 96);
			this.window.Root.Padding = new Margins (8, 8, 8, 8);

			ResizeKnob resize = new ResizeKnob( this.window.Root);
			resize.Anchor = AnchorStyles.BottomRight;
			resize.Margins = new Margins (0, -8, 0, -8);
			ToolTip.Default.SetToolTip (resize, "Redimensionner la fenêtre");

			CommandDispatcher.SetDispatcher (this.window, new CommandDispatcher ());

			this.window.WindowCloseClicked += delegate
			{
				this.result = Dialogs.DialogResult.Cancel;
				this.CloseDialog ();
			};

			this.window.Root.SetFocusOnTabWidget ();
			this.window.AdjustWindowSize ();
		}

		private Widget CreateContents()
		{
			FrameBox container = new FrameBox ();
			FrameBox buttonContainer = new FrameBox (container);

			container.Dock = DockStyle.Fill;
			buttonContainer.Dock = DockStyle.Bottom;

			ItemTable table = new ItemTable (container);

			table.Dock = DockStyle.Fill;
			table.Margins = new Margins (0, 0, 0, 10);
			table.Items = this.collectionView;
			table.ItemPanel.ItemViewDefaultSize = new Size (100, 48);
			table.ItemPanel.CustomItemViewFactoryGetter = delegate { return this.factory; };
			table.ItemPanel.CurrentItemTrackingMode = CurrentItemTrackingMode.AutoSelect;
			table.HeaderVisibility = false;
			table.SeparatorVisibility = false;
			table.HorizontalScrollMode = ItemTableScrollMode.None;
			table.ItemPanel.ApertureChanged +=
				delegate(object sender, DependencyPropertyChangedEventArgs e)
				{
					Rectangle newAperture = (Rectangle) e.NewValue;
					table.ItemPanel.ItemViewDefaultSize = new Size (newAperture.Width, 48);
				};
			
			table.Columns.Add (new Epsitec.Common.UI.ItemTableColumn ("UserName", 72));
			table.ColumnHeader.SetColumnComparer (0,
				delegate (object a, object b)
				{
					IdentityCard ca = (IdentityCard) a;
					IdentityCard cb = (IdentityCard) b;

					string nameA = ca.UserName;
					string nameB = cb.UserName;

					nameA = nameA.Substring (nameA.LastIndexOf (' ')+1);
					nameB = nameB.Substring (nameB.LastIndexOf (' ')+1);

					return nameA.CompareTo (nameB);
				});
					
			Button button;
			
			button = new Button (buttonContainer);
			button.PreferredHeight = 20;
			button.Text = Widgets.Res.Strings.Dialog.Button.OK;
			button.Dock = DockStyle.Left;
			button.Margins = new Drawing.Margins (0, 10, 0, 0);
			button.TabIndex = 1;
			button.Shortcuts.Add (Common.Widgets.Feel.Factory.Active.AcceptShortcut);
			button.Clicked +=
				delegate
				{
					this.result = Dialogs.DialogResult.Accept;
					this.CloseDialog ();
				};
			
			button = new Button (buttonContainer);
			button.PreferredHeight = 20;
			button.Text = Widgets.Res.Strings.Dialog.Button.Cancel;
			button.Dock = DockStyle.Left;
			button.Margins = new Drawing.Margins (0, 0, 0, 0);
			button.TabIndex = 2;
			button.Shortcuts.Add (Common.Widgets.Feel.Factory.Active.CancelShortcut);
			button.Clicked +=
				delegate
				{
					this.result = Dialogs.DialogResult.Cancel;
					this.CloseDialog ();
				};

			return container;
		}

		private class ItemViewFactory : IItemViewFactory
		{
			#region IItemViewFactory Members

			public ItemViewWidget CreateUserInterface(ItemView itemView)
			{
				ItemViewWidget container = new ItemViewWidget (itemView);
				IdentityCard card = itemView.Item as IdentityCard;
				IdentityCardWidget widget = new IdentityCardWidget (container);

				widget.Dock = DockStyle.Fill;
				widget.IdentityCard = card;
				widget.Clicked +=
					delegate
					{
						itemView.Owner.SelectItemView (itemView);
					};

				return container;
			}

			public void DisposeUserInterface(ItemViewWidget widget)
			{
				widget.Dispose ();
			}

			public Size GetPreferredSize(ItemView itemView)
			{
				return itemView.Owner.ItemViewDefaultSize;
			}

			#endregion
		}

		private CollectionView collectionView;
		private ItemViewFactory factory;
	}
}
