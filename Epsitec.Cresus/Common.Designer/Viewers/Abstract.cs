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
	public abstract class Abstract : Widget
	{
		public Abstract(Module module)
		{
			this.module = module;

			this.labelsIndex = new List<string>();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		public static Abstract Create(string type, Module module)
		{
			//	Cr�e un Viewer d'un type donn�.
			if (type == "Strings")  return new Strings(module);
			if (type == "Panels" )  return new Panels(module);
			if (type == "Scripts")  return new Scripts(module);
			return null;
		}


		public virtual AbstractTextField CurrentTextField
		{
			//	Retourne le texte �ditable en cours d'�dition.
			get
			{
				return this.currentTextField;
			}
		}

		public virtual void DoSearch(string search, Searcher.SearchingMode mode)
		{
			//	Effectue une recherche.
		}

		public virtual void DoCount(string search, Searcher.SearchingMode mode)
		{
			//	Effectue un comptage.
		}

		public virtual void DoReplace(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un remplacement.
		}

		public virtual void DoReplaceAll(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un 'remplacer tout'.
		}

		public void DoFilter(string filter, Searcher.SearchingMode mode)
		{
			//	Change le filtre des ressources visibles.
			string label = "";
			int sel = this.array.SelectedRow;
			if (sel != -1 && sel < this.labelsIndex.Count)
			{
				label = this.labelsIndex[sel];
			}

			this.UpdateLabelsIndex(filter, mode);
			this.UpdateArray();

			sel = this.labelsIndex.IndexOf(label);
			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();
			this.UpdateCommands();
		}

		public void DoAccess(string name)
		{
			//	Change la ressource visible.
			int sel = this.array.SelectedRow;

			if ( name == "AccessFirst" )  sel = 0;
			if ( name == "AccessPrev"  )  sel --;
			if ( name == "AccessNext"  )  sel ++;
			if ( name == "AccessLast"  )  sel = 1000000;

			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();
			this.UpdateCommands();
		}

		public virtual void DoModification(string name)
		{
			//	Change la ressource modifi�e visible.
		}

		public virtual void DoDelete()
		{
			//	Supprime la ressource s�lectionn�e.
		}

		public virtual void DoDuplicate(bool duplicate)
		{
			//	Duplique la ressource s�lectionn�e.
		}

		public virtual void DoMove(int direction)
		{
			//	D�place la ressource s�lectionn�e.
		}

		public virtual void DoNewCulture()
		{
			//	Cr�e une nouvelle culture.
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

		public virtual void DoTool(string name)
		{
			//	Choix de l'outil.
			this.tool = name;
			this.UpdateCommands();
		}

		public virtual void DoCommand(string name)
		{
			//	Ex�cute une commande.
			this.UpdateCommands();
		}


		public string InfoAccessText
		{
			//	Donne le texte d'information sur l'acc�s en cours.
			get
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				int sel = this.array.SelectedRow;
				if (sel == -1)
				{
					builder.Append("-");
				}
				else
				{
					builder.Append((sel+1).ToString());
				}

				builder.Append("/");
				builder.Append(this.labelsIndex.Count.ToString());

				if (this.labelsIndex.Count < this.InfoAccessTotalCount)
				{
					builder.Append(" (");
					builder.Append(this.InfoAccessTotalCount.ToString());
					builder.Append(")");
				}

				return builder.ToString();
			}
		}

		protected virtual int InfoAccessTotalCount
		{
			get
			{
				return this.labelsIndex.Count;
			}
		}


		protected virtual void UpdateLabelsIndex(string filter, Searcher.SearchingMode mode)
		{
		}

		protected virtual void UpdateArray()
		{
		}

		public virtual void Update()
		{
			//	Met � jour le contenu du Viewer.
		}

		public virtual void UpdateCommands()
		{
			//	Met � jour les commandes en fonction de la ressource s�lectionn�e.
			int sel = this.array.SelectedRow;
			int count = this.labelsIndex.Count;
			bool build = (this.module.Mode == DesignerMode.Build);

			////////////////////////////////////////////////////////////////////
			//	TODO:
			//	
			//	Dans tout le code de "designer", remplacer les :
			//
			//			GetCommandState("xxx").Enable = ...
			//
			//	par des :
			//	
			//			CommandContext.SetLocalEnable("xxx", ...);
			////////////////////////////////////////////////////////////////////

			this.CommandContext.SetLocalEnable ("Save", this.module.Modifier.IsDirty);
			this.CommandContext.SetLocalEnable ("SaveAs", true);

			this.GetCommandState("Filter").Enable = true;

			this.GetCommandState("AccessFirst").Enable = (sel != -1 && sel > 0);
			this.GetCommandState("AccessPrev").Enable = (sel != -1 && sel > 0);
			this.GetCommandState("AccessLast").Enable = (sel != -1 && sel < count-1);
			this.GetCommandState("AccessNext").Enable = (sel != -1 && sel < count-1);

			this.GetCommandState("Delete").Enable = (sel != -1 && count > 1 && build);
			this.GetCommandState("Create").Enable = (sel != -1 && build);
			this.GetCommandState("Duplicate").Enable = (sel != -1 && build);

			this.GetCommandState("Up").Enable = (sel != -1 && sel > 0 && build);
			this.GetCommandState("Down").Enable = (sel != -1 && sel < count-1 && build);

			this.UpdateCommandTool("ToolSelect");
			this.UpdateCommandTool("ToolGlobal");
			this.UpdateCommandTool("ToolEdit");
			this.UpdateCommandTool("ToolZoom");
			this.UpdateCommandTool("ToolHand");
			this.UpdateCommandTool("ObjectLine");
			this.UpdateCommandTool("ObjectButton");
			this.UpdateCommandTool("ObjectText");
		}

		protected void UpdateCommandTool(string name)
		{
			this.GetCommandState(name).ActiveState = (this.tool == name) ? ActiveState.Yes : ActiveState.No;
		}

		protected CommandContext CommandContext
		{
			get
			{
				return this.module.MainWindow.CommandContext;
			}
		}

		protected Command GetCommandState(string command)
		{
			return this.module.MainWindow.GetCommandState(command);
		}


		protected void HandleEditKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appel� lorsqu'une ligne �ditable voit son focus changer.
			bool focused = (bool) e.NewValue;

			if (focused)
			{
				this.currentTextField = sender as AbstractTextField;
			}
		}


		protected Module					module;
		protected List<string>				labelsIndex;
		protected bool						ignoreChange = false;
		protected MyWidgets.StringArray		array;
		protected AbstractTextField			currentTextField;
		protected string					tool = "ToolSelect";
	}
}
