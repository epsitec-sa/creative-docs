using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Cresus.Strings.ViewModels;
using Epsitec.Cresus.Strings.Views;
using Epsitec.VisualStudio;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

// Properties / Debug / Command Line Arguments
//		/rootsuffix Exp "..\..\..\..\App.CresusGraph\App.CresusGraph.sln"
//		/rootsuffix Exp "..\..\..\..\Epsitec.Cresus.sln"
//		/rootsuffix Exp "..\..\..\Vsx.CresusStrings.Sample1\Vsx.CresusStrings.Sample1.sln"
namespace Epsitec.Cresus.Strings
{
	internal class QuickInfoSource : IQuickInfoSource
	{
		public QuickInfoSource(QuickInfoSourceProvider provider, ITextBuffer subjectBuffer)
		{
			using (new TimeTrace ())
			{
				this.provider = provider;
				this.subjectBuffer = subjectBuffer;
			}
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				Trace.WriteLine ("### DISPOSE");
				this.isDisposed = true;
			}
		}

		public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan)
		{
			applicableToSpan = null;

			try
			{
				// Map the trigger point down to our buffer.
				SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint (this.subjectBuffer.CurrentSnapshot);

				if (subjectTriggerPoint.HasValue)
				{
					var point = subjectTriggerPoint.Value;
					var cts = new CancellationTokenSource (QuickInfoSource.Timeout(Config.MaxQuickInfoDelay));
					try
					{
						var symbolInfoTask = this.Engine.GetResourceSymbolInfoAsync (point, cts.Token);
						symbolInfoTask.Wait (cts.Token);
						var symbolInfo = symbolInfoTask.Result;
						if (symbolInfo != null)
						{
							var content = QuickInfoSource.CreateQiView (symbolInfo);
							if (content != null)
							{
								var textSpan = symbolInfo.SyntaxToken.Span;
								var span = Span.FromBounds (textSpan.Start, textSpan.End);
								applicableToSpan = point.Snapshot.CreateTrackingSpan (span, SpanTrackingMode.EdgeInclusive);

								qiContent.Add (content);
							}
						}
					}
					catch(OperationCanceledException)
					{
						var span = Span.FromBounds (point.Position, point.Position);
						applicableToSpan = point.Snapshot.CreateTrackingSpan (span, SpanTrackingMode.EdgeInclusive);
						QuickInfoSource.SetQiPending ("Cresus Strings", qiContent);
					}
				}
			}
			catch (AggregateException e)
			{
				QuickInfoSource.AddQiException (qiContent, e);
				foreach (var ie in e.InnerExceptions)
				{
					QuickInfoSource.AddQiException (qiContent, ie, "  ");
				}
			}
			catch (Exception e)
			{
				QuickInfoSource.AddQiException (qiContent, e);
			}
		}

		private static int Timeout(int timeout)
		{
			return Debugger.IsAttached ? -1 : timeout;
		}

		private static UserControl CreateQiView(ResourceSymbolInfo info)
		{
			var resources = info.Resources;
			var count = resources.Count;
			if (count > 0)
			{
				if (count == 1)
				{
					return new MultiCultureResourceItemView (resources.First ());
				}
				else
				{
					return new MultiCultureResourceItemCollectionView (resources)
					{
						MaxHeight = 600
					};
				}
			}
			return null;
		}

		private static void SetQiPending(string subject, IList<object> qiContent)
		{
			QuickInfoSource.AddQiContent (qiContent, string.Format("({0} cache is still being constructed. Please try again in a few seconds...)", subject));
			Trace.WriteLine ("CANCELING");
		}

		private static void AddQiException(IList<object> qiContent, Exception e, string prefix = "Cresus Strings Extension Exception\n")
		{
			string message = string.Format ("{0}{1} : {2}", prefix, e.GetType ().Name, e.Message);
			QuickInfoSource.AddQiContent (qiContent, message);
		}

		private static void AddQiContent(IList<object> qiContent, string message)
		{
			Trace.WriteLine (message);
			qiContent.Add (message);
		}

		private Epsitec.VisualStudio.Engine Engine
		{
			get
			{
				return this.provider.EngineSource.Engine;
			}
		}


		private readonly QuickInfoSourceProvider provider;
		private readonly ITextBuffer subjectBuffer;

		private bool isDisposed;
	}
}
