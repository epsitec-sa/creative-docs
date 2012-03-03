//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Permanents.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données de la balance de vérification de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteDataAccessor : AbstractDataAccessor
	{
		public ExtraitDeCompteDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.permanents = this.mainWindowController.GetSettingsPermanents<ExtraitDeComptePermanents> ("Présentation.ExtraitDeCompte.Permanents", this.compta);
			this.options    = this.mainWindowController.GetSettingsOptions<ExtraitDeCompteOptions> ("Présentation.ExtraitDeCompte.Options", this.compta);
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.ExtraitDeCompte.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData ("Présentation.ExtraitDeCompte.Filter");

			this.UpdateAfterOptionsChanged ();
		}


		public override bool IsEditionCreationEnable
		{
			get
			{
				return false;
			}
		}


		public override void FilterUpdate()
		{
			Date? beginDate, endDate;
			this.filterData.GetBeginnerDates (out beginDate, out endDate);

			if (this.lastBeginDate != beginDate || this.lastEndDate != endDate)
			{
				this.UpdateAfterOptionsChanged ();
			}

			base.FilterUpdate ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			this.readonlyAllData.Clear ();

			FormattedText numéroCompte = this.Permanents.NuméroCompte;
			if (numéroCompte.IsNullOrEmpty)
			{
				return;
			}

			this.filterData.GetBeginnerDates (out this.lastBeginDate, out this.lastEndDate);
			this.soldesJournalManager.Initialize (this.période.Journal, this.lastBeginDate, this.lastEndDate);

			var compte = this.compta.PlanComptable.Where (x => x.Numéro == numéroCompte).FirstOrDefault ();
			if (compte == null)
			{
				return;
			}

			foreach (var écriture in this.période.Journal)
			{
				if (!Dates.DateInRange (écriture.Date, this.lastBeginDate, this.lastEndDate))
				{
					continue;
				}

				bool débit  = (ExtraitDeCompteDataAccessor.Match (écriture.Débit,  numéroCompte));
				bool crédit = (ExtraitDeCompteDataAccessor.Match (écriture.Crédit, numéroCompte));

				if (débit)
				{
					var data = new ExtraitDeCompteData ()
					{
						IsDébit = true,
						Entity  = écriture,
						Date    = écriture.Date,
						Pièce   = écriture.Pièce,
						Libellé = écriture.Libellé,
						CP      = écriture.Crédit,
						Débit   = écriture.Montant,
						Journal = écriture.Journal.Nom,
					};

					this.readonlyAllData.Add (data);
				}

				if (crédit)
				{
					var data = new ExtraitDeCompteData ()
					{
						IsDébit = false,
						Entity  = écriture,
						Date    = écriture.Date,
						Pièce   = écriture.Pièce,
						Libellé = écriture.Libellé,
						CP      = écriture.Débit,
						Crédit  = écriture.Montant,
						Journal = écriture.Journal.Nom,
					};

					this.readonlyAllData.Add (data);
				}
			}

			this.SetBottomSeparatorToPreviousLine ();

			//	Génère la dernière ligne.
			{
				var data = new ExtraitDeCompteData ()
				{
					Libellé       = "Mouvement",
					IsItalic      = true,
					NeverFiltered = true,
				};

				this.readonlyAllData.Add (data);
			}

			this.UpdateSoldes ();

			this.FilterUpdate ();
		}

		private void UpdateSoldes()
		{
			this.MinMaxClear ();

			FormattedText numéroCompte = this.Permanents.NuméroCompte;
			if (numéroCompte.IsNullOrEmpty)
			{
				return;
			}

			var compte = this.compta.PlanComptable.Where (x => x.Numéro == numéroCompte).FirstOrDefault ();
			if (compte == null)
			{
				return;
			}

			decimal solde       = 0;
			decimal totalDébit  = 0;
			decimal totalCrédit = 0;

			foreach (var d in this.readonlyAllData)
			{
				var data = d as ExtraitDeCompteData;
				var écriture = data.Entity as ComptaEcritureEntity;

				if (data.NeverFiltered)  // dernière ligne "mouvement" ?
				{
					data.Débit  = totalDébit;
					data.Crédit = totalCrédit;
				}
				else
				{
					if (data.IsDébit)
					{
						solde      += écriture.Montant;
						totalDébit += écriture.Montant;

						if (compte.Catégorie == CatégorieDeCompte.Passif ||
							compte.Catégorie == CatégorieDeCompte.Produit)
						{
							solde = -solde;
						}

						data.Solde = solde;
					}
					else
					{
						solde       -= écriture.Montant;
						totalCrédit += écriture.Montant;

						if (compte.Catégorie == CatégorieDeCompte.Passif ||
							compte.Catégorie == CatégorieDeCompte.Produit)
						{
							solde = -solde;
						}

						data.Solde = solde;
					}

					this.SetMinMaxValue (solde);
				}
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var data = this.GetReadOnlyData (row, all) as ExtraitDeCompteData;

			if (data == null)
			{
				return FormattedText.Null;
			}

			switch (column)
			{
				case ColumnType.Date:
					if (data.Date.HasValue)
					{
						return data.Date.Value.ToString ();
					}
					else
					{
						return FormattedText.Empty;
					}

				case ColumnType.CP:
					return ExtraitDeCompteDataAccessor.GetNuméro (data.CP);

				case ColumnType.Pièce:
					return data.Pièce;

				case ColumnType.Libellé:
					return data.Libellé;

				case ColumnType.Débit:
					return Converters.MontantToString (data.Débit);

				case ColumnType.Crédit:
					return Converters.MontantToString (data.Crédit);

				case ColumnType.Solde:
					return Converters.MontantToString (data.Solde);

				case ColumnType.SoldeGraphique:
					return this.GetMinMaxText (data.Solde);

				case ColumnType.Journal:
					return data.Journal;

				default:
					return FormattedText.Null;
			}
		}


		public override void StartCreationLine()
		{
			this.editionLine.Clear ();

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isCreation = true;
			this.isModification = false;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.readonlyData.Count)
			{
				var extrait = new ExtraitDeCompteEditionLine (this.controller);
				var data = this.readonlyData[row];
				extrait.EntityToData (data);

				this.editionLine.Add (extrait);
				this.countEditedRow++;
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isCreation = false;
			this.isModification = true;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();
		}

		public override void UpdateEditionLine()
		{
			if (this.isModification)
			{
				this.UpdateModificationData ();
				this.justCreated = false;
			}

			this.SearchUpdate ();
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var data = this.readonlyData[row];
			var écriture = data.Entity as ComptaEcritureEntity;
			var initialDate    = écriture.Date;
			var initialMontant = écriture.Montant;

			this.editionLine[0].DataToEntity (data);

			if (écriture.Date != initialDate)  // changement de date ?
			{
				//...
			}

			if (écriture.Montant != initialMontant)  // changement de montant ?
			{
				this.UpdateSoldes ();
			}
		}


		private static FormattedText GetNuméro(ComptaCompteEntity compte)
		{
			if (compte == null)
			{
				return JournalDataAccessor.multi;
			}
			else
			{
				return compte.Numéro;
			}
		}

		private new ExtraitDeComptePermanents Permanents
		{
			get
			{
				return this.permanents as ExtraitDeComptePermanents;
			}
		}

		private new ExtraitDeCompteOptions Options
		{
			get
			{
				return this.options as ExtraitDeCompteOptions;
			}
		}


		private static bool Match(ComptaCompteEntity compte, FormattedText numéro)
		{
			//	Retroune true si le compte ou ses fils correspond au numéro.
			while (compte != null && !compte.Numéro.IsNullOrEmpty)
			{
				if (compte.Numéro == numéro)
				{
					return true;
				}

				compte = compte.Groupe;
			}

			return false;
		}

	}
}