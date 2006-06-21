using System;
using System.Collections.Generic;
using System.Text;

// Conversion entre format presse-papier natif et 

namespace Epsitec.Common.Text.Exchange
{
	class NativeConverter
	{
		/// <summary>
		/// Convertit les attributs d'un textwrapper en format presse-papier natif
		/// </summary>
		/// <param name="textwrapper"></param>
		/// <returns></returns>
		public static string GetDefinedString(Wrappers.TextWrapper textWrapper, bool paragraphSep)
		{
			StringBuilder output = new StringBuilder();

			output.Append ('[');

			if (paragraphSep)
			{
				output.Append("par/") ;
			}

			if (textWrapper.Defined.IsInvertItalicDefined && textWrapper.Defined.InvertItalic)
			{
				output.Append ("i/");
			}

			if (textWrapper.Defined.IsInvertBoldDefined && textWrapper.Defined.InvertBold)
			{
				output.Append ("b/");
			}

			if (textWrapper.Defined.IsColorDefined)
			{
				output.AppendFormat ("c:{0}/", textWrapper.Defined.Color);
			}


			if (textWrapper.Defined.IsUnderlineDefined)
			{
				output.Append ("u/");
			}

			output.Append (']');
			return output.ToString() ;
		}

		public static void SetDefined(Wrappers.TextWrapper textwrapper, string input, out bool paragrpahSep)
		{
			char[] separators = new char[] {'/'};

			paragrpahSep = false;

			bool invertItalic = false;
			bool invertBold = false;
			bool underline = false;
			bool colorDef = false;

			System.Diagnostics.Debug.Assert (input[0] == '[' && input[input.Length-1] == ']');
			input = input.Substring(1, input.Length - 2) ;

			string[] elements = input.Split (separators, StringSplitOptions.RemoveEmptyEntries);

			foreach (string elem in elements)
			{
				string[] subelements = elem.Split (':');
				switch (subelements[0])
				{
					case "par":
						paragrpahSep = true;
						break;
					case "i":
						textwrapper.Defined.InvertItalic = true;
						invertItalic = true;
						break;
					case "b":
						textwrapper.Defined.InvertBold = true;
						invertBold = true;
						break;
					case "u":
						//					textwrapper.Defined.Underline. = true;
						underline = true;
						break;
					case "c":
						textwrapper.Defined.Color = subelements[1];
						colorDef = true;
						break;
				}
			}

			if (!invertItalic)
			{
				textwrapper.Defined.ClearInvertItalic ();
			}

			if (!invertBold)
			{
				textwrapper.Defined.ClearInvertBold ();
			}

			if (!underline)
			{
				textwrapper.Defined.ClearUnderline ();
			}

			if (!colorDef)
			{
				textwrapper.Defined.ClearColor ();
			}
		}
	}

}