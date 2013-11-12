//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Override;
using Epsitec.Aider.Data.Common;

using Epsitec.Common.Types;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Loader;

using System.Linq;
using System.Collections.Generic;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Common.Support;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (2)]
	public sealed class ActionAiderMailingViewController2Duplicate : ActionViewController<AiderMailingEntity>
	{
		public override bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("Dupliquer");
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<string, string, AiderMailingCategoryEntity> (this.Execute);
		}

		private void Execute(string newName,string newDesc, AiderMailingCategoryEntity cat)
		{
			var aiderUser = this.BusinessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);

			if (string.IsNullOrEmpty (newName))
			{
				Logic.BusinessRuleException (this.Entity, "L'intitulé est obligatoire.");
			}

			if (cat.IsNull ())
			{
				Logic.BusinessRuleException (this.Entity, "La catégorie est obligatoire.");
			}

			var copy = AiderMailingEntity.Create (this.BusinessContext, aiderUser, newName, newDesc, cat, isReady: false);

			copy.AddContacts (this.BusinessContext, this.Entity.RecipientContacts);

			foreach (var group in this.Entity.RecipientGroups)
			{
				copy.AddGroup (this.BusinessContext,group);
			}

			foreach (var household in this.Entity.RecipientHouseholds)
			{
				copy.AddHousehold (this.BusinessContext, household);
			}

			copy.ExludeContacts (this.BusinessContext, this.Entity.Exclusions);
			
		}

		protected override void GetForm(ActionBrick<AiderMailingEntity, SimpleBrick<AiderMailingEntity>> form)
		{
			var categories = this.GetCategories ();

			form
				.Title (this.GetTitle ())
				.Field<string> ()
					.Title ("Nouvel Intitulé")
					.InitialValue ("Copie de " + this.Entity.Name)
				.End ()
				.Field<string> ()
					.Title ("Description")
					.InitialValue (this.Entity.Description)
				.End ()
				.Field<AiderMailingCategoryEntity> ()
					.WithFavorites (categories, favoritesOnly: true)
					.Title ("Catégorie")
					.InitialValue(this.Entity.Category)
				.End ()
			.End ();
		}

		private List<AiderMailingCategoryEntity> GetCategories()
		{
			List<AiderMailingCategoryEntity> categories = new List<AiderMailingCategoryEntity> ();

			var aiderUser = this.BusinessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);

			if (aiderUser.EnableGroupEditionCanton)
			{
				categories.AddRange (AiderMailingCategoryEntity.GetCantonCategories (this.BusinessContext, aiderUser.ParishGroupPathCache));
			}

			if (aiderUser.EnableGroupEditionRegion)
			{
				categories.AddRange (AiderMailingCategoryEntity.GetRegionCategories (this.BusinessContext, aiderUser.ParishGroupPathCache));
			}

			if (aiderUser.EnableGroupEditionParish)
			{
				categories.AddRange (AiderMailingCategoryEntity.GetParishCategories (this.BusinessContext, aiderUser.ParishGroupPathCache));
			}

			return categories;
		}
	}
}
