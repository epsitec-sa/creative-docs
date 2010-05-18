//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

namespace Epsitec.Cresus.Core
{
	public class UIBuilder
	{
		public UIBuilder(Widget container, CoreViewController controller)
		{
			this.container = container;
			this.controller = controller;
			this.dataItems = new List<SummaryData> ();
		}

		public TitleTile CreateEditionGroupingTile(string iconUri, string title)
		{
			var group = this.CreateSummaryGroupingTile (iconUri, title);

			group.IsReadOnly = false;

			return group;
		}

		public TitleTile CreateSummaryGroupingTile(string iconUri, string title)
		{
			var group = new TitleTile
			{
				Parent = this.container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
				ArrowDirection = Direction.Right,
				IconUri = iconUri,
				Title = title,
				IsReadOnly = true,
			};

			UIBuilder.CreateGroupingTileHandler (group, this.controller);

			return group;
		}

		public SummaryTile CreateSummaryTile(TitleTile parent, Accessors.AbstractEntityAccessor accessor)
		{
			var controller = new Controllers.EntityTileController<AbstractEntity> ()
			{
				Entity = accessor.AbstractEntity,
				ChildrenMode = accessor.ViewControllerMode
			};

			SummaryTile tile;

			if (accessor.EnableAddAndRemove)
			{
				tile = new CollectionItemTile ();
			}
			else
			{
				tile = new SummaryTile ();
			}

			tile.AutoHilite = accessor.ViewControllerMode != ViewControllerMode.None;
			tile.Controller = controller;
			tile.Summary = accessor.Summary;

			parent.Items.Add (tile);

			UIBuilder.CreateTileHandler (tile, this.controller);

#if false
			tile.CreateEntity += this.HandleTileCreateEntity;
			tile.RemoveEntity += this.HandleTileRemoveEntity;
#endif

			return tile;
		}

		public EditionTile CreateEditionTile(TitleTile parent, Accessors.AbstractEntityAccessor accessor)
		{
			var controller = new Controllers.EntityTileController<AbstractEntity> ()
			{
				Entity = accessor.AbstractEntity,
				ChildrenMode = accessor.ViewControllerMode
			};

			var tile = new EditionTile
			{
				AutoHilite = accessor.ViewControllerMode != ViewControllerMode.None,
				Controller = controller,
				IsReadOnly = false,
			};

			UIBuilder.CreateTileHandler (tile, this.controller);

			parent.Items.Add (tile);

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
			var tile = new EditionTile
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


		public void MapDataToTiles(IEnumerable<SummaryData> items)
		{
			this.dataItems.Clear ();
			this.dataItems.AddRange (items);
			this.dataItems.Sort ();

			this.CreateMissingDataTiles ();
			this.ResetDataTiles ();
			this.RefreshDataTiles ();
			this.SetDataTilesParent (this.container);
		}



		private void CreateMissingDataTiles()
		{
			foreach (var item in this.dataItems)
			{
				if (item.SummaryTile == null)
				{
					item.SummaryTile = new Widgets.Tiles.SummaryTile ();
				}

				if (item.TitleTile == null)
				{
					item.TitleTile = new Widgets.Tiles.TitleTile ();
					item.TitleTile.Items.Add (item.SummaryTile);
				}
			}
		}

		private void ResetDataTiles()
		{
			HashSet<long> visualIds = new HashSet<long> ();

			foreach (var item in this.dataItems)
			{
				System.Diagnostics.Debug.Assert (item.TitleTile != null);
				System.Diagnostics.Debug.Assert (item.SummaryTile != null);

				long visualId = item.TitleTile.GetVisualSerialId ();

				if (visualIds.Contains (visualId))
				{
					item.TitleTile.Items.Remove (item.SummaryTile);
					item.TitleTile = new Widgets.Tiles.TitleTile ();
					item.TitleTile.Items.Add (item.SummaryTile);
				}
				else
				{
					item.TitleTile.Parent = null;
					visualIds.Add (visualId);
				}

				item.SummaryTile.IsCompact = false;
			}
		}

		private double RefreshDataTiles()
		{
			var visualIds = new HashSet<long> ();
			var titleTiles = new List<Widgets.Tiles.TitleTile> ();

			foreach (var item in this.dataItems)
			{
				if (item.SummaryTile.IsCompact)
				{
					item.SummaryTile.Summary = item.CompactText.ToString ();
					item.TitleTile.Title     = item.CompactTitle.ToString ();
				}
				else
				{
					item.SummaryTile.Summary = item.Text.ToString ();
					item.TitleTile.Title     = item.Title.ToString ();
				}

				long visualId = item.TitleTile.GetVisualSerialId ();

				if (!visualIds.Contains (visualId))
				{
					visualIds.Add (visualId);
					titleTiles.Add (item.TitleTile);
				}
			}

			double height = 0;

			foreach (var tile in titleTiles)
			{
				if (height > 0)
				{
					height += 5;
				}

				height += tile.GetFullHeight ();
			}

			return height;
		}

		private void SetDataTilesParent(Widget parent)
		{
			var visualIds = new HashSet<long> ();

			foreach (var item in this.dataItems)
			{
				long visualId = item.TitleTile.GetVisualSerialId ();

				if (!visualIds.Contains (visualId))
				{
					visualIds.Add (visualId);
					item.TitleTile.Parent = parent;
				}
			}
		}
		
		
		private static void CreateGroupingTileHandler(TitleTile group, CoreViewController controller)
		{
			group.Clicked +=
				delegate
				{
					if (group.Items.Count == 1)
					{
						//	Si on a cliqué dans le conteneur GroupingTile d'un seul SummaryTile, il
						//	faut faire comme si on avait cliqué dans ce dernier.
						var tile = group.Items[0] as SummaryTile;
						if (tile != null)
						{
							tile.ToggleSubView (controller.Orchestrator, controller);
						}
					}
				};
		}

		private static void CreateTileHandler(GenericTile tile, CoreViewController controller)
		{
			tile.Clicked +=
				delegate
				{
					//	Appelé lorsqu'une tuile quelconque est cliquée.
					tile.ToggleSubView (controller.Orchestrator, controller);
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
		private readonly List<SummaryData> dataItems;
		private int tabIndex;
	}
}
