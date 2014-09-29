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
	public sealed class ActionAiderMailingViewController18AddQueryExtraction : ActionViewController<AiderMailingEntity>
	{
		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Ajouter depuis une requête");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string> (this.Execute);
		}

		protected override void GetForm(ActionBrick<AiderMailingEntity, SimpleBrick<AiderMailingEntity>> form)
		{

			var settings = AiderUserManager
									.Current
									.AuthenticatedUser
									.CustomUISettings
									.DataSetUISettings
									.Where 
									(
										d => 
										d.DataSetCommandId == Res.Commands.Base.ShowAiderContactFiltered.CommandId
									);
			
			var queries = new List<string> ();
			foreach(var setting in settings)
			{
				queries.AddRange (setting.DataSetSettings.AvailableQueries.Select (q => q.Name.ToSimpleText ()));
			}

			form
				.Title ("Choix de la requête")
				.Field<List<string>> ()
					.Title ("Requête")
					.WithStringCollection (queries)
				.End ()
			.End ();
		}

		private void Execute(string queryName)
		{
			this.Entity
				.RecipientQuery = AiderUserManager
									.Current
									.AuthenticatedUser
									.CustomUISettings
									.DataSetUISettings
									.Where
									(
										d =>
										d.DataSetCommandId == Res.Commands.Base.ShowAiderContactFiltered.CommandId
									).SelectMany
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

			this.Entity.AddContactsFromQuery (this.BusinessContext);
		}
	}
}