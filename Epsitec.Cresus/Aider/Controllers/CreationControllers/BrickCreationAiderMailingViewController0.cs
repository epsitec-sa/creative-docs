//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

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

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderMailingViewController0 : BrickCreationViewController<AiderMailingEntity>
	{
		protected override void GetForm(ActionBrick<AiderMailingEntity, SimpleBrick<AiderMailingEntity>> action)
		{
			var categories = this.GetCategories ();

			action
				.Title ("Création d'un publipostage")
				.Field<string> ()
					.Title ("Intitulé du publipostage")
				.End ()
				.Field<string> ()
					.Title ("Description")
				.End ()
				.Field<AiderMailingCategoryEntity> ()
					.WithFavorites (categories, favoritesOnly: true)
					.Title ("Catégorie")
				.End ()
			.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, string, AiderMailingCategoryEntity, AiderMailingEntity> (this.Execute);
		}

		private AiderMailingEntity Execute(string name, string desc, AiderMailingCategoryEntity cat)
		{
			var aiderUser = this.BusinessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);

			if (string.IsNullOrEmpty (name))
			{
				Logic.BusinessRuleException (this.Entity, "L'intitulé est obligatoire.");
			}
			
			if (cat.IsNull ())
			{
				Logic.BusinessRuleException (this.Entity, "La catégorie est obligatoire.");
			}

			return AiderMailingEntity.Create (this.BusinessContext, aiderUser, name, desc, cat, isReady: false);
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
