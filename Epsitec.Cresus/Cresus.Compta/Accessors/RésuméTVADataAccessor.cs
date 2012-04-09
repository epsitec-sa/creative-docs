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
					var compte = (écritureDeBase.OrigineTVA == "D") ? écritureDeBase.Débit : écritureDeBase.Crédit;
					var bloc = blocs.Where (x => x.Compte == compte).FirstOrDefault ();

					if (bloc == null)
					{
						bloc = new BlocDeRésumé (compte);
						blocs.Add (bloc);
					}

					bloc.AddEcriture (écriture);
				}
			}

			//	Passe en revue tous les blocs.
			var orderedBlocs = blocs.OrderBy (x => x.Compte.Numéro);

			decimal montantDu    = 0;
			decimal TVADue       = 0;
			decimal montantRecup = 0;
			decimal TVARecup     = 0;
			decimal montantTotal = 0;
			decimal TVATotal     = 0;

			foreach (var bloc in orderedBlocs)
			{
				decimal soustotalMontant = 0;
				decimal soustotalTVA     = 0;

				foreach (var ligne in bloc.Lignes)
				{
					soustotalMontant += ligne.Montant;
					soustotalTVA     += ligne.TVA;

					var data = new RésuméTVAData
					{
						CodeTVA = true,
						Titre   = ligne.Titre,
						Montant = ligne.Montant,
						TVA     = ligne.TVA,
					};

					this.readonlyAllData.Add (data);
				}

				{
					if (bloc.Compte.Catégorie == CatégorieDeCompte.Passif ||
						bloc.Compte.Catégorie == CatégorieDeCompte.Produit)
					{
						montantDu += soustotalMontant;
						TVADue    += soustotalTVA;
					}

					if (bloc.Compte.Catégorie == CatégorieDeCompte.Actif ||
						bloc.Compte.Catégorie == CatégorieDeCompte.Charge)
					{
						montantRecup += soustotalMontant;
						TVARecup     += soustotalTVA;
					}

					montantTotal += soustotalMontant;
					TVATotal     += soustotalTVA;

					var data = new RésuméTVAData
					{
						Compte             = bloc.Compte.Numéro,
						Titre              = bloc.Compte.Titre,
						Montant            = soustotalMontant,
						TVA                = soustotalTVA,
						HasBottomSeparator = true,
					};

					this.readonlyAllData.Add (data);
				}
			}

			//	Ajoute les lignes de total.
			{
				var data = new RésuméTVAData
				{
					Titre              = "Total TVA due",
					Montant            = montantDu,
					TVA                = TVADue,
				};

				this.readonlyAllData.Add (data);
			}

			{
				var data = new RésuméTVAData
				{
					Titre              = "Total TVA à récupérer",
					Montant            = montantRecup,
					TVA                = TVARecup,
				};

				this.readonlyAllData.Add (data);
			}

			{
				var data = new RésuméTVAData
				{
					Titre              = "Total",
					Montant            = montantTotal,
					TVA                = TVATotal,
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
					return data.Compte;

				case ColumnType.Titre:
					return data.Titre;

				case ColumnType.Montant:
					return RésuméTVADataAccessor.GetMontant (data, data.Montant);

				case ColumnType.MontantTVA:
					return RésuméTVADataAccessor.GetMontant (data, data.TVA);

				default:
					return FormattedText.Null;
			}
		}

		private static FormattedText GetMontant(RésuméTVAData data, decimal montant)
		{
			if (data.CodeTVA)
			{
				return Converters.MontantToString (montant) + "  ";
			}
			else
			{
				return Converters.MontantToString (montant);
			}
		}


		private class BlocDeRésumé
		{
			public BlocDeRésumé(ComptaCompteEntity compte)
			{
				this.compte = compte;
				this.lignes = new List<RésuméTVAData>();
			}

			public ComptaCompteEntity Compte
			{
				get
				{
					return this.compte;
				}
			}

			public void AddEcriture(ComptaEcritureEntity écritureDeCode)
			{
				var ligne = this.lignes.Where (x => x.Titre == écritureDeCode.CodeTVA.Code).FirstOrDefault ();

				if (ligne == null)
				{
					ligne = new RésuméTVAData ();
					ligne.Titre = écritureDeCode.CodeTVA.Code;

					this.lignes.Add (ligne);
				}

				ligne.Montant += écritureDeCode.MontantComplément.GetValueOrDefault ();
				ligne.TVA     += écritureDeCode.Montant;
			}

			public List<RésuméTVAData> Lignes
			{
				get
				{
					return this.lignes;
				}
			}


			private readonly ComptaCompteEntity		compte;
			private readonly List<RésuméTVAData>	lignes;
		}
	}
}