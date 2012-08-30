//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library.Settings
{
	public sealed class UserEntityTableSettings
	{
		public UserEntityTableSettings ()
		{
			this.sort = new List<ColumnRef<EntityColumnSort>> ();
			this.display = new List<ColumnRef<EntityColumnDisplay>> ();
		}

		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				new XElement (Xml.SortColumns, this.sort.Select (x => x.Save (Xml.SortColumn))),
				new XElement (Xml.DisplayColumns, this.display.Select (x => x.Save (Xml.DisplayColumn))));
		}


		public IList<ColumnRef<EntityColumnSort>> Sort
		{
			get
			{
				return this.sort;
			}
		}

		public IList<ColumnRef<EntityColumnDisplay>> Display
		{
			get
			{
				return this.display;
			}
		}


		public static UserEntityTableSettings Restore(XElement xml)
		{
			var settings = new UserEntityTableSettings ();

			settings.sort.AddRange (xml.Element (Xml.SortColumns).Elements ().Select (x => ColumnRef.Restore<EntityColumnSort> (x)));
			settings.display.AddRange (xml.Element (Xml.DisplayColumns).Elements ().Select (x => ColumnRef.Restore<EntityColumnDisplay> (x)));
			
			return settings;
		}


		private static class Xml
		{
			public const string					SortColumns    = "S";
			public const string					DisplayColumns = "D";
			
			public const string					SortColumn     = "s";
			public const string					DisplayColumn  = "d";
		}


		private readonly List<ColumnRef<EntityColumnSort>>	sort;
		private readonly List<ColumnRef<EntityColumnDisplay>>	display;
	}
}
