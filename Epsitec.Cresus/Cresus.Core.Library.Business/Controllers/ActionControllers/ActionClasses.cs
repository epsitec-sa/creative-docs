//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	/// <summary>
	/// The <c>ActionClasses</c> enumeration defines a few well-known <see cref="ActionClass"/>
	/// instances.
	/// </summary>
	public enum ActionClasses
	{
		None,

		Create,
		Delete,
		Validate,
		Clear,
		Cancel,

		Start,
		NextStep,
		Stop,

		Output,
		Input,
	}
}
