using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	public class PrintModule : AbstractAuthenticatedModule
	{


		public PrintModule(CoreServer coreServer)
			: base (coreServer, "/print")
		{
			Get["/labellayouts"] = p => this.GetLabelLayouts ();
		}


		private Response GetLabelLayouts()
		{
			return EnumModule.GetEnumResponse (typeof (LabelLayout));
		}


	}


}
