//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Fields.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le pied de page pour l'édition de la comptabilité.
	/// </summary>
	public class UtilisateursFooterController : AbstractFooterController
	{
		public UtilisateursFooterController(AbstractController controller)
			: base (controller)
		{
			this.checkButtons = new List<CheckButton> ();
			this.ignoreChanges = new SafeCounter ();
		}


		public override bool HasManualGeometry
		{
			get
			{
				return true;
			}
		}


		public override void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			this.fieldControllers.Clear ();

			var mainFrame = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
			};

			this.bottomToolbarController = new BottomToolbarController (this.businessContext);
			var toolbar = this.bottomToolbarController.CreateUI (mainFrame);
			toolbar.Dock    = DockStyle.Top;
			toolbar.Margins = new Margins (0);
			toolbar.Padding = new Margins (0);

			var band = new FrameBox
			{
				Parent        = mainFrame,
				DrawFullFrame = true,
				Dock          = DockStyle.Fill,
				Padding       = new Margins (10),
			};

			this.buttonsFrame = new FrameBox
			{
				Parent              = band,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth      = 200,
				Dock                = DockStyle.Right,
				Margins             = new Margins (0, 0, 0, 0),
			};

			this.linesFrame = new FrameBox
			{
				Parent              = band,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth      = 200,
				Dock                = DockStyle.Right,
				Margins             = new Margins (0, 10, 0, 0),
			};

			this.CreateLineUI (this.linesFrame);
			this.CreateButtonsUI (this.buttonsFrame);

			base.CreateUI (parent, updateArrayContentAction);
		}

		private void CreateLineUI(Widget parent)
		{
			//	Comme on est en mode HasRightFooter, la "ligne" est en fait une colonne dans laquelle
			//	on empile les choses de haut en bas.
			this.fieldControllers.Add (new List<AbstractFieldController> ());

			var footerFrame = new TabCatcherFrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				Margins         = new Margins (0, 0, 1, 0),
			};

			footerFrame.TabPressed += new TabCatcherFrameBox.TabPressedEventHandler (this.HandleLinesContainerTabPressed);

			this.linesFrames.Add (footerFrame);
			int line = this.linesFrames.Count - 1;
			int tabIndex = 0;

			footerFrame.TabIndex = line+1;

			foreach (var mapper in this.columnMappers.Where (x => x.Show))
			{
				if (mapper.Column == ColumnType.Résumé)
				{
					continue;
				}

				if (mapper.Column == ColumnType.IdentitéWindows)  // insère un gap ?
				{
					new FrameBox
					{
						Parent          = footerFrame,
						PreferredHeight = 10,
						Dock            = DockStyle.Top,
					};
				}

				bool compact = mapper.Description.IsNullOrEmpty            ||
							   mapper.Column == ColumnType.IdentitéWindows ||
							   mapper.Column == ColumnType.Désactivé       ;

				if (!compact)
				{
					new StaticText
					{
						Parent        = footerFrame,
						FormattedText = mapper.Description + " :",
						Dock          = DockStyle.Top,
						Margins       = new Margins (0, 0, 0, 1),
					};
				}

				AbstractFieldController field;

				if (mapper.Column == ColumnType.Pièce)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, this.comptaEntity.PiècesGenerator.Select (x => x.Nom).ToArray ());
				}
				else if (mapper.Column == ColumnType.MotDePasse)
				{
					field = new PasswordFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);
				}
				else if (mapper.Column == ColumnType.IdentitéWindows ||
						 mapper.Column == ColumnType.Désactivé       )
				{
					field = new CheckButtonController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);
				}
				else
				{
					field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);
				}

				field.Box.TabIndex = ++tabIndex;
				field.Box.Dock = DockStyle.Top;
				field.Box.Margins = new Margins (0, 0, compact ? -6:0, 5);

				this.fieldControllers[line].Add (field);
			}
		}

		private void CreateButtonsUI(Widget parent)
		{
			this.checkButtons.Clear ();

			var group = new GroupBox
			{
				Parent  = parent,
				Text    = "Présentations accessibles par l'utilisateur",
				Dock    = DockStyle.Fill,
				Padding = new Margins (10, 0, 5, 5),
			};

			var top = new FrameBox
			{
				Parent = group,
				Dock   = DockStyle.Fill,
			};

			var columns = new List<FrameBox> ();

			for (int i = 0; i < 2; i++)
			{
				var column = new FrameBox
				{
					Parent         = top,
					PreferredWidth = 120,
					Dock           = DockStyle.Left,
					Margins        = new Margins (0, 5, 0, 0),
				};

				columns.Add (column);
			}

			int rank = 0;
			foreach (var cmd in Converters.PrésentationCommands)
			{
				if (cmd == Res.Commands.Présentation.Login)  // cette présentation est toujours accessible !
				{
					continue;
				}

				int column = rank++/16;  // 16 boutons par colonne
				if (column >= columns.Count)
				{
					break;
				}

				this.CreatePrésentationButton (columns[column], cmd);
			}

			{
				var footer = new FrameBox
				{
					Parent          = group,
					PreferredHeight = 20,
					Dock            = DockStyle.Bottom,
				};

				this.zeroPrésentationButton = new Button
				{
					Parent          = footer,
					Text            = "Aucune",
					PreferredWidth  = 80,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins         = new Margins (0, 10, 0, 0),
				};

				this.allPrésentationButton = new Button
				{
					Parent          = footer,
					Text            = "Toutes",
					PreferredWidth  = 80,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins         = new Margins (0, 10, 0, 0),
				};

				ToolTip.Default.SetToolTip (this.zeroPrésentationButton, "Interdit l'accès à toutes les présentations");
				ToolTip.Default.SetToolTip (this.allPrésentationButton,  "Autorise l'accès à toutes les présentations");
			}

			this.zeroPrésentationButton.Clicked += delegate
			{
				foreach (var button in this.checkButtons)
				{
					button.ActiveState = ActiveState.No;
				}
			};

			this.allPrésentationButton.Clicked += delegate
			{
				foreach (var button in this.checkButtons)
				{
					button.ActiveState = ActiveState.Yes;
				}
			};
		}

		private void CreatePrésentationButton(Widget parent, Command cmd)
		{
			var icon = UIBuilder.GetTextIconUri ("Présentation." + Converters.PrésentationCommandToString (cmd), iconSize: 20);
			var desc = Converters.GetPrésentationCommandDescription (cmd);

			var button = new CheckButton
			{
				Parent          = parent,
				FormattedText   = icon + " " + desc,
				TextBreakMode   = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Name            = Converters.PrésentationCommandToString (cmd),
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			ToolTip.Default.SetToolTip (button, desc);

			this.checkButtons.Add (button);

			button.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.EditionLine.SetPrésenttion (cmd, button.ActiveState == ActiveState.Yes);
					this.FooterTextChanged ();
				}
			};
		}


		protected override void UpdateEditionWidgets()
		{
			base.UpdateEditionWidgets ();
			this.UpdateButtons ();
		}

		private void UpdateButtons()
		{
			using (this.ignoreChanges.Enter ())
			{
				bool admin = this.EditionLine.IsAdmin;
				bool zero  = true;
				bool all   = true;

				foreach (var button in this.checkButtons)
				{
					var cmd = Converters.StringToPrésentationCommand(button.Name);
					var state = this.EditionLine.HasPrésentation (cmd);

					button.ActiveState = state || admin ? ActiveState.Yes : ActiveState.No;
					button.Enable = !admin;

					if (button.ActiveState == ActiveState.Yes)
					{
						zero = false;
					}
					else
					{
						all = false;
					}
				}

				this.zeroPrésentationButton.Enable = !admin && !zero;
				this.allPrésentationButton .Enable = !admin && !all;
			}
		}

		private UtilisateursEditionLine EditionLine
		{
			get
			{
				return this.dataAccessor.EditionLine[0] as UtilisateursEditionLine;
			}
		}


		protected override FormattedText GetOperationDescription(bool modify)
		{
			return modify ? "Modification d'un utilisateur :" : "Création d'un utilisateur :";
		}


		private readonly List<CheckButton>	checkButtons;
		private readonly SafeCounter		ignoreChanges;

		private FrameBox					linesFrame;
		private FrameBox					buttonsFrame;
		private Button						zeroPrésentationButton;
		private Button						allPrésentationButton;
	}
}
