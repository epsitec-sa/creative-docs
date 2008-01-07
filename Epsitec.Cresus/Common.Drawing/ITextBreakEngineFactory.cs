//	Copyright © 2006-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	public interface ITextBreakEngineFactory
	{
		ITextBreakEngine Create();
	}
}
