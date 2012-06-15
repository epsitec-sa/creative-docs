//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Behaviors;
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


		public bool ReadOnly
		{
			//	Détermine l'état de tous les widgets qui seront créés.
			get;
			set;
		}

		public void Add(Widget widget)
		{
			widget.Parent = this.Container;
			widget.TabIndex = ++this.tabIndex;
			widget.Dock = DockStyle.Stacked;

			this.ContentListAdd (widget);
		}

		public void Add(Widget container, Widget widget)
		{
			widget.Parent = container;
			widget.TabIndex = ++this.tabIndex;
			widget.Dock = DockStyle.Stacked;

			this.ContentListAdd (widget);
		}


		public ConfirmationButton CreateCreationButtonWithInitializer<T>(CreationViewController<T> controller, string title, string description, System.Action<BusinessContext, T> initializer = null)
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

		public ConfirmationButton CreateCreationButton<T, TDerived>(CreationViewController<T> controller, string title, string description, System.Action<BusinessContext, TDerived> initializer = null)
			where T : AbstractEntity, new ()
			where TDerived : T, new ()
		{
			var button = new ConfirmationButton
			{
				Text = ConfirmationButton.FormatContent (title, description),
				PreferredHeight = 52,
			};

			button.Clicked += delegate
			{
				controller.CreateRealEntity<TDerived> (initializer);
			};

			this.Add (button);

			return button;
		}

		
		public TitleTileWithVerticalLayout CreatePanelTitleTile(string iconUri, string title)
		{
			double bottomMargin = -1;

			this.panelTitleTile = new TitleTileWithVerticalLayout
			{
				Parent = this.Container,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, 0, 0, bottomMargin),
				IconUri = iconUri,
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
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, 0, 0, bottomMargin),
				IconUri = iconUri,
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

		public EditionTile CreateEditionTile(EditionTile parent)
		{
			var tile = new EditionTile
			{
				AutoHilite = false,
				IsReadOnly = false,
				TabIndex   = ++this.tabIndex,
				Parent     = parent,
				Dock       = DockStyle.Stacked,
			};

			this.ContentListAdd (tile);

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

			public EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator, CoreViewController parentController, NavigationPathElement navigationPathElement)
			{
				return EntityViewControllerFactory.Create (typeof (T).Name, this.entity, this.mode, orchestrator, parentController,
					navigationPathElement: this.navigationPathElement,
					controllerSubTypeId: this.controllerSubType);
			}

			#endregion

			private readonly T entity;
			private readonly TileNavigationPathElement navigationPathElement;
			private readonly int controllerSubType;
			private readonly ViewControllerMode mode;
		}




		public TileTabBook<T> CreateTabBook<T>(EditionTile tile, TabPageDef selectedPage, params TabPageDef<T>[] pageDescriptions)
		{
			var tileTabBook = new TileTabBook<T> (pageDescriptions)
			{
				Parent = tile.Container,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, 5),
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				TabIndex = ++this.tabIndex,
			};

			tileTabBook.SelectTabPage (selectedPage);

			return tileTabBook;
		}


		public void CreateHeaderEditorTile()
		{
			var controller = this.container.EntityViewController;

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
			var controller = this.container.EntityViewController;

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

			var controller   = this.container.EntityViewController;
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
				Margins = Widgets.Tiles.TileArrow.GetContainerPadding (Direction.Right) + new Margins (0, 2+offset1+offset2, 2, 0),
				Name = "DataContext#Debug",
				FormattedText = FormattedText.FromSimpleText (string.Format ("#{0}/{1}", controller.DataContext.UniqueId, businessContextId)).ApplyFontColor (Color.FromRgb (0.8, 0.8, 0.8)),
			};
		}


		public static FrameBox CreateMiniToolbar(Widget parent, double height=0)
		{
			var toolbar = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				BackColor = TileColors.SurfaceDefaultColors.First (),
				Padding = new Margins (2),
				Dock = DockStyle.Stacked,
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

			var controller   = container.EntityViewController;
			var orchestrator = controller.Orchestrator;

			closeButton = new GlyphButton
			{
				Parent = container,
				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = GlyphShape.Close,
				Anchor = AnchorStyles.TopRight,
				PreferredSize = new Size (18, 18),
				Margins = Widgets.Tiles.TileArrow.GetContainerPadding (Direction.Right) + new Margins (0, 2, 2, 0),
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
			var controller   = container.EntityViewController;
			var orchestrator = controller.Orchestrator;

			var lockButton = new StaticGlyph
			{
				Parent = container,
//-				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = GlyphShape.Lock,
				Anchor = AnchorStyles.TopRight,
				PreferredSize = new Size (20, 20),
				Margins = Widgets.Tiles.TileArrow.GetContainerPadding (Direction.Right) + new Margins (0, 2+offset, 2, 0),
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
					bool isLocked = lockMonitor.LockState == DataLayer.Infrastructure.LockState.Locked;
					
					lockButton.Visibility = isLocked;
					
					if (isLocked == false)
					{
						if (this.businessContext.ReloadEntities ())
						{
							this.businessContext.NotifyUIChanges ();
						}
					}
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
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 5, 5),
			};

			this.ContentListAdd (staticText);

			return staticText;
		}

		public StaticText CreateStaticText(EditionTile tile, string text)
		{
			var staticText = new StaticText
			{
				Parent = tile.Container,
				Text = text,
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split,
				PreferredHeight =  Misc.GetEstimatedHeight (new FormattedText (text)),
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, 0),
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
					Dock = DockStyle.Stacked,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			var frameBox = new FrameBox
			{
				Parent = tile.Container,
#if SLIMFIELD
				PreferredHeight = 10,
#else
				PreferredHeight = 20,
#endif
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, 0, 0, Library.UI.Constants.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			this.ContentListAdd (frameBox);

			return frameBox;
		}


		public Button CreateButton(EditionTile tile, double width, string label, string buttonText)
		{
			return this.CreateButton (tile.Container, width, label, buttonText);
		}

		public Button CreateButton(FrameBox parent, double width, string label, string buttonText)
		{
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = parent,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Stacked,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			return this.CreateButton (parent, DockStyle.Stacked, width, buttonText);
		}

		public Button CreateButton(FrameBox parent, DockStyle dockStyle, double width, string buttonText)
		{
			var button = new Button
			{
				Parent = parent,
				Text = buttonText,
				PreferredHeight = 20,
				Dock = dockStyle,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderTextField),
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
		public Widget CreateAccountEditor(EditionTile tile, string label, bool forceReadOnly, Marshaler marshaler)
		{
			//	Crée un widget AutoCompleteTextField permettant d'éditer un numéro de compte,
			//	selon le plan comptable en cours. Si le plan comptable n'existe pas, on crée
			//	une ligne éditable toute simple.

			//	Cherche le plan comptable en cours.
			var businessSettings = this.businessContext.GetCachedBusinessSettings ();
			var financeSettings  = businessSettings.Finance;

			System.Diagnostics.Debug.Assert (financeSettings != null);
			var chart = financeSettings.GetChartOfAccountsOrDefaultToNearest (this.businessContext.GetReferenceDate ());

			if (chart == null)  // aucun plan comptable trouvé ?
			{
				return this.CreateTextField (tile, 150, forceReadOnly, label, marshaler);  // crée une simple ligne éditable
			}

			//	Crée les widgets.
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Stacked,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			var container = new FrameBox
			{
				Parent = tile.Container,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			var editor = new Widgets.AutoCompleteTextFieldEx
			{
				Parent = container,
				IsReadOnly = this.ReadOnly || forceReadOnly,
				MenuButtonWidth = Library.UI.Constants.ComboButtonWidth-1,
				PreferredHeight = 20,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
				HintEditorMode = Widgets.HintEditorMode.DisplayMenuForSmallList,
				HintEditorSmallListLimit = 100,
				TabIndex = ++this.tabIndex,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
			};

			//	Ce bouton vient juste après (et tout contre) la ligne éditable.
			var menuButton = new GlyphButton
			{
				Parent = container,
				Enable = !this.ReadOnly && !forceReadOnly,
				ButtonStyle = Common.Widgets.ButtonStyle.Combo,
				GlyphShape = GlyphShape.Menu,
				PreferredWidth = Library.UI.Constants.ComboButtonWidth,
				PreferredHeight = 20,
				Dock = DockStyle.Right,
				Margins = new Margins (-1, 0, 0, 0),
				AutoFocus = false,
				TabIndex = ++this.tabIndex,
			};

			this.RegisterTextField (editor);
			this.ContentListAdd (container);

			foreach (var account in chart.Items)
			{
				editor.Items.Add (account);
			}

			editor.ValueToDescriptionConverter = value => UIBuilder.GetAccountText ((Business.Accounting.BookAccountDefinition) value);
			editor.HintComparer                = (value, text) => UIBuilder.MatchAccountText ((Business.Accounting.BookAccountDefinition) value, text);
			editor.HintComparisonConverter     = x => HintComparer.GetComparableText (x);

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

			var itemText = HintComparer.GetComparableText (UIBuilder.GetAccountText (account).ToSimpleText ());
			return HintComparer.Compare (itemText, userText);
		}

		private static void InitializeAccount(Widgets.AutoCompleteTextFieldEx editor, Marshaler marshaler)
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

		private static void UpdateAccount(Widgets.AutoCompleteTextFieldEx editor, Marshaler marshaler)
		{
			//	Met à jour le champ de l'entité (numéro de compte seul), en fonction de l'état du widget.
			if (string.IsNullOrEmpty (editor.Text) || editor.SelectedItemIndex == -1)
			{
				marshaler.SetStringValue ("");
			}
			else
			{
				var account = (Business.Accounting.BookAccountDefinition) editor.Items.GetValue (editor.SelectedItemIndex);
				marshaler.SetStringValue (account.AccountNumber);
			}
		}
		#endregion

#if SLIMFIELD
		public SlimField CreateSlimField(EditionTile tile, double width, bool forceReadOnly, Caption caption, Marshaler marshaler)
		{
			return this.CreateSlimField (tile.Container, DockStyle.Stacked, width, forceReadOnly, caption, marshaler);
		}

		public SlimField CreateSlimField(FrameBox parent, DockStyle dockStyle, double width, bool forceReadOnly, Caption caption, Marshaler marshaler, INamedType fieldType = null)
		{
			string value = marshaler.GetStringValue ();

			var slimField = SlimFieldBuilder.CreateSlimField (caption, string.IsNullOrEmpty (value) ? SlimFieldDisplayMode.Label : SlimFieldDisplayMode.Text);

			slimField.MaxWidth = 240;
			slimField.Parent = parent;
			slimField.IsReadOnly = this.ReadOnly || marshaler.IsReadOnly || forceReadOnly;
			slimField.Dock = dockStyle;
			slimField.FieldText = value;
			slimField.TabIndex = ++this.tabIndex;
			
			this.ContentListAdd (slimField);

			if (width > 0)
			{
				slimField.HorizontalAlignment = HorizontalAlignment.Left;
				slimField.PreferredWidth = width;
			}

			var behavior = new SlimFieldTextBehavior (slimField);
			
			this.RegisterSlimField (behavior);
			
			var valueController = new TextValueController (marshaler);
			valueController.FieldType = fieldType;

			behavior.TextEditionStarted += 
				delegate
				{
					valueController.Attach (behavior.GetTextField ());
				};

			behavior.TextEditionEnded +=
				delegate
				{
					marshaler.SetStringValue (slimField.FieldText);
					SlimFieldBuilder.SetFieldTexts (slimField, caption);
//-					behavior.GetTextField ().FormattedText = FormattedText.FromSimpleText (marshaler.GetStringValue ());
				};

			this.container.Add (valueController);

			return slimField;
		}

		public SlimField CreateSlimField<T>(EditionTile tile, Caption caption, bool forceReadOnly, SelectionController<T> controller)
			where T : AbstractEntity, new ()
		{
			var parent = tile.Container;
			var dockStyle = DockStyle.Stacked;
			var value = controller.ToFormattedTextConverter (controller.GetValue ()).ToSimpleText ();

			var slimField = SlimFieldBuilder.CreateSlimField (caption, string.IsNullOrEmpty (value) ? SlimFieldDisplayMode.Label : SlimFieldDisplayMode.Text);

			slimField.MaxWidth = 240;
			slimField.Parent = parent;
			slimField.Dock = dockStyle;
			slimField.IsReadOnly = this.ReadOnly || forceReadOnly;
			slimField.FieldText = value;
			slimField.TabIndex = ++this.tabIndex;

			foreach (var item in controller.GetPossibleItems ().Take (5))
			{
				var texts = item.GetTextValues ().Where (x => x.Category == TextValueCategory.Name);
				var array = new StringArray (texts.Select (x => x.SimpleText));

				if (array.IsEmpty)
				{
					continue;
				}

				slimField.MenuItems.Add (new SlimFieldMenuItem<T> (item, array));
			}

			slimField.MenuItems.Add (new SlimFieldMenuItem<T> (null, "plus…", style: SlimFieldMenuItemStyle.Extra));

//-			this.RegisterTextField (slimField);
			this.ContentListAdd (slimField);

			var behavior = new SlimFieldMenuBehavior (slimField);

			behavior.Selected +=
				delegate
				{
					var item = behavior.GetSelectedItem () as SlimFieldMenuItem<T>;
					controller.SetValue (item.Value);
				};

			return slimField;
		}
#endif		


		public TextFieldEx CreateTextField(EditionTile tile, double width, bool forceReadOnly, string label, Marshaler marshaler)
		{
			return this.CreateTextField (tile.Container, width, forceReadOnly, label, marshaler);
		}

		public TextFieldEx CreateTextField(FrameBox parent, double width, bool forceReadOnly, string label, Marshaler marshaler)
		{
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = parent,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Stacked,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			return this.CreateTextField (parent, DockStyle.Stacked, width, forceReadOnly, marshaler);
		}

		public TextFieldEx CreateTextField(Marshaler marshaler, INamedType fieldType = null)
		{
			return this.CreateTextField (null, DockStyle.None, 0, false,  marshaler, fieldType);
		}

		public TextFieldEx CreateTextField(FrameBox parent, DockStyle dockStyle, double width, bool forceReadOnly, Marshaler marshaler, INamedType fieldType = null)
		{
			var textField = new TextFieldEx
			{
				Parent = parent,
				IsReadOnly = this.ReadOnly || marshaler.IsReadOnly || forceReadOnly,
				PreferredHeight = 20,
				Dock = dockStyle,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderTextField),
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
			valueController.FieldType = fieldType;
			valueController.Attach (textField);
			this.container.Add (valueController);

			return textField;
		}

		public CheckButton CreateCheckButton(EditionTile tile, double width, bool forceReadOnly, string label, Marshaler marshaler)
		{
			return this.CreateCheckButton (tile.Container, DockStyle.Stacked, width, forceReadOnly, label, marshaler);
		}

		public CheckButton CreateCheckButton(FrameBox parent, DockStyle dockStyle, double width, bool forceReadOnly, string label, Marshaler marshaler)
		{
			var checkButton = new CheckButton
			{
				Parent = parent,
				Enable = !this.ReadOnly && !marshaler.IsReadOnly && !forceReadOnly,
				PreferredHeight = 20,
				Text = label ?? "???",
				Dock = dockStyle,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			this.RegisterCheckButton (checkButton);
			this.ContentListAdd (checkButton);

			if (width > 0)
			{
				checkButton.HorizontalAlignment = HorizontalAlignment.Left;
				checkButton.PreferredWidth = width;
			}

			var valueController = new TextValueController (marshaler);
			valueController.Attach (checkButton);
			this.container.Add (valueController);

			return checkButton;
		}

		public TextFieldMultiEx CreateTextFieldMulti(EditionTile tile, double height, bool forceReadOnly, string label, Marshaler marshaler)
		{
			return this.CreateTextFieldMulti (tile.Container, height, forceReadOnly, label, marshaler);
		}

		public TextFieldMultiEx CreateTextFieldMulti(FrameBox parent, double height, bool forceReadOnly, string label, Marshaler marshaler)
		{
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = parent,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Stacked,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			return this.CreateTextFieldMulti (parent, DockStyle.Stacked, height, forceReadOnly, marshaler);
		}

		public TextFieldMultiEx CreateTextFieldMulti(FrameBox container, DockStyle dockStyle, double height, bool forceReadOnly, Marshaler marshaler)
		{
			var textField = new TextFieldMultiEx
			{
				Parent = container,
				IsReadOnly = this.ReadOnly || forceReadOnly,
				PreferredHeight = height,
				Dock = dockStyle,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderTextField),
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

		private void RegisterSlimField(SlimFieldBehavior slimFieldBehavior)
		{
			if (this.businessContext != null && this.ReadOnly == false)
			{
				slimFieldBehavior.TextEditionStarting += (_, e) =>
				{
					if (!this.businessContext.AcquireLock ())
					{
						e.Cancel = true;
					}
				};
			}
		}

		private void RegisterTextField(AbstractTextField textField)
		{
			if (this.businessContext != null && this.ReadOnly == false)
			{
				textField.TextEdited += delegate
				{
					if (!this.businessContext.AcquireLock ())
					{
						textField.RejectEdition ();
					}
				};
			}
		}

		private void RegisterCheckButton(CheckButton button)
		{
			if (this.businessContext != null && this.ReadOnly == false)
			{
				button.ActiveStateChanged += delegate
				{
					if (!this.businessContext.AcquireLock ())
					{
						//	...
					}
				};
			}
		}


		public Widgets.AutoCompleteTextFieldEx CreateAutoCompleteTextField(EditionTile tile, double width, string label, Marshaler marshaler, IEnumerable<string[]> possibleItems, ValueToFormattedTextConverter<string[]> getUserText)
		{
			//	possibleItems[0] doit obligatoirement être la 'key' !
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Stacked,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			var container = new FrameBox
			{
				Parent = tile.Container,
				PreferredHeight = 20,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			Widgets.AutoCompleteTextFieldEx textField;
			GlyphButton menuButton;

			if (width == 0)
			{
				textField = new Widgets.AutoCompleteTextFieldEx
				{
					Parent = container,
					IsReadOnly = this.ReadOnly,
					MenuButtonWidth = Library.UI.Constants.ComboButtonWidth-1,
					PreferredHeight = 20,
					Dock = DockStyle.Fill,
					Margins = new Margins (0, 0, 0, 0),
					HintEditorMode = Widgets.HintEditorMode.DisplayMenu,
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
					PreferredWidth = Library.UI.Constants.ComboButtonWidth,
					PreferredHeight = 20,
					Dock = DockStyle.Right,
					Margins = new Margins (-1, 0, 0, 0),
					AutoFocus = false,
				};
			}
			else
			{
				textField = new Widgets.AutoCompleteTextFieldEx
				{
					Parent = container,
					IsReadOnly = this.ReadOnly,
					MenuButtonWidth = Library.UI.Constants.ComboButtonWidth-1,
					PreferredWidth = width,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, 0, 0),
					HintEditorMode = Widgets.HintEditorMode.DisplayMenu,
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
					PreferredWidth = Library.UI.Constants.ComboButtonWidth,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (-1, 0, 0, 0),
					AutoFocus = false,
				};
			}

			this.RegisterTextField (textField);
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

		public Widgets.AutoCompleteTextFieldEx CreateAutoCompleteTextField<T>(EditionTile tile, double width, bool forceReadOnly, string label, Marshaler marshaler, IEnumerable<EnumKeyValues<T>> possibleItems)
		{
			return this.CreateAutoCompleteTextField<T> (tile, width, forceReadOnly, label, marshaler, possibleItems, x => TextFormatter.FormatText (x));
		}

		private Widgets.AutoCompleteTextFieldEx CreateAutoCompleteTextField<T>(EditionTile tile, double width, bool forceReadOnly, string label, Marshaler marshaler, IEnumerable<EnumKeyValues<T>> possibleItems, ValueToFormattedTextConverter<EnumKeyValues<T>> getUserText)
		{
			//	possibleItems.Item1 est la 'key' !
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Stacked,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			var container = new FrameBox
			{
				Parent = tile.Container,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			Widgets.AutoCompleteTextFieldEx textField;
			GlyphButton menuButton;


			if (width == 0)
			{
				textField = new Widgets.AutoCompleteTextFieldEx
				{
					Parent = container,
					IsReadOnly = this.ReadOnly || forceReadOnly,
					MenuButtonWidth = Library.UI.Constants.ComboButtonWidth-1,
					PreferredHeight = 20,
					Dock = DockStyle.Fill,
					Margins = new Margins (0, 0, 0, 0),
					HintEditorMode = Widgets.HintEditorMode.DisplayMenu,
					TabIndex = ++this.tabIndex,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
				};

				menuButton = new GlyphButton
				{
					Parent = container,
					Enable = !this.ReadOnly && !forceReadOnly,
					ButtonStyle = Common.Widgets.ButtonStyle.Combo,
					GlyphShape = GlyphShape.Menu,
					PreferredWidth = Library.UI.Constants.ComboButtonWidth,
					PreferredHeight = 20,
					Dock = DockStyle.Right,
					Margins = new Margins (-1, 0, 0, 0),
					AutoFocus = false,
				};
			}
			else
			{
				textField = new Widgets.AutoCompleteTextFieldEx
				{
					Parent = container,
					IsReadOnly = this.ReadOnly || forceReadOnly,
					MenuButtonWidth = Library.UI.Constants.ComboButtonWidth-1,
					PreferredWidth = width,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (0, 0, 0, 0),
					HintEditorMode = Widgets.HintEditorMode.DisplayMenu,
					TabIndex = ++this.tabIndex,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
				};

				menuButton = new GlyphButton
				{
					Parent = container,
					Enable = !this.ReadOnly && !forceReadOnly,
					ButtonStyle = Common.Widgets.ButtonStyle.Combo,
					GlyphShape = GlyphShape.Menu,
					PreferredWidth = Library.UI.Constants.ComboButtonWidth,
					PreferredHeight = 20,
					Dock = DockStyle.Left,
					Margins = new Margins (-1, 0, 0, 0),
					AutoFocus = false,
				};
			}

			this.RegisterTextField (textField);
			this.ContentListAdd (container);

			menuButton.Clicked += delegate
			{
				textField.OpenComboMenu ();
			};

			var valueController = new EnumValueController<T> (marshaler, possibleItems, getUserText);
			valueController.Attach (textField);
			this.container.Add (valueController);

			return textField;
		}


		public Widgets.AutoCompleteTextFieldEx CreateAutoCompleteTextField<T>(string label, bool forceReadOnly, SelectionController<T> controller)
			where T : AbstractEntity, new ()
		{
			var tile = this.CreateEditionTile ();
			return this.CreateAutoCompleteTextField<T>(tile, label, forceReadOnly, controller);
		}

		public Widgets.AutoCompleteTextFieldEx CreateAutoCompleteTextField<T>(EditionTile tile, string label, bool forceReadOnly, SelectionController<T> controller)
			where T : AbstractEntity, new ()
		{
			var autoCompleteTextField = this.CreateAutoCompleteTextField (tile, label, forceReadOnly, x => controller.SetValue (x as T), controller.ReferenceController);

			controller.Attach (autoCompleteTextField);
			this.container.Add (controller);

			return autoCompleteTextField;
		}

		public Widgets.AutoCompleteTextFieldEx CreateCompactAutoCompleteTextField<T>(FrameBox frame, string label, SelectionController<T> controller)
			where T : AbstractEntity, new ()
		{
			var autoCompleteTextField = this.CreateCompactAutoCompleteTextField (frame, label);

			controller.Attach (autoCompleteTextField);
			this.container.Add (controller);

			return autoCompleteTextField;
		}

		public Widget CreateEditionDetailedItemPicker<T1, T2>(EditionTile tile, string name, T1 entity, string label, SelectionController<T2> controller, EnumValueCardinality cardinality, bool forceReadOnly, ViewControllerMode mode = ViewControllerMode.Summary, int controllerSubType = -1)
			where T1 : AbstractEntity
			where T2 : AbstractEntity, new ()
		{
			Widget widget;
			GlyphButton tileButton;
			GlyphButton dialogButton = null;
			
			// TODO Should we also use the this.IsReadonly property to know whether this widget
			// should be readonly or not ?

			if (controller.GetPossibleItems().Count () <= 5)  // limite arbitraire !	@PA
			{
				//	S'il y a 5 choix ou moins, on utilise un ItemPicker, qui crée des CheckButtons.
				var picker = this.CreateDetailedItemPickerButtons (tile, label, cardinality, out tileButton);
				
				picker.Enable = !forceReadOnly;

				controller.Attach (picker);

				if (cardinality == EnumValueCardinality.ExactlyOne ||
					cardinality == EnumValueCardinality.AtLeastOne)
				{
					if (picker.SelectionCount == 0)  // aucune sélection ?
					{
						picker.AddSelection (Enumerable.Range (0, 1));  // sélectionne le premier
					}
				}

				widget = picker;
			}
			else
			{
				//	S'il y a plus de 5 choix, on utilise un ItemPickerCombo, qui crée un TextFieldCombo
				//	qui occupera une place très réduite.
				var picker = this.CreateDetailedItemPickerCombo (tile, label, cardinality, out tileButton, out dialogButton);

				picker.IsReadOnly = forceReadOnly;

				controller.Attach (picker);

				if (cardinality == EnumValueCardinality.ExactlyOne ||
					cardinality == EnumValueCardinality.AtLeastOne)
				{
					if (picker.SelectionCount == 0)  // aucune sélection ?
					{
						picker.AddSelection (Enumerable.Range (0, 1));  // sélectionne le premier
					}
				}

				widget = picker;
			}

			tileButton.Entered += delegate
			{
				tile.Hilite = true;
			};

			tileButton.Exited += delegate
			{
				tile.Hilite = false;
			};

			tile.Selected += delegate
			{
				tileButton.GlyphShape = GlyphShape.ArrowLeft;
			};

			tile.Deselected += delegate
			{
				tileButton.GlyphShape = GlyphShape.ArrowRight;
			};

			var rootController = this.GetRootController ();
			var fullName       = string.Format (System.Globalization.CultureInfo.InstalledUICulture, "{0}", name);
			var clickSimulator = new TileButtonClickSimulator (tileButton, this.controller, fullName);

			tile.Controller = new SummaryTileController<T1> (entity, fullName, mode, controllerSubType);

			if (dialogButton != null)
			{
				dialogButton.Clicked += delegate
				{
					using (var dialog = new Dialogs.ItemPickerDialog<T2> (this.businessContext.Data.Host, controller, cardinality, label))
					{
						dialog.OpenDialog ();

						if (dialog.Result == Common.Dialogs.DialogResult.Accept)
						{
							controller.RefreshSelection ();
						}
					}
				};
			}

			tileButton.Clicked += delegate
			{
				tile.ToggleSubView (rootController.Orchestrator, rootController, new TileNavigationPathElement (clickSimulator.Name));
			};

			return widget;
		}


		public void CreateMargin(bool horizontalSeparator = false)
		{
			if (horizontalSeparator)
			{
				var separator = new Separator
				{
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 5, 5),
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
					Dock = DockStyle.Stacked,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 5, 5),
					PreferredHeight = 1,
				};

				this.ContentListAdd (separator);
			}
			else
			{
				var frame = new FrameBox
				{
					Parent = tile.Container,
					Dock = DockStyle.Stacked,
					PreferredHeight = 10,
				};

				this.ContentListAdd (frame);
			}
		}

		private Widgets.AutoCompleteTextFieldEx CreateCompactAutoCompleteTextField(FrameBox parent, string label)
		{
			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = parent,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Dock = DockStyle.Stacked,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			var container = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			var editor = new Widgets.AutoCompleteTextFieldEx
			{
				Parent = container,
				IsReadOnly = this.ReadOnly,
				MenuButtonWidth = Library.UI.Constants.ComboButtonWidth-1,
				PreferredHeight = 20,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
				HintEditorMode = Widgets.HintEditorMode.DisplayMenuForSmallList,
				HintEditorSmallListLimit = 100,
				TabIndex = ++this.tabIndex,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
			};

			//	Ce bouton vient juste après (et tout contre) la ligne éditable.
			var pullDownButton = new GlyphButton
			{
				Parent = container,
				Enable = !this.ReadOnly,
				ButtonStyle = Common.Widgets.ButtonStyle.Combo,
				GlyphShape = GlyphShape.Menu,
				PreferredWidth = Library.UI.Constants.ComboButtonWidth,
				PreferredHeight = 20,
				Dock = DockStyle.Right,
				Margins = new Margins (-1, 0, 0, 0),
				AutoFocus = false,
				TabIndex = ++this.tabIndex,
			};

			pullDownButton.Clicked += delegate
			{
				editor.SelectAll ();
				editor.Focus ();
				editor.OpenComboMenu ();
			};

			this.RegisterTextField (editor);
			this.ContentListAdd (container);

			return editor;
		}

		private Widgets.AutoCompleteTextFieldEx CreateAutoCompleteTextField(EditionTile tile, string label, bool forceReadOnly, System.Action<AbstractEntity> valueSetter, ReferenceController referenceController)
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
					Dock = DockStyle.Stacked,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderLabel),
				};

				this.ContentListAdd (staticText);
			}

			var container = new FrameBox
			{
				Parent = tile.Container,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			var editor = new Widgets.AutoCompleteTextFieldEx
			{
				Parent = container,
				IsReadOnly = this.ReadOnly || forceReadOnly,
				MenuButtonWidth = Library.UI.Constants.ComboButtonWidth-1,
				PreferredHeight = 20,
				Dock = DockStyle.Fill,
				HintEditorMode = Widgets.HintEditorMode.DisplayMenuForSmallList,
				HintEditorSmallListLimit = 100,
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
				PreferredWidth = Library.UI.Constants.ComboButtonWidth,
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
				Enable = !this.ReadOnly && !forceReadOnly,
				ButtonStyle = Common.Widgets.ButtonStyle.Combo,
				GlyphShape = GlyphShape.Menu,
				PreferredWidth = Library.UI.Constants.ComboButtonWidth,
				PreferredHeight = 20,
				Dock = DockStyle.Right,
				Margins = new Margins (-1, 0, 0, 0),
				AutoFocus = false,
				TabIndex = tabIndex1,
			};

			this.RegisterTextField (editor);
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
				tile.Hilite = true;
			};

			tileButton.Exited += delegate
			{
				tile.Hilite = false;
			};

			tile.Selected += delegate
			{
				tileButton.GlyphShape = GlyphShape.ArrowLeft;
			};

			tile.Deselected += delegate
			{
				tileButton.GlyphShape = GlyphShape.ArrowRight;
			};

			var controller = this.GetRootController ();
			var clickSimulator = new TileButtonClickSimulator (tileButton, controller, referenceController.Id);

			tileButton.Clicked += delegate
			{
				editor.DefocusAndAcceptOrReject ();

				var navPath = new TileNavigationPathElement (clickSimulator.Name);

				switch (tileButton.GlyphShape)
				{
					case GlyphShape.ArrowRight:
					case GlyphShape.ArrowLeft:
						tile.Controller = new ReferenceTileController (referenceController);
						tile.ToggleSubView (controller.Orchestrator, controller, navPath);
						break;
				
					case GlyphShape.Plus:
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
								var newController = EntityViewControllerFactory.Create ("Creation", newEntity, newValue.CreationControllerMode, controller.Orchestrator, controller, navigationPathElement: navPath);
								tile.OpenSubView (controller.Orchestrator, controller, newController);
								editor.SelectedItemIndex = editor.Items.Add (refEntity);
								valueSetter (refEntity);

								new AutoCompleteItemSynchronizer (editor, newController, refEntity);
							}
						}
						break;
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

		private ItemPickerButtons CreateDetailedItemPickerButtons(EditionTile tile, string label, EnumValueCardinality cardinality, out GlyphButton tileButton)
		{
			tile.AllowSelection = true;

			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 2, Library.UI.Constants.MarginUnderTextField),
					Dock = DockStyle.Stacked,
				};

				this.ContentListAdd (staticText);
			}

			tileButton = new GlyphButton
			{
				Parent = tile.Container,
				GlyphShape = Common.Widgets.GlyphShape.ArrowRight,
				PreferredWidth = Library.UI.Constants.ComboButtonWidth,
				PreferredHeight = 20,
				Anchor = AnchorStyles.TopRight,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, 0),
				AutoFocus = false,
				TabIndex = ++this.tabIndex,
			};

			var widget = new Widgets.ItemPickerButtons
			{
				Parent = tile.Container,
				Enable = !this.ReadOnly,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, 0, 0, 3),
				Cardinality = cardinality,
				TabIndex = ++this.tabIndex,
			};

			this.ContentListAdd (widget);
			this.AttachPreProcessingLockAcquisition (widget);

			return widget;
		}

		private ItemPickerCombo CreateDetailedItemPickerCombo(EditionTile tile, string label, EnumValueCardinality cardinality, out GlyphButton tileButton, out GlyphButton dialogButton)
		{
			tile.AllowSelection = true;

			if (!string.IsNullOrEmpty (label))
			{
				var staticText = new StaticText
				{
					Parent = tile.Container,
					Text = string.Concat (label, " :"),
					TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
					Margins = new Margins (0, Library.UI.Constants.RightMargin, 2, Library.UI.Constants.MarginUnderTextField),
					Dock = DockStyle.Stacked,
				};

				this.ContentListAdd (staticText);
			}

			var container = new FrameBox
			{
				Parent = tile.Container,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, Library.UI.Constants.MarginUnderTextField),
				TabIndex = ++this.tabIndex,
			};

			var widget = new ItemPickerCombo
			{
				Parent = container,
				MenuButtonWidth = Library.UI.Constants.ComboButtonWidth-1,
				Enable = !this.ReadOnly,
				PreferredHeight = 20,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
				Cardinality = cardinality,
				TabIndex = ++this.tabIndex,
			};

			var tabIndex1 = ++this.tabIndex;
			var tabIndex2 = ++this.tabIndex;

			//	Ce bouton vient tout à droite.
			tileButton = new GlyphButton
			{
				Parent = container,
				GlyphShape = Common.Widgets.GlyphShape.ArrowRight,
				PreferredWidth = Library.UI.Constants.ComboButtonWidth,
				PreferredHeight = 20,
				Dock = DockStyle.Right,
				Margins = new Margins (3, 0, 0, 0),
				AutoFocus = false,
				TabIndex = tabIndex2,
			};

			//	Ce bouton vient juste après (et tout contre) la ligne éditable.
			dialogButton = new GlyphButton
			{
				Parent = container,
				Enable = !this.ReadOnly,
				ButtonStyle = Common.Widgets.ButtonStyle.Combo,
				GlyphShape = GlyphShape.Dots,
				PreferredWidth = Library.UI.Constants.ComboButtonWidth,
				PreferredHeight = 20,
				Dock = DockStyle.Right,
				Margins = new Margins (-1, 0, 0, 0),
				AutoFocus = false,
				TabIndex = tabIndex1,
			};

			this.ContentListAdd (widget);
			this.AttachPreProcessingLockAcquisition (widget);

			return widget;
		}

		private void ContentListAdd(Widget widget)
		{
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
			public AutoCompleteItemSynchronizer(AutoCompleteTextFieldEx field, EntityViewController controller, AbstractEntity entity)
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


			private readonly AutoCompleteTextFieldEx field;
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

			public EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator, CoreViewController parentController, NavigationPathElement navigationPathElement)
			{
				return this.referenceController.CreateSubViewController (orchestrator, parentController, navigationPathElement);
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

		
		
		private static System.Action CreateAutoCompleteTextFieldChangeHandler(Widgets.AutoCompleteTextFieldEx editor, GlyphButton showButton, ReferenceController referenceController, bool createEnabled)
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
						summaryTile.ToggleSubView (controller.Orchestrator, controller);
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
					if (this.tabIndex > 0)
					{
						if (this.Container.ContainsKeyboardFocus)
						{
							//	The container already contains the keyboard focus: there is no need to set
							//	the initial focus; this would only remove the focus from where it already
							//	is...
						}
						else
						{
							Library.UI.Services.SetInitialFocus (this.Container);
						}

						if (this.nextBuilder != null)
						{
							this.nextBuilder.tabIndex = this.tabIndex;
						}
					}

					UIBuilder.current = this.nextBuilder;
					this.isDisposed = true;
				}

				System.GC.SuppressFinalize (this);
			}
		}

		#endregion


		[System.ThreadStatic]
		private static UIBuilder				current;

		private readonly CoreViewController		controller;
		private readonly TileContainer			container;
		private readonly BusinessContext		businessContext;
		private readonly UIBuilder				nextBuilder;

		private bool							isDisposed;
		private int								tabIndex;
		private int								recursionCount;
		private TitleTile						titleTile;
		private TitleTileWithVerticalLayout		panelTitleTile;
	}
}
