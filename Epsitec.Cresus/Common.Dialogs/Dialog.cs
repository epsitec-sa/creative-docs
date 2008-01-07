//	Copyright © 2004-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

		public DialogData DialogData
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
					this.dialogData = value;

					if (this.panel != null)
					{
						this.InitializeDataBinding ();
					}
				}
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
			this.DialogResult = DialogResult.Cancel;
			this.CloseDialog ();
		}

		protected virtual void HandleAcceptCommand()
		{
			this.DialogData.ApplyChanges ();
			this.DialogResult = DialogResult.Accept;
			this.CloseDialog ();
		}

		protected virtual void HandleApplyCommand()
		{
			this.DialogData.ApplyChanges ();
		}

		private void InitializeDataBinding()
		{
			if (this.panel != null)
			{
				this.DialogData.BindToUserInterface (this.panel);
			}
		}

		private UI.Panel CreateUserInterface(ResourceBundle bundle)
		{
			switch (bundle.Type)
			{
				case Resources.PanelTypeName:
					return this.CreateUserInterfaceFromPanel (bundle);
				
				case Resources.FormTypeName:
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
			FormEngine.Engine formEngine = new FormEngine.Engine (this.resourceManager);
			
			formDescription.Deserialize (xmlSource);

			this.userInterfaceEntityId = formDescription.EntityId;

			return formEngine.CreateForm (formDescription);
		}

		private DialogData CreateDefaultDialogData()
		{
			if (this.userInterfaceEntityId.IsValid)
			{
				EntityContext context = EntityContext.Current;
				AbstractEntity data = context.CreateEntity (this.userInterfaceEntityId);

				return new DialogData (data, DialogDataMode.Isolated);
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
		private UI.Panel						panel;
	}
}
