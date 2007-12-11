//	Copyright � 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
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
		public Dialog(Support.ResourceManager resourceManager)
			: this (resourceManager, "AnonymousDialog")
		{
		}
		
		public Dialog(Support.ResourceManager resourceManager, string name)
		{
			this.name            = name;
			this.dispatcher      = new CommandDispatcher (this.name, CommandDispatcherLevel.Secondary);
			this.resourceManager = resourceManager;
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
			if (this.window != null)
			{
				throw new System.InvalidOperationException ("Dialog may not be loaded twice.");
			}
			
			this.name = name;
			
			Support.ResourceBundle bundle = this.resourceManager.GetBundle (this.name);
		
			//	TODO: handle whatever needs to be done here
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
		
		protected virtual void AttachData()
		{
			//	Attache la structure de donn�es aux divers "partenaires" qui g�rent
			//	le dialogue :
			
			if (this.window != null)
			{
				this.AttachWindow ();
			}
		}
		
		
		protected virtual void OnDataBindingChanged()
		{
		}
		
		protected virtual void OnScriptBindingChanged()
		{
		}
		
		
		
		
		
		protected enum InternalMode
		{
			None,
			Dialog
		}
		
		
		protected Support.ResourceManager		resourceManager;
		protected InternalMode					mode;
		protected Types.IDataGraph				data;
		protected Types.IDataFolder				initial_data_folder;
		protected CommandDispatcher				dispatcher;
		protected string						name;
	}
}
