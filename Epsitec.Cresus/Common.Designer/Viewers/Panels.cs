using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de repr�senter les ressources d'un module.
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
			this.labelEdit.Margins = new Margins(10, 10, 0, 10);
			this.labelEdit.Dock = DockStyle.Bottom;
			this.labelEdit.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.labelEdit.TabIndex = tabIndex++;
			this.labelEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);

			this.array = new MyWidgets.StringArray(left);
			this.array.Columns = 1;
			this.array.SetColumnsRelativeWidth(0, 1.00);
			this.array.SetDynamicsToolTips(0, true);
			this.array.Margins = new Margins(10, 10, 10, 10);
			this.array.Dock = DockStyle.Fill;
			this.array.CellsQuantityChanged += new EventHandler(this.HandleArrayCellsQuantityChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
			this.array.TabIndex = tabIndex++;
			this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			StaticText s = new StaticText(this);
			s.Text = "<b>TODO:</b> <i>Editeur d'interfaces...</i>";
			s.Margins = new Margins(20, 0, 0, 0);
			s.Dock = DockStyle.Fill;

			this.UpdateLabelsIndex("", Searcher.SearchingMode.None);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.array.CellsQuantityChanged -= new EventHandler(this.HandleArrayCellsQuantityChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);

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
			//	Change la ressource modifi�e visible.
		}

		public override void DoDelete()
		{
			//	Supprime la ressource s�lectionn�e.
		}

		public override void DoDuplicate(bool duplicate)
		{
			//	Duplique la ressource s�lectionn�e.
		}

		public override void DoMove(int direction)
		{
			//	D�place la ressource s�lectionn�e.
		}

		public override void DoNewCulture()
		{
			//	Cr�e une nouvelle culture.
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


		protected void UpdateLabelsIndex(string filter, Searcher.SearchingMode mode)
		{
			//	Construit l'index en fonction des ressources primaires.
			this.resourceNames = this.module.ResourceManager.GetBundleIds("P.*", "Panel", ResourceLevel.Default);

			if (this.resourceNames.Length == 0)
			{
				string prefix = this.module.ResourceManager.ActivePrefix;
				System.Globalization.CultureInfo culture = this.module.BaseCulture;
				ResourceBundle bundle = ResourceBundle.Create(this.module.ResourceManager, prefix, "P.New", ResourceLevel.Default, culture);

				bundle.DefineType("Panel");
				this.module.ResourceManager.SetBundle(bundle, ResourceSetMode.CreateOnly);

				this.resourceNames = this.module.ResourceManager.GetBundleIds("P.*", "Panel", ResourceLevel.Default);
			}

			this.labelsIndex.Clear();

			if ((mode&Searcher.SearchingMode.CaseSensitive) == 0)
			{
				filter = Searcher.RemoveAccent(filter.ToLower());
			}

			Regex regex = null;
			if ((mode&Searcher.SearchingMode.Jocker) != 0)
			{
				regex = RegexFactory.FromSimpleJoker(filter, RegexFactory.Options.None);
			}

			foreach (string name in this.resourceNames)
			{
				if (filter != "")
				{
					if ((mode&Searcher.SearchingMode.Jocker) != 0)
					{
						string text = name;
						if ((mode&Searcher.SearchingMode.CaseSensitive) == 0)
						{
							text = Searcher.RemoveAccent(text.ToLower());
						}
						if (!regex.IsMatch(text))  continue;
					}
					else
					{
						int index = Searcher.IndexOf(name, filter, 0, mode);
						if (index == -1)  continue;
						if ((mode&Searcher.SearchingMode.AtBeginning) != 0 && index != 0)  continue;
					}
				}

				this.labelsIndex.Add(name);
			}

			this.UpdateArray();
		}

		protected void UpdateArray()
		{
			//	Met � jour tout le contenu du tableau.
			this.array.TotalRows = this.labelsIndex.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.labelsIndex.Count)
				{
					this.array.SetLineString(0, first+i, this.labelsIndex[first+i]);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);
				}
			}
		}

		public override void UpdateCommands()
		{
			//	Met � jour les commandes en fonction de la ressource s�lectionn�e.
			base.UpdateCommands();

			int sel = this.SelectedRow;
			int count = this.labelsIndex.Count;
			bool build = (this.module.Mode == DesignerMode.Build);

			this.GetCommandState("NewCulture").Enable = false;
			this.GetCommandState("DeleteCulture").Enable = false;

			this.GetCommandState("Filter").Enable = false;
			this.GetCommandState("Search").Enable = false;

			this.GetCommandState("SearchPrev").Enable = false;
			this.GetCommandState("SearchNext").Enable = false;

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


		void HandleArrayCellsQuantityChanged(object sender)
		{
			//	Le nombre de lignes a chang�.
			this.UpdateArray();
		}

		void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne s�lectionn�e a chang�.
			this.UpdateCommands();
		}

		void HandleTextChanged(object sender)
		{
			//	Un texte �ditable a chang�.
			if ( this.ignoreChange )  return;

			AbstractTextField edit = sender as AbstractTextField;
		}


		protected TextFieldEx				labelEdit;
		protected string[]					resourceNames;
	}
}
