//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>CoreAppPolicy</c> class defines internal settings which define how the application
	/// class behaves.
	/// </summary>
	public class CoreAppPolicy
	{
		public bool RequiresCoreCommandHandlers
		{
			get;
			set;
		}
	}
}
