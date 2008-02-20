//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
