//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Michael WALZ

/// Problèmes non résolus:
/// 1)
///		Pour les tags html sup, sub et u les paramètres supplémentaires sont hardcodées de la même façon
///		que dans la classe Epsitec.Common.Document.Wappers du fichier Common.Text/Wrappers.cs,
///		chercher commentaires [HARDCODE]
///		D'ailleurs les méthodes Fill*Definition(...) ont été copiées depuis Common.Text/Wrappers.cs
///		
///	2)
///		Il faut voir si on ne peux pas simplifier l'usage des multiples SuspendSynchronizations()/ResumeSynchronizations()
///		
///	3)
///		Dans la fonction ProcessIt () il faut voir si les divers Clear...() sont appelées correctement
///		
	
using System;
using System.Collections.Generic;
using System.Text;
using Epsitec.Common.Text.Exchange.HtmlParser;

namespace Epsitec.Common.Text.Exchange
{
	/// <summary>
	/// La classe HtmlToTextWriter s'occupe d'écrire un texte html sous forme de HtmlDocument (arbre html parsé)
	/// dans un pavé de text par son Wrapper.TextWrapper
	/// </summary>
	/// 
	class HtmlToTextWriter
	{
		public HtmlToTextWriter(HtmlDocument thehtmldoc, CopyPasteContext cpContext, PasteMode pasteMode)
		{
			this.htmlDoc = thehtmldoc;
			this.navigator = cpContext.Navigator;
			this.paraWrapper = cpContext.ParaWrapper;
			this.textWrapper = cpContext.TextWrapper;
			this.pasteMode = pasteMode;
		}

		public void ProcessIt()
		{
			this.textWrapper.Defined.ClearDefinedProperties ();
			this.ProcessNodes (this.htmlDoc.Nodes);
		}

		private void ProcessSpan(HtmlElement htmlelement)
		{
			string style = string.Empty;

			foreach (HtmlAttribute attr in htmlelement.Attributes)
			{
				switch (attr.Name)
				{
					case "style":
						style = attr.Value;
						break;
				}
			}

			string fontface = string.Empty;
			string fontsize = string.Empty;
			string fontcolor = string.Empty;

			if (style.Length > 0)
			{
				SpanStyleElements spanstyleelements = new SpanStyleElements (style);

				foreach (string element in spanstyleelements)
				{
					string value = spanstyleelements[element];

					if (element == "font-size")
					{
						fontsize = value;
					}

					if (element == "font-family")
					{
						fontface = value;
					}

					if (element == "color")
					{
						fontcolor = value;
					}

					if (element == "mso-spacerun" && value == "yes")
					{
						this.IsSpacerun = true;
					}
				}

				if (this.IsSpacerun)
				{
					this.IsSpacerun = true;
					this.ProcessNodes (htmlelement.Nodes);
					this.IsSpacerun = false;
				}
				else
				{
					HtmlFontProperties oldfontprops = SaveFontProps ();

					this.textWrapper.SuspendSynchronizations ();

					if (fontface.Length > 0)
					{
						this.textWrapper.Defined.FontFace = fontface;
					}

					if (fontsize.Length > 0)
					{
						int ptindex = fontsize.LastIndexOf ("pt");

						if (ptindex != -1)
						{
							fontsize = fontsize.Substring (0, ptindex);
							double size = Misc.ParseDouble (fontsize);

							if (size != 0.0)
							{
								size = size * HtmlTextOut.FontSizeFactor;

								if (size != 0)
								{
									this.textWrapper.Defined.FontSize = size;
									this.textWrapper.Defined.Units = Common.Text.Properties.SizeUnits.Points;
								}
							}
						}
						else
						{
							fontsize = string.Empty;
						}
					}

					if (fontcolor.Length > 0)
					{
						Epsitec.Common.Drawing.RichColor richcolor = Epsitec.Common.Drawing.RichColor.FromName (fontcolor);
						this.textWrapper.Defined.Color = Epsitec.Common.Drawing.RichColor.ToString (richcolor);
					}

					this.textWrapper.ResumeSynchronizations ();

					this.ProcessNodes (htmlelement.Nodes);

					this.textWrapper.SuspendSynchronizations ();
					this.RestoreFontProps (oldfontprops);
					this.textWrapper.ResumeSynchronizations ();
				}
			}
			else
			{
				this.ProcessNodes (htmlelement.Nodes);
			}
		}

		private void ProcessItalic(HtmlElement element)
		{
#if false	// voir avec Pierre si c'est mieux de faire comme ça
			using (CresusWrapper w = new CresusWrapper (this))
			{
				w.InvertItalic = true;
			}

			this.ProcessNodes (element.Nodes);

			using (CresusWrapper w = new CresusWrapper (this))
			{
				w.InvertItalic = false;
			}
#else
			this.textWrapper.SuspendSynchronizations ();
			this.textWrapper.Defined.InvertItalic = true;
			this.textWrapper.ResumeSynchronizations ();
			this.ProcessNodes (element.Nodes);
			this.textWrapper.SuspendSynchronizations ();
			this.textWrapper.Defined.InvertItalic = false;
			this.textWrapper.ResumeSynchronizations ();
#endif
		}

		private void ProcessBold(HtmlElement element)
		{
			this.textWrapper.SuspendSynchronizations ();
			this.textWrapper.Defined.InvertBold = true;
			this.textWrapper.ResumeSynchronizations ();
			this.ProcessNodes (element.Nodes);
			this.textWrapper.SuspendSynchronizations ();
			this.textWrapper.Defined.InvertBold = false;
			this.textWrapper.ResumeSynchronizations ();
		}



		private void ProcessSup(HtmlElement element)
		{
			this.textWrapper.SuspendSynchronizations ();
			FillSuperscriptDefinition (this.textWrapper.Defined.Xscript);
			this.textWrapper.ResumeSynchronizations ();
			this.ProcessNodes (element.Nodes);
			this.textWrapper.SuspendSynchronizations ();
			this.textWrapper.Defined.ClearXscript ();
			this.textWrapper.ResumeSynchronizations ();

		}

		private void ProcessSub(HtmlElement element)
		{
			this.textWrapper.SuspendSynchronizations ();
			FillSubscriptDefinition (this.textWrapper.Defined.Xscript);
			this.textWrapper.ResumeSynchronizations ();
			this.ProcessNodes (element.Nodes);
			this.textWrapper.SuspendSynchronizations ();
			this.textWrapper.Defined.ClearXscript ();
			this.textWrapper.ResumeSynchronizations ();
		}

		private void ProcessUnderline(HtmlElement element)
		{
			this.textWrapper.SuspendSynchronizations ();

			this.FillUnderlineDefinition (this.textWrapper.Defined.Underline);

			this.textWrapper.ResumeSynchronizations ();
			this.ProcessNodes (element.Nodes);
			this.textWrapper.SuspendSynchronizations ();
			this.textWrapper.Defined.ClearUnderline ();
			this.textWrapper.ResumeSynchronizations ();
		}


		private void ProcessFont(HtmlElement element)
		{
			string fontface = string.Empty;
			string fontsize = string.Empty;
			string fontcolor = string.Empty ;

			foreach (HtmlAttribute attr in element.Attributes)
			{
				switch (attr.Name)
				{
					case "face":
						fontface = attr.Value;
						break;
					case "size":
						fontsize = attr.Value;
						break;
					case "color":
						fontcolor = attr.Value;
						break;
				}
			}

			HtmlFontProperties oldfontprops = SaveFontProps();
			
			this.textWrapper.SuspendSynchronizations ();

			if (fontface.Length > 0)
			{
				this.textWrapper.Defined.FontFace = fontface;
			}

			if (fontsize.Length > 0)
			{
				int size = Misc.ParseInt (fontsize);

				if (size != 0)
				{
					this.textWrapper.Defined.FontSize = HtmlTextOut.HtmlFontSizeToPointFontSize (size) * HtmlTextOut.FontSizeFactor;
					this.textWrapper.Defined.Units = Common.Text.Properties.SizeUnits.Points;
				}
			}

			if (fontcolor.Length > 0)
			{
				Epsitec.Common.Drawing.RichColor richcolor = Epsitec.Common.Drawing.RichColor.FromName (fontcolor);
				this.textWrapper.Defined.Color = Epsitec.Common.Drawing.RichColor.ToString(richcolor);
			}

			this.textWrapper.ResumeSynchronizations ();

			this.ProcessNodes (element.Nodes);
#if true // c'est là que ça foire
			this.textWrapper.SuspendSynchronizations ();
			this.RestoreFontProps (oldfontprops);
			this.textWrapper.ResumeSynchronizations ();
#endif
		}

		private void ProcessBr(HtmlElement element)
		{
			this.navigator.Insert (Unicode.Code.LineSeparator);
			this.ProcessNodes (element.Nodes);
		}

		private void ProcessP(HtmlElement element)
		{
			if (this.pendingP)
			{
				pendingP = false;
				this.navigator.Insert (Unicode.Code.ParagraphSeparator);
			}

			this.ProcessNodes (element.Nodes);
			this.pendingP = true;
		}

		private void ProcessDiv(HtmlElement element)
		{
			if (this.pendingDiv)
			{
//				pendingDiv = false;
//				this.navigator.Insert (Unicode.Code.ParagraphSeparator);
			}

			string alignMode = string.Empty;

			foreach (HtmlAttribute attr in element.Attributes)
			{
				switch (attr.Name)
				{
					case "align":
						alignMode = attr.Value;
						break;
				}
			}


			bool justificationModeDefined = false;
			Wrappers.JustificationMode oldjustificationmode = this.paraWrapper.Defined.JustificationMode;

			if (alignMode.Length > 0)
			{
				this.paraWrapper.SuspendSynchronizations ();
				justificationModeDefined = this.paraWrapper.Defined.IsJustificationModeDefined;
				this.paraWrapper.Defined.JustificationMode = DecodeAlignMode (alignMode);
				this.paraWrapper.ResumeSynchronizations ();
			}

			this.ProcessNodes (element.Nodes);

				this.navigator.Insert (Unicode.Code.ParagraphSeparator);

			if (justificationModeDefined)
			{
				this.paraWrapper.SuspendSynchronizations ();
				this.paraWrapper.Defined.JustificationMode = oldjustificationmode;
				this.paraWrapper.ResumeSynchronizations ();
			}
			else
				this.paraWrapper.Defined.ClearJustificationMode();

			this.pendingDiv = true;
		}



		private static Wrappers.JustificationMode DecodeAlignMode(string mode)
		{
			 switch (mode)
			 {
				 case "center":
					 return Wrappers.JustificationMode.Center ;
				 case "right":
					 return Wrappers.JustificationMode.AlignRight;
				 case "justify":
					 return Wrappers.JustificationMode.JustifyAlignLeft;
				 case "left":
					 return Wrappers.JustificationMode.AlignLeft;
				 default:
					 return Wrappers.JustificationMode.Unknown;
			 }
		}


		private HtmlFontProperties SaveFontProps()
		{
			HtmlFontProperties props = new HtmlFontProperties ();
			props.IsFontFaceDefined = this.textWrapper.Defined.IsFontFaceDefined;
			props.FontFace  = this.textWrapper.Defined.FontFace;

			props.IsColorDefined = this.textWrapper.Defined.IsColorDefined;
			props.Color = this.textWrapper.Defined.Color;

			props.IsFontSizeDefined = this.textWrapper.Defined.IsFontSizeDefined;
			props.FontSize  = this.textWrapper.Defined.FontSize;

			props.IsUnitsDefined = this.textWrapper.Defined.IsUnitsDefined;
			props.Units = this.textWrapper.Defined.Units;

			return props;
		}

		private void RestoreFontProps(HtmlFontProperties oldprops)
		{
			if (oldprops.IsFontFaceDefined)
				this.textWrapper.Defined.FontFace = oldprops.FontFace;
			else
				this.textWrapper.Defined.ClearFontFace ();

			if (oldprops.IsColorDefined)
				this.textWrapper.Defined.Color = oldprops.Color;
			else
				this.textWrapper.Defined.ClearColor ();

			if (oldprops.IsFontSizeDefined)
				this.textWrapper.Defined.FontSize = oldprops.FontSize;
			else
				this.textWrapper.Defined.ClearFontSize ();

			if (oldprops.IsUnitsDefined)
				this.textWrapper.Defined.Units = oldprops.Units;
			else
				this.textWrapper.Defined.ClearUnits ();
		}

		private void ProcessStyleNodes(HtmlNodeCollection nodes)
		{
			foreach (HtmlNode node in nodes)
			{
				string s = node.ToString ();
			}
		}


		private void ProcessStyle(HtmlElement element)
		{
			ProcessStyleNodes (element.Nodes);
		}

		
		private void ProcessNodes(HtmlNodeCollection nodes)
		{
			foreach (HtmlNode node in nodes)
			{
				string s = node.ToString ();
				if (node is HtmlElement)
				{
					HtmlElement element = node as HtmlElement;

					switch (element.Name)
					{
						case "span":
							this.ProcessSpan (element);
							break ;
					
						case "i":
						case "em":
							this.ProcessItalic (element);
							break ;
					
						case "b" :
						case "strong" :
							this.ProcessBold (element);
							break ;
					
						case "sup" :
							this.ProcessSup (element);
							break ;
					
						case "sub" :
							this.ProcessSub (element);
							break ;

						case "u" :
							this.ProcessUnderline (element);
							break;

#if true	// ne traite pas <font> pour des raisons de recherche de bug
						case "font" :
							this.ProcessFont (element);
							break ;
#endif
						
						case "br" :
						case "<br />" :
							this.ProcessBr (element);
							break ;

						case "p" :
							this.ProcessP (element);
							break ;

						case "div":
							this.ProcessDiv (element);
							break;

						case "style":
							this.ProcessStyle (element);
							break;

						default :
							// element html inconnu, on traite l'intérieur sans s'occuper de l'élément lui même
							this.ProcessNodes (element.Nodes);
							break;
					}
				}
				else
				{
					HtmlText text = node as HtmlText;

					if (this.IsSpacerun)
					{
						string str = this.TransformSpaceRun (text.Text);
						this.navigator.Insert (str);
					}
					else
					{
						this.navigator.InsertWithTabs (text.Text);
					}
				}
			}
		}

		private string TransformSpaceRun(string input)
		{
			return input.Replace((char)(0xa0), ' ') ;
		}

		private void FillSubscriptDefinition(Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript)
		{
			xscript.IsDisabled = false;

			xscript.Scale  =  0.6;			// [HARDCODE]
			xscript.Offset = -0.15;			// [HARDCODE]
		}

		private void FillSuperscriptDefinition(Common.Text.Wrappers.TextWrapper.XscriptDefinition xscript)
		{
			xscript.IsDisabled = false;

			xscript.Scale  = 0.6;			// [HARDCODE]
			xscript.Offset = 0.25;			// [HARDCODE]
		}


		private void FillUnderlineDefinition(Common.Text.Wrappers.TextWrapper.XlineDefinition xline)
		{
			xline.IsDisabled = false;

			double thickness;
			double position;
			if (System.Globalization.RegionInfo.CurrentRegion.IsMetric)
			{
				thickness = 1.0;  // 0.1mm			// [HARDCODE]
				position  = 5.0;  // 0.5mm			// [HARDCODE]
			}
			else
			{
				thickness = 1.27;  // 0.005in			// [HARDCODE]
				position  = 5.08;  // 0.02in			// [HARDCODE]
			}

			xline.Thickness      = thickness;
			xline.ThicknessUnits = Common.Text.Properties.SizeUnits.Points;
			xline.Position       = position;
			xline.PositionUnits  = Common.Text.Properties.SizeUnits.Points;
			xline.DrawClass      = "";
			xline.DrawStyle      = null;
		}

		private HtmlDocument htmlDoc;
		private Wrappers.TextWrapper textWrapper;
		private Wrappers.ParagraphWrapper paraWrapper;
		private TextNavigator navigator;
		private PasteMode pasteMode;

		private bool IsSpacerun = false;

		private bool pendingDiv = false;
		private bool pendingP = false;

		public Wrappers.TextWrapper TextWrapper
		{
			get
			{
				return textWrapper;
			}
		}

		public TextNavigator Navigator
		{
			get
			{
				return navigator;
			}
		}

	}


	struct HtmlFontProperties
	{
		string fontFace;
		double fontSize;
		string color;
		Text.Properties.SizeUnits units;
		bool isFontFaceDefined;
		bool isColorDefined;
		bool isFontSizeDefined;
		bool isUnitsDefined;

		public bool IsUnitsDefined
		{
			get
			{
				return isUnitsDefined;
			}
			set
			{
				isUnitsDefined = value;
			}
		}

		public bool IsFontSizeDefined
		{
			get
			{
				return isFontSizeDefined;
			}
			set
			{
				isFontSizeDefined = value;
			}
		}

		public bool IsFontFaceDefined
		{
			get
			{
				return isFontFaceDefined;
			}
			set
			{
				isFontFaceDefined = value;
			}
		}


		public bool IsColorDefined
		{
			get
			{
				return isColorDefined;
			}
			set
			{
				isColorDefined = value;
			}
		}

		public Text.Properties.SizeUnits Units
		{
			get
			{
				return units;
			}
			set
			{
				units = value;
			}
		}

		public string FontFace
		{
			get
			{
				return fontFace;
			}
			set
			{
				fontFace = value;
			}
		}

		public string Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		public double FontSize
		{
			get
			{
				return fontSize;
			}
			set
			{
				fontSize = value;
			}
		}
	}

}
