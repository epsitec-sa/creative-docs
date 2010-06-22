//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public abstract class EditionViewController<T> : EntityViewController<T>, IEditionStatus
		where T : AbstractEntity
	{
		protected EditionViewController(string name, T entity)
			: base (name, entity)
		{
		}


		#region IEditionStatus Members

		/// <summary>
		/// Gets the edition status of the entity.
		/// </summary>
		/// <value>The edition status.</value>
		public EditionStatus EditionStatus
		{
			get
			{
				return this.GetEditionStatus ();
			}
		}

		#endregion
		
		protected virtual EditionStatus GetEditionStatus()
		{
			return EditionStatus.Unknown;
		}

		protected override void AboutToCloseUI()
		{
			var entity  = this.Entity;
			var context = Epsitec.Cresus.DataLayer.DataContextPool.Instance.FindDataContext (entity);

			if ((context.IsRegisteredAsEmptyEntity (entity)) &&
				(this.EditionStatus == EditionControllers.EditionStatus.Valid))
			{
				context.UnregisterEmptyEntity (entity);
			}
			
			base.AboutToCloseUI ();
		}

	}
}