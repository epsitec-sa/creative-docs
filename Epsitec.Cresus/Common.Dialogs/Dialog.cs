//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe Dialog permet d'ouvrir et de gérer un dialogue à partir d'une
	/// ressource et d'une source de données.
	/// </summary>
	public class Dialog : AbstractDialog, Support.ICommandDispatcherHost
	{
		public Dialog()
		{
			this.name       = "AnonymousDialog";
			this.dispatcher = new Support.CommandDispatcher (this.name, true);
		}
		
		public Dialog(string name)
		{
			this.name       = name;
			this.dispatcher = new Support.CommandDispatcher (this.name);
		}
		
		
		public Types.IDataGraph					Data
		{
			get
			{
				return this.data;
			}
			set
			{
				if (this.data != value)
				{
					if (this.data != null)
					{
						throw new System.InvalidOperationException ("Data may not be set twice.");
					}
					
					this.data = value;
					this.AttachData ();
					this.OnDataBindingChanged ();
				}
			}
		}
		
		public Script.ScriptWrapper				Script
		{
			get
			{
				return this.script;
			}
			set
			{
				if (this.script != value)
				{
					this.script = value;
					this.AttachScript ();
					this.OnScriptBindingChanged ();
				}
			}
		}
		
		
		public Support.CommandDispatcher		CommandDispatcher
		{
			get
			{
				return this.dispatcher;
			}
		}
		
		
		public override bool					IsReady
		{
			get
			{
				//	Un dialogue est considéré comme "prêt" uniquement s'il est actuellement
				//	configuré pour s'afficher comme dialogue (il a été initialisé et il n'est
				//	pas en cours d'édition dans l'éditeur).
				
				return (this.mode == InternalMode.Dialog);
			}
		}
		
		public bool								IsLoaded
		{
			get
			{
				//	Un dialogue est chargé dès qu'il a été complètement initialisé.
				
				return (this.mode != InternalMode.None);
			}
		}
		
		
		#region ICommandDispatcherHost Members
		Support.CommandDispatcher				Support.ICommandDispatcherHost.CommandDispatcher
		{
			get
			{
				return this.CommandDispatcher;
			}
			set
			{
				throw new System.InvalidOperationException ("CommandDispatcher is read-only.");
			}
		}
		#endregion
		
		public static IDialogDesignerFactory	DesignerFactory
		{
			get
			{
				Dialog.LoadDesignerFactory ();
				return Dialog.factory;
			}
		}
		
		
		public void Load()
		{
			this.Load (this.name);
		}
		
		public void Load(string name)
		{
			if ((this.designer != null) ||
				(this.window != null))
			{
				throw new System.InvalidOperationException ("Dialog may not be loaded twice.");
			}
			
			this.name = name;
			
			Support.ResourceBundle bundle = Support.Resources.GetBundle (this.name);
			
			if (bundle == null)
			{
				this.designer = Dialog.CreateDesigner (DesignerType.DialogWindow);
				this.window   = new Widgets.Window ();
				this.mode     = InternalMode.Design;
				
				this.window.Root.Text = "&lt; à créer ... &gt;";
				
				this.AttachDesigner ();
				
				this.designer.StartDesign ();
			}
			else
			{
				Support.ObjectBundler bundler = new Support.ObjectBundler ();
				
				Widgets.Widget root = bundler.CreateFromBundle (bundle) as Widgets.Widget;
				
				System.Diagnostics.Debug.Assert (root != null);
				
				this.window = root.Window;
				this.mode   = InternalMode.Dialog;
				
				this.window.CommandDispatcher = this.CommandDispatcher;
				
				this.CreateDesignerActivatorWidget ();
				this.AttachWindow ();
			}
		}
		
		public void AddRule(Support.IValidator validator, string command_states)
		{
			this.CommandDispatcher.AddValidator (new Support.ValidationRule (validator, command_states));
		}
		
		public void AddController(object controller)
		{
			this.CommandDispatcher.RegisterController (controller);
		}
		
		
		public void StoreInitialData()
		{
			this.initial_data_folder = null;
			
			if ((this.data != null) &&
				(this.data.Root != null))
			{
				//	Conserve une copie des données d'origine en réalisant un "clonage" en
				//	profondeur :
				
				this.initial_data_folder = this.data.Root.Clone () as Types.IDataFolder;
			}
		}
		
		public void RestoreInitialData()
		{
			if ((this.initial_data_folder != null) &&
				(this.data != null) &&
				(this.data.Root != null))
			{
				Types.IDataFolder root = this.data.Root;
				
				int changes = Types.DataGraph.CopyValues (this.initial_data_folder, root);
				
				if (changes > 0)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Restored {0} changes.", changes));
				}
			}
		}
		
		
		protected virtual void AttachWindow()
		{
			if (this.window != null)
			{
				if (this.data != null)
				{
					UI.Engine.BindWidgets (this.data, this.window.Root);
				}
			}
		}
		
		protected virtual void AttachScript()
		{
			if (this.designer != null)
			{
				this.designer.DialogScript = this.script;
			}
			
			if (this.data != null)
			{
				this.script.Data = this.data;
			}
		}
		
		protected virtual void AttachData()
		{
			//	Attache la structure de données aux divers "partenaires" qui gèrent
			//	le dialogue :
			
			if (this.designer != null)
			{
				this.designer.DialogData = this.data;
			}
			
			if (this.window != null)
			{
				this.AttachWindow ();
			}
			
			if (this.script != null)
			{
				this.script.Data = this.data;
			}
		}
		
		
		protected virtual void SwitchToDesigner()
		{
			if (this.mode == InternalMode.Design)
			{
				return;
			}
			
			if (this.name == null)
			{
				throw new System.InvalidOperationException ("Cannot switch to designer.");
			}
			
			this.designer = Dialog.CreateDesigner (DesignerType.DialogWindow);
			this.mode     = InternalMode.Design;
			
			if (this.designer_activator_widget != null)
			{
				this.designer_activator_widget.SetVisible (false);
			}
			
			this.AttachDesigner ();
			
			this.designer.StartDesign ();
		}
		
		protected virtual void SwitchBackToDialog()
		{
			if (this.mode != InternalMode.Design)
			{
				return;
			}
			
			this.DetachDesigner ();
			
			this.designer = null;
			this.mode     = InternalMode.Dialog;
			
			if (this.designer_activator_widget != null)
			{
				this.designer_activator_widget.SetVisible (true);
			}
		}
		
		protected virtual void CreateDesignerActivatorWidget()
		{
			Widgets.IconButton button = new Widgets.IconButton (this.window.Root);
			
			button.IconName = "manifest:Epsitec.Common.Dialogs.Images.StartDesigner.icon";
			button.Clicked += new Widgets.MessageEventHandler (this.HandleDesignerActivatorClicked);
			button.Anchor   = Widgets.AnchorStyles.BottomLeft;
			button.AnchorMargins = new Drawing.Margins (0, 0, 0, 0);
			
			this.designer_activator_widget = button;
		}
		
		
		protected virtual void AttachDesigner()
		{
			this.designer.DialogData     = this.data;
			this.designer.DialogWindow   = this.window;
			this.designer.ResourceName   = this.name;
			this.designer.DialogCommands = this.dispatcher;
			this.designer.DialogScript   = this.script;
			
			this.designer.Disposed += new Support.EventHandler (this.HandleDesignerDisposed);
		}
		
		protected virtual void DetachDesigner()
		{
			this.designer.Disposed -= new Support.EventHandler (this.HandleDesignerDisposed);
		}
		
		
		
		protected virtual void OnDataBindingChanged()
		{
		}
		
		protected virtual void OnScriptBindingChanged()
		{
		}
		
		
		public static bool LoadDesignerFactory()
		{
			if (Dialog.factory != null)
			{
				return true;
			}
			
			System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadWithPartialName ("Common.Designer");
			
			if (assembly != null)
			{
				System.Type type = assembly.GetType ("Epsitec.Common.Designer.DialogDesignerFactory");
				object      obj  = type.InvokeMember ("GetFactory", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, null, null);
				
				Dialog.factory = obj as IDialogDesignerFactory;
				
				if (Dialog.factory != null)
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		public static IDialogDesigner CreateDesigner(DesignerType type)
		{
			IDialogDesignerFactory factory = Dialog.DesignerFactory;
			
			if (factory != null)
			{
				return factory.CreateDialogDesigner (type);
			}
			
			return null;
		}
		
		
		private void HandleDesignerActivatorClicked(object sender, Widgets.MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.designer_activator_widget == sender);
			
			this.SwitchToDesigner ();
		}
		
		private void HandleDesignerDisposed(object sender)
		{
			System.Diagnostics.Debug.Assert (this.designer == sender);
			this.SwitchBackToDialog ();
		}
		
		
		protected enum InternalMode
		{
			None,
			Design,
			Dialog
		}
		
		
		private static IDialogDesignerFactory	factory;
		
		protected InternalMode					mode;
		protected Types.IDataGraph				data;
		protected Types.IDataFolder				initial_data_folder;
		protected Support.CommandDispatcher		dispatcher;
		protected Script.ScriptWrapper			script;
		protected IDialogDesigner				designer;
		protected string						name;
		protected Widgets.Widget				designer_activator_widget;
	}
}
