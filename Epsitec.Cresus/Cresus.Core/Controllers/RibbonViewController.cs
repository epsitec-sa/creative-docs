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
using Epsitec.Cresus.Core.Workflows;

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

			this.orchestrator          = orchestrator;
			this.commandDispatcher     = app.CommandDispatcher;
			this.coreCommandDispatcher = app.GetComponent<CoreCommandDispatcher> ();
			this.persistenceManager    = app.PersistenceManager;
			this.settingsManager       = app.SettingsManager;
			this.userManager           = userManager;
			this.featureManager        = featureManager;

			this.sectionGroupFrames = new List<FrameBox> ();
			this.sectionIconFrames  = new List<FrameBox> ();
			this.sectionTitleFrames = new List<StaticText> ();
			this.sectionTitles      = new List<FormattedText> ();

			this.featureManager.EnableGodMode ();

			//	Les réglages sont restaurés après la création du ruban. Il est donc nécessaire d'écouter
			//	cet événement pour mettre à jour le ruban dès que les réglages sont restaurés.
			this.settingsManager.SettingsRestored += delegate
			{
				this.UpdateRibbon ();
			};
		}

		
		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.container = container;

			base.CreateUI (container);
			this.CreateRibbon (container);
			this.UpdateRibbon ();
		}


		private void CreateRibbon(Widget container)
		{
			//	Construit le faux ruban.
			var frame = new GradientFrameBox
			{
				Parent              = container,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				BackColor1          = RibbonViewController.GetBackgroundColor1 (),
				BackColor2          = RibbonViewController.GetBackgroundColor2 (),
				IsVerticalGradient  = true,
				BottomPercentOffset = 1.0 - 0.15,  // ombre dans les 15% supérieurs
				Dock                = DockStyle.Fill,
				Margins             = new Margins (-1, -1, 0, 0),
			};

			var separator = new Separator
			{
				Parent           = container,
				PreferredHeight  = 1,
				IsHorizontalLine = true,
				Dock             = DockStyle.Bottom,
			};

			this.sectionGroupFrames.Clear ();
			this.sectionIconFrames.Clear ();
			this.sectionTitleFrames.Clear ();
			this.sectionTitles.Clear ();

			//	|-->
			{
				var section = this.CreateSection (frame, DockStyle.Left, "Navigation");

				section.Children.Add (this.CreateButton (Library.Res.Commands.History.NavigateBackward));
				section.Children.Add (this.CreateButton (Library.Res.Commands.History.NavigateForward));
			}

			{
				var section = this.CreateSection (frame, DockStyle.Left, "Validation");

				section.Children.Add (this.CreateButton (Library.Res.Commands.Edition.SaveRecord));
				section.Children.Add (this.CreateButton (Library.Res.Commands.Edition.DiscardRecord));
			}

			{
				var section = this.CreateSection (frame, DockStyle.Left, "Edition");

				Widget topSection, bottomSection;
				this.CreateSubsections (section, out topSection, out bottomSection);

				topSection.Children.Add (this.CreateButton (ApplicationCommands.Bold, large: false, isActivable: true));
				topSection.Children.Add (this.CreateButton (ApplicationCommands.Italic, large: false, isActivable: true));
				bottomSection.Children.Add (this.CreateButton (ApplicationCommands.Underlined, large: false, isActivable: true));
				bottomSection.Children.Add (this.CreateButton (ApplicationCommands.MultilingualEdition, large: false));

				section.Children.Add (this.CreateGap ());
				this.CreateLanguage (section);
			}

			{
				var section = this.CreateSection (frame, DockStyle.Left, "Presse-papier");

				Widget topSection, bottomSection;
				this.CreateSubsections (section, out topSection, out bottomSection);

				topSection.Children.Add (this.CreateButton (ApplicationCommands.Cut, large: false));
				bottomSection.Children.Add (this.CreateButton (ApplicationCommands.Copy, large: false));

				section.Children.Add (this.CreateButton (ApplicationCommands.Paste));
			}

			{
				var section = this.CreateSection (frame, DockStyle.Left, "Actions");

				this.CreateRibbonWorkflowTransition (section);
				section.Children.Add (this.CreateButton (Res.Commands.Edition.Print));
				section.Children.Add (this.CreateGap ());

				Widget topSection, bottomSection;
				this.CreateSubsections (section, out topSection, out bottomSection);

				topSection.Children.Add (this.CreateButton (Res.Commands.File.ImportV11, large: false));
				bottomSection.Children.Add (this.CreateButton (Res.Commands.Feedback, large: false));
			}

			{
				var section = this.CreateSection (frame, DockStyle.Left, "Bases de données");

				section.Children.Add (this.CreateButton (Res.Commands.Base.ShowCustomer));
				section.Children.Add (this.CreateButton (Res.Commands.Base.ShowArticleDefinition));
				section.Children.Add (this.CreateButton (Res.Commands.Base.ShowDocumentMetadata));

				this.CreateRibbonDatabaseSectionMenuButton (section);
			}

			//	<--|
			{
				var section = this.CreateSection (frame, DockStyle.Right, "Réglages");

				Widget topSection, bottomSection;
				this.CreateSubsections (section, out topSection, out bottomSection);

				topSection.Children.Add (this.CreateButton (Res.Commands.Global.ShowSettings, large: false));
				bottomSection.Children.Add (this.CreateButton (Res.Commands.Global.ShowDebug, large: false));

				this.CreateRibbonUserSection (section);

				section.Children.Add (this.CreateGap ());  // espace pour le bouton 'v', avec chevauchement partiel volontaire
			}

			//	Bouton 'v'
			var showRibbonButton = new GlyphButton
			{
				Parent        = container.Window.Root,
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = new Size (14, 14),
				Margins       = new Margins (0, -1, -1, 0),
				GlyphShape    = GlyphShape.Menu,
				ButtonStyle   = ButtonStyle.Icon,
			};

			ToolTip.Default.SetToolTip (showRibbonButton, "Mode d'affichage de la barre d'icônes");

			showRibbonButton.Clicked += delegate
			{
				this.ShowRibbonModeMenu (showRibbonButton);
			};
		}


		private void UpdateRibbon()
		{
			//	Met à jour le faux ruban en fonction du RibbonViewMode en cours.
			var mode = this.RibbonViewMode;

			if (mode == RibbonViewMode.Hide)
			{
				this.container.Visibility = false;
			}
			else
			{
				this.container.Visibility = true;

				double  frameGap    = 0;
				Margins iconMargins = Margins.Zero;
				double  buttonWidth = 0;
				double  gapWidth    = 0;
				double  titleHeight = 0;
				double  titleSize   = 0;

				switch (mode)
				{
					case RibbonViewMode.Minimal:
						frameGap    = -1;  // les sections se chevauchent
						iconMargins = new Margins (0);
						buttonWidth = Library.UI.Constants.ButtonLargeWidth;
						gapWidth    = 3;
						titleHeight = 0;
						titleSize   = 0;
						break;

					case RibbonViewMode.Compact:
						frameGap    = -1;  // les sections se chevauchent
						iconMargins = new Margins (3);
						buttonWidth = Library.UI.Constants.ButtonLargeWidth;
						gapWidth    = 5;
						titleHeight = 0;
						titleSize   = 0;
						break;

					case RibbonViewMode.Default:
						frameGap    = 2;
						iconMargins = new Margins (3, 3, 3, 3-1);
						buttonWidth = Library.UI.Constants.ButtonLargeWidth;
						gapWidth    = 6;
						titleHeight = 11;
						titleSize   = 8;
						break;

					case RibbonViewMode.Large:
						frameGap    = 3;
						iconMargins = new Margins (3, 3, 3, 3-1);
						buttonWidth = Library.UI.Constants.ButtonLargeWidth+2;
						gapWidth    = 8;
						titleHeight = 14;
						titleSize   = 10;
						break;

					case RibbonViewMode.Hires:
						frameGap    = 4;
						iconMargins = new Margins (5, 5, 5, 5-1);
						buttonWidth = Library.UI.Constants.ButtonLargeWidth+6;
						gapWidth    = 10;
						titleHeight = 18;
						titleSize   = 12;
						break;
				}

				for (int i = 0; i < this.sectionGroupFrames.Count; i++)
				{
					//	Met à jour le panneau du groupe de la section.
					{
						var groupFrame = this.sectionGroupFrames[i];

						double leftMargin  = (groupFrame.Dock == DockStyle.Right) ? frameGap : 0;
						double rightMargin = (groupFrame.Dock == DockStyle.Left ) ? frameGap : 0;

						groupFrame.Margins = new Margins (leftMargin, rightMargin, -1, -1);
					}

					//	Met à jour le panneau des icônes de la section.
					{
						var iconFrame = this.sectionIconFrames[i];

						iconFrame.Padding = iconMargins;

						foreach (var gap in iconFrame.FindAllChildren ().Where (x => x.Name == "Gap"))
						{
							gap.PreferredWidth = gapWidth;
						}

						foreach (var widget in iconFrame.FindAllChildren ())
						{
							if (widget is IconButton || widget is RibbonIconButton)
							{
								var button = widget as IconButton;

								if (button.PreferredIconSize.Width == Library.UI.Constants.IconSmallWidth)
								{
									button.PreferredSize = new Size (buttonWidth/2, buttonWidth/2);
								}
								else
								{
									button.PreferredSize = new Size (buttonWidth, buttonWidth);
								}
							}

							if (widget is IconOrImageButton)
							{
								var button = widget as IconOrImageButton;

								button.PreferredSize = new Size (buttonWidth, buttonWidth);
							}
						}
					}

					//	Met à jour le titre de la section.
					{
						var titleFrame = this.sectionTitleFrames[i];
						var title = this.sectionTitles[i].ApplyFontSize (titleSize).ApplyFontColor (Color.FromBrightness (1.0)).ApplyBold ();

						titleFrame.Visibility      = (mode != RibbonViewMode.Minimal && mode != RibbonViewMode.Compact);
						titleFrame.FormattedText   = title;
						titleFrame.PreferredHeight = titleHeight;
					}
				}
			}
		}


		private Widget CreateSection(Widget frame, DockStyle dockStyle, FormattedText description)
		{
			//	Crée une section dans le faux ruban.
			var groupFrame = new FrameBox
			{
				Parent              = frame,
				DrawFullFrame       = true,
				BackColor           = RibbonViewController.GetSectionBackgroundColor (),
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth      = 10,
				Dock                = dockStyle,
			};

			var iconFrame = new FrameBox
			{
				Parent              = groupFrame,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth      = 10,
				Dock                = DockStyle.Fill,
			};

			var titleFrame = new StaticText
			{
				Parent           = groupFrame,
				ContentAlignment = ContentAlignment.MiddleCenter,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				BackColor        = RibbonViewController.GetTitleBackgroundColor (),
				PreferredWidth   = 10,
				Dock             = DockStyle.Bottom,
				Margins          = new Margins (1, 1, 0, 1),
			};

			this.sectionGroupFrames.Add (groupFrame);
			this.sectionIconFrames .Add (iconFrame);
			this.sectionTitleFrames.Add (titleFrame);
			this.sectionTitles     .Add (description);

			return iconFrame;
		}

		private void CreateSubsections(Widget section, out Widget topSection, out Widget bottomSection)
		{
			//	Crée deux sous-sections dans le faux ruban.
			var frame = new FrameBox
			{
				Parent              = section,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth      = 10,
				Dock                = DockStyle.StackBegin,
			};

			topSection = new FrameBox
			{
				Parent              = frame,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth      = 10,
				Dock                = DockStyle.Top,
			};

			bottomSection = new FrameBox
			{
				Parent              = frame,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredWidth      = 10,
				Dock                = DockStyle.Bottom,
			};
		}


		private void CreateLanguage(Widget section)
		{
			//	Crée les boutons pour choisir la langue.
			Widget topSection, bottomSection;
			this.CreateSubsections (section, out topSection, out bottomSection);

			//	TODO: faire cela proprement avec des commandes multi-états.
			var selectLanaguage1 = new IconButton ()
			{
				Parent            = topSection,
				Name              = "language=fr",
				PreferredSize     = new Size (Library.UI.Constants.ButtonLargeWidth/2, Library.UI.Constants.ButtonLargeWidth/2),
				PreferredIconSize = new Size (Library.UI.Constants.IconSmallWidth, Library.UI.Constants.IconSmallWidth),
				Dock              = DockStyle.Stacked,
				Text              = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagFR.icon""/>",
				ActiveState       = ActiveState.Yes,
			};

			var selectLanaguage2 = new IconButton ()
			{
				Parent            = topSection,
				Name              = "language=de",
				PreferredSize     = new Size (Library.UI.Constants.ButtonLargeWidth/2, Library.UI.Constants.ButtonLargeWidth/2),
				PreferredIconSize = new Size (Library.UI.Constants.IconSmallWidth, Library.UI.Constants.IconSmallWidth),
				Dock              = DockStyle.Stacked,
				Text              = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagDE.icon""/>",
			};

			var selectLanaguage3 = new IconButton ()
			{
				Parent            = bottomSection,
				Name              = "language=en",
				PreferredSize     = new Size (Library.UI.Constants.ButtonLargeWidth/2, Library.UI.Constants.ButtonLargeWidth/2),
				PreferredIconSize = new Size (Library.UI.Constants.IconSmallWidth, Library.UI.Constants.IconSmallWidth),
				Dock              = DockStyle.Stacked,
				Text              = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagGB.icon""/>",
			};

			var selectLanaguage4 = new IconButton ()
			{
				Parent            = bottomSection,
				Name              = "language=it",
				PreferredSize     = new Size (Library.UI.Constants.ButtonLargeWidth/2, Library.UI.Constants.ButtonLargeWidth/2),
				PreferredIconSize = new Size (Library.UI.Constants.IconSmallWidth, Library.UI.Constants.IconSmallWidth),
				Dock              = DockStyle.Stacked,
				Text              = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagIT.icon""/>",
			};

			ToolTip.Default.SetToolTip (selectLanaguage1, "Français");
			ToolTip.Default.SetToolTip (selectLanaguage2, "Allemand");
			ToolTip.Default.SetToolTip (selectLanaguage3, "Anglais");
			ToolTip.Default.SetToolTip (selectLanaguage4, "Italien");

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


		#region Workflow transitions
		private void CreateRibbonWorkflowTransition(Widget section)
		{
			//	Crée le bouton permettant de choisir une action pour créer un nouveau document dans l'affaire en cours,
			//	par le biais d'un menu.
			this.workflowTransitionButton = this.CreateButton ();
			this.workflowTransitionButton.IconUri = Misc.GetResourceIconUri ("WorkflowTransition");
			this.workflowTransitionButton.Enable = false;

			ToolTip.Default.SetToolTip (this.workflowTransitionButton, "Crée une nouvelle affaire ou un nouveau document");

			section.Children.Add (this.workflowTransitionButton);

			this.workflowTransitionButton.Clicked += delegate
			{
				this.ShowWorkflowTransitionMenu (this.workflowTransitionButton);
			};

			// TODO: C'est un moyen bricolé pour obtenir la liste des WorkflowTransition.
			WorkflowController.SetCallbackWorkflowTransitions (this.SetWorkflowTransitions);
		}

		private void ShowWorkflowTransitionMenu(Widget parentButton)
		{
			//	Affiche le menu permettant de choisir une action pour créer un nouveau document dans l'affaire en cours.
			if (this.workflowTransitions != null)
			{
				var menu = new VMenu ();

				int index = 0;
				foreach (var workflowTransition in this.workflowTransitions)
				{
					this.AddWorkflowTransitionToMenu (menu, workflowTransition.Edge.Name, index++);
				}

				TextFieldCombo.AdjustComboSize (parentButton, menu, false);

				menu.Host = this.container;
				menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
			}
		}

		private void AddWorkflowTransitionToMenu(VMenu menu, FormattedText text, int index)
		{
			var item = new MenuItem ()
			{
				FormattedText = text,
				TabIndex = index,
			};

			item.Clicked += delegate
			{
				var workflowTransition = this.workflowTransitions[item.TabIndex];
				this.ExecuteWorkflowTransition (workflowTransition);
			};

			menu.Items.Add (item);
		}

		private void ExecuteWorkflowTransition(WorkflowTransition workflowTransition)
		{
			//	Crée un nouveau document dans l'affaire en cours.
			using (var engine = new WorkflowExecutionEngine (workflowTransition))
			{
				engine.Associate (this.orchestrator.Navigator);
				engine.Execute ();
			}

			this.RefreshNavigation ();
		}

		private void RefreshNavigation()
		{
			this.orchestrator.Navigator.PreserveNavigation (() => this.orchestrator.ClearActiveEntity ());
		}

		private void SetWorkflowTransitions(List<WorkflowTransition> workflowTransitions)
		{
			//	Appelé par WorkflowController lorsque les actions possibles ont changé.
			this.workflowTransitions = workflowTransitions;
			this.workflowTransitionButton.Enable = (this.workflowTransitions != null && this.workflowTransitions.Any ());
		}
		#endregion


		#region User button
		private void CreateRibbonUserSection(Widget section)
		{
			//	Crée le bouton 'utilisateur', qui affiche l'utilisateur en cours et permet d'en changer.
			this.authenticateUserButton = RibbonViewController.CreateIconOrImageButton (Res.Commands.Global.ShowUserManager);
			this.authenticateUserButton.CoreData = this.Orchestrator.Data;
			this.authenticateUserButton.IconUri = Misc.GetResourceIconUri ("UserManager");
			this.authenticateUserButton.IconPreferredSize = new Size (31, 31);

			section.Children.Add (this.authenticateUserButton);

			//	Le widget 'authenticateUserWidget' déborde volontairement sur le bas du bouton 'ShowUserManager',
			//	pour permettre d'afficher un nom d'utilisateur lisible.
			//	L'icône UserManager.icon est décalée vers le haut en conséquence.
			this.authenticateUserWidget = new StaticText
			{
				Parent = this.authenticateUserButton,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				PreferredHeight = 14,
				Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom,
				Margins = new Margins (0, 0, 0, 0),
			};
		}

		private void HandleAuthenticatedUserChanged(object sender)
		{
			this.UpdateAuthenticatedUser ();
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
				FormattedText text = user.LoginName;
				this.authenticateUserWidget.FormattedText = text.ApplyFontSize (9.0);

				ToolTip.Default.SetToolTip (this.authenticateUserButton, user.ShortDescription);
				ToolTip.Default.SetToolTip (this.authenticateUserWidget, user.ShortDescription);
			}

			this.UpdateDatabaseMenu ();
		}
		#endregion


		#region Additionnal databases
		private void CreateRibbonDatabaseSectionMenuButton(Widget section)
		{
			//	Crée le bouton 'magique' qui donne accès aux bases de données d'usage moins fréquent par le biais d'un menu.
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

		private void UpdateDatabaseMenu()
		{
			//	Met à jour les boutons pour les bases de données d'usage peu fréquent, après un changement d'utilisateur par exemple.
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
					var icon = Misc.GetResourceIconUri (RibbonViewController.GetSubMenuIcon (type));
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

			menu.Host = this.container;
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
					Name          = command.Name,
				};

				menu.Items.Add (item);
			}
		}

		private void UpdateDatabaseButton()
		{
			//	Met à jour le bouton qui surplombe le bouton du menu, en fonction de la base sélectionnée.
			var selectedCommand = this.GetSelectedDatabaseCommand ();

			if (selectedCommand == null)
			{
				this.databaseButton.CommandObject = this.GetDatabaseCommand (this.AdditionalDatabase);
			}
			else
			{
				this.databaseButton.CommandObject = selectedCommand;
				this.AdditionalDatabase           = selectedCommand.Name;
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
				yield return new SubMenuItem (Res.Commands.Base.ShowCustomerCategory,    SubMenuType.Customers);
				yield return new SubMenuItem (Res.Commands.Base.ShowPriceDiscount,       SubMenuType.Customers);
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
		#endregion


		#region Ribbon mode menu
		private void ShowRibbonModeMenu(Widget parentButton)
		{
			//	Affiche le menu permettant de choisir le mode pour le ruban.
			var menu = new VMenu ();

			this.AddRibbonModeToMenu (menu, "Pas de barre d'icônes",           RibbonViewMode.Hide);
			menu.Items.Add (new MenuSeparator ());
			this.AddRibbonModeToMenu (menu, "Barre d'icônes minimaliste",      RibbonViewMode.Minimal);
			this.AddRibbonModeToMenu (menu, "Barre d'icônes compacte",         RibbonViewMode.Compact);
			menu.Items.Add (new MenuSeparator ());
			this.AddRibbonModeToMenu (menu, "Barre d'icônes standard",         RibbonViewMode.Default);
			this.AddRibbonModeToMenu (menu, "Barre d'icônes aérée",            RibbonViewMode.Large);
			this.AddRibbonModeToMenu (menu, "Barre d'icônes pour grand écran", RibbonViewMode.Hires);

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = this.container;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddRibbonModeToMenu(VMenu menu, FormattedText text, RibbonViewMode mode)
		{
			bool selected = (this.RibbonViewMode == mode);

			var item = new MenuItem ()
			{
				IconUri       = Misc.GetResourceIconUri(selected ? "Button.RadioYes" : "Button.RadioNo"),
				FormattedText = text,
				Name          = mode.ToString (),
			};

			item.Clicked += delegate
			{
				this.RibbonViewMode = (RibbonViewMode) System.Enum.Parse (typeof (RibbonViewMode), item.Name);
				this.UpdateRibbon ();
			};

			menu.Items.Add (item);
		}
		#endregion


		private IconButton CreateButton(Command command = null, DockStyle dockStyle = DockStyle.StackBegin, CommandEventHandler handler = null, bool large = true, bool isActivable = false)
		{
			if (command != null && handler != null)
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

		private Widget CreateGap()
		{
			var gap = new FrameBox
			{
				Name = "Gap",
				Dock = DockStyle.StackBegin,
			};

			return gap;
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


		#region Properties using the SettingsManager
		private RibbonViewMode RibbonViewMode
		{
			//	Mode d'affichage du ruban, directement stocké dans les réglages via le SettingsManager.
			get
			{
				var s = this.settingsManager.GetSettings ("RibbonViewController.RibbonViewMode");

				if (!string.IsNullOrEmpty (s))
				{
					RibbonViewMode mode;
					if (System.Enum.TryParse<RibbonViewMode> (s, out mode))
					{
						return mode;
					}
				}

				return RibbonViewMode.Default;  // retourne le mode par défaut
			}
			set
			{
				this.settingsManager.SetSettings ("RibbonViewController.RibbonViewMode", value.ToString ());
			}
		}

		private string AdditionalDatabase
		{
			//	Base de donnée additionnelle choisie, directement stocké dans les réglages via le SettingsManager.
			get
			{
				var s = this.settingsManager.GetSettings ("RibbonViewController.AdditionalDatabase");

				if (!string.IsNullOrEmpty (s))
				{
					return s;
				}

				return "Base.ShowBusinessSettings";  // retourne la base de données par défaut
			}
			set
			{
				this.settingsManager.SetSettings ("RibbonViewController.AdditionalDatabase", value);
			}
		}
		#endregion


		#region Color manager
		private static Color GetBackgroundColor1()
		{
			//	Couleur pour l'ombre en haut des zones libres du ruban.
			return RibbonViewController.GetColor (RibbonViewController.GetBaseColor (), saturation: 0.06, value: 0.9);
		}

		private static Color GetBackgroundColor2()
		{
			//	Couleur pour le fond des zones libres du ruban.
			return RibbonViewController.GetColor (RibbonViewController.GetBaseColor (), saturation: 0.06, value: 0.7);
		}

		private static Color GetSectionBackgroundColor()
		{
			//	Couleur pour le fond d'une section du ruban.
			return RibbonViewController.GetColor (RibbonViewController.GetBaseColor (), saturation: 0.02, value: 0.95);
		}

		private static Color GetTitleBackgroundColor()
		{
			//	Couleur pour le fond du titre d'une section du ruban.
			return RibbonViewController.GetColor (RibbonViewController.GetBaseColor (), saturation: 0.2, value: 0.7);
		}

		private static Color GetBaseColor()
		{
			//	Couleur de base pour le ruban, dont on utilise la teinte (hue).
#if true
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			return adorner.ColorBorder;
#else
			return Color.FromHexa ("6485b7");  // "le" bleu des icônes des bases de données
#endif
		}

		private static Color GetColor(Color color, double? saturation = null, double? value = null)
		{
			//	Retourne une couleur en forçant éventuellement la saturation et la valeur.
			double h, s, v;
			Color.ConvertRgbToHsv (color.R, color.G, color.B, out h, out s, out v);

			if (saturation.HasValue)
			{
				s = saturation.Value;
			}

			if (value.HasValue)
			{
				v = value.Value;
			}

			double r, g, b;
			Color.ConvertHsvToRgb (h, s, v, out r, out g, out b);

			return new Color (r, g, b);
		}
		#endregion


		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultViewControllerComponentFactory<RibbonViewController>
		{
		}

		#endregion


		private readonly DataViewOrchestrator		orchestrator;
		private readonly CommandDispatcher			commandDispatcher;
		private readonly CoreCommandDispatcher		coreCommandDispatcher;
		private readonly PersistenceManager			persistenceManager;
		private readonly SettingsManager			settingsManager;
		private readonly UserManager				userManager;
		private readonly FeatureManager				featureManager;
		private readonly List<FrameBox>				sectionGroupFrames;
		private readonly List<FrameBox>				sectionIconFrames;
		private readonly List<StaticText>			sectionTitleFrames;
		private readonly List<FormattedText>		sectionTitles;

		private Widget								container;
		
		private IconButton							databaseButton;
		private GlyphButton							databaseMenuButton;

		private IconButton							workflowTransitionButton;
		private List<WorkflowTransition>			workflowTransitions;

		private IconOrImageButton					authenticateUserButton;
		private StaticText							authenticateUserWidget;
	}
}
