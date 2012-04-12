//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.DebugViewer.Accessors
{
	public class LogFolderDataAccessor : IItemDataProvider<Data.LogFolderRecord>, IItemDataMapper<Data.LogFolderRecord>
	{
		public LogFolderDataAccessor(string rootPath)
		{
			this.rootPath = rootPath;
			this.folderRecords = new List<Data.LogFolderRecord> ();

			this.folderRecords.AddRange (from rec in this.GetFolderFileNames ().Select (x => new Data.LogFolderRecord (x))
										 orderby rec.TimeStamp ascending
										 select rec);
		}

		public FormattedText GetMessage(Data.LogFolderRecord record)
		{
			if (record == null)
			{
				return FormattedText.Empty;
			}

			return FormattedText.FromSimpleText (record.GetMessage (this.rootPath).Trim ());
		}

		private IEnumerable<string> GetFolderFileNames()
		{
			return System.IO.Directory.EnumerateDirectories (this.rootPath, "*@*-*.*", System.IO.SearchOption.TopDirectoryOnly);
		}

		#region IItemDataMapper<LogFolderRecord> Members

		public ItemData<Data.LogFolderRecord> Map(Data.LogFolderRecord value)
		{
			return new ItemData<Data.LogFolderRecord> (value,
				new ItemState
				{
					Height        = 18,
					PaddingBefore = 4,
					PaddingAfter  = 4,
					MarginBefore  = 0,
					MarginAfter   = 0,
				});
		}

		#endregion

		#region IItemDataProvider<LogFolderRecord> Members

		public bool Resolve(int index, out Data.LogFolderRecord value)
		{
			if ((index < 0) ||
					(index >= this.folderRecords.Count))
			{
				value = null;
				return false;
			}

			value = this.folderRecords[index];

			return true;
		}

		#endregion

		#region IItemDataProvider Members

		public int Count
		{
			get
			{
				return this.folderRecords.Count;
			}
		}

		#endregion


		private readonly string rootPath;
		private readonly List<Data.LogFolderRecord> folderRecords;
	}
}
