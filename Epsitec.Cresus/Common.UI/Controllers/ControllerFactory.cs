//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for ControllerFactory.
	/// </summary>
	public class ControllerFactory
	{
		private ControllerFactory()
		{
		}
		
		
		public static IController[] CreateControllers(Adapters.IAdapter adapter)
		{
			//	Trouve tous les contrôleurs susceptibles de se connecter avec
			//	l'adaptateur spécifié.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			object[] attributes = adapter.GetType ().GetCustomAttributes (typeof (Adapters.ControllerAttribute), true);
			
			System.Array.Sort (attributes, Adapters.ControllerAttribute.RankComparer);
			
			foreach (Adapters.ControllerAttribute attribute in attributes)
			{
				System.Type type       = attribute.Type;
				IController controller = System.Activator.CreateInstance (type) as IController;
				
				System.Diagnostics.Debug.Assert (controller != null);
				
				controller.Adapter = adapter;
				
				list.Add (controller);
			}
			
			IController[] controllers = new IController[list.Count];
			list.CopyTo (controllers);
			
			return controllers;
		}
	}
}
