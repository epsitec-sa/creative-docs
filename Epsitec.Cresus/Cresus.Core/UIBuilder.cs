//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public sealed class UIBuilder : System.IDisposable
	{
		public UIBuilder(TileContainer container, CoreViewController controller)
		{
			System.Diagnostics.Debug.Assert (container != null);
			System.Diagnostics.Debug.Assert (controller != null);
			
			this.container = container;
			this.controller = controller;
			this.nextBuilder = UIBuilder.current;

			if (this.nextBuilder != null)
			{
				this.tabIndex = this.nextBuilder.tabIndex;
			}

			UIBuilder.current = this;
		}


		public List<Widget> ContentList
		{
			get
			{
				return this.contentList;
			}
			set
			{
				this.contentList = value;
			}
		}

		private void ContentListAdd(Widget widget)
		{
			if (this.contentList != null)
			{
				this.contentList.Add (widget);
			}
		}


		public TitleTile CurrentTitleTile
		{
			get
			{
				return this.titleTile;
			}
		}


		public TitleTile CreateEditionTitleTile(string iconUri, string title)
		{
			double bottomMargin = -1;

			this.titleTile = new TitleTile
			{
				Parent = this.container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, bottomMargin),
				ArrowDirection = Direction.Right,
				TitleIconUri = iconUri,
				Title = title,
				IsReadOnly = false,
				TabIndex = ++this.tabIndex,
			};

			UIBuilder.CreateTitleTileHandler (this.titleTile, this.controller);

			return this.titleTile;
		}

		public EditionTile CreateEditionTile()
		{
			var tile = new EditionTile
			{
				AutoHilite = false,
				IsReadOnly = false,
				TabIndex = ++this.tabIndex,
			};

			this.ContentListAdd (tile);
			this.titleTile.Items.Add (tile);

			return tile;
		}


		public FrameBox CreateTabBook(EditionTile tile, List<string> pagesDescription, string defaultName, System.Action<string> action)
		{
			var container = new FrameBox
			{
				Parent = tile.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.HorizontalFlow,
				TabIndex = ++this.tabIndex,
			};

			for (int i = 0; i < pagesDescription.Count; i++)
			{
				string[] parts = pagesDescription[i].Split ('.');
				System.Diagnostics.Debug.Assert (parts.Length == 2);
				string name = parts[0];
				string text = parts[1];

				var tilePage = new Widgets.TilePage
				{
					Parent = container,
					Name = name,
					Text = text,
					PreferredHeight = 24 + Widgets.TileArrow.Breadth,
					Margins = new Margins (0, (i == pagesDescription.Count-1) ? 0 : -1, 0, 0),
					Dock = DockStyle.StackFill,
				};

				if (name == defaultName)
				{
					tilePage.SetSelected (true);
				}

				tilePage.Clicked +=
				delegate
				{
					foreach (Widgets.TilePage t in container.Children)
					{
						t.SetSelected (t.Name == tilePage.Name);
					}

					action (tilePage.Name);
				};
			}

			return container;
		}


		public void CreateHeaderEditorTile()
		{
			if (this.container.Controller != this.controller)
			{
				//	The header is not being created by the main controller; we can safely skip it,
				//	since we create only one header for every container.
				
				return;
			}

			System.Diagnostics.Debug.Assert (this.container.HasChildren == false);
		}

		public void CreateFooterEditorTile()
		{
			if (this.container.Controller != this.controller)
			{
				//	This method is not being called by the main controller; we can safely skip the
				//	creation of the footer, since we create only one footer for every container and
				//	the main controller will do it later on...
				
				return;
			}

			System.Diagnostics.Debug.Assert (this.container.FindChild ("ColumnTileCloseButton", Widget.ChildFindMode.Deep) == null);

			var closeButton = new GlyphButton
			{
				Parent = this.container,
				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = GlyphShape.Close,
				Anchor = AnchorStyles.TopRight,
				PreferredSize = new Size (18, 18),
				Margins = new Margins (0, Widgets.TileArrow.Breadth+2, 2, 0),
				Name = "ColumnTileCloseButton",
			};

			var controller   = this.container.Controller;
			var orchestrator = controller.Orchestrator;

			closeButton.Clicked +=
			delegate
				{
					orchestrator.CloseView (controller);
				};
		}


		public StaticText CreateWarning(EditionTile tile)
		{
			return this.CreateStaticText (tile, 60, "<i><b>ATTENTION:</b><br/>Les modifications effectuées ici seront répercutées<br/>dans tous les enregistrements.</i>");
		}

		public StaticText CreateStaticText(EditionTile tile, int height, string text)
		{
			var staticText = new StaticText
			{
				Parent = tile.Container,
				Text = text,
				TextBreakMode = Common.Drawing.TextBreakMode.Hyphenate,
				PreferredHeight = height,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 10, 10),
			};

			this.ContentListAdd (staticText);

			return staticText;
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

			this.ContentListAdd (staticText);
			this.ContentListAdd (textField);

			var validator = new Epsitec.Common.Widgets.Validators.MarshalerValidator (textField, marshaler);

			if (width > 0)
			{
				textField.HorizontalAlignment = HorizontalAlignment.Left;
				textField.PreferredWidth = width;
			}

			UIBuilder.CreateTextFieldHandler (textField, marshaler);

			return textField;
		}

		public TextFieldMultiEx CreateTextFieldMulti(EditionTile tile, int height, string label, Epsitec.Common.Types.Converters.Marshaler marshaler)
		{
			var staticText = new StaticText
			{
				Parent = tile.Container,
				Text = string.Concat (label, " :"),
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 2),
			};

			var textField = new TextFieldMultiEx
			{
				Parent = tile.Container,
				Text = TextConverter.ConvertToTaggedText (marshaler.GetStringValue ()),
				PreferredHeight = height,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 10, 0, 5),
				TabIndex = ++this.tabIndex,
				DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
				ScrollerVisibility = false,
				PreferredLayout = TextFieldMultiExPreferredLayout.PreserveScrollerHeight,
			};

			this.ContentListAdd (staticText);
			this.ContentListAdd (textField);

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

			//	Ce bouton vient tout à droite.
			var tileButton = new GlyphButton
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

			//	Ce bouton vient juste après (et tout contre) la ligne éditable.
			var menuButton = new GlyphButton
			{
				Parent = container,
				ButtonStyle = Common.Widgets.ButtonStyle.Combo,
				GlyphShape = GlyphShape.Menu,
				PreferredWidth = 20,
				PreferredHeight = 20,
				Dock = DockStyle.Right,
				Margins = new Margins (-1, 0, 0, 0),
				AutoFocus = false,
			};

			this.ContentListAdd (staticText);
			this.ContentListAdd (container);

			var changeHandler = UIBuilder.CreateAutoCompleteTextFieldChangeHandler (editor, tileButton);

			editor.SelectedItemChanged += sender => changeHandler ();
			editor.TextChanged         += sender => changeHandler ();

			menuButton.Clicked +=
				delegate
				{
					editor.SelectAll ();
					editor.Focus ();
					editor.OpenComboMenu ();
				};

			tileButton.Entered +=
				delegate
				{
					tile.TileArrowHilite = true;
				};

			tileButton.Exited +=
				delegate
				{
					tile.TileArrowHilite = false;
				};

			var controller = this.GetActiveController ();

			tileButton.Clicked +=
				delegate
				{
					if (tileButton.GlyphShape == GlyphShape.ArrowRight)
					{
						tile.Controller = new AutoCompleteTextFieldTileController (entity);
						tile.ToggleSubView (controller.Orchestrator, controller);
					}

					if (tileButton.GlyphShape == GlyphShape.Plus)
					{
						// TODO:
					}
				};

			return editor;
		}

		private class AutoCompleteTextFieldTileController : ITileController
		{
			public AutoCompleteTextFieldTileController(AbstractEntity entity)
			{
				this.entity = entity;
			}


			#region ITileController Members

			public EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator)
			{
				return EntityViewController.CreateEntityViewController ("AutoCompleteTextFieldEditionViewController", this.entity, ViewControllerMode.Edition, orchestrator);
			}

			#endregion

			#region IGroupedItem Members

			public string GetGroupId()
			{
				return null;
			}

			#endregion

			#region IGroupedItemPosition Members

			public int GroupedItemIndex
			{
				get
				{
					return 0;
				}
				set
				{
					throw new System.NotImplementedException ();
				}
			}

			public int GroupedItemCount
			{
				get
				{
					return 0;
				}
			}

			#endregion

			private readonly AbstractEntity entity;
		}

		private CoreViewController GetActiveController()
		{
			if (this.nextBuilder != null)
			{
				return this.nextBuilder.GetActiveController ();
			}
			else
			{
				return this.controller;
			}
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

			if (combo.SelectionCount == 0)  // aucune sélection ?
			{
				combo.AddSelection (Enumerable.Range (0, 1));  // sélectionne le premier
			}

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
				AllowMultipleSelection = false,
				TabIndex = ++this.tabIndex,
			};

			this.ContentListAdd (staticText);
			this.ContentListAdd (combo);

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
				AllowMultipleSelection = true,
				TabIndex = ++this.tabIndex,
			};

			this.ContentListAdd (staticText);
			this.ContentListAdd (combo);

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
					PreferredHeight = 1,
				};

				this.ContentListAdd (separator);
			}
			else
			{
				var frame = new FrameBox
				{
					Parent = tile.Container,
					Dock = DockStyle.Top,
					PreferredHeight = 10,
				};

				this.ContentListAdd (frame);
			}
		}


		public static FormattedText FormatText(params object[] values)
		{
			var buffer = new System.Text.StringBuilder ();
			var items  = new List<string> ();

			bool emptyItem = true;

			foreach (var value in values.Select (item => UIBuilder.ConvertToText (item)))
			{
				items.Add (value.Replace ("\n", "<br/>").Trim ());
			}

			int count = items.Count;

			items.Add ("");

			for (int i = 0; i < count; i++)
			{
				string text = items[i];
				string next = items[i+1];

				if (text.Length == 0)
				{
					emptyItem = true;
					continue;
				}

				if (text.FirstCharacter () == '~')
				{
					if (emptyItem)
					{
						continue;
					}

					text = text.Substring (1);
				}
				if (text.LastCharacter () == '~')
				{
					if (next.Length == 0)
					{
						continue;
					}

					text = text.Substring (0, text.Length-1);
				}

				if (!emptyItem && buffer.LastCharacter () != '(' && !Misc.IsPunctuationMark (text.FirstCharacter ()))
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


		private static void CreateTitleTileHandler(TitleTile titleTile, CoreViewController controller)
		{
			titleTile.Clicked +=
				delegate
				{
					if (titleTile.Items.Count == 1)
					{
						//	Si on a cliqué dans le conteneur TitleTile d'un seul SummaryTile, il
						//	faut faire comme si on avait cliqué dans ce dernier.
						var summaryTile = titleTile.Items[0] as SummaryTile;
						if (summaryTile != null)
						{
							if (!summaryTile.IsClickForDrag)
							{
								summaryTile.ToggleSubView (controller.Orchestrator, controller);
							}
						}
					}
				};
		}

		private static void CreateTextFieldHandler(AbstractTextField textField, Epsitec.Common.Types.Converters.Marshaler marshaler)
		{
			textField.AcceptingEdition +=
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

		#region IDisposable Members

		public void Dispose()
		{
			UI.SetInitialFocus (this.container);

			if (this.nextBuilder != null)
			{
				this.nextBuilder.tabIndex = this.tabIndex;
			}
			
			UIBuilder.current = this.nextBuilder;
		}

		#endregion

		private static UIBuilder current;

		private readonly CoreViewController controller;
		private readonly TileContainer container;
		private readonly UIBuilder nextBuilder;
		private int tabIndex;
		private TitleTile titleTile;
		private List<Widget> contentList;
	}
	
	static class StringExtension
	{
		public static char LastCharacter(this string text)
		{
			int n = text.Length - 1;
			return n < 0 ? (char) 0 : text[n];
		}
		
		public static char FirstCharacter(this string text)
		{
			int n = text.Length;
			return n < 1 ? (char) 0 : text[0];
		}
	}
	
	static class StringBuilderExtension
	{
		public static char LastCharacter(this System.Text.StringBuilder text)
		{
			int n = text.Length - 1;
			return n < 0 ? (char) 0 : text[n];
		}
	}
}
