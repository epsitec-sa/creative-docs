//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.App
{
	public sealed class ChunkSourceProcessor
	{
		public ChunkSourceProcessor(string[] lines)
		{
			this.lines = lines;
			this.index = 0;
		}


		public bool EndReached
		{
			get
			{
				return this.GetCurrentLine ().Length == 0;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.index;
			}
		}


		public string GetNextLine(string prefix, bool skip = false)
		{
			var buffer = new System.Text.StringBuilder ();

			while (true)
			{
				var line = this.GetCurrentLine ();

				if (line.Length == 0)
				{
					break;
				}

				if (line.StartsWith (prefix))
				{
					if (buffer.Length > 0)
					{
						buffer.Append ("\n");
					}

					buffer.Append (line.Substring (prefix.Length));
					this.index++;
				}
				else if (skip)
				{
					this.index++;
				}
				else
				{
					break;
				}
			}

			return buffer.ToString ();
		}

		private string GetCurrentLine()
		{
			while (this.index < this.lines.Length)
			{
				var line = this.lines[this.index];

				if ((line.Length == 0) ||
						(line.StartsWith ("#")) ||
						(line.StartsWith ("//")))
				{
					this.index++;
					continue;
				}

				return line;
			}

			return "";
		}

		private int index;
		private string[] lines;
	}
}
