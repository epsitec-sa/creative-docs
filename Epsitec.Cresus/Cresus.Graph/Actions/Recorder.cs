//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Actions
{
	public class Recorder
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

				this.Push (ActionRecord.Parse (line));
			}
		}


		public ActionRecord Push(ActionRecord record)
		{
			this.actionRecords.Add (record);

			System.Diagnostics.Debug.WriteLine (record.ToString ());
			
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
				return record;
			}
		}

		
		
		
		public static Recorder Current
		{
			get
			{
				return Recorder.current;
			}
		}


		public static ActionRecord Push(Action userAction)
		{
			return Recorder.Current.Push (new ActionRecord (userAction.Tag, ""));
		}

		public static ActionRecord Push<T>(Action userAction, T arg)
		{
			return Recorder.Current.Push (new ActionRecord (userAction.Tag, Recorder.Serialize (arg)));
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

		
		
		private static Recorder current = new Recorder ();
		
		private readonly List<ActionRecord> actionRecords;
		
	}
}
