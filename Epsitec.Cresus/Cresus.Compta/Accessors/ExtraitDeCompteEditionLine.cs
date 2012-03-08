//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données éditables pour l'extrait d'un compte du plan comptable de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteEditionLine : AbstractEditionLine
	{
		public ExtraitDeCompteEditionLine(AbstractController controller)
			: base (controller)
		{
			this.dataDict.Add (ColumnType.Date,    new EditionData (this.controller, this.ValidateDate));
			this.dataDict.Add (ColumnType.CP,      new EditionData (this.controller, this.ValidateCompte));
			this.dataDict.Add (ColumnType.Pièce,   new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Libellé, new EditionData (this.controller, this.ValidateLibellé));
			this.dataDict.Add (ColumnType.Débit,   new EditionData (this.controller, this.ValidateMontant));
			this.dataDict.Add (ColumnType.Crédit,  new EditionData (this.controller, this.ValidateMontant));
			this.dataDict.Add (ColumnType.Journal, new EditionData (this.controller, this.ValidateJournal));
		}


		#region Validators
		private void ValidateDate(EditionData data)
		{
			data.ClearError ();

			if (!data.Enable)
			{
				return;  // toujours ok si disable
			}

			Validators.ValidateDate (this.période, data, emptyAccepted: false);
		}

		private void ValidateCompte(EditionData data)
		{
			data.ClearError ();

			if (!data.Enable)
			{
				return;  // toujours ok si disable
			}

			if (data.HasText)
			{
				var n = PlanComptableDataAccessor.GetCompteNuméro (data.Text);
				var compte = this.compta.PlanComptable.Where (x => x.Numéro == n).FirstOrDefault ();

				if (compte == null)
				{
					data.Error = "Ce compte n'existe pas";
					return;
				}

				if (compte.Type != TypeDeCompte.Normal)
				{
					data.Error = "Ce compte n'a pas le type \"Normal\"";
					return;
				}

				data.Text = n;
			}
			else
			{
				data.Error = "Il manque le numéro du compte";
			}
		}

		private void ValidateLibellé(EditionData data)
		{
			data.ClearError ();

			if (!data.Enable)
			{
				return;  // toujours ok si disable
			}

			Validators.ValidateText (data, "Il manque le libellé");
		}

		private void ValidateMontant(EditionData data)
		{
			data.ClearError ();

			if (!data.Enable)
			{
				return;  // toujours ok si disable
			}

			if (!this.controller.SettingsList.GetBool (SettingsType.EcritureMontantZéro) &&  // refuse les montants nuls ?
				data.Text == Converters.MontantToString (0))  // montant nul ?
			{
				data.Error = "Le montant ne peut pas être nul";
				return;
			}

			Validators.ValidateMontant (data, emptyAccepted: false);
		}

		private void ValidateJournal(EditionData data)
		{
			data.ClearError ();

			if (!data.Enable)
			{
				return;  // toujours ok si disable
			}

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


		public override void EntityToData(AbstractData data)
		{
			//	ExtraitDeCompteData/ComptaEcritureEntity -> EditionData
			var extrait = data as ExtraitDeCompteData;
			var écriture = data.Entity as ComptaEcritureEntity;
			bool enable = (écriture != null);

			this.SetEnable (ColumnType.Date,    enable);
			this.SetEnable (ColumnType.CP,      enable);
			this.SetEnable (ColumnType.Pièce,   enable);
			this.SetEnable (ColumnType.Libellé, enable);
			this.SetEnable (ColumnType.Débit,   enable);
			this.SetEnable (ColumnType.Crédit,  enable);
			this.SetEnable (ColumnType.Journal, enable);

			if (écriture != null)
			{
				this.SetText (ColumnType.Date,    Converters.DateToString (écriture.Date));
				this.SetText (ColumnType.Pièce,   écriture.Pièce);
				this.SetText (ColumnType.Libellé, écriture.Libellé);
				this.SetText (ColumnType.Journal, écriture.Journal.Nom);

				if (extrait.IsDébit)
				{
					this.SetText (ColumnType.CP,    JournalDataAccessor.GetNuméro (écriture.Crédit));
					this.SetText (ColumnType.Débit, Converters.MontantToString (écriture.MontantTTC));

					this.SetEnable (ColumnType.CP,     écriture.Crédit != null);
					this.SetEnable (ColumnType.Débit,  écriture.MultiId == 0 || !écriture.TotalAutomatique);
					this.SetEnable (ColumnType.Crédit, false);
				}
				else
				{
					this.SetText (ColumnType.CP,     JournalDataAccessor.GetNuméro (écriture.Débit));
					this.SetText (ColumnType.Crédit, Converters.MontantToString (écriture.MontantTTC));

					this.SetEnable (ColumnType.CP,     écriture.Débit != null);
					this.SetEnable (ColumnType.Débit,  false);
					this.SetEnable (ColumnType.Crédit, écriture.MultiId == 0 || !écriture.TotalAutomatique);
				}
			}
		}

		public override void DataToEntity(AbstractData data)
		{
			//	EditionData -> ExtraitDeCompteData/ComptaEcritureEntity
			var extrait = data as ExtraitDeCompteData;
			var écriture = data.Entity as ComptaEcritureEntity;

			Date? date;
			if (this.période.ParseDate (this.GetText (ColumnType.Date), out date))
			{
				écriture.Date = date.Value;
			}

			écriture.Pièce   = this.GetText (ColumnType.Pièce);
			écriture.Libellé = this.GetText (ColumnType.Libellé);

			if (extrait.IsDébit)
			{
				écriture.Crédit     = JournalDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.CP));
				écriture.MontantTTC = Converters.ParseMontant (this.GetText (ColumnType.Débit)).GetValueOrDefault ();
			}
			else
			{
				écriture.Débit      = JournalDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.CP));
				écriture.MontantTTC = Converters.ParseMontant (this.GetText (ColumnType.Crédit)).GetValueOrDefault ();
			}

			var journal = JournalDataAccessor.GetJournal (this.compta, this.GetText (ColumnType.Journal));
			if (journal == null)  // dans un journal spécifique ?
			{
				//	Normalement, le journal a déjà été initialisé. Mais si ce n'est pas le cas, on met le premier,
				//	car il est impératif qu'une écriture ait un journal !
				if (écriture.Journal == null)
				{
					écriture.Journal = this.compta.Journaux.FirstOrDefault ();
				}
			}
			else  // mode "tous les journaux" ?
			{
				//	Utilise le journal choisi par l'utilisateur dans le widget ad-hoc.
				écriture.Journal = journal;
			}

			this.EcritureToExtrait (écriture, extrait);
		}

		private void EcritureToExtrait(ComptaEcritureEntity écriture, ExtraitDeCompteData extrait)
		{
			//	ComptaEcritureEntity -> ExtraitDeCompteData
			extrait.Date    = écriture.Date;
			extrait.Pièce   = écriture.Pièce;
			extrait.Libellé = écriture.Libellé;
			extrait.Journal = écriture.Journal.Nom;

			if (extrait.IsDébit)
			{
				extrait.CP    = écriture.Crédit;
				extrait.Débit = écriture.MontantTTC;
			}
			else
			{
				extrait.CP     = écriture.Débit;
				extrait.Crédit = écriture.MontantTTC;
			}
		}
	}
}