//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Gère l'accès aux données du plan comptable de la comptabilité.
	/// </summary>
	public class PlanComptableAccessor : AbstractDataAccessor<PlanComptableColumn, ComptabilitéCompteEntity>
	{
		public PlanComptableAccessor(ComptabilitéEntity comptabilitéEntity)
			: base (comptabilitéEntity)
		{
			this.UpdateSortedList ();
		}


		public override int Add(ComptabilitéCompteEntity compte)
		{
			this.comptabilitéEntity.PlanComptable.Add (compte);
			this.UpdateSortedList ();
			return this.sortedEntities.IndexOf (compte);
		}

		public override void Remove(ComptabilitéCompteEntity compte)
		{
			this.comptabilitéEntity.PlanComptable.Remove (compte);
			this.UpdateSortedList ();
		}

		public override void UpdateSortedList()
		{
			this.sortedEntities = this.comptabilitéEntity.PlanComptable.OrderBy (x => x.Numéro).ToList ();
		}


		public override FormattedText GetText(int row, PlanComptableColumn column)
		{
			if (row < 0 || row >= this.Count)
			{
				return FormattedText.Null;
			}

			var compte = this.sortedEntities[row];

			switch (column)
			{
				case PlanComptableColumn.Numéro:
					return compte.Numéro;

				case PlanComptableColumn.Titre:
					return compte.Titre;

				case PlanComptableColumn.Catégorie:
					return PlanComptableAccessor.CatégorieToText (compte.Catégorie);

				case PlanComptableColumn.Type:
					return PlanComptableAccessor.TypeToText (compte.Type);

				case PlanComptableColumn.Groupe:
					return PlanComptableAccessor.GetNuméro (compte.Groupe);

				case PlanComptableColumn.TVA:
					//?return PlanComptableAccessor.TVAToText (compte.TVA);
					return FormattedText.Null;

				case PlanComptableColumn.CompteOuvBoucl:
					return PlanComptableAccessor.GetNuméro (compte.CompteOuvBoucl);

				case PlanComptableColumn.IndexOuvBoucl:
					if (compte.IndexOuvBoucl == 0)
					{
						return FormattedText.Empty;
					}
					else
					{
						return compte.IndexOuvBoucl.ToString ();
					}

				case PlanComptableColumn.Monnaie:
					return compte.Monnaie;

				default:
					return FormattedText.Null;
			}
		}

		public override void SetText(int row, PlanComptableColumn column, FormattedText text)
		{
			if (row < 0 || row >= this.Count)
			{
				return;
			}

			var compte = this.sortedEntities[row];

			switch (column)
			{
				case PlanComptableColumn.Numéro:
					compte.Numéro = text;
					break;

				case PlanComptableColumn.Titre:
					compte.Titre = text;
					break;

				case PlanComptableColumn.Catégorie:
					CatégorieDeCompte c;
					if (PlanComptableAccessor.TextToCatégorie (text, out c))
					{
						compte.Catégorie = c;
					}
					break;

				case PlanComptableColumn.Type:
					TypeDeCompte t;
					if (PlanComptableAccessor.TextToType (text, out t))
					{
						compte.Type = t;
					}
					break;

				case PlanComptableColumn.Groupe:
					compte.Groupe = PlanComptableAccessor.GetCompte (this.comptabilitéEntity, text);
					break;

				case PlanComptableColumn.TVA:
					VatCode v;
					if (PlanComptableAccessor.TextToTVA (text, out v))
					{
						//?compte.TVA = v;
					}
					break;

				case PlanComptableColumn.CompteOuvBoucl:
					compte.CompteOuvBoucl = PlanComptableAccessor.GetCompte (this.comptabilitéEntity, text);
					break;

				case PlanComptableColumn.IndexOuvBoucl:
					int index;
					if (int.TryParse (text.ToSimpleText (), out index) &&
						index >= 1 && index <= 9)
					{
						compte.IndexOuvBoucl = index;
					}
					else
					{
						compte.IndexOuvBoucl = 0;
					}
					break;

				case PlanComptableColumn.Monnaie:
					compte.Monnaie = text.ToSimpleText ();
					break;
			}
		}


		private static FormattedText GetNuméro(ComptabilitéCompteEntity compte)
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

		private static ComptabilitéCompteEntity GetCompte(ComptabilitéEntity comptabilité, FormattedText numéro)
		{
			numéro = PlanComptableAccessor.GetCompteNuméro (numéro);

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
			description = PlanComptableAccessor.GetCompteNuméro (description);
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
	}
}