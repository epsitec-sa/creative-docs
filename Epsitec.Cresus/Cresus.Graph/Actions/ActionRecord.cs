//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Actions
{
	/// <summary>
	/// The <c>ActionRecord</c> structure represents a single action in a
	/// serialized state.
	/// </summary>
	public struct ActionRecord
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ActionRecord"/> struct.
		/// </summary>
		/// <param name="tag">The action tag.</param>
		/// <param name="arg">The argument (if any).</param>
		public ActionRecord(string tag, string arg)
		{
			this.tag  = tag;
			this.arg1 = arg;
			this.arg2 = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionRecord"/> struct.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <param name="arg1">The first argument.</param>
		/// <param name="arg2">The second argument.</param>
		public ActionRecord(string tag, string arg1, string arg2)
		{
			this.tag  = tag;
			this.arg1 = arg1 ?? "";
			this.arg2 = arg2 ?? "";
		}


		/// <summary>
		/// Gets a value indicating whether this instance is empty.
		/// </summary>
		/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get
			{
				return this.tag == null;
			}
		}


		/// <summary>
		/// Plays back the action: deserializes the action and invokes it with its
		/// associated argument.
		/// </summary>
		public void PlayBack()
		{
			var action = Factory.Find (this.tag);

			if (action == null)
			{
				throw new System.InvalidOperationException ("Cannot play back action " + this.tag);
			}

			var type1 = action.ArgumentType1;

			if (type1 == typeof (void))
			{
				action.DynamicInvoke ();
				
				return;
			}

			var type2 = action.ArgumentType2;

			if (type2 == typeof (void))
			{
				action.DynamicInvoke (Recorder.Deserialize (this.arg1, type1));
			}
			else
			{
				action.DynamicInvoke (Recorder.Deserialize (this.arg1, type1),
									  Recorder.Deserialize (this.arg2, type2));
			}
		}


		/// <summary>
		/// Returns a <see cref="string"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="string"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			if (this.arg2 == null)
			{
				return string.Concat (ActionRecord.Escape (this.tag), ">", ActionRecord.Escape (this.arg1));
			}
			else
			{
				return string.Concat (ActionRecord.Escape (this.tag), ">", ActionRecord.Escape (this.arg1), ">", ActionRecord.Escape (this.arg2));
			}
		}

		/// <summary>
		/// Parses the specified <see cref="string"/> and produces the corresponding
		/// <c>ActionRecord</c>.
		/// </summary>
		/// <param name="s">The serialized <c>ActionRecord</c>.</param>
		/// <returns>The <c>ActionRecord</c>.</returns>
		public static ActionRecord Parse(string s)
		{
			string[] args = s.Split ('>');

			switch (args.Length)
			{
				case 2:
					return new ActionRecord (ActionRecord.Unescape (args[0]), ActionRecord.Unescape (args[1]));

				case 3:
					return new ActionRecord (ActionRecord.Unescape (args[0]), ActionRecord.Unescape (args[1]), ActionRecord.Unescape (args[2]));

				default:
					throw new System.ArgumentException ("Invalid ActionRecord syntax");
			}
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

					case '>':
						buffer.Append ("\\x");
						break;

					case '\n':
						buffer.Append ("\\n");
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

						case 'x':
							buffer.Append ('>');
							break;
						
						case 'n':
							buffer.Append ('\n');
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

		
		private readonly string tag;
		private readonly string arg1;
		private readonly string arg2;
	}
}
