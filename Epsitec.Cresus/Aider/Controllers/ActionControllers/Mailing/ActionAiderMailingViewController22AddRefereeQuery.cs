//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (22)]
	public sealed class ActionAiderMailingViewController22AddRefereeQuery : ActionViewController<AiderMailingEntity>
	{
		public override bool ExecuteInQueue
		{
			get
			{
				return true;
			}
		}

		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Répondances");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderMailingEntity, SimpleBrick<AiderMailingEntity>> form)
		{
			var contactDataset      = Res.Commands.Base.ShowAiderReferee.CommandId;

			var settings  = AiderUserManager.GetCurrentDataSetUISettings (contactDataset);
			
			var queries = new List<string> ();
			foreach(var setting in settings)
			{
				queries.AddRange (setting.DataSetSettings.AvailableQueries.Select (q => q.Name.ToSimpleText ()));
			}

			form
				.Title ("Ajouter une requête sur les répondances")
				.Field<List<string>> ()
					.Title ("Requête")
					.WithStringCollection (queries)
				.End ()
			.End ();
		}

		private void Execute(string queryName)
		{
			var dataset   = Res.Commands.Base.ShowAiderReferee.CommandId;
			var query     = AiderMailingQueryEntity.Create (this.BusinessContext, queryName, dataset);
			this.Entity.AddQuery (this.BusinessContext, query);
		}
	}
}