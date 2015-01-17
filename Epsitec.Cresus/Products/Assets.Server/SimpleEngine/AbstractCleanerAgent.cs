//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Un 'agent de nettoyage' est chargé de faire le ménage après la suppression
	/// d'un objet.
	/// </summary>
	public abstract class AbstractCleanerAgent
	{
		public AbstractCleanerAgent(DataAccessor accessor)
		{
			this.accessor = accessor;
		}

		public virtual void Clean(BaseType baseType, Guid guid)
		{
		}

		protected readonly DataAccessor accessor;
	}
}
