using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewer
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public abstract class Abstract : Widget
	{
		public Abstract(Module module)
		{
			this.module = module;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		public virtual AbstractTextField CurrentTextField
		{
			//	Retourne le texte éditable en cours d'édition.
			get
			{
				return null;
			}
		}

		public virtual void DoSearch(string search, Searcher.SearchingMode mode)
		{
			//	Effectue une recherche.
		}

		public virtual void DoCount(string search, Searcher.SearchingMode mode)
		{
			//	Effectue un décompte.
		}

		public virtual void DoReplace(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un remplacement.
		}

		public virtual void DoReplaceAll(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un 'remplacer tout'.
		}

		public virtual void DoFilter(string filter, Searcher.SearchingMode mode)
		{
			//	Change le filtre des ressources visibles.
		}

		public virtual void DoAccess(string name)
		{
			//	Change la ressource visible.
		}

		public virtual void DoModification(string name)
		{
			//	Change la ressource modifiée visible.
		}

		public virtual void DoWarning(string name)
		{
			//	Change la ressource manquante visible.
		}

		public virtual void DoDelete()
		{
			//	Supprime la ressource sélectionnée.
		}

		public virtual void DoDuplicate(bool duplicate)
		{
			//	Duplique la ressource sélectionnée.
		}

		public virtual void DoMove(int direction)
		{
			//	Déplace la ressource sélectionnée.
		}

		public virtual void DoNewCulture()
		{
			//	Crée une nouvelle culture.
		}

		public virtual void DoDeleteCulture()
		{
			//	Supprime la culture courante.
		}

		public virtual void DoClipboard(string name)
		{
			//	Effectue une action avec le bloc-notes.
		}

		public virtual void DoFont(string name)
		{
			//	Effectue une modification de typographie.
		}


		public virtual string InfoAccessText
		{
			//	Donne le texte d'information sur l'accès en cours.
			get
			{
				return "";
			}
		}


		public virtual void UpdateCommands()
		{
			//	Met à jour les commandes en fonction de la ressource sélectionnée.
		}


		protected Module					module;
	}
}
