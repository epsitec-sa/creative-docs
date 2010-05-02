//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.EntitiesAccessors
{
	public class BidirectionnalConverter
	{
		public BidirectionnalConverter()
		{
			this.content = new List<List<string>> ();
		}

		public void Add(string text1, string text2)
		{
			List<string> line = new List<string> ();
			line.Add (text1);
			line.Add (text2);

			this.content.Add (line);
		}


		public void Text1ToText2(Common.Widgets.AbstractTextField textField1, Common.Widgets.AbstractTextField textField2)
		{
			string text2 = this.Text1ToText2 (textField1.Text);

			if (!string.IsNullOrEmpty (text2))
			{
				textField2.Text = text2;
			}
		}

		public void Text2ToText1(Common.Widgets.AbstractTextField textField2, Common.Widgets.AbstractTextField textField1)
		{
			string text1 = this.Text2ToText1 (textField2.Text);

			if (!string.IsNullOrEmpty (text1))
			{
				textField1.Text = text1;
			}
		}


		public string Text1ToText2(string text1)
		{
			var list = this.Text1ToTexts2 (text1);

			if (list.Count == 0)
			{
				return null;
			}
			else
			{
				return list[0];
			}
		}

		public string Text2ToText1(string text2)
		{
			var list = this.Text2ToTexts1 (text2);

			if (list.Count == 0)
			{
				return null;
			}
			else
			{
				return list[0];
			}
		}


		public List<string> Text1ToTexts2(string text1)
		{
			return this.TextToTexts (text1, 0, 1);
		}

		public List<string> Text2ToTexts1(string text2)
		{
			return this.TextToTexts(text2, 1, 0);
		}

		private List<string> TextToTexts(string searchedText, int searchedIndex, int resultIndex)
		{
			var list = new List<string> ();

			searchedText = Misc.RemoveAccentsToLower (searchedText);

			foreach (var line in this.content)
			{
				if (Misc.RemoveAccentsToLower (line[searchedIndex]).StartsWith (searchedText))
				{
					list.Add (line[resultIndex]);
				}
			}

			return list;
		}


		public void InitializeComboWithText1(Widgets.SuperCombo combo)
		{
			this.InitializeComboWithText (combo, 0);
		}

		public void InitializeComboWithText2(Widgets.SuperCombo combo)
		{
			this.InitializeComboWithText (combo, 1);
		}

		private void InitializeComboWithText(Widgets.SuperCombo combo, int index)
		{
			var list = new List<string> ();

			foreach (var line in this.content)
			{
				if (!list.Contains (line[index]))
				{
					list.Add (line[index]);
				}
			}

			list.Sort ();

			foreach (var text in list)
			{
				combo.Items.Add (text);
			}

			combo.Autocompletion = true;
			combo.AutocompletionList.AddRange (list);
			combo.AutocompletionConverter = Misc.RemoveAccentsToLower;
		}


		private readonly List<List<string>> content;
	}
}
