//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Actions
{
	public class Recorder : IEnumerable<ActionRecord>
	{
		public Recorder()
		{
			this.actionRecords = new List<ActionRecord> ();
		}


		public int Count
		{
			get
			{
				return this.actionRecords.Count;
			}
		}

		
		public void Save(System.IO.TextWriter stream)
		{
			foreach (var item in this.actionRecords)
			{
				stream.WriteLine (item.ToString ());
			}
		}

		public void Restore(System.IO.TextReader stream)
		{
			this.actionRecords.Clear ();

			while (true)
			{
				string line = stream.ReadLine ();

				if (line == null)
				{
					break;
				}

				this.actionRecords.Add (ActionRecord.Parse (line));
			}

			this.OnChanged ();
		}


		public ActionRecord Push(ActionRecord record)
		{
			this.actionRecords.Add (record);

			this.OnRecordPushed ();
			this.OnChanged ();

			System.Diagnostics.Debug.WriteLine (record.ToString ());

			return record;
		}

		private ActionRecord PushNewAction(ActionRecord record)
		{
			this.Push (record);
			
			this.OnActionCreated ();

			return record;
		}

		public ActionRecord Pop()
		{
			int index = this.actionRecords.Count - 1;
			
			if (index < 0)
			{
				return new ActionRecord ();
			}
			else
			{
				var record = this.actionRecords[index];
				this.actionRecords.RemoveAt (index);
				
				this.OnRecordPopped ();
				this.OnChanged ();

				return record;
			}
		}

		public void Clear()
		{
			this.actionRecords.Clear ();
			this.OnChanged ();
		}


		#region IEnumerable<ActionRecord> Members

		public IEnumerator<ActionRecord> GetEnumerator()
		{
			return this.actionRecords.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.actionRecords.GetEnumerator ();
		}

		#endregion
		
		
		public static ActionRecord Push(Action userAction)
		{
			return GraphProgram.Application.Document.Recorder.PushNewAction (new ActionRecord (userAction.Tag, ""));
		}

		public static ActionRecord Push<T>(Action userAction, T arg)
		{
			return GraphProgram.Application.Document.Recorder.PushNewAction (new ActionRecord (userAction.Tag, Recorder.Serialize (arg)));
		}
		
		
		public static string Serialize(object arg)
		{
			if (arg is IEnumerable<int>)
			{
				return Recorder.Serialize ((IEnumerable<int>) arg);
			}

			throw new System.ArgumentException ();
		}

		public static object Deserialize(string arg, System.Type type)
		{
			if (type == typeof (IEnumerable<int>))
			{
				IEnumerable<int> result;
				Recorder.Deserialize (arg, out result);
				return result;
			}

			throw new System.ArgumentException ();
		}

		
		private static string Serialize(IEnumerable<int> arg)
		{
			return string.Join (" ", arg.Select (x => x.ToString (System.Globalization.CultureInfo.InvariantCulture)).ToArray ());
		}

		private static void Deserialize(string arg, out IEnumerable<int> result)
		{
			result = arg.Length == 0 ? Enumerable.Empty<int> () : arg.Split (' ').Select (x => int.Parse (x, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture)).ToArray ();
		}


		private void OnRecordPushed()
		{
			var handler = this.RecordPushed;

			if (handler != null)
			{
				handler (this);
			}
		}

		private void OnRecordPopped()
		{
			var handler = this.RecordPopped;

			if (handler != null)
			{
				handler (this);
			}
		}

		private void OnChanged()
		{
			var handler = this.Changed;

			if (handler != null)
			{
				handler (this);
			}
		}

		private void OnActionCreated()
		{
			var handler = this.ActionCreated;

			if (handler != null)
			{
				handler (this);
			}
		}

		public event EventHandler				RecordPushed;
		public event EventHandler				RecordPopped;
		public event EventHandler				Changed;
		public event EventHandler				ActionCreated;
		
		private readonly List<ActionRecord> actionRecords;

	}
}
