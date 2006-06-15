//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class Caption : DependencyObject
	{
		public Caption()
		{
		}

		public IEnumerable<string> Labels
		{
			get
			{
				yield return this.ShortLabel;
				yield return this.Label;
				yield return this.LongLabel;
			}
		}

		public string ShortLabel
		{
			get
			{
				return "";
			}
		}

		public string Label
		{
			get
			{
				return "";
			}
		}

		public string LongLabel
		{
			get
			{
				return "";
			}
		}

		public string Description
		{
			get
			{
				return "";
			}
		}

		public string HelpReference
		{
			get
			{
				return null;
			}
		}
		
	}
}
