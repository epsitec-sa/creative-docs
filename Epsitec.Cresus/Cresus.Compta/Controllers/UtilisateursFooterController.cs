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
				else
				{
					field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.FooterTextChanged);
					field.CreateUI (footerFrame);
				}

				if (mapper.Column == ColumnType.MotDePasse)
				{
					var f = field.EditWidget as AbstractTextField;
					f.IsPassword = true;
					f.PasswordReplacementCharacter = '●';
				}

				field.Box.TabIndex = ++tabIndex;

				this.fieldControllers[line].Add (field);
			}
		}

		private void CreateButtonsUI(Widget parent)
		{
			var userAccess = this.UserAccess;

			var column1 = new FrameBox
			{
				Parent         = parent,
				PreferredWidth = 120,
				Dock           = DockStyle.Left,
			};

			var column2 = new FrameBox
			{
				Parent         = parent,
				PreferredWidth = 150*2,
				Dock           = DockStyle.Left,
			};

			this.adminButton = new CheckButton
			{
				Parent = column1,
				Text   = "Administrateur",
				Dock   = DockStyle.Top,
			};

			var group = new GroupBox
			{
				Parent  = column2,
				Text    = "Présentations accessibles",
				Dock    = DockStyle.Fill,
				Padding = new Margins (10, 10, 5, 5),
			};

			var group1 = new FrameBox
			{
				Parent         = group,
				PreferredWidth = 150,
				Dock           = DockStyle.Left,
			};

			var group2 = new FrameBox
			{
				Parent         = group,
				PreferredWidth = 150,
				Dock           = DockStyle.Left,
			};

			{
				this.planComptableButton = new CheckButton
				{
					Parent = group1,
					Text   = "Plan comptable",
					Dock   = DockStyle.Top,
				};

				this.libellésButton = new CheckButton
				{
					Parent = group1,
					Text   = "Libellés usuels",
					Dock   = DockStyle.Top,
				};

				this.modèlesButton = new CheckButton
				{
					Parent = group1,
					Text   = "Ecritures modèles",
					Dock   = DockStyle.Top,
				};

				this.journauxButton = new CheckButton
				{
					Parent = group1,
					Text   = "Journaux",
					Dock   = DockStyle.Top,
				};
			}

			{
				this.périodesButton = new CheckButton
				{
					Parent = group2,
					Text   = "Périodes comptables",
					Dock   = DockStyle.Top,
				};

				this.piècesGeneratorButton = new CheckButton
				{
					Parent = group2,
					Text   = "Générateurs de pièces",
					Dock   = DockStyle.Top,
				};

				this.utilisateursButton = new CheckButton
				{
					Parent = group2,
					Text   = "Utilisateurs",
					Dock   = DockStyle.Top,
				};

				this.réglagesButton = new CheckButton
				{
					Parent = group2,
					Text   = "Réglages",
					Dock   = DockStyle.Top,
				};
			}

			this.réglagesButton.ActiveStateChanged += delegate
			{
				this.SetUserAccess (this.réglagesButton, UserAccess.Réglages);
			};

			this.utilisateursButton.ActiveStateChanged += delegate
			{
				this.SetUserAccess (this.utilisateursButton, UserAccess.Utilisateurs);
			};

			this.piècesGeneratorButton.ActiveStateChanged += delegate
			{
				this.SetUserAccess (this.piècesGeneratorButton, UserAccess.PiécesGenerator);
			};

			this.libellésButton.ActiveStateChanged += delegate
			{
				this.SetUserAccess (this.libellésButton, UserAccess.Libellés);
			};

			this.modèlesButton.ActiveStateChanged += delegate
			{
				this.SetUserAccess (this.modèlesButton, UserAccess.Modèles);
			};

			this.journauxButton.ActiveStateChanged += delegate
			{
				this.SetUserAccess (this.journauxButton, UserAccess.Journaux);
			};

			this.périodesButton.ActiveStateChanged += delegate
			{
				this.SetUserAccess (this.périodesButton, UserAccess.Périodes);
			};

			this.planComptableButton.ActiveStateChanged += delegate
			{
				this.SetUserAccess (this.planComptableButton, UserAccess.PlanComptable);
			};
		}

		private void SetUserAccess(CheckButton button, UserAccess mode)
		{
			if (this.ignoreChanges.IsZero)
			{
				this.SetUserAccess (mode, button.ActiveState == ActiveState.Yes);
				this.FooterTextChanged ();
			}
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
				this.GetUserAccess (this.adminButton,           UserAccess.Admin          );
				this.GetUserAccess (this.réglagesButton,        UserAccess.Réglages       );
				this.GetUserAccess (this.utilisateursButton,    UserAccess.Utilisateurs   );
				this.GetUserAccess (this.piècesGeneratorButton, UserAccess.PiécesGenerator);
				this.GetUserAccess (this.libellésButton,        UserAccess.Libellés       );
				this.GetUserAccess (this.modèlesButton,         UserAccess.Modèles        );
				this.GetUserAccess (this.journauxButton,        UserAccess.Journaux       );
				this.GetUserAccess (this.périodesButton,        UserAccess.Périodes       );
				this.GetUserAccess (this.planComptableButton,   UserAccess.PlanComptable  );
			}
		}

		private void GetUserAccess(CheckButton button, UserAccess mode)
		{
			if (mode == UserAccess.Admin)
			{
				button.Enable = false;  // on ne peut jamais changer ce mode !
			}
			else
			{
				button.Enable = !this.GetUserAccess (UserAccess.Admin);
			}

			button.ActiveState =  this.GetUserAccess (mode) ? ActiveState.Yes : ActiveState.No;
		}


		private bool GetUserAccess(UserAccess mode)
		{
			return (this.UserAccess & mode) != 0;
		}

		private void SetUserAccess(UserAccess mode, bool state)
		{
			if (state)
			{
				this.UserAccess |= mode;
			}
			else
			{
				this.UserAccess &= ~mode;
			}
		}

		private UserAccess UserAccess
		{
			get
			{
				return (this.dataAccessor.EditionLine[0] as UtilisateursEditionLine).UserAccess;
			}
			set
			{
				(this.dataAccessor.EditionLine[0] as UtilisateursEditionLine).UserAccess = value;
			}
		}


		protected override FormattedText GetOperationDescription(bool modify)
		{
			return modify ? "Modification d'un utilisateur :" : "Création d'un utilisateur :";
		}


		private readonly SafeCounter		ignoreChanges;

		private FrameBox					linesFrame;
		private FrameBox					buttonsFrame;

		private CheckButton					adminButton;
		private CheckButton					réglagesButton;
		private CheckButton					utilisateursButton;
		private CheckButton					piècesGeneratorButton;
		private CheckButton					libellésButton;
		private CheckButton					modèlesButton;
		private CheckButton					journauxButton;
		private CheckButton					périodesButton;
		private CheckButton					planComptableButton;
	}
}
