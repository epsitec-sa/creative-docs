using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Common.RequestData;

namespace Epsitec.Cresus.ComptaNG.Server
{
	/// <summary>
	/// Cette classe doit être la seule appelée par le client.
	/// </summary>
	public class Interface
	{
		// Ouvre une vue d'un type donné. Retourne un canal.
		// Le callback 'refresh' est appelé lorsque la vue a changé suite à la
		// modification indirecte (par une autre vue ou par un autre utilisateur).
		public Guid OpenView(ViewData view, SearchData seach, FilterData filter, System.Action<Guid> refresh)
		{
			return System.Guid.Empty;
		}

		// Donne un nouveau critère de recherche.
		public void UpdateSearch(Guid viewChannel, SearchData seach)
		{
		}

		// Donne un nouveau critère de filtre.
		public void UpdateFilter(Guid viewChannel, FilterData filter)
		{
		}

		// Donne de nouveaux critères de recherche et de filtre.
		// Pour être plus efficace, il est avantageux d'appeler cette méthode lorsque
		// les deux critères changent.
		public void UpdateSearchAndFilter(Guid viewChannel, SearchData seach, FilterData filter)
		{
		}

		// Retourne le nombre d'enregistrements contenus dans une vue.
		public int GetCount(Guid viewChannel)
		{
			return 0;
		}

		// Retourne le contenu de plusieurs enregistrements.
		public List<TravellingRecord> GetRecord(Guid viewChannel, int firstIndex, int count)
		{
			return null;
		}

		// Retourne l'index d'un enregistrement.
		public int GetIndex(Guid viewChannel, Guid dataGuid)
		{
			return -1;
		}

		// Retourne la prochaine donnée selon le critère de recherche, vers le haut ou vers le bas.
		public Guid Search(Guid viewChannel, Guid dataGuid, bool ascendent)
		{
			return System.Guid.Empty;
		}

		// Retourne l'ensemble des données correspondant au critère de recherche.
		public IEnumerable<Guid> SearchResults(Guid viewChannel)
		{
			return null;
		}

		// Valide un enregistrement.
		public IEnumerable<ErrorField> Validate(Guid viewChannel, TravellingRecord record)
		{
			return null;
		}

		// Modifie un enregistrement.
		public void SetRecord(Guid viewChannel, TravellingRecord record)
		{
		}

		// Met à jour une vue, si elle a été modifiée par d'autres utilisateurs.
		// Ceci évite de faire un CloseView suivi d'un OpenView.
		public void Refresh(Guid viewChannel)
		{
		}

		// Ferme la vue.
		public void CloseView(Guid viewChannel)
		{
		}
	}
}
