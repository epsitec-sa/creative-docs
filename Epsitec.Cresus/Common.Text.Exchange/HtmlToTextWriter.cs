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
			//this.navigator.Insert ("Hello you world !");
			ProcessNodes (this.htmlDoc.Nodes);
		}



		private void ProcessNodes(HtmlNodeCollection nodes)
		{
			bool italique = false;
			bool bold = false;
			bool span = false;

			foreach (HtmlNode node in nodes)
			{
				string s = node.ToString ();
				if (node is HtmlElement)
				{
					HtmlElement element = node as HtmlElement;

					if (element.Name == "span")
					{
						span = true;
					}

					if (element.Name == "i" || element.Name == "em")
					{
						italique = true;
						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.InvertItalic = true;
						textWrapper.ResumeSynchronizations ();
					}

					if (element.Name == "b" || element.Name == "strong")
					{ 
						bold = true;
						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.InvertBold = true;
						textWrapper.ResumeSynchronizations ();

					}

#if false
					Console.WriteLine ("Element: {0}", element.Name) ;
					if (element.Attributes.Count > 0)
					{
						foreach (HtmlAttribute attribute in element.Attributes)
						{
							Console.WriteLine (" Arribute: {0} = {1}", attribute.Name, attribute.Value);
						}
					}
#endif
					ProcessNodes (element.Nodes);

					if (italique)
					{
						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.InvertItalic = false;
						textWrapper.ResumeSynchronizations ();
					}
					if (span)
					{
					}

					if (bold)
					{
						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.InvertBold = false;
						textWrapper.ResumeSynchronizations ();
					}

				}
				else
				{
					HtmlText text = node as HtmlText;
					//Console.WriteLine ("Text: {0}", text.Text);
					this.navigator.Insert (text.Text);
				}
			}
		}


		private HtmlDocument htmlDoc;
		private Wrappers.TextWrapper textWrapper;
		private Wrappers.ParagraphWrapper paraWrapper;
		private TextNavigator navigator;
	}
}
