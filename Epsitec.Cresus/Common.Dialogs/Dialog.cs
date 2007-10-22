//	Copyright � 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe Dialog permet d'ouvrir et de g�rer un dialogue � partir d'une
	/// ressource et d'une source de donn�es.
	/// </summary>
	public class Dialog : AbstractDialog
	{
		public Dialog(Support.ResourceManager resource_manager)
		{
			this.name             = "AnonymousDialog";
			this.dispatcher       = new CommandDispatcher (this.name, CommandDispatcherLevel.Secondary);
			this.resource_manager = resource_manager;
		}
		
		public Dialog(Support.ResourceManager resource_manager, string name)
		{
			this.name             = name;
			this.dispatcher       = new CommandDispatcher (this.name, CommandDispatcherLevel.Secondary);
			this.resource_manager = resource_manager;
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
		
# if false
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
#endif
		
		
		public CommandDispatcher				CommandDispatcher
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
				//	Un dialogue est consid�r� comme "pr�t" uniquement s'il est actuellement
				//	configur� pour s'afficher comme dialogue (il a �t� initialis� et il n'est
				//	pas en cours d'�dition dans l'�diteur).
				
				return (this.mode == InternalMode.Dialog);
			}
		}
		
		public bool								IsLoaded
		{
			get
			{
				//	Un dialogue est charg� d�s qu'il a �t� compl�tement initialis�.
				
				return (this.mode != InternalMode.None);
			}
		}
		
		
		public static IDialogDesignerFactory	DesignerFactory
		{
			get
			{
				Dialog.LoadDesignerFactory ();
				return Dialog.factory;
			}
		}

		public static Command					ValidateDialogCommand
		{
			get
			{
				return Command.Get ("ValidateDialog");
			}
		}

		public static Command					ValidateDialogYesCommand
		{
			get
			{
				return Command.Get ("ValidateDialogYes");
			}
		}

		public static Command					ValidateDialogNoCommand
		{
			get
			{
				return Command.Get ("ValidateDialogNo");
			}
		}

		public static Command					QuitDialogCommand
		{
			get
			{
				return Command.Get ("QuitDialog");
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
			
			Support.ResourceBundle bundle = this.resource_manager.GetBundle (this.name);
			
			if (bundle == null)
			{
				this.designer = Dialog.CreateDesigner ();
				this.window   = new Widgets.Window ();
				this.mode     = InternalMode.Design;
				
				this.window.Root.Text = "&lt; � cr�er ... &gt;";
//#				this.window.Root.ResourceManager = this.resource_manager;
				
				this.AttachDesigner ();
				
				this.designer.StartDesign ();
			}
			else
			{
#if false
				Support.ObjectBundler bundler = new Support.ObjectBundler (this.resource_manager);
				
				Widgets.Widget root = bundler.CreateFromBundle (bundle) as Widgets.Widget;
				
				System.Diagnostics.Debug.Assert (root != null);
				
				this.window = root.Window;
				this.mode   = InternalMode.Dialog;
				
				this.window.AttachCommandDispatcher (this.CommandDispatcher);
				
				this.CreateDesignerActivatorWidget ();
				this.AttachWindow ();
#endif
			}
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
				//	Conserve une copie des donn�es d'origine en r�alisant un "clonage" en
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
//-					UI.Engine.BindWidgets (this.data, this.window.Root);
				}
			}
		}
		
		protected virtual void AttachScript()
		{
			if (this.designer != null)
			{
#if false
				this.designer.DialogScript = this.script;
#endif
			}
			
			if (this.data != null)
			{
#if false
				this.script.Data = this.data;
#endif
			}
		}
		
		protected virtual void AttachData()
		{
			//	Attache la structure de donn�es aux divers "partenaires" qui g�rent
			//	le dialogue :
			
			if (this.designer != null)
			{
				this.designer.DialogData = this.data;
			}
			
			if (this.window != null)
			{
				this.AttachWindow ();
			}
			
#if false
			if (this.script != null)
			{
				this.script.Data = this.data;
			}
#endif
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
			
			this.designer = Dialog.CreateDesigner ();
			this.mode     = InternalMode.Design;
			
			if (this.designer_activator_widget != null)
			{
				this.designer_activator_widget.Visibility = false;
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
				this.designer_activator_widget.Visibility = true;
			}
		}
		
		protected virtual void CreateDesignerActivatorWidget()
		{
			Widgets.IconButton button = new Widgets.IconButton (this.window.Root);
			
			button.IconName = "manifest:Epsitec.Common.Dialogs.Images.StartDesigner.icon";
			button.Clicked += new Widgets.MessageEventHandler (this.HandleDesignerActivatorClicked);
			button.Anchor   = Widgets.AnchorStyles.BottomLeft;
			button.Margins = new Drawing.Margins (0, 0, 0, 0);
			
			this.designer_activator_widget = button;
		}
		
		
		protected virtual void AttachDesigner()
		{
			this.designer.ResourceManager = this.resource_manager;
			this.designer.ResourceName    = this.name;
			this.designer.DialogData      = this.data;
			this.designer.DialogWindow    = this.window;
			this.designer.DialogCommands  = this.dispatcher;
#if false
			this.designer.DialogScript    = this.script;
#endif
			
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
			
			System.Reflection.Assembly assembly = Support.AssemblyLoader.Load ("Common.Designer");
			
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
		
		
		public static IDialogDesigner CreateDesigner()
		{
			IDialogDesignerFactory factory = Dialog.DesignerFactory;
			
			if (factory != null)
			{
				IDialogDesigner designer = factory.CreateDialogDesigner ();
				
				designer.IsEditOnlyInterface = true;
				
				return designer;
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
		
		protected Support.ResourceManager		resource_manager;
		protected InternalMode					mode;
		protected Types.IDataGraph				data;
		protected Types.IDataFolder				initial_data_folder;
		protected CommandDispatcher				dispatcher;
#if false
		protected Script.ScriptWrapper			script;
#endif
		protected IDialogDesigner				designer;
		protected string						name;
		protected Widgets.Widget				designer_activator_widget;
	}
}
