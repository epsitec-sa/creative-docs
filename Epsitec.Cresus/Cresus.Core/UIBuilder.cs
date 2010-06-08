//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

namespace Epsitec.Cresus.Core
{
	public class UIBuilder
	{
		public UIBuilder(Widget container, CoreViewController controller)
		{
			this.container = container;
			this.controller = controller;
		}

		public TitleTile CurrentGroupingTile
		{
			get
			{
				return this.groupingTile;
			}
		}


		public TitleTile CreateEditionGroupingTile(string iconUri, string title)
		{
			this.groupingTile = this.CreateSummaryGroupingTile (iconUri, title);
			this.groupingTile.IsReadOnly = false;

			return this.groupingTile;
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
			var controller = new Controllers.TileController<AbstractEntity> ()
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
			var controller = new Controllers.TileController<AbstractEntity> ()
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

		public EditionTile CreateEditionTile(TitleTile parent = null)
		{
			if (parent == null)
            {
				parent = this.groupingTile;
            }

			var tile = new EditionTile
			{
				AutoHilite = false,
				IsReadOnly = false,
			};

			parent.Items.Add (tile);

			return tile;
		}

		public EditionTile CreateEditionTile(TitleTile parent, AbstractEntity entity)
		{
			var controller = new Controllers.TileController<AbstractEntity> ()
			{
				Entity = entity,
//-				ChildrenMode = accessor.ViewControllerMode
			};

			var tile = new EditionTile
			{
				AutoHilite = false,
				Controller = controller,
				IsReadOnly = false,
			};

			UIBuilder.CreateTileHandler (tile, this.controller);

			parent.Items.Add (tile);

			return tile;
		}

		public void CreateHeaderEditorTile()
		{
		}

		public void CreateFooterEditorTile()
		{
			System.Diagnostics.Debug.Assert (this.container != null);

			var closeButton = new GlyphButton
			{
				Parent = this.container,
				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = GlyphShape.Close,
				Anchor = AnchorStyles.TopRight,
				PreferredSize = new Size (18, 18),
				Margins = new Margins (0, Widgets.TileArrow.Breadth+2, 2, 0),
			};

			var controller   = this.controller;
			var orchestrator = controller.Orchestrator;

			closeButton.Clicked +=
				delegate
				{
					orchestrator.CloseView (controller);
				};
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

		public TextField CreateTextField(EditionTile tile, int width, string label, Epsitec.Common.Types.Converters.Marshaler marshaler)
		{
			return this.CreateTextField (tile.Container, width, label, marshaler);
		}

		public TextField CreateTextField(Widget embedder, int width, string label, Epsitec.Common.Types.Converters.Marshaler marshaler)
		{
			var staticText = new StaticText
			{
				Parent = embedder,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var textField = new TextFieldEx
			{
				Parent = embedder,
				Text = TextConverter.ConvertToTaggedText (marshaler.GetStringValue ()),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
				TabIndex = ++this.tabIndex,
				DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
			};

			var validator = new Epsitec.Common.Widgets.Validators.MarshalerValidator (textField, marshaler);

			if (width > 0)
			{
				textField.HorizontalAlignment = HorizontalAlignment.Left;
				textField.PreferredWidth = width;
			}

			UIBuilder.CreateTextFieldHandler (textField, marshaler);

			return textField;
		}

		public TextField CreateTextField(EditionTile tile, int width, string label, string initialValue, System.Action<string> valueSetter, System.Func<string, bool> validator)
		{
			return this.CreateTextField (tile.Container, width, label, initialValue, valueSetter, validator);
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


		public Widgets.AutoCompleteTextField CreateAutoCompleteTextField<T>(string label, SelectionController<T> controller)
			where T : AbstractEntity
		{
			var tile = this.CreateEditionTile ();
			var autoCompleteTextField = this.CreateAutoCompleteTextField (tile, label, controller.GetValue (), null, x => controller.SetValue (x as T));

			controller.Attach (autoCompleteTextField);

			return autoCompleteTextField;
		}

		private Widgets.AutoCompleteTextField CreateAutoCompleteTextField(EditionTile tile, string label, AbstractEntity entity, Accessors.AbstractAccessor accessor, System.Action<AbstractEntity> valueSetter)
		{
			tile.AllowSelection = true;

			var staticText = new StaticText
			{
				Parent = tile.Container,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var container = new FrameBox
			{
				Parent = tile.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
				TabIndex = ++this.tabIndex,
			};

			var editor = new Widgets.AutoCompleteTextField
			{
				Parent = container,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
				HintEditorComboMenu = Widgets.HintEditorComboMenu.IfReasonable,
				ComboMenuReasonableItemsLimit = 100,
				TabIndex = ++this.tabIndex,
			};

			var showButton = new GlyphButton
			{
				Parent = container,
				GlyphShape = GlyphShape.ArrowRight,
				PreferredWidth = 20,
				PreferredHeight = 20,
				Dock = DockStyle.Right,
				Margins = new Margins (5, 0, 0, 0),
				Enable = false,
				AutoFocus = false,
			};

			editor.SelectedItemChanged +=
				delegate
				{
					if (editor.InError)
					{
						showButton.GlyphShape = GlyphShape.Plus;
						showButton.Enable = true;
					}
					else
					{
						if (editor.HasItemValue)
						{
							showButton.GlyphShape = GlyphShape.ArrowRight;
							showButton.Enable = true;
						}
						else
						{
							showButton.GlyphShape = GlyphShape.ArrowRight;
							showButton.Enable = false;
						}
					}

					if (editor.SelectedItemIndex > -1)
					{
						valueSetter (editor.Items.GetValue (editor.SelectedItemIndex) as AbstractEntity);
					}
				};

			editor.TextChanged +=
				delegate
				{
					if (editor.InError)
					{
						showButton.GlyphShape = GlyphShape.Plus;
						showButton.Enable = true;
					}
					else
					{
						if (editor.HasItemValue)
						{
							showButton.GlyphShape = GlyphShape.ArrowRight;
							showButton.Enable = true;
						}
						else
						{
							showButton.GlyphShape = GlyphShape.ArrowRight;
							showButton.Enable = false;
						}
					}
				};

			showButton.Clicked +=
				delegate
				{
					if (tile.IsSelected)
					{
						this.controller.Orchestrator.CloseSubViews (this.controller);
						tile.SetSelected (false);
					}
					else
					{
						var newController = EntityViewController.CreateEntityViewController ("ViewController", entity, ViewControllerMode.Summary, this.controller.Orchestrator);

						if (newController != null)
						{
							this.controller.Orchestrator.ShowSubView (this.controller, newController);
							tile.SetSelected (true);
						}
					}
				};

			if (accessor != null)
			{
				accessor.WidgetInitialize (editor, entity);
			}

			return editor;
		}


		public Widget CreateEditionDetailedRadio<T>(int width, string label, SelectionController<T> controller)
			where T : AbstractEntity
		{
			var tile = this.CreateEditionTile ();
			var combo = this.CreateDetailedRadio (tile, width, label);

			controller.Attach (combo);

			return combo;
		}


		private Widgets.ItemPicker CreateDetailedRadio(EditionTile tile, int width, string label)
		{
			var staticText = new StaticText
			{
				Parent = tile.Container,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var combo = new Widgets.ItemPicker
			{
				Parent = tile.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 5, 10),
				AllowMultipleSelection = true,
				TabIndex = ++this.tabIndex,
			};

			return combo;
		}


		public Widget CreateEditionDetailedCheck<T>(int width, string label, SelectionController<T> controller)
			where T : AbstractEntity
		{
			var tile  = this.CreateEditionTile ();
			var combo = this.CreateDetailedCheck (tile, width, label);

			controller.Attach (combo);
			
			return combo;
		}

		private Widgets.ItemPicker CreateDetailedCheck(EditionTile tile, int width, string label)
		{
			var staticText = new StaticText
			{
				Parent = tile.Container,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var combo = new Widgets.ItemPicker
			{
				Parent = tile.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 5, 10),
				AllowMultipleSelection = false,
				TabIndex = ++this.tabIndex,
			};

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
				var combo = new Widgets.ItemPicker
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

		public void CreateMargin(EditionTile tile, bool horizontalSeparator = false)
		{
			this.CreateMargin (tile.Container, horizontalSeparator);
		}
		
		public void CreateMargin(Widget embedder, bool horizontalSeparator = false)
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


		public static FormattedText FormatText(params object[] values)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			bool emptyItem = true;

			foreach (var value in values.Select (item => UIBuilder.ConvertToText (item)))
			{
				string text = value.Replace ("\n", "<br/>").Trim ();
				
				if (text.Length == 0)
				{
					emptyItem = true;
					continue;
				}

				if (text[0] == '~')
				{
					if (emptyItem)
					{
						continue;
					}

					text = text.Substring (1);
				}

				if (!emptyItem && buffer[buffer.Length-1] != '(' && !Misc.IsPunctuationMark (text[0]))
				{
					buffer.Append (" ");
				}

				buffer.Append (text);

				emptyItem = text.EndsWith ("<br/>");
			}

			return new FormattedText (string.Join ("<br/>", buffer.ToString ().Split (new string[] { "<br/>" }, System.StringSplitOptions.RemoveEmptyEntries)).Replace ("()", ""));
		}

		public static string ConvertToText(object value)
		{
			if (value == null)
			{
				return "";
			}

			string text = value as string;

			if (text != null)
			{
				return text;
			}

			if (value is Date)
			{
				return ((Date)value).ToDateTime ().ToShortDateString ();
			}

			return value.ToString ();
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

		internal static void CreateTileHandler(GenericTile tile, CoreViewController controller)
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

		private static void CreateTextFieldHandler(Widget textField, Epsitec.Common.Types.Converters.Marshaler marshaler)
		{
			textField.TextChanged +=
				delegate
				{
					string text = TextConverter.ConvertToSimpleText (textField.Text);
					marshaler.SetStringValue (text);
				};

			textField.KeyboardFocusChanged += (sender, e) => textField.Text = TextConverter.ConvertToTaggedText (marshaler.GetStringValue ());
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


		private readonly CoreViewController controller;
		private readonly Widget container;
		private int tabIndex;
		private TitleTile groupingTile;
	}
}
