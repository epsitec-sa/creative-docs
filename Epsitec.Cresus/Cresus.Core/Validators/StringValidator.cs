//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Core.Validators
{
	public static class StringValidator
	{
		public static bool Validate(string value)
		{
			return value.Length < 40;  // TODO: pour tester...
		}
	}
}
