using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Epsitec.VisualStudio;

namespace Epsitec
{
	public static partial class Extensions
	{
		public static IObservable<DteNotification> Watch(this EnvDTE80.DTE2 dte, DteChangeTypes changeTypes = DteChangeTypes.All)
		{
			return Observable.Create<DteNotification> (
				observer =>
				{
					var mergedEvents = Observable.Merge (dte.CreateObservableEvents (changeTypes));
					return mergedEvents.Subscribe (
						e => observer.OnNext (e),
						ex => observer.OnError (ex),
						() => observer.OnCompleted ());
				}
			);
		}

		public static IObservable<DteNotification> FromSolutionOpened(this EnvDTE80.DTE2 dte)
		{
			return Observable.Create<DteNotification>
			(
				observer =>
				{
					EnvDTE._dispSolutionEvents_OpenedEventHandler handler = () =>
					{
						observer.OnNext (new DteNotification (DteChangeTypes.SolutionOpened));
					};
					var events = dte.Events.SolutionEvents;
					events.Opened += handler;
					return () =>
					{
						events.Opened -= handler;
					};
				}
			);
		}

		public static IObservable<DteNotification> FromSolutionRenamed(this EnvDTE80.DTE2 dte)
		{
			return Observable.Create<DteNotification>
			(
				observer =>
				{
					EnvDTE._dispSolutionEvents_RenamedEventHandler handler = (string oldName) =>
					{
						observer.OnNext (new DteNotification (DteChangeTypes.SolutionRenamed, oldName));
					};
					var events = dte.Events.SolutionEvents;
					events.Renamed += handler;
					return () =>
					{
						events.Renamed -= handler;
					};
				}
			);
		}

		public static IObservable<DteNotification> FromSolutionBeforeClosing(this EnvDTE80.DTE2 dte)
		{
			return Observable.Create<DteNotification>
			(
				observer =>
				{
					EnvDTE._dispSolutionEvents_BeforeClosingEventHandler handler = () =>
					{
						observer.OnNext (new DteNotification (DteChangeTypes.SolutionBeforeClosing));
					};
					var events = dte.Events.SolutionEvents;
					events.BeforeClosing += handler;
					return () =>
					{
						events.BeforeClosing -= handler;
					};
				}
			);
		}

		public static IObservable<DteNotification> FromSolutionAfterClosing(this EnvDTE80.DTE2 dte)
		{
			return Observable.Create<DteNotification>
			(
				observer =>
				{
					EnvDTE._dispSolutionEvents_AfterClosingEventHandler handler = () =>
					{
						observer.OnNext (new DteNotification (DteChangeTypes.SolutionAfterClosing));
					};
					var events = dte.Events.SolutionEvents;
					events.AfterClosing += handler;
					return () =>
					{
						events.AfterClosing -= handler;
					};
				}
			);
		}

		public static IObservable<DteNotification> FromProjectAdded(this EnvDTE80.DTE2 dte)
		{
			return Observable.Create<DteNotification>
			(
				observer =>
				{
					EnvDTE._dispSolutionEvents_ProjectAddedEventHandler handler = (EnvDTE.Project project) =>
					{
						observer.OnNext (new DteNotification (DteChangeTypes.ProjectAdded, project));
					};
					var events = dte.Events.SolutionEvents;
					events.ProjectAdded += handler;
					return () =>
					{
						events.ProjectAdded -= handler;
					};
				}
			);
		}

		public static IObservable<DteNotification> FromProjectRemoved(this EnvDTE80.DTE2 dte)
		{
			return Observable.Create<DteNotification>
			(
				observer =>
				{
					EnvDTE._dispSolutionEvents_ProjectRemovedEventHandler handler = (EnvDTE.Project project) =>
					{
						observer.OnNext (new DteNotification (DteChangeTypes.ProjectRemoved, project));
					};
					var events = dte.Events.SolutionEvents;
					events.ProjectRemoved += handler;
					return () =>
					{
						events.ProjectRemoved -= handler;
					};
				}
			);
		}

		public static IObservable<DteNotification> FromProjectRenamed(this EnvDTE80.DTE2 dte)
		{
			return Observable.Create<DteNotification>
			(
				observer =>
				{
					EnvDTE._dispSolutionEvents_ProjectRenamedEventHandler handler = (EnvDTE.Project project, string oldName) =>
					{
						observer.OnNext (new DteNotification (DteChangeTypes.ProjectRenamed, project, oldName));
					};
					var events = dte.Events.SolutionEvents;
					events.ProjectRenamed += handler;
					return () =>
					{
						events.ProjectRenamed -= handler;
					};
				}
			);
		}

		public static IObservable<DteNotification> FromDocumentOpened(this EnvDTE80.DTE2 dte)
		{
			return Observable.Create<DteNotification>
			(
				observer =>
				{
					EnvDTE._dispDocumentEvents_DocumentOpenedEventHandler handler = (EnvDTE.Document document) =>
					{
						observer.OnNext (new DteNotification (DteChangeTypes.DocumentOpened, document));
					};
					var events = dte.Events.DocumentEvents;
					events.DocumentOpened += handler;
					return () =>
					{
						events.DocumentOpened -= handler;
					};
				}
			);
		}

		public static IObservable<DteNotification> FromDocumentClosing(this EnvDTE80.DTE2 dte)
		{
			return Observable.Create<DteNotification>
			(
				observer =>
				{
					EnvDTE._dispDocumentEvents_DocumentClosingEventHandler handler = (EnvDTE.Document document) =>
					{
						observer.OnNext (new DteNotification (DteChangeTypes.DocumentClosing, document));
					};
					var events = dte.Events.DocumentEvents;
					events.DocumentClosing += handler;
					return () =>
					{
						events.DocumentClosing -= handler;
					};
				}
			);
		}

		public static IObservable<DteNotification> FromDocumentSaved(this EnvDTE80.DTE2 dte)
		{
			return Observable.Create<DteNotification>
			(
				observer =>
				{
					EnvDTE._dispDocumentEvents_DocumentSavedEventHandler handler = (EnvDTE.Document document) =>
					{
						observer.OnNext (new DteNotification (DteChangeTypes.DocumentSaved, document));
					};
					var events = dte.Events.DocumentEvents;
					events.DocumentSaved += handler;
					return () =>
					{
						events.DocumentSaved -= handler;
					};
				}
			);
		}


		private static IEnumerable<IObservable<DteNotification>> CreateObservableEvents(this EnvDTE80.DTE2 dte, DteChangeTypes changeTypes)
		{
			if (changeTypes.HasFlag (DteChangeTypes.SolutionOpened))
			{
				yield return dte.FromSolutionOpened ();
			}
			if (changeTypes.HasFlag (DteChangeTypes.SolutionRenamed))
			{
				yield return dte.FromSolutionRenamed ();
			}
			if (changeTypes.HasFlag (DteChangeTypes.SolutionBeforeClosing))
			{
				yield return dte.FromSolutionBeforeClosing ();
			}
			if (changeTypes.HasFlag (DteChangeTypes.SolutionAfterClosing))
			{
				yield return dte.FromSolutionAfterClosing ();
			}
			if (changeTypes.HasFlag (DteChangeTypes.ProjectAdded))
			{
				yield return dte.FromProjectAdded ();
			}
			if (changeTypes.HasFlag (DteChangeTypes.ProjectRemoved))
			{
				yield return dte.FromProjectRemoved ();
			}
			if (changeTypes.HasFlag (DteChangeTypes.ProjectRenamed))
			{
				yield return dte.FromProjectRenamed ();
			}
			if (changeTypes.HasFlag (DteChangeTypes.DocumentOpened))
			{
				yield return dte.FromDocumentOpened ();
			}
			if (changeTypes.HasFlag (DteChangeTypes.DocumentClosing))
			{
				yield return dte.FromDocumentClosing ();
			}
			if (changeTypes.HasFlag (DteChangeTypes.DocumentSaved))
			{
				yield return dte.FromDocumentSaved ();
			}
		}
	}
}
