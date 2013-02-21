using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Common.Data;

namespace Epsitec.Cresus.ComptaNG.Server
{
	public class Interface
	{
		// Ouvre une vue d'un type donné. Retourne un canal.
		// Le callback 'refresh' est appelé lorsque la vue a changé suite à la
		// modification indirecte (par une autre vue ou par un autre utilisateur).
		public Guid OpenView(View view, System.Action<Guid> refresh)
		{
			return System.Guid.NewGuid ();
		}

		// Retourne le nombre d'enregistrements contenus dans une vue.
		public int GetCount(Guid channel)
		{
			return 0;
		}

		// Retourne le contenu d'enregistrements.
		public List<AbstractObjetComptable> GetData(Guid channel, int firstIndex, int count)
		{
			return null;
		}

		// Retourne l'index d'un enregistrement.
		public int GetIndex(Guid channel, AbstractObjetComptable data)
		{
			return -1;
		}

		// Valide un enregistrement.
		public Erreur Validate(Guid channel, AbstractObjetComptable data)
		{
			return Erreur.OK;
		}

		// Modifie un enregistrement.
		public void SetData(Guid channel, AbstractObjetComptable data)
		{
		}

		// Met à jour une vue, si elle a été modifiée par d'autres utilisateurs.
		// Ceci évite de faire un CloseView suivi d'un OpenView .
		public void Refresh(Guid channel)
		{
		}

		// Ferme la vue.
		public void CloseView(Guid channel)
		{
		}
	}
}
