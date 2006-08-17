//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Text
{
	public class TextBreakEngineFactory : Drawing.ITextBreakEngineFactory
	{
		public TextBreakEngineFactory()
		{
		}

		#region ITextBreakEngineFactory Members

		public Epsitec.Common.Drawing.ITextBreakEngine Create()
		{
			return new TextBreakEngine ();
		}

		#endregion
	}
}
