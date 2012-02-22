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


		public override void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			this.fieldControllers.Clear ();

			this.buttonsFrame = new FrameBox
			{
				Parent              = parent,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredHeight     = 80,
				Dock                = DockStyle.Bottom,
				Margins             = new Margins (0, 0, 10, 0),
			};

			this.linesFrame = new FrameBox
			{
				Parent              = parent,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock                = DockStyle.Bottom,
			};

			this.CreateLineUI (this.linesFrame);
			this.CreateButtonsUI (this.buttonsFrame);

			base.CreateUI (parent, updateArrayContentAction);
		}

		private void CreateLineUI(Widget parent)
		{
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
				else
				{
					field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);
				}

				field.Box.TabIndex = ++tabIndex;

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
				Dock    = DockStyle.Left,
				Padding = new Margins (10, 10, 5, 5),
			};

			var columns = new List<FrameBox> ();

			for (int i = 0; i < 4; i++)
			{
				var column = new FrameBox
				{
					Parent         = group,
					PreferredWidth = 170,
					Dock           = DockStyle.Left,
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

				int column = rank++/6;  // 6 boutons par colonne
				if (column >= columns.Count)
				{
					break;
				}

				this.CreatePrésentationButton (columns[column], cmd);
			}

			{
				var right = new FrameBox
				{
					Parent         = group,
					PreferredWidth = 60,
					Dock           = DockStyle.Left,
				};

				this.zeroPrésentationButton = new Button
				{
					Parent          = right,
					Text            = "Aucune",
					PreferredWidth  = 60,
					PreferredHeight = 20,
					Dock            = DockStyle.Top,
					Margins         = new Margins (0, 0, 0, 2),
				};

				this.allPrésentationButton = new Button
				{
					Parent          = right,
					Text            = "Toutes",
					PreferredWidth  = 60,
					PreferredHeight = 20,
					Dock            = DockStyle.Top,
					Margins         = new Margins (0, 0, 0, 2),
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

			var button = new CheckButton
			{
				Parent          = parent,
				FormattedText   = icon + " " + Converters.GetPrésentationCommandDescription (cmd),
				Name            = Converters.PrésentationCommandToString (cmd),
				PreferredHeight = 24,
				Dock            = DockStyle.Top,
			};

			this.checkButtons.Add (button);

			button.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					(this.dataAccessor.EditionLine[0] as UtilisateursEditionLine).SetPrésenttion (cmd, button.ActiveState == ActiveState.Yes);
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
				bool admin = (this.dataAccessor.EditionLine[0] as UtilisateursEditionLine).IsAdmin;

				foreach (var button in this.checkButtons)
				{
					var cmd = Converters.StringToPrésentationCommand(button.Name);
					var state = (this.dataAccessor.EditionLine[0] as UtilisateursEditionLine).HasPrésentation(cmd);

					button.ActiveState = state || admin ? ActiveState.Yes : ActiveState.No;
					button.Enable = !admin;
				}

				this.zeroPrésentationButton.Enable = !admin;
				this.allPrésentationButton.Enable = !admin;
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
