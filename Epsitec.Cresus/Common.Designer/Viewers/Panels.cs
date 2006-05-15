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
			int tabIndex = 0;

			Widget left = new Widget(this);
			left.MinWidth = 200;
			left.PreferredWidth = 200;
			left.Dock = DockStyle.Left;

			this.labelEdit = new TextFieldEx(left);
			this.labelEdit.Name = "LabelEdit";
			this.labelEdit.Margins = new Margins(10, 10, 10, 10);
			this.labelEdit.Dock = DockStyle.Bottom;
			this.labelEdit.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.labelEdit.TabIndex = tabIndex++;
			this.labelEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);

			this.list = new MyWidgets.StringList(left);
			this.list.Margins = new Margins(10, 10, 10, 0);
			this.list.Dock = DockStyle.Fill;
			this.list.TabIndex = tabIndex++;
			this.list.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			StaticText s = new StaticText(this);
			s.Text = "<b>TODO:</b> <i>Editeur d'interfaces...</i>";
			s.Margins = new Margins(20, 0, 0, 0);
			s.Dock = DockStyle.Fill;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.labelEdit.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			}

			base.Dispose(disposing);
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


		public override void UpdateCommands()
		{
			//	Met à jour les commandes en fonction de la ressource sélectionnée.
			this.GetCommandState("Save").Enable = this.module.Modifier.IsDirty;
			this.GetCommandState("SaveAs").Enable = true;

			this.GetCommandState("NewCulture").Enable = false;
			this.GetCommandState("DeleteCulture").Enable = false;

			this.GetCommandState("Filter").Enable = false;
			this.GetCommandState("Search").Enable = false;

			this.GetCommandState("SearchPrev").Enable = false;
			this.GetCommandState("SearchNext").Enable = false;

			this.GetCommandState("AccessFirst").Enable = false;
			this.GetCommandState("AccessPrev").Enable = false;
			this.GetCommandState("AccessLast").Enable = false;
			this.GetCommandState("AccessNext").Enable = false;

			this.GetCommandState("ModificationPrev").Enable = false;
			this.GetCommandState("ModificationNext").Enable = false;
			this.GetCommandState("ModificationAll").Enable = false;
			this.GetCommandState("ModificationClear").Enable = false;

			this.GetCommandState("Delete").Enable = false;
			this.GetCommandState("Create").Enable = false;
			this.GetCommandState("Duplicate").Enable = false;

			this.GetCommandState("Up").Enable = false;
			this.GetCommandState("Down").Enable = false;

			this.GetCommandState("FontBold").Enable = false;
			this.GetCommandState("FontItalic").Enable = false;
			this.GetCommandState("FontUnderlined").Enable = false;
			this.GetCommandState("Glyphs").Enable = false;
		}


		void HandleTextChanged(object sender)
		{
			//	Un texte éditable a changé.
			if ( this.ignoreChange )  return;

			AbstractTextField edit = sender as AbstractTextField;
		}


		protected MyWidgets.StringList		list;
		protected TextFieldEx				labelEdit;
	}
}
