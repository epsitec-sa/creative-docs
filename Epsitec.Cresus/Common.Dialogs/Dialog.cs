//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe Dialog permet d'ouvrir et de gérer un dialogue à partir d'une
	/// ressource et d'une source de données.
	/// </summary>
	public class Dialog : Support.ICommandDispatcherHost
	{
		public Dialog()
		{
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
					
					if (this.designer != null)
					{
						this.designer.DialogData = this.data;
					}
					if (this.window != null)
					{
						this.UpdateWindowBindings ();
					}
					if (this.script_wrapper != null)
					{
						this.script_wrapper.Data = this.data;
					}
				}
			}
		}
		
		public Widgets.Window					Window
		{
			get
			{
				return this.window;
			}
		}
		
		public Support.CommandDispatcher		CommandDispatcher
		{
			get
			{
				return this.dispatcher;
			}
			set
			{
				if (this.dispatcher != value)
				{
					this.dispatcher = value;
					
					if (this.designer != null)
					{
						this.designer.DialogCommands = this.dispatcher;
					}
					if (this.window != null)
					{
						this.window.CommandDispatcher = this.dispatcher;
					}
				}
			}
		}
		
		public Script.ScriptWrapper				ScriptWrapper
		{
			get
			{
				return this.script_wrapper;
			}
			set
			{
				if (this.script_wrapper != value)
				{
					this.script_wrapper = value;
					
					if (this.designer != null)
					{
						this.designer.DialogScript = this.script_wrapper;
					}
					if (this.data != null)
					{
						this.script_wrapper.Data = this.data;
					}
				}
			}
		}
		
		public bool								IsReady
		{
			get
			{
				return (this.mode == InternalMode.Dialog);
			}
		}
		
		public bool								IsLoaded
		{
			get
			{
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
		
		
		public void Load(string full_name)
		{
			if ((this.designer != null) ||
				(this.window != null))
			{
				throw new System.InvalidOperationException ("Dialog may not be loaded twice.");
			}
			
			this.full_name = full_name;
			
			Support.ResourceBundle bundle = Support.Resources.GetBundle (full_name);
			
			if (bundle == null)
			{
				this.designer = Dialog.CreateDesigner ();
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
				
				this.CreateDesignerActivatorWidget ();
				this.UpdateWindowBindings ();
			}
		}
		
		
		public void ShowWindow()
		{
			if (this.IsReady)
			{
				this.window.Show ();
			}
		}
		
		public void ShowDialog()
		{
			if (this.IsReady)
			{
				this.window.ShowDialog ();
			}
		}
		
		public void Close()
		{
			if (this.IsReady)
			{
				this.window.Hide ();
			}
		}
		
		
		
		protected virtual void UpdateWindowBindings()
		{
			if (this.window != null)
			{
				if (this.data != null)
				{
					UI.Engine.BindWidgets (this.data, this.window.Root);
				}
			}
		}
		
		protected virtual void SwitchToDesigner()
		{
			if (this.mode == InternalMode.Design)
			{
				return;
			}
			
			if (this.full_name == null)
			{
				throw new System.InvalidOperationException ("Cannot switch to designer.");
			}
			
			this.designer = Dialog.CreateDesigner ();
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
			this.designer.ResourceName   = this.full_name;
			this.designer.DialogCommands = this.dispatcher;
			this.designer.DialogScript   = this.script_wrapper;
			
			this.designer.Disposed += new Support.EventHandler (this.HandleDesignerDisposed);
		}
		
		protected virtual void DetachDesigner()
		{
			this.designer.Disposed -= new Support.EventHandler (this.HandleDesignerDisposed);
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
		
		
		public static IDialogDesigner CreateDesigner()
		{
			IDialogDesignerFactory factory = Dialog.DesignerFactory;
			
			if (factory != null)
			{
				return factory.CreateDialogDesigner ();
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
		protected Widgets.Window				window;
		protected Support.CommandDispatcher		dispatcher;
		protected Script.ScriptWrapper			script_wrapper;
		protected IDialogDesigner				designer;
		protected string						full_name;
		protected Widgets.Widget				designer_activator_widget;
	}
}
