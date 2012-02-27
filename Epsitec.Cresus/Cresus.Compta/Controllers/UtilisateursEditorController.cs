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
	public class UtilisateursEditorController : AbstractEditorController
	{
		public UtilisateursEditorController(AbstractController controller)
			: base (controller)
		{
			this.checkPrésentationsButtons = new List<CheckButton> ();
			this.ignoreChanges = new SafeCounter ();
		}


		public override void CreateUI(FrameBox parent, System.Action updateArrayContentAction)
		{
			var band = this.CreateRightEditorUI (parent);

			this.buttonsFrame = new FrameBox
			{
				Parent              = band,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth      = 100,
				Dock                = DockStyle.Right,
				Margins             = new Margins (0, 0, 0, 0),
			};

			this.linesFrame = new FrameBox
			{
				Parent              = band,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredWidth      = 180,
				Dock                = DockStyle.Right,
				Margins             = new Margins (0, 10, 0, 0),
			};

			this.CreateLineUI (this.linesFrame);
			this.CreatePrésentationsButtonsUI (this.buttonsFrame);

			base.CreateUI (parent, updateArrayContentAction);
		}

		private void CreateLineUI(Widget parent)
		{
			//	Comme on est en mode HasRightEditor, la "ligne" est en fait une colonne dans laquelle
			//	on empile les choses de haut en bas.
			this.fieldControllers.Add (new List<AbstractFieldController> ());

			var editorFrame = new TabCatcherFrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Margins = new Margins (0, 0, 1, 0),
			};

			editorFrame.TabPressed += new TabCatcherFrameBox.TabPressedEventHandler (this.HandleLinesContainerTabPressed);

			this.linesFrames.Add (editorFrame);
			int line = this.linesFrames.Count - 1;
			int tabIndex = 0;

			editorFrame.TabIndex = line+1;

			foreach (var mapper in this.columnMappers.Where (x => x.Edition))
			{
				if (mapper.Column == ColumnType.IdentitéWindows ||
					mapper.Column == ColumnType.MotDePasse      )  // insère un gap ?
				{
					new FrameBox
					{
						Parent          = editorFrame,
						PreferredHeight = 10,
						Dock            = DockStyle.Top,
					};
				}

				AbstractFieldController field;

				if (mapper.Column == ColumnType.Pièce)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, this.comptaEntity.PiècesGenerator.Select (x => x.Nom).ToArray ());
				}
				else if (mapper.Column == ColumnType.DateDébut ||
						 mapper.Column == ColumnType.DateFin   )
				{
					field = new DateFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
				}
				else if (mapper.Column == ColumnType.MotDePasse)
				{
					field = new PasswordFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
				}
				else if (mapper.Column == ColumnType.IdentitéWindows ||
						 mapper.Column == ColumnType.Désactivé       )
				{
					field = new CheckButtonController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
				}
				else
				{
					field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (editorFrame);
				}

				field.Box.TabIndex = ++tabIndex;

				this.fieldControllers[line].Add (field);
			}
		}

		private void CreatePrésentationsButtonsUI(Widget parent)
		{
			this.checkPrésentationsButtons.Clear ();

			var list = Converters.PrésentationCommands.Where (x => x != Res.Commands.Présentation.Login);  //  Login est toujours accessible !
			int numberPerColumn = 16;  // 16 boutons par colonne
			int columnCount = (list.Count ()+numberPerColumn-1) / numberPerColumn;

			var group = new GroupBox
			{
				Parent  = parent,
				Text    = "Présentations",
				Dock    = DockStyle.Fill,
				Padding = new Margins (10, 0, 5, 5),
			};

			var top = new FrameBox
			{
				Parent = group,
				Dock   = DockStyle.Fill,
			};

			var columns = new List<FrameBox> ();

			for (int i = 0; i < columnCount; i++)
			{
				var column = new FrameBox
				{
					Parent         = top,
					PreferredWidth = 50,
					Dock           = DockStyle.Left,
				};

				columns.Add (column);
			}

			int rank = 0;
			foreach (var cmd in list)
			{
				int column = rank++/numberPerColumn;
				if (column >= columns.Count)
				{
					break;
				}

				this.CreatePrésentationButton (columns[column], cmd);
			}

			{
				var footer = new FrameBox
				{
					Parent              = group,
					ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.HorizontalFlow,
					PreferredHeight     = 20,
					Dock                = DockStyle.Bottom,
					Margins             = new Margins (-5, 10-5, 0, 0),
				};

				this.zeroPrésentationButton = new Button
				{
					Parent          = footer,
					Text            = "Aucune",
					PreferredHeight = 20,
					Dock            = DockStyle.Fill,
					Margins         = new Margins (0, 1, 0, 0),
				};

				this.allPrésentationButton = new Button
				{
					Parent          = footer,
					Text            = "Toutes",
					PreferredHeight = 20,
					Dock            = DockStyle.Fill,
					Margins         = new Margins (1, 0, 0, 0),
				};

				ToolTip.Default.SetToolTip (this.zeroPrésentationButton, "Interdit l'accès à toutes les présentations");
				ToolTip.Default.SetToolTip (this.allPrésentationButton,  "Autorise l'accès à toutes les présentations");
			}

			this.zeroPrésentationButton.Clicked += delegate
			{
				foreach (var button in this.checkPrésentationsButtons)
				{
					button.ActiveState = ActiveState.No;
				}
			};

			this.allPrésentationButton.Clicked += delegate
			{
				foreach (var button in this.checkPrésentationsButtons)
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
				FormattedText   = icon,  // juste l'icône, sans texte
				PreferredWidth  = 50,
				Name            = Converters.PrésentationCommandToString (cmd),
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			ToolTip.Default.SetToolTip (button, desc);

			this.checkPrésentationsButtons.Add (button);

			button.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.EditionLine.SetPrésenttion (cmd, button.ActiveState == ActiveState.Yes);
					this.EditorTextChanged ();
				}
			};
		}


		protected override void UpdateEditionWidgets()
		{
			base.UpdateEditionWidgets ();
			this.UpdatePrésentationsButtons ();
		}

		private void UpdatePrésentationsButtons()
		{
			using (this.ignoreChanges.Enter ())
			{
				bool admin = this.EditionLine.IsAdmin;
				bool zero  = true;
				bool all   = true;

				foreach (var button in this.checkPrésentationsButtons)
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


		private readonly List<CheckButton>	checkPrésentationsButtons;
		private readonly SafeCounter		ignoreChanges;

		private FrameBox					linesFrame;
		private FrameBox					buttonsFrame;
		private Button						zeroPrésentationButton;
		private Button						allPrésentationButton;
	}
}
