//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class SearchEngine
	{
		public SearchEngine(string filter, SearchMode mode = SearchMode.IgnoreCase | SearchMode.IgnoreDiacritic | SearchMode.Fragment)
		{
			this.filter = filter;
			this.mode   = mode;

			if ((this.mode & SearchMode.Regex) != 0)
			{
				try
				{
					this.regexFilter = new Regex (this.filter, RegexOptions.Compiled);
				}
				catch
				{
					this.regexFilter = null;
				}
			}
			else
			{
				this.processFilter = this.Process (this.filter);
			}
		}

		public bool IsMatching(string text)
		{
			if (this.HasFilter)
			{
				text = this.Process (text);

				if ((this.mode & SearchMode.FullText) != 0)
				{
					return this.IsMatchingFullText (text);
				}
				else if ((this.mode & SearchMode.Fragment) != 0)
				{
					return this.IsMatchingFragment (text);
				}
				else if ((this.mode & SearchMode.Regex) != 0)
				{
					return this.IsMatchingRegex (text);
				}
				else
				{
					return false;
				}
			}
			else
			{
				return true;
			}
		}


		private bool IsMatchingFullText(string text)
		{
			return text == this.processFilter;
		}

		private bool IsMatchingFragment(string text)
		{
			return text.Contains (this.processFilter);
		}

		private bool IsMatchingRegex(string text)
		{
			if (this.regexFilter == null)
			{
				return false;
			}
			else
			{
				return this.regexFilter.IsMatch (text);
			}
		}


		private string Process(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				if ((this.mode & SearchMode.IgnoreCase) != 0)
				{
					text = text.ToLowerInvariant ();
				}

				if ((this.mode & SearchMode.IgnoreDiacritic) != 0)
				{
					text = ApproximativeSearching.RemoveDiatritic (text);
				}

				if ((this.mode & SearchMode.Phonetic) != 0)
				{
					text = ApproximativeSearching.Phonetic (text);
				}
			}

			return text;
		}


		private bool HasFilter
		{
			get
			{
				return !string.IsNullOrEmpty (this.filter);
			}
		}


		private readonly string					filter;
		private readonly string					processFilter;
		private readonly Regex					regexFilter;
		private readonly SearchMode				mode;
	}
}
