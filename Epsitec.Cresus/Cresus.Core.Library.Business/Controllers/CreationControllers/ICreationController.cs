//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	using EntityCreatorFunction	= System.Func<System.Func<BusinessContext, AbstractEntity>, System.Action<BusinessContext, AbstractEntity>, AbstractEntity>;

	/// <summary>
	/// The <c>ICreationController</c> interface is used to identify view
	/// controllers used to create new entities.
	/// </summary>
	public interface ICreationController
	{
		void RegisterDisposeAction(System.Action disposeAction);
		void RegisterEntityCreator(EntityCreatorFunction entityCreator);
	}
}
