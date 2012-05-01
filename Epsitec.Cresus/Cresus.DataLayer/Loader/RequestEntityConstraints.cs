//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.DataLayer.Loader
{
	/// <summary>
	/// The <c>RequestEntityConstraints</c> class maintains a list of constraints for every
	/// entity specified in a request.
	/// </summary>
	public sealed class RequestEntityConstraints : Dictionary<AbstractEntity, RequestConstraintList>
	{
	}
}
