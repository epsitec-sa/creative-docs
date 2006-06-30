//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ

using System;
using System.Collections.Generic;
using System.Text;



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

		public override string ToString()
		{
			return styles.ToString () + output.ToString ();
		}

		private StringBuilder output = new StringBuilder() ;
		private StringBuilder styles = new StringBuilder ();
	}
}
