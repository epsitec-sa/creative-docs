//	Copyright © 2013-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Cresus.WebCore.Server.Core;
using Nancy;

namespace Epsitec.Cresus.WebCore.Server.NancyModules
{
	public class JobsModule : AbstractAuthenticatedModule
	{
		public JobsModule(CoreServer coreServer)
			: base (coreServer, "/jobs")
		{
			Get["/list"] = p => Response.AsJson (this.GetJobs ());

			Get["/cancel/{job}"] = p =>
			{
				var jobId = (string) p.job;
				var job   = this.FindJob (jobId);

				if (this.CancelJob (job))
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
			};

			Get["/delete/{job}"] = p =>
			{
				var jobId = (string) p.job;
				var job   = this.FindJob (jobId);

				if (this.RemoveJob (job))
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
			};
		}
	}
}
