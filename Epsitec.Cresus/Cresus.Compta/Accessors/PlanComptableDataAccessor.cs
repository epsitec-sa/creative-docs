//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données du plan comptable de la comptabilité.
	/// </summary>
	public class PlanComptableDataAccessor : AbstractDataAccessor
	{
		public PlanComptableDataAccessor(BusinessContext businessContext, ComptabilitéEntity comptabilitéEntity)
			: base (businessContext, comptabilitéEntity)
		{
			this.StartCreationData ();
		}


		public override int Count
		{
			get
			{
				return this.comptabilitéEntity.PlanComptable.Count;
			}
		}

		public override AbstractEntity GetEditionData(int row)
		{
			if (row < 0 || row >= this.comptabilitéEntity.PlanComptable.Count)
			{
				return null;
			}
			else
			{
				return this.comptabilitéEntity.PlanComptable[row];
			}
		}

		public override FormattedText GetText(int row, ColumnType column)
		{
			if (row < 0 || row >= this.Count)
			{
				return FormattedText.Null;
			}

			var compte = this.comptabilitéEntity.PlanComptable[row];

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

			var compte1 = this.comptabilitéEntity.PlanComptable[row];
			var compte2 = this.comptabilitéEntity.PlanComptable[row+1];

			return compte1.Catégorie != compte2.Catégorie;
		}


		public override int InsertionPointRow
		{
			get
			{
				var numéro = this.editionData[0].GetText (ColumnType.Numéro);

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


		public override void InsertEditionData(int index)
		{
			var newData = new PlanComptableEditionData (this.comptabilitéEntity);

			if (index == -1)
			{
				this.editionData.Add (newData);
			}
			else
			{
				this.editionData.Insert (index, newData);
			}

			this.countEditedRow = this.editionData.Count;
		}

		public override void StartCreationData()
		{
			this.editionData.Clear ();
			this.editionData.Add (new PlanComptableEditionData (this.comptabilitéEntity));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isModification = false;
			this.justCreated = false;
		}

		protected override void PrepareEditionLine(int line)
		{
			this.editionData[line].SetText (ColumnType.Type,          "Normal");
			this.editionData[line].SetText (ColumnType.IndexOuvBoucl, "1");
		}

		public override void StartModificationData(int row)
		{
			this.editionData.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.comptabilitéEntity.PlanComptable.Count)
			{
				var data = new PlanComptableEditionData (this.comptabilitéEntity);
				var compte = this.comptabilitéEntity.PlanComptable[row];
				data.EntityToData (compte);

				this.editionData.Add (data);
				this.countEditedRow++;
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isModification = true;
			this.justCreated = false;
		}

		public override void UpdateEditionData()
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

			this.comptabilitéEntity.PlanComptableUpdate ();
			this.SearchUpdate ();
		}

		private void UpdateCreationData()
		{
			int firstRow = -1;

			foreach (var data in this.editionData)
			{
				var compte = this.CreateCompte ();
				data.DataToEntity (compte);

				int row = this.GetSortedRow (compte.Numéro);
				this.comptabilitéEntity.PlanComptable.Insert (row, compte);

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

			foreach (var data in this.editionData)
			{
				if (row >= this.firstEditedRow+this.initialCountEditedRow)
				{
					//	Crée un compte manquant.
					var compte = this.CreateCompte ();
					data.DataToEntity (compte);
					this.comptabilitéEntity.PlanComptable.Insert (row, compte);
				}
				else
				{
					//	Met à jour un compte existante.
					var compte = this.comptabilitéEntity.PlanComptable[row];
					data.DataToEntity (compte);
				}

				row++;
			}

			//	Supprime les comptes surnuméraires.
			int countToDelete  = this.initialCountEditedRow - this.editionData.Count;
			while (countToDelete > 0)
			{
				var compte = this.comptabilitéEntity.PlanComptable[row];
				this.DeleteCompte (compte);
				this.comptabilitéEntity.PlanComptable.RemoveAt (row);

				countToDelete--;
			}

			this.countEditedRow = this.editionData.Count;

			//	Vérifie si les comptes modifiés doivent changer de place.
			if (!this.HasCorrectOrder (this.firstEditedRow) ||
				!this.HasCorrectOrder (this.firstEditedRow+this.countEditedRow-1))
			{
				var temp = new List<ComptabilitéCompteEntity> ();

				for (int i = 0; i < this.countEditedRow; i++)
				{
					var compte = this.comptabilitéEntity.PlanComptable[this.firstEditedRow];
					temp.Add (compte);
					this.comptabilitéEntity.PlanComptable.RemoveAt (this.firstEditedRow);
				}

				int newRow = this.GetSortedRow (temp[0].Numéro);

				for (int i = 0; i < this.countEditedRow; i++)
				{
					var compte = temp[i];
					this.comptabilitéEntity.PlanComptable.Insert (newRow+i, compte);
				}

				this.firstEditedRow = newRow;
			}
		}

		public override void RemoveModificationData()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
				{
					var compte = this.comptabilitéEntity.PlanComptable[row];
					this.DeleteCompte (compte);
					this.comptabilitéEntity.PlanComptable.RemoveAt (row);
				}

				this.SearchUpdate ();
				this.StartCreationData ();
			}
		}


		private ComptabilitéCompteEntity CreateCompte()
		{
			if (this.businessContext == null)
			{
				return new ComptabilitéCompteEntity ();
			}
			else
			{
				return this.businessContext.CreateEntity<ComptabilitéCompteEntity> ();
			}
		}

		private void DeleteCompte(ComptabilitéCompteEntity compte)
		{
			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (compte);
			}
		}


		private bool HasCorrectOrder(int row)
		{
			var compte = this.comptabilitéEntity.PlanComptable[row];
			return this.HasCorrectOrder (row, compte.Numéro);
		}

		private bool HasCorrectOrder(int row, FormattedText numéro)
		{
			if (row > 0 && this.Compare (row-1, numéro) > 0)
			{
				return false;
			}

			if (row > 0 && row < this.comptabilitéEntity.PlanComptable.Count-1 && this.Compare (row+1, numéro) < 0)
			{
				return false;
			}

			return true;
		}

		private int Compare(int row, FormattedText numéro)
		{
			var compte = this.comptabilitéEntity.PlanComptable[row];
			return compte.Numéro.ToSimpleText ().CompareTo (numéro.ToSimpleText ());
		}

		private int GetSortedRow(FormattedText numéro)
		{
			string n = numéro.ToSimpleText ();
			int count = this.comptabilitéEntity.PlanComptable.Count;

			for (int row = count-1; row >= 0; row--)
			{
				var compte = this.comptabilitéEntity.PlanComptable[row];

				int c = compte.Numéro.ToSimpleText ().CompareTo (n);
				if (c <= 0)
				{
					return row+1;
				}
			}

			return 0;
		}


		public static FormattedText GetNuméro(ComptabilitéCompteEntity compte)
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

		public static ComptabilitéCompteEntity GetCompte(ComptabilitéEntity comptabilité, FormattedText numéro)
		{
			numéro = PlanComptableDataAccessor.GetCompteNuméro (numéro);

			if (numéro.IsNullOrEmpty)
			{
				return null;
			}
			else
			{
				return comptabilité.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();
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

		public static ComptabilitéCompteEntity GetCompteEntity(ComptabilitéEntity comptabilité, FormattedText description)
		{
			//	Retourne le compte, à partir de la description "numéro titre".
			description = PlanComptableDataAccessor.GetCompteNuméro (description);
			return comptabilité.PlanComptable.Where (x => x.Numéro == description).FirstOrDefault ();
		}

		public static FormattedText GetCompteDescription(ComptabilitéCompteEntity compte)
		{
			//	Retourne la description "numéro titre" d'un compte.
			return TextFormatter.FormatText (compte.Numéro, compte.Titre);
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
	}
}