//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Designer.Panels
{
	/// <summary>
	/// La classe Panel est la base de tous les panneaux du designer.
	/// </summary>
	public abstract class AbstractPanel : Common.UI.AbstractPanel
	{
		public AbstractPanel(Application application)
		{
			this.application = application;
			this.application.UserResourceManagerChanged += new Support.EventHandler (this.HandleUserResourceManagerChanged);
		}
		
		
		private void HandleUserResourceManagerChanged(object sender)
		{
			this.UpdateUserResourceManager ();
		}
		
		protected virtual void UpdateUserResourceManager()
		{
		}
		
		
		protected Application					application;
	}
}
