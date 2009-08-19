//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Actions
{
	public struct ActionRecord
	{
		public ActionRecord(string tag, string arg)
		{
			this.tag = tag;
			this.arg = arg;
		}


		public bool IsEmpty
		{
			get
			{
				return this.tag == null;
			}
		}
		
		
		public void PlayBack()
		{
			var action = Factory.Find (this.tag);

			if (action == null)
			{
				throw new System.InvalidOperationException ("Cannot play back action " + this.tag);
			}

			var type = action.ArgumentType;

			if (type == typeof (void))
			{
				action.DynamicInvoke ();
			}
			else
			{
				action.DynamicInvoke (Recorder.Deserialize (this.arg, type));
			}
		}

		
		public override string ToString()
		{
			return string.Concat (ActionRecord.Escape (this.tag), ">", ActionRecord.Escape (this.arg));
		}

		public static ActionRecord Parse(string s)
		{
			string[] args = s.Split ('>');
			
			if (args.Length != 2)
			{
				throw new System.ArgumentException ("Invalid ActionRecord syntax");
			}

			return new ActionRecord (ActionRecord.Unescape (args[0]), ActionRecord.Unescape (args[1]));
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
		private readonly string arg;
	}
}
