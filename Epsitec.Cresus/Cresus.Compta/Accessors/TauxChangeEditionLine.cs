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
	/// Données éditables pour un taux de change de la comptabilité.
	/// </summary>
	public class TauxChangeEditionLine : AbstractEditionLine
	{
		public TauxChangeEditionLine(AbstractController controller)
			: base (controller)
		{
			this.dataDict.Add (ColumnType.Code,        new EditionData (this.controller, this.ValidateCode));
			this.dataDict.Add (ColumnType.Description, new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Cours,       new EditionData (this.controller, this.ValidateCours));
			this.dataDict.Add (ColumnType.Unité,       new EditionData (this.controller, this.ValidateUnité));
			this.dataDict.Add (ColumnType.CompteGain,  new EditionData (this.controller, this.ValidateCompte));
			this.dataDict.Add (ColumnType.ComptePerte, new EditionData (this.controller, this.ValidateCompte));
		}


		#region Validators
		private void ValidateCode(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var taux = this.compta.TauxChange.Where (x => x.CodeISO == data.Text).FirstOrDefault ();
				if (taux == null)
				{
					return;
				}

				var himself = (this.controller.DataAccessor.JustCreated || this.controller.EditorController.Duplicate) ? null : this.controller.DataAccessor.GetEditionEntity (this.controller.DataAccessor.FirstEditedRow) as ComptaTauxChangeEntity;
				if (himself != null && himself.CodeISO == data.Text)
				{
					return;
				}

				data.Error = "Cette monnaie existe déjà";
			}
			else
			{
				data.Error = "Il manque le code ISO de la monnaie";
			}
		}

		private void ValidateCours(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				decimal n;
				if (decimal.TryParse (data.Text.ToSimpleText (), out n))
				{
					return;
				}

				data.Error = "Vous devez donner une valeur du cours";
			}
			else
			{
				data.Error = "Il manque le cours";
			}
		}

		private void ValidateUnité(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				int n;
				if (int.TryParse (data.Text.ToSimpleText (), out n))
				{
					if (n >= 1 && n <= 1000000)
					{
						return;
					}
				}

				data.Error = "Vous devez donner une unité comprise entre 1 et 1'000'000";
			}
			else
			{
				data.Error = "Il manque l'unité";
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
					data.Error = "Ce n'est pas un compte normal";
					return;
				}
			}
			else
			{
				data.Error = "Il manque le numéro du compte";
			}
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var taux = entity as ComptaTauxChangeEntity;

			this.SetText (ColumnType.Code,        taux.CodeISO);
			this.SetText (ColumnType.Description, taux.Description);
			this.SetText (ColumnType.Cours,       Converters.DecimalToString (taux.Cours, 6));
			this.SetText (ColumnType.Unité,       Converters.IntToString (taux.Unité));
			this.SetText (ColumnType.CompteGain,  this.GetNuméro (taux.CompteGain));
			this.SetText (ColumnType.ComptePerte, this.GetNuméro (taux.ComptePerte));
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var taux = entity as ComptaTauxChangeEntity;

			taux.CodeISO     = this.GetText (ColumnType.Code);
			taux.Description = this.GetText (ColumnType.Description);
			taux.Cours       = Converters.ParseDecimal (this.GetText (ColumnType.Cours)).GetValueOrDefault (1);
			taux.Unité       = Converters.ParseInt (this.GetText (ColumnType.Unité)).GetValueOrDefault (1);
			taux.CompteGain  = this.GetCompte (this.GetText (ColumnType.CompteGain));
			taux.ComptePerte = this.GetCompte (this.GetText (ColumnType.ComptePerte));
		}

		private FormattedText GetNuméro(ComptaCompteEntity compte)
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

		private ComptaCompteEntity GetCompte(FormattedText numéro)
		{
			if (numéro.IsNullOrEmpty)
			{
				return null;
			}
			else
			{
				return this.compta.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();
			}
		}
	}
}