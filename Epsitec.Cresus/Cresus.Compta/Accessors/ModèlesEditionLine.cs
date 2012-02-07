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
	/// Données éditables pour une écriture modèle de la comptabilité.
	/// </summary>
	public class ModèlesEditionLine : AbstractEditionLine
	{
		public ModèlesEditionLine(AbstractController controller)
			: base (controller)
		{
			this.datas.Add (ColumnType.Code,      new EditionData (this.controller, this.ValidateCode));
			this.datas.Add (ColumnType.Débit,     new EditionData (this.controller, this.ValidateCompte));
			this.datas.Add (ColumnType.Crédit,    new EditionData (this.controller, this.ValidateCompte));
			this.datas.Add (ColumnType.Libellé,   new EditionData (this.controller, this.ValidateLibellé));
			this.datas.Add (ColumnType.Montant,   new EditionData (this.controller, this.ValidateMontant));
		}


		#region Validators
		private void ValidateCode(EditionData data)
		{
			Validators.ValidateText (data, "Il manque le code");
		}

		private void ValidateCompte(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
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
		}

		private void ValidateLibellé(EditionData data)
		{
			Validators.ValidateText (data, "Il manque le libellé");
		}

		private void ValidateMontant(EditionData data)
		{
			Validators.ValidateMontant (data, emptyAccepted: true);
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
			this.SetText (ColumnType.Montant,   modèle.Montant.HasValue ? modèle.Montant.Value.ToString ("0.00") : FormattedText.Empty);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var modèle = entity as ComptaModèleEntity;

			modèle.Débit  = ModèlesDataAccessor.GetCompte (this.comptaEntity, this.GetText (ColumnType.Débit));
			modèle.Crédit = ModèlesDataAccessor.GetCompte (this.comptaEntity, this.GetText (ColumnType.Crédit));

			modèle.Code      = this.GetText (ColumnType.Code).ToSimpleText ();
			modèle.Raccourci = this.GetText (ColumnType.Raccourci).ToSimpleText ();
			modèle.Pièce     = this.GetText (ColumnType.Pièce);
			modèle.Libellé   = this.GetText (ColumnType.Libellé);

			decimal montant;
			if (decimal.TryParse (this.GetText (ColumnType.Montant).ToSimpleText (), out montant))
			{
				modèle.Montant = montant;
			}
			else
			{
				modèle.Montant = null;
			}
		}
	}
}