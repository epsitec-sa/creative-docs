using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Panels : Abstract
	{
		public Panels(Module module) : base(module)
		{
			StaticText s = new StaticText(this);
			s.Text = "<b>TODO:</b> <i>Editeur d'interfaces...</i>";
			s.Margins = new Margins(20, 0, 0, 0);
			s.Dock = DockStyle.Fill;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		public override AbstractTextField CurrentTextField
		{
			//	Retourne le texte éditable en cours d'édition.
			get
			{
				return null;
			}
		}

		public override void DoSearch(string search, Searcher.SearchingMode mode)
		{
			//	Effectue une recherche.
		}

		public override void DoCount(string search, Searcher.SearchingMode mode)
		{
			//	Effectue une recherche.
		}

		public override void DoReplace(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un remplacement.
		}

		public override void DoReplaceAll(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un 'remplacer tout'.
		}

		public override void DoFilter(string filter, Searcher.SearchingMode mode)
		{
			//	Change le filtre des ressources visibles.
		}

		public override void DoAccess(string name)
		{
			//	Change la ressource visible.
		}

		public override void DoModification(string name)
		{
			//	Change la ressource modifiée visible.
		}

		public override void DoWarning(string name)
		{
			//	Change la ressource manquante visible.
		}

		public override void DoDelete()
		{
			//	Supprime la ressource sélectionnée.
		}

		public override void DoDuplicate(bool duplicate)
		{
			//	Duplique la ressource sélectionnée.
		}

		public override void DoMove(int direction)
		{
			//	Déplace la ressource sélectionnée.
		}

		public override void DoNewCulture()
		{
			//	Crée une nouvelle culture.
		}

		public override void DoDeleteCulture()
		{
			//	Supprime la culture courante.
		}

		public override void DoClipboard(string name)
		{
			//	Effectue une action avec le bloc-notes.
		}

		public override void DoFont(string name)
		{
			//	Effectue une modification de typographie.
		}


		public override string InfoAccessText
		{
			//	Donne le texte d'information sur l'accès en cours.
			get
			{
				return "";
			}
		}


		protected void UpdateCultures()
		{
			//	Met à jour les widgets pour les cultures.
		}

	}
}
