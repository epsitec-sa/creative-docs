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
			this.dataDict.Add (ColumnType.MontantTTC, new EditionData (this.controller, this.ValidateMontantTTC));
			this.dataDict.Add (ColumnType.Montant,    new EditionData (this.controller, this.ValidateMontant));
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
			data.ClearError ();

			var type = Converters.StringToTypeEcriture (this.GetText (ColumnType.Type));
			if (type == TypeEcriture.CodeTVA)
			{
				return;  // toujours ok
			}

			Validators.ValidateText (data, "Il manque le libellé");
		}

		private void ValidateMontantTTC(EditionData data)
		{
			Validators.ValidateMontant (data, emptyAccepted: true);
		}

		private void ValidateMontant(EditionData data)
		{
			data.ClearError ();

			var montantTTC = Converters.ParseMontant (this.GetText (ColumnType.MontantTTC));
			var montantHT  = Converters.ParseMontant (this.GetText (ColumnType.Montant));

			if (montantTTC.GetValueOrDefault () != 0 || montantHT.GetValueOrDefault () != 0)
			{
				return;
			}

			if (!this.controller.SettingsList.GetBool (SettingsType.EcritureMontantZéro) &&  // refuse les montants nuls ?
				montantHT.GetValueOrDefault () == 0)  // montant nul ?
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
#if false
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
#endif
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


		public override void EntityToData(AbstractEntity entity)
		{
			var écriture = entity as ComptaEcritureEntity;

			this.SetText (ColumnType.Date,              Converters.DateToString (écriture.Date));
			this.SetText (ColumnType.Débit,             JournalDataAccessor.GetNuméro (écriture.Débit));
			this.SetText (ColumnType.Crédit,            JournalDataAccessor.GetNuméro (écriture.Crédit));
			this.SetText (ColumnType.Pièce,             écriture.Pièce);
			this.SetText (ColumnType.Libellé,           écriture.Libellé);
			this.SetText (ColumnType.Montant,           Converters.MontantToString (écriture.Montant));
			this.SetText (ColumnType.MontantTTC,        Converters.MontantToString (écriture.Montant + écriture.MontantComplément));
			this.SetText (ColumnType.MontantComplément, Converters.MontantToString (écriture.MontantComplément));
			this.SetText (ColumnType.TotalAutomatique,  écriture.TotalAutomatique ? "1" : "0");
			this.SetText (ColumnType.CodeTVA,           JournalEditionLine.GetCodeTVADescription (écriture.CodeTVA));
			this.SetText (ColumnType.TauxTVA,           Converters.PercentToString (écriture.TauxTVA));
			this.SetText (ColumnType.CompteTVA,         JournalEditionLine.GetCodeTVACompte (écriture.CodeTVA));
			this.SetText (ColumnType.Journal,           écriture.Journal.Nom);
			this.SetText (ColumnType.Type,              Converters.TypeEcritureToString (écriture.Type));

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

			écriture.Pièce             = this.GetText (ColumnType.Pièce);
			écriture.Libellé           = this.GetText (ColumnType.Libellé);
			écriture.Montant           = Converters.ParseMontant (this.GetText (ColumnType.Montant)).GetValueOrDefault ();
			écriture.MontantComplément = Converters.ParseMontant (this.GetText (ColumnType.MontantComplément)).GetValueOrDefault ();
			écriture.TotalAutomatique  = (this.GetText (ColumnType.TotalAutomatique) == "1");
			écriture.CodeTVA           = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));
			écriture.TauxTVA           = Converters.ParsePercent (this.GetText (ColumnType.TauxTVA));
			écriture.Type              = (int) Converters.StringToTypeEcriture (this.GetText (ColumnType.Type));

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

		public bool LockedTVA
		{
			get
			{
				var compteDébit  = JournalDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.Débit));
				var compteCrédit = JournalDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.Crédit));

				int count = 0;

				if (compteDébit != null && compteDébit.CodeTVA != null)
				{
					count++;
				}

				if (compteCrédit != null && compteCrédit.CodeTVA != null)
				{
					count++;
				}

				return count == 1;
			}
		}
	}
}