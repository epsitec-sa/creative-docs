using System;
using System.Collections.Generic;
using System.Text;
using Epsitec.Common.Text.Exchange.HtmlParser;

namespace Epsitec.Common.Text.Exchange
{
	class HtmlToTextWriter
	{

		public HtmlToTextWriter (HtmlDocument thehtmldoc, Wrappers.TextWrapper textWrapper, Wrappers.ParagraphWrapper paraWrapper, TextNavigator navigator)
		{
			this.htmlDoc = thehtmldoc;
			this.navigator = navigator;
			this.paraWrapper = paraWrapper;
			this.textWrapper = textWrapper;
		}

		public void ProcessIt()
		{
			navigator.Insert ("Hello you world !");
		}

		private HtmlDocument htmlDoc;
		private Wrappers.TextWrapper textWrapper;
		private Wrappers.ParagraphWrapper paraWrapper;
		private TextNavigator navigator;
	}
}
