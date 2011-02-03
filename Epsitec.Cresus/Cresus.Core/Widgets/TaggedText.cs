//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public class TaggedText
	{
		#region Self integrated test
#if DEBUG
		static TaggedText()
		{
			var tt = new TaggedText ();

			tt.SetSimpleText (null);
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (tt.GetTaggedText ()));
			tt.InsertTag (0, "<br/>");
			System.Diagnostics.Debug.Assert (tt.GetTaggedText () == "<br/>");

			tt.SetSimpleText ("ceci est un test");
			System.Diagnostics.Debug.Assert (tt.GetTaggedText () == "ceci est un test");
			tt.InsertTag (5, "<b>");
			tt.InsertTag (8, "</b>");
			System.Diagnostics.Debug.Assert (tt.GetTaggedText () == "ceci <b>est</b> un test");
			tt.InsertTag (5, "<i>");
			tt.InsertTag (8, "</i>");
			System.Diagnostics.Debug.Assert (tt.GetTaggedText () == "ceci <i><b>est</i></b> un test");

			tt.SetSimpleText ("x<y");
			System.Diagnostics.Debug.Assert (tt.GetTaggedText () == "x&lt;y");
			tt.InsertTag (1, "<b>");
			tt.InsertTag (2, "</b>");
			System.Diagnostics.Debug.Assert (tt.GetTaggedText () == "x<b>&lt;</b>y");
		}
#endif
		#endregion

		public TaggedText()
		{
			this.blocs = new List<string> ();
			this.blocs.Add ("");
		}

		public void SetSimpleText(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				this.blocs.Clear ();
				this.blocs.Add ("");
			}
			else
			{
				text = TextLayout.ConvertToTaggedText (text);
				this.SetTaggedText (text);
			}
		}

		private void SetTaggedText(string text)
		{
			this.blocs.Clear ();

			if (string.IsNullOrEmpty (text))
			{
				this.blocs.Add ("");
			}
			else
			{
				int index = 0;
				int last = 0;

				while (index < text.Length)
				{
					char c = text[index];

					if (c == '<')  // début d'un tag <xx> ?
					{
						if (index > last)
						{
							this.blocs.Add (text.Substring (last, index-last));
						}

						int end = text.IndexOf ('>', index+1);
						System.Diagnostics.Debug.Assert (end != -1);
						this.blocs.Add (text.Substring (index, end-index+1));

						index = last = end+1;
					}
					else if (c == '&')  // début d'un tag &xx; ?
					{
						if (index > last)
						{
							this.blocs.Add (text.Substring (last, index-last));
						}

						int end = text.IndexOf (';', index+1);
						System.Diagnostics.Debug.Assert (end != -1);
						this.blocs.Add (text.Substring (index, end-index+1));

						index = last = end+1;
					}
					else  // texte ?
					{
						index++;
					}
				}

				if (index > last)
				{
					this.blocs.Add (text.Substring (last, index-last));
				}
			}
		}

		public void InsertSimpleText(int indexToSimpleText, string text)
		{
			int blocIndex, stringIndex;
			this.GetIndex (indexToSimpleText, out blocIndex, out stringIndex);

			this.blocs[blocIndex] = this.blocs[blocIndex].Insert (stringIndex, text);
		}

		public void InsertTag(int indexToSimpleText, string tag)
		{
			System.Diagnostics.Debug.Assert (TaggedText.IsTag (tag));

			int blocIndex, stringIndex;
			this.GetIndex (indexToSimpleText, out blocIndex, out stringIndex);

			if (stringIndex == 0)
			{
				this.blocs.Insert (blocIndex, tag);
			}
			else if (stringIndex == this.blocs[blocIndex].Length)
			{
				this.blocs.Insert (blocIndex+1, tag);
			}
			else
			{
				string t1 = this.blocs[blocIndex].Substring (0, stringIndex);
				string t2 = this.blocs[blocIndex].Substring (stringIndex);

				this.blocs[blocIndex] = t1;
				this.blocs.Insert (blocIndex+1, tag);
				this.blocs.Insert (blocIndex+2, t2);
			}
		}

		public string GetSimpleText()
		{
			return string.Concat (this.blocs.Where (x => !TaggedText.IsTag (x)));
		}

		public string GetTaggedText()
		{
			return string.Concat (this.blocs);
		}


		private bool GetIndex(int indexToSimpleText, out int blocIndex, out int stringIndex)
		{
			int startIndex = 0;

			for (blocIndex = 0; blocIndex < this.blocs.Count; blocIndex++)
			{
				var bloc = this.blocs[blocIndex];

				if (!TaggedText.IsTag (bloc))
				{
					if (indexToSimpleText >= startIndex && indexToSimpleText <= startIndex+bloc.Length)
					{
						stringIndex = indexToSimpleText - startIndex;
						return true;
					}

					startIndex += bloc.Length;
				}
			}

			blocIndex = this.blocs.Count-1;
			stringIndex = this.blocs[blocIndex].Length;  // à la fin du dernier bloc
			return false;
		}


		private static bool IsTag(string text)
		{
			if (string.IsNullOrEmpty(text) || text.Length < 1)
			{
				return false;
			}

			return text[0] == '<' || text[0] == '&';
		}


		private readonly List<string> blocs;
	}
}
