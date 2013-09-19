using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Epsitec.Cresus.Strings
{
	[Export (typeof (IIntellisenseControllerProvider))]
	[Name ("Cresus Designer Strings Controller")]
	[ContentType ("CSharp")]
	internal class QuickInfoControllerProvider : IIntellisenseControllerProvider
	{
		public QuickInfoControllerProvider()
		{
			using (new TimeTrace ())
			{
			}
		}

		[Import]
		internal IQuickInfoBroker QuickInfoBroker
		{
			get;
			set;
		}

		public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
		{
			return new QuickInfoController (textView, subjectBuffers, this);
		}
	}
}
