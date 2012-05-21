//	Copyright © 2004-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Linq;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>FilterItem</c> class represents a filter for the file dialogs.
	/// </summary>
	public sealed class FilterItem
	{
		public FilterItem(string name, string caption, string filter)
		{
			ExceptionThrower.ThrowIfNullOrEmpty (name, "name");
			ExceptionThrower.ThrowIfNullOrEmpty (caption, "caption");
			ExceptionThrower.ThrowIfNullOrEmpty (filter, "filter");

			string[] items = filter.Split (' ', '|', ':', '/').Where (x => x.Length > 0).Select (x => FilterItem.SanitizeFilterItem (x)).ToArray ();

			this.name    = name;
			this.caption = caption;
			this.filter  = string.Join (";", items);
		}


		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public string							Caption
		{
			get
			{
				return this.caption;
			}
		}

		public string							Filter
		{
			get
			{
				return this.filter;
			}
		}


		public string GetFileDialogFilter()
		{
			var buffer = new System.Text.StringBuilder ();
			
			buffer.Append (this.caption);
			buffer.Append (" (");
			buffer.Append (this.filter);
			buffer.Append (")|");
			buffer.Append (this.filter);
			
			return buffer.ToString ();
		}


		/// <summary>
		/// Ensures that the filter item starts always with <c>"*."</c> so that it is
		/// compatible with the file dialogs.
		/// </summary>
		/// <param name="item">The filter item.</param>
		/// <returns>The sanitized filter item.</returns>
		private static string SanitizeFilterItem(string item)
		{
			if (item.StartsWith ("*"))
			{
				return item;
			}
			else if (item.StartsWith ("."))
			{
				return "*" + item;
			}
			else
			{
				return "*." + item;
			}
		}
		
		private readonly string					name;
		private readonly string					caption;
		private readonly string					filter;
	}
}
