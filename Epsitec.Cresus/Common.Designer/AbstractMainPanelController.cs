//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe AbstractMainPanelController sert de base aux contrôleurs qui
	/// occupent la fenêtre principale de l'application.
	/// </summary>
	public abstract class AbstractMainPanelController : AbstractController
	{
		public AbstractMainPanelController(Application application) : base (application)
		{
		}
		
		
		public Widget							MainPanel
		{
			get
			{
				return this.main_panel;
			}
		}
		
		
		protected abstract void CreateMainPanel();
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.main_panel = null;
			}
			
			base.Dispose (disposing);
		}

		
		protected Widget						main_panel;
	}
}
