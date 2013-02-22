using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	/// <summary>
	/// Description d'un compte.
	/// Il n'y a pas de type; un compte terminal peut forcément recevoir des écritures.
	/// </summary>
	public class Compte : AbstractRecord
	{
		public FormattedText		Numéro;
		public FormattedText		Titre;
		public FormattedText		Commentaire;  // utile ?
		public CatégorieDeCompte	Catégorie;

		public Compte				Parent;
		public List<Compte>			Enfants;  // redondant, utile ?
		public int					Niveau;  // redondant, utile ?

		public CodeTva				CodeTvaParDéfaut;
		public List<CodeTva>		CodesTvaPossibles;

		public Compte				CompteOuvBoucl;
		public int					IndexOuvBoucl;
		public Monnaie				Monnaie;
		public Budget				Budgets;
	}
}
