//	Copyright © 2007-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityCollection</c> class is used as a common base class for all
	/// its generic versions.
	/// </summary>
	public abstract class EntityCollection : ObservableList<object>
	{
		public bool								IsVirtualizerEnabled
		{
			get
			{
				return this.enableVirtualizer;
			}
		}


		/// <summary>
		/// Enables the virtualization of null entities. See <see cref="EntityNullReferenceVirtualizer"/>.
		/// </summary>
		internal void EnableEntityNullReferenceVirtualizer()
		{
			this.enableVirtualizer = true;
		}

		/// <summary>
		/// Virtualizes the null references of the specified entity, if this has
		/// been enabled for this collection.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="entity">The entity.</param>
		protected void Virtualize<T>(T entity)
			where T: AbstractEntity
		{
			if (this.enableVirtualizer)
			{
				EntityNullReferenceVirtualizer.PatchNullReferences (entity);
			}
		}

		private bool							enableVirtualizer;
	}
}
