//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP
using System.Linq;
using Epsitec.Aider.Entities;

using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SetControllers;

using Epsitec.Cresus.Core.Data;

using System.Collections.Generic;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Aider.Controllers.SetControllers
{
	[ControllerSubType (1)]
	internal sealed class SetAiderMailingViewController1ExcludedContact : SetViewController<AiderMailingEntity, AiderContactEntity, AiderContactEntity>
	{
		public override string GetIcon()
		{
			return Res.Commands.Base.ShowAiderMailingExclusions.Icon;
		}

		public override FormattedText GetTitle()
		{
			return Res.Commands.Base.ShowAiderMailingExclusions.Caption.DefaultLabel;
		}

		public override Druid GetDisplayDataSetId()
		{
			return Res.CommandIds.Base.ShowAiderContact;
		}

		public override Druid GetPickDataSetId()
		{
			return Res.CommandIds.Base.ShowAiderContact;
		}

		public override bool? GetOverrideEnableCreate()
		{
			return this.EnableButtons ();
		}

		public override bool? GetOverrideEnableDelete()
		{
			return this.EnableButtons ();
		}

		private bool EnableButtons()
		{
			return true;
		}

		protected override void SetupDisplayDataSetAccessor(AiderMailingEntity entity, DataSetAccessor dataSetAccessor)
		{
			var excludedContacts = this.Entity.Exclusions;
			var ids = new List<Constant> ();
			foreach (var contact in excludedContacts)
			{
				ids.Add (new Constant (this.BusinessContext.DataContext.GetNormalizedEntityKey (contact).Value.RowKey.Id.Value));
			}

			if (ids.Count () == 0)
			{
				ids.Add (new Constant (0));
			}

			dataSetAccessor.Customizers.Add ((dataContext, request, example) =>
			{
				request.Conditions.Add (
					new ValueSetComparison
					(
						InternalField.CreateId (example),
						SetComparator.In,
						ids
					)
				);
				
			});
		}

		protected override void SetupPickDataSetAccessor(AiderMailingEntity entity, DataSetAccessor dataSetAccessor)
		{
			var excludedContacts = this.Entity.Exclusions;
			var ids = new List<Constant> ();
			foreach (var contact in excludedContacts)
			{
				ids.Add (new Constant (this.BusinessContext.DataContext.GetNormalizedEntityKey (contact).Value.RowKey.Id.Value));
			}

			if (ids.Count () == 0)
			{
				ids.Add (new Constant (0));
			}

			dataSetAccessor.Customizers.Add ((dataContext, request, example) =>
			{
				request.Conditions.Add (
					new ValueSetComparison
					(
						InternalField.CreateId (example),
						SetComparator.NotIn,
						ids
					)
				);

			});
		}

		protected override void AddItems(IEnumerable<AiderContactEntity> entitiesToAdd)
		{
			this.Entity.ExludeContacts (this.BusinessContext, entitiesToAdd);
		}

		protected override void RemoveItems(IEnumerable<AiderContactEntity> entitiesToRemove)
		{
			this.Entity.UnExludeContacts (this.BusinessContext, entitiesToRemove);
		}
	}
}
