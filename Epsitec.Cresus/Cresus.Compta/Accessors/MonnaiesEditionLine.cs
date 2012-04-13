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
	/// Données éditables pour les monnaies de la comptabilité.
	/// </summary>
	public class MonnaiesEditionLine : AbstractEditionLine
	{
		public MonnaiesEditionLine(AbstractController controller)
			: base (controller)
		{
			this.dataDict.Add (ColumnType.Code,        new EditionData (this.controller, this.ValidateCode));
			this.dataDict.Add (ColumnType.Description, new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Décimales,   new EditionData (this.controller, this.ValidateDécimales));
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
				var monnaie = this.compta.Monnaies.Where (x => x.CodeISO == data.Text).FirstOrDefault ();
				if (monnaie == null)
				{
					return;
				}

				var himself = (this.controller.DataAccessor.JustCreated || this.controller.EditorController.Duplicate) ? null : this.controller.DataAccessor.GetEditionEntity (this.controller.DataAccessor.FirstEditedRow) as ComptaMonnaieEntity;
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

		private void ValidateDécimales(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				int n;
				if (int.TryParse (data.Text.ToSimpleText (), out n))
				{
					if (n >= 0 && n <= 9)
					{
						return;
					}
				}

				data.Error = "Vous devez donner un nombre de décimales compris entre 0 et 9";
			}
			else
			{
				data.Error = "Il manque le nombre de décimales";
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
			var monnaie = entity as ComptaMonnaieEntity;

			this.SetText (ColumnType.Code,        monnaie.CodeISO);
			this.SetText (ColumnType.Description, monnaie.Description);
			this.SetText (ColumnType.Décimales,   Converters.IntToString (monnaie.Décimales));
			this.SetText (ColumnType.Cours,       Converters.DecimalToString (monnaie.Cours, 6));
			this.SetText (ColumnType.Unité,       Converters.IntToString (monnaie.Unité));
			this.SetText (ColumnType.CompteGain,  this.GetNuméro (monnaie.CompteGain));
			this.SetText (ColumnType.ComptePerte, this.GetNuméro (monnaie.ComptePerte));
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var monnaie = entity as ComptaMonnaieEntity;

			monnaie.CodeISO     = this.GetText (ColumnType.Code);
			monnaie.Description = this.GetText (ColumnType.Description);
			monnaie.Décimales   = Converters.ParseInt (this.GetText (ColumnType.Décimales)).GetValueOrDefault (2);
			monnaie.Cours       = Converters.ParseDecimal (this.GetText (ColumnType.Cours)).GetValueOrDefault (1);
			monnaie.Unité       = Converters.ParseInt (this.GetText (ColumnType.Unité)).GetValueOrDefault (1);
			monnaie.CompteGain  = this.GetCompte (this.GetText (ColumnType.CompteGain));
			monnaie.ComptePerte = this.GetCompte (this.GetText (ColumnType.ComptePerte));
		}

		private FormattedText GetNuméro(ComptaCompteEntity compte)
		{
			if (compte == null)
			{
				return FormattedText.Empty;
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