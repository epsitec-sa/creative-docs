//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public class UIBuilder
	{
		public UIBuilder(Widget container, CoreViewController controller)
		{
			this.container = container;
			this.controller = controller;
		}

		public Widgets.GroupingTile CreateEditionGroupingTile(string iconUri, string title)
		{
			var group = this.CreateSummaryGroupingTile (iconUri, title);

			group.IsReadOnly = false;

			return group;
		}

		public Widgets.GroupingTile CreateSummaryGroupingTile(string iconUri, string title)
		{
			var group = new Widgets.GroupingTile
			{
				Parent = this.container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
				ArrowDirection = Direction.Right,
				EnteredSensitivity = true,
				TopLeftIconUri = iconUri,
				Title = title,
				IsReadOnly = true,
			};

			UIBuilder.CreateGroupingTileHandler (group, this.controller);

			return group;
		}

		public Widgets.SummaryTile CreateSummaryTile(Widgets.GroupingTile parent, Accessors.AbstractEntityAccessor accessor)
		{
			var tile = new Widgets.SummaryTile
			{
				Parent = parent.Container,
				ParentGroupingTile = parent,
				Dock = DockStyle.Top,
				ArrowDirection = Direction.Right,
				EnteredSensitivity = accessor.ViewControllerMode != ViewControllerMode.None,
				EntitiesAccessor = accessor,
				Entity = accessor.AbstractEntity,
				ChildrenMode = accessor.ViewControllerMode,
				EnableCreateAndRemoveButton = accessor.EnableAddAndRemove,
				IsReadOnly = true,
				Summary = accessor.Summary,
			};

			tile.PreferredHeight = tile.ContentHeight;

			parent.ChildrenTiles.Add (tile);

			UIBuilder.CreateTileHandler (tile, this.controller);

#if false
			tile.CreateEntity += this.HandleTileCreateEntity;
			tile.RemoveEntity += this.HandleTileRemoveEntity;
#endif

			return tile;
		}

		public Widgets.EditionTile CreateEditionTile(Widgets.GroupingTile parent, Accessors.AbstractEntityAccessor accessor)
		{
			var tile = new Widgets.EditionTile
			{
				Parent = parent.Container,
				ParentGroupingTile = parent,
				Dock = DockStyle.Top,
				ArrowDirection = Direction.Right,
				EnteredSensitivity = accessor.ViewControllerMode != ViewControllerMode.None,
				EntitiesAccessor = accessor,
				Entity = accessor.AbstractEntity,
				ChildrenMode = accessor.ViewControllerMode,
				IsReadOnly = false,
			};

			UIBuilder.CreateTileHandler (tile, controller);

			parent.ChildrenTiles.Add (tile);

			return tile;
		}

		public void CreateHeaderEditorTile()
		{
			var tile = new FrameBox
			{
				Parent = this.container,
				Dock = DockStyle.Top,
			};

#if false
			var title = new StaticText
			{
				Parent = tile,
				Dock = DockStyle.Fill,
				ContentAlignment = ContentAlignment.MiddleCenter,
				Text = "Sous-titre",
			};
#endif

			var closeButton = new GlyphButton
			{
				Parent = tile,
				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = GlyphShape.Close,
				Dock = DockStyle.Right,
				//?GlyphShape = GlyphShape.ArrowLeft,
				//?Dock = DockStyle.Left,
				PreferredSize = new Size (18, 18),
				Margins = new Margins (0, Widgets.TileArrow.Breadth+2, 2, 2-1),
				//?Margins = new Margins (2, Widgets.ArrowedTileArrow.ArrowBreadth+2, 2, 2-1),
			};


			var controller   = this.controller;
			var orchestrator = controller.Orchestrator;

			closeButton.Clicked +=
				delegate
				{
					orchestrator.CloseView (controller);
				};
		}

		public void CreateFooterEditorTile()
		{
			System.Diagnostics.Debug.Assert (this.container != null);

#if false
			var tile = new Widgets.EditionTile
			{
				Parent = this.container,
				Dock = DockStyle.Top,
				ArrowLocation = Direction.Right,
				IsReadOnly = false
			};

			var closeButton = new Button
			{
				Parent = tile,
				Text = "Fermer",
				PreferredWidth = 75,
				Dock = DockStyle.Right,
				Margins = new Margins(0, Widgets.ArrowedTileArrow.ArrowBreadth+10, 10, 10),
			};

			closeButton.Clicked += new EventHandler<MessageEventArgs> (this.HandleCloseButtonClicked);
#endif
		}

		public TextField CreateTextField(Widget embedder, int width, string initialValue, System.Action<string> valueSetter, System.Func<string, bool> validator)
		{
			var textField = new TextField
			{
				Parent = embedder,
				Text = initialValue,
				Dock = (width == 0) ? DockStyle.Fill : DockStyle.Left,
				PreferredWidth = width,
				Margins = new Margins (0, (width == 0) ? 10:2, 0, 0),
				TabIndex = ++this.tabIndex,
			};

			UIBuilder.CreateTextFieldHandler (textField, valueSetter, validator);

			return textField;
		}

		public TextField CreateTextField(Widget embedder, int width, string label, string initialValue, System.Action<string> valueSetter, System.Func<string, bool> validator)
		{
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			TextField textField;

			if (width == 0)  // occupe toute la largeur ?
			{
				textField = new TextField
				{
					Parent = embedder,
					Text = initialValue,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 10, 0, 5),
					TabIndex = ++this.tabIndex,
				};
			}
			else  // largeur partielle fixe ?
			{
				var box = new FrameBox
				{
					Parent = embedder,
					Dock = DockStyle.Top,
					TabIndex = ++this.tabIndex,
				};

				textField = new TextField
				{
					Parent = box,
					Text = initialValue,
					Dock = DockStyle.Left,
					PreferredWidth = width,
					Margins = new Margins (0, 10, 0, 5),
					TabIndex = ++this.tabIndex,
				};
			}

			UIBuilder.CreateTextFieldHandler (textField, valueSetter, validator);

			return textField;
		}

		public TextFieldMulti CreateTextFieldMulti(Widget embedder, int height, string label, string initialValue, System.Action<string> valueSetter, System.Func<string, bool> validator)
		{
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var textField = new TextFieldMulti
			{
				Parent = embedder,
				Text = initialValue,
				PreferredHeight = height,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
				TabIndex = ++this.tabIndex,
			};

			UIBuilder.CreateTextFieldHandler (textField, valueSetter, validator);

			return textField;
		}

		public Widgets.HintEditor CreateHintEditor(Widget embedder, string label, AbstractEntity entity, Accessors.AbstractAccessor accessor, System.Action<AbstractEntity> valueSetter)
		{
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var container = new FrameBox
			{
				Parent = embedder,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
			};

			var editor = new Widgets.HintEditor
			{
				Parent = container,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
				HintEditorComboMenu = Widgets.HintEditorComboMenu.IfReasonable,
				ComboMenuReasonableItemsLimit = 100,
				TabIndex = ++this.tabIndex,
			};

			var button = new Button
			{
				Parent = container,
				Text = "Ajouter",
				PreferredWidth = 60,
				PreferredHeight = 20,
				Dock = DockStyle.Right,
				Margins = new Margins (5, 0, 0, 0),
				Visibility = false,
				TabIndex = ++this.tabIndex,
			};

			accessor.WidgetInitialize (editor, entity);

			editor.SelectedItemChanged +=
				delegate
				{
					if (editor.SelectedItemIndex > -1)
					{
						valueSetter (editor.Items.GetValue (editor.SelectedItemIndex) as AbstractEntity);
					}
				};

			editor.TextChanged +=
				delegate
				{
					button.Visibility = editor.InError;
				};

			return editor;
		}

		public Widget CreateDetailed(Widget embedder, int width, string label, bool allowMultipleSelection, object listEntities, Accessors.AbstractAccessor accessor, System.Action<AbstractEntity> valueSetter)
		{
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var combo = new Widgets.DetailedCombo
			{
				Parent = embedder,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 5, 10),
				AllowMultipleSelection = allowMultipleSelection,
				TabIndex = ++this.tabIndex,
			};

			accessor.WidgetInitialize (combo, listEntities);

			// TODO: câbler l'événement MultiSelectionChanged
				
			return combo;
		}

		public Widget CreateCombo(Widget embedder, int width, string label, Accessors.ComboInitializer initializer, bool readOnly, bool allowMultipleSelection, bool detailed, string initialValue, System.Action<string> callback, System.Func<string, bool> validator)
		{
			// TODO: à supprimer
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			if (detailed)
			{
				var combo = new Widgets.DetailedCombo
				{
					Parent = embedder,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 5, 10),
					AllowMultipleSelection = allowMultipleSelection,
					//?ComboInitializer = initializer,
					Text = initializer.ConvertInternalToEdition (initialValue),
					TabIndex = ++this.tabIndex,
				};

				UIBuilder.CreateComboHandler (combo, callback, validator, initializer.ConvertEditionToInternal);
				
				return combo;
			}
			else
			{
				Widgets.SuperCombo combo;

				if (width == 0)  // occupe toute la largeur ?
				{
					combo = new Widgets.SuperCombo
					{
						Parent = embedder,
						IsReadOnly = readOnly,
						AllowMultipleSelection = allowMultipleSelection,
						//?Text = initializer.ConvertInternalToEdition (initialValue),
						Dock = DockStyle.Top,
						Margins = new Margins (0, 10, 0, 5),
						TabIndex = ++this.tabIndex,
					};
				}
				else  // largeur partielle fixe ?
				{
					var box = new FrameBox
					{
						Parent = embedder,
						Dock = DockStyle.Top,
						TabIndex = ++this.tabIndex,
					};

					combo = new Widgets.SuperCombo
					{
						Parent = box,
						IsReadOnly = readOnly,
						AllowMultipleSelection = allowMultipleSelection,
						//?Text = initializer.ConvertInternalToEdition (initialValue),
						Dock = DockStyle.Left,
						PreferredWidth = width,
						Margins = new Margins (0, 10, 0, 5),
						TabIndex = ++this.tabIndex,
					};
				}

				initializer.InitializeCombo (combo);
				combo.AddSelection (Enumerable.Range (0, 1));

				UIBuilder.CreateComboHandler (combo, callback, validator, initializer.ConvertEditionToInternal);

				return combo;
			}
		}

		public void CreateLinkButtons(Widget embedder)
		{
#if false
			//	TODO: Prototype non fonctionnel, à valider puis terminer
			var frameBox = new ClippingFrameBox
			{
				Parent = embedder,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 10, 20),
				TabIndex = ++this.tabIndex,
				ClippingMode = ClippingMode.ClipWidth,
			};

			var linkButton = new Button
			{
				Parent = frameBox,
				Text = "Lier avec...",
				PreferredWidth = 70,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 2, 0, 0),
				TabIndex = ++this.tabIndex,
			};

			var unlinkButton = new Button
			{
				Parent = frameBox,
				Text = "Délier",
				PreferredWidth = 70,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 10, 0, 0),
				TabIndex = ++this.tabIndex,
				Enable = false,
			};

			var label = new StaticText
			{
				Parent = frameBox,
				Text = "Utilisation unique",  // ou "Utilisé 10x"
				Dock = DockStyle.Fill,
			};
#endif
		}
		
		public void CreateMargin(Widget embedder, bool horizontalSeparator)
		{
			if (horizontalSeparator)
			{
				var separator = new Separator
				{
					Parent = embedder,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 10, 10, 10),
					PreferredHeight = 2,
				};
			}
			else
			{
				var frame = new FrameBox
				{
					Parent = embedder,
					Dock = DockStyle.Top,
					PreferredHeight = 10,
				};
			}
		}

		public static double GetContentHeight(Widget container)
		{
			Common.Widgets.Layouts.LayoutContext.SyncMeasure (container);
			
			double currentHeight = 0;
			foreach (Widget widget in container.Children)
			{
				currentHeight += Epsitec.Common.Widgets.Layouts.LayoutMeasure.GetHeight (widget).Desired;
				currentHeight += widget.Margins.Height;
			}
			return currentHeight;
		}

		
		private static void CreateGroupingTileHandler(Widgets.GroupingTile group, CoreViewController controller)
		{
			group.Clicked +=
				delegate
				{
					if (group.ChildrenTiles.Count == 1)
					{
						//	Si on a cliqué dans le conteneur GroupingTile d'un seul SummaryTile, il
						//	faut faire comme si on avait cliqué dans ce dernier.
						var tile = group.ChildrenTiles[0] as Widgets.SummaryTile;
						if (tile != null)
						{
							tile.OpenOrCloseSubView (controller.Orchestrator, controller);
						}
					}
				};
		}

		private static void CreateTileHandler(Widgets.AbstractTile tile, CoreViewController controller)
		{
			tile.Clicked +=
				delegate
				{
					//	Appelé lorsqu'une tuile quelconque est cliquée.
					tile.OpenOrCloseSubView (controller.Orchestrator, controller);
				};
		}
		
		private static void CreateTextFieldHandler(Widget textField, System.Action<string> valueSetter, System.Func<string, bool> validator)
		{
			textField.TextChanged +=
				delegate
				{
					if (validator == null || validator (textField.Text))
					{
						valueSetter (textField.Text);
						textField.SetError (false);
					}
					else
					{
						textField.SetError (true);
					}
				};
		}

		private static void CreateComboHandler(Widget widget, System.Action<string> valueSetter, System.Func<string, bool> validator, System.Func<string, string> converter)
		{
			widget.TextChanged +=
				delegate
				{
					string text = converter (widget.Text);

					if (validator == null || validator (text))
					{
						valueSetter (text);
						widget.SetError (false);
					}
					else
					{
						widget.SetError (true);
					}
				};
		}


		private static void CreateHintHandler(Epsitec.Cresus.Core.Widgets.HintEditor hint, System.Action<string> callback1, System.Action<string> callback2, Accessors.BidirectionnalConverter converter)
		{
			hint.EditionAccepted +=
				delegate
				{
					int sel = hint.SelectedItemIndex;
					if (sel != -1)
					{
						string text1, text2;
						converter.Get (sel, out text1, out text2);
						callback1 (text1);
						callback2 (text2);
					}
				};
		}
		private readonly Widget container;
		private readonly CoreViewController controller;
		private int tabIndex;
	}
}
