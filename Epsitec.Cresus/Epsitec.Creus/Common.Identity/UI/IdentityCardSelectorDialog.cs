//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			this.factory = new ItemViewFactory (this);
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

		protected override Window CreateWindow()
		{
			Window dialogWindow = new Window ();

			dialogWindow.Root.Children.Add (this.CreateContents ());

			dialogWindow.Text = "Sélection de l'identité";
			dialogWindow.Name = "Dialog";
			dialogWindow.PreventAutoClose = true;
			dialogWindow.ClientSize = new Size (395, 300);
			dialogWindow.MakeSecondaryWindow ();
			dialogWindow.Root.Padding = new Margins (8, 8, 8, 8);

			ResizeKnob resize = new ResizeKnob (dialogWindow.Root);
			resize.Anchor = AnchorStyles.BottomRight;
			resize.Margins = new Margins (0, -8, 0, -8);
			ToolTip.Default.SetToolTip (resize, "Redimensionner la fenêtre");

			CommandDispatcher.SetDispatcher (dialogWindow, new CommandDispatcher ());

			dialogWindow.WindowCloseClicked += delegate
			{
				this.Result = Dialogs.DialogResult.Cancel;
				this.CloseDialog ();
			};

			dialogWindow.Root.SetFocusOnTabWidget ();
			dialogWindow.AdjustWindowSize ();

			return dialogWindow;
		}

		private Widget CreateContents()
		{
			FrameBox container = new FrameBox ();
			FrameBox buttonContainer = new FrameBox (container);

			container.Dock = DockStyle.Fill;
			container.TabIndex = 1;

			buttonContainer.Dock = DockStyle.Bottom;
			buttonContainer.TabIndex = 2;

			ItemTable table = new ItemTable (container);

			table.Dock = DockStyle.Fill;
			table.MinWidth = 180+18;
			table.MinHeight = 64;
			table.Margins = new Margins (0, 0, 0, 10);
			table.TabIndex = 1;
			table.Items = this.collectionView;
			table.ItemPanel.ItemViewDefaultSize = new Size (180, 48);
			
			//	TODO: use ColumnsOfTiles here, once the code is implemented in ItemPanel
			
			table.ItemPanel.Layout = ItemPanelLayout.RowsOfTiles;
			table.ItemPanel.CustomItemViewFactoryGetter = delegate { return this.factory; };
			table.ItemPanel.CurrentItemTrackingMode = CurrentItemTrackingMode.AutoSelect;
			table.HeaderVisibility = false;
			table.SeparatorVisibility = false;
			table.HorizontalScrollMode = ItemTableScrollMode.None;
#if false
			table.ItemPanel.ApertureChanged +=
				delegate(object sender, DependencyPropertyChangedEventArgs e)
				{
					Rectangle newAperture = (Rectangle) e.NewValue;
					table.ItemPanel.ItemViewDefaultSize = new Size (newAperture.Width, 48);
				};
#endif

			table.Columns.Add (
				new Epsitec.Common.UI.ItemTableColumn (
					IdentityCard.UserNameProperty, 72,
					delegate (object a, object b)
					{
						IdentityCard ca = (IdentityCard) a;
						IdentityCard cb = (IdentityCard) b;

						if ((ca.DeveloperId < 0) &&
							(cb.DeveloperId > -1))
						{
							return -1;
						}
						if ((ca.DeveloperId > -1) &&
							(cb.DeveloperId < 0))
						{
							return 1;
						}

						string nameA = ca.UserName;
						string nameB = cb.UserName;

						nameA = nameA.Substring (nameA.LastIndexOf (' ')+1);
						nameB = nameB.Substring (nameB.LastIndexOf (' ')+1);

						return nameA.CompareTo (nameB);
					}));

			table.ColumnHeader.SetColumnSort (0, ListSortDirection.Ascending);
					
			Button button;
			
			button = new Button (buttonContainer);
			button.PreferredHeight = 20;
			button.FormattedText = Widgets.Res.Strings.Dialog.Button.Cancel;
			button.Dock = DockStyle.Right;
			button.Margins = new Drawing.Margins (0, 0, 0, 0);
			button.TabIndex = 2;
			button.Shortcuts.Add (Common.Widgets.Feel.Factory.Active.CancelShortcut);
			button.Clicked +=
				delegate
				{
					this.Result = Dialogs.DialogResult.Cancel;
					this.CloseDialog ();
				};

			button = new Button (buttonContainer);
			button.PreferredHeight = 20;
			button.FormattedText = Widgets.Res.Strings.Dialog.Button.OK;
			button.Dock = DockStyle.Right;
			button.Margins = new Drawing.Margins (0, 10, 0, 0);
			button.TabIndex = 1;
			button.Shortcuts.Add (Common.Widgets.Feel.Factory.Active.AcceptShortcut);
			button.Clicked +=
				delegate
				{
					this.Result = Dialogs.DialogResult.Accept;
					this.CloseDialog ();
				};
			
			return container;
		}

		private class ItemViewFactory : IItemViewFactory
		{
			public ItemViewFactory(IdentityCardSelectorDialog dialog)
			{
				this.dialog = dialog;
			}
			
			#region IItemViewFactory Members

			public ItemViewWidget CreateUserInterface(ItemView itemView)
			{
				ItemViewWidget container = new ItemViewWidget (itemView);
				IdentityCard card = itemView.Item as IdentityCard;
				IdentityCardWidget widget = new IdentityCardWidget (container);

				container.IsPassive = true;

				widget.AutoDoubleClick = true;
				widget.Dock = DockStyle.Fill;
				widget.IdentityCard = card;
				widget.Clicked +=
					delegate
					{
						itemView.Owner.SelectItemView (itemView);
					};
				widget.DoubleClicked +=
					delegate
					{
						this.dialog.Result = Dialogs.DialogResult.Accept;
						this.dialog.CloseDialog ();
					};

				container.AddEventHandler (Visual.SelectedProperty,
					delegate (object sender, DependencyPropertyChangedEventArgs e)
					{
						widget.SetSelected ((bool) e.NewValue);
					});

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

			private IdentityCardSelectorDialog dialog;
		}

		private CollectionView collectionView;
		private ItemViewFactory factory;
	}
}
