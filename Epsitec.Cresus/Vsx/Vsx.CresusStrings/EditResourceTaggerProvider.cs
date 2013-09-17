using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Epsitec.Cresus.Strings
{
	[Export (typeof (IViewTaggerProvider))]
	[ContentType ("CSharp")]
	[Order (Before = "default")]
	[TagType (typeof (SmartTag))]
	internal class EditResourceTaggerProvider : IViewTaggerProvider
	{
		public EditResourceTaggerProvider()
		{
			Trace.WriteLine ("EditResourceTaggerProvider()");
		}

		[Import]
		internal Epsitec.VisualStudio.ResourceSymbolInfoProvider ResourceSymbolInfoProvider
		{
			get;
			set;
		}

		#region IViewTaggerProvider Members

		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
		{
			// make sure we are tagging only the top buffer
			if (textView != null && buffer == textView.TextBuffer)
			{
				return new EditResourceTagger (buffer, textView, this) as ITagger<T>;
			}
			return null;
		}

		#endregion
	}
}
