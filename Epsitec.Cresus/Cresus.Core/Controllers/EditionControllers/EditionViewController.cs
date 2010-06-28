//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public abstract class EditionViewController<T> : EntityViewController<T>
		where T : AbstractEntity
	{
		protected EditionViewController(string name, T entity)
			: base (name, entity)
		{
		}


		protected override void AboutToCloseUI()
		{
			this.UpgradeEmptyEntity ();
			base.AboutToCloseUI ();
		}

		protected override void AboutToSave()
		{
			this.UpgradeEmptyEntity ();
			base.AboutToSave ();
		}

		/// <summary>
		/// If the current entity was registered in the <see cref="DataContext"/> as an empty
		/// entity, upgrade it to a real entity if its content is valid.
		/// </summary>
		private void UpgradeEmptyEntity()
		{
			var entity  = this.Entity;
			var context = DataContextPool.Instance.FindDataContext (entity);

			bool isEmpty = this.EditionStatus == EditionStatus.Empty;

			this.UpdateEmptyEntityStatus (context, isEmpty);
		}

		protected virtual void UpdateEmptyEntityStatus(DataContext context, bool isEmpty)
		{
		}
	}
}