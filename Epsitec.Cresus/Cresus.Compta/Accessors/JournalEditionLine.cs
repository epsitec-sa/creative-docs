//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données éditables pour le journal de la comptabilité.
	/// </summary>
	public class JournalEditionLine : AbstractEditionLine
	{
		public JournalEditionLine(AbstractController controller)
			: base (controller)
		{
			this.datas.Add (ColumnType.Date,    new EditionData (this.controller, this.ValidateDate));
			this.datas.Add (ColumnType.Débit,   new EditionData (this.controller, this.ValidateCompte));
			this.datas.Add (ColumnType.Crédit,  new EditionData (this.controller, this.ValidateCompte));
			this.datas.Add (ColumnType.Pièce,   new EditionData (this.controller));
			this.datas.Add (ColumnType.Libellé, new EditionData (this.controller, this.ValidateLibellé));
			this.datas.Add (ColumnType.Montant, new EditionData (this.controller, this.ValidateMontant));
			this.datas.Add (ColumnType.Journal, new EditionData (this.controller, this.ValidateJournal));
		}


		#region Validators
		private void ValidateDate(EditionData data)
		{
			Validators.ValidateDate (this.périodeEntity, data, emptyAccepted: false);
		}

		private void ValidateCompte(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				if (data.Text == JournalDataAccessor.multi)
				{
					return;
				}

				var n = PlanComptableDataAccessor.GetCompteNuméro (data.Text);
				var compte = this.comptaEntity.PlanComptable.Where (x => x.Numéro == n).FirstOrDefault ();

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
			Validators.ValidateMontant (data, emptyAccepted: false);
		}

		private void ValidateJournal(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var t = data.Text;
				if (!this.comptaEntity.Journaux.Where (x => x.Name == t).Any ())
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


		public override void EntityToData(AbstractEntity entity)
		{
			var écriture = entity as ComptaEcritureEntity;

			this.SetText (ColumnType.Date,             écriture.Date.ToString ());
			this.SetText (ColumnType.Débit,            JournalDataAccessor.GetNuméro (écriture.Débit));
			this.SetText (ColumnType.Crédit,           JournalDataAccessor.GetNuméro (écriture.Crédit));
			this.SetText (ColumnType.Pièce,            écriture.Pièce);
			this.SetText (ColumnType.Libellé,          écriture.Libellé);
			this.SetText (ColumnType.Montant,          écriture.Montant.ToString ("0.00"));
			this.SetText (ColumnType.TotalAutomatique, écriture.TotalAutomatique ? "True" : "False");
			this.SetText (ColumnType.Journal,          écriture.Journal.Name);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var écriture = entity as ComptaEcritureEntity;

			Date? date;
			if (this.périodeEntity.ParseDate (this.GetText (ColumnType.Date), out date))
			{
				écriture.Date = date.Value;
			}

			écriture.Débit  = JournalDataAccessor.GetCompte (this.comptaEntity, this.GetText (ColumnType.Débit));
			écriture.Crédit = JournalDataAccessor.GetCompte (this.comptaEntity, this.GetText (ColumnType.Crédit));

			écriture.Pièce   = this.GetText (ColumnType.Pièce);
			écriture.Libellé = this.GetText (ColumnType.Libellé);

			decimal montant;
			if (decimal.TryParse (this.GetText (ColumnType.Montant).ToSimpleText (), out montant))
			{
				écriture.Montant = montant;
			}

			écriture.TotalAutomatique = (this.GetText (ColumnType.TotalAutomatique) == "True");

			var journal = JournalDataAccessor.GetJournal (this.comptaEntity, this.GetText (ColumnType.Journal));
			if (journal == null)  // dans un journal spécifique ?
			{
				//	Normalement, le journal a déjà été initialisé. Mais si ce n'est pas le cas, on met le premier,
				//	car il est impératif qu'une écriture ait un journal !
				if (écriture.Journal == null)
				{
					écriture.Journal = this.comptaEntity.Journaux.FirstOrDefault ();
				}
			}
			else  // mode "tous les journaux" ?
			{
				//	Utilise le journal choisi par l'utilisateur dans le widget ad-hoc.
				écriture.Journal = journal;
			}
		}
	}
}