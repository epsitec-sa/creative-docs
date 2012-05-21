//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Text.Exchange
{
	public class ClipboardData
	{
		public void Clear()
		{
			this.list.Clear ();
		}
		
		public void Add(string id, string text)
		{
			this.list.Add (new Record (id, text));
		}

		public void Add(string id, System.IO.MemoryStream stream)
		{
			this.list.Add (new Record (id, stream));
		}

		public void Add(string id, object data)
		{
			this.list.Add (new Record (id, data));
		}

		public void CopyToSystemClipboard()
		{
			System.Windows.Forms.IDataObject data = new System.Windows.Forms.DataObject ();

			foreach (Record record in this.list)
			{
				if (record.Text != null)
				{
					data.SetData (record.Id, true, record.Text);
					this.DebugDump (record.Id, record.Text);
				}
				else if (record.Stream != null)
				{
					data.SetData (record.Id, true, record.Stream);
					this.DebugDump (record.Id, record.Stream);
				}
				else if (record.Data != null)
				{
					data.SetData (record.Id, false, record.Data);
					this.DebugDump (record.Id, record.Data.ToString ());
				}
			}

			System.Windows.Forms.Clipboard.SetDataObject (data, true);
		}

		public bool Contains(string id)
		{
			foreach (Record record in this.list)
			{
				if (record.Id == id)
				{
					return true;
				}
			}
			
			return false;
		}

		public void CopyFromSystemClipboard()
		{
			this.list.Clear ();

			System.Windows.Forms.IDataObject ido = System.Windows.Forms.Clipboard.GetDataObject ();
			List<string> blackList = new List<string> () { "EnhancedMetafile" };
			
			foreach (string format in ido.GetFormats (false))
			{
				if (!blackList.Contains (format))
				{
					try
					{
						object data = ido.GetData (format);
						this.list.Add (new Record (format, data));
					}
					catch
					{
						//	Ignore formats which produce exceptions.
					}
				}
			}

			foreach (string format in ido.GetFormats (true))
			{
				if (!blackList.Contains (format))
				{
					try
					{
						if (!this.Contains (format))
						{
							object data = ido.GetData (format, true);
							this.list.Add (new Record (format, data));
						}
					}
					catch
					{
						//	Ignore formats which produce exceptions.
					}
				}
			}
		}

		public string GetDataText()
		{
			string text;

			text = this.GetData ("System.Text") as string;

			if (text != null)
			{
				return text;
			}

			text = this.GetData (System.Windows.Forms.DataFormats.UnicodeText) as string;

			if (text != null)
			{
				return text;
			}
			
			text = this.GetData (System.Windows.Forms.DataFormats.Text) as string;

			if (text != null)
			{
				return text;
			}

			return null;
		}

		public string GetFormattedText()
		{
			Internal.FormattedText text = this.GetData (Internal.FormattedText.ClipboardFormat.Name) as Internal.FormattedText;

			if (text == null)
			{
				return null;
			}
			else
			{
				return text.EncodedText;
			}
		}

		public object GetData(string id)
		{
			Record record = this.GetRecord (id);

			if (record.Text != null)
			{
				return record.Text;
			}
			else if (record.Stream != null)
			{
				return record.Stream;
			}
			else if (record.Data != null)
			{
				return record.Data;
			}
			else
			{
				return null;
			}
		}

		public void SetFormattedText(string text)
		{
			Internal.FormattedText formattedText = new Epsitec.Common.Text.Exchange.Internal.FormattedText (text);

			this.Add (Internal.FormattedText.ClipboardFormat.Name, formattedText);
		}

		#region Record Structure

		private struct Record
		{
			internal Record(string id, string text)
			{
				this.id = id;
				this.text = text;
				this.data = null;
				this.stream = null;
			}

			internal Record(string id, object data)
			{
				this.id = id;
				this.text = data as string;
				this.stream = data as System.IO.MemoryStream;
				this.data = (this.text == null) && (this.stream == null) ? data : null;
			}

			internal Record(string id, System.IO.MemoryStream stream)
			{
				this.id = id;
				this.text = null;
				this.data = null;
				this.stream = stream;
			}

			public string Id
			{
				get
				{
					return this.id;
				}
			}

			public string Text
			{
				get
				{
					return this.text;
				}
			}

			public object Data
			{
				get
				{
					return this.data;
				}
			}

			public System.IO.MemoryStream Stream
			{
				get
				{
					return this.stream;
				}
			}

			private string id;
			private object data;
			private string text;
			private System.IO.MemoryStream stream;
		}

		#endregion

		private Record GetRecord(string id)
		{
			foreach (Record record in this.list)
			{
				if (record.Id == id)
				{
					return record;
				}
			}

			return new Record ();
		}

		
		private void DebugDump(string id, System.IO.MemoryStream stream)
		{
			if (Epsitec.Common.Support.Globals.IsDebugBuild)
			{
				try
				{
					string temp = System.IO.Path.GetTempPath ();
					string name = "Clipboard " + id + ".txt";
					System.IO.File.WriteAllBytes (System.IO.Path.Combine (temp, name), stream.ToArray ());
				}
				catch
				{
				}
			}
		}

		private void DebugDump(string id, string text)
		{
			if (Epsitec.Common.Support.Globals.IsDebugBuild)
			{
				try
				{
					string temp = System.IO.Path.GetTempPath ();
					string name = "Clipboard " + id + ".txt";
					System.IO.File.WriteAllText (System.IO.Path.Combine (temp, name), text);
				}
				catch
				{
				}
			}
		}

		List<Record> list = new List<Record> ();
	}
}
