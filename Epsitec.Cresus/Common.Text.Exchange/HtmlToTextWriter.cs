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

			textWrapper.Defined.ClearInvertItalic ();
			textWrapper.Defined.ClearColor ();
			//textWrapper.Defined.ClearConditions ();
			textWrapper.Defined.ClearFontFace ();
			//textWrapper.Defined.ClearFontFeatures ();
			//textWrapper.Defined.ClearFontGlue ();
			textWrapper.Defined.ClearFontSize ();
			//textWrapper.Defined.ClearFontStyle ();
			textWrapper.Defined.ClearInvertBold ();
			textWrapper.Defined.ClearInvertItalic ();
			textWrapper.Defined.ClearStrikeout() ;
			//textWrapper.Defined.ClearTextBox ();
			//textWrapper.Defined.ClearTextMarker ();
			textWrapper.Defined.ClearUnderline ();
			//textWrapper.Defined.ClearUnits ();
			//textWrapper.Defined.ClearUserTags ();
			textWrapper.Defined.ClearXscript ();

			ProcessNodes (this.htmlDoc.Nodes);
		}



		private void ProcessNodes(HtmlNodeCollection nodes)
		{
			foreach (HtmlNode node in nodes)
			{
				string s = node.ToString ();
				if (node is HtmlElement)
				{
					HtmlElement element = node as HtmlElement;

					if (element.Name == "span")
					{
						ProcessNodes (element.Nodes);
					}
					else if (element.Name == "i" || element.Name == "em")
					{
						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.InvertItalic = true;
						textWrapper.ResumeSynchronizations ();
						ProcessNodes (element.Nodes);
						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.InvertItalic = false;
						textWrapper.ResumeSynchronizations ();
					}
					else if (element.Name == "b" || element.Name == "strong")
					{ 
						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.InvertBold = true;
						textWrapper.ResumeSynchronizations ();
						ProcessNodes (element.Nodes);
						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.InvertBold = false;
						textWrapper.ResumeSynchronizations ();
					} // [MW:MYSTERE] <sup> et <sub> ne fonctionnent pas
					else if (element.Name == "sup")
					{
						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.Xscript.Offset += 0.25;
						textWrapper.ResumeSynchronizations ();
						ProcessNodes (element.Nodes);
						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.Xscript.Offset -= 0.25;
						textWrapper.ResumeSynchronizations ();
					}
					else if (element.Name == "sub")
					{
						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.Xscript.Offset -= 0.25;
						textWrapper.ResumeSynchronizations ();
						ProcessNodes (element.Nodes);
						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.Xscript.Offset += 0.25;
						textWrapper.ResumeSynchronizations ();
					}
					else if (element.Name == "font")
					{
						string fontface = "";
						string fontsize = "";

						foreach (HtmlAttribute attr in element.Attributes)
						{
							switch (attr.Name)
							{
								case "face":
									fontface = attr.Value ;
									break ;
								case "size":
									fontsize = attr.Value ;
									break ;
							}
						}

						string oldfontface = textWrapper.Defined.FontFace;
						double oldfontsize = textWrapper.Defined.FontSize;

						textWrapper.SuspendSynchronizations ();

						if (fontface.Length > 0)
						{
							textWrapper.Defined.FontFace = fontface;
						}

						if (fontsize.Length > 0)
						{
							textWrapper.Defined.FontSize = HtmlTextOut.HtmlFontSizeTopointFontSize (Int32.Parse (fontsize)) * HtmlTextOut.FontSizeFactor;
							textWrapper.Defined.Units = Common.Text.Properties.SizeUnits.Points;
						}

						textWrapper.ResumeSynchronizations ();

						ProcessNodes (element.Nodes);

						textWrapper.SuspendSynchronizations ();
						textWrapper.Defined.FontFace = oldfontface;
						textWrapper.Defined.FontSize = oldfontsize;
						textWrapper.ResumeSynchronizations ();
					}
					else
					{
						// element html inconnu, on traite l'intérieur sans s'occuper de l'élément lui même
						ProcessNodes (element.Nodes);
					}
				}
				else
				{
					HtmlText text = node as HtmlText;
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
