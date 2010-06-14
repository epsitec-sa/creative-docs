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
		public virtual EditionStatus EditionStatus
		{
			get
			{
				return EditionStatus.Unknown;
			}
		}

		#endregion
	}
}