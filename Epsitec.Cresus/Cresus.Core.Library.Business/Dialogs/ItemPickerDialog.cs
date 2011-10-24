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
	/// Dialogue pour l'ensemble des réglages globaux.
	/// </summary>
	public class ItemPickerDialog<T> : CoreDialog
			where T : AbstractEntity, new ()
	{
		public ItemPickerDialog(CoreApp application, SelectionController<T> controller, EnumValueCardinality cardinality)
			: base (application)
		{
			this.controller = controller;
			this.cardinality = cardinality;

			this.items = this.controller.GetPossibleItems ().ToList ();
			this.containers = new List<FrameBox> ();
			this.selectedIndexes = new List<int> ();

			this.UpdateSelectedIndexes ();
		}


		protected override void SetupWindow(Window window)
		{
			window.Text = "Choix";
			window.ClientSize = new Size (300, 300);
		}

		protected override void SetupWidgets(Window window)
		{
			var adorner = Common.Widgets.Adorners.Factory.Active;

			var filterFrame = new FrameBox
			{
				Parent  = window.Root,
				Dock    = DockStyle.Top,
				Margins = new Margins (10, 10, 10, 10),
			};

			var mainFrame = new FrameBox
			{
				Parent    = window.Root,
				BackColor = adorner.ColorTextBackground,
				Dock      = DockStyle.Fill,
				Margins   = new Margins (10, 10, 0, 0),
			};

			var footerFrame = new FrameBox
			{
				Parent          = window.Root,
				PreferredHeight = 20,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (10, 10, 10, 10),
			};

			new Separator
			{
				Parent          = window.Root,
				PreferredHeight = 1,
				Dock            = DockStyle.Bottom,
			};

			var actionsFrame = new FrameBox
			{
				Parent          = window.Root,
				PreferredHeight = 20,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (10, 10, 10, 10),
			};

			//	Crée les interfaces.
			this.CreateFilterUI  (filterFrame);
			this.CreateMainUI    (mainFrame);
			this.CreateActionsUI (actionsFrame);
			this.CreateFooterUI  (footerFrame);
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

			var filterField = new TextField
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			var clearButton = new GlyphButton
			{
				Parent     = parent,
				GlyphShape = GlyphShape.Reject,
				Enable     = false,
				Margins    = new Margins (1, 0, 0, 0),
				Dock       = DockStyle.Right,
			};

			ToolTip.Default.SetToolTip (clearButton, "Annuler le filtre");

			filterField.TextChanged += delegate
			{
				clearButton.Enable = !string.IsNullOrEmpty (filterField.Text);
			};

			clearButton.Clicked += delegate
			{
				filterField.Text = null;
			};
		}

		private void CreateMainUI(Widget parent)
		{
			var containersFrame = new FrameBox
			{
				Parent        = parent,
				DrawFullFrame = true,
				Margins       = new Margins (0, 1, 0, 0),
				Padding       = new Margins (2),
				Dock          = DockStyle.Fill,
			};

			this.scroller = new VScroller
			{
				Parent = parent,
				Dock   = DockStyle.Right,
			};

			parent.SizeChanged += delegate
			{
				this.UpdateContainers (containersFrame);
				this.UpdateLines ();
			};
		}

		private void CreateActionsUI(Widget parent)
		{
			parent.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			
			this.deselectButton = new Button
			{
				Parent  = parent,
				Text    = "Tout désélectionner",
				Margins = new Margins (0, 5, 0, 0),
				Dock    = DockStyle.Fill,
			};

			this.selectAllButton = new Button
			{
				Parent  = parent,
				Text    = "Tout sélectionner",
				Margins = new Margins (5, 0, 0, 0),
				Dock    = DockStyle.Fill,
			};

			this.deselectButton.Clicked += delegate
			{
				this.selectedIndexes.Clear ();

				this.UpdateLines ();
				this.UpdateActions ();
			};

			this.selectAllButton.Clicked += delegate
			{
				this.selectedIndexes.Clear ();

				var itemsCount = this.items.Count;
				for (int i = 0; i < itemsCount; i++)
				{
					this.selectedIndexes.Add (i);
				}

				this.UpdateLines ();
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
			this.selectAllButton.Enable = (this.selectedIndexes.Count != this.items.Count);
		}

		private void UpdateContainers(Widget parent)
		{
			int linesRequired = (int) (parent.ActualHeight / ItemPickerDialog<T>.lineHeight);

			if (this.containers.Count != linesRequired)
			{
				this.containers.Clear ();
				parent.Children.Clear ();

				double y = 0;

				for (int i = 0; i < linesRequired; i++)
				{
					var container = new FrameBox
					{
						Parent          = parent,
						PreferredHeight = ItemPickerDialog<T>.lineHeight,
						Index           = i,
						Margins         = new Margins (0, 0, y, 0),
						Anchor          = AnchorStyles.TopLeft | AnchorStyles.Right,
					};

					y += ItemPickerDialog<T>.lineHeight;

					container.Clicked += delegate
					{
						this.ContainerClicked (container.Index);
					};

					this.containers.Add (container);
				}
			}
		}

		private void ContainerClicked(int index)
		{
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

			this.UpdateLines ();
			this.UpdateActions ();
		}

		private void UpdateLines()
		{
			for (int i = 0; i < this.containers.Count; i++)
			{
				this.containers[i].Children.Clear ();

				if (i < this.items.Count)
				{
					var item = this.items.ElementAt (i);

					var icon = this.GetIconTag (this.selectedIndexes.Contains (i));
					var text = this.controller.ConvertHintValueToDescription (item);

					var line = new StaticText
					{
						Parent        = this.containers[i],
						FormattedText = FormattedText.Concat (icon, " ", text),
						TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
						Dock          = DockStyle.Fill,
					};
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
		private readonly List<T>								items;
		private readonly List<FrameBox>							containers;
		private readonly List<int>								selectedIndexes;

		private VScroller										scroller;
		private Button											deselectButton;
		private Button											selectAllButton;
		private Button											acceptButton;
		private Button											cancelButton;
	}
}
