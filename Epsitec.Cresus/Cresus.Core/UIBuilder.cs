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
			this.groupingTile = new TitleTile
			{
				Parent = this.container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
				ArrowDirection = Direction.Right,
				IconUri = iconUri,
				Title = title,
				IsReadOnly = false,
			};

			UIBuilder.CreateGroupingTileHandler (this.groupingTile, this.controller);

			return this.groupingTile;
		}

		public EditionTile CreateEditionTile()
		{
			var tile = new EditionTile
			{
				AutoHilite = false,
				IsReadOnly = false,
			};

			this.groupingTile.Items.Add (tile);

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


		public TextField CreateTextField(EditionTile tile, int width, string label, Epsitec.Common.Types.Converters.Marshaler marshaler)
		{
			var staticText = new StaticText
			{
				Parent = tile.Container,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var textField = new TextFieldEx
			{
				Parent = tile.Container,
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

		public TextFieldMulti CreateTextFieldMulti(EditionTile tile, int height, string label, Epsitec.Common.Types.Converters.Marshaler marshaler)
		{
			var staticText = new StaticText
			{
				Parent = tile.Container,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var textField = new TextFieldMulti
			{
				Parent = tile.Container,
				Text = TextConverter.ConvertToTaggedText (marshaler.GetStringValue ()),
				PreferredHeight = height,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
				TabIndex = ++this.tabIndex,
			};

			var validator = new Epsitec.Common.Widgets.Validators.MarshalerValidator (textField, marshaler);

			UIBuilder.CreateTextFieldHandler (textField, marshaler);

			return textField;
		}


		public Widgets.AutoCompleteTextField CreateAutoCompleteTextField<T>(string label, SelectionController<T> controller)
			where T : AbstractEntity
		{
			var tile = this.CreateEditionTile ();
			var autoCompleteTextField = this.CreateAutoCompleteTextField (tile, label, controller.GetValue (), x => controller.SetValue (x as T));

			controller.Attach (autoCompleteTextField);

			return autoCompleteTextField;
		}

		private Widgets.AutoCompleteTextField CreateAutoCompleteTextField(EditionTile tile, string label, AbstractEntity entity, System.Action<AbstractEntity> valueSetter)
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

			var changeHandler = UIBuilder.CreateAutoCompleteTextFieldChangeHandler (editor, showButton);

			editor.SelectedItemChanged += sender => changeHandler ();
			editor.TextChanged         += sender => changeHandler ();

			showButton.Clicked +=
				delegate
				{
					if (showButton.GlyphShape == GlyphShape.ArrowRight)
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
					}

					if (showButton.GlyphShape == GlyphShape.Plus)
					{
						// TODO:
					}
				};

			return editor;
		}

		private static System.Action CreateAutoCompleteTextFieldChangeHandler(Widgets.AutoCompleteTextField editor, GlyphButton showButton)
		{
			return
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
							showButton.GlyphShape = GlyphShape.Plus;
							showButton.Enable = true;
						}
					}
				};
		}

		void editor_SelectedItemChanged(object sender)
		{
			throw new System.NotImplementedException ();
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


		public void CreateMargin(EditionTile tile, bool horizontalSeparator = false)
		{
			if (horizontalSeparator)
			{
				var separator = new Separator
				{
					Parent = tile.Container,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 10, 10, 10),
					PreferredHeight = 2,
				};
			}
			else
			{
				var frame = new FrameBox
				{
					Parent = tile.Container,
					Dock = DockStyle.Top,
					PreferredHeight = 10,
				};
			}
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
