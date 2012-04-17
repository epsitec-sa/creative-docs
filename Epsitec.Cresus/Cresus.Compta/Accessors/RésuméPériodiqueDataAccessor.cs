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
	/// Gère l'accès aux données du résumé périodique de la comptabilité.
	/// </summary>
	public class RésuméPériodiqueDataAccessor : AbstractDataAccessor
	{
		public RésuméPériodiqueDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.options    = this.mainWindowController.GetSettingsOptions<RésuméPériodiqueOptions> ("Présentation.RésuméPériodique.Options", this.compta);
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.RésuméPériodique.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData ("Présentation.RésuméPériodique.Filter");

			this.UpdateAfterOptionsChanged ();
		}

		private void FilterInitialize(SearchData data)
		{
			data.FirstTabData.Column              = ColumnType.Solde;
			data.FirstTabData.SearchText.Mode     = SearchMode.WholeContent;
			data.FirstTabData.SearchText.Invert   = true;
			data.FirstTabData.SearchText.FromText = Converters.MontantToString (0, this.compta.Monnaies[0]);
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

			//	Crée un SoldesJournalManager par colunne.
			var soldesManagers = new List<SoldesJournalManager> ();
			RésuméPériodiqueDataAccessor.ColumnsProcess (this.période, this.Options, (index, dateDébut, dateFin) =>
			{
				var soldesManager = new SoldesJournalManager (this.compta);
				soldesManager.Initialize (this.période.Journal, dateDébut, Dates.AddDays (dateFin, -1));
				soldesManagers.Add (soldesManager);
			});

			//	Génère les différentes lignes, une par compte.
			foreach (var compte in this.compta.PlanComptable)
			{
				if (compte.Catégorie == CatégorieDeCompte.Inconnu)
				{
					continue;
				}

#if false
				bool empty = true;
				for (int i = 0; i < soldesManagers.Count; i++)
				{
					var solde = soldesManagers[i].GetSolde (compte);
					if (solde.GetValueOrDefault () != 0)
					{
						empty = false;
					}
				}
				if (empty)
				{
					continue;
				}
#endif

				var data = new RésuméPériodiqueData
				{
					Entity    = compte,
					Numéro    = compte.Numéro,
					Titre     = compte.Titre,
					Catégorie = compte.Catégorie,
					Type      = compte.Type,
					Niveau    = compte.Niveau,
				};

				decimal total = 0;
				for (int i = 0; i < soldesManagers.Count; i++)
				{
					var solde = soldesManagers[i].GetSolde (compte).GetValueOrDefault ();
					data.SetSolde (i, solde);
					total += solde;
				}

				data.Solde = total;

				this.readonlyAllData.Add (data);
			}

			base.UpdateFilter ();
		}

		public static void ColumnsProcess(ComptaPériodeEntity période, RésuméPériodiqueOptions options, System.Action<int, Date, Date> action)
		{
			var dateDébut    = période.DateDébut;
			var dateFin      = dateDébut;
			var débutPériode = période.DateDébut;
			var finPériode   = Dates.AddDays (période.DateFin, 1);  // normalement, 1er janvier de l'année suivante

			int index = 0;
			do
			{
				dateFin = Dates.AddMonths (dateDébut, options.NumberOfMonths);

				if (dateFin > finPériode)
				{
					dateFin = finPériode;
				}

				action (index++, options.Cumul ? débutPériode : dateDébut, Dates.AddDays (dateFin, -1));

				dateDébut = dateFin;
			}
			while (dateFin < finPériode);
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var data = this.GetReadOnlyData (row, all) as RésuméPériodiqueData;

			if (data == null)
			{
				return FormattedText.Null;
			}

			if (column >= ColumnType.Solde1 && column <= ColumnType.Solde12)
			{
				int rank = column - ColumnType.Solde1;
				var solde = data.GetSolde (rank);
				return Converters.MontantToString (solde, this.compta.Monnaies[0]);
			}

			switch (column)
			{
				case ColumnType.Numéro:
					return data.Numéro;

				case ColumnType.Titre:
					return data.Titre;

				case ColumnType.Catégorie:
					return Converters.CatégorieToString (data.Catégorie);

				case ColumnType.Type:
					return Converters.TypeToString (data.Type);

				case ColumnType.Profondeur:
					return (data.Niveau+1).ToString ();

				case ColumnType.Solde:
					return Converters.MontantToString (data.Solde, this.compta.Monnaies[0]);

				default:
					return FormattedText.Null;
			}
		}


		private new RésuméPériodiqueOptions Options
		{
			get
			{
				return this.options as RésuméPériodiqueOptions;
			}
		}
	}
}