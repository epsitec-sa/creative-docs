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
	[ControllerSubType (18)]
	public sealed class ActionAiderMailingViewController18AddContactsFromQuery : ActionViewController<AiderMailingEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Ajouter un filtre");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderMailingEntity, SimpleBrick<AiderMailingEntity>> form)
		{
			var dataset   = Res.Commands.Base.ShowAiderContactFiltered.CommandId;
			var settings  = AiderUserManager.GetCurrentDataSetSettingsUISettings(dataset);
			
			var queries = new List<string> ();
			foreach(var setting in settings)
			{
				queries.AddRange (setting.DataSetSettings.AvailableQueries.Select (q => q.Name.ToSimpleText ()));
			}

			form
				.Title ("Choix du filtre")
				.Field<List<string>> ()
					.Title ("Filtre")
					.WithStringCollection (queries)
				.End ()
			.End ();
		}

		private void Execute(string queryName)
		{
			var dataset   = Res.Commands.Base.ShowAiderContactFiltered.CommandId;
			this.Entity
				.RecipientQuery = AiderUserManager
									.GetCurrentDataSetSettingsUISettings (dataset)
									.SelectMany
									(
										d => d.DataSetSettings.AvailableQueries
									).Where
									(
										q => q.Name == queryName
									).Select 
									(
										q => DataSetUISettingsEntity.XmlToByteArray (q.Save("q"))
									)
									.FirstOrDefault();

			this.Entity.RecipientQueryName = queryName;
			this.Entity.UpdateMailingParticipants (this.BusinessContext);

		}
	}
}