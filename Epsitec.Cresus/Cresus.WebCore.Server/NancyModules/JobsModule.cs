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
	public class JobsModule : AbstractAuthenticatedModule
	{
		public JobsModule(CoreServer coreServer)
			: base (coreServer, "/jobs")
		{
			Get["/list"] = p => Response.AsJson (this.GetJobs ());

			Get["/cancel/{job}"] = (p =>
			{
				var job = this.GetJob (p.job);
				this.CancelJob (job);
				return new Response ()
				{
					StatusCode = HttpStatusCode.Accepted
				};
			});

			Get["/delete/{job}"] = (p =>
			{
				var job = this.GetJob (p.job);
				if (this.RemoveJob (job.Id))
				{
					return new Response ()
					{
						StatusCode = HttpStatusCode.Accepted
					};
				}
				else
				{
					return new Response ()
					{
						StatusCode = HttpStatusCode.InternalServerError
					};
				}
				
			});
		}
	}
}
