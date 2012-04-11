//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données du résumé TVA de la comptabilité.
	/// </summary>
	public class RésuméTVADataAccessor : AbstractDataAccessor
	{
		public RésuméTVADataAccessor(AbstractController controller)
			: base (controller)
		{
			this.options    = this.mainWindowController.GetSettingsOptions<RésuméTVAOptions> ("Présentation.RésuméTVA.Options", this.compta);
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.RésuméTVA.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData ("Présentation.RésuméTVA.Filter");

			this.UpdateAfterOptionsChanged ();
		}


		public override void UpdateFilter()
		{
			this.UpdateReadonlyAllData ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			this.UpdateReadonlyAllData ();
		}

		private void UpdateReadonlyAllData()
		{
			this.readonlyAllData.Clear ();

			this.filterData.GetBeginnerDates (out this.lastBeginDate, out this.lastEndDate);
			this.soldesJournalManager.Initialize (this.période.Journal, this.lastBeginDate, this.lastEndDate);

			var blocs = new List<BlocDeRésumé> ();
			ComptaEcritureEntity écritureDeBase = null;

			//	Passe en revue toutes les écritures.
			foreach (var écriture in this.période.Journal)
			{
				if (!Dates.DateInRange (écriture.Date, this.lastBeginDate, this.lastEndDate))
				{
					continue;
				}

				if (écriture.Type == (int) TypeEcriture.BaseTVA)
				{
					écritureDeBase = écriture;
				}
				else if (écriture.Type == (int) TypeEcriture.CodeTVA)
				{
					BlocDeRésumé bloc;
					var compte = écritureDeBase.CompteTVA;
					var codeTVA = écriture.CodeTVA;

					if (this.Options.ParCodeTVA)
					{
						bloc = blocs.Where (x => x.CodeTVA == codeTVA).FirstOrDefault ();
					}
					else
					{
						bloc = blocs.Where (x => x.Compte == compte).FirstOrDefault ();
					}

					if (bloc == null)
					{
						bloc = new BlocDeRésumé (compte, codeTVA, this.Options);
						blocs.Add (bloc);
					}

					bloc.AddEcriture (écritureDeBase, écriture);
				}
			}

			//	Passe en revue tous les blocs.
			IEnumerable<BlocDeRésumé> orderedBlocs;

			if (this.Options.ParCodeTVA)
			{
				orderedBlocs = blocs.OrderBy (x => x.CodeTVA.Code);
			}
			else
			{
				orderedBlocs = blocs.OrderBy (x => x.Compte.Numéro);
			}

			decimal montantDu    = 0;
			decimal TVADue       = 0;
			decimal DiffDue      = 0;
			decimal montantRecup = 0;
			decimal TVARecup     = 0;
			decimal DiffRecup    = 0;

			foreach (var bloc in orderedBlocs)
			{
				//	Ajoute l'en-tête du bloc.
				{
					if (this.Options.MontreEcritures && !this.Options.ParCodeTVA)
					{
						var data = new RésuméTVAData
						{
							LigneEnTête = true,
							Numéro      = bloc.Compte.Numéro,
							Titre       = bloc.Compte.GetCompactSummary (),
						};

						this.readonlyAllData.Add (data);
					}

					if (this.Options.MontreEcritures && this.Options.ParCodeTVA)
					{
						var data = new RésuméTVAData
						{
							LigneEnTête = true,
							Titre       = bloc.CodeTVA.Code,
						};

						this.readonlyAllData.Add (data);
					}
				}

				decimal soustotalMontant = 0;
				decimal soustotalTVA     = 0;
				decimal soustotalDiff    = 0;

				//	Ajoute les lignes du bloc.
				IEnumerable<RésuméTVAData> orderedLignes;

				if (this.Options.ParCodeTVA && !this.Options.MontreEcritures)
				{
					orderedLignes = bloc.Lignes.OrderBy (x => x.Numéro);
				}
				else
				{
					orderedLignes = bloc.Lignes;
				}

				foreach (var ligne in orderedLignes)
				{
					soustotalMontant += ligne.Montant.GetValueOrDefault ();
					soustotalTVA     += ligne.TVA.GetValueOrDefault ();
					soustotalDiff    += ligne.Différence.GetValueOrDefault ();

					var data = new RésuméTVAData
					{
						Entity     = ligne.Entity,
						Numéro     = ligne.Numéro,
						CodeTVA    = ligne.CodeTVA,
						Taux       = ligne.Taux,
						Date       = ligne.Date,
						Pièce      = ligne.Pièce,
						Titre      = ligne.Titre,
						Montant    = ligne.Montant,
						TVA        = ligne.TVA,
						Différence = ligne.Différence,
					};

					this.readonlyAllData.Add (data);
				}

				//	Ajoute le total du bloc.
				{
					if (bloc.CodeTVA.Compte.Catégorie == CatégorieDeCompte.Passif)
					{
						montantDu += soustotalMontant;
						TVADue    += soustotalTVA;
						DiffDue   += soustotalDiff;
					}

					if (bloc.CodeTVA.Compte.Catégorie == CatégorieDeCompte.Actif)
					{
						montantRecup += soustotalMontant;
						TVARecup     += soustotalTVA;
						DiffRecup    += soustotalDiff;
					}

					if (this.Options.MontreEcritures)
					{
						var data = new RésuméTVAData
						{
							LigneDeTotal       = true,
							Titre              = "Total",
							Montant            = soustotalMontant,
							TVA                = soustotalTVA,
							HasBottomSeparator = true,
						};

						if (!this.Options.ParCodeTVA)
						{
							data.Numéro = bloc.Compte.Numéro;
						}

						this.readonlyAllData.Add (data);
					}
					else
					{
						if (this.Options.ParCodeTVA)
						{
							var codeTVA = this.compta.CodesTVA.Where (x => x == bloc.CodeTVA).FirstOrDefault ();

							var data = new RésuméTVAData
							{
								LigneDeTotal       = true,
								Numéro             = bloc.Compte.Numéro,
								CodeTVA            = bloc.CodeTVA.Code,
								Taux               = codeTVA.DefaultTauxValue.GetValueOrDefault (),
								Titre              = "Total",
								Montant            = soustotalMontant,
								TVA                = soustotalTVA,
								HasBottomSeparator = true,
							};

							this.readonlyAllData.Add (data);
						}
						else
						{
							var data = new RésuméTVAData
							{
								LigneDeTotal       = true,
								Numéro             = bloc.Compte.Numéro,
								Titre              = bloc.Compte.Titre,
								Montant            = soustotalMontant,
								TVA                = soustotalTVA,
								HasBottomSeparator = true,
							};

							this.readonlyAllData.Add (data);
						}
					}
				}
			}

			//	Ajoute les lignes de total.
			{
				var data = new RésuméTVAData
				{
					Titre      = "Total TVA due",
					Montant    = montantDu,
					TVA        = TVADue,
					Différence = DiffDue,
				};

				this.readonlyAllData.Add (data);
			}

			{
				var data = new RésuméTVAData
				{
					Titre      = "Total TVA à récupérer",
					Montant    = montantRecup,
					TVA        = TVARecup,
					Différence = DiffRecup,
				};

				this.readonlyAllData.Add (data);
			}

			{
				var data = new RésuméTVAData
				{
					Titre              = "Total",
					Montant            = montantDu - montantRecup,
					TVA                = TVADue - TVARecup,
					Différence         = DiffDue - DiffRecup,
					IsBold             = true,
					HasBottomSeparator = true,
				};

				this.readonlyAllData.Add (data);
			}

			base.UpdateFilter ();
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var data = this.GetReadOnlyData (row, all) as RésuméTVAData;

			if (data == null)
			{
				return FormattedText.Null;
			}

			switch (column)
			{
				case ColumnType.Compte:
				case ColumnType.Compte2:
					return data.Numéro;

				case ColumnType.CodeTVA:
					return data.CodeTVA;

				case ColumnType.TauxTVA:
					return Converters.PercentToString (data.Taux);

				case ColumnType.Date:
					return Converters.DateToString (data.Date);

				case ColumnType.Pièce:
					return data.Pièce;

				case ColumnType.Titre:
					if (data.LigneEnTête)
					{
						return data.Titre.ApplyBold ();
					}
					else
					{
						return data.Titre;
					}

				case ColumnType.Montant:
					if (this.Options.MontantTTC)
					{
						return this.GetMontant (data, data.Montant + data.TVA);
					}
					else
					{
						return this.GetMontant (data, data.Montant);
					}

				case ColumnType.MontantTVA:
					return this.GetMontant (data, data.TVA);

				case ColumnType.Différence:
					return this.GetMontant (data, data.Différence);

				default:
					return FormattedText.Null;
			}
		}

		private FormattedText GetMontant(RésuméTVAData data, decimal? montant)
		{
			if (montant.HasValue)
			{
				FormattedText m = Converters.MontantToString (montant.Value);

				if (data.LigneDeTotal)
				{
					m = m.ApplyBold ();
				}

				return m;
			}
			else
			{
				return FormattedText.Empty;
			}
		}


		private new RésuméTVAOptions Options
		{
			get
			{
				return this.options as RésuméTVAOptions;
			}
		}


		private class BlocDeRésumé
		{
			public BlocDeRésumé(ComptaCompteEntity compte, ComptaCodeTVAEntity codeTVA, RésuméTVAOptions options)
			{
				this.compte  = compte;
				this.codeTVA = codeTVA;
				this.options = options;

				this.lignes = new List<RésuméTVAData>();
			}

			public ComptaCompteEntity Compte
			{
				get
				{
					return this.compte;
				}
			}

			public ComptaCodeTVAEntity CodeTVA
			{
				get
				{
					return this.codeTVA;
				}
			}

			public void AddEcriture(ComptaEcritureEntity écritureDeBase, ComptaEcritureEntity écritureDeCode)
			{
				RésuméTVAData ligne;
				var compteTVA = écritureDeBase.CompteTVA;

				if (this.options.MontreEcritures)
				{
					ligne = new RésuméTVAData
					{
						Entity  = écritureDeBase,
						Numéro  = compteTVA.Numéro,
						Titre   = écritureDeBase.Libellé,
						CodeTVA = écritureDeCode.CodeTVA.Code,
						Taux    = écritureDeCode.TauxTVA,
						Date    = écritureDeBase.Date,
						Pièce   = écritureDeBase.Pièce,
					};

					this.lignes.Add (ligne);
				}
				else
				{
					if (this.options.ParCodeTVA)  // par code TVA ?
					{
						ligne = this.lignes.Where (x => x.Numéro == compteTVA.Numéro).FirstOrDefault ();

						if (ligne == null)
						{
							ligne = new RésuméTVAData
							{
								Numéro = compteTVA.Numéro,
								Titre  = compteTVA.Titre,
							};

							this.lignes.Add (ligne);
						}
					}
					else  // par comptes ?
					{
						ligne = this.lignes.Where (x => x.Titre == écritureDeCode.CodeTVA.Code).FirstOrDefault ();

						if (ligne == null)
						{
							ligne = new RésuméTVAData
							{
								Titre = écritureDeCode.CodeTVA.Code,
							};

							this.lignes.Add (ligne);
						}
					}
				}

				//	Totalise le montant.
				if (écritureDeCode.MontantComplément.HasValue)
				{
					if (ligne.Montant.HasValue)
					{
						ligne.Montant += écritureDeCode.MontantComplément;
					}
					else
					{
						ligne.Montant = écritureDeCode.MontantComplément;
					}
				}

				//	Totalise la TVA.
				if (ligne.TVA.HasValue)
				{
					ligne.TVA += écritureDeCode.Montant;
				}
				else
				{
					ligne.TVA = écritureDeCode.Montant;
				}

				//	Calcule la différence éventuelle.
				if (écritureDeCode.MontantComplément.HasValue)
				{
					var montantHT  = ligne.Montant.GetValueOrDefault ();
					var montantTVA = ligne.TVA.GetValueOrDefault ();
					var tauxTVA    = écritureDeCode.TauxTVA.GetValueOrDefault ();

					var calculHT = TVA.CalculeHT (montantHT+montantTVA, tauxTVA);
					var diff = montantHT - calculHT;

					if (diff != 0)
					{
						if (ligne.Différence.HasValue)
						{
							ligne.Différence += diff;
						}
						else
						{
							ligne.Différence = diff;
						}
					}
				}
			}

			public List<RésuméTVAData> Lignes
			{
				get
				{
					return this.lignes;
				}
			}


			private readonly ComptaCompteEntity			compte;
			private readonly ComptaCodeTVAEntity		codeTVA;
			private readonly RésuméTVAOptions			options;
			private readonly List<RésuméTVAData>		lignes;
		}
	}
}