//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	/// <summary>
	/// The <c>ICreationController</c> interface is used to identify view
	/// controllers used to create new entities.
	/// </summary>
	public interface ICreationController
	{
		void RegisterDisposeAction(System.Action disposeAction);
		void RegisterEntityCreator(System.Func<System.Action<BusinessContext, AbstractEntity>, AbstractEntity> entityCreator);
	}
}
