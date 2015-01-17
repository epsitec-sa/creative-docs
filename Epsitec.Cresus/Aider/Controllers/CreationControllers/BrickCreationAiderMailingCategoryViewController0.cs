//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.SpecialFieldControllers;
using Epsitec.Aider.Data.Common;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderMailingCategoryViewController0 : BrickCreationViewController<AiderMailingCategoryEntity>
	{
		protected override void GetForm(ActionBrick<AiderMailingCategoryEntity, SimpleBrick<AiderMailingCategoryEntity>> action)
		{
			AiderGroupEntity defaultGroup = null;

			action
				.Title ("Création d'une catégorie de publipostage")
				.Field<string> ()
					.Title ("Nom de la catégorie")
				.End ()
				.Field<AiderGroupEntity> ()
					.WithSpecialField<AiderGroupSpecialField<AiderMailingCategoryEntity>> ()
					.Title ("Groupe auquel lier la catégorie")
					.InitialValue (defaultGroup)
				.End ()
			.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, AiderGroupEntity, AiderMailingCategoryEntity> (this.Execute);
		}

		private AiderMailingCategoryEntity Execute(string name, AiderGroupEntity group)
		{
			return AiderMailingCategoryEntity.Create (this.BusinessContext, name, group);
		}
	}
}
