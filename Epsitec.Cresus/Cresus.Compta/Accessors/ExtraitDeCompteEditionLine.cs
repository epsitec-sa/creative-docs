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
			Validators.ValidateText (data, "Il manque le libellé");
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
		#endregion


		public override void EntityToData(AbstractData data)
		{
			var extrait = data as ExtraitDeCompteData;
			var écriture = data.Entity as ComptaEcritureEntity;

			this.SetText (ColumnType.Date,    Converters.DateToString (écriture.Date));
			this.SetText (ColumnType.Pièce,   écriture.Pièce);
			this.SetText (ColumnType.Libellé, écriture.Libellé);
			
			if (extrait.IsDébit)
			{
				this.SetText (ColumnType.CP,    JournalDataAccessor.GetNuméro (écriture.Débit));
				this.SetText (ColumnType.Débit, Converters.MontantToString (écriture.Montant));
			}
			else
			{
				this.SetText (ColumnType.CP,     JournalDataAccessor.GetNuméro (écriture.Crédit));
				this.SetText (ColumnType.Crédit, Converters.MontantToString (écriture.Montant));
			}
		}

		public override void DataToEntity(AbstractData data)
		{
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
				écriture.Débit   = JournalDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.CP));
				écriture.Montant = Converters.ParseMontant (this.GetText (ColumnType.Débit)).GetValueOrDefault ();
			}
			else
			{
				écriture.Crédit  = JournalDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.CP));
				écriture.Montant = Converters.ParseMontant (this.GetText (ColumnType.Crédit)).GetValueOrDefault ();
			}
		}
	}
}