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
			this.dataDict.Add (ColumnType.Date,       new EditionData (this.controller, this.ValidateDate));
			this.dataDict.Add (ColumnType.Débit,      new EditionData (this.controller, this.ValidateCompte));
			this.dataDict.Add (ColumnType.Crédit,     new EditionData (this.controller, this.ValidateCompte));
			this.dataDict.Add (ColumnType.Pièce,      new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Libellé,    new EditionData (this.controller, this.ValidateLibellé));
			this.dataDict.Add (ColumnType.MontantHT,  new EditionData (this.controller, this.ValidateMontantTVA));
			this.dataDict.Add (ColumnType.MontantTVA, new EditionData (this.controller, this.ValidateMontantTVA));
			this.dataDict.Add (ColumnType.MontantTTC, new EditionData (this.controller, this.ValidateMontant));
			this.dataDict.Add (ColumnType.CodeTVA,    new EditionData (this.controller, this.ValidateCodeTVA));
			this.dataDict.Add (ColumnType.TauxTVA,    new EditionData (this.controller, this.ValidateTauxTVA));
			this.dataDict.Add (ColumnType.Journal,    new EditionData (this.controller, this.ValidateJournal));
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
			data.ClearError ();

			if (this.GetText (ColumnType.TotalAutomatique) == "1")
			{
				return;
			}

			if (this.HasTVA)  // y a-t-il un code TVA reconnu ?
			{
				if (!data.HasText)
				{
					data.Error = "Vous devez donner un montant, ou supprimer le code TVA";
					return;
				}
			}
			else  // pas de code TVA ?
			{
				if (data.HasText)
				{
					data.Error = "Vous devez donner un code TVA pour pouvoir mettre un montant dans cette colonne";
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


		public void CompteChanged()
		{
			var numéroDébit  = this.GetText (ColumnType.Débit);
			var numéroCrédit = this.GetText (ColumnType.Crédit);

			//	Cherche les codes TVA des comptes donnés au débit et au crédit.
			ComptaCodeTVAEntity codeTVADébitEntity  = null;
			ComptaCodeTVAEntity codeTVACréditEntity = null;
			FormattedText codeTVADébit  = FormattedText.Null;  // Null = pas de changement
			FormattedText codeTVACrédit = FormattedText.Null;

			if (!numéroDébit.IsNullOrEmpty && numéroDébit != JournalDataAccessor.multi)
			{
				var compte = this.compta.PlanComptable.Where (x => x.Numéro == numéroDébit).FirstOrDefault ();

				if (compte != null && compte.Type == TypeDeCompte.TVA)
				{
					this.ClearTVA ();
					return;
				}

				if (compte == null || compte.Type != TypeDeCompte.Normal)
				{
					return;
				}

				codeTVADébitEntity = compte.CodeTVA;
				codeTVADébit       = (codeTVADébitEntity == null) ? FormattedText.Empty : compte.CodeTVA.Code;  // Empty = pas de TVA
			}

			if (!numéroCrédit.IsNullOrEmpty && numéroCrédit != JournalDataAccessor.multi)
			{
				var compte = this.compta.PlanComptable.Where (x => x.Numéro == numéroCrédit).FirstOrDefault ();

				if (compte != null && compte.Type == TypeDeCompte.TVA)
				{
					this.ClearTVA ();
					return;
				}

				if (compte == null || compte.Type != TypeDeCompte.Normal)
				{
					return;
				}

				codeTVACréditEntity = compte.CodeTVA;
				codeTVACrédit       = (codeTVACréditEntity == null) ? FormattedText.Empty : compte.CodeTVA.Code;  // Empty = pas de TVA
			}

			//	Attention, il ne peut y avoir qu'un seul compte (débit ou crédit) avec un code TVA.
			if (codeTVADébitEntity != null && codeTVACréditEntity != null)
			{
				codeTVADébitEntity  = null;
				codeTVACréditEntity = null;
				codeTVADébit  = FormattedText.Null;
				codeTVACrédit = FormattedText.Null;
			}

			//	Choisi le code TVA unique à utiliser (débit ou crédit).
			ComptaCodeTVAEntity codeTVAEntity = null;
			FormattedText codeTVA  = FormattedText.Null;
			bool TVAAuDébit = false;

			if (codeTVADébit == FormattedText.Empty)
			{
				codeTVA = FormattedText.Empty;
			}

			if (codeTVACrédit == FormattedText.Empty)
			{
				codeTVA = FormattedText.Empty;
			}

			if (!codeTVADébit.IsNullOrEmpty)
			{
				codeTVAEntity = codeTVADébitEntity;
				codeTVA       = codeTVADébit;
				TVAAuDébit    = true;
			}

			if (!codeTVACrédit.IsNullOrEmpty)
			{
				codeTVAEntity = codeTVACréditEntity;
				codeTVA       = codeTVACrédit;
				TVAAuDébit    = false;
			}

			if (codeTVA != FormattedText.Null)  // changement ?
			{
				FormattedText currentCodeTVA = this.GetText (ColumnType.CodeTVA);

				if (currentCodeTVA.IsNullOrEmpty)
				{
					currentCodeTVA = FormattedText.Empty;
				}

				if (codeTVA != currentCodeTVA)
				{
					this.SetText (ColumnType.CodeTVA, codeTVA);
					this.SetText (ColumnType.TauxTVA, (codeTVAEntity == null) ? null : Converters.PercentToString (codeTVAEntity.DefaultTauxValue));
					this.SetText (ColumnType.TVAAuDébit, TVAAuDébit ? "1" : "0");
					this.CodeTVAChanged ();  // met à jour les autres colonnes
				}
			}
		}

		private void ClearTVA()
		{
			this.SetText (ColumnType.CodeTVA, null);
			this.SetText (ColumnType.TauxTVA, null);
			this.CodeTVAChanged ();  // met à jour les autres colonnes
		}

		public void MontantBrutChanged()
		{
			var montantHT  = Converters.ParseMontant (this.GetText (ColumnType.MontantHT));
			var montantTVA = Converters.ParseMontant (this.GetText (ColumnType.MontantTVA));
			var codeTVA    = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));
			var tauxTVA    = Converters.ParsePercent (this.GetText (ColumnType.TauxTVA));

			if (montantHT.HasValue && codeTVA != null && tauxTVA.HasValue)
			{
				var montant = Converters.RoundMontant (montantHT.Value + montantHT.Value * tauxTVA.Value);
				var tva     = montant - montantHT.Value;

				this.SetText (ColumnType.MontantTVA, Converters.MontantToString (tva));
				this.SetText (ColumnType.MontantTTC, Converters.MontantToString (montant));
			}
		}

		public void MontantTVAChanged()
		{
			var montant    = Converters.ParseMontant (this.GetText (ColumnType.MontantTTC));
			var montantTVA = Converters.ParseMontant (this.GetText (ColumnType.MontantTVA));

			if (montant.HasValue && montantTVA.HasValue)
			{
				var montantHT = montant.Value - montantTVA.Value;

				this.SetText (ColumnType.MontantHT, Converters.MontantToString (montantHT));
			}
		}

		public void MontantChanged()
		{
			var montantTVA = Converters.ParseMontant (this.GetText (ColumnType.MontantTVA));
			var montant    = Converters.ParseMontant (this.GetText (ColumnType.MontantTTC));
			var codeTVA    = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));
			var tauxTVA    = Converters.ParsePercent (this.GetText (ColumnType.TauxTVA));

			if (montant.HasValue && codeTVA != null && tauxTVA.HasValue)
			{
				var montantHT = Converters.RoundMontant (montant.Value / (1+tauxTVA.Value));
				var tva         = montant.Value - montantHT;

				this.SetText (ColumnType.MontantHT, Converters.MontantToString (montantHT));
				this.SetText (ColumnType.MontantTVA,  Converters.MontantToString (tva));
			}
		}

		public void CodeTVAChanged()
		{
			var montantHT  = Converters.ParseMontant (this.GetText (ColumnType.MontantHT));
			var montantTVA = Converters.ParseMontant (this.GetText (ColumnType.MontantTVA));
			var montant    = Converters.ParseMontant (this.GetText (ColumnType.MontantTTC));
			var codeTVA    = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));

			if (codeTVA == null)
			{
				this.SetText (ColumnType.MontantHT,  null);
				this.SetText (ColumnType.MontantTVA, null);
				this.SetText (ColumnType.TauxTVA,    null);
				this.SetText (ColumnType.CompteTVA,  null);
			}
			else
			{
				this.SetText (ColumnType.TauxTVA, Converters.PercentToString (codeTVA.DefaultTauxValue));

				if (montantHT.HasValue && montant.GetValueOrDefault () == 0)
				{
					this.MontantBrutChanged ();
				}
				else
				{
					this.MontantChanged ();
				}

				this.SetText (ColumnType.CompteTVA, JournalEditionLine.GetCodeTVACompte (codeTVA));
			}

			this.UpdateCodeTVAParameters ();
		}

		public void TauxTVAChanged()
		{
			var montantHT  = Converters.ParseMontant (this.GetText (ColumnType.MontantHT));
			var montantTVA = Converters.ParseMontant (this.GetText (ColumnType.MontantTVA));
			var montant    = Converters.ParseMontant (this.GetText (ColumnType.MontantTTC));
			var codeTVA    = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));

			if (codeTVA == null)
			{
				this.SetText (ColumnType.MontantHT,  null);
				this.SetText (ColumnType.MontantTVA, null);
				this.SetText (ColumnType.MontantTTC, null);
			}
			else
			{
				if (montantHT.HasValue && montant.GetValueOrDefault () == 0)
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
			this.SetText (ColumnType.MontantHT,        (écriture.CodeTVA == null) ? null : Converters.MontantToString (écriture.MontantHT));
			this.SetText (ColumnType.MontantTVA,       (écriture.CodeTVA == null) ? null : Converters.MontantToString (écriture.MontantTVA));
			this.SetText (ColumnType.MontantTTC,       Converters.MontantToString (écriture.MontantTTC));
			this.SetText (ColumnType.TotalAutomatique, écriture.TotalAutomatique ? "1" : "0");
			this.SetText (ColumnType.CodeTVA,          JournalEditionLine.GetCodeTVADescription (écriture.CodeTVA));
			this.SetText (ColumnType.TauxTVA,          Converters.PercentToString (écriture.TauxTVA));
			this.SetText (ColumnType.CompteTVA,        JournalEditionLine.GetCodeTVACompte (écriture.CodeTVA));
			this.SetText (ColumnType.TVAAuDébit,       écriture.TVAAuDébit ? "1" : "0");
			this.SetText (ColumnType.Journal,          écriture.Journal.Nom);

			this.UpdateCodeTVAParameters ();
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
			écriture.MontantHT        = Converters.ParseMontant (this.GetText (ColumnType.MontantHT)).GetValueOrDefault ();
			écriture.MontantTVA       = Converters.ParseMontant (this.GetText (ColumnType.MontantTVA)).GetValueOrDefault ();
			écriture.MontantTTC       = Converters.ParseMontant (this.GetText (ColumnType.MontantTTC)).GetValueOrDefault ();
			écriture.TotalAutomatique = (this.GetText (ColumnType.TotalAutomatique) == "1");
			écriture.CodeTVA          = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));
			écriture.TauxTVA          = Converters.ParsePercent (this.GetText (ColumnType.TauxTVA));
			écriture.TVAAuDébit       = this.GetText (ColumnType.TVAAuDébit) == "1";

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

		private void UpdateCodeTVAParameters()
		{
			var parameters = this.GetParameters (ColumnType.TauxTVA);
			parameters.Clear ();

			var codeTVA = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));
			if (codeTVA != null)
			{
				foreach (var taux in codeTVA.ListeTaux.Taux)
				{
					parameters.Add (Converters.PercentToString (taux.Taux));
				}
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

		public bool HasTVA
		{
			get
			{
				var code = this.GetText (ColumnType.CodeTVA);
				return this.compta.CodesTVA.Where (x => x.Code == code).Any ();
			}
		}
	}
}