//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

using System.Globalization;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe BundleEditController gère l'édition d'un bundle quelconque, en
	/// redirigeant les commandes sur les contrôleurs appropriés.
	/// </summary>
	public class BundleEditController : AbstractController
	{
		public BundleEditController(Application application) : base (application)
		{
			this.bundles     = new System.Collections.ArrayList ();
		}
		
		
		[Command ("OpenBundle")]  void CommandOpenBundle(CommandDispatcher d, CommandEventArgs e)
		{
			if (e.CommandArgs.Length == 0)
			{
				Dialogs.OpenExistingBundle dialog = new Dialogs.OpenExistingBundle ("OpenBundle (\"{0}\")", this.dispatcher);
				dialog.Owner = this.application.MainWindow;
				dialog.UpdateListContents ();
				dialog.Show ();
			}
			else if (e.CommandArgs.Length == 1)
			{
				string         full_id = e.CommandArgs[0];
				string         prefix  = Resources.ExtractPrefix (full_id);
				string         name    = Resources.ExtractName (full_id);
				ResourceBundle bundle  = Resources.GetBundle (e.CommandArgs[0], ResourceLevel.Default);
				
				switch (bundle.Type)
				{
					case "String":
						this.application.StringEditController.AttachExistingBundle (prefix, name);
						break;
					
					default:
						//	TODO: signale le fait que l'on ne sait pas que faire du bundle...
						break;
				}
			}
			else
			{
				this.ThrowInvalidOperationException (e, 1);
			}
		}
		
		
		protected System.Collections.ArrayList	bundles;
	}
}
