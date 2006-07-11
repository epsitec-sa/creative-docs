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
	public abstract class Abstract : Widget
	{
		public Abstract(Module module, PanelsContext context)
		{
			this.module = module;
			this.context = context;

			this.druidsIndex = new List<Druid>();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		public static Abstract Create(string type, Module module, PanelsContext context)
		{
			//	Crée un Viewer d'un type donné.
			if (type == "Strings")  return new Strings(module, context);
			if (type == "Panels" )  return new Panels(module, context);
			if (type == "Scripts")  return new Scripts(module, context);
			if (type == "Captions")  return new Captions(module, context);
			if (type == "Commands")  return new Commands(module, context);
			if (type == "Types")  return new Types(module, context);
			return null;
		}


		public virtual Module.BundleType BundleType
		{
			get
			{
				return Module.BundleType.Unknow;
			}
		}

		public virtual AbstractTextField CurrentTextField
		{
			//	Retourne le texte éditable en cours d'édition.
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
			Druid druid = Druid.Empty;
			int sel = this.array.SelectedRow;
			if (sel != -1 && sel < this.druidsIndex.Count)
			{
				druid = this.druidsIndex[sel];
			}

			this.UpdateDruidsIndex(filter, mode);
			this.UpdateArray();

			sel = this.druidsIndex.IndexOf(druid);
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
			//	Change la ressource modifiée visible.
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

		public virtual void DoTool(string name)
		{
			//	Choix de l'outil.
			this.context.Tool = name;
			this.UpdateCommands();
		}

		public virtual void DoCommand(string name)
		{
			//	Exécute une commande.
			this.UpdateCommands();
		}


		public virtual string InfoViewerText
		{
			//	Donne le texte d'information sur le visualisateur en cours.
			get
			{
				return "";
			}
		}

		public virtual string InfoAccessText
		{
			//	Donne le texte d'information sur l'accès en cours.
			get
			{
				return "";
			}
		}

		protected virtual int InfoAccessTotalCount
		{
			get
			{
				return this.druidsIndex.Count;
			}
		}


		protected virtual void UpdateDruidsIndex(string filter, Searcher.SearchingMode mode)
		{
		}

		protected virtual void UpdateArray()
		{
		}

		public virtual void Update()
		{
			//	Met à jour le contenu du Viewer.
		}

		public virtual void UpdateCommands()
		{
			//	Met à jour les commandes en fonction de la ressource sélectionnée.
			int sel = this.array.SelectedRow;
			int count = this.druidsIndex.Count;
			bool build = (this.module.Mode == DesignerMode.Build);

			this.GetCommandState ("Save").Enable = this.module.Modifier.IsDirty;
			this.GetCommandState ("SaveAs").Enable = true;
			
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
			this.UpdateCommandTool("ToolGrid");
			this.UpdateCommandTool("ToolEdit");
			this.UpdateCommandTool("ToolZoom");
			this.UpdateCommandTool("ToolHand");
			this.UpdateCommandTool("ObjectVLine");
			this.UpdateCommandTool("ObjectHLine");
			this.UpdateCommandTool("ObjectButton");
			this.UpdateCommandTool("ObjectText");
			this.UpdateCommandTool("ObjectStatic");
			this.UpdateCommandTool("ObjectGroup");
			this.UpdateCommandTool("ObjectGroupBox");
		}

		protected void UpdateCommandTool(string name)
		{
			this.GetCommandState(name).ActiveState = (this.context.Tool == name) ? ActiveState.Yes : ActiveState.No;
		}

		protected CommandState GetCommandState(string command)
		{
			return this.module.MainWindow.GetCommandState(command);
		}


		protected void SetTextField(AbstractTextField field, string text)
		{
			if (text == null)
			{
				field.Text = "";
			}
			else
			{
				field.Text = text;
			}
		}

		protected bool IsExistingName(string baseName)
		{
			//	Indique si un nom existe.
			ResourceBundleCollection bundles = this.module.GetBundles(this.BundleType);
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];

			ResourceBundle.Field field = defaultBundle[baseName];
			return (field != null && field.Name != null);
		}

		protected string GetDuplicateName(string baseName)
		{
			//	Retourne le nom à utiliser lorsqu'un nom existant est dupliqué.
			ResourceBundleCollection bundles = this.module.GetBundles(this.BundleType);

			int numberLength = 0;
			while (baseName.Length > 0)
			{
				char last = baseName[baseName.Length-1-numberLength];
				if (last >= '0' && last <= '9')
				{
					numberLength++;
				}
				else
				{
					break;
				}
			}

			int nextNumber = 2;
			if (numberLength > 0)
			{
				nextNumber = int.Parse(baseName.Substring(baseName.Length-numberLength))+1;
				baseName = baseName.Substring(0, baseName.Length-numberLength);
			}

			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			string newName = baseName;
			for (int i=nextNumber; i<nextNumber+100; i++)
			{
				newName = string.Concat(baseName, i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				ResourceBundle.Field field = defaultBundle[newName];
				if (field.IsEmpty)
					break;
			}

			return newName;
		}

		
		protected void HandleEditKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsqu'une ligne éditable voit son focus changer.
			bool focused = (bool) e.NewValue;

			if (focused)
			{
				this.currentTextField = sender as AbstractTextField;
			}
		}


		#region CultureInfo
		public class CultureInfo
		{
			public CultureInfo(System.Globalization.CultureInfo culture)
			{
				this.name = Misc.CultureName(culture);
				this.tooltip = Misc.CultureLongName(culture);
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public string Tooltip
			{
				get
				{
					return this.tooltip;
				}
			}

			protected string			name;
			protected string			tooltip;
		}
		#endregion


		protected Module					module;
		protected PanelsContext				context;
		protected List<Druid>				druidsIndex;
		protected bool						ignoreChange = false;
		protected MyWidgets.StringArray		array;
		protected AbstractTextField			currentTextField;
	}
}
