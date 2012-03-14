//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.DebugViewer.Accessors
{
	public class LogDataAccessor
	{
		public LogDataAccessor(string folderPath)
		{
			this.folderPath = folderPath;
			
			this.messageRecords = new List<Data.LogRecord> ();
			this.imageRecords   = new List<Data.LogRecord> ();

			this.messageRecords.AddRange (from rec in this.GetLogFileNames ().Select (x => new Data.LogRecord (x))
										  where rec.RecordType == Data.LogRecordType.Message
										  orderby rec.TimeStamp ascending
										  select rec);
			
			this.imageRecords.AddRange (from rec in this.GetLogFileNames ().Select (x => new Data.LogRecord (x))
										where rec.RecordType == Data.LogRecordType.Image
										orderby rec.TimeStamp ascending
										select rec);
		}


		public IList<Data.LogRecord> Messages
		{
			get
			{
				return this.messageRecords;
			}
		}

		public IList<Data.LogRecord> Images
		{
			get
			{
				return this.imageRecords;
			}
		}

		public FormattedText GetMessage(Data.LogRecord record)
		{
			if (record == null)
			{
				return FormattedText.Empty;
			}

			return FormattedText.FromSimpleText (record.GetMessage (this.folderPath));
		}

		public StaticImage GetStaticImage(Data.LogRecord record)
		{
			if (record == null)
			{
				return null;
			}

			var bitmap = record.GetImage (this.folderPath);

			return new StaticImage ()
			{
				Image = bitmap,
				MinSize = bitmap.Size,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
			};
		}


		private IEnumerable<string> GetLogFileNames()
		{
			return System.IO.Directory.EnumerateFiles (this.folderPath, "*.*.*", System.IO.SearchOption.TopDirectoryOnly);
		}

		private readonly string folderPath;
		private readonly List<Data.LogRecord> messageRecords;
		private readonly List<Data.LogRecord> imageRecords;
	}
}
