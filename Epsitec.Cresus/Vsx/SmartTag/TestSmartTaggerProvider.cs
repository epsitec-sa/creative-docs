using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
	[ContentType ("text")]
	[Order (Before = "default")]
	[TagType (typeof (SmartTag))]
	internal class TestSmartTaggerProvider : IViewTaggerProvider
	{
		public TestSmartTaggerProvider()
		{
		}

		[Import (typeof (ITextStructureNavigatorSelectorService))]
		internal ITextStructureNavigatorSelectorService NavigatorService
		{
			get;
			set;
		}

		#region IViewTaggerProvider Members

		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
		{
			if (buffer == null || textView == null)
			{
				return null;
			}

			//make sure we are tagging only the top buffer 
			if (buffer == textView.TextBuffer)
			{
				return new TestSmartTagger (buffer, textView, this) as ITagger<T>;
			}
			else
				return null;
		}

		#endregion
	}
}
