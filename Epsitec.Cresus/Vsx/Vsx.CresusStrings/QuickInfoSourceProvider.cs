using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Strings.Bundles;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using Roslyn.Compilers;
using Roslyn.Services;
using Roslyn.Services.Host;

namespace Epsitec.Cresus.Strings
{
	[Export (typeof (IQuickInfoSourceProvider))]
	[Name ("Cresus Strings Source")]
	[Order (Before = "Default Quick Info Presenter")]
	[ContentType ("CSharp")]
	internal class QuickInfoSourceProvider : IQuickInfoSourceProvider
	{
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

		[Import]
		internal SolutionResources BundleManager
		{
			get;
			set;
		}

		public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
		{
			//Trace.WriteLine (System.Reflection.Assembly.GetExecutingAssembly ().Location);
			using (new TimeTrace ("TryCreateQuickInfoSource"))
			{
				var workspace = Workspace.PrimaryWorkspace;
				var solution = workspace.CurrentSolution;
				var newWorkspace = Workspace.LoadSolution (solution.FilePath, enableFileTracking: true);
				var newSolution = newWorkspace.CurrentSolution;

				//var eq1 = workspace == newWorkspace;
				//var eq2 = Workspace.PrimaryWorkspace == newWorkspace;
				//var eq3 = solution == newSolution;
				//var eq4 = Workspace.PrimaryWorkspace.CurrentSolution == newSolution;
	
				return new QuickInfoSource (this, textBuffer, newSolution);
			}
		}
	}
}
