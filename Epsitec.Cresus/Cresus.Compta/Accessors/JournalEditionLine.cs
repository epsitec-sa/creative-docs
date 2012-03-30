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
			this.dataDict.Add (ColumnType.Débit,       new EditionData (this.controller, this.ValidateCompteDébit));
			this.dataDict.Add (ColumnType.Crédit,      new EditionData (this.controller, this.ValidateCompteCrédit));
			this.dataDict.Add (ColumnType.Pièce,       new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Libellé,     new EditionData (this.controller, this.ValidateLibellé));
			this.dataDict.Add (ColumnType.MontantTTC,  new EditionData (this.controller, this.ValidateMontantTTC));
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

		private void ValidateCompteDébit(EditionData data)
		{
			data.ClearError ();
			data.ClearOverlayText ();

			if (!data.HasText)
			{
				data.OverlayText = "Débit";
			}


			if (this.IsEmptyLine)
			{
				return;  // une ligne vide est toujours ok
			}

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

		private void ValidateCompteCrédit(EditionData data)
		{
			data.ClearError ();
			data.ClearOverlayText ();

			if (!data.HasText)
			{
				data.OverlayText = "Crédit";
			}


			if (this.IsEmptyLine)
			{
				return;  // une ligne vide est toujours ok
			}

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
			data.ClearOverlayText ();

			if (!data.HasText)
			{
				data.OverlayText = "Libellé";
			}

			if (this.IsEmptyLine)
			{
				return;  // une ligne vide est toujours ok
			}

			var type = this.TypeEcriture;
			if (type == TypeEcriture.CodeTVA)
			{
				return;  // toujours ok
			}

			Validators.ValidateText (data, "Il manque le libellé");
		}

		private void ValidateMontantTTC(EditionData data)
		{
			data.ClearError ();
			data.ClearOverlayText ();

			if (this.GetEnable (ColumnType.MontantTTC))  // création avec soit TTC soit HT ?
			{
				data.OverlayText = "TTC";
			}

			if (this.IsEmptyLine)
			{
				return;  // une ligne vide est toujours ok
			}

			var type = this.TypeEcriture;
			var montantTTC = Converters.ParseMontant (this.GetText (ColumnType.MontantTTC));
			var montantHT  = Converters.ParseMontant (this.GetText (ColumnType.Montant));

			if (type == TypeEcriture.Nouveau ||
				type == TypeEcriture.Vide    )
			{
				if (this.GetEnable (ColumnType.MontantTTC))  // création avec soit TTC soit HT ?
				{
					if (montantTTC.GetValueOrDefault () == 0 && montantHT.GetValueOrDefault () == 0)
					{
						data.Error = "Donnez ici le montant TTC, ou à côté le montant HT";
						return;
					}

					if (montantTTC.GetValueOrDefault () != 0 && montantHT.GetValueOrDefault () != 0)
					{
						data.Error = "Il faut donner le montant TTC ou le montant HT, mais pas les deux";
						return;
					}

					if (montantTTC.HasValue)
					{
						if (!this.controller.SettingsList.GetBool (SettingsType.EcritureMontantZéro) &&  // refuse les montants nuls ?
						montantTTC.GetValueOrDefault () == 0)  // montant nul ?
						{
							data.Error = "Le montant ne peut pas être nul";
							data.Text = Converters.MontantToString (0);
							return;
						}

						Validators.ValidateMontant (data, emptyAccepted: false);
					}
				}

				return;
			}

			if (type == TypeEcriture.BaseTVA)
			{
				if (!this.controller.SettingsList.GetBool (SettingsType.EcritureMontantZéro) &&  // refuse les montants nuls ?
					montantTTC.GetValueOrDefault () == 0)  // montant nul ?
				{
					data.Error = "Le montant ne peut pas être nul";
					data.Text = Converters.MontantToString (0);
					return;
				}

				Validators.ValidateMontant (data, emptyAccepted: false);
				return;
			}
		}

		private void ValidateMontant(EditionData data)
		{
			data.ClearError ();
			data.ClearOverlayText ();

			var type = this.TypeEcriture;
			var hasTVA = this.controller.SettingsList.GetBool (SettingsType.EcritureTVA);

			if (this.GetText (ColumnType.TotalAutomatique) == "1")
			{
				data.OverlayText = "∑";
			}
			else if (hasTVA && type == Compta.TypeEcriture.CodeTVA)
			{
				data.OverlayText = "TVA";
			}
			else if (hasTVA && this.GetEnable (ColumnType.MontantTTC))  // création avec soit TTC soit HT ?
			{
				data.OverlayText = "HT";
			}

			if (this.IsEmptyLine)
			{
				return;  // une ligne vide est toujours ok
			}

			var montantTTC = Converters.ParseMontant (this.GetText (ColumnType.MontantTTC));
			var montantHT  = Converters.ParseMontant (this.GetText (ColumnType.Montant));

			if (type == TypeEcriture.Nouveau ||
				type == TypeEcriture.Vide    )
			{
				if (this.GetEnable (ColumnType.MontantTTC))  // création avec soit TTC soit HT ?
				{
					if (montantTTC.GetValueOrDefault () == 0 && montantHT.GetValueOrDefault () == 0)
					{
						data.Error = "Donnez ici le montant HT, ou à côté le montant TTC";
						return;
					}

					if (montantTTC.GetValueOrDefault () != 0 && montantHT.GetValueOrDefault () != 0)
					{
						data.Error = "Il faut donner le montant TTC ou le montant HT, mais pas les deux";
						return;
					}

					if (montantTTC.HasValue)
					{
						return;
					}
				}
			}

			if (type == Compta.TypeEcriture.BaseTVA && !this.controller.SettingsList.GetBool (SettingsType.EcritureEditeMontantHT))
			{
				return;
			}

			if (montantHT.HasValue &&
				type != TypeEcriture.CodeTVA &&  // sur la ligne CodeTVA, le montant peut être nul (par *ex. lors d'exportation)
				this.GetText (ColumnType.TotalAutomatique) != "1")  // le total automatique peut être nul
			{
				if (!this.controller.SettingsList.GetBool (SettingsType.EcritureMontantZéro) &&  // refuse les montants nuls ?
					montantHT.GetValueOrDefault () == 0)  // montant nul ?
				{
					data.Error = "Le montant ne peut pas être nul";
					data.Text = Converters.MontantToString (0);
					return;
				}
			}

			Validators.ValidateMontant (data, emptyAccepted: false);
		}

		private void ValidateCodeTVA(EditionData data)
		{
			data.ClearError ();

			if (this.IsEmptyLine)
			{
				return;  // une ligne vide est toujours ok
			}

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
			data.ClearError ();

			if (this.IsEmptyLine)
			{
				return;  // une ligne vide est toujours ok
			}

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

			if (écriture.IsEmptyLine)
			{
				this.SetText (ColumnType.Date,              Converters.DateToString (écriture.Date));
				this.SetText (ColumnType.Débit,             FormattedText.Empty);
				this.SetText (ColumnType.Crédit,            FormattedText.Empty);
				this.SetText (ColumnType.OrigineTVA,        FormattedText.Empty);
				this.SetText (ColumnType.Pièce,             FormattedText.Empty);
				this.SetText (ColumnType.Libellé,           FormattedText.Empty);
				this.SetText (ColumnType.Montant,           Converters.MontantToString (0));
				this.SetText (ColumnType.MontantTTC,        FormattedText.Empty);
				this.SetText (ColumnType.MontantComplément, FormattedText.Empty);
				this.SetText (ColumnType.TotalAutomatique,  "0");
				this.SetText (ColumnType.CodeTVA,           FormattedText.Empty);
				this.SetText (ColumnType.TauxTVA,           FormattedText.Empty);
				this.SetText (ColumnType.CompteTVA,         FormattedText.Empty);
				this.SetText (ColumnType.Journal,           (écriture.Journal == null) ? null : écriture.Journal.Nom);
				this.TypeEcriture =                         (TypeEcriture) écriture.Type;
			}
			else
			{
				this.SetText (ColumnType.Date,              Converters.DateToString (écriture.Date));
				this.SetText (ColumnType.Débit,             JournalDataAccessor.GetNuméro (écriture.Débit));
				this.SetText (ColumnType.Crédit,            JournalDataAccessor.GetNuméro (écriture.Crédit));
				this.SetText (ColumnType.OrigineTVA,        écriture.OrigineTVA);
				this.SetText (ColumnType.Pièce,             écriture.Pièce);
				this.SetText (ColumnType.Libellé,           écriture.Libellé);
				this.SetText (ColumnType.Montant,           Converters.MontantToString (écriture.Montant));
				this.SetText (ColumnType.MontantTTC,        Converters.MontantToString (écriture.Montant + écriture.MontantComplément));
				this.SetText (ColumnType.MontantComplément, Converters.MontantToString (écriture.MontantComplément));
				this.SetText (ColumnType.TotalAutomatique,  écriture.TotalAutomatique ? "1" : "0");
				this.SetText (ColumnType.CodeTVA,           JournalEditionLine.GetCodeTVADescription (écriture.CodeTVA));
				this.SetText (ColumnType.TauxTVA,           Converters.PercentToString (écriture.TauxTVA));
				this.SetText (ColumnType.CompteTVA,         JournalEditionLine.GetCodeTVACompte (écriture.CodeTVA));
				this.SetText (ColumnType.Journal,           (écriture.Journal == null) ? null : écriture.Journal.Nom);
				this.TypeEcriture =                         (TypeEcriture) écriture.Type;
			}

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

			if (this.IsEmptyLine)
			{
				écriture.Débit             = null;
				écriture.Crédit            = null;
				écriture.OrigineTVA        = null;
				écriture.Pièce             = FormattedText.Empty;
				écriture.Libellé           = FormattedText.Empty;
				écriture.Montant           = 0;
				écriture.MontantComplément = null;
				écriture.TotalAutomatique  = false;
				écriture.CodeTVA           = null;
				écriture.TauxTVA           = null;
				écriture.Type              = (int) this.TypeEcriture;
			}
			else
			{
				écriture.Débit             = JournalDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.Débit));
				écriture.Crédit            = JournalDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.Crédit));
				écriture.OrigineTVA        = this.GetText (ColumnType.OrigineTVA).ToString ();
				écriture.Pièce             = this.GetText (ColumnType.Pièce);
				écriture.Libellé           = this.GetText (ColumnType.Libellé);
				écriture.Montant           = Converters.ParseMontant (this.GetText (ColumnType.Montant)).GetValueOrDefault ();
				écriture.MontantComplément = Converters.ParseMontant (this.GetText (ColumnType.MontantComplément)).GetValueOrDefault ();
				écriture.TotalAutomatique  = (this.GetText (ColumnType.TotalAutomatique) == "1");
				écriture.CodeTVA           = this.TextToCodeTVA (this.GetText (ColumnType.CodeTVA));
				écriture.TauxTVA           = Converters.ParsePercent (this.GetText (ColumnType.TauxTVA));
				écriture.Type              = (int) this.TypeEcriture;
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
		}

		public void UpdateCodeTVAParameters()
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


		public bool IsEmptyLine
		{
			//	Retourne true si une ligne est entièrement vide.
			get
			{
				var débit  = this.GetText (ColumnType.Débit);
				var crédit = this.GetText (ColumnType.Crédit);

				return (débit .IsNullOrEmpty || débit  == JournalDataAccessor.multi) &&
					   (crédit.IsNullOrEmpty || crédit == JournalDataAccessor.multi) &&
					   this.GetText (ColumnType.Libellé).IsNullOrEmpty &&
					   Converters.ParseMontant (this.GetText (ColumnType.Montant)).GetValueOrDefault () == 0 &&
					   this.GetText (ColumnType.TotalAutomatique) != "1";
			}
		}

		private TypeEcriture TypeEcriture
		{
			get
			{
				return Converters.StringToTypeEcriture (this.GetText (ColumnType.Type));
			}
			set
			{
				this.SetText (ColumnType.Type, Converters.TypeEcritureToString (value));
			}
		}
	}
}
