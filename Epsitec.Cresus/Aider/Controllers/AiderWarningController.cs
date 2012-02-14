//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers
{
	public class AiderWarningController : CoreDataComponent
	{
		public AiderWarningController(CoreData data)
			: base (data)
		{
		}

		
		public static AiderWarningController	Current
		{
			get
			{
				var coreData  = CoreApp.FindCurrentAppSessionComponent<CoreData> ();
				var component = coreData.GetComponent<AiderWarningController> ();

				return component;
			}
		}


		#region Factory Class

		private sealed class Factory : ICoreDataComponentFactory
		{
			#region ICoreDataComponentFactory Members

			public bool CanCreate(CoreData data)
			{
				return true;
			}

			public CoreDataComponent Create(CoreData data)
			{
				return new AiderWarningController (data);
			}

			public System.Type GetComponentType()
			{
				return typeof (AiderWarningController);
			}

			#endregion
		}

		#endregion
	}
}
