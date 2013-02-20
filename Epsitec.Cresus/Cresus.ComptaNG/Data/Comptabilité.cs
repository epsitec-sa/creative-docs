using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Data
{
	public class Comptabilité : ObjetComptable
	{
		public FormattedText			Nom;
		public FormattedText			Commentaire;  // utile ?
		public List<Compte>				PlanComptable;
		public List<PériodeComptable>	Périodes;
		public List<Journal>			Journaux;
		public List<Libellé>			Libellés;
		public List<EcritureModèle>		EcrituresModèle;
		public List<GénérateurPièce>	GénérateursPièce;
		public List<CodeTva>			CodesTva;
		public List<ListeTauxTva>		ListeTauxTva;
		public List<Monnaie>			Monnaies;
		public List<Utilisateur>		Utilisateurs;
	}
}
