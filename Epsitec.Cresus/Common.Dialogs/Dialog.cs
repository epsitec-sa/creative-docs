//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe Dialog permet d'ouvrir et de gérer un dialogue à partir d'une
	/// ressource et d'une source de données.
	/// </summary>
	public class Dialog
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
					else if (this.window != null)
					{
						this.UpdateWindowBindings ();
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
			
			Support.ResourceBundle bundle = Support.Resources.GetBundle (full_name);
			
			if (bundle == null)
			{
				this.designer = Dialog.CreateDesigner ();
				this.window   = new Widgets.Window ();
				this.mode     = InternalMode.Design;
				
				this.window.Root.Text = "&lt; à créer ... &gt;";
				
				this.designer.DialogData   = this.data;
				this.designer.DialogWindow = this.window;
				this.designer.ResourceName = full_name;
				
				this.designer.StartDesign ();
			}
			else
			{
				Support.ObjectBundler bundler = new Support.ObjectBundler ();
				
				Widgets.Widget root = bundler.CreateFromBundle (bundle) as Widgets.Widget;
				
				System.Diagnostics.Debug.Assert (root != null);
				
				this.window = root.Window;
				this.mode   = InternalMode.Dialog;
				
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
		protected IDialogDesigner				designer;
	}
}
