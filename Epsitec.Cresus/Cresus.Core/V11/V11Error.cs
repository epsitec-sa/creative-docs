//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.V11
{
	public enum V11Error
	{
		OK,
		Aborted,
		GenericError,

		FileNotFound,
		InvalidFormat,
		InvalidNoClient,
		InvalidNoRéférence,
	}
}
