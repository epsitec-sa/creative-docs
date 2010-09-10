//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	// TODO Comment this enum.
	// Marc
	public enum ConnectionStatus
	{
		NotYetOpen = 0,
		Open = 1,
		Closed = 2,
		Interrupted = 3,
	}
}
