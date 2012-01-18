//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	/// <summary>
	/// Gère l'accès aux données de la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceDataAccessor : AbstractDataAccessor
	{
		public BalanceDataAccessor(BusinessContext businessContext, ComptabilitéEntity comptabilitéEntity)
			: base (businessContext, comptabilitéEntity)
		{
			this.options = new BalanceOptions (this.comptabilitéEntity);

			this.UpdateAfterOptionsChanged ();
		}


		public override void UpdateAfterOptionsChanged()
		{
			this.readonlyData.Clear ();

			ComptabilitéCompteEntity lastCompte = null;
			decimal totalDébit  = 0;
			decimal totalCrédit = 0;
			decimal totalSoldeD = 0;
			decimal totalSoldeC = 0;

			this.comptabilitéEntity.PlanComptableUpdate (this.options.DateDébut, this.options.DateFin);

			foreach (var compte in this.comptabilitéEntity.PlanComptable.OrderBy (x => x.Numéro))
			{
				if (compte.Catégorie == CatégorieDeCompte.Inconnu ||
					compte.Catégorie == CatégorieDeCompte.Exploitation)
				{
					continue;
				}

				if (this.Options.Profondeur.HasValue && compte.Niveau >= this.Options.Profondeur.Value)
				{
					continue;
				}

				var soldeDébit  = this.comptabilitéEntity.GetSoldeCompteDébit  (compte);
				var soldeCrédit = this.comptabilitéEntity.GetSoldeCompteCrédit (compte);
				var différence = soldeCrédit.GetValueOrDefault () - soldeDébit.GetValueOrDefault ();

				if (!this.Options.ComptesNuls && soldeDébit.GetValueOrDefault () == 0 && soldeCrédit.GetValueOrDefault () == 0)
				{
					continue;
				}

				var data = new BalanceData ();

				data.Numéro = compte.Numéro;
				data.Titre  = compte.Titre;
				data.Niveau = compte.Niveau;
				data.Débit  = soldeDébit .GetValueOrDefault () == 0 ? null : soldeDébit;
				data.Crédit = soldeCrédit.GetValueOrDefault () == 0 ? null : soldeCrédit;

				if (différence < 0)
				{
					data.SoldeDébit = -différence;
				}
				if (différence > 0)
				{
					data.SoldeCrédit = différence;
				}

				if (compte.Type == TypeDeCompte.Normal)
				{
					totalDébit  += soldeDébit.GetValueOrDefault ();
					totalCrédit += soldeCrédit.GetValueOrDefault ();

					if (différence < 0)
					{
						totalSoldeD -= différence;
					}
					if (différence > 0)
					{
						totalSoldeC += différence;
					}
				}

				if (lastCompte == null || lastCompte.Catégorie != compte.Catégorie)  // changement de catégorie ?
				{
					data.IsBold = true;
					this.SetBottomSeparatorToPreviousLine ();
				}

				lastCompte = compte;

				this.readonlyData.Add (data);
			}

			this.SetBottomSeparatorToPreviousLine ();

			// Génère la dernière ligne.
			{
				var data = new BalanceData ();

				data.Titre       = "Mouvement";
				data.Débit       = totalDébit;
				data.Crédit      = totalCrédit;
				data.SoldeDébit  = totalSoldeD;
				data.SoldeCrédit = totalSoldeC;
				data.IsItalic    = true;

				this.readonlyData.Add (data);
			}
		}


		public override FormattedText GetText(int row, ColumnType column)
		{
			if (row < 0 || row >= this.Count)
			{
				return FormattedText.Null;
			}

			var data = this.readonlyData[row] as BalanceData;

			switch (column)
			{
				case ColumnType.Numéro:
					return data.Numéro;

				case ColumnType.Titre:
					return data.Titre;

				case ColumnType.Débit:
					return BalanceDataAccessor.GetMontant (data.Débit);

				case ColumnType.Crédit:
					return BalanceDataAccessor.GetMontant (data.Crédit);

				case ColumnType.SoldeDébit:
					return BalanceDataAccessor.GetMontant (data.SoldeDébit);

				case ColumnType.SoldeCrédit:
					return BalanceDataAccessor.GetMontant (data.SoldeCrédit);

				case ColumnType.Budget:
					return BalanceDataAccessor.GetMontant (data.Budget);

				default:
					return FormattedText.Null;
			}
		}


		private static FormattedText GetMontant(decimal? montant)
		{
			if (montant.HasValue)
			{
				return montant.Value.ToString ("0.00");
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		private BalanceOptions Options
		{
			get
			{
				return this.options as BalanceOptions;
			}
		}
	}
}