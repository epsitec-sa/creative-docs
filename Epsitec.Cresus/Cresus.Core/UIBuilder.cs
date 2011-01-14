﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core
{
	public sealed class UIBuilder : System.IDisposable
	{
		public UIBuilder(EntityViewController controller)
			: this (controller.TileContainer, controller)
		{
		}

		~UIBuilder()
		{
			throw new System.InvalidOperationException ("UIBuilder must be used within a 'using' block or properly disposed by calling 'Dispose'");
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

			this.businessContext = this.controller.BusinessContext;

			if (this.businessContext != null)
			{
				if (this.businessContext.AreAllLocksAvailable ())
				{
					//	All locks for this business context are still available : read/write mode
				}
				else
				{
					this.ReadOnly = true;
				}
			}

			UIBuilder.current = this;
		}

		public static UIBuilder Create(EntityViewController controller)
		{
			if (UIBuilder.current == null)
			{
				return new UIBuilder (controller);
			}
			else
			{
				UIBuilder.current.recursionCount++;
				return UIBuilder.current;
			}
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


		public bool ReadOnly
		{
			//	Détermine l'état de tous les widgets qui seront créés.
			get;
			set;
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



		public ConfirmationButton CreateCreationButton<T>(CreationViewController<T> controller, string title, string description, System.Action<BusinessContext, T> initializer = null)
			where T : AbstractEntity, new ()
		{
			var button = new ConfirmationButton
			{
				Text = ConfirmationButton.FormatContent (title, description),
				PreferredHeight = 52,
			};

			button.Clicked += delegate
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

		public SummaryTile CreateSummaryTile<T>(string name, T entity, FormattedText summary, ViewControllerMode mode = ViewControllerMode.Summary, int controllerSubType = -1)
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

			tile.Controller = new SummaryTileController<T> (entity, fullName, mode, controllerSubType);
			tile.Summary    = summary.ToString ();

			return tile;
		}

		class SummaryTileController<T> : ITileController
			where T : AbstractEntity
		{
			public SummaryTileController(T entity, string name, ViewControllerMode mode, int controllerSubType)
			{
				this.entity = entity;
				this.navigationPathElement = new TileNavigationPathElement (name);
				this.mode = mode;
				this.controllerSubType = controllerSubType;
			}
			
			#region ITileController Members

			public EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator, NavigationPathElement navigationPathElement)
			{
				return EntityViewControllerFactory.Create (typeof (T).Name, this.entity, this.mode, orchestrator,
					navigationPathElement: this.navigationPathElement,
					controllerSubTypeId: this.controllerSubType);
			}

			#endregion

			private readonly T entity;
			private readonly TileNavigationPathElement navigationPathElement;
			private readonly int controllerSubType;
			private readonly ViewControllerMode mode;
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

#if false
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

				tilePage.Clicked += delegate
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
				list.Add (new TabPageDef (name, text, () => action (name)));
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
#endif


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

			if (controller.Mode == ViewControllerMode.Edition)
			{
				UIBuilder.CreateColumnTileCloseButton (this.container);
			}

			this.CreateColumnTileLockButton (this.container);

			this.CreateDataContextDebugInfo ();
		}

		private void CreateDataContextDebugInfo()
		{
			var offset1 = UIBuilder.ContainsWidget (this.container, "ColumnTileCloseButton") ? 18 : 0;
			var offset2 = UIBuilder.ContainsWidget (this.container, "ColumnTileLockButton")  ? 18 : 0;

			var controller   = this.container.Controller;
			var orchestrator = controller.Orchestrator;

			string businessContextId = "-";

			if (this.businessContext != null)
			{
				businessContextId = this.businessContext.UniqueId.ToString ();
			}

			var label = new StaticText
			{
				Parent = container,
				Anchor = AnchorStyles.TopRight,
				PreferredSize = new Size (40, 18),
				Margins = new Margins (0, Widgets.TileArrow.Breadth+2+offset1+offset2, 2, 0),
				Name = "DataContext#Debug",
				FormattedText = FormattedText.FromSimpleText (string.Format ("#{0}/{1}", controller.DataContext.UniqueId, businessContextId)),
			};
		}


		public static FrameBox CreateMiniToolbar(Widget parent, double height=0)
		{
			var toolbar = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = ArrowedFrame.SurfaceColors.First (),
				Padding = new Margins (2),
				Dock = DockStyle.Top,
			};

			if (height != 0)
			{
				toolbar.PreferredHeight = height;
			}

			return toolbar;
		}


		public static Button CreateColumnTileCloseButton(TileContainer container)
		{
			Button closeButton = container.FindChild ("ColumnTileCloseButton", WidgetChildFindMode.Deep) as Button;

			if (closeButton != null)
			{
				return closeButton;
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

			closeButton.Clicked += delegate
			{
				orchestrator.CloseView (controller);
			};

			return closeButton;
		}
		
		public Widget CreateColumnTileLockButton(TileContainer container)
		{
			var offset = UIBuilder.ContainsWidget (container, "ColumnTileCloseButton") ? 18 : 0;
			var controller   = container.Controller;
			var orchestrator = controller.Orchestrator;

			var lockButton = new StaticGlyph
			{
				Parent = container,
//-				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = GlyphShape.Lock,
				Anchor = AnchorStyles.TopRight,
				PreferredSize = new Size (20, 20),
				Margins = new Margins (0, Widgets.TileArrow.Breadth+2 + offset, 2, 0),
				Name = "ColumnTileLockButton",
				Visibility = this.ReadOnly
			};

			//	TODO: faire en sorte que lorsque la fiche est verrouillée par un tiers, cela soit
			//	indiqué visuellement avec un petit verrou en haut à droite.

			//	TODO: rendre tous les champs read-only du moment que le verrou est actif...

			this.AttachLockMonitor (lockButton);

			return lockButton;
		}

		private void AttachLockMonitor(StaticGlyph lockButton)
		{
			var lockMonitor = this.businessContext.CreateLockMonitor ();

			//	Make sure the lock monitor will not be garbage collected by tying it to the
			//	button itself in the UI :
			
			lockButton.SetValue (Data.LockMonitor.LockMonitorProperty, lockMonitor);
			lockMonitor.LockStateChanged +=
				delegate
				{
					lockButton.Visibility = lockMonitor.LockState == DataLayer.Infrastructure.LockState.Locked;
				};
		}

		private static bool ContainsWidget(TileContainer container, string columnTileCloseButton)
		{
			return container.FindChild (columnTileCloseButton, WidgetChildFindMode.Deep) != null;
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


		public FrameBox CreateGroup(EditionTile tile, string label = null)
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

			this.AttachPreProcessingLockAcquisition (button);
			
			return button;
		}


		private void AttachPreProcessingLockAcquisition(Widget widget)
		{
			widget.PreProcessing += this.HandleWidgetPreProcessingLockAcquisition;
		}

		private void HandleWidgetPreProcessingLockAcquisition(object sender, MessageEventArgs e)
		{
			//	If the event might produce some action (click or key press), first make
			//	sure that we may write to the entities stored in the business context
			//	(i.e. acquire the lock). If not, cancel the user event.

			if ((e.Message.MessageType == MessageType.MouseDown) ||
				(e.Message.MessageType == MessageType.MouseUp) ||
				(e.Message.MessageType == MessageType.KeyDown) ||
				(e.Message.MessageType == MessageType.KeyUp))
			{
				if (this.businessContext.AcquireLock () == false)
				{
					//	TODO: make the whole UI read-only ...

					e.Cancel = true;
				}
			}
		}


		#region Account editor
		public Widget CreateAccountEditor(EditionTile tile, string label, Marshaler marshaler)
		{
			//	Crée un widget AutoCompleteTextField permettant d'éditer un numéro de compte,
			//	selon le plan comptable en cours. Si le plan comptable n'existe pas, on crée
			//	une ligne éditable toute simple.

			//	Cherche le plan comptable en cours.
			var financeSettings = CoreProgram.Application.BusinessSettings.Finance;
			System.Diagnostics.Debug.Assert (financeSettings != null);
			var chart = financeSettings.GetRecentChartOfAccounts (this.businessContext);

			if (chart == null)  // aucun plan comptable trouvé ?
			{
				return this.CreateTextField (tile, 150, label, marshaler);  // crée une simple ligne éditable
			}

			//	Crée les widgets.
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
				IsReadOnly = this.ReadOnly,
				MenuButtonWidth = UIBuilder.ComboButtonWidth-1,
				PreferredHeight = 20,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
				HintEditorComboMenu = Widgets.HintEditorComboMenu.IfReasonable,
				ComboMenuReasonableItemsLimit = 100,
				TabIndex = ++this.tabIndex,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
			};

			//	Ce bouton vient juste après (et tout contre) la ligne éditable.
			var menuButton = new GlyphButton
			{
				Parent = container,
				Enable = !this.ReadOnly,
				ButtonStyle = Common.Widgets.ButtonStyle.Combo,
				GlyphShape = GlyphShape.Menu,
				PreferredWidth = UIBuilder.ComboButtonWidth,
				PreferredHeight = 20,
				Dock = DockStyle.Right,
				Margins = new Margins (-1, 0, 0, 0),
				AutoFocus = false,
				TabIndex = ++this.tabIndex,
			};

			this.ContentListAdd (container);

			foreach (var account in chart.Items)
			{
				editor.Items.Add (account);
			}

			editor.ValueToDescriptionConverter = value => UIBuilder.GetAccountText ((Business.Accounting.BookAccountDefinition) value);
			editor.HintComparer                = (value, text) => UIBuilder.MatchAccountText ((Business.Accounting.BookAccountDefinition) value, text);
			editor.HintComparisonConverter     = x => TextConverter.ConvertToLowerAndStripAccents (x);

			UIBuilder.InitializeAccount (editor, marshaler);

			editor.EditionAccepted += delegate
			{
				UIBuilder.UpdateAccount (editor, marshaler);
			};

			menuButton.Clicked += delegate
			{
				editor.SelectAll ();
				editor.Focus ();
				editor.OpenComboMenu ();
			};

			return editor;
		}

		private static FormattedText GetAccountText(Business.Accounting.BookAccountDefinition account)
		{
			//	Retourne le texte complet à utiliser pour un compte donné.
			return TextFormatter.FormatText (account.AccountNumber, account.Caption);
		}

		private static HintComparerResult MatchAccountText(Business.Accounting.BookAccountDefinition account, string userText)
		{
			//	Compare un compte avec le texte partiel entré par l'utilisateur.
			if (string.IsNullOrWhiteSpace (userText))
			{
				return Widgets.HintComparerResult.NoMatch;
			}

			var itemText = TextConverter.ConvertToLowerAndStripAccents (UIBuilder.GetAccountText (account).ToSimpleText ());
			return AutoCompleteTextField.Compare (itemText, userText);
		}

		private static void InitializeAccount(Widgets.AutoCompleteTextField editor, Marshaler marshaler)
		{
			//	Initialise le texte complet du widget, en fonction du numéro de compte stocké dans le champ de l'entité.
			FormattedText value = marshaler.GetStringValue();

			foreach (var item in editor.Items)
			{
				var account = (Business.Accounting.BookAccountDefinition) item;

				if (account.AccountNumber == value)
				{
					value = UIBuilder.GetAccountText (account);
					break;
				}
			}

			editor.FormattedText = value;
		}

		private static void UpdateAccount(Widgets.AutoCompleteTextField editor, Marshaler marshaler)
		{
			//	Met à jour le champ de l'entité (numéro de compte seul), en fonction de l'état du widget.
			if (string.IsNullOrEmpty (editor.Text) || editor.SelectedItemIndex == -1)
			{
				marshaler.SetStringValue (null);
			}
			else
			{
				var account = (Business.Accounting.BookAccountDefinition) editor.Items.GetValue (editor.SelectedItemIndex);
				marshaler.SetStringValue (account.AccountNumber);
			}
		}
		#endregion


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
				IsReadOnly = this.ReadOnly || marshaler.IsReadOnly,
				PreferredHeight = 20,
				Dock = dockStyle,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
				DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
			};

			this.RegisterTextField (textField);
			this.ContentListAdd (textField);

			if (width > 0)
			{
				textField.HorizontalAlignment = HorizontalAlignment.Left;
				textField.PreferredWidth = width;
			}

			var valueController = new TextValueController (marshaler);
			valueController.Attach (textField);
			this.container.Add (valueController);

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
				IsReadOnly = this.ReadOnly,
				PreferredHeight = height,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 0, UIBuilder.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
				DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
				ScrollerVisibility = false,
				PreferredLayout = TextFieldMultiExPreferredLayout.PreserveScrollerHeight,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
			};

			this.RegisterTextField (textField);
			this.ContentListAdd (textField);

			var valueController = new TextValueController (marshaler);
			valueController.Attach (textField);
			this.container.Add (valueController);

			return textField;
		}

		private void RegisterTextField(AbstractTextField textField)
		{
			if ((this.businessContext != null) &&
				(this.ReadOnly == false))
			{
				textField.TextEdited +=
					delegate
					{
						if (this.businessContext.AcquireLock ())
						{
						}
						else
						{
							textField.RejectEdition ();
						}
					};
			}
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
					IsReadOnly = this.ReadOnly,
					MenuButtonWidth = UIBuilder.ComboButtonWidth-1,
					PreferredHeight = 20,
					Dock = DockStyle.Fill,
					Margins = new Margins (0, 0, 0, 0),
					HintEditorComboMenu = Widgets.HintEditorComboMenu.Always,
					TabIndex = ++this.tabIndex,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
				};

				menuButton = new GlyphButton
				{
					Parent = container,
					Enable = !this.ReadOnly,
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
					IsReadOnly = this.ReadOnly,
					MenuButtonWidth = UIBuilder.ComboButtonWidth-1,
					PreferredWidth = width,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, 0, 0),
					HintEditorComboMenu = Widgets.HintEditorComboMenu.Always,
					TabIndex = ++this.tabIndex,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
				};

				menuButton = new GlyphButton
				{
					Parent = container,
					Enable = !this.ReadOnly,
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

			menuButton.Clicked += delegate
			{
				textField.SelectAll ();
				textField.Focus ();
				textField.OpenComboMenu ();
			};

			var valueController = new TextValueController (marshaler, possibleItems, getUserText);
			valueController.Attach (textField);
			this.container.Add (valueController);

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
					IsReadOnly = this.ReadOnly,
					MenuButtonWidth = UIBuilder.ComboButtonWidth-1,
					PreferredHeight = 20,
					Dock = DockStyle.Fill,
					Margins = new Margins (0, 0, 0, 0),
					HintEditorComboMenu = Widgets.HintEditorComboMenu.Always,
					TabIndex = ++this.tabIndex,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
				};

				menuButton = new GlyphButton
				{
					Parent = container,
					Enable = !this.ReadOnly,
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
					IsReadOnly = this.ReadOnly,
					MenuButtonWidth = UIBuilder.ComboButtonWidth-1,
					PreferredWidth = width,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, 0, 0),
					HintEditorComboMenu = Widgets.HintEditorComboMenu.Always,
					TabIndex = ++this.tabIndex,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
				};

				menuButton = new GlyphButton
				{
					Parent = container,
					Enable = !this.ReadOnly,
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

			menuButton.Clicked += delegate
			{
				textField.SelectAll ();
				textField.Focus ();
				textField.OpenComboMenu ();
			};

			var valueController = new EnumValueController<T> (marshaler, possibleItems, getUserText);
			valueController.Attach (textField);
			this.container.Add (valueController);

			return textField;
		}


		public Widgets.AutoCompleteTextField CreateAutoCompleteTextField<T>(string label, SelectionController<T> controller)
			where T : AbstractEntity, new ()
		{
			var tile = this.CreateEditionTile ();

			var autoCompleteTextField = this.CreateAutoCompleteTextField (tile, label, x => controller.SetValue (x as T), controller.ReferenceController);

			controller.Attach (autoCompleteTextField);
			this.container.Add (controller);

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
				Enable = !this.ReadOnly,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 3),
				TabIndex = ++this.tabIndex,
			};

			this.ContentListAdd (widget);

			controller.Attach (widget);

			return widget;
		}


		public Widgets.ItemPicker CreateEditionDetailedItemPicker<T1, T2>(string name, T1 entity, string label, SelectionController<T2> controller, Business.EnumValueCardinality cardinality, ViewControllerMode mode = ViewControllerMode.Summary, int controllerSubType = -1)
			where T1 : AbstractEntity
			where T2 : AbstractEntity, new ()
		{
			var tile = this.CreateEditionTile ();

			Button tileButton;
			var picker = this.CreateDetailedItemPicker (tile, label, cardinality, true, out tileButton);

			controller.Attach (picker);

			if (cardinality == Business.EnumValueCardinality.ExactlyOne ||
				cardinality == Business.EnumValueCardinality.AtLeastOne)
			{
				if (picker.SelectionCount == 0)  // aucune sélection ?
				{
					picker.AddSelection (Enumerable.Range (0, 1));  // sélectionne le premier
				}
			}

			tileButton.Entered += delegate
			{
				tile.TileArrowHilite = true;
			};

			tileButton.Exited += delegate
			{
				tile.TileArrowHilite = false;
			};

			var rootController = this.GetRootController ();
			var fullName = string.Format (System.Globalization.CultureInfo.InstalledUICulture, "{0}.{1}", name, this.titleTile.Items.Count);
			var clickSimulator = new TileButtonClickSimulator (tileButton, this.controller, fullName);

			tile.Controller = new SummaryTileController<T1> (entity, fullName, mode, controllerSubType);

			tileButton.Clicked += delegate
			{
				tile.ToggleSubView (rootController.Orchestrator, rootController, new TileNavigationPathElement (clickSimulator.Name));
			};

			return picker;
		}

		public Button CreateButtonOpeningSubviewController<T>(string name, FormattedText buttonText, T entity, ViewControllerMode mode = ViewControllerMode.Summary, int controllerSubType = -1)
			where T : AbstractEntity
		{
			var tile = this.CreateEditionTile ();

			Button tileButton = this.CreateButton (tile, 0, null, buttonText.ToString ());

			tileButton.Entered += delegate
			{
				tile.TileArrowHilite = true;
			};

			tileButton.Exited += delegate
			{
				tile.TileArrowHilite = false;
			};

			var rootController = this.GetRootController ();
			var fullName = string.Format (System.Globalization.CultureInfo.InstalledUICulture, "{0}.{1}", name, this.titleTile.Items.Count);
			var clickSimulator = new TileButtonClickSimulator (tileButton, this.controller, fullName);

			tile.Controller = new SummaryTileController<T> (entity, fullName, mode, controllerSubType);

			tileButton.Clicked += delegate
			{
				tile.ToggleSubView (rootController.Orchestrator, rootController, new TileNavigationPathElement (clickSimulator.Name));
			};

			return tileButton;
		}

		public Widgets.ItemPicker CreateEditionDetailedItemPicker<T>(string label, SelectionController<T> controller, Business.EnumValueCardinality cardinality)
			where T : AbstractEntity, new ()
		{
			var tile = this.CreateEditionTile ();

			Button tileButton;
			var picker = this.CreateDetailedItemPicker (tile, label, cardinality, false, out tileButton);

			controller.Attach (picker);

			if (cardinality == Business.EnumValueCardinality.ExactlyOne ||
				cardinality == Business.EnumValueCardinality.AtLeastOne)
			{
				if (picker.SelectionCount == 0)  // aucune sélection ?
				{
					picker.AddSelection (Enumerable.Range (0, 1));  // sélectionne le premier
				}
			}

			return picker;
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
				IsReadOnly = this.ReadOnly,
				MenuButtonWidth = UIBuilder.ComboButtonWidth-1,
				PreferredHeight = 20,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
				HintEditorComboMenu = Widgets.HintEditorComboMenu.IfReasonable,
				ComboMenuReasonableItemsLimit = 100,
				TabIndex = ++this.tabIndex,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
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
				Enable = !this.ReadOnly,
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

			menuButton.Clicked += delegate
			{
				editor.SelectAll ();
				editor.Focus ();
				editor.OpenComboMenu ();
			};

			tileButton.Entered += delegate
			{
				tile.TileArrowHilite = true;
			};

			tileButton.Exited += delegate
			{
				tile.TileArrowHilite = false;
			};

			var controller = this.GetRootController ();
			var clickSimulator = new TileButtonClickSimulator (tileButton, controller, referenceController.Id);

			tileButton.Clicked += delegate
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
							var newController = EntityViewControllerFactory.Create ("Creation", newEntity, newValue.CreationControllerMode, controller.Orchestrator);
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

		private Widgets.ItemPicker CreateDetailedItemPicker(EditionTile tile, string label, Business.EnumValueCardinality cardinality, bool createTileButton, out Button tileButton)
		{
			tile.AllowSelection = true;

			var header = new FrameBox
			{
				Parent = tile.Container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, UIBuilder.RightMargin, 2, UIBuilder.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = header,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Fill,
				};

				this.ContentListAdd (staticText);
			}

			if (createTileButton)
			{
				tileButton = new GlyphButton
				{
					Parent = header,
					GlyphShape = Common.Widgets.GlyphShape.ArrowRight,
					PreferredWidth = UIBuilder.ComboButtonWidth,
					PreferredHeight = 20,
					Dock = DockStyle.Right,
					Margins = new Margins (3, 0, 0, 0),
					AutoFocus = false,
					TabIndex = ++this.tabIndex,
				};
			}
			else
			{
				tileButton = null;
			}

			var widget = new Widgets.ItemPicker
			{
				Parent = tile.Container,
				Enable = !this.ReadOnly,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 3),
				Cardinality = cardinality,
				TabIndex = ++this.tabIndex,
			};

			this.ContentListAdd (widget);
			this.AttachPreProcessingLockAcquisition (widget);

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
			titleTile.Clicked += delegate
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
			widget.TextChanged += delegate
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
			if (this.recursionCount > 0)
			{
				this.recursionCount--;
			}
			else
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

				System.GC.SuppressFinalize (this);
			}
		}

		#endregion


		public static readonly double RightMargin				= 10;
		public static readonly double MarginUnderLabel			= 1;
		public static readonly double MarginUnderTextField		= 2;
		public static readonly double TinyButtonSize			= 19;  // doit être impair à cause de GlyphButton !
		public static readonly double ComboButtonWidth			= 14;

		[System.ThreadStatic]
		private static UIBuilder				current;

		private readonly CoreViewController controller;
		private readonly TileContainer container;
		private readonly BusinessContext businessContext;
		private readonly UIBuilder nextBuilder;
		private bool isDisposed;
		private int tabIndex;
		private int recursionCount;
		private TitleTile titleTile;
		private PanelTitleTile panelTitleTile;
		private TileTabBook tileTabBook;
		private IList<Widget> contentList;
	}
}
