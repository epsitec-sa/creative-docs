//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données éditables pour une écriture modèle de la comptabilité.
	/// </summary>
	public class ModèlesEditionLine : AbstractEditionLine
	{
		public ModèlesEditionLine(AbstractController controller)
			: base (controller)
		{
			this.dataDict.Add (ColumnType.Code,      new EditionData (this.controller, this.ValidateCode));
			this.dataDict.Add (ColumnType.Raccourci, new EditionData (this.controller, this.ValidateRaccourci));
			this.dataDict.Add (ColumnType.Débit,     new EditionData (this.controller, this.ValidateCompte));
			this.dataDict.Add (ColumnType.Crédit,    new EditionData (this.controller, this.ValidateCompte));
			this.dataDict.Add (ColumnType.Pièce,     new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Libellé,   new EditionData (this.controller, this.ValidateLibellé));
			this.dataDict.Add (ColumnType.Montant,   new EditionData (this.controller, this.ValidateMontant));
		}


		#region Validators
		private void ValidateCode(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				if (data.Text == Converters.RaccourciToString (RaccourciModèle.None))
				{
					return;  // plusieurs écritures modèles peuvent avoir "aucun" raccourci !
				}

				var compte = this.compta.Modèles.Where (x => x.Code == data.Text).FirstOrDefault ();
				if (compte == null)
				{
					return;
				}

				var himself = (this.controller.DataAccessor.JustCreated) ? null : this.controller.DataAccessor.GetEditionEntity (this.controller.DataAccessor.FirstEditedRow) as ComptaModèleEntity;
				if (himself != null && himself.Code == data.Text)
				{
					return;
				}

				data.Error = "Ce code est déjà utilisé";
			}
			else
			{
				data.Error = "Il manque le code";
			}
		}

		private void ValidateRaccourci(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				if (data.Text == Converters.RaccourciToString (RaccourciModèle.None))
				{
					return;  // plusieurs écritures modèles peuvent avoir "aucun" raccourci !
				}

				var compte = this.compta.Modèles.Where (x => x.Raccourci == data.Text).FirstOrDefault ();
				if (compte == null)
				{
					return;
				}

				var himself = (this.controller.DataAccessor.JustCreated) ? null : this.controller.DataAccessor.GetEditionEntity (this.controller.DataAccessor.FirstEditedRow) as ComptaModèleEntity;
				if (himself != null && himself.Raccourci == data.Text)
				{
					return;
				}

				data.Error = "Ce raccourci est déjà utilisé";
			}
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

				if (compte.Type != TypeDeCompte.Normal)
				{
					data.Error = "Ce compte n'a pas le type \"Normal\"";
					return;
				}
			}
		}

		private void ValidateLibellé(EditionData data)
		{
			Validators.ValidateText (data, "Il manque le libellé");
		}

		private void ValidateMontant(EditionData data)
		{
			Validators.ValidateMontant (data, this.compta.Monnaies[0], emptyAccepted: true);
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var modèle = entity as ComptaModèleEntity;

			this.SetText (ColumnType.Code,      modèle.Code);
			this.SetText (ColumnType.Raccourci, modèle.Raccourci);
			this.SetText (ColumnType.Débit,     ModèlesDataAccessor.GetNuméro (modèle.Débit));
			this.SetText (ColumnType.Crédit,    ModèlesDataAccessor.GetNuméro (modèle.Crédit));
			this.SetText (ColumnType.Pièce,     modèle.Pièce);
			this.SetText (ColumnType.Libellé,   modèle.Libellé);
			this.SetText (ColumnType.Montant,   Converters.MontantToString (modèle.Montant, this.compta.Monnaies[0]));
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var modèle = entity as ComptaModèleEntity;

			modèle.Débit  = ModèlesDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.Débit));
			modèle.Crédit = ModèlesDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.Crédit));

			modèle.Code      = this.GetText (ColumnType.Code).ToSimpleText ();
			modèle.Raccourci = this.GetText (ColumnType.Raccourci).ToSimpleText ();
			modèle.Pièce     = this.GetText (ColumnType.Pièce);
			modèle.Libellé   = this.GetText (ColumnType.Libellé);
			modèle.Montant   = Converters.ParseMontant (this.GetText (ColumnType.Montant));
		}
	}
}