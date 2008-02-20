//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Michael WALZ

using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Text.Exchange
{
	class CresusWrapper : IDisposable
	{
		public CresusWrapper(HtmlToTextWriter writer)
		{
			this.writer = writer ;
			this.writer.TextWrapper.SuspendSynchronizations ();
		}

		void ClearInvertItalic ()
		{
			this.writer.TextWrapper.Defined.ClearInvertItalic() ;
		}

		public bool InvertItalic
		{
			set
			{
				writer.TextWrapper.Defined.InvertItalic = value;
			}
		}

		public bool InvertBold
		{
			set
			{
				writer.TextWrapper.Defined.InvertBold = value;
			}
		}

		public void Dispose()
		{
			this.writer.TextWrapper.ResumeSynchronizations ();
		}

		private HtmlToTextWriter writer;
	}

}
