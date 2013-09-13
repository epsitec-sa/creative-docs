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
	[Order (Before = "Default Quick Info Presenter")]
	[ContentType ("CSharp")]
	internal class QuickInfoSourceProvider : IQuickInfoSourceProvider
	{
		public QuickInfoSourceProvider()
		{
			Trace.WriteLine ("QuickInfoSourceProvider()");
		}

		[Import]
		internal Epsitec.VisualStudio.ResourceSymbolInfoProvider ResourceSymbolInfoProvider
		{
			get;
			set;
		}

		[Import]
		internal ITextStructureNavigatorSelectorService NavigatorService
		{
			get;
			set;
		}

		[Import]
		internal ITextBufferFactoryService TextBufferFactoryService
		{
			get;
			set;
		}

		public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
		{
			//Trace.WriteLine (System.Reflection.Assembly.GetExecutingAssembly ().Location);
			using (new TimeTrace ("TryCreateQuickInfoSource"))
			{
				this.ResourceSymbolInfoProvider.DocumentSource.TextBuffer = textBuffer;
				return new QuickInfoSource (this, textBuffer);
			}
		}
	}
}
