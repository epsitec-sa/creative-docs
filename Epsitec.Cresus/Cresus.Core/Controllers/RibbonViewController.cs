//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Features;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

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
			this.CreateRibbonBook (container);
			this.CreateRibbonHomePage ();
		}


		private void UpdateAuthenticatedUser()
		{
			//	Met à jour le nom de l'utilisateur dans le ruban.
			var user = this.userManager.AuthenticatedUser;

			if (user.IsNull ())
			{
				this.authenticateUserButton.ImageEntity = null;
				this.authenticateUserWidget.Text = null;

				ToolTip.Default.HideToolTipForWidget (this.authenticateUserButton);
				ToolTip.Default.HideToolTipForWidget (this.authenticateUserWidget);
			}
			else
			{
#if false
				throw new System.NotImplementedException ();
				this.authenticateUserButton.ImageEntity = user.Person.Pictures.FirstOrDefault ();
#endif
				this.authenticateUserWidget.Text = string.Concat ("<font size=\"9\">", user.LoginName, "</font>");

				ToolTip.Default.SetToolTip (this.authenticateUserButton, user.ShortDescription);
				ToolTip.Default.SetToolTip (this.authenticateUserWidget, user.ShortDescription);
			}

			this.UpdateDatabaseMenu ();
		}

		
		private void CreateRibbonBook(Widget container)
		{
			this.ribbonBook = new RibbonBook ()
			{
				Parent = container,
				Dock = DockStyle.Fill,
				Name = "Ribbon"
			};

			this.persistenceManager.Register (this.ribbonBook);
			this.persistenceManager.Register (this, this.ribbonBook.FullPathName + ".DatabaseMenu.DefaultCommand",
				x => this.DatabaseMenuDefaultCommandNameChanged += x,
				x => this.DatabaseMenuDefaultCommandNameChanged -= x,
				xml => xml.Add (new XAttribute ("name", this.DatabaseMenuDefaultCommandName)),
				xml => this.DatabaseMenuDefaultCommandName = xml.Attribute ("name").Value);
		}

		private void CreateRibbonHomePage()
		{
			this.ribbonPageHome     = RibbonViewController.CreateRibbonPage (this.ribbonBook, "Home", "Principal");
			this.ribbonPageBusiness = RibbonViewController.CreateRibbonPage (this.ribbonBook, "Business", "Documents commerciaux");

			this.ribbonBook.ActivePage = this.ribbonPageHome;

			//	Home ribbon:
			this.CreateRibbonUserSection (this.ribbonPageHome);
			this.CreateRibbonEditSection (this.ribbonPageHome);

			this.CreateRibbonClipboardSection (this.ribbonPageHome);
			this.CreateRibbonFontSection (this.ribbonPageHome);
			this.CreateRibbonDatabaseSection (this.ribbonPageHome);
			this.CreateRibbonStateSection (this.ribbonPageHome);
			this.CreateRibbonSettingsSection (this.ribbonPageHome);
			this.CreateRibbonNavigationSection (this.ribbonPageHome);

			//	Business ribbon:
			this.CreateRibbonUserSection (this.ribbonPageBusiness);
			this.CreateRibbonEditSection (this.ribbonPageBusiness);

			//?this.CreateRibbonBusinessActionSection (this.ribbonPageBusiness);
			this.CreateRibbonBusinessCreateSection (this.ribbonPageBusiness);
			this.CreateRibbonBusinessOperSection (this.ribbonPageBusiness);
			this.CreateRibbonBusinessGroupSection (this.ribbonPageBusiness);
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
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 1,
			};

			{
				var frame = new FrameBox
				{
					Parent = section,
					Dock = DockStyle.StackBegin,
					ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
					PreferredWidth = Library.UI.Constants.ButtonLargeWidth,
				};

				this.authenticateUserButton = RibbonViewController.CreateIconOrImageButton (Res.Commands.Global.ShowUserManager);
				this.authenticateUserButton.CoreData = this.Orchestrator.Data;
				this.authenticateUserButton.IconUri = Misc.GetResourceIconUri ("UserManager");
				this.authenticateUserButton.IconPreferredSize = new Size (31, 31);

				frame.Children.Add (this.authenticateUserButton);

				//	Le widget 'authenticateUserWidget' déborde volontairement sur le bas du bouton 'ShowUserManager',
				//	pour permettre d'afficher un nom d'utilisateur lisible.
				this.authenticateUserWidget = new StaticText
				{
					Parent = frame,
					ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
					PreferredHeight = 12,
					PreferredWidth = Library.UI.Constants.ButtonLargeWidth,
					Dock = DockStyle.Stacked,
					Margins = new Margins (0, 0, -2, 0),
				};
			}
		}

		private void CreateRibbonEditSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Edit",
				Title = "Édition",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 4 +
								 Library.UI.Constants.ButtonSmallWidth * 1,
			};

			section.Children.Add (this.CreateButton (Library.Res.Commands.Edition.SaveRecord));
			section.Children.Add (this.CreateButton (Library.Res.Commands.Edition.DiscardRecord));
			section.Children.Add (this.CreateButton (Res.Commands.Edition.Print));
			section.Children.Add (this.CreateButton (Res.Commands.Edition.Preview));

			var frame = new FrameBox
			{
				Parent = section,
				Dock = DockStyle.StackBegin,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth = Library.UI.Constants.ButtonSmallWidth,
			};

			frame.Children.Add (this.CreateButton (Res.Commands.File.ImportV11, large: false));
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

			frame.Children.Add (this.CreateButton (ApplicationCommands.Cut, large: false));
			frame.Children.Add (this.CreateButton (ApplicationCommands.Copy, large: false));

			section.Children.Add (this.CreateButton (ApplicationCommands.Paste));
		}

		private void CreateRibbonFontSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Font",
				Title = "Police",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = Library.UI.Constants.ButtonSmallWidth * 3,
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

			topFrame.Children.Add (this.CreateButton (ApplicationCommands.Bold, large: false, isActivable: true));
			topFrame.Children.Add (this.CreateButton (ApplicationCommands.Italic, large: false, isActivable: true));
			topFrame.Children.Add (this.CreateButton (ApplicationCommands.Underlined, large: false, isActivable: true));

			//?bottomFrame.Children.Add (this.CreateButton (ApplicationCommands.Subscript,   large: false, isActivable: true));
			//?bottomFrame.Children.Add (this.CreateButton (ApplicationCommands.Superscript, large: false, isActivable: true));
		}

		private void CreateRibbonDatabaseSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Database",
				Title = "Bases de données",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 3,
				Dock = DockStyle.Fill,
			};

			//	Place les boutons pour les bases de données les plus courantes.
			section.Children.Add (this.CreateButton (Res.Commands.Base.ShowCustomer));
			section.Children.Add (this.CreateButton (Res.Commands.Base.ShowArticleDefinition));
			section.Children.Add (this.CreateButton (Res.Commands.Base.ShowDocumentMetadata));

			this.CreateRibbonDatabaseSectionMenuButton (section);
		}

		private void CreateRibbonDatabaseSectionMenuButton(RibbonSection section)
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

		private void CreateRibbonStateSection(RibbonPage page)
		{
#if false
			var section = new RibbonSection (page)
			{
				Name = "State",
				Title = "États",
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow
			};
#endif
		}

		private void CreateRibbonNavigationSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Navigation",
				Title = "Navigation",
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 2,
				Dock = DockStyle.Right,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow
			};

			section.Children.Add (this.CreateButton (Res.Commands.History.NavigateBackward));
			section.Children.Add (this.CreateButton (Res.Commands.History.NavigateForward));
		}

		private void CreateRibbonSettingsSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Settings",
				Title = "Réglages",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Right,
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 4,
			};

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

				var selectLanaugage1 = new IconButton ()
				{
					Parent = frame1,
					Name = "language=fr",
					Dock = DockStyle.Stacked,
					Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagFR.icon""/>",
					ActiveState = ActiveState.Yes,
				};

				var selectLanaugage2 = new IconButton ()
				{
					Parent = frame1,
					Name = "language=de",
					Dock = DockStyle.Stacked,
					Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagDE.icon""/>",
				};

				var selectLanaugage3 = new IconButton ()
				{
					Parent = frame2,
					Name = "language=en",
					Dock = DockStyle.Stacked,
					Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagGB.icon""/>",
				};

				var selectLanaugage4 = new IconButton ()
				{
					Parent = frame2,
					Name = "language=it",
					Dock = DockStyle.Stacked,
					Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagIT.icon""/>",
				};

				selectLanaugage1.Clicked += delegate
				{
					Library.UI.Services.Settings.CultureForData.SelectLanguage (null);
					selectLanaugage1.ActiveState = ActiveState.Yes;
					selectLanaugage2.ActiveState = ActiveState.No;
					selectLanaugage3.ActiveState = ActiveState.No;
					selectLanaugage4.ActiveState = ActiveState.No;
				};

				selectLanaugage2.Clicked += delegate
				{
					Library.UI.Services.Settings.CultureForData.SelectLanguage ("de");
					selectLanaugage1.ActiveState = ActiveState.No;
					selectLanaugage2.ActiveState = ActiveState.Yes;
					selectLanaugage3.ActiveState = ActiveState.No;
					selectLanaugage4.ActiveState = ActiveState.No;
				};

				selectLanaugage3.Clicked += delegate
				{
					Library.UI.Services.Settings.CultureForData.SelectLanguage ("en");
					selectLanaugage1.ActiveState = ActiveState.No;
					selectLanaugage2.ActiveState = ActiveState.No;
					selectLanaugage3.ActiveState = ActiveState.Yes;
					selectLanaugage4.ActiveState = ActiveState.No;
				};

				selectLanaugage4.Clicked += delegate
				{
					Library.UI.Services.Settings.CultureForData.SelectLanguage ("it");
					selectLanaugage1.ActiveState = ActiveState.No;
					selectLanaugage2.ActiveState = ActiveState.No;
					selectLanaugage3.ActiveState = ActiveState.No;
					selectLanaugage4.ActiveState = ActiveState.Yes;
				};
			}

			section.Children.Add (this.CreateButton (ApplicationCommands.MultilingualEdition));
			section.Children.Add (this.CreateButton (Res.Commands.Global.ShowSettings));
			section.Children.Add (this.CreateButton (Res.Commands.Global.ShowDebug));
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

			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateArticle));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateText));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateTitle));
			section.Children.Add (this.CreateSeparator ());
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateDiscount));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateTax));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateQuantity));
			section.Children.Add (this.CreateSeparator ());
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateGroup));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.CreateGroupSeparator));
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

			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Duplicate));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Delete));
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

			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Group));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Ungroup));
		}

		private void CreateRibbonBusinessActionSection(RibbonPage page)
		{
			var section = new RibbonSection (page)
			{
				Name = "Action",
				Title = "Actions",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth = Library.UI.Constants.ButtonLargeWidth * 1,
			};

			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Ok));
			section.Children.Add (this.CreateButton (Library.Business.Res.Commands.Lines.Cancel));
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
				yield return new SubMenuItem (Res.Commands.Base.ShowPriceGroup,                SubMenuType.Finance);
				yield return new SubMenuItem (Res.Commands.Base.ShowPriceRoundingMode,         SubMenuType.Finance);
				yield return new SubMenuItem (Res.Commands.Base.ShowIsrDefinition,             SubMenuType.Finance);
				yield return new SubMenuItem (Res.Commands.Base.ShowPaymentMode,               SubMenuType.Finance);
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
				yield return new SubMenuItem (Res.Commands.Base.ShowStateProvinceCounty, SubMenuType.Customers);
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
					return "Base.PaymentMode";

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


		private IconButton CreateButton(Command command = null, DockStyle dockStyle = DockStyle.StackBegin, CommandEventHandler handler = null, bool large = true, bool isActivable = false)
		{
			if (handler != null)
			{
				this.commandDispatcher.Register (command, handler);
			}

			double buttonWidth = large ? Library.UI.Constants.ButtonLargeWidth : Library.UI.Constants.ButtonSmallWidth;
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
		
		
		public event EventHandler				DatabaseMenuDefaultCommandNameChanged;

		private readonly CommandDispatcher		commandDispatcher;
		private readonly PersistenceManager		persistenceManager;
		private readonly UserManager			userManager;
		private readonly FeatureManager			featureManager;
		private readonly CoreCommandDispatcher	coreCommandDispatcher;
		
		private RibbonBook						ribbonBook;
		private RibbonPage						ribbonPageHome;
		private RibbonPage						ribbonPageBusiness;

		private IconButton						databaseButton;
		private GlyphButton						databaseMenuButton;
		private string							databaseMenuDefaultCommandName;

		private IconOrImageButton				authenticateUserButton;
		private StaticText						authenticateUserWidget;
	}
}
