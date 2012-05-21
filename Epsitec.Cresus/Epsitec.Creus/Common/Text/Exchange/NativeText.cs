//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Michael WALZ

using System.Collections.Generic;

namespace Epsitec.Common.Text.Exchange
{
	public class NativeTextOut
	{
		public NativeTextOut()
		{
			this.Initialize() ;
		}

		private void Initialize()
		{
		}

		public void AppendTextLine(string text)
		{
			this.output.AppendLine (text);
		}

		public void AppendStyleLine(string text)
		{
			this.styles.AppendLine (text);
		}

		internal Internal.FormattedText FormattedText
		{
			get
			{
				return new Internal.FormattedText (this.ToString ());
			}
		}

		public override string ToString()
		{
			return this.styles.ToString () + this.output.ToString ();
		}

		private System.Text.StringBuilder output = new System.Text.StringBuilder ();
		private System.Text.StringBuilder styles = new System.Text.StringBuilder ();
	}
}
