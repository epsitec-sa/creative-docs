//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Assistants.Data
{
	/// <summary>
	/// Données éditables génériques d'un assistant de la comptabilité.
	/// </summary>
	public class AssistantEcritureTVA : AbstractEditionLine
	{
		public AssistantEcritureTVA(AbstractController controller)
			: base (controller)
		{
			this.dataDict.Add (ColumnType.Date,        new EditionData (this.controller, this.ValidateDate));
			this.dataDict.Add (ColumnType.Débit,       new EditionData (this.controller, this.ValidateCompte));
			this.dataDict.Add (ColumnType.Crédit,      new EditionData (this.controller, this.ValidateCompte));
			this.dataDict.Add (ColumnType.Pièce,       new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Libellé,     new EditionData (this.controller, this.ValidateLibellé));
			this.dataDict.Add (ColumnType.MontantTTC,  new EditionData (this.controller, this.ValidateMontant));
			this.dataDict.Add (ColumnType.CodeTVA,     new EditionData (this.controller, this.ValidateCodeTVA));
			this.dataDict.Add (ColumnType.Journal,     new EditionData (this.controller, this.ValidateJournal));

			this.dataDict.Add (ColumnType.DateDébut,   new EditionData (this.controller));
			this.dataDict.Add (ColumnType.DateFin,     new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Jours1,      new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Jours2,      new EditionData (this.controller));
			this.dataDict.Add (ColumnType.TauxTVA1,    new EditionData (this.controller, this.ValidateTauxTVA));
			this.dataDict.Add (ColumnType.TauxTVA2,    new EditionData (this.controller, this.ValidateTauxTVA));
			this.dataDict.Add (ColumnType.MontantTVA1, new EditionData (this.controller, this.ValidateMontantTVA));
			this.dataDict.Add (ColumnType.MontantTVA2, new EditionData (this.controller, this.ValidateMontantTVA));
			this.dataDict.Add (ColumnType.MontantHT1,  new EditionData (this.controller, this.ValidateMontantTVA));
			this.dataDict.Add (ColumnType.MontantHT2,  new EditionData (this.controller, this.ValidateMontantTVA));
		}


		#region Validators
		private void ValidateDate(EditionData data)
		{
			Validators.ValidateDate (this.période, data, emptyAccepted: false);
		}

		private void ValidateCompte(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var compte = this.compta.PlanComptable.Where (x => x.Numéro == data.Text).FirstOrDefault ();

				if (compte == null)
				{
					data.Error = "Ce compte n'existe pas";
					return;
				}

				if (compte.Type == TypeDeCompte.Groupe)
				{
					data.Error = "C'est un compte de groupement";
					return;
				}
			}
			else
			{
				data.Error = "Il manque le numéro du compte";
			}
		}

		private void ValidateLibellé(EditionData data)
		{
			Validators.ValidateText (data, "Il manque le libellé");
		}

		private void ValidateMontantTVA(EditionData data)
		{
			Validators.ValidateMontant (data, emptyAccepted: false);
		}

		private void ValidateMontant(EditionData data)
		{
			if (!this.controller.SettingsList.GetBool (SettingsType.EcritureMontantZéro) &&  // refuse les montants nuls ?
				data.Text == Converters.MontantToString (0))  // montant nul ?
			{
				data.Error = "Le montant ne peut pas être nul";
				return;
			}

			Validators.ValidateMontant (data, emptyAccepted: false);
		}

		private void ValidateCodeTVA(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var edit = data.Text.ToSimpleText ();
				var code = this.compta.CodesTVA.Where (x => x.Code == edit).FirstOrDefault ();
				if (code == null)
				{
					data.Error = "Ce code TVA n'existe pas";
					return;
				}
				else
				{
					var débit = PlanComptableDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.Débit));
					if (débit != null && débit.Type == TypeDeCompte.TVA)
					{
						data.Error = string.Format ("Il est interdit d'utiliser un code TVA avec le compte de TVA {0}", débit.Numéro);
						return;
					}

					var crédit = PlanComptableDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.Crédit));
					if (crédit != null && crédit.Type == TypeDeCompte.TVA)
					{
						data.Error = string.Format ("Il est interdit d'utiliser un code TVA avec le compte de TVA {0}", crédit.Numéro);
						return;
					}
				}
			}
			else
			{
				data.Error = "Il manque le code TVA";
			}
		}

		private void ValidateTauxTVA(EditionData data)
		{
			if (!data.HasText)
			{
				data.Error = "Vous devez donner un taux de TVA";
				return;
			}

			Validators.ValidatePercent (data, emptyAccepted: false);
		}

		private void ValidateJournal(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var t = data.Text;
				if (!this.compta.Journaux.Where (x => x.Nom == t).Any ())
				{
					data.Error = "Ce journal n'existe pas";
				}
			}
			else
			{
				data.Error = "Il manque le journal";
			}
		}
		#endregion


	}
}