using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant d'éditer les informations d'un module.
	/// </summary>
	public class ModuleInfo : Abstract
	{
		public ModuleInfo(DesignerApplication designerApplication)
			: base (designerApplication)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.PreventAutoClose = true;
				this.WindowInit ("ModuleInfo", 540, 340, true);
				this.window.Text = "Informations du module";  // Res.Strings.Dialog.ModuleInfo.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob (this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins (0, -8, 0, -8);
				ToolTip.Default.SetToolTip (resize, Res.Strings.Dialog.Tooltip.Resize);

				this.CreateUI (this.window.Root);

				//	Boutons de fermeture.
				Widget footer = new Widget (this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins (0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonCancel = new Button (footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += this.HandleButtonCloseClicked;
				this.buttonCancel.TabIndex = 11;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonOk = new Button (footer);
				this.buttonOk.PreferredWidth = 75;
				this.buttonOk.Text = Res.Strings.Dialog.Button.OK;
				this.buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonOk.Dock = DockStyle.Right;
				this.buttonOk.Margins = new Margins (0, 6, 0, 0);
				this.buttonOk.Clicked += this.HandleButtonOkClicked;
				this.buttonOk.TabIndex = 10;
				this.buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.Update ();

			this.window.Show ();
		}


		public Module Module
		{
			get;
			set;
		}

		public bool IsEditOk
		{
			get
			{
				return this.isEditOk;
			}
		}


		private void CreateUI(Widget parent)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			this.title = new StaticText
			{
				Parent = box,
				ContentAlignment = ContentAlignment.TopLeft,
				PreferredHeight = 34,
				Dock = DockStyle.Top,
			};

			new Separator
			{
				Parent = box,
				PreferredHeight = 1,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 10),
			};

			this.fieldPath       = this.CreateField (box, "Chemin");
			this.fieldName       = this.CreateField (box, "Nom");
			this.fieldId         = this.CreateField (box, "Numéro");

			new Separator
			{
				Parent = box,
				PreferredHeight = 1,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 10, 10),
			};

			this.fieldAssemblies = this.CreateField (box, "Assemblies", true);
			
			this.fieldDefault    = this.CreateField (box, "Namespace par défaut");
			this.fieldEntities   = this.CreateField (box, "Namespace pour les entités");
			this.fieldForms      = this.CreateField (box, "Namespace pour les panneaux");
			this.fieldRes        = this.CreateField (box, "Namespace pour les ressources");

			this.radioString = new RadioButton
			{
				Parent = box,
				Text = "Textes simples (string)",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 10, 0),
			};

			this.radioFormatted = new RadioButton
			{
				Parent = box,
				Text = "Textes riches (FormattedText)",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 0),
			};
		}

		private TextField CreateField(Widget parent, string label, bool bottomSpace = false)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, bottomSpace ? 10 : 1),
				TabIndex = ++this.tabIndex,
			};

			new StaticText
			{
				Parent = box,
				PreferredWidth = 160,
				Text = label,
				Dock = DockStyle.Left,
			};

			var field = new TextField
			{
				Parent = box,
				Dock = DockStyle.Fill,
				TabIndex = ++this.tabIndex,
			};

			return field;
		}


		private void Update()
		{
			this.isEditOk = false;
			this.closed = false;

			string name = Misc.ExtractName (this.Module.ModuleId.Name);
			this.title.Text = string.Concat("<font size=\"200%\"><b>", name, "</b></font>");

			this.fieldPath.Text       = this.Module.ModuleInfo.FullId.Path;
			this.fieldName.Text       = this.Module.ModuleInfo.FullId.Name;
			this.fieldId.Text         = this.Module.ModuleInfo.FullId.Id.ToString ();
			this.fieldAssemblies.Text = this.Module.ModuleInfo.Assemblies;
			this.fieldDefault.Text 	  = this.Module.ModuleInfo.SourceNamespaceDefault;
			this.fieldEntities.Text   = this.Module.ModuleInfo.SourceNamespaceEntities;
			this.fieldForms.Text 	  = this.Module.ModuleInfo.SourceNamespaceForms;
			this.fieldRes.Text 		  = this.Module.ModuleInfo.SourceNamespaceRes;

			this.radioString.ActiveState    = this.Module.ModuleInfo.TextMode == ResourceTextMode.String        ? ActiveState.Yes : ActiveState.No;
			this.radioFormatted.ActiveState = this.Module.ModuleInfo.TextMode == ResourceTextMode.FormattedText ? ActiveState.Yes : ActiveState.No;

			this.fieldPath.IsReadOnly       = true;
			this.fieldName.IsReadOnly       = true;
			this.fieldId.IsReadOnly         = true;
			this.fieldAssemblies.IsReadOnly = this.designerApplication.IsReadonly;
			this.fieldDefault.IsReadOnly    = this.designerApplication.IsReadonly;
			this.fieldEntities.IsReadOnly   = this.designerApplication.IsReadonly;
			this.fieldForms.IsReadOnly      = this.designerApplication.IsReadonly;
			this.fieldRes.IsReadOnly        = this.designerApplication.IsReadonly;

			this.radioString.Enable    = !this.designerApplication.IsReadonly;
			this.radioFormatted.Enable = !this.designerApplication.IsReadonly;

			this.buttonOk.Enable = !this.designerApplication.IsReadonly;
		}

		private void Accept()
		{
			if (!this.designerApplication.IsReadonly)
			{
				this.Module.ModuleInfo.Assemblies              = this.fieldAssemblies.Text;
				this.Module.ModuleInfo.SourceNamespaceDefault  = this.fieldDefault.Text;
				this.Module.ModuleInfo.SourceNamespaceEntities = this.fieldEntities.Text;
				this.Module.ModuleInfo.SourceNamespaceForms    = this.fieldForms.Text;
				this.Module.ModuleInfo.SourceNamespaceRes      = this.fieldRes.Text;

				this.Module.ModuleInfo.TextMode = (this.radioString.ActiveState == ActiveState.Yes) ? ResourceTextMode.String : ResourceTextMode.FormattedText;

				this.Module.AccessEntities.SetLocalDirty ();
			}
		}


		public void Close()
		{
			if (this.closed)
			{
				return;
			}

			if (this.buttonOk != null)  // mode "dialogue" (par opposition au mode "volet") ?
			{
				this.parentWindow.MakeActive ();
				this.window.Hide ();
				this.OnClosed ();
			}

			this.closed = true;
		}


		private void HandleWindowCloseClicked(object sender)
		{
			this.Close ();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.Close ();
		}

		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.Accept ();
			this.Close ();
			this.isEditOk = true;
		}


		private bool							isEditOk;
		private bool							closed;
		private int								tabIndex;

		private StaticText						title;
		private TextField						fieldPath;
		private TextField						fieldName;
		private TextField						fieldId;
		private TextField						fieldAssemblies;
		private TextField						fieldDefault;
		private TextField						fieldEntities;
		private TextField						fieldForms;
		private TextField						fieldRes;
		private RadioButton						radioString;
		private RadioButton						radioFormatted;
		private Button							buttonOk;
		private Button							buttonCancel;
	}
}
