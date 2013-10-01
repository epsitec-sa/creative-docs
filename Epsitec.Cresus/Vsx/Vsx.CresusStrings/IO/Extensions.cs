using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Epsitec.IO;

namespace Epsitec
{
	public static partial class Extensions
	{
		public static IObservable<FileSystemNotification> Watch(this FileSystemWatcher watcher, WatcherChangeTypes changeTypes = WatcherChangeTypes.All)
		{
			var notifications = Observable.Create<FileSystemNotification> (
				observer =>
				{
					var mergedEvents = Observable.Merge (watcher.CreateObservableEvents (changeTypes));
					watcher.EnableRaisingEvents = true;

					return mergedEvents.Subscribe (
						e => observer.OnNext (e),
						ex => observer.OnError (ex),
						() => observer.OnCompleted ());
				}
			);
			return Observable.Using (() => watcher, _ => notifications);
		}

		private static IEnumerable<IObservable<FileSystemNotification>> CreateObservableEvents(this FileSystemWatcher watcher, WatcherChangeTypes changeTypes)
		{
			if (changeTypes.HasFlag (WatcherChangeTypes.Created))
			{
				yield return Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs> (h => watcher.Created += h, h => watcher.Created -= h).Select (e => new FileSystemNotification (e.EventArgs));
			}
			if (changeTypes.HasFlag (WatcherChangeTypes.Changed))
			{
				yield return Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs> (h => watcher.Changed += h, h => watcher.Changed -= h).Select (e => new FileSystemNotification (e.EventArgs));
			}
			if (changeTypes.HasFlag (WatcherChangeTypes.Deleted))
			{
				yield return Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs> (h => watcher.Deleted += h, h => watcher.Deleted -= h).Select (e => new FileSystemNotification (e.EventArgs));
			}
			if (changeTypes.HasFlag (WatcherChangeTypes.Renamed))
			{
				yield return Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs> (h => watcher.Renamed += h, h => watcher.Renamed -= h).Select (e => new FileSystemNotification (e.EventArgs));
			}
			yield return Observable.FromEventPattern<ErrorEventHandler, ErrorEventArgs> (h => watcher.Error += h, h => watcher.Error -= h)
				.SelectMany (e => Observable.Throw<FileSystemNotification> (e.EventArgs.GetException ()));
		}
	}
}
