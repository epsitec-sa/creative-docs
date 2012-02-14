//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données du plan comptable de la comptabilité.
	/// </summary>
	public class PlanComptableDataAccessor : AbstractDataAccessor
	{
		public PlanComptableDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.searchData = this.mainWindowController.GetSettingsSearchData<SearchData> ("Présentation.PlanComptable.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData<SearchData> ("Présentation.PlanComptable.Filter");

			this.StartCreationLine ();
		}


		public override void FilterUpdate()
		{
			this.UpdateAfterOptionsChanged ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			this.planComptableAll = this.comptaEntity.PlanComptable;

			if (this.IsAllComptes)
			{
				this.planComptable = this.planComptableAll;
			}
			else
			{
				this.planComptable = new List<ComptaCompteEntity> ();
				this.planComptable.Clear ();

				int count = this.planComptableAll.Count;
				for (int row = 0; row < count; row++)
				{
					int founds = this.FilterLine (row);

					if (founds != 0 && (this.filterData.OrMode || founds == this.filterData.TabsData.Count))
					{
						this.planComptable.Add (this.planComptableAll[row]);
					}
				}
			}
		}


		public override int AllCount
		{
			get
			{
				return this.planComptableAll.Count;
			}
		}

		public override int Count
		{
			get
			{
				return this.planComptable.Count;
			}
		}

		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.planComptable.Count)
			{
				return null;
			}
			else
			{
				return this.planComptable[row];
			}
		}

		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var planComptable = all ? this.planComptableAll : this.planComptable;

			if (row < 0 || row >= planComptable.Count)
			{
				return FormattedText.Null;
			}

			var compte = planComptable[row];

			switch (column)
			{
				case ColumnType.Numéro:
					return compte.Numéro;

				case ColumnType.Titre:
					return compte.Titre;

				case ColumnType.Catégorie:
					return PlanComptableDataAccessor.CatégorieToText (compte.Catégorie);

				case ColumnType.Type:
					return PlanComptableDataAccessor.TypeToText (compte.Type);

				case ColumnType.Groupe:
					return PlanComptableDataAccessor.GetNuméro (compte.Groupe);

				case ColumnType.TVA:
					//?return PlanComptableAccessor.TVAToText (compte.TVA);
					return FormattedText.Null;

				case ColumnType.CompteOuvBoucl:
					return PlanComptableDataAccessor.GetNuméro (compte.CompteOuvBoucl);

				case ColumnType.IndexOuvBoucl:
					if (compte.IndexOuvBoucl == 0)
					{
						return FormattedText.Empty;
					}
					else
					{
						return compte.IndexOuvBoucl.ToString ();
					}

				case ColumnType.Monnaie:
					return compte.Monnaie;

				case ColumnType.Profondeur:
					return (compte.Niveau+1).ToString ();

				default:
					return FormattedText.Null;
			}
		}

		public override bool HasBottomSeparator(int row)
		{
			if (row < 0 || row >= this.Count-1)
			{
				return false;
			}

			var compte1 = this.planComptable[row];
			var compte2 = this.planComptable[row+1];

			return compte1.Catégorie != compte2.Catégorie;
		}


		public override ColumnType ColumnForInsertionPoint
		{
			get
			{
				return ColumnType.Numéro;
			}
		}

		public override int InsertionPointRow
		{
			get
			{
				var numéro = this.editionLine[0].GetText (ColumnType.Numéro);

				if (!this.justCreated && this.firstEditedRow != -1 && this.countEditedRow != 0)
				{
					if (this.HasCorrectOrder (this.firstEditedRow, numéro))
					{
						return -1;
					}
				}

				return this.GetSortedRow (numéro);
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new PlanComptableEditionLine (this.controller);

			if (index == -1)
			{
				this.editionLine.Add (newData);
			}
			else
			{
				this.editionLine.Insert (index, newData);
			}

			this.countEditedRow = this.editionLine.Count;
		}

		public override void StartCreationLine()
		{
			this.editionLine.Clear ();
			this.editionLine.Add (new PlanComptableEditionLine (this.controller));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isModification = false;
			this.justCreated = false;
		}

		protected override void PrepareEditionLine(int line)
		{
			this.editionLine[line].SetText (ColumnType.Type, Converters.TypeToString (TypeDeCompte.Normal));
			this.editionLine[line].SetText (ColumnType.IndexOuvBoucl, "1");
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.planComptable.Count)
			{
				var data = new PlanComptableEditionLine (this.controller);
				var compte = this.planComptable[row];
				data.EntityToData (compte);

				this.editionLine.Add (data);
				this.countEditedRow++;
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isModification = true;
			this.justCreated = false;
		}

		public override void UpdateEditionLine()
		{
			if (this.isModification)
			{
				this.UpdateModificationData ();
				this.justCreated = false;
			}
			else
			{
				this.UpdateCreationData ();
				this.justCreated = true;
			}

			this.soldesJournalManager.Initialize (this.périodeEntity.Journal);
			this.SearchUpdate ();
			this.controller.MainWindowController.SetDirty ();
		}

		private void UpdateCreationData()
		{
			int firstRow = -1;

			foreach (var data in this.editionLine)
			{
				var compte = this.CreateCompte ();
				data.DataToEntity (compte);

				int row = this.GetSortedRow (compte.Numéro);
				this.planComptable.Insert (row, compte);

				if (!this.IsAllComptes)
				{
					int globalRow = this.GetSortedRow (compte.Numéro, global: true);
					this.comptaEntity.PlanComptable.Insert (globalRow, compte);
				}

				if (firstRow == -1)
				{
					firstRow = row;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;
			var initialCompte = this.planComptable[row];

			//	On passe dans l'espace global.
			row = this.comptaEntity.PlanComptable.IndexOf (initialCompte);
			int globalFirstEditerRow = row;

			foreach (var data in this.editionLine)
			{
				if (row >= globalFirstEditerRow+this.initialCountEditedRow)
				{
					//	Crée un compte manquant.
					var compte = this.CreateCompte ();
					data.DataToEntity (compte);
					this.comptaEntity.PlanComptable.Insert (row, compte);
				}
				else
				{
					//	Met à jour un compte existante.
					var compte = this.comptaEntity.PlanComptable[row];
					data.DataToEntity (compte);
				}

				row++;
			}

			//	Supprime les comptes surnuméraires.
			int countToDelete  = this.initialCountEditedRow - this.editionLine.Count;
			while (countToDelete > 0)
			{
				var compte = this.comptaEntity.PlanComptable[row];
				this.DeleteCompte (compte);
				this.comptaEntity.PlanComptable.RemoveAt (row);

				countToDelete--;
			}

			this.countEditedRow = this.editionLine.Count;

			//	Vérifie si les comptes modifiés doivent changer de place.
			if (!this.HasCorrectOrder (globalFirstEditerRow, global: true) ||
				!this.HasCorrectOrder (globalFirstEditerRow+this.countEditedRow-1, global: true))
			{
				var temp = new List<ComptaCompteEntity> ();

				for (int i = 0; i < this.countEditedRow; i++)
				{
					var compte = this.comptaEntity.PlanComptable[globalFirstEditerRow];
					temp.Add (compte);
					this.comptaEntity.PlanComptable.RemoveAt (globalFirstEditerRow);
				}

				int newRow = this.GetSortedRow (temp[0].Numéro, global: true);

				for (int i = 0; i < this.countEditedRow; i++)
				{
					var compte = temp[i];
					this.comptaEntity.PlanComptable.Insert (newRow+i, compte);
				}

				globalFirstEditerRow = newRow;
			}

			//	On revient dans l'espace spécifique.
			this.UpdateAfterOptionsChanged ();
			this.firstEditedRow = this.planComptable.IndexOf (initialCompte);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
				{
					var compte = this.comptaEntity.PlanComptable[row];
					this.DeleteCompte (compte);
					this.planComptable.RemoveAt (row);

					this.comptaEntity.PlanComptable.Remove (compte);
				}

				this.SearchUpdate ();
				this.StartCreationLine ();
			}
		}


#if false
		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.planComptable[this.firstEditedRow];
				var t2 = this.planComptable[this.firstEditedRow+direction];

				this.planComptable[this.firstEditedRow] = t2;
				this.planComptable[this.firstEditedRow+direction] = t1;

				this.firstEditedRow += direction;

				return true;
			}
			else
			{
				return false;
			}
		}

		public override bool IsMoveEditionLineEnable(int direction)
		{
			if (this.firstEditedRow == -1)
			{
				return false;
			}

			return this.firstEditedRow+direction >= 0 && this.firstEditedRow+direction < this.Count;
		}
#endif


		private ComptaCompteEntity CreateCompte()
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				return new ComptaCompteEntity ();
			}
			else
			{
				return this.businessContext.CreateEntity<ComptaCompteEntity> ();
			}
		}

		private void DeleteCompte(ComptaCompteEntity compte)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (compte);
			}
		}


		private bool HasCorrectOrder(int row, bool global = false)
		{
			var planComptable = this.GetPlanComptable (global);
			var compte = planComptable[row];
			return this.HasCorrectOrder (row, compte.Numéro, global);
		}

		private bool HasCorrectOrder(int row, FormattedText numéro, bool global = false)
		{
			var planComptable = this.GetPlanComptable (global);

			if (row > 0 && this.Compare (row-1, numéro, global) > 0)
			{
				return false;
			}

			if (row < planComptable.Count-1 && this.Compare (row+1, numéro, global) < 0)
			{
				return false;
			}

			return true;
		}

		private int Compare(int row, FormattedText numéro, bool global)
		{
			var planComptable = this.GetPlanComptable (global);
			var compte = planComptable[row];
			return compte.Numéro.ToSimpleText ().CompareTo (numéro.ToSimpleText ());
		}

		private int GetSortedRow(FormattedText numéro, bool global = false)
		{
			var planComptable = this.GetPlanComptable (global);

			string n = numéro.ToSimpleText ();
			int count = planComptable.Count;

			for (int row = count-1; row >= 0; row--)
			{
				var compte = planComptable[row];

				int c = compte.Numéro.ToSimpleText ().CompareTo (n);
				if (c <= 0)
				{
					return row+1;
				}
			}

			return 0;
		}

		private IList<ComptaCompteEntity> GetPlanComptable(bool global)
		{
			if (global)
			{
				return this.comptaEntity.PlanComptable;
			}
			else
			{
				return this.planComptable;
			}
		}


		private bool IsAllComptes
		{
			get
			{
				return this.filterData == null || this.filterData.IsEmpty;
			}
		}


		public static FormattedText GetNuméro(ComptaCompteEntity compte)
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

		public static ComptaCompteEntity GetCompte(ComptaEntity compta, FormattedText numéro)
		{
			numéro = PlanComptableDataAccessor.GetCompteNuméro (numéro);

			if (numéro.IsNullOrEmpty)
			{
				return null;
			}
			else
			{
				return compta.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();
			}
		}


		public static FormattedText GetCompteNuméro(FormattedText description)
		{
			if (!description.IsNullOrEmpty)
			{
				int i = description.ToSimpleText ().IndexOf (' ');  // contient "numéro titre" ?
				if (i != -1)
				{
					description = description.ToSimpleText ().Substring (0, i);
				}
			}

			return description;
		}

		public static ComptaCompteEntity GetCompteEntity(ComptaEntity compta, FormattedText description)
		{
			//	Retourne le compte, à partir de la description "numéro titre".
			description = PlanComptableDataAccessor.GetCompteNuméro (description);
			return compta.PlanComptable.Where (x => x.Numéro == description).FirstOrDefault ();
		}

		public static FormattedText GetCompteDescription(ComptaCompteEntity compte)
		{
			//	Retourne la description "numéro titre" d'un compte.
			return Core.TextFormatter.FormatText (compte.Numéro, compte.Titre);
		}


		public static bool TextToCatégorie(FormattedText text, out CatégorieDeCompte catégorie)
		{
			if (System.Enum.TryParse<CatégorieDeCompte> (text.ToSimpleText (), out catégorie))
			{
				return true;
			}

			catégorie = CatégorieDeCompte.Inconnu;
			return false;
		}

		public static FormattedText CatégorieToText(CatégorieDeCompte catégorie)
		{
			return catégorie.ToString ();
		}


		public static bool TextToType(FormattedText text, out TypeDeCompte type)
		{
			if (System.Enum.TryParse<TypeDeCompte> (text.ToSimpleText (), out type))
			{
				return true;
			}

			type = TypeDeCompte.Normal;
			return false;
		}

		public static FormattedText TypeToText(TypeDeCompte type)
		{
			return type.ToString ();
		}


#if false
		public static bool TextToTVA(FormattedText text, out VatCode tva)
		{
			if (System.Enum.TryParse<VatCode> (text.ToSimpleText (), out tva))
			{
				return true;
			}

			tva = VatCode.None;
			return false;
		}

		public static FormattedText TVAToText(VatCode tva)
		{
			return tva.ToString ();
		}
#endif


		private IList<ComptaCompteEntity>			planComptableAll;
		private IList<ComptaCompteEntity>			planComptable;
	}
}