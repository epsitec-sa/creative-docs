using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Data
{
	/// <summary>
	/// Cette classe représente l'ensemble d'une comptabilité, parfois appelé "mandat".
	/// Elle contient principalement le plan comptable et les périodes, qui contiennent
	/// elles-mêmes les écritures.
	/// </summary>
	public class Comptabilité : AbstractObjetComptable
	{
		public FormattedText			Nom;
		public FormattedText			Commentaire;  // utile ?
		public List<Compte>				PlanComptable;
		public List<PériodeComptable>	Périodes;
		public List<Journal>			Journaux;
		public List<Libellé>			Libellés;
		public List<EcritureModèle>		EcrituresModèle;
		public List<GénérateurDePièces>	GénérateursDePièces;
		public List<CodeTva>			CodesTva;
		public List<ListeTauxTva>		ListeTauxTva;
		public List<Monnaie>			Monnaies;
		public List<Utilisateur>		Utilisateurs;
	}
}
