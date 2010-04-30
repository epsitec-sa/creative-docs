//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;


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
