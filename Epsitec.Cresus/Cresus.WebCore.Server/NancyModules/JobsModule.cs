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
			Get["/cancel/{job}"] = (p =>
			{
				var job = this.GetJob (p.job);
				this.CancelJob (job);
				this.Execute (wa => this.UpdateTaskStatusInBag (wa, job));
				return this.RemoveJob (job);
			});
		}

		private Response UpdateTaskStatusInBag(WorkerApp app, CoreJob task)
		{
			var user = LoginModule.GetUserName (this);
			var entityBag = EntityBagManager.GetCurrentEntityBagManager ();
			entityBag.RemoveFromBag (user, task.Id, When.Now);
			entityBag.AddToBag (user, task.Title, task.HtmlView, task.Id, When.Now);

			return new Response ()
			{
				StatusCode = HttpStatusCode.Accepted
			};
		}
	}
}
