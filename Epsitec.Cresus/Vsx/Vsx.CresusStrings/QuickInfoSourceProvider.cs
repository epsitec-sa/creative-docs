using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ResourceManagement;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
//using Roslyn.Compilers;
//using Roslyn.Services.Host;

namespace Epsitec.Cresus.Strings
{
	[Export (typeof (IQuickInfoSourceProvider))]
	[Name ("Cresus Strings Source")]
	[Order (Before = "default")]
	[ContentType ("CSharp")]
	internal class QuickInfoSourceProvider : IQuickInfoSourceProvider
	{
		public QuickInfoSourceProvider()
		{
			using (new TimeTrace ())
			{
			}
		}

		[Import]
		internal Epsitec.VisualStudio.ResourceSymbolInfoProvider ResourceSymbolInfoProvider
		{
			get;
			set;
		}

		public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
		{
			using (new TimeTrace ())
			{
				this.ResourceSymbolInfoProvider.ActiveTextBuffer = textBuffer;
				return new QuickInfoSource (this, textBuffer);
			}
		}
	}
}
