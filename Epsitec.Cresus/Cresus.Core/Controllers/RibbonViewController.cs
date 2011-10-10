//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Features;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>RibbonViewController</c> manages the ribbon of the main application window.
	/// </summary>
	public sealed class RibbonViewController : ViewControllerComponent<RibbonViewController>
	{
		private RibbonViewController(DataViewOrchestrator orchestrator)
			: base (orchestrator)
		{
			var app            = orchestrator.Host;
			var coreData       = app.FindComponent<CoreData> ();
			var featureManager = app.FindComponent<FeatureManager> ();
			var userManager    = coreData.GetComponent<UserManager> ();

			userManager.AuthenticatedUserChanged += this.HandleAuthenticatedUserChanged;

			this.authenticateUserButtons = new List<IconOrImageButton> ();
			this.authenticateUserWidgets = new List<StaticText> ();
			this.ribbonShowPages = new Dictionary<string, bool> ();

			this.commandDispatcher  = app.CommandDispatcher;
			this.coreCommandDispatcher = app.GetComponent<CoreCommandDispatcher> ();
			this.persistenceManager = app.PersistenceManager;
			this.userManager        = userManager;
			this.featureManager     = featureManager;

			this.featureManager.EnableGodMode ();
		}

		
		public string							DatabaseMenuDefaultCommandName
		{
			get
			{
				if (string.IsNullOrEmpty (this.databaseMenuDefaultCommandName))
				{
					return "";
				}
				else
				{
					return this.databaseMenuDefaultCommandName;
				}
			}
			set
			{
				if (this.databaseMenuDefaultCommandName != value)
				{
					this.databaseMenuDefaultCommandName = value;
					this.OnDatabaseMenuDefaultCommandNameChanged ();
				}
			}
		}

		
		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			base.CreateUI (container);
#if false
			this.CreateRibbonBook (container);
			this.CreateRibbonHomePage ();
#else
			this.CreateRibbon (container);
#endif
		}


		#region New style ribbon
		private void CreateRibbon(Widget container)
		{
			var frame = new FrameBox
			{
				Parent = container,
				BackColor = Color.FromBrightness (1),
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Fill,
				Name = "Ribbon",
				Margins = new Margins (-1, -1, 0, 0),
			};

			var separator = new Separator
			{
				Parent = container,
				PreferredHeight = 1,
				IsHorizontalLine = true,
				Dock = DockStyle.Bottom,
			};

			//	|-->
			{
				var section = this.CreateNewStyleSection (frame, DockStyle.Left, "Navigation");

				section.Children.Add (this.CreateButton (Library.Res.Commands.History.NavigateBackward));
				section.Children.Add (this.CreateButton (Library.Res.Commands.History.NavigateForward));
			}

			{
				var section = this.CreateNewStyleSection (frame, DockStyle.Left, "Validation");

				section.Children.Add (this.CreateButton (Library.Res.Commands.Edition.SaveRecord));
				section.Children.Add (this.CreateButton (Library.Res.Commands.Edition.DiscardRecord));
			}

			{
				var section = this.CreateNewStyleSection (frame, DockStyle.Left, "Edition");

				Widget topSection, bottomSection;
				this.CreateNewStyleSubsections (section, out topSection, out bottomSection);

				topSection.Children.Add (this.CreateButton (ApplicationCommands.Bold, large: false, isActivable: true));
				topSection.Children.Add (this.CreateButton (ApplicationCommands.Italic, large: false, isActivable: true));
				bottomSection.Children.Add (this.CreateButton (ApplicationCommands.Underlined, large: false, isActivable: true));
				bottomSection.Children.Add (this.CreateButton (ApplicationCommands.MultilingualEdition, large: false));

				this.CreateNewStyleLanguage (section);
			}

			{
				var section = this.CreateNewStyleSection (frame, DockStyle.Left, "Presse-papier");

				Widget topSection, bottomSection;
				this.CreateNewStyleSubsections (section, out topSection, out bottomSection);

				topSection.Children.Add (this.CreateButton (ApplicationCommands.Cut, large: false));
				bottomSection.Children.Add (this.CreateButton (ApplicationCommands.Copy, large: false));

				section.Children.Add (this.CreateButton (ApplicationCommands.Paste));
			}

			{
				var section = this.CreateNewStyleSection (frame, DockStyle.Left, "Actions");

				section.Children.Add (this.CreateButton (Res.Commands.Edition.Print));

				Widget topSection, bottomSection;
				this.CreateNewStyleSubsections (section, out topSection, out bottomSection);

				topSection.Children.Add (this.CreateButton (Res.Commands.File.ImportV11, large: false));
				bottomSection.Children.Add (this.CreateButton (Res.Commands.Feedback, large: false));
			}

			{
				var section = this.CreateNewStyleSection (frame, DockStyle.Left, "Bases de données");

				section.Children.Add (this.CreateButton (Res.Commands.Base.ShowCustomer));
				section.Children.Add (this.CreateButton (Res.Commands.Base.ShowArticleDefinition));
				section.Children.Add (this.CreateButton (Res.Commands.Base.ShowDocumentMetadata));

				this.CreateNewStyleRibbonDatabaseSectionMenuButton (section);
			}

			//	<-->
			{
				var section = this.CreateNewStyleSection (frame, DockStyle.Fill, "Affaire");

				// TODO: C'est un moyen bricolé pour que WorkflowController sache où placer ses boutons.
				WorkflowController.SetWorkflowButtonsContainer (section);
			}

			//	<--|
			{
				var section = this.CreateNewStyleSection (frame, DockStyle.Right, "Réglages");

				Widget topSection, bottomSection;
				this.CreateNewStyleSubsections (section, out topSection, out bottomSection);

				topSection.Children.Add (this.CreateButton (Res.Commands.Global.ShowSettings, large: false));
				bottomSection.Children.Add (this.CreateButton (Res.Commands.Global.ShowDebug, large: false));

				this.CreateNewStyleRibbonUserSection (section);

				var space = new FrameBox  // espace pour le bouton 'v'
				{
					PreferredWidth = 6,  // chevauchement partiel volontaire
					Dock = DockStyle.StackBegin,
				};
				section.Children.Add (space);
			}

			//	Bouton 'v'
			var showRibbonButton = new GlyphButton
			{
				Parent = container.Window.Root,
				Anchor = AnchorStyles.TopRight,
				PreferredSize = new Size (14, 14),
				Margins = new Margins (0, -1, -1, 0),
				GlyphShape = GlyphShape.TriangleUp,
				ButtonStyle = ButtonStyle.Icon,
			};

			ToolTip.Default.SetToolTip (showRibbonButton, "Montre ou cache la barre d'icônes");

			showRibbonButton.Clicked += delegate
			{
				container.Visibility = !container.Visibility;
				showRibbonButton.GlyphShape = container.Visibility ? GlyphShape.TriangleUp : GlyphShape.TriangleDown;
			};
		}

		private Widget CreateNewStyleSection(Widget frame, DockStyle dockStyle, FormattedText description)
		{
			double leftMargin  = (dockStyle == DockStyle.Right) ? 2 : 0;
			double rightMargin = (dockStyle == DockStyle.Left ) ? 2 : 0;

			var section = new FrameBox
			{
				Parent = frame,
				DrawFullFrame = true,
				BackColor = Color.FromHexa ("ebe9ed"),
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth = 10,
				Dock = dockStyle,
				Margins = new Margins (leftMargin, rightMargin, -1, -1),
			};

			var top = new FrameBox
			{
				Parent = section,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = 10,
				Dock = DockStyle.Fill,
				Padding = new Margins (2, 2, 2, 1),
			};

			new StaticText
			{
				Parent = section,
				FormattedText = description.ApplyFontSize (8.0).ApplyFontColor (Color.FromBrightness (1.0)).ApplyBold (),
				ContentAlignment = ContentAlignment.MiddleCenter,
				TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				BackColor = Color.FromHexa ("a3b3c7"),
				PreferredWidth = 10,
				PreferredHeight = 11,
				Dock = DockStyle.Bottom,
				Margins = new Margins (1, 1, 0, 1),
			};

			return top;
		}

		private void CreateNewStyleSubsections(Widget section, out Widget topSection, out Widget bottomSection)
		{
			var frame = new FrameBox
			{
				Parent = section,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth = 10,
				Dock = DockStyle.StackBegin,
			};

			topSection = new FrameBox
			{
				Parent = frame,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = 10,
				Dock = DockStyle.Top,
			};

			bottomSection = new FrameBox
			{
				Parent = frame,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = 10,
				Dock = DockStyle.Bottom,
			};
		}

		private void CreateNewStyleLanguage(Widget section)
		{
			var frame1 = new FrameBox
			{
				Parent = section,
				Dock = DockStyle.StackBegin,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth = 21,
			};

			var frame2 = new FrameBox
			{
				Parent = section,
				Dock = DockStyle.StackBegin,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth = 21,
			};

			//	HACK: faire cela proprement avec des commandes multi-états

			var selectLanaguage1 = new IconButton ()
			{
				Parent = frame1,
				Name = "language=fr",
				PreferredSize = new Size (Library.UI.Constants.ButtonLargeWidth/2, Library.UI.Constants.ButtonLargeWidth/2),
				Dock = DockStyle.Stacked,
				Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagFR.icon""/>",
				ActiveState = ActiveState.Yes,
			};

			var selectLanaguage2 = new IconButton ()
			{
				Parent = frame1,
				Name = "language=de",
				PreferredSize = new Size (Library.UI.Constants.ButtonLargeWidth/2, Library.UI.Constants.ButtonLargeWidth/2),
				Dock = DockStyle.Stacked,
				Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagDE.icon""/>",
			};

			var selectLanaguage3 = new IconButton ()
			{
				Parent = frame2,
				Name = "language=en",
				PreferredSize = new Size (Library.UI.Constants.ButtonLargeWidth/2, Library.UI.Constants.ButtonLargeWidth/2),
				Dock = DockStyle.Stacked,
				Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagGB.icon""/>",
			};

			var selectLanaguage4 = new IconButton ()
			{
				Parent = frame2,
				Name = "language=it",
				PreferredSize = new Size (Library.UI.Constants.ButtonLargeWidth/2, Library.UI.Constants.ButtonLargeWidth/2),
				Dock = DockStyle.Stacked,
				Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagIT.icon""/>",
			};

			Library.UI.Services.Settings.CultureForData.SelectLanguage ("fr");
			Library.UI.Services.Settings.CultureForData.DefineDefaultLanguage ("fr");

			selectLanaguage1.Clicked += delegate
			{
				Library.UI.Services.Settings.CultureForData.SelectLanguage ("fr");
				selectLanaguage1.ActiveState = ActiveState.Yes;
				selectLanaguage2.ActiveState = ActiveState.No;
				selectLanaguage3.ActiveState = ActiveState.No;
				selectLanaguage4.ActiveState = ActiveState.No;
			};

			selectLanaguage2.Clicked += delegate
			{
				Library.UI.Services.Settings.CultureForData.SelectLanguage ("de");
				selectLanaguage1.ActiveState = ActiveState.No;
				selectLanaguage2.ActiveState = ActiveState.Yes;
				selectLanaguage3.ActiveState = ActiveState.No;
				selectLanaguage4.ActiveState = ActiveState.No;
			};

			selectLanaguage3.Clicked += delegate
			{
				Library.UI.Services.Settings.CultureForData.SelectLanguage ("en");
				selectLanaguage1.ActiveState = ActiveState.No;
				selectLanaguage2.ActiveState = ActiveState.No;
				selectLanaguage3.ActiveState = ActiveState.Yes;
				selectLanaguage4.ActiveState = ActiveState.No;
			};

			selectLanaguage4.Clicked += delegate
			{
				Library.UI.Services.Settings.CultureForData.SelectLanguage ("it");
				selectLanaguage1.ActiveState = ActiveState.No;
				selectLanaguage2.ActiveState = ActiveState.No;
				selectLanaguage3.ActiveState = ActiveState.No;
				selectLanaguage4.ActiveState = ActiveState.Yes;
			};
		}

		private void CreateNewStyleRibbonDatabaseSectionMenuButton(Widget section)
		{
			//	Place le bouton 'magique' qui donne accès aux bases de données d'usage moins fréquent.
			this.databaseButton = this.CreateButton ();
			section.Children.Add (this.databaseButton);

			this.databaseMenuButton = new GlyphButton ()
			{
				ButtonStyle = ButtonStyle.ComboItem,
				GlyphShape = GlyphShape.Menu,
				AutoFocus = false,
				PreferredSize = new Size (13, Library.UI.Constants.ButtonLargeWidth),
				Dock = DockStyle.StackBegin,
				Margins = new Margins (-1, 0, 0, 0),
			};
			section.Children.Add (this.databaseMenuButton);

			ToolTip.Default.SetToolTip (this.databaseMenuButton, "Montre une autre base de données...");

			this.databaseMenuButton.Clicked += delegate
			{
				this.ShowDatabaseSelectionMenu (this.databaseButton);
			};

			this.databaseMenuButton.Entered += delegate
			{
				this.databaseButton.ButtonStyle = ButtonStyle.Combo;
			};

			this.databaseMenuButton.Exited += delegate
			{
				this.databaseButton.ButtonStyle = ButtonStyle.ToolItem;
			};

			var databaseCommandHandler = this.GetDatabaseCommandHandler ();

			databaseCommandHandler.Changed += delegate
			{
				this.UpdateDatabaseButton ();
			};

			this.UpdateDatabaseMenu ();
		}

		private void CreateNewStyleRibbonUserSection(Widget section)
		{
			var authenticateUserButton = RibbonViewController.CreateIconOrImageButton (Res.Commands.Global.ShowUserManager);
			authenticateUserButton.CoreData = this.Orchestrator.Data;
			authenticateUserButton.IconUri = Misc.GetResourceIconUri ("UserManager");
			authenticateUserButton.IconPreferredSize = new Size (31, 31);

			section.Children.Add (authenticateUserButton);

			//	Le widget 'authenticateUserWidget' déborde volontairement sur le bas du bouton 'ShowUserManager',
			//	pour permettre d'afficher un nom d'utilisateur lisible.
			var authenticateUserWidget = new StaticText
			{
				Parent = authenticateUserButton,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				PreferredHeight = 14,
				Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.authenticateUserButtons.Add (authenticateUserButton);
			this.authenticateUserWidgets.Add (authenticateUserWidget);
		}
		#endregion


		private void UpdateAuthenticatedUser()
		{
			//	Met à jour le nom de l'utilisateur dans le ruban.
			var user = this.userManager.AuthenticatedUser;

			for (int i = 0; i < this.authenticateUserButtons.Count; i++)
			{
				var authenticateUserButton = this.authenticateUserButtons[i];
				var authenticateUserWidget = this.authenticateUserWidgets[i];

				if (user.IsNull ())
				{
					authenticateUserButton.ImageEntity = null;
					authenticateUserWidget.Text = null;

					ToolTip.Default.HideToolTipForWidget (authenticateUserButton);
					ToolTip.Default.HideToolTipForWidget (authenticateUserWidget);
				}
				else
				{
#if false
					throw new System.NotImplementedException ();
					this.authenticateUserButton.ImageEntity = user.Person.Pictures.FirstOrDefault ();
#endif
					FormattedText text = user.LoginName;
					authenticateUserWidget.FormattedText = text.ApplyFontSize (9.0);

					ToolTip.Default.SetToolTip (authenticateUserButton, user.ShortDescription);
					ToolTip.Default.SetToolTip (authenticateUserWidget, user.ShortDescription);
				}
			}

			this.UpdateDatabaseMenu ();
		}


		private void RibbonShowPage(string name, bool visibility)
		{
			this.ribbonShowPages[name] = visibility;

			//	Montre le ruban qui semble le plus utile.
			//	En fait, les rubans sont prioritaires, dans l'ordre Business, Workflow et finalement Home.
			if (this.ribbonShowPages.ContainsKey ("Business") && this.ribbonShowPages["Business"] == true)
			{
				this.ribbonBook.ActivePage = this.ribbonPageBusiness;
			}
			else if (this.ribbonShowPages.ContainsKey ("Workflow") && this.ribbonShowPages["Workflow"] == true)
			{
				this.ribbonBook.ActivePage = this.ribbonPageWorkflow;
			}
			else
			{
				this.ribbonBook.ActivePage = this.ribbonPageHome;
			}
		}

		private void CreateRibbonBook(Widget container)
		{
			this.ribbonBook = new RibbonBook ()
			{
				Parent = container,
				Dock = DockStyle.Fill,
				Name = "Ribbon",
			};

#if false
			this.persistenceManager.Register (this.ribbonBook);
			this.persistenceManager.Register (this, this.ribbonBook.FullPathName + ".DatabaseMenu.DefaultCommand",
				x => this.DatabaseMenuDefaultCommandNameChanged += x,
				x => this.DatabaseMenuDefaultCommandNameChanged -= x,
				xml => xml.Add (new XAttribute ("name", this.DatabaseMenuDefaultCommandName)),
				xml => this.DatabaseMenuDefaultCommandName = xml.Attribute ("name").Value);
#endif
		}

		private void CreateRibbonHomePage()
		{
			this.ribbonPageHome     = RibbonViewController.CreateRibbonPage (this.ribbonBook, "Home",     "Accueil");
			this.ribbonPageMisc     = RibbonViewController.CreateRibbonPage (this.ribbonBook, "Misc",     "Compléments");
			this.ribbonPageWorkflow = RibbonViewController.CreateRibbonPage (this.ribbonBook, "Workflow", "Flux");
			this.ribbonPageBusiness = RibbonViewController.CreateRibbonPage (this.ribbonBook, "Business", "Document commercial");

			this.ribbonBook.ActivePage = this.ribbonPageHome;

			//	Home ribbon:
			this.CreateRibbonValidateSection (this.ribbonPageHome);  // communs
			this.CreateRibbonEditSection (this.ribbonPageHome);  // communs
			this.CreateRibbonClipboardSection (this.ribbonPageHome);
			this.CreateRibbonPrintSection (this.ribbonPageHome);
			this.CreateRibbonDatabaseSection (this.ribbonPageHome);
			this.CreateRibbonNavigationSection (this.ribbonPageHome);

			this.CreateRibbonUserSection (this.ribbonPageHome);  // communs

			//	Workflow ribbon:
			this.CreateRibbonValidateSection (this.ribbonPageWorkflow);  // communs
			this.CreateRibbonEditSection (this.ribbonPageWorkflow);  // communs
			this.CreateRibbonClipboardSection (this.ribbonPageWorkflow);
			this.CreateRibbonPrintSection (this.ribbonPageWorkflow);
			this.CreateRibbonWorkflowContainerSection (this.ribbonPageWorkflow);

			this.CreateRibbonUserSection (this.ribbonPageWorkflow);  // communs

			//	Business ribbon:
			this.CreateRibbonValidateSection (this.ribbonPageBusiness);  // communs
			this.CreateRibbonEditSection (this.ribbonPageBusiness);  // communs

			this.CreateRibbonBusinessCreateSection (this.ribbonPageBusiness);
			this.CreateRibbonBusinessOperSection (this.ribbonPageBusiness);
			this.CreateRibbonBusinessGroupSection (this.ribbonPageBusiness);

			this.CreateRibbonUserSection (this.ribbonPageBusiness);  // communs

			//	Misc ribbon:
			this.CreateRibbonValidateSection (this.ribbonPageMisc);  // communs
			this.CreateRibbonEditSection (this.ribbonPageMisc);  // communs
			this.CreateRibbonClipboardSection (this.ribbonPageMisc);
			this.CreateRibbonPrintSection (this.ribbonPageMisc);

			this.CreateRibbonMiscActionSection (this.ribbonPageMisc);

			this.CreateRibbonUserSection (this.ribbonPageMisc);  // communs
			this.CreateRibbonSettingsSection (this.ribbonPageMisc);
		}

		private static RibbonPage CreateRibbonPage(RibbonBook book, string name, string title)
		{
			return new RibbonPage (book)
			{
				Name = name,
				RibbonTitle = title,
				PreferredHeight = 78,
			};
		}

		#region Create home ribbon sections
		private void CreateRibbonUserSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "User",
				Title = "Identité",
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 1,
				Dock = DockStyle.Right,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			{
				var frame = new FrameBox
				{
					Parent = section,
					Dock = DockStyle.StackBegin,
					ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
					PreferredWidth = Library.UI.Constants.ButtonLargeWidth,
					PreferredHeight = 52,
				};

				var authenticateUserButton = RibbonViewController.CreateIconOrImageButton (Res.Commands.Global.ShowUserManager);
				authenticateUserButton.CoreData = this.Orchestrator.Data;
				authenticateUserButton.IconUri = Misc.GetResourceIconUri ("UserManager");
				authenticateUserButton.IconPreferredSize = new Size (31, 31);

				frame.Children.Add (authenticateUserButton);

				//	Le widget 'authenticateUserWidget' déborde volontairement sur le bas du bouton 'ShowUserManager',
				//	pour permettre d'afficher un nom d'utilisateur lisible.
				var authenticateUserWidget = new StaticText
				{
					Parent = frame,
					ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
					PreferredHeight = 14,
					Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom,
					Margins = new Margins (0, 0, 0, 0),
				};

				this.authenticateUserButtons.Add (authenticateUserButton);
				this.authenticateUserWidgets.Add (authenticateUserWidget);
			}
		}

		private void CreateRibbonValidateSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Validate",
				Title = "Validation",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 2,
			};

			section.Children.Add (this.CreateButton (Library.Res.Commands.Edition.SaveRecord));
			section.Children.Add (this.CreateButton (Library.Res.Commands.Edition.DiscardRecord));
		}

		private void CreateRibbonPrintSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Print",
				Title = "Impression",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 1,
			};

			section.Children.Add (this.CreateButton (Res.Commands.Edition.Print));
			//?section.Children.Add (this.CreateButton (Res.Commands.Edition.Preview));
		}

		private void CreateRibbonMiscActionSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Action",
				Title = "Actions",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 3,
			};

			section.Children.Add (this.CreateButton (Res.Commands.File.ImportV11));
			section.Children.Add (this.CreateButton (Res.Commands.Feedback));
		}

		private void CreateRibbonClipboardSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Clipboard",
				Title = "Presse-papier",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = Library.UI.Constants.ButtonSmallWidth + Library.UI.Constants.ButtonLargeWidth,
			};

			var frame = new FrameBox
			{
				Parent = section,
				Dock = DockStyle.StackBegin,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth = Library.UI.Constants.ButtonSmallWidth,
			};

			frame.Children.Add (this.CreateButton (ApplicationCommands.Cut,  large: false));
			frame.Children.Add (this.CreateButton (ApplicationCommands.Copy, large: false));

			section.Children.Add (this.CreateButton (ApplicationCommands.Paste));
		}

		private void CreateRibbonEditSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Edit",
				Title = "Edition",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			var frame = new FrameBox
			{
				Parent = section,
				Dock = DockStyle.StackBegin,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth = section.PreferredWidth,
			};

			var topFrame = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.StackBegin,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = section.PreferredWidth,
			};

			var bottomFrame = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.StackBegin,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = section.PreferredWidth,
			};

			topFrame.Children.Add (this.CreateButton (ApplicationCommands.Bold,       large: false, isActivable: true));
			topFrame.Children.Add (this.CreateButton (ApplicationCommands.Italic,     large: false, isActivable: true));
			topFrame.Children.Add (this.CreateButton (ApplicationCommands.Underlined, large: false, isActivable: true));

			//?bottomFrame.Children.Add (this.CreateButton (ApplicationCommands.Subscript,   large: false, isActivable: true));
			//?bottomFrame.Children.Add (this.CreateButton (ApplicationCommands.Superscript, large: false, isActivable: true));

			{
				var frame1 = new FrameBox
				{
					Parent = section,
					Dock = DockStyle.StackBegin,
					ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
					PreferredWidth = 21,
				};

				var frame2 = new FrameBox
				{
					Parent = section,
					Dock = DockStyle.StackBegin,
					ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
					PreferredWidth = 21,
				};

				//	HACK: faire cela proprement avec des commandes multi-états

				var selectLanaguage1 = new IconButton ()
				{
					Parent = frame1,
					Name = "language=fr",
					Dock = DockStyle.Stacked,
					Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagFR.icon""/>",
					ActiveState = ActiveState.Yes,
				};

				var selectLanaguage2 = new IconButton ()
				{
					Parent = frame1,
					Name = "language=de",
					Dock = DockStyle.Stacked,
					Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagDE.icon""/>",
				};

				var selectLanaguage3 = new IconButton ()
				{
					Parent = frame2,
					Name = "language=en",
					Dock = DockStyle.Stacked,
					Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagGB.icon""/>",
				};

				var selectLanaguage4 = new IconButton ()
				{
					Parent = frame2,
					Name = "language=it",
					Dock = DockStyle.Stacked,
					Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagIT.icon""/>",
				};

				Library.UI.Services.Settings.CultureForData.SelectLanguage ("fr");
				Library.UI.Services.Settings.CultureForData.DefineDefaultLanguage ("fr");

				selectLanaguage1.Clicked += delegate
				{
					Library.UI.Services.Settings.CultureForData.SelectLanguage ("fr");
					selectLanaguage1.ActiveState = ActiveState.Yes;
					selectLanaguage2.ActiveState = ActiveState.No;
					selectLanaguage3.ActiveState = ActiveState.No;
					selectLanaguage4.ActiveState = ActiveState.No;
				};

				selectLanaguage2.Clicked += delegate
				{
					Library.UI.Services.Settings.CultureForData.SelectLanguage ("de");
					selectLanaguage1.ActiveState = ActiveState.No;
					selectLanaguage2.ActiveState = ActiveState.Yes;
					selectLanaguage3.ActiveState = ActiveState.No;
					selectLanaguage4.ActiveState = ActiveState.No;
				};

				selectLanaguage3.Clicked += delegate
				{
					Library.UI.Services.Settings.CultureForData.SelectLanguage ("en");
					selectLanaguage1.ActiveState = ActiveState.No;
					selectLanaguage2.ActiveState = ActiveState.No;
					selectLanaguage3.ActiveState = ActiveState.Yes;
					selectLanaguage4.ActiveState = ActiveState.No;
				};

				selectLanaguage4.Clicked += delegate
				{
					Library.UI.Services.Settings.CultureForData.SelectLanguage ("it");
					selectLanaguage1.ActiveState = ActiveState.No;
					selectLanaguage2.ActiveState = ActiveState.No;
					selectLanaguage3.ActiveState = ActiveState.No;
					selectLanaguage4.ActiveState = ActiveState.Yes;
				};
			}

			section.Children.Add (this.CreateButton (ApplicationCommands.MultilingualEdition));
		}

		private void CreateRibbonDatabaseSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Database",
				Title = "Bases de données",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Left,
			};

			//	Place les boutons pour les bases de données les plus courantes.
			section.Children.Add (this.CreateButton (Res.Commands.Base.ShowCustomer));
			section.Children.Add (this.CreateButton (Res.Commands.Base.ShowArticleDefinition));
			section.Children.Add (this.CreateButton (Res.Commands.Base.ShowDocumentMetadata));

			this.CreateRibbonDatabaseSectionMenuButton (section);
		}

		private void CreateRibbonDatabaseSectionMenuButton(Widget section)
		{
			//	Place le bouton 'magique' qui donne accès aux bases de données d'usage moins fréquent.
			double buttonWidth = Library.UI.Constants.ButtonLargeWidth;

			var group = new FrameBox ()
			{
				Parent = section,
				PreferredSize = new Size (buttonWidth, buttonWidth+11-1),
				Dock = DockStyle.StackBegin,
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center,
			};

			this.databaseMenuButton = new GlyphButton ()
			{
				Parent = group,
				ButtonStyle = ButtonStyle.ComboItem,
				GlyphShape = GlyphShape.Menu,
				AutoFocus = false,
				PreferredSize = new Size (buttonWidth, 11),
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, -1, 0),
			};

			ToolTip.Default.SetToolTip (this.databaseMenuButton, "Montre une autre base de données...");

			this.databaseButton = this.CreateButton ();
			this.databaseButton.Parent = group;
			this.databaseButton.PreferredSize = new Size (buttonWidth, buttonWidth);
			this.databaseButton.Dock = DockStyle.Fill;

			
			this.databaseMenuButton.Clicked += delegate
			{
				this.ShowDatabaseSelectionMenu (this.databaseMenuButton);
			};

			this.databaseMenuButton.Entered += delegate
			{
				this.databaseButton.ButtonStyle = ButtonStyle.Combo;
			};

			this.databaseMenuButton.Exited += delegate
			{
				this.databaseButton.ButtonStyle = ButtonStyle.ToolItem;
			};

			var databaseCommandHandler = this.GetDatabaseCommandHandler ();

			databaseCommandHandler.Changed += delegate
			{
				this.UpdateDatabaseButton ();
			};

			this.UpdateDatabaseMenu ();
		}

		private void CreateRibbonNavigationSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Navigation",
				Title = "Navigation",
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 2,
				Dock = DockStyle.Left,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			section.Children.Add (this.CreateButton (Library.Res.Commands.History.NavigateBackward));
			section.Children.Add (this.CreateButton (Library.Res.Commands.History.NavigateForward));
		}

		private void CreateRibbonSettingsSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Settings",
				Title = "Réglages",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Right,
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 2,
			};

			section.Children.Add (this.CreateButton (Res.Commands.Global.ShowSettings));
			section.Children.Add (this.CreateButton (Res.Commands.Global.ShowDebug));
		}
		#endregion

		#region Create workflow ribbon sections
		private void CreateRibbonWorkflowContainerSection(RibbonPage page)
		{
			this.workflowContainer = new RibbonSection (page)
			{
				Name = "Workflow.Container",
				Title = "Actions",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Left,
				PreferredWidth = 80,
			};

			// TODO: C'est un moyen bricolé pour que WorkflowController sache où placer ses boutons.
			WorkflowController.SetWorkflowButtonsContainer (this.workflowContainer);

			// TODO: Et encore de la bricole !
			MainViewController.actionRibbonShow = this.RibbonShowPage;
			BusinessDocumentLinesController.actionRibbonShow = this.RibbonShowPage;
		}
		#endregion

		#region Create business ribbon sections
		private void CreateRibbonBusinessCreateSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Create",
				Title = "Insertion",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 1,
			};

			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateArticle,  isWithText: true));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateQuantity, isWithText: true));
			section.Children.Add (this.CreateSeparator ());
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateText,     isWithText: true));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateTitle,    isWithText: true));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateGroup,    isWithText: true));
			section.Children.Add (this.CreateSeparator ());
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateDiscount, isWithText: true));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateTax,      isWithText: true));
		}

		private void CreateRibbonBusinessOperSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Oper",
				Title = "Opérations",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 1,
			};

			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.MoveUp,    isWithText: true));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.MoveDown,  isWithText: true));
			section.Children.Add (this.CreateSeparator ());
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Duplicate, isWithText: true));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Delete,    isWithText: true));
		}

		private void CreateRibbonBusinessGroupSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Group",
				Title = "Groupes",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 1,
			};

			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Group,   isWithText: true));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Ungroup, isWithText: true));
			section.Children.Add (this.CreateSeparator ());
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Split,   isWithText: true));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Combine, isWithText: true));
			section.Children.Add (this.CreateSeparator ());
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Flat,    isWithText: true));
		}
		#endregion



		private void UpdateDatabaseMenu()
		{
			//	Met à jour les boutons pour les bases de données d'usage peu fréquent, après un changement d'utilisateur.
			var list  = this.GetDatabaseMenuCommands ().Where (x => x.Command != null).ToList ();
			int count = list.Count;

			this.databaseButton.Visibility     = (count > 0);
			this.databaseMenuButton.Visibility = (count > 1);

			if (count > 0)
			{
				this.databaseButton.CommandObject = list[0].Command;
				this.UpdateDatabaseButton ();
			}
		}

		private void ShowDatabaseSelectionMenu(Widget parentButton)
		{
			//	Construit puis affiche le menu des bases de données d'usage peu fréquent.
			var menu = new VMenu ();

			foreach (var type in this.GetSubMenuTypes ())
			{
				var commands = this.GetDatabaseMenuCommands ().Where (x => x.Type == type).ToArray ();

				if (commands.Length > 0)
				{
					var icon = string.Format ("manifest:Epsitec.Cresus.Core.Images.{0}.icon", RibbonViewController.GetSubMenuIcon (type));
					menu.Items.Add (new MenuItem (type.ToString (), icon, RibbonViewController.GetSubMenuName (type), ""));

					var subMenu = new VMenu ();

					for (int i=0; i<commands.Length; i++)
					{
						var command = commands[i];

						if (i == commands.Length-1 && command.Command == null)  // séparateur à la fin ?
						{
							break;
						}

						RibbonViewController.AddDatabaseToMenu (subMenu, command.Command);
					}

					menu.Items[menu.Items.Count-1].Submenu = subMenu;
				}
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = this.ribbonBook;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private static void AddDatabaseToMenu(VMenu menu, Command command)
		{
			if (command == null)
			{
				menu.Items.Add (new MenuSeparator ());
			}
			else
			{
				var item = new MenuItem ()
				{
					CommandObject = command,
					Name = command.Name,
				};

				menu.Items.Add (item);
			}
		}

		private void UpdateDatabaseButton()
		{
			//	Met à jour le bouton qui surplombe le bouton du menu, en fonction de la base sélectionnée.
			var selectedCommand = this.GetSelectedDatabaseCommand ();

			if (selectedCommand != null)
			{
				this.databaseButton.CommandObject = selectedCommand;
				this.DatabaseMenuDefaultCommandName = selectedCommand.Name;
			}
			else
			{
				this.databaseButton.CommandObject = this.GetDatabaseCommand (this.DatabaseMenuDefaultCommandName);
			}
		}

		private Command GetSelectedDatabaseCommand()
		{
			var commandHandler = this.GetDatabaseCommandHandler ();
			var name = commandHandler.SelectedDatabaseCommandName;
			return this.GetDatabaseCommand (name);
		}

		private Command GetDatabaseCommand(string name)
		{
			var item = this.GetDatabaseMenuCommands ().Where (x => x.Command != null && x.Command.Name == name).FirstOrDefault ();

			if (item == null)
			{
				return null;
			}
			else
			{
				return item.Command;
			}
		}

		private CommandHandlers.DatabaseCommandHandler GetDatabaseCommandHandler()
		{
			var handlers = this.coreCommandDispatcher.CommandHandlers.OfType<CommandHandlers.DatabaseCommandHandler> ();

			System.Diagnostics.Debug.Assert (handlers.Count () == 1);

			return handlers.First ();
		}

		private IEnumerable<SubMenuItem> GetDatabaseMenuCommands()
		{
			return this.GetDatabaseMenuCommands1 ().Where (x => x.Command == null || this.featureManager.IsCommandEnabled (x.Command.Caption.Id));
		}
		private IEnumerable<SubMenuItem> GetDatabaseMenuCommands1()
		{
			//	Retourne null lorsque le menu doit contenir un séparateur.
			
			bool admin = this.userManager.IsAuthenticatedUserAtPowerLevel (UserPowerLevel.Administrator);
			bool devel = this.userManager.IsAuthenticatedUserAtPowerLevel (UserPowerLevel.Developer);
			bool power = this.userManager.IsAuthenticatedUserAtPowerLevel (UserPowerLevel.PowerUser);

			if (admin || devel || power)
			{
				yield return new SubMenuItem (Res.Commands.Base.ShowDocumentCategoryMapping, SubMenuType.Printing);
				yield return new SubMenuItem (Res.Commands.Base.ShowDocumentCategory,        SubMenuType.Printing);
				yield return new SubMenuItem (Res.Commands.Base.ShowDocumentOptions,         SubMenuType.Printing);
				yield return new SubMenuItem (Res.Commands.Base.ShowDocumentPrintingUnits,   SubMenuType.Printing);

				yield return new SubMenuItem (Res.Commands.Base.ShowCurrency,                  SubMenuType.Finance);
				yield return new SubMenuItem (Res.Commands.Base.ShowExchangeRateSource,        SubMenuType.Finance);
				yield return new SubMenuItem (Res.Commands.Base.ShowVatDefinition,             SubMenuType.Finance);
				yield return new SubMenuItem (Res.Commands.Base.ShowAccountingOperation,	   SubMenuType.Finance);
				yield return new SubMenuItem (Res.Commands.Base.ShowPriceGroup,				   SubMenuType.Finance);
				yield return new SubMenuItem (Res.Commands.Base.ShowPriceRoundingMode,         SubMenuType.Finance);
				yield return new SubMenuItem (Res.Commands.Base.ShowIsrDefinition,             SubMenuType.Finance);
				yield return new SubMenuItem (Res.Commands.Base.ShowPaymentCategory,           SubMenuType.Finance);
				yield return new SubMenuItem (Res.Commands.Base.ShowPaymentReminderDefinition, SubMenuType.Finance);

				yield return new SubMenuItem (Res.Commands.Base.ShowImage, SubMenuType.Images);

				if (admin || devel)
				{
					yield return new SubMenuItem (Res.Commands.Base.ShowImageBlob,     SubMenuType.Images);
					yield return new SubMenuItem (null,                                SubMenuType.Images);
					yield return new SubMenuItem (Res.Commands.Base.ShowImageCategory, SubMenuType.Images);
					yield return new SubMenuItem (Res.Commands.Base.ShowImageGroup,    SubMenuType.Images);
				}
			}

			if (admin || devel)
			{
				yield return new SubMenuItem (Res.Commands.Base.ShowPersonGender,        SubMenuType.Customers);
				yield return new SubMenuItem (Res.Commands.Base.ShowPersonTitle,         SubMenuType.Customers);
				yield return new SubMenuItem (Res.Commands.Base.ShowLegalPersonType,     SubMenuType.Customers);
				yield return new SubMenuItem (Res.Commands.Base.ShowContactGroup,        SubMenuType.Customers);
				yield return new SubMenuItem (null,                                      SubMenuType.Customers);
				yield return new SubMenuItem (Res.Commands.Base.ShowTelecomType,         SubMenuType.Customers);
				yield return new SubMenuItem (Res.Commands.Base.ShowUriType,             SubMenuType.Customers);
				yield return new SubMenuItem (null,                                      SubMenuType.Customers);
//-				yield return new SubMenuItem (Res.Commands.Base.ShowStateProvinceCounty, SubMenuType.Customers);
				yield return new SubMenuItem (Res.Commands.Base.ShowLocation,            SubMenuType.Customers);
				yield return new SubMenuItem (Res.Commands.Base.ShowCountry,             SubMenuType.Customers);

				yield return new SubMenuItem (Res.Commands.Base.ShowArticleCategory,                    SubMenuType.Articles);
				yield return new SubMenuItem (Res.Commands.Base.ShowArticleGroup,                       SubMenuType.Articles);
				yield return new SubMenuItem (Res.Commands.Base.ShowArticleQuantityColumn,              SubMenuType.Articles);
				yield return new SubMenuItem (Res.Commands.Base.ShowAbstractArticleParameterDefinition, SubMenuType.Articles);
				yield return new SubMenuItem (Res.Commands.Base.ShowArticleStockLocation,               SubMenuType.Articles);
				yield return new SubMenuItem (Res.Commands.Base.ShowUnitOfMeasure,                      SubMenuType.Articles);

				yield return new SubMenuItem (Res.Commands.Base.ShowBusinessSettings,    SubMenuType.Misc);
				yield return new SubMenuItem (Res.Commands.Base.ShowGeneratorDefinition, SubMenuType.Misc);
				yield return new SubMenuItem (null,                                      SubMenuType.Misc);
			}

			if (devel)
			{
				yield return new SubMenuItem (Res.Commands.Base.ShowLanguage,           SubMenuType.Misc);
				yield return new SubMenuItem (null,                                     SubMenuType.Misc);
				yield return new SubMenuItem (Res.Commands.Base.ShowSoftwareUserGroup,  SubMenuType.Misc);
				yield return new SubMenuItem (Res.Commands.Base.ShowSoftwareUserRole,   SubMenuType.Misc);
				yield return new SubMenuItem (null,                                     SubMenuType.Misc);
				yield return new SubMenuItem (Res.Commands.Base.ShowWorkflowDefinition, SubMenuType.Misc);
			}
		}

		private IEnumerable<SubMenuType> GetSubMenuTypes()
		{
			yield return SubMenuType.Printing;
			yield return SubMenuType.Customers;
			yield return SubMenuType.Articles;
			yield return SubMenuType.Finance;
			yield return SubMenuType.Images;
			yield return SubMenuType.Misc;
		}

		private class SubMenuItem
		{
			public SubMenuItem(Command command, SubMenuType type)
			{
				this.Command = command;
				this.Type = type;
			}

			public Command Command
			{
				get;
				private set;
			}

			public SubMenuType Type
			{
				get;
				private set;
			}
		}

		private static string GetSubMenuName(SubMenuType type)
		{
			switch (type)
			{
				case SubMenuType.Printing:
					return "Impression";

				case SubMenuType.Finance:
					return "Finances";

				case SubMenuType.Images:
					return "Images";

				case SubMenuType.Customers:
					return "Clients";

				case SubMenuType.Articles:
					return "Articles";

				case SubMenuType.Misc:
					return "Divers";

				default:
					return null;
			}
		}

		private static string GetSubMenuIcon(SubMenuType type)
		{
			switch (type)
			{
				case SubMenuType.Printing:
					return "Base.DocumentPrintingUnits";

				case SubMenuType.Finance:
					return "Base.PaymentCategory";

				case SubMenuType.Images:
					return "Base.Image";

				case SubMenuType.Customers:
					return "Base.Customer";

				case SubMenuType.Articles:
					return "Base.ArticleDefinition";

				case SubMenuType.Misc:
					return "Base.BusinessSettings";

				default:
					return null;
			}
		}

		private enum SubMenuType
		{
			Printing,
			Finance,
			Images,
			Customers,
			Articles,
			Misc,
		}


		private void HandleAuthenticatedUserChanged(object sender)
		{
			this.UpdateAuthenticatedUser ();
		}


		private IconButton CreateButton(Command command = null, DockStyle dockStyle = DockStyle.StackBegin, CommandEventHandler handler = null, bool large = true, bool isActivable = false, bool isWithText = false)
		{
			if (handler != null)
			{
				this.commandDispatcher.Register (command, handler);
			}

			double buttonWidth = large ? Library.UI.Constants.ButtonLargeWidth : Library.UI.Constants.ButtonLargeWidth/2;
			double iconWidth   = large ? Library.UI.Constants.IconLargeWidth : Library.UI.Constants.IconSmallWidth;

			if (isActivable)
			{
				return new IconButton
				{
					CommandObject       = command,
					PreferredIconSize   = new Size (iconWidth, iconWidth),
					PreferredSize       = new Size (buttonWidth, buttonWidth),
					Dock                = dockStyle,
					Name                = (command == null) ? null : command.Name,
					VerticalAlignment   = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Center,
					AutoFocus           = false,
				};
			}
			else if (isWithText)
			{
				return new IconButtonWithText
				{
					CommandObject       = command,
					PreferredIconSize   = new Size (iconWidth, iconWidth),
					PreferredSize       = new Size (buttonWidth, buttonWidth+10),
					MaxAdditionnalWidth = 20,	// après CommandObject et PreferredSize !
					Dock                = dockStyle,
					Name                = (command == null) ? null : command.Name,
					VerticalAlignment   = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Center,
					AutoFocus           = false,
				};
			}
			else
			{
				return new RibbonIconButton
				{
					CommandObject       = command,
					PreferredIconSize   = new Size (iconWidth, iconWidth),
					PreferredSize       = new Size (buttonWidth, buttonWidth),
					Dock                = dockStyle,
					Name                = (command == null) ? null : command.Name,
					VerticalAlignment   = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Center,
					AutoFocus           = false,
				};
			}
		}

		private Separator CreateSeparator(DockStyle dockStyle = DockStyle.StackBegin)
		{
			return new Separator
			{
				IsVerticalLine = true,
				Dock           = dockStyle,
				PreferredWidth = 10,
			};
		}

		private static IconOrImageButton CreateIconOrImageButton(Command command)
		{
			double buttonWidth = Library.UI.Constants.ButtonLargeWidth;

			var button = new IconOrImageButton
			{
				CommandObject       = command,
				PreferredSize       = new Size (buttonWidth, buttonWidth),
				Dock                = DockStyle.StackBegin,
				Name                = (command == null) ? null : command.Name,
				VerticalAlignment   = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Center,
				AutoFocus           = false,
			};

			return button;
		}


		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultViewControllerComponentFactory<RibbonViewController>
		{
		}

		#endregion

		private void OnDatabaseMenuDefaultCommandNameChanged()
		{
			var handler = this.DatabaseMenuDefaultCommandNameChanged;

			if (handler != null)
			{
				handler (this);
			}
		}


		public event EventHandler					DatabaseMenuDefaultCommandNameChanged;

		private readonly CommandDispatcher			commandDispatcher;
		private readonly PersistenceManager			persistenceManager;
		private readonly UserManager				userManager;
		private readonly FeatureManager				featureManager;
		private readonly CoreCommandDispatcher		coreCommandDispatcher;
		private readonly List<IconOrImageButton>	authenticateUserButtons;
		private readonly List<StaticText>			authenticateUserWidgets;
		private readonly Dictionary<string, bool>	ribbonShowPages;
		
		private RibbonBook							ribbonBook;
		private RibbonPage							ribbonPageHome;
		private RibbonPage							ribbonPageWorkflow;
		private RibbonPage							ribbonPageBusiness;
		private RibbonPage							ribbonPageMisc;

		private IconButton							databaseButton;
		private GlyphButton							databaseMenuButton;
		private string								databaseMenuDefaultCommandName;

		private RibbonSection						workflowContainer;
	}
}
