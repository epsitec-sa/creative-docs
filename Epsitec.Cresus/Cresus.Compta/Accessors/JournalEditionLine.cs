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
	/// Données éditables pour le journal de la comptabilité.
	/// </summary>
	public class JournalEditionLine : AbstractEditionLine
	{
		public JournalEditionLine(AbstractController controller)
			: base (controller)
		{
			this.dataDict.Add (ColumnType.Date,        new EditionData (this.controller, this.ValidateDate));
			this.dataDict.Add (ColumnType.Débit,       new EditionData (this.controller, this.ValidateCompte));
			this.dataDict.Add (ColumnType.Crédit,      new EditionData (this.controller, this.ValidateCompte));
			this.dataDict.Add (ColumnType.Pièce,       new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Libellé,     new EditionData (this.controller, this.ValidateLibellé));
			this.dataDict.Add (ColumnType.MontantBrut, new EditionData (this.controller, this.ValidateMontantTVA));
			this.dataDict.Add (ColumnType.MontantTVA,  new EditionData (this.controller, this.ValidateMontantTVA));
			this.dataDict.Add (ColumnType.Montant,     new EditionData (this.controller, this.ValidateMontant));
			this.dataDict.Add (ColumnType.CodeTVA,     new EditionData (this.controller, this.ValidateCodeTVA));
			this.dataDict.Add (ColumnType.TauxTVA,     new EditionData (this.controller, this.ValidateTauxTVA));
			this.dataDict.Add (ColumnType.Journal,     new EditionData (this.controller, this.ValidateJournal));
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
				if (data.Text == JournalDataAccessor.multi)
				{
					return;
				}

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

		private void ValidateMontantTVA(EditionData data)
		{
			if (this.GetText (ColumnType.CodeTVA).IsNullOrEmpty)  // pas de code TVA ?
			{
				if (data.HasText)
				{
					data.Error = "Vous devez donner un code TVA pour pouvoir mettre un montant dans cette colonne";
					return;
				}
			}
			else
			{
				if (!data.HasText)
				{
					data.Error = "Vous devez donner un montant, ou supprimer le code TVA";
					return;
				}
			}

			Validators.ValidateMontant (data, emptyAccepted: true);
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
		}

		private void ValidateTauxTVA(EditionData data)
		{
			if (this.GetText (ColumnType.CodeTVA).IsNullOrEmpty)  // pas de code TVA ?
			{
				if (data.HasText)
				{
					data.Error = "Vous devez donner un code TVA pour pouvoir mettre un taux dans cette colonne";
					return;
				}
			}
			else
			{
				if (!data.HasText)
				{
					data.Error = "Vous devez donner un taux, ou supprimer le code TVA";
					return;
				}
			}

			Validators.ValidatePercent (data, emptyAccepted: true);
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


		public void CompteDébitChanged()
		{
			this.CompteChanged ();
		}

		public void CompteCréditChanged()
		{
			this.CompteChanged ();
		}

		private void CompteChanged()
		{
			var numéroDébit  = this.GetText (ColumnType.Débit);
			var numéroCrédit = this.GetText (ColumnType.Crédit);

			//	Cherche les codes TVA des comptes donnés au débit et au crédit.
			ComptaCodeTVAEntity codeTVADébit  = null;
			ComptaCodeTVAEntity codeTVACrédit = null;

			if (!numéroDébit.IsNullOrEmpty)
			{
				var compte = this.compta.PlanComptable.Where (x => x.Numéro == numéroDébit).FirstOrDefault ();
				if (compte != null)
				{
					codeTVADébit = compte.CodeTVA;
				}
			}

			if (!numéroCrédit.IsNullOrEmpty)
			{
				var compte = this.compta.PlanComptable.Where (x => x.Numéro == numéroCrédit).FirstOrDefault ();
				if (compte != null)
				{
					codeTVACrédit = compte.CodeTVA;
				}
			}

			//	Cherche la description du code TVA à utiliser.
			FormattedText currentCodeTVA = this.GetText (ColumnType.CodeTVA);
			FormattedText newCodeTVA = FormattedText.Empty;
			ComptaCodeTVAEntity codeTVA = null;

			//	Attention, il ne peut y avoir qu'un seul compte (débit ou crédit) avec un code TVA.
			if (codeTVADébit != null && codeTVACrédit == null)
			{
				newCodeTVA = JournalEditionLine.GetCodeTVADescription (codeTVADébit);
				codeTVA = codeTVADébit;
			}

			if (codeTVADébit == null && codeTVACrédit != null)
			{
				newCodeTVA = JournalEditionLine.GetCodeTVADescription (codeTVACrédit);
				codeTVA = codeTVACrédit;
			}

			//	Effectue le changement seulement s'il y a lieu.
			if (newCodeTVA != currentCodeTVA)
			{
				this.SetText (ColumnType.CodeTVA, newCodeTVA);
				this.SetText (ColumnType.TauxTVA, (codeTVA == null) ? null : Converters.PercentToString (codeTVA.Taux1));
				this.CodeTVAChanged ();  // met à jour les autres colonnes
			}
		}

		public void MontantBrutChanged()
		{
			var montantBrut = Converters.ParseMontant (this.GetText (ColumnType.MontantBrut));
			var montantTVA  = Converters.ParseMontant (this.GetText (ColumnType.MontantTVA));
			var codeTVA     = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));
			var tauxTVA     = Converters.ParsePercent (this.GetText (ColumnType.TauxTVA));

			if (montantBrut.HasValue && codeTVA != null && tauxTVA.HasValue)
			{
				var tva   = montantBrut.Value * tauxTVA.Value;
				var total = montantBrut.Value + tva;

				this.SetText (ColumnType.MontantTVA, Converters.MontantToString (tva));
				this.SetText (ColumnType.Montant,    Converters.MontantToString (total));
			}
		}

		public void MontantTVAChanged()
		{
			var montant    = Converters.ParseMontant (this.GetText (ColumnType.Montant));
			var montantTVA = Converters.ParseMontant (this.GetText (ColumnType.MontantTVA));

			if (montant.HasValue && montantTVA.HasValue)
			{
				var brut = montant.Value - montantTVA.Value;

				this.SetText (ColumnType.MontantBrut, Converters.MontantToString (brut));
			}
		}

		public void MontantChanged()
		{
			var montantTVA = Converters.ParseMontant (this.GetText (ColumnType.MontantTVA));
			var montant    = Converters.ParseMontant (this.GetText (ColumnType.Montant));
			var codeTVA    = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));
			var tauxTVA     = Converters.ParsePercent (this.GetText (ColumnType.TauxTVA));

			if (montant.HasValue && codeTVA != null && tauxTVA.HasValue)
			{
				var montantBrut = montant.Value / (1+tauxTVA.Value);
				var tva         = montant.Value - montantBrut;

				this.SetText (ColumnType.MontantBrut, Converters.MontantToString (montantBrut));
				this.SetText (ColumnType.MontantTVA,  Converters.MontantToString (tva));
			}
		}

		public void CodeTVAChanged()
		{
			var montantBrut = Converters.ParseMontant (this.GetText (ColumnType.MontantBrut));
			var montantTVA  = Converters.ParseMontant (this.GetText (ColumnType.MontantTVA));
			var montant     = Converters.ParseMontant (this.GetText (ColumnType.Montant));
			var codeTVA     = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));

			if (codeTVA == null)
			{
				this.SetText (ColumnType.MontantBrut, null);
				this.SetText (ColumnType.MontantTVA,  null);
				this.SetText (ColumnType.Montant,     null);
				this.SetText (ColumnType.TauxTVA,     null);
				this.SetText (ColumnType.CompteTVA,   null);
			}
			else
			{
				if (codeTVA.Taux2.HasValue)
				{
					this.SetText (ColumnType.TauxTVA, Converters.PercentToString (codeTVA.Taux2));
				}
				else
				{
					this.SetText (ColumnType.TauxTVA, Converters.PercentToString (codeTVA.Taux1));
				}

				if (montantBrut.HasValue && montant.GetValueOrDefault () == 0)
				{
					this.MontantBrutChanged ();
				}
				else
				{
					this.MontantChanged ();
				}

				this.SetText (ColumnType.CompteTVA, JournalEditionLine.GetCodeTVACompte (codeTVA));
			}
		}

		public void TauxTVAChanged()
		{
			var montantBrut = Converters.ParseMontant (this.GetText (ColumnType.MontantBrut));
			var montantTVA  = Converters.ParseMontant (this.GetText (ColumnType.MontantTVA));
			var montant     = Converters.ParseMontant (this.GetText (ColumnType.Montant));
			var codeTVA     = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));

			if (codeTVA == null)
			{
				this.SetText (ColumnType.MontantBrut, null);
				this.SetText (ColumnType.MontantTVA,  null);
				this.SetText (ColumnType.Montant,     null);
			}
			else
			{
				if (montantBrut.HasValue && montant.GetValueOrDefault () == 0)
				{
					this.MontantBrutChanged ();
				}
				else
				{
					this.MontantChanged ();
				}
			}
		}


		public override void EntityToData(AbstractEntity entity)
		{
			var écriture = entity as ComptaEcritureEntity;

			this.SetText (ColumnType.Date,             Converters.DateToString (écriture.Date));
			this.SetText (ColumnType.Débit,            JournalDataAccessor.GetNuméro (écriture.Débit));
			this.SetText (ColumnType.Crédit,           JournalDataAccessor.GetNuméro (écriture.Crédit));
			this.SetText (ColumnType.Pièce,            écriture.Pièce);
			this.SetText (ColumnType.Libellé,          écriture.Libellé);
			this.SetText (ColumnType.MontantBrut,      (écriture.CodeTVA == null) ? null : Converters.MontantToString (écriture.MontantBrut));
			this.SetText (ColumnType.MontantTVA,       (écriture.CodeTVA == null) ? null : Converters.MontantToString (écriture.MontantTVA));
			this.SetText (ColumnType.Montant,          Converters.MontantToString (écriture.Montant));
			this.SetText (ColumnType.TotalAutomatique, écriture.TotalAutomatique ? "True" : "False");
			this.SetText (ColumnType.CodeTVA,          JournalEditionLine.GetCodeTVADescription (écriture.CodeTVA));
			this.SetText (ColumnType.TauxTVA,          Converters.PercentToString (écriture.TauxTVA));
			this.SetText (ColumnType.CompteTVA,        JournalEditionLine.GetCodeTVACompte (écriture.CodeTVA));
			this.SetText (ColumnType.Journal,          écriture.Journal.Nom);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var écriture = entity as ComptaEcritureEntity;

			Date? date;
			if (this.période.ParseDate (this.GetText (ColumnType.Date), out date))
			{
				écriture.Date = date.Value;
			}

			écriture.Débit  = JournalDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.Débit));
			écriture.Crédit = JournalDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.Crédit));

			écriture.Pièce            = this.GetText (ColumnType.Pièce);
			écriture.Libellé          = this.GetText (ColumnType.Libellé);
			écriture.MontantBrut      = Converters.ParseMontant (this.GetText (ColumnType.MontantBrut)).GetValueOrDefault ();
			écriture.MontantTVA       = Converters.ParseMontant (this.GetText (ColumnType.MontantTVA)).GetValueOrDefault ();
			écriture.Montant          = Converters.ParseMontant (this.GetText (ColumnType.Montant)).GetValueOrDefault ();
			écriture.TotalAutomatique = (this.GetText (ColumnType.TotalAutomatique) == "True");
			écriture.CodeTVA          = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));
			écriture.TauxTVA          = Converters.ParsePercent (this.GetText (ColumnType.TauxTVA));

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
		}


		public static FormattedText GetCodeTVADescription(ComptaCodeTVAEntity codeTVA)
		{
			if (codeTVA == null)
			{
				return FormattedText.Null;
			}
			else
			{
				return codeTVA.Code;
			}
		}

		private ComptaCodeTVAEntity TextToCodeTVA(FormattedText text)
		{
			return JournalEditionLine.TextToCodeTVA (this.compta, text);
		}

		public static ComptaCodeTVAEntity TextToCodeTVA(ComptaEntity compta, FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return null;
			}

			string s = text.ToSimpleText ();  // ignore le gras autour du code
			
			int i = s.IndexOf (' ');
			if (i != -1)
			{
				s = s.Substring (0, i);  // "IPM 7.6%" -> "IPM"
			}

			return compta.CodesTVA.Where (x => x.Code == s).FirstOrDefault ();
		}

		public static FormattedText GetCodeTVACompte(ComptaCodeTVAEntity codeTVA)
		{
			if (codeTVA == null)
			{
				return FormattedText.Null;
			}
			else
			{
				return codeTVA.Compte.Numéro;
			}
		}
	}
}