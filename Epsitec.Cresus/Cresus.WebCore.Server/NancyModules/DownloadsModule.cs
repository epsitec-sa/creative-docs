//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Favorites;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
using Nancy;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{
	/// <summary>
	/// 
	/// </summary>
	public class DownloadsModule : AbstractAuthenticatedModule
	{
		public DownloadsModule(CoreServer coreServer)
			: base (coreServer, "/downloads")
		{
			Get["/get/{filename}"] = p =>
				this.Execute (wa => this.DownloadFile (wa, p));
		}

		private Response DownloadFile(WorkerApp app, dynamic parameters)
		{
			var filePath = CoreContext.GetFileDepotPath ("downloads",parameters.filename);
			var stream = System.IO.File.OpenRead (filePath);
			return CoreResponse.CreateStreamResponse (stream, parameters.filename);
		}
	}
}
