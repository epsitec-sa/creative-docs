//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Ce dialogue permet d'éditer une enumération avec un support complet de tous les modes de cardinalité.
	/// Il supporte un très grand nombre d'éléments dans l'énumération. En effet, seul les widgets correspondants
	/// aux lignes visibles sont créés, et non l'ensemble. De plus, un filtre rend l'utilisation agréable.
	/// </summary>
	public class ItemPickerDialog<T> : CoreDialog
			where T : AbstractEntity, new ()
	{
		public ItemPickerDialog(CoreApp application, SelectionController<T> controller, EnumValueCardinality cardinality, string title)
			: base (application)
		{
			this.controller  = controller;
			this.cardinality = cardinality;
			this.title       = title;

			this.items = this.controller.GetPossibleItems ().ToList ();
			this.filteredItems = new List<FiltererItem> ();
			this.containers = new List<FrameBox> ();
			this.selectedIndexes = new List<int> ();

			this.UpdateFilter ();
			this.UpdateSelectedIndexes ();
		}


		protected override void SetupWindow(Window window)
		{
			window.Text = this.title;
			window.ClientSize = new Size (600, 400);
		}

		protected override void SetupWidgets(Window window)
		{
			var adorner = Common.Widgets.Adorners.Factory.Active;

			//	Crée tous les conteneurs.
			var middleFrame = new FrameBox
			{
				Parent              = window.Root,
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.HorizontalFlow,
				Dock                = DockStyle.Fill,
				Margins             = new Margins (10, 10, 10, 0),
				TabIndex            = ++this.tabIndex,
			};

			var leftFrame = new FrameBox
			{
				Parent    = middleFrame,
				Dock      = DockStyle.Fill,
				Margins   = new Margins (0, 10, 0, 10),
				TabIndex  = ++this.tabIndex,
			};

			var rightFrame = new FrameBox
			{
				Parent    = middleFrame,
				Dock      = DockStyle.Fill,
				Margins   = new Margins (10, 0, 0, 10),
				TabIndex  = ++this.tabIndex,
			};

			var filterFrame = new FrameBox
			{
				Parent    = leftFrame,
				Dock      = DockStyle.Top,
				Margins   = new Margins (0, 0, 0, 10),
				TabIndex  = ++this.tabIndex,
			};

			var mainFrame = new FrameBox
			{
				Parent    = leftFrame,
				BackColor = adorner.ColorTextBackground,
				Dock      = DockStyle.Fill,
				TabIndex  = ++this.tabIndex,
			};

			var actionsFrame = new FrameBox
			{
				Parent          = leftFrame,
				PreferredHeight = 20,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (0, 0, 10, 0),
				TabIndex        = ++this.tabIndex,
			};

			var footerFrame = new FrameBox
			{
				Parent          = window.Root,
				PreferredHeight = 20,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (10, 10, 10, 10),
				TabIndex        = ++this.tabIndex,
			};

			new Separator
			{
				Parent          = window.Root,
				PreferredHeight = 1,
				Dock            = DockStyle.Bottom,
			};

			//	Crée la marque '>' flottante.
			this.floatingArrowMark = new Widgets.StaticGlyph
			{
				Parent        = middleFrame,
				GlyphShape    = GlyphShape.TriangleRight,
				PreferredSize = new Size (36, 36),
				Anchor        = AnchorStyles.TopLeft,
				Visibility    = false,
			};

			//	Crée les interfaces.
			this.CreateFilterUI  (filterFrame);
			this.CreateMainUI    (mainFrame);
			this.CreatePreviewUI (rightFrame);
			this.CreateActionsUI (actionsFrame);
			this.CreateFooterUI  (footerFrame);

			this.filterField.Focus ();
		}

		private void CreateFilterUI(Widget parent)
		{
			parent.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			new StaticText
			{
				Parent         = parent,
				Text           = "Filtre",
				PreferredWidth = 32,
				Dock           = DockStyle.Left,
			};

			this.filterField = new TextFieldEx
			{
				Parent   = parent,
				Dock     = DockStyle.Fill,
				TabIndex = ++this.tabIndex,
			};

			this.filterClearButton = new GlyphButton
			{
				Parent     = parent,
				GlyphShape = GlyphShape.Minus,
				Enable     = false,
				Margins    = new Margins (1, 0, 0, 0),
				Dock       = DockStyle.Right,
				TabIndex   = ++this.tabIndex,
			};

			ToolTip.Default.SetToolTip (this.filterClearButton, "Tout montrer");

			this.filterField.EditionAccepted += delegate
			{
				this.UpdateAfterFilterChanged ();
			};

			this.filterClearButton.Clicked += delegate
			{
				this.filterField.Text = null;
				this.UpdateAfterFilterChanged ();
			};
		}

		private void UpdateAfterFilterChanged()
		{
			this.firstRow = 0;

			this.UpdateFilter ();
			this.UpdateRows ();
			this.UpdateScroller ();
			this.UpdateActions ();

			bool hasFilter = this.HasFilter;
			this.filterClearButton.Enable = hasFilter;
			this.selectAllButton.Margins = new Margins (5, hasFilter ? 5:0, 0, 0);
			this.selectFilterButton.Visibility = hasFilter;
		}

		private void CreateMainUI(Widget parent)
		{
			var containersFrame = new FrameBox
			{
				Parent        = parent,
				DrawFullFrame = true,
				Dock          = DockStyle.Fill,
			};

			this.scroller = new VScroller
			{
				Parent     = parent,
				IsInverted = true,
				Margins    = new Margins (1, 0, 0, 0),
				Dock       = DockStyle.Right,
			};

			this.scroller.ValueChanged += delegate
			{
				this.FirstRow = (int) (this.scroller.DoubleValue + 0.5);
			};

			containersFrame.SizeChanged += delegate
			{
				this.UpdateContainers (containersFrame);
				this.UpdateRows ();
				this.UpdateScroller ();
			};
		}

		private void CreatePreviewUI(Widget parent)
		{
			new StaticText
			{
				Parent  = parent,
				Text    = "Détails :",
				Dock    = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
			};

			this.previewFrame = new FrameBox
			{
				Parent        = parent,
				DrawFullFrame = true,
				Dock          = DockStyle.Fill,
				Padding       = new Margins (5),
			};
		}

		private void CreateActionsUI(Widget parent)
		{
			parent.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			
			this.deselectButton = new Button
			{
				Parent     = parent,
				Text       = "Aucun",
				Dock       = DockStyle.Fill,
				Margins    = new Margins (0, 5, 0, 0),
				TabIndex   = ++this.tabIndex,
			};

			this.selectAllButton = new Button
			{
				Parent     = parent,
				Text       = "Tout",
				Dock       = DockStyle.Fill,
				Margins    = new Margins (5, 0, 0, 0),
				TabIndex   = ++this.tabIndex,
			};

			this.selectFilterButton = new Button
			{
				Parent     = parent,
				Text       = "Selon le filtre",
				Dock       = DockStyle.Fill,
				Margins    = new Margins (5, 0, 0, 0),
				Visibility = false,
				TabIndex   = ++this.tabIndex,
			};

			ToolTip.Default.SetToolTip (this.deselectButton,     "Désélectionne tout");
			ToolTip.Default.SetToolTip (this.selectAllButton,    "Sélectionne tout");
			ToolTip.Default.SetToolTip (this.selectFilterButton, "Sélectionne selon le filtre");

			this.deselectButton.Clicked += delegate
			{
				this.selectedIndexes.Clear ();

				this.UpdateRows ();
				this.UpdateActions ();
			};

			this.selectAllButton.Clicked += delegate
			{
				this.selectedIndexes.Clear ();

				for (int i = 0; i < this.items.Count; i++)
				{
					this.selectedIndexes.Add (i);
				}

				this.UpdateRows ();
				this.UpdateActions ();
			};

			this.selectFilterButton.Clicked += delegate
			{
				this.selectedIndexes.Clear ();

				for (int i = 0; i < this.filteredItems.Count; i++)
				{
					this.selectedIndexes.Add (this.filteredItems[i].Index);
				}

				this.UpdateRows ();
				this.UpdateActions ();
			};
		}

		private void CreateFooterUI(Widget parent)
		{
			this.cancelButton = new Button ()
			{
				CommandObject = Epsitec.Common.Dialogs.Res.Commands.Dialog.Generic.Cancel,
				Parent        = parent,
				ButtonStyle   = ButtonStyle.DefaultCancel,
				Dock          = DockStyle.Right,
				Margins       = new Margins (10, 0, 0, 0),
				TabIndex      = 101,
			};

			this.acceptButton = new Button ()
			{
				CommandObject = Epsitec.Common.Dialogs.Res.Commands.Dialog.Generic.Ok,
				Parent        = parent,
				ButtonStyle   = ButtonStyle.DefaultAccept,
				Dock          = DockStyle.Right,
				TabIndex      = 100,
			};
		}

		private void UpdateActions()
		{
			this.deselectButton.Enable = this.selectedIndexes.Any ();
			this.selectAllButton.Enable = (this.filteredItems.Count > 0 && this.selectedIndexes.Count != this.items.Count);

			bool filterEnable = false;

			if (this.filteredItems.Count == 0)
			{
			}
			else if (this.filteredItems.Count == this.selectedIndexes.Count)
			{
				for (int i = 0; i < this.filteredItems.Count; i++)
				{
					int index = this.filteredItems[i].Index;

					if (!this.selectedIndexes.Contains (index))
					{
						filterEnable = true;
						break;
					}
				}
			}
			else
			{
				filterEnable = true;
			}

			this.selectFilterButton.Enable = filterEnable;
		}

		private void UpdateContainers(Widget parent)
		{
			var bounds = parent.ActualBounds;
			bounds.Deflate (2);

			double lineHeight = ItemPickerDialog<T>.lineHeight;
			int linesRequired = (int) (bounds.Height / lineHeight) + 1;

			//	Crée le containers.
			if (this.containers.Count != linesRequired)
			{
				this.containers.Clear ();
				parent.Children.Clear ();

				for (int i = 0; i < linesRequired; i++)
				{
					var container = new FrameBox
					{
						Parent          = parent,
						PreferredHeight = lineHeight,
					};

					container.Clicked += delegate
					{
						this.ContainerClicked (container.Index);
					};

					container.Entered += delegate
					{
						this.ContainerHilite (container, true);
						this.UpdatePreview (container.Index);
					};

					container.Exited += delegate
					{
						this.ContainerHilite (container, false);
						this.UpdatePreview (-1);
					};

					this.containers.Add (container);
				}
			}

			//	Positionne les containers.
			var rect = new Rectangle (bounds.Left, bounds.Top-lineHeight, bounds.Width, lineHeight);

			foreach (var container in this.containers)
			{
				container.SetManualBounds (rect);
				rect.Offset (0, -lineHeight);
			}
		}

		private void ContainerClicked(int index)
		{
			if (index == -1)
			{
				return;
			}

			if (this.cardinality == EnumValueCardinality.Any)
			{
				if (this.selectedIndexes.Contains (index))
				{
					this.selectedIndexes.Remove (index);
				}
				else
				{
					this.selectedIndexes.Add (index);
				}
			}
			else if (this.cardinality == EnumValueCardinality.ZeroOrOne)
			{
				if (index == 0)
				{
					this.selectedIndexes.Clear ();
				}
				else
				{
					index--;

					if (this.selectedIndexes.Contains (index))
					{
						this.selectedIndexes.Remove (index);
					}
					else
					{
						this.selectedIndexes.Clear ();
						this.selectedIndexes.Add (index);
					}
				}
			}
			else if (this.cardinality == EnumValueCardinality.AtLeastOne)
			{
				if (this.selectedIndexes.Contains (index))
				{
					if (this.selectedIndexes.Count > 1)
					{
						this.selectedIndexes.Remove (index);
					}
				}
				else
				{
					this.selectedIndexes.Add (index);
				}
			}
			else if (this.cardinality == EnumValueCardinality.ExactlyOne)
			{
				this.selectedIndexes.Clear ();
				this.selectedIndexes .Add (index);
			}

			this.UpdateRows ();
			this.UpdateActions ();
		}

		private void ContainerHilite(FrameBox container, bool entered)
		{
			var color = Color.Empty;

			if (entered && container.Index != -1)
			{
				color = Color.FromBrightness (0.9);
			}

			container.BackColor = color;

			this.UpdateFloatingArrowMark (container, entered);
		}

		private void UpdateFloatingArrowMark(FrameBox container, bool entered)
		{
			if (entered && container.Index != -1)
			{
				var frame = this.floatingArrowMark.Parent;
				double x = frame.ActualWidth/2 - this.floatingArrowMark.ActualWidth/2 - 1;
				double y = container.Parent.ActualHeight - container.ActualLocation.Y + 4;

				this.floatingArrowMark.Margins = new Margins (x, 0, y, 0);
				this.floatingArrowMark.Visibility = true;
			}
			else
			{
				this.floatingArrowMark.Visibility = false;
			}
		}

		private void UpdatePreview(int index)
		{
			this.previewFrame.Children.Clear ();

			if (index != -1)
			{
				var entity = this.items[index];
				var content = Helpers.EntityPreviewHelper.CreateSummaryUI (entity, this.previewFrame, this.controller.CoreData);
				content.Dock = DockStyle.Fill;
			}
		}

		private void UpdateRows()
		{
			for (int i = 0; i < this.containers.Count; i++)
			{
				this.containers[i].Children.Clear ();

				int ii = this.firstRow+i;

				if (ii < this.filteredItems.Count)
				{
					var filteredItem = this.filteredItems[ii];

					var icon = this.GetIconTag (this.selectedIndexes.Contains (filteredItem.Index));
					var text = this.controller.ConvertHintValueToDescription (filteredItem.Item);

					new StaticText
					{
						Parent        = this.containers[i],
						FormattedText = FormattedText.Concat (icon, " ", text),
						TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
						Dock          = DockStyle.Fill,
					};

					this.containers[i].Index = filteredItem.Index;
				}
				else
				{
					this.containers[i].Index = -1;
				}
			}
		}

		private void UpdateScroller()
		{
			//	Met à jour l'ascenseur en fonction de la liste.
			int total = this.filteredItems.Count;
			int visibleRows = this.containers.Count - 1;

			if (total <= visibleRows)
			{
				if (this.scroller.Visibility)
				{
					this.scroller.Hide ();
				}
			}
			else
			{
				this.scroller.MaxValue          = (decimal) (total-visibleRows);
				this.scroller.VisibleRangeRatio = (decimal) ((double) visibleRows/total);
				this.scroller.Value             = (decimal) (this.firstRow);
				this.scroller.SmallChange       = 1;
				this.scroller.LargeChange       = (decimal) (visibleRows/2.0);

				if (!this.scroller.Visibility)
				{
					this.scroller.Show ();
				}
			}
		}

		private int FirstRow
		{
			//	Première ligne visible.
			get
			{
				return this.firstRow;
			}

			set
			{
				value = System.Math.Max (value, 0);
				value = System.Math.Min (value, System.Math.Max (this.filteredItems.Count-this.containers.Count+1, 0));

				if (value != this.firstRow)
				{
					this.firstRow = value;
					this.UpdateRows ();
				}
			}
		}

		private string GetIconTag(bool isSelected)
		{
			return Misc.GetResourceIconImageTag (this.GetIcon (isSelected), -4);
		}

		private string GetIcon(bool isSelected)
		{
			if (this.cardinality == EnumValueCardinality.ExactlyOne ||
				this.cardinality == EnumValueCardinality.ZeroOrOne)
			{
				if (isSelected)
				{
					return "Button.RadioYes";
				}
				else
				{
					return "Button.RadioNo";
				}
			}
			else
			{
				if (isSelected)
				{
					return "Button.CheckYes";
				}
				else
				{
					return "Button.CheckNo";
				}
			}
		}


		[Command (Epsitec.Common.Dialogs.Res.CommandIds.Dialog.Generic.Cancel)]
		private void ExecuteCancelCommand()
		{
			if (this.cancelButton.Enable)
			{
				this.CloseAndRejectChanges ();
			}
		}

		[Command (Epsitec.Common.Dialogs.Res.CommandIds.Dialog.Generic.Ok)]
		private void ExecuteOkCommand()
		{
			if (this.acceptButton.Enable)
			{
				this.CloseAndAcceptChanges ();
			}
		}

		private void CloseAndAcceptChanges()
		{
			this.UpdateSelection ();

			this.Result = DialogResult.Accept;
			this.CloseDialog ();
		}

		private void CloseAndRejectChanges()
		{
			this.Result = DialogResult.Cancel;
			this.CloseDialog ();
		}


		#region Filter engine
		private void UpdateFilter()
		{
			this.filteredItems.Clear ();

			if (this.HasFilter)
			{
				string filter = this.filterField.Text.ToLower ();

				for (int i = 0; i < this.items.Count; i++)
				{
					var item = this.items.ElementAt (i);

					var text = this.controller.ConvertHintValueToDescription (item).ToSimpleText ().ToLower ();

					if (text.Contains (filter))
					{
						this.filteredItems.Add (new FiltererItem (item, i));
					}
				}
			}
			else
			{
				for (int i = 0; i < this.items.Count; i++)
				{
					var item = this.items.ElementAt (i);

					this.filteredItems.Add (new FiltererItem (item, i));
				}
			}
		}

		private bool HasFilter
		{
			get
			{
				return this.filterField != null && !string.IsNullOrEmpty (this.filterField.Text);
			}
		}

		private class FiltererItem
		{
			public FiltererItem(T item, int index)
			{
				this.Item  = item;
				this.Index = index;
			}

			public T Item
			{
				get;
				private set;
			}

			public int Index
			{
				get;
				private set;
			}
		}
		#endregion


		private void UpdateSelection()
		{
			var selectedItems = this.controller.CollectionValueGetter ();

			selectedItems.Clear ();

			foreach (var index in this.selectedIndexes)
			{
				var item = this.items.ElementAt (index);
				selectedItems.Add (item);
			}

#if false
			using (selectedItems.SuspendNotifications ())
			{
				selectedItems.Clear ();
				selectedItems.AddRange (newSelection);
			}
#endif
		}

		private void UpdateSelectedIndexes()
		{
			this.selectedIndexes.Clear ();

			var selectedItems = this.controller.CollectionValueGetter ();

			for (int i = 0; i < this.items.Count; i++)
			{
				var item = this.items.ElementAt (i);

				if (selectedItems.Contains (item))
				{
					this.selectedIndexes.Add (i);
				}
			}
		}


		private static readonly double							lineHeight = 16;

		private readonly SelectionController<T>					controller;
		private readonly EnumValueCardinality					cardinality;
		private readonly string									title;
		private readonly List<T>								items;
		private readonly List<FiltererItem>						filteredItems;
		private readonly List<FrameBox>							containers;
		private readonly List<int>								selectedIndexes;

		private TextFieldEx										filterField;
		private GlyphButton										filterClearButton;

		private VScroller										scroller;
		private FrameBox										previewFrame;

		private Button											deselectButton;
		private Button											selectAllButton;
		private Button											selectFilterButton;

		private Button											acceptButton;
		private Button											cancelButton;

		private Widgets.StaticGlyph								floatingArrowMark;

		private int												firstRow;
		private int												tabIndex;
	}
}
