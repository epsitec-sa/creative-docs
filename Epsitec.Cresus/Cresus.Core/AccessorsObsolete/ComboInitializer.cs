//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Accessors
{
	public class ComboInitializer
	{
		public ComboInitializer()
		{
			this.content = new Dictionary<string, string> ();
		}

		public Dictionary<string, string> Content
		{
			get
			{
				return this.content;
			}
		}

		public string DefaultInternalContent
		{
			get;
			set;
		}


		public void InitializeCombo(Widgets.SuperCombo combo)
		{
			var list = new List<string> ();

			foreach (var pair in this.content)
			{
				combo.Items.Add (pair.Key, pair.Value);
				list.Add (pair.Value);
			}

			list.Sort ();

			combo.Autocompletion = true;
			combo.AutocompletionList.AddRange (list);
			combo.AutocompletionConverter = Misc.RemoveAccentsToLower;
		}


		public string ConvertInternalToEdition(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				text = this.DefaultInternalContent;
			}

			if (text == null)
			{
				text = "";
			}

			var words = Misc.Split (text.Replace (",", " "), " ");
			var list = new List<string> ();

			foreach (string word in words)
			{
				if (!string.IsNullOrEmpty (word))
				{
					list.Add (this.WordInternalToEdition (word));
				}
			}

			return Misc.Join (", ", list);
		}

		public string ConvertEditionToInternal(string text)
		{
			var words = Misc.Split (text.Replace (",", " "), " ");
			var list = new List<string> ();

			foreach (string word in words)
			{
				if (!string.IsNullOrEmpty (word))
				{
					list.Add (this.WordEditionToInternal (word));
				}
			}

			return Misc.Join (", ", list);
		}


		private string WordInternalToEdition(string text)
		{
			if (this.content.ContainsKey (text))
			{
				return this.content[text];
			}

			return text;
		}

		private string WordEditionToInternal(string text)
		{
			foreach (KeyValuePair<string, string> pair in this.content)
			{
				if (text == pair.Value)
				{
					return pair.Key;
				}
			}

			return text;
		}


		private Dictionary<string, string> content;
	}
}
