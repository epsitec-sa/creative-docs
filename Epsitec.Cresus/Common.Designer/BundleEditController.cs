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
		public BundleEditController(Application application)
		{
			this.application = application;
			this.bundles     = new System.Collections.ArrayList ();
			this.dispatcher  = this.application.CommandDispatcher;
			
			this.dispatcher.RegisterController (this);
		}
		
		[Command ("OpenBundle")]  void CommandOpenBundle(CommandDispatcher d, CommandEventArgs e)
		{
			if (e.CommandArgs.Length == 0)
			{
				Dialogs.OpenExistingBundle dialog = new Dialogs.OpenExistingBundle ("OpenBundle (\"{0}\", {1})", this.dispatcher);
				dialog.Owner = this.application.MainWindow;
				dialog.Show ();
			}
			else if (e.CommandArgs.Length == 2)
			{
				ResourceLevel  level  = (ResourceLevel) System.Enum.Parse (typeof (ResourceLevel), e.CommandArgs[1]);
				ResourceBundle bundle = Resources.GetBundle (e.CommandArgs[0], level);
				
				switch (bundle.Type)
				{
					case "String":
						this.application.StringEditController.AttachExistingBundle (bundle);
						break;
					
					default:
						//	TODO: signale le fait que l'on ne sait pas que faire du bundle...
						break;
				}
			}
			else
			{
				this.ThrowInvalidOperationException (e, 2);
			}
		}
		
		
		protected Application					application;
		protected System.Collections.ArrayList	bundles;
		protected Support.CommandDispatcher		dispatcher;
	}
}
