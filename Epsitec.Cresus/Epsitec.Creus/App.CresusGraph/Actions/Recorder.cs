//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Actions
{
	/// <summary>
	/// The <c>Recorder</c> class maintains a stack of <see cref="ActionRecord"/> items.
	/// </summary>
	public class Recorder : IEnumerable<ActionRecord>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Recorder"/> class.
		/// </summary>
		public Recorder()
		{
			this.actionRecords = new List<ActionRecord> ();
		}


		/// <summary>
		/// Gets the number of items stored in the recorder.
		/// </summary>
		/// <value>The number of items stored in the recorder.</value>
		public int Count
		{
			get
			{
				return this.actionRecords.Count;
			}
		}


		/// <summary>
		/// Saves the recorder contentes to a string.
		/// </summary>
		/// <returns>The recorder contents.</returns>
		public string SaveToString()
		{
			using (System.IO.StringWriter writer = new System.IO.StringWriter ())
			{
				this.Save (writer);
				return writer.ToString ();
			}
		}

		/// <summary>
		/// Restores the recorder contents from a string. A single <see cref="OnChanged"/>
		/// gets fired at the end of the restore.
		/// </summary>
		/// <param name="data">The data.</param>
		public void RestoreFromString(string data)
		{
			using (System.IO.StringReader reader = new System.IO.StringReader (data))
			{
				this.Restore (reader);
			}
		}

		/// <summary>
		/// Saves the recorder contents to the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public void Save(System.IO.TextWriter stream)
		{
			foreach (var item in this.actionRecords)
			{
				stream.WriteLine (item.ToString ());
			}
		}

		/// <summary>
		/// Restores the recorder contents from a stream. A single <see cref="OnChanged"/>
		/// gets fired at the end of the restore.
		/// </summary>
		/// <param name="stream">The stream.</param>
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


		/// <summary>
		/// Pushes the specified record onto the stack.
		/// </summary>
		/// <param name="record">The record.</param>
		/// <returns>The pushed record.</returns>
		public ActionRecord Push(ActionRecord record)
		{
			this.actionRecords.Add (record);

			this.OnChanged ();

			System.Diagnostics.Debug.WriteLine (record.ToString ());

			return record;
		}

		/// <summary>
		/// Pops the topmost record from the stack.
		/// </summary>
		/// <returns>The record (or an empty record if the stack is empty).</returns>
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
				this.OnChanged ();

				return record;
			}
		}

		/// <summary>
		/// Clears the stack.
		/// </summary>
		public void Clear()
		{
			if (this.actionRecords.Count > 0)
			{
				this.actionRecords.Clear ();
				this.OnChanged ();
			}
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


		/// <summary>
		/// Pushes the specified user action onto the stack.
		/// </summary>
		/// <param name="userAction">The user action.</param>
		/// <returns>The record.</returns>
		public static ActionRecord Push(Action userAction)
		{
			return Recorder.PushNew (new ActionRecord (userAction.Tag, ""));
		}

		/// <summary>
		/// Pushes the specified generic user action onto the stack.
		/// </summary>
		/// <typeparam name="T">Argument type.</typeparam>
		/// <param name="userAction">The user action.</param>
		/// <param name="arg">The argument.</param>
		/// <returns>The record.</returns>
		public static ActionRecord Push<T>(Action userAction, T arg)
		{
			return Recorder.PushNew (new ActionRecord (userAction.Tag, Recorder.Serialize (arg)));
		}

		public static ActionRecord Push<T1, T2>(Action userAction, T1 arg1, T2 arg2)
		{
			return Recorder.PushNew (new ActionRecord (userAction.Tag, Recorder.Serialize (arg1), Recorder.Serialize (arg2)));
		}


		/// <summary>
		/// Serializes the specified argument.
		/// </summary>
		/// <exception cref="System.ArgumentException">Throws an argument exception if the type is not supported.</exception>
		/// <param name="arg">The argument.</param>
		/// <returns>The serialized argument</returns>
		public static string Serialize(object arg)
		{
			if (arg is IEnumerable<int>)
			{
				return Recorder.Serialize ((IEnumerable<int>) arg);
			}
			if (arg is IEnumerable<string>)
            {
				return Recorder.Serialize ((IEnumerable<string>) arg);
            }
			if (arg is string)
            {
				return Recorder.Serialize ((string) arg);
            }
			if (arg is int)
            {
				return Recorder.Serialize ((int) arg);
            }

			throw new System.ArgumentException ();
		}

		/// <summary>
		/// Deserializes the specified argument.
		/// </summary>
		/// <exception cref="System.ArgumentException">Throws an argument exception if the type is not supported.</exception>
		/// <param name="arg">The argument.</param>
		/// <param name="type">The expected type.</param>
		/// <returns>The deserialized argument.</returns>
		public static object Deserialize(string arg, System.Type type)
		{
			if (type == typeof (IEnumerable<int>))
			{
				IEnumerable<int> result;
				Recorder.Deserialize (arg, out result);
				return result;
			}
			if (type ==	typeof (IEnumerable<string>))
            {
				IEnumerable<string> result;
				Recorder.Deserialize (arg, out result);
				return result;
            }
			if (type == typeof (string))
            {
				string result;
				Recorder.Deserialize (arg, out result);
				return result;
            }
			if (type == typeof (int))
			{
				int result;
				Recorder.Deserialize (arg, out result);
				return result;
			}

			throw new System.ArgumentException ();
		}


		/// <summary>
		/// Pushes a new action record onto the active undo recorder.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns>The action.</returns>
		private static ActionRecord PushNew(ActionRecord action)
		{
			var manager = UndoRedoManager.Active;

			manager.UndoRecorder.Push (action);
			manager.RedoRecorder.Clear ();

			return action;
		}


		private static string Serialize(IEnumerable<int> arg)
		{
			return string.Join (" ", arg.Select (x => x.ToString (System.Globalization.CultureInfo.InvariantCulture)).ToArray ());
		}

		private static string Serialize(IEnumerable<string> arg)
		{
			return string.Join (" ", arg.Select (x => Recorder.Escape (x)).ToArray ());
		}

		private static string Serialize(string arg)
		{
			return arg;
		}

		private static string Serialize(int arg)
		{
			return arg.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}

		
		private static void Deserialize(string arg, out IEnumerable<int> result)
		{
			result = arg.Length == 0 ? Enumerable.Empty<int> () : arg.Split (' ').Select (x => int.Parse (x, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture)).ToArray ();
		}

		private static void Deserialize(string arg, out IEnumerable<string> result)
		{
			result = arg.Length == 0 ? Enumerable.Empty<string> () : arg.Split (' ').Select (x => Recorder.Unescape (x)).ToArray ();
		}

		private static void Deserialize(string arg, out string result)
		{
			result = arg;
		}

		private static void Deserialize(string arg, out int result)
		{
			result = int.Parse (arg, System.Globalization.CultureInfo.InvariantCulture);
		}

		
		private static string Escape(string text)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (char c in text)
			{
				switch (c)
				{
					case '\\':
						buffer.Append ("\\\\");
						break;

					case ' ':
						buffer.Append ("\\_");
						break;

					default:
						buffer.Append (c);
						break;

				}
			}

			return buffer.ToString ();
		}

		private static string Unescape(string text)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			bool escape = false;

			foreach (char c in text)
			{
				if (escape)
				{
					switch (c)
					{
						case '\\':
							buffer.Append ('\\');
							break;

						case '_':
							buffer.Append (' ');
							break;

						default:
							throw new System.InvalidOperationException ("Unexpected escape sequence \\" + escape);
					}

					escape = false;
				}
				else if (c == '\\')
				{
					escape = true;
				}
				else
				{
					buffer.Append (c);
				}
			}

			return buffer.ToString ();
		}


		private void OnChanged()
		{
			var handler = this.Changed;

			if (handler != null)
			{
				handler (this);
			}
		}

		
		public event EventHandler				Changed;
		
		private readonly List<ActionRecord>		actionRecords;
	}
}
