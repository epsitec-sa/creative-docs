//	Copyright © 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
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

			UI.Panel panel = this.CreateUserInterface (bundle);

			if (panel == null)
			{
				return null;
			}
			
			Window window = new Window ();

			window.Root.Children.Add (panel);

			panel.Dock = DockStyle.Fill;

			return window;
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

			FormEngine.FormDescription formDescription = new FormEngine.FormDescription ();
			FormEngine.Engine formEngine = new FormEngine.Engine (this.resourceManager);
			
			formDescription.Deserialize (xmlSource);
			return formEngine.CreateForm (formDescription);
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
	}
}
