//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library.Business;

namespace Epsitec.Cresus.DebugViewer.Data
{
	public class LogRecord
	{
		public LogRecord(string path)
		{
			this.path = System.IO.Path.GetFileName (path);

			var args = this.path.Split ('.');

			this.timeStamp  = new System.DateTime (long.Parse (args[0]));
			this.eventType  = LogRecord.GetEventType (args[1]);
			this.recordType = LogRecord.GetRecordType (args[2]);
		}


		public System.DateTime					TimeStamp
		{
			get
			{
				return this.timeStamp;
			}
		}

		public LogRecordType					RecordType
		{
			get
			{
				return this.recordType;
			}
		}

		public LogEventType						EventType
		{
			get
			{
				return this.eventType;
			}
		}

		public string GetMessage(string folderPath)
		{
			if (this.recordType == LogRecordType.Message)
			{
				return System.IO.File.ReadAllText (System.IO.Path.Combine (folderPath, this.path), System.Text.Encoding.Default);
			}
			else
			{
				return null;
			}
		}

		public Epsitec.Common.Drawing.Image GetImage(string folderPath)
		{
			if (this.recordType == LogRecordType.Image)
			{
				return Epsitec.Common.Drawing.Bitmap.FromFile (System.IO.Path.Combine (folderPath, this.path));
			}
			else
			{
				return null;
			}
		}


		private static LogRecordType GetRecordType(string ext)
		{
			switch (ext)
			{
				case "txt":
					return LogRecordType.Message;

				case "jpg":
					return LogRecordType.Image;

				default:
					return LogRecordType.None;
			}
		}

		private static LogEventType GetEventType(string value)
		{
			switch (value)
			{
				case CoreSnapshotService.Keyword.SET_DB_FIELD:
					return LogEventType.SetDbField;
				case CoreSnapshotService.Keyword.SET_LIVE_FIELD:
					return LogEventType.SetLiveField;
				case CoreSnapshotService.Keyword.WIDGET_FOCUS:
					return LogEventType.WidgetFocus;
				case CoreSnapshotService.Keyword.WINDOW_SHOW:
					return LogEventType.WindowShow;
				case CoreSnapshotService.Keyword.MOUSE_DOWN:
					return LogEventType.MouseDown;
				case CoreSnapshotService.Keyword.WINDOW_FOCUS:
					return LogEventType.WindowFocus;
				case CoreSnapshotService.Keyword.NAV:
					return LogEventType.Nav;
				case CoreSnapshotService.Keyword.USER_MESSAGE:
					return LogEventType.UserMessage;
				case CoreSnapshotService.Keyword.CMD:
					return LogEventType.Cmd;
				case CoreSnapshotService.Keyword.TRACE:
					return LogEventType.Trace;

				default:
					return LogEventType.None;
			}
		}

		private readonly string					path;
		private readonly System.DateTime		timeStamp;
		private readonly LogRecordType			recordType;
		private readonly LogEventType			eventType;
	}
}