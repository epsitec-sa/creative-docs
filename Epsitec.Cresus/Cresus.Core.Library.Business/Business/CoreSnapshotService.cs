//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Orchestrators;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Debug;

namespace Epsitec.Cresus.Core.Library.Business
{
	/// <summary>
	/// The <c>CoreSnapshotService</c> class is responsible for the creation of the
	/// database snapshots.
	/// </summary>
	public class CoreSnapshotService
	{
		public CoreSnapshotService()
		{
			this.listener = new DebugOutputTraceListener (this.HandleDebugOutputWriter);
			this.sessionId = CoreSnapshotService.GetTimeStampedId ();

			if (CoreSnapshotService.defaultService == null)
			{
				CoreSnapshotService.defaultService = this;
			}

			this.StartDebugMonitor ();

			System.Diagnostics.Debug.Listeners.Add (this.listener);

			this.CreateDatabaseSnapshot ();
		}

		public void NotifyApplicationStarted(CoreApp app)
		{
			var dataViewOrchestrator = app.FindActiveComponent<DataViewOrchestrator> ();
			var navigationOrchestrator = dataViewOrchestrator.Navigator;

			Window.GlobalFocusedWindowChanged += this.HandleGlobalFocusedWindowChanged;
			Window.GlobalFocusedWidgetChanged += this.HandleGlobalFocusedWidgetChanged;
			Window.GlobalWindowShown += this.HandleGlobalWindowShown;
			
			Widget.GlobalMouseDown += this.HandleWidgetGlobalMouseDown;

			navigationOrchestrator.NodeAdded   += this.HandleNavigationOrchestratorNodeChanged;
			navigationOrchestrator.NodeRemoved += this.HandleNavigationOrchestratorNodeChanged;

			CommandDispatcher.CommandDispatching       += this.HandleCommandDispatcherCommandDispatching;
			CommandDispatcher.CommandDispatched        += this.HandleCommandDispatcherCommandDispatchFinished;
			CommandDispatcher.CommandDispatchCancelled += this.HandleCommandDispatcherCommandDispatchFinished;
			CommandDispatcher.CommandDispatchFailed    += this.HandleCommandDispatcherCommandDispatchFinished;

			BridgeSpy.ExecutingSetter += this.HandleBridgeSpyExecutingSetter;
		}

		public static void NotifyUserMessage(string message)
		{
			if (CoreSnapshotService.defaultService != null)
			{
				CoreSnapshotService.defaultService.RecordEvent ("USER_MESSAGE", message);
			}
		}

		
		private void StartDebugMonitor()
		{
			this.sessionPath = System.IO.Path.Combine (System.IO.Path.GetTempPath (), "dbg-" + this.sessionId);

			System.IO.Directory.CreateDirectory (this.sessionPath);

			string arguments = InvariantConverter.Format (@"-monitor ""{0}"" {1}", this.sessionPath, System.Diagnostics.Process.GetCurrentProcess ().Id);

			var startInfo = new System.Diagnostics.ProcessStartInfo ("App.DebugService.exe", arguments)
			{
				WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized
			};
			
			System.Diagnostics.Process.Start (startInfo);
			System.Diagnostics.Debug.WriteLine ("Started debug service : " + arguments);
		}
		
		private void CreateDatabaseSnapshot()
		{
			var remoteBackupFolder   = CoreSnapshotService.GetRemoteBackupFolder ();
			var remoteBackupFileName = CoreSnapshotService.GetTimeStampedId ("db.firebird-backup");
			var remoteBackupPath     = System.IO.Path.Combine (remoteBackupFolder, remoteBackupFileName);

			CoreData.BackupDatabase (remoteBackupPath, CoreData.GetDatabaseAccess ());
		}

		private void RecordEvent(string eventName, string eventArg)
		{
			this.listener.Flush ();
			this.RecordEventData (eventName, eventArg);
		}

		private void RecordEventData(string eventName, string text)
		{
			System.IO.File.WriteAllText (this.GetLoggingFilePath (eventName + ".txt"), text, System.Text.Encoding.UTF8);
		}


		private void HandleCommandDispatcherCommandDispatching(object sender, CommandEventArgs e)
		{
			this.RecordEvent ("CMD", string.Format ("{0} ({1})", e.Command.CommandId, e.Command.Name));
			this.commandDispatchDepth++;
		}

		private void HandleCommandDispatcherCommandDispatchFinished(object sender, CommandEventArgs e)
		{
			this.commandDispatchDepth--;
		}

		private void HandleBridgeSpyExecutingSetter(object sender, BridgeSpyEventArgs e)
		{
			var entity = e.Entity;

			var dataContext = Epsitec.Cresus.DataLayer.Context.DataContextPool.GetDataContext (entity);
			var entityKey   = dataContext.GetNormalizedEntityKey (entity);

			if (entityKey.HasValue)
			{
				this.RecordEvent ("SET_DB_FIELD", string.Format ("{0}/{1} : {2} -> {3}", entityKey.Value, e.FieldCaption.Name, e.OldValue ?? "<null>", e.NewValue ?? "<null>"));
			}
			else
			{
				this.RecordEvent ("SET_LIVE_FIELD", string.Format ("{0}/{1} : {2} -> {3}", e.Entity.GetEntityStructuredTypeId (), e.FieldCaption.Name, e.OldValue ?? "<null>", e.NewValue ?? "<null>"));
			}
		}

		private void HandleGlobalFocusedWidgetChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var widget = e.NewValue as Widget;

			if ((widget != null) &&
				(widget.IsFocused))
			{
				this.RecordEvent ("WIDGET_FOCUS", InvariantConverter.Format ("{0}:{1}/{2}", widget.GetVisualSerialId (), widget.GetType ().Name, widget.FullPathName));

				Application.QueueAsyncCallback (() => this.CreateWindowFocusSnapshot (widget, System.Drawing.Pens.Red));
			}
		}

		private void HandleGlobalWindowShown(object sender)
		{
			var window = sender as Window;

			this.RecordEvent ("WINDOW_SHOW", InvariantConverter.Format ("{0}:{1}/{2}", window.GetWindowSerialId (), window.GetType ().Name, window.Name));
			this.CreateWindowSnapshot (window);
		}

		private void HandleWidgetGlobalMouseDown(object sender, MessageEventArgs e)
		{
			var messageId = e.Message.UserMessageId;
			var widget = sender as Widget;

			if (messageId == this.lastUserMessageId)
			{
				return;
			}

			this.lastUserMessageId = messageId;

			this.RecordEvent ("MOUSE_DOWN", InvariantConverter.Format ("{0}:{1}/{2}:{3}", widget.GetVisualSerialId (), widget.GetType ().Name, widget.FullPathName, e.Message.Cursor.ToString ()));

			this.CreateWindowClickSnapshot (widget, e.Message.Cursor, System.Drawing.Brushes.Red);
		}

		private void HandleGlobalFocusedWindowChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var window = e.NewValue as Window;

			if ((window != null) &&
				(window.IsFocused))
			{
				this.RecordEvent ("WINDOW_FOCUS", InvariantConverter.Format ("{0}:{1}/{2}", window.GetWindowSerialId (), window.GetType ().Name, window.Name));
			}
		}

		private void HandleNavigationOrchestratorNodeChanged(object sender)
		{
			var navigationOrchestrator = sender as NavigationOrchestrator;
			var navigationPath         = navigationOrchestrator.GetTopNavigationPath ();

			if (navigationPath != null)
			{
				this.RecordEvent ("NAV", navigationPath.ToString ());
			}
		}

		private void HandleDebugOutputWriter(string text)
		{
			this.RecordEventData ("TRACE", text);
		}

		private void CreateWindowSnapshot(Window window)
		{
			var bitmap = window.GetWindowBitmap ();

			if (bitmap != null)
			{
				var source = bitmap.NativeBitmap;
				var copy   = new System.Drawing.Bitmap (source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				var graphics = System.Drawing.Graphics.FromImage (copy);
				graphics.DrawImageUnscaled (source, 0, 0);
				graphics.Dispose ();
				copy.RotateFlip (System.Drawing.RotateFlipType.RotateNoneFlipY);

				var codec = Epsitec.Common.Drawing.Bitmap.GetCodecInfo (Common.Drawing.ImageFormat.Jpeg);
				var encoder = new System.Drawing.Imaging.EncoderParameters (1);
				encoder.Param[0] = new System.Drawing.Imaging.EncoderParameter (System.Drawing.Imaging.Encoder.Quality, 50L);

				copy.Save (this.GetLoggingFilePath ("window.jpg"), codec, encoder);
				copy.Dispose ();

				bitmap.Dispose ();
			}
		}

		private void CreateWindowFocusSnapshot(Widget widget, System.Drawing.Pen pen)
		{
			if ((widget.IsVisible == false) ||
				(widget.Window == null) ||
				(widget.Window.IsVisible == false))
			{
				return;
			}

			var bounds = widget.MapClientToRoot (widget.Client.Bounds);
			var bitmap = widget.Window.GetWindowBitmap ();

			if (bitmap != null)
			{
				var source = bitmap.NativeBitmap;
				var copy   = new System.Drawing.Bitmap (source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				var graphics = System.Drawing.Graphics.FromImage (copy);
				graphics.DrawImageUnscaled (source, 0, 0);
				var rect = new System.Drawing.Rectangle ((int) bounds.X, (int) bounds.Y, (int) bounds.Width, (int) bounds.Height);
				graphics.DrawRectangle (pen, rect);
				graphics.Dispose ();
				copy.RotateFlip (System.Drawing.RotateFlipType.RotateNoneFlipY);

				var codec = Epsitec.Common.Drawing.Bitmap.GetCodecInfo (Common.Drawing.ImageFormat.Jpeg);
				var encoder = new System.Drawing.Imaging.EncoderParameters (1);
				encoder.Param[0] = new System.Drawing.Imaging.EncoderParameter (System.Drawing.Imaging.Encoder.Quality, 50L);

				copy.Save (this.GetLoggingFilePath ("window.jpg"), codec, encoder);
				copy.Dispose ();

				bitmap.Dispose ();
			}
		}

		private void CreateWindowClickSnapshot(Widget widget, Epsitec.Common.Drawing.Point point, System.Drawing.Brush brush)
		{
			var bitmap = widget.Window.GetWindowBitmap ();

			if (bitmap != null)
			{
				var source = bitmap.NativeBitmap;
				var copy   = new System.Drawing.Bitmap (source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				var graphics = System.Drawing.Graphics.FromImage (copy);
				graphics.DrawImageUnscaled (source, 0, 0);
				graphics.FillPie (brush, (int) point.X-4, (int) point.Y-4, 8, 8, 0, 360);
				graphics.Dispose ();
				copy.RotateFlip (System.Drawing.RotateFlipType.RotateNoneFlipY);

				var codec = Epsitec.Common.Drawing.Bitmap.GetCodecInfo (Common.Drawing.ImageFormat.Jpeg);
				var encoder = new System.Drawing.Imaging.EncoderParameters (1);
				encoder.Param[0] = new System.Drawing.Imaging.EncoderParameter (System.Drawing.Imaging.Encoder.Quality, 50L);

				copy.Save (this.GetLoggingFilePath ("window.jpg"), codec, encoder);
				copy.Dispose ();

				bitmap.Dispose ();
			}
		}

		private string GetLoggingFilePath(string name)
		{
			var totalTicks = System.DateTime.UtcNow.Ticks / 10000;
			var fileName   = InvariantConverter.Format ("{0:00000000000000}.{1}", totalTicks, name);
			
			return System.IO.Path.Combine (this.sessionPath, fileName);
		}

		private static string GetRemoteBackupFolder()
		{
			return "Snapshots";
		}

		private static string GetTimeStampedId(string name = null)
		{
			var userName   = System.Environment.UserName.ToLowerInvariant ();
			var hostName   = System.Environment.MachineName.ToLowerInvariant ();
			var totalTicks = System.DateTime.UtcNow.Ticks / 10000;

			if (name == null)
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}@{1}-{2:00000000000000}", userName, hostName, totalTicks);
			}
			else
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}@{1}-{2:00000000000000}.{3}", userName, hostName, totalTicks, name);
			}
		}


		private static CoreSnapshotService defaultService;

		private string sessionId;
		private string sessionPath;
		private int commandDispatchDepth;
		private long lastUserMessageId;
		private DebugOutputTraceListener listener;
	}
}
