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
using System.Linq;
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

			Get["/test/{filename}"] = p =>
				this.Execute (wa => this.TestDownload (wa, p));

			Get["/delete/{filename}"] = p =>
				this.Execute (wa => this.DeleteFile (wa, p));

			Get["/delete/jobfile/{jobid}"] = p =>
				this.Execute (wa => this.DeleteJobFile (wa, p));

			Get["/list/"] = p => this.ListFiles ();
				
		}

		private Response DownloadFile(WorkerApp app, dynamic parameters)
		{
			var filePath = CoreContext.GetFileDepotPath ("downloads",parameters.filename);
			var stream = System.IO.File.OpenRead (filePath);
			return CoreResponse.CreateStreamResponse (stream, parameters.filename);
		}

		private Response TestDownload(WorkerApp app, dynamic parameters)
		{
			var filePath = CoreContext.GetFileDepotPath ("downloads", parameters.filename);
			if (!System.IO.File.Exists (filePath))
			{
				return Response.AsJson (false);
			}
			else
			{
				return Response.AsJson (true);
			}
		}

		private Response DeleteFile(WorkerApp app, dynamic parameters)
		{
			var filePath = CoreContext.GetFileDepotPath ("downloads", parameters.filename);
			System.IO.File.Delete (filePath);
			return Response.AsJson ("deleted");
		}

		private Response DeleteJobFile(WorkerApp app, dynamic parameters)
		{
			var path = CoreContext.GetFileDepotPath ("downloads");
			var filesInfo = System.IO.Directory.EnumerateFiles (path).Select (f => new System.IO.FileInfo (f));
			var file = filesInfo.SingleOrDefault (f => f.Name.StartsWith (parameters.jobid));
			if(file != null)
			{
				var filePath = CoreContext.GetFileDepotPath ("downloads", file.Name);
				System.IO.File.Delete (filePath);
				return Response.AsJson ("deleted");
			}
			else
			{
				return Response.AsJson ("nothing to delete");
			}			
		}


		private Response ListFiles()
		{
			var path = CoreContext.GetFileDepotPath ("downloads");
			var filesInfo = System.IO.Directory.EnumerateFiles (path).Select (f => new System.IO.FileInfo (f));

			var view = filesInfo.Where(f => f.Name != "dummy.txt").Select (f => new System.Tuple<string, string> (f.Name, (f.Length / 1024).ToString ()));
			return Response.AsJson (view);
		}
	}
}
