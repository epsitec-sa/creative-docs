using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;

using Nancy;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// This module is used to retrieve some configuration data for the production of pdf files.
	/// </summary>
	public class PrintModule : AbstractAuthenticatedModule
	{


		public PrintModule(CoreServer coreServer)
			: base (coreServer, "/print")
		{
			// Gets the list of label layouts that are allowed to print labels.
			Get["/labellayouts"] = p =>
				this.GetLabelLayouts ();
		}


		private Response GetLabelLayouts()
		{
			return EnumModule.GetEnumResponse (typeof (LabelLayout));
		}


	}


}
