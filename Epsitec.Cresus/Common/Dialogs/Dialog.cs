//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.ResourceAccessors;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>Dialog</c> class manages a dialog which description is stored
	/// in a resource.
	/// </summary>
	public class Dialog : AbstractDialog
	{
		public Dialog(ResourceManager resourceManager)
			: this (resourceManager, "AnonymousDialog")
		{
		}
		
		public Dialog(ResourceManager resourceManager, string name)
		{
			this.name            = name;
			this.resourceManager = resourceManager;
			this.validationContext = new ValidationContext ();
		}

		public Druid UserInterfaceResourceId
		{
			get
			{
				return this.userInterfaceResourceId;
			}
			set
			{
				if (this.HasWindow)
				{
					throw new System.InvalidOperationException ("UserInterfaceResourceId may not be changed while the dialog has a window");
				}

				this.userInterfaceResourceId = value;
			}
		}

		public DialogData Data
		{
			get
			{
				if (this.dialogData == null)
				{
					this.dialogData = this.CreateDefaultDialogData ();
				}
				
				return this.dialogData;
			}
			set
			{
				if (this.dialogData != value)
				{
					if ((this.dialogData != null) &&
						(this.panel != null))
					{
						this.DisposeDataBinding ();
					}

					this.dialogData = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the search controller for this dialog.
		/// </summary>
		/// <value>The search controller.</value>
		public DialogSearchController SearchController
		{
			get
			{
				return this.dialogSearchController;
			}
			set
			{
				this.dialogSearchController = value;
			}
		}

		public UI.Panel DialogPanel
		{
			get
			{
				return this.panel;
			}
		}
		
		
		public static Dialog Load(ResourceManager resourceManager, Druid resourceId)
		{
			//	TODO: ...
			
			Dialog dialog = new Dialog (resourceManager);

			dialog.UserInterfaceResourceId = resourceId;

			return dialog;
		}
		
		
		protected override Window CreateWindow()
		{
			ResourceBundle bundle = this.LoadBundle ();

			if (bundle == null)
			{
				return null;
			}

			this.panel = this.CreateUserInterface (bundle);

			if (this.panel == null)
			{
				return null;
			}
			
			Window window = new Window ();

			window.Root.Children.Add (this.panel);

			this.panel.Dock = DockStyle.Fill;

			return window;
		}

		protected override void OnDialogOpening()
		{
			this.InitializeDataBinding ();

			base.OnDialogOpening ();
		}

		protected override void OnDialogClosed()
		{
			this.DisposeDataBinding ();

			base.OnDialogClosed ();
		}

		protected override void OnDialogWindowCreated()
		{
			base.OnDialogWindowCreated ();

			this.CommandDispatcher.Register (Res.Commands.Dialog.Generic.Cancel, this.HandleCancelCommand);
			this.CommandDispatcher.Register (Res.Commands.Dialog.Generic.Ok, this.HandleAcceptCommand);
			this.CommandDispatcher.Register (Res.Commands.Dialog.Generic.Apply, this.HandleApplyCommand);
		}

		protected override void SetDefaultFocus()
		{
			if (this.panel != null)
			{
				this.panel.SetFocusOnTabWidget ();
			}
		}

		protected virtual void HandleCancelCommand()
		{
			this.dialogData.RevertChanges ();
			this.Result = DialogResult.Cancel;
			this.CloseDialog ();
		}

		protected virtual void HandleAcceptCommand()
		{
			this.Data.ApplyChanges ();
			this.Result = DialogResult.Accept;
			this.CloseDialog ();
		}

		protected virtual void HandleApplyCommand()
		{
			this.Data.ApplyChanges ();
		}

		private void InitializeDataBinding()
		{
			if (this.dialogData == null)
			{
				this.dialogData = this.CreateDefaultDialogData ();
			}

			if ((this.panel != null) &&
				(this.dialogData != null) &&
				(this.isDataBound == false))
			{
				this.isDataBound = true;

				this.dialogData.BindToUserInterface (this.panel);

				if (this.dialogSearchController != null)
				{
					this.dialogSearchController.DialogData = this.dialogData;
					this.dialogSearchController.DialogWindow = this.DialogWindow;
					this.dialogSearchController.DialogPanel = this.panel;
				}

				ValidationContext.SetContext (this.panel, this.validationContext);
				
				this.validationContext.CommandContext = Widgets.Helpers.VisualTree.GetCommandContext (this.panel);
				this.validationContext.Refresh (this.panel);
			}
		}

		private void DisposeDataBinding()
		{
			if ((this.panel != null) &&
				(this.dialogData != null) &&
				(this.isDataBound))
			{
				this.isDataBound = false;
				
				if (this.dialogSearchController != null)
				{
					this.dialogSearchController.DialogData = null;
					this.dialogSearchController.DialogWindow = null;
				}
				
				this.dialogData.UnbindFromUserInterface (this.panel);

				ValidationContext.SetContext (this.panel, null);
				this.validationContext.CommandContext = null;
			}
		}

		private UI.Panel CreateUserInterface(ResourceBundle bundle)
		{
			switch (bundle.Type)
			{
				case Epsitec.Common.Support.Resources.PanelTypeName:
					return this.CreateUserInterfaceFromPanel (bundle);

				case Epsitec.Common.Support.Resources.FormTypeName:
					return this.CreateUserInterfaceFromForm (bundle);

				default:
					return null;
			}
		}

		private UI.Panel CreateUserInterfaceFromPanel(ResourceBundle bundle)
		{
			return null;
		}

		private UI.Panel CreateUserInterfaceFromForm(ResourceBundle bundle)
		{
			string xmlSource = bundle[FormResourceAccessor.Strings.XmlSource].AsString;
			Drawing.Size size = FormResourceAccessor.GetFormDefaultSize (bundle);

			FormEngine.FormDescription formDescription = new FormEngine.FormDescription ();
			FormEngine.Engine formEngine = new FormEngine.Engine (new FormEngine.DefaultResourceProvider (this.resourceManager));

			if (this.dialogData != null)
			{
				formEngine.Data = this.dialogData.Data;
			}
			
			formDescription.Deserialize (xmlSource);

			this.userInterfaceEntityId = formDescription.EntityId;

			return formEngine.CreateForm (bundle.Id, ref size);
		}

		private DialogData CreateDefaultDialogData()
		{
			if (this.userInterfaceEntityId.IsValid)
			{
				EntityContext context = EntityContext.Current;
				AbstractEntity entity = context.CreateEntity (this.userInterfaceEntityId);
				
				return new DialogData (entity, DialogDataMode.Isolated);
			}
			else
			{
				return null;
			}
		}

		private ResourceBundle LoadBundle()
		{
			if (this.userInterfaceResourceId.IsValid)
			{
				return this.resourceManager.GetBundle (this.userInterfaceResourceId);
			}
			else
			{
				return null;
			}
		}

		
		private readonly ResourceManager		resourceManager;
		private readonly string					name;
		private Druid							userInterfaceResourceId;
		private Druid							userInterfaceEntityId;
		private DialogData						dialogData;
		private DialogSearchController			dialogSearchController;
		private ValidationContext				validationContext;
		private bool							isDataBound;
		private UI.Panel						panel;
	}
}
