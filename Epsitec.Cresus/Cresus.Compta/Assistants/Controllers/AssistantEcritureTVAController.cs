//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Settings.Data;
using Epsitec.Cresus.Compta.Fields.Controllers;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Assistants.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Assistants.Controllers
{
	/// <summary>
	/// Contrôleur générique pour un assistant la comptabilité.
	/// </summary>
	public class AssistantEcritureTVAController : AbstractAssistantController
	{
		public AssistantEcritureTVAController(AbstractController controller)
			: base (controller)
		{
			this.editionLine = new AssistantEcritureTVA (this.controller);
			this.data = new AssistantEcritureTVAData ();

			var écritureTVAData = this.data as AssistantEcritureTVAData;
			écritureTVAData.Date      = this.controller.MainWindowController.Période.ProchaineDate;
			//?écritureTVAData.Pièce     = this.controller.MainWindowController.PiècesGenerator.GetProchainePièce (this.GetDefaultJournal);
			écritureTVAData.DateDébut = Date.Today;
			écritureTVAData.DateFin   = Date.Today;
			écritureTVAData.Journal   = this.compta.Journaux.FirstOrDefault ();

			this.editionLine.EntityToData (this.data);
		}


		public override FrameBox CreateUI(Widget parent)
		{
			this.fieldControllers.Clear ();

			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			int tabIndex = 0;

			new StaticText
			{
				Parent          = frame,
				FormattedText   = "Création d'une écriture pour une prestation avec TVA sur une période :",
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (1, 0, 1, 0),
			};

			var line0 = new FrameBox
			{
				Parent   = frame,
				Dock     = DockStyle.Top,
				Margins  = new Margins (0, 0, 0, 5),
				TabIndex = ++tabIndex,
			};

			var line1 = new FrameBox
			{
				Parent   = frame,
				Dock     = DockStyle.Top,
				Margins  = new Margins (0, 0, 0, -1),
				TabIndex = ++tabIndex,
			};

			var line2 = new FrameBox
			{
				Parent   = frame,
				Dock     = DockStyle.Top,
				Margins  = new Margins (0, 0, 0, 0),
				TabIndex = ++tabIndex,
			};

			this.CreateTopLineUI (line0);
			this.CreateAdditionnalLineUI (line1, 1);
			this.CreateAdditionnalLineUI (line2, 2);

			return frame;
		}

		private void CreateTopLineUI(Widget parent)
		{
			int line = 0;
			int tabIndex = 0;

			var comptes = this.compta.PlanComptable.Where (x => x.Type != TypeDeCompte.Groupe);

			foreach (var mapper in this.columnMappers.Where (x => x.LineLayout == 0))
			{
				AbstractFieldController field;

				if (mapper.Column == ColumnType.Date)
				{
					field = new DateFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (parent);
				}
				else if (mapper.Column == ColumnType.Débit)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.CompteChanged);
					field.CreateUI (parent);

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, comptes);
				}
				else if (mapper.Column == ColumnType.Crédit)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.CompteChanged);
					field.CreateUI (parent);

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, comptes);
				}
				else if (mapper.Column == ColumnType.Libellé)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (parent);
					(field.EditWidget as AutoCompleteTextField).AcceptFreeText = true;

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, this.compta.GetLibellésDescriptions (this.période).ToArray ());

					//?this.CreateButtonModèleUI (field, line);
				}
				else if (mapper.Column == ColumnType.CodeTVA)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.CodeTVAChanged);
					field.CreateUI (parent);

					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, '#', this.compta.CodesTVAMenuDescription);
				}
				else if (mapper.Column == ColumnType.TauxTVA)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.TauxTVAChanged);
					field.CreateUI (parent);
				}
				else if (mapper.Column == ColumnType.Journal)
				{
					field = new AutoCompleteFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (parent);

					var journaux = this.compta.Journaux.Select (x => x.Nom);
					UIBuilder.UpdateAutoCompleteTextField (field.EditWidget as AutoCompleteTextField, journaux.ToArray ());
				}
				else if (mapper.Column == ColumnType.MontantTTC)
				{
					field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.MontantTTCChanged);
					field.CreateUI (parent);
				}
				else
				{
					field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (parent);
				}

				if (mapper.Column == ColumnType.MontantTTC)
				{
					field.EditWidget.ContentAlignment = ContentAlignment.MiddleRight;
				}

				field.Box.TabIndex = ++tabIndex;

				this.fieldControllers.Add (field);
			}
		}

		private void CreateAdditionnalLineUI(Widget parent, int line)
		{
			int tabIndex = 0;

			foreach (var mapper in this.columnMappers.Where (x => x.LineLayout == line))
			{
				bool docked = false;
				bool jours = false;

				if (mapper.Column == ColumnType.DateDébut ||
					mapper.Column == ColumnType.DateFin   )
				{
					new StaticText
					{
						Parent           = parent,
						FormattedText    = mapper.Description,
						ContentAlignment = ContentAlignment.MiddleRight,
						PreferredWidth   = 100,
						Dock             = DockStyle.Left,
						Margins          = new Margins (0, 5, 0, 0),
					};

					docked = true;
				}

				if (mapper.Column == ColumnType.Jours1 ||
					mapper.Column == ColumnType.Jours2)
				{
					new StaticText
					{
						Parent           = parent,
						FormattedText    = mapper.Description,
						ContentAlignment = ContentAlignment.MiddleRight,
						PreferredWidth   = 200,
						Dock             = DockStyle.Left,
						Margins          = new Margins (0, 5, 0, 0),
					};

					docked = true;
					jours = true;
				}

				AbstractFieldController field;

				if (mapper.Column == ColumnType.DateDébut ||
					mapper.Column == ColumnType.DateFin   )
				{
					field = new DateFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (parent);
				}
				else
				{
					field = new TextFieldController (this.controller, line, mapper, this.HandleSetFocus, this.EditorTextChanged);
					field.CreateUI (parent);
				}

				if (mapper.Column == ColumnType.Jours1 ||
					mapper.Column == ColumnType.Jours2 )
				{
					new StaticText
					{
						Parent           = parent,
						FormattedText    = "jours",
						ContentAlignment = ContentAlignment.MiddleLeft,
						PreferredWidth   = 40,
						Dock             = DockStyle.Left,
						Margins          = new Margins (5, 0, 0, 0),
					};

					docked = true;
					jours = true;
				}

				if (docked)
				{
					field.Box.PreferredWidth = jours ? 35 : 60;
					field.Box.Anchor = AnchorStyles.None;
					field.Box.Dock = DockStyle.Left;
				}

				if (mapper.Column == ColumnType.Jours1      ||
					mapper.Column == ColumnType.Jours2      ||
					mapper.Column == ColumnType.MontantTTC1 ||
					mapper.Column == ColumnType.MontantTTC2 ||
					mapper.Column == ColumnType.MontantTVA1 ||
					mapper.Column == ColumnType.MontantTVA2 ||
					mapper.Column == ColumnType.MontantHT1  ||
					mapper.Column == ColumnType.MontantHT2  ||
					mapper.Column == ColumnType.TauxTVA1    ||
					mapper.Column == ColumnType.TauxTVA2    )
				{
					field.EditWidget.ContentAlignment = ContentAlignment.MiddleRight;
				}

				field.Box.TabIndex = ++tabIndex;

				this.fieldControllers.Add (field);
			}
		}


		private void HandleSetFocus(int line, ColumnType columnType)
		{
		}

		private void CompteChanged()
		{
			this.EditorTextChanged ();
		}

		private void CodeTVAChanged()
		{
			this.EditorTextChanged ();
		}

		private void MontantTTCChanged()
		{
			this.EditorTextChanged ();
		}

		private void TauxTVAChanged()
		{
			this.EditorTextChanged ();
		}

		private void MontantBrutChanged()
		{
			this.EditorTextChanged ();
		}

		private void MontantTVAChanged()
		{
			this.EditorTextChanged ();
		}


		public override void UpdateGeometry()
		{
			base.UpdateGeometry ();

			this.UpdateColumnGeometry (ColumnType.MontantTTC1, ColumnType.MontantTTC);
			this.UpdateColumnGeometry (ColumnType.MontantTTC2, ColumnType.MontantTTC);
			this.UpdateColumnGeometry (ColumnType.MontantTVA1, ColumnType.MontantTVA);
			this.UpdateColumnGeometry (ColumnType.MontantTVA2, ColumnType.MontantTVA);
			this.UpdateColumnGeometry (ColumnType.MontantHT1, ColumnType.MontantHT);
			this.UpdateColumnGeometry (ColumnType.MontantHT2, ColumnType.MontantHT);
			this.UpdateColumnGeometry (ColumnType.TauxTVA1, ColumnType.TauxTVA);
			this.UpdateColumnGeometry (ColumnType.TauxTVA2, ColumnType.TauxTVA);
		}


		protected override void InitializeColumnMappers()
		{
			this.columnMappers.Clear ();

			var options = this.dataAccessor.Options as JournalOptions;

			this.columnMappers.Add (new ColumnMapper (ColumnType.Date, "Date", "Date de l'écriture"));
			this.columnMappers.Add (new ColumnMapper (ColumnType.Débit, "Débit", "Numéro ou nom du compte à débiter"));
			this.columnMappers.Add (new ColumnMapper (ColumnType.Crédit, "Crédit", "Numéro ou nom du compte à créditer"));

			if (this.settingsList.GetBool (SettingsType.EcriturePièces))
			{
				bool enable = !this.settingsList.GetBool (SettingsType.EcritureForcePièces);
				this.columnMappers.Add (new ColumnMapper (ColumnType.Pièce, "Pièce", "Numéro de la pièce comptable correspondant à l'écriture", enable: enable));
			}

			this.columnMappers.Add (new ColumnMapper (ColumnType.Libellé, "Libellé", "Libellé de l'écriture"));
			this.columnMappers.Add (new ColumnMapper (ColumnType.MontantTTC, "Montant TTC", "Montant de l'écriture"));
			this.columnMappers.Add (new ColumnMapper (ColumnType.CodeTVA, "Code TVA", "Code TVA", enable: this.settingsList.GetBool (SettingsType.EcritureEditeCodeTVA)));
			this.columnMappers.Add (new ColumnMapper (ColumnType.CompteTVA, "Compte TVA", "Compte de la TVA", enable: false, show: this.settingsList.GetBool (SettingsType.EcritureMontreCompteTVA)));
			this.columnMappers.Add (new ColumnMapper (ColumnType.Journal, "Journal", "Journal auquel appartient l'écriture", show: options.JournalId == 0));  // tous les journaux ?

			this.columnMappers.Add (new ColumnMapper (ColumnType.DateDébut, "Début prestation", "Date de début de la prestation", lineLayout: 1));
			this.columnMappers.Add (new ColumnMapper (ColumnType.DateFin, "Fin prestation", "Date de fin de la prestation", lineLayout: 2));
			this.columnMappers.Add (new ColumnMapper (ColumnType.Jours1, "Première écriture", "Nombre de jours", lineLayout: 1, enable: false));
			this.columnMappers.Add (new ColumnMapper (ColumnType.Jours2, "Deuxième écriture", "Nombre de jours", lineLayout: 2, enable: false));
			this.columnMappers.Add (new ColumnMapper (ColumnType.MontantTTC1, "Montant TTC", "Montant de l'écriture", lineLayout: 1));
			this.columnMappers.Add (new ColumnMapper (ColumnType.MontantTTC2, "Montant TTC", "Montant de l'écriture", lineLayout: 2));
			this.columnMappers.Add (new ColumnMapper (ColumnType.MontantTVA1, "TVA", "Montant de la TVA", lineLayout: 1, enable: this.settingsList.GetBool (SettingsType.EcritureEditeMontantTVA)));
			this.columnMappers.Add (new ColumnMapper (ColumnType.MontantTVA2, "TVA", "Montant de la TVA", lineLayout: 2, enable: this.settingsList.GetBool (SettingsType.EcritureEditeMontantTVA)));
			this.columnMappers.Add (new ColumnMapper (ColumnType.MontantHT1, "Montant HT", "Montant de l'écriture sans la TVA", lineLayout: 1, enable: this.settingsList.GetBool (SettingsType.EcritureEditeMontantHT)));
			this.columnMappers.Add (new ColumnMapper (ColumnType.MontantHT2, "Montant HT", "Montant de l'écriture sans la TVA", lineLayout: 2, enable: this.settingsList.GetBool (SettingsType.EcritureEditeMontantHT)));
			this.columnMappers.Add (new ColumnMapper (ColumnType.TauxTVA1, "Taux", "Taux de la TVA", lineLayout: 1, enable: this.settingsList.GetBool (SettingsType.EcritureEditeTauxTVA)));
			this.columnMappers.Add (new ColumnMapper (ColumnType.TauxTVA2, "Taux", "Taux de la TVA", lineLayout: 2, enable: this.settingsList.GetBool (SettingsType.EcritureEditeTauxTVA)));
		}
	}
}
