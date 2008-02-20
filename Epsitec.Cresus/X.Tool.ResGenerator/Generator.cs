//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Tool.ResGenerator
{
	/// <summary>
	/// The <c>Generator</c> class is used to produce block indented source
	/// code.
	/// </summary>
	class Generator
	{
		public Generator(System.Text.StringBuilder buffer)
		{
			this.buffer    = buffer;
			this.tabCount = 0;
		}

		/// <summary>
		/// Gets a string with tabs for the current indentation level.
		/// </summary>
		/// <value>A string filled with tabs.</value>
		public string							Tabs
		{
			get
			{
				return new string ('\t', this.tabCount);
			}
		}

		/// <summary>
		/// Begins a block which starts with <c>prefix Name {</c>.
		/// </summary>
		/// <param name="prefix">The prefix.</param>
		/// <param name="name">The name of the block.</param>
		public void BeginBlock(string prefix, string name)
		{
			if (char.IsLower (name[0]))
			{
				name = char.ToUpper (name[0]) + name.Substring (1);
			}

			this.buffer.Append (this.Tabs);
			this.buffer.Append (prefix);
			this.buffer.Append (" ");
			this.buffer.Append (name);
			this.buffer.Append ("\n");

			this.buffer.Append (this.Tabs);
			this.buffer.Append ("{\n");

			this.tabCount++;
		}

		/// <summary>
		/// Ends a block. Closes the opened bracket.
		/// </summary>
		public void EndBlock()
		{
			this.tabCount--;

			this.buffer.Append (this.Tabs);
			this.buffer.Append ("}\n");
		}


		private System.Text.StringBuilder		buffer;
		private int								tabCount;
	}
}
