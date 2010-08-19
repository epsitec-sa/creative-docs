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
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer;
using System.Linq.Expressions;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core
{
	public sealed class UIBuilder : System.IDisposable
	{
		public UIBuilder(EntityViewController controller)
			: this (controller.TileContainer, controller)
		{
		}

		private UIBuilder(TileContainer container, CoreViewController controller)
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


		public Widget Container
		{
			get
			{
				if (this.panelTitleTile != null)
				{
					return this.panelTitleTile.Panel;
				}
				else
				{
					return this.container;
				}
			}
		}

		public TileContainer TileContainer
		{
			get
			{
				return this.container;
			}
		}


		public TitleTile CurrentTitleTile
		{
			get
			{
				return this.titleTile;
			}
		}

		public TileTabBook CurrentTileTabBook
		{
			get
			{
				return this.tileTabBook;
			}
		}


		public void BeginTileTabPage(int index)
		{
			this.contentList = this.tileTabBook.Items.ElementAt (index).PageWidgets;
		}

		public void BeginTileTabPage<T>(T id)
		{
			var book = this.tileTabBook as TileTabBook<T>;
			var item = book.Items.Where (x => x.Id.Equals (id)).First ();
			
			this.contentList = item.PageWidgets;
		}


		public void EndTileTabPage()
		{
			this.contentList = null;
		}


		public void Add(Widget widget)
		{
			widget.Parent = this.Container;
			widget.TabIndex = ++this.tabIndex;
			widget.Dock = DockStyle.Top;

			this.ContentListAdd (widget);
		}

		public void CreateButton(string id, string title, string description, System.Action callback)
		{
			var button = new ConfirmationButton
			{
				Margins = new Margins (10, 16, 1, 0),
				Name = id,
				Text = ConfirmationButton.FormatContent (title, description),
				PreferredHeight = 52,
			};

			this.Add (button);

			button.Clicked += (sender, e) => callback ();
		}



		public ConfirmationButton CreateCreationButton<T>(CreationViewController<T> controller, string title, string description, System.Action<DataContext, T> initializer = null)
			where T : AbstractEntity, new ()
		{
			var button = new ConfirmationButton
			{
				Text = ConfirmationButton.FormatContent (title, description),
				PreferredHeight = 52,
			};

			button.Clicked +=
				delegate
				{
					controller.CreateRealEntity (initializer);
				};

			this.Add (button);

			return button;
		}

		
		public PanelTitleTile CreatePanelTitleTile(string iconUri, string title)
		{
			double bottomMargin = -1;

			this.panelTitleTile = new PanelTitleTile
			{
				Parent = this.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, bottomMargin),
				TitleIconUri = iconUri,
				Title = title,
				IsReadOnly = true,
				TabIndex = ++this.tabIndex,
			};

			return this.panelTitleTile;
		}

		public void EndPanelTitleTile()
		{
			this.panelTitleTile = null;
		}


		public TitleTile CreateEditionTitleTile(string iconUri, string title)
		{
			double bottomMargin = -1;

			this.titleTile = new TitleTile
			{
				Parent = this.Container,
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

		public SummaryTile CreateSummaryTile<T>(string name, T entity, FormattedText summary)
			where T : AbstractEntity
		{
			var fullName = string.Format (System.Globalization.CultureInfo.InstalledUICulture, "{0}.{1}", name, this.titleTile.Items.Count);

			var tile = new SummaryTile
			{
				AutoHilite     = false,
				IsReadOnly     = true,
				TabIndex       = ++this.tabIndex,
				Name           = fullName,
				Margins        = new Margins (0, 0, 0, -1),
				ArrowDirection = Direction.Right,
			};

			this.ContentListAdd (tile);
			this.titleTile.Items.Add (tile);
			this.titleTile.CanExpandSubTile = true;

			var clickSimulator = new TileButtonClickSimulator (this.titleTile, controller, fullName);

			tile.Controller = new SummaryTileController<T> (entity, fullName);
			tile.Summary    = summary.ToString ();

			return tile;
		}

		class SummaryTileController<T> : ITileController
			where T : AbstractEntity
		{
			public SummaryTileController(T entity, string name)
			{
				this.entity = entity;
				this.navigationPathElement = new TileNavigationPathElement (name);
			}
			#region ITileController Members

			public EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator, NavigationPathElement navigationPathElement)
			{
				return EntityViewController.CreateEntityViewController (typeof (T).Name, this.entity, ViewControllerMode.Summary, orchestrator, navigationPathElement: this.navigationPathElement);
			}

			#endregion

			private readonly T entity;
			private readonly TileNavigationPathElement navigationPathElement;
		}

		


		public TileTabBook<T> CreateTabBook<T>(params TabPageDef<T>[] pageDescriptions)
		{
			var tile = this.titleTile.Items.Last () as EditionTile;

			var tileTabBook = new TileTabBook<T> (pageDescriptions)
			{
				Parent = tile.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, 5),
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				TabIndex = ++this.tabIndex,
			};

			this.tileTabBook = tileTabBook;
			
			return tileTabBook;
		}

		public TileTabBook CreateTabBook(List<string> pageDescriptions, System.Action<string> action)
		{
#if false
			var container = new FrameBox
			{
				Parent = tile.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, 5),
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.HorizontalFlow,
				TabIndex = ++this.tabIndex,
			};

			for (int i = 0; i < pageDescriptions.Count; i++)
			{
				string[] parts = pageDescriptions[i].Split ('.');
				System.Diagnostics.Debug.Assert (parts.Length == 2);
				string name = parts[0];
				string text = parts[1];

				var tilePage = new Widgets.TilePage
				{
					Parent = container,
					Name = name,
					Text = text,
					PreferredHeight = 24 + Widgets.TileArrow.Breadth,
					Margins = new Margins (0, (i == pageDescriptions.Count-1) ? 0 : -1, 0, 0),
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
#else
			var list = new List<TabPageDef> ();

			foreach (var def in pageDescriptions)
			{
				string[] parts = def.Split ('.');
				System.Diagnostics.Debug.Assert (parts.Length == 2);
				string name = parts[0];
				string text = parts[1];
				list.Add (new TabPageDef (name, new FormattedText (text), () => action (name)));
			}

			var tile = this.titleTile.Items.Last () as EditionTile;

			this.tileTabBook = new TileTabBook (list)
			{
				Parent = tile.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, 5),
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				TabIndex = ++this.tabIndex,
			};

			return this.tileTabBook;
#endif
		}


		public void CreateHeaderEditorTile()
		{
			var controller = this.container.Controller;

			if (controller != this.controller)
			{
				//	The header is not being created by the main controller; we can safely skip it,
				//	since we create only one header for every container.
				
				return;
			}

			System.Diagnostics.Debug.Assert (this.Container.HasChildren == false);
		}

		public void CreateFooterEditorTile()
		{
			var controller = this.container.Controller;

			if (controller != this.controller)
			{
				//	This method is not being called by the main controller; we can safely skip the
				//	creation of the footer, since we create only one footer for every container and
				//	the main controller will do it later on...

				return;
			}

			UIBuilder.CreateColumnTileCloseButton (this.container);
		}


		public static Button CreateColumnTileCloseButton(TileContainer container)
		{
			Button closeButton = container.FindChild ("ColumnTileCloseButton", Widget.ChildFindMode.Deep) as Button;

			if (closeButton != null)
			{
				//?return closeButton;
			}

			var controller   = container.Controller;
			var orchestrator = controller.Orchestrator;

			closeButton = new GlyphButton
			{
				Parent = container,
				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = GlyphShape.Close,
				Anchor = AnchorStyles.TopRight,
				PreferredSize = new Size (18, 18),
				Margins = new Margins (0, Widgets.TileArrow.Breadth+2, 2, 0),
				Name = "ColumnTileCloseButton",
			};

			closeButton.Clicked +=
				delegate
				{
					orchestrator.CloseView (controller);
				};

			return closeButton;
		}
		
		
		public StaticText CreateWarning(EditionTile tile)
		{
			return this.CreateStaticText (tile, 60, "<i><b>ATTENTION:</b><br/>Les modifications effectuées ici seront répercutées<br/>dans tous les enregistrements.</i>");
		}

		public StaticText CreateStaticText(EditionTile tile, double height, string text)
		{
			var staticText = new StaticText
			{
				Parent = tile.Container,
				Text = text,
				TextBreakMode = Common.Drawing.TextBreakMode.Hyphenate,
				PreferredHeight = height,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 5, 5),
			};

			this.ContentListAdd (staticText);

			return staticText;
		}


		public FrameBox CreateGroup(EditionTile tile, string label)
		{
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Top,
					Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			var frameBox = new FrameBox
			{
				Parent = tile.Container,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			this.ContentListAdd (frameBox);

			return frameBox;
		}


		public Button CreateButton(EditionTile tile, double width, string label, string buttonText)
		{
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Top,
					Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			return this.CreateButton (tile.Container, DockStyle.Top, width, buttonText);
		}

		public Button CreateButton(FrameBox parent, DockStyle dockStyle, double width, string buttonText)
		{
			var button = new Button
			{
				Parent = parent,
				Text = buttonText,
				PreferredHeight = 20,
				Dock = dockStyle,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			this.ContentListAdd (button);

			if (width > 0)
			{
				button.HorizontalAlignment = HorizontalAlignment.Left;
				button.PreferredWidth = width;
			}

			return button;
		}


		public TextFieldEx CreateTextField(EditionTile tile, double width, string label, Marshaler marshaler)
		{
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Top,
					Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			return this.CreateTextField (tile.Container, DockStyle.Top, width, marshaler);
		}

		public TextFieldEx CreateTextField(FrameBox parent, DockStyle dockStyle, double width, Marshaler marshaler)
		{
			var textField = new TextFieldEx
			{
				Parent = parent,
				PreferredHeight = 20,
				Dock = dockStyle,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
				DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
			};

			this.ContentListAdd (textField);

			if (width > 0)
			{
				textField.HorizontalAlignment = HorizontalAlignment.Left;
				textField.PreferredWidth = width;
			}

			var valueController = new TextValueController (marshaler);
			valueController.Attach (textField);
			this.container.WidgetUpdaters.Add (valueController);

			return textField;
		}

		public TextFieldMultiEx CreateTextFieldMulti(EditionTile tile, double height, string label, Marshaler marshaler)
		{
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Top,
					Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			var textField = new TextFieldMultiEx
			{
				Parent = tile.Container,
				PreferredHeight = height,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
				DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
				ScrollerVisibility = false,
				PreferredLayout = TextFieldMultiExPreferredLayout.PreserveScrollerHeight,
			};

			this.ContentListAdd (textField);

			var valueController = new TextValueController (marshaler);
			valueController.Attach (textField);
			this.container.WidgetUpdaters.Add (valueController);

			return textField;
		}


		public Widgets.AutoCompleteTextField CreateAutoCompleteTextField(EditionTile tile, double width, string label, Marshaler marshaler, IEnumerable<string[]> possibleItems, System.Func<string[], FormattedText> getUserText)
		{
			//	possibleItems[0] doit obligatoirement être la 'key' !
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Top,
					Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			var container = new FrameBox
			{
				Parent = tile.Container,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			Widgets.AutoCompleteTextField textField;
			GlyphButton menuButton;

			if (width == 0)
			{
				textField = new Widgets.AutoCompleteTextField
				{
					Parent = container,
					MenuButtonWidth = UIBuilder.ComboButtonWidth-1,
					PreferredHeight = 20,
					Dock = DockStyle.Fill,
					Margins = new Margins (0, 0, 0, 0),
					HintEditorComboMenu = Widgets.HintEditorComboMenu.Always,
					TabIndex = ++this.tabIndex,
				};

				menuButton = new GlyphButton
				{
					Parent = container,
					ButtonStyle = Common.Widgets.ButtonStyle.Combo,
					GlyphShape = GlyphShape.Menu,
					PreferredWidth = UIBuilder.ComboButtonWidth,
					PreferredHeight = 20,
					Dock = DockStyle.Right,
					Margins = new Margins (-1, 0, 0, 0),
					AutoFocus = false,
				};
			}
			else
			{
				textField = new Widgets.AutoCompleteTextField
				{
					Parent = container,
					MenuButtonWidth = UIBuilder.ComboButtonWidth-1,
					PreferredWidth = width,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, 0, 0),
					HintEditorComboMenu = Widgets.HintEditorComboMenu.Always,
					TabIndex = ++this.tabIndex,
				};

				menuButton = new GlyphButton
				{
					Parent = container,
					ButtonStyle = Common.Widgets.ButtonStyle.Combo,
					GlyphShape = GlyphShape.Menu,
					PreferredWidth = UIBuilder.ComboButtonWidth,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (-1, 0, 0, 0),
					AutoFocus = false,
				};
			}

			this.ContentListAdd (container);

			menuButton.Clicked +=
			delegate
			{
				textField.SelectAll ();
				textField.Focus ();
				textField.OpenComboMenu ();
			};

			var valueController = new TextValueController (marshaler, possibleItems, getUserText);
			valueController.Attach (textField);
			this.container.WidgetUpdaters.Add (valueController);

			return textField;
		}


		public Widgets.AutoCompleteTextField CreateAutoCompleteTextField<T>(EditionTile tile, double width, string label, Marshaler marshaler, IEnumerable<EnumKeyValues<T>> possibleItems, System.Func<EnumKeyValues<T>, FormattedText> getUserText)
		{
			//	possibleItems.Item1 est la 'key' !
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Top,
					Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			var container = new FrameBox
			{
				Parent = tile.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			Widgets.AutoCompleteTextField textField;
			GlyphButton menuButton;


			if (width == 0)
			{
				textField = new Widgets.AutoCompleteTextField
				{
					Parent = container,
					MenuButtonWidth = UIBuilder.ComboButtonWidth-1,
					PreferredHeight = 20,
					Dock = DockStyle.Fill,
					Margins = new Margins (0, 0, 0, 0),
					HintEditorComboMenu = Widgets.HintEditorComboMenu.Always,
					TabIndex = ++this.tabIndex,
				};

				menuButton = new GlyphButton
				{
					Parent = container,
					ButtonStyle = Common.Widgets.ButtonStyle.Combo,
					GlyphShape = GlyphShape.Menu,
					PreferredWidth = UIBuilder.ComboButtonWidth,
					PreferredHeight = 20,
					Dock = DockStyle.Right,
					Margins = new Margins (-1, 0, 0, 0),
					AutoFocus = false,
				};
			}
			else
			{
				textField = new Widgets.AutoCompleteTextField
				{
					Parent = container,
					MenuButtonWidth = UIBuilder.ComboButtonWidth-1,
					PreferredWidth = width,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, 0, 0),
					HintEditorComboMenu = Widgets.HintEditorComboMenu.Always,
					TabIndex = ++this.tabIndex,
				};

				menuButton = new GlyphButton
				{
					Parent = container,
					ButtonStyle = Common.Widgets.ButtonStyle.Combo,
					GlyphShape = GlyphShape.Menu,
					PreferredWidth = UIBuilder.ComboButtonWidth,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (-1, 0, 0, 0),
					AutoFocus = false,
				};
			}

			this.ContentListAdd (container);

			menuButton.Clicked +=
			delegate
			{
				textField.SelectAll ();
				textField.Focus ();
				textField.OpenComboMenu ();
			};

			var valueController = new EnumValueController<T> (marshaler, possibleItems, getUserText);
			valueController.Attach (textField);
			this.container.WidgetUpdaters.Add (valueController);

			return textField;
		}


		public Widgets.AutoCompleteTextField CreateAutoCompleteTextField<T>(string label, SelectionController<T> controller)
			where T : AbstractEntity
		{
			var tile = this.CreateEditionTile ();

			var autoCompleteTextField = this.CreateAutoCompleteTextField (tile, label, x => controller.SetValue (x as T), controller.ReferenceController);

			controller.Attach (autoCompleteTextField);
			this.container.WidgetUpdaters.Add (controller);

			return autoCompleteTextField;
		}

		public Widgets.ItemPicker CreateEditionDetailedItemPicker<T>(string label, EnumController<T> controller)
			where T : struct
		{
			var tile = this.CreateEditionTile ();

			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Top,
					Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				};

				this.ContentListAdd (staticText);
			}

			var widget = new Widgets.ItemPicker
			{
				Parent = tile.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 3),
				TabIndex = ++this.tabIndex,
			};

			this.ContentListAdd (widget);

			controller.Attach (widget);

			return widget;
		}


		public Widgets.ItemPicker CreateEditionDetailedItemPicker<T>(string label, SelectionController<T> controller, BusinessLogic.EnumValueCardinality cardinality)
			where T : AbstractEntity
		{
			var tile = this.CreateEditionTile ();
			var combo = this.CreateDetailedItemPicker (tile, label, cardinality);

			controller.Attach (combo);

			if (cardinality == BusinessLogic.EnumValueCardinality.ExactlyOne ||
				cardinality == BusinessLogic.EnumValueCardinality.AtLeastOne)
			{
				if (combo.SelectionCount == 0)  // aucune sélection ?
				{
					combo.AddSelection (Enumerable.Range (0, 1));  // sélectionne le premier
				}
			}

			return combo;
		}


		public void CreateMargin(bool horizontalSeparator = false)
		{
			if (horizontalSeparator)
			{
				var separator = new Separator
				{
					Margins = new Margins (0, UIBuilder.RightMargin, 5, 5),
					PreferredHeight = 1,
				};

				this.Add (separator);
			}
			else
			{
				var frame = new FrameBox
				{
					PreferredHeight = 10,
				};

				this.Add (frame);
			}
		}


		public void CreateMargin(EditionTile tile, bool horizontalSeparator = false)
		{
			if (horizontalSeparator)
			{
				var separator = new Separator
				{
					Parent = tile.Container,
					Dock = DockStyle.Top,
					Margins = new Margins (0, UIBuilder.RightMargin, 5, 5),
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

		private Widgets.AutoCompleteTextField CreateAutoCompleteTextField(EditionTile tile, string label, System.Action<AbstractEntity> valueSetter, ReferenceController referenceController)
		{
			System.Diagnostics.Debug.Assert (referenceController != null, "ReferenceController may not be null");

			tile.AllowSelection = true;

			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Top,
					Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			var container = new FrameBox
			{
				Parent = tile.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			var editor = new Widgets.AutoCompleteTextField
			{
				Parent = container,
				MenuButtonWidth = UIBuilder.ComboButtonWidth-1,
				PreferredHeight = 20,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
				HintEditorComboMenu = Widgets.HintEditorComboMenu.IfReasonable,
				ComboMenuReasonableItemsLimit = 100,
				TabIndex = ++this.tabIndex,
			};

			var tabIndex1 = ++this.tabIndex;
			var tabIndex2 = ++this.tabIndex;

			//	Ce bouton vient tout à droite.
			var tileButton = new GlyphButton
			{
				Parent = container,
				PreferredWidth = UIBuilder.ComboButtonWidth,
				PreferredHeight = 20,
				Dock = DockStyle.Right,
				Margins = new Margins (3, 0, 0, 0),
				AutoFocus = false,
				TabIndex = tabIndex2,
			};

			//	Ce bouton vient juste après (et tout contre) la ligne éditable.
			var menuButton = new GlyphButton
			{
				Parent = container,
				ButtonStyle = Common.Widgets.ButtonStyle.Combo,
				GlyphShape = GlyphShape.Menu,
				PreferredWidth = UIBuilder.ComboButtonWidth,
				PreferredHeight = 20,
				Dock = DockStyle.Right,
				Margins = new Margins (-1, 0, 0, 0),
				AutoFocus = false,
				TabIndex = tabIndex1,
			};

			this.ContentListAdd (container);

			var changeHandler = UIBuilder.CreateAutoCompleteTextFieldChangeHandler (editor, tileButton, referenceController, createEnabled: referenceController.HasCreator);

			editor.SelectedItemChanged += sender => changeHandler ();
			editor.TextChanged         += sender => changeHandler ();

			changeHandler ();  // met à jour tout de suite le bouton '>/+' selon l'état actuel

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

			var controller = this.GetRootController ();

			var clickSimulator = new TileButtonClickSimulator (tileButton, controller, referenceController.Id);

			tileButton.Clicked +=
				delegate
				{
					editor.DefocusAndAcceptOrReject ();
					
					if (tileButton.GlyphShape == GlyphShape.ArrowRight)
					{
						tile.Controller = new ReferenceTileController (referenceController);
						tile.ToggleSubView (controller.Orchestrator, controller, new TileNavigationPathElement (clickSimulator.Name));
					}

					if (tileButton.GlyphShape == GlyphShape.Plus)
					{
						if (referenceController.HasCreator)
						{
							if (tile.IsSelected)
							{
								tile.CloseSubView (controller.Orchestrator);
							}
							else
							{
								var newValue  = referenceController.CreateNewValue (controller.DataContext);
								var newEntity = newValue.GetEditionEntity ();
								var refEntity = newValue.GetReferenceEntity ();
								var newController = EntityViewController.CreateEntityViewController ("Creation", newEntity, newValue.CreationControllerMode, controller.Orchestrator);
								tile.OpenSubView (controller.Orchestrator, controller, newController);
								editor.SelectedItemIndex = editor.Items.Add (refEntity);
								valueSetter (refEntity);

								new AutoCompleteItemSynchronizer (editor, newController, refEntity);
							}
						}
					}
				};

			return editor;
		}

		class TileButtonClickSimulator : IClickSimulator
		{
			public TileButtonClickSimulator(Widget tileButton, CoreViewController controller, string name)
			{
				this.name = name;
				this.button = tileButton;
				this.controller = controller;
				this.controller.Navigator.Register (this);
				this.controller.Disposing += sender => this.controller.Navigator.Unregister (this);
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			#region IClickSimulator Members

			bool IClickSimulator.SimulateClick(string name)
			{
				if (name == this.name)
				{
					this.button.SimulateClicked ();
					return true;
				}
				else
				{
					return false;
				}
			}

			#endregion

			private readonly string name;
			private readonly Widget button;
			private readonly CoreViewController controller;
		}

		private Widgets.ItemPicker CreateDetailedItemPicker(EditionTile tile, string label, BusinessLogic.EnumValueCardinality cardinality)
		{
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Top,
					Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				};

				this.ContentListAdd (staticText);
			}

			var widget = new Widgets.ItemPicker
			{
				Parent = tile.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 3),
				Cardinality = cardinality,
				TabIndex = ++this.tabIndex,
			};

			this.ContentListAdd (widget);

			return widget;
		}

		private void ContentListAdd(Widget widget)
		{
			if (this.contentList != null)
			{
				this.contentList.Add (widget);
			}
		}

		private CoreViewController GetRootController()
		{
			if (this.nextBuilder != null)
			{
				return this.nextBuilder.GetRootController ();
			}
			else
			{
				return this.controller;
			}
		}


		#region AutoCompleteItemSynchronizer Class

		class AutoCompleteItemSynchronizer
		{
			public AutoCompleteItemSynchronizer(AutoCompleteTextField field, EntityViewController controller, AbstractEntity entity)
			{
				this.field = field;
				this.controller = controller;
				this.entity = entity;

				this.entity.AttachListener ("*", this.HandleEntityChanged);
				this.controller.Disposing += this.HandleControllerDisposing;
			}

			private void HandleControllerDisposing(object sender)
			{
				this.entity.DetachListener ("*", this.HandleEntityChanged);
				this.field.RefreshTextBasedOnSelectedItem ();
			}

			private void HandleEntityChanged(object sender, DependencyPropertyChangedEventArgs e)
			{
				this.field.RefreshTextBasedOnSelectedItem ();
			}


			private readonly AutoCompleteTextField field;
			private readonly EntityViewController controller;
			private readonly IStructuredData entity;
		}

		#endregion

		#region ReferenceTileController Class

		private class ReferenceTileController : ITileController
		{
			public ReferenceTileController(ReferenceController referenceController)
			{
				this.referenceController = referenceController;
			}


			#region ITileController Members

			public EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator, NavigationPathElement navigationPathElement)
			{
				return this.referenceController.CreateSubViewController (orchestrator, navigationPathElement);
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

			private readonly ReferenceController referenceController;
		}

		#endregion

		
		
		private static System.Action CreateAutoCompleteTextFieldChangeHandler(Widgets.AutoCompleteTextField editor, GlyphButton showButton, ReferenceController referenceController, bool createEnabled)
		{
			return
				delegate
				{
					if (!editor.InError && editor.SelectedItemIndex >= 0)
					{
						showButton.GlyphShape = GlyphShape.ArrowRight;
						showButton.Enable = referenceController.Mode != ViewControllerMode.None;
					}
					else
					{
						if (createEnabled)
						{
							showButton.GlyphShape = GlyphShape.Plus;
							showButton.Enable = true;
						}
						else
						{
							showButton.GlyphShape = GlyphShape.ArrowRight;
							showButton.Enable = false;
						}
					}
				};
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
			if (!this.isDisposed)
			{
				UI.SetInitialFocus (this.Container);

				if (this.nextBuilder != null)
				{
					this.nextBuilder.tabIndex = this.tabIndex;
				}

				UIBuilder.current = this.nextBuilder;
				this.isDisposed = true;
			}
		}

		#endregion


		public static readonly double RightMargin				= 10;
		public static readonly double MarginUnderLabel			= 1;
		public static readonly double MarginUnderTextField		= 2;
		public static readonly double TinyButtonSize			= 19;  // doit être impair à cause de GlyphButton !
		public static readonly double ComboButtonWidth			= 14;

		private static UIBuilder current;

		private readonly CoreViewController controller;
		private readonly TileContainer container;
		private readonly UIBuilder nextBuilder;
		private bool isDisposed;
		private int tabIndex;
		private TitleTile titleTile;
		private PanelTitleTile panelTitleTile;
		private TileTabBook tileTabBook;
		private IList<Widget> contentList;
	}
}
