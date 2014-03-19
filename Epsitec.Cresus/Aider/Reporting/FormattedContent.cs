//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Reporting
{
	public class FormattedContent : IContent
	{
		public FormattedContent(params string[] args)
		{
			this.Define (args);
		}

		public FormattedContent(IEnumerable<string> args)
		{
			this.Define (args);
		}

		
		public IList<string>					Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		
		public void Clear()
		{
			this.arguments = null;
		}

		public void Define(params string[] args)
		{
			this.arguments = args.Length == 0 ? null : args;
		}

		public void Define(IEnumerable<string> args)
		{
			if (args == null)
			{
				this.Clear ();
			}
			else
			{
				this.Define (args.ToArray ());
			}
		}


		#region IContent Members

		public string Format
		{
			get
			{
				return "FormattedContent";
			}
		}

		public byte[] GetContentBlob()
		{
			if ((this.arguments == null) ||
				(this.arguments.Length == 0))
			{
				return null;
			}

			var buffer = new System.Text.StringBuilder ();

			foreach (var arg in this.arguments)
			{
				buffer.Append (FormattedContent.separator);
				buffer.Append (arg);
			}
			
			return System.Text.Encoding.UTF8.GetBytes (buffer.ToString ());
		}

		public FormattedText GetContentText(string template)
		{
			return new FormattedText (string.Format (template, this.arguments));
		}

		public IContent Setup(byte[] blob)
		{
			if ((blob == null) ||
				(blob.Length == 0))
			{
				this.arguments = null;
			}
			else
			{
				var source = System.Text.Encoding.UTF8.GetString (blob);
				var sep = source[0];

				this.arguments = source.Substring (1).Split (sep);
			}

			return this;
		}

		#endregion


		public static FormattedContent Escape(params string[] args)
		{
			return new FormattedContent (args.Select (x => FormattedText.Escape (x)));
		}
		
		
		private string[]						arguments;
		private const char						separator = (char) 0;
	}
}
