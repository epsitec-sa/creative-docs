using System;
using System.Collections.Generic;
using System.Text;

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

		public struct Record
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
				this.text = null;
				this.data = data;
				this.stream = null;
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

		public Record GetRecord(string id)
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

		public void CopyFromSystemClipboard()
		{
			this.list.Clear ();

			System.Windows.Forms.IDataObject ido = System.Windows.Forms.Clipboard.GetDataObject ();
			
			foreach (string format in ido.GetFormats (false))
			{
				object data = ido.GetData (format);
				this.list.Add (new Record (format, data));
			}

			foreach (string format in ido.GetFormats (true))
			{
				if (!this.Contains (format))
				{
					object data = ido.GetData (format, true);
					this.list.Add (new Record (format, data));
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
