//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Xml.Linq;

namespace Epsitec.Common.Support
{
	public class XmlExtractor
	{
		public XmlExtractor()
		{
			this.buffer = new System.Text.StringBuilder ();
		}


		public bool Finished
		{
			get
			{
				return this.depth == 0;
			}
		}
		
		public void Append(string line)
		{
			char prev = ' ';

			foreach (char c in line)
			{
				if ((this.depth == 0) &&
					(this.open == 0))
				{
					break;
				}
				
				if (c == '<')
				{
					this.open++;
				}
				else if (c == '>')
				{
					this.open--;

					if (prev == '/')			//	"<xxx/>"
					{							//	     ^
						this.depth--;
					}
				}
				else if (c == '/')
				{
					if (prev == '<')			//	"</xxx>"
					{							//	  ^
						this.depth--;
					}
				}
				else if (prev == '<')			//	"<xxx.."
				{								//	  ^
					if (this.depth == -1)
					{
						this.depth = 0;
					}
					this.depth++;
				}

				prev = c;

				this.buffer.Append (c);
			}
		}



		private readonly System.Text.StringBuilder buffer;
		private int depth;
		private int open;
	}
}
