//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Aider.Reporting;

using System.Collections.Generic;
using System.Linq;

[assembly: ContentFormatter (typeof (FormattedContent))]

namespace Epsitec.Aider.Reporting
{
	public class FormattedContent : AbstractContent<FormattedContent>
	{
		public FormattedContent()
		{
		}
		
		public FormattedContent(params string[] args)
			: this ()
		{
			this.Define (args);
		}

		public FormattedContent(IEnumerable<string> args)
			: this ()
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

		public override byte[] GetContentBlob()
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

		public override IContentStore Setup(byte[] blob)
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

		#region IContentTextProducer Members

		public override FormattedText GetFormattedText(string template)
		{
			return new FormattedText (string.Format (template, this.arguments));
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
