using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.IO
{
	/// <summary>
	/// Holds the information for a file system change event.
	/// </summary>
	public sealed class FileSystemNotification
	{
		/// <summary>
		/// Constructs a new instance of the <see cref="T:System.IO.FileSystemNotification" /> class for <see cref="F:System.IO.FileSystemEventArgs" />.
		/// </summary>
		/// <param name="e">The file system event args.</param>
		public FileSystemNotification(FileSystemEventArgs e)
			: this (e.ChangeType, e.Name, e.FullPath)
		{
		}
		/// <summary>
		/// Constructs a new instance of the <see cref="T:System.IO.FileSystemNotification" /> class for <see cref="F:System.IO.RenamedEventArgs" />.
		/// </summary>
		/// <param name="e">The file system event args.</param>
		public FileSystemNotification(RenamedEventArgs e)
			: this (e.OldName, e.OldFullPath, e.Name, e.FullPath)
		{
		}
		/// <summary>
		/// Constructs a new instance of the <see cref="T:System.IO.FileSystemNotification" /> class for all change types other than <see cref="F:System.IO.WatcherChangeTypes.Renamed" />.
		/// </summary>
		/// <param name="change">The type of change that occurred.</param>
		/// <param name="name">The name of the file or directory that changed.</param>
		/// <param name="fullPath">The full path to the file or directory that changed.</param>
		public FileSystemNotification(WatcherChangeTypes change, string name, string fullPath)
		{
			if (change == WatcherChangeTypes.Renamed)
			{
				throw new ArgumentOutOfRangeException ("change", "Should not be Renamed");
			}
			this.change = change;
			this.name = name;
			this.fullPath = fullPath;
		}
		/// <summary>
		/// Constructs a new instance of the <see cref="T:System.IO.FileSystemNotification" /> class for <see cref="F:System.IO.WatcherChangeTypes.Renamed" />.
		/// </summary>
		/// <param name="oldName">The previous name of the file or directory that was renamed.</param>
		/// <param name="oldFullPath">The previous full path of the file or directory that was renamed.</param>
		/// <param name="name">The new name of the file or directory.</param>
		/// <param name="fullPath">The new full path of the file or directory.</param>
		public FileSystemNotification(string oldName, string oldFullPath, string name, string fullPath)
		{
			this.change = WatcherChangeTypes.Renamed;
			this.name = name;
			this.fullPath = fullPath;
			this.oldName = oldName;
			this.oldFullPath = oldFullPath;
		}

		/// <summary>
		/// Gets the kind of change that the <see cref="T:System.IO.FileSystemNotification" /> represents.
		/// </summary>
		public WatcherChangeTypes Change
		{
			get
			{
				return this.change;
			}
		}
		/// <summary>
		/// Gets the name of the file or directory that changed.
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
		}
		/// <summary>
		/// Gets the full path of the file or directory that changed.
		/// </summary>
		public string FullPath
		{
			get
			{
				return this.fullPath;
			}
		}
		/// <summary>
		/// Gets the previous name of the file or directory, or <see langword="null" />.
		/// </summary>
		/// <value>Returns the previous name of the file or directory when <see cref="P:System.IO.FileSystemNotification.Change" /> is <see cref="F:System.IO.WatcherChangeTypes.Renamed" />;
		/// otherwise, returns <see langword="null" />.</value>
		public string OldName
		{
			get
			{
				return this.oldName;
			}
		}
		/// <summary>
		/// Gets the previous full path of the file or directory, or <see langword="null" />.
		/// </summary>
		/// <value>Returns the previous full path of the file or directory when <see cref="P:System.IO.FileSystemNotification.Change" /> is <see cref="F:System.IO.WatcherChangeTypes.Renamed" />;
		/// otherwise, returns <see langword="null" />.</value>
		public string OldFullPath
		{
			get
			{
				return this.oldFullPath;
			}
		}

		/// <summary>
		/// Gets a string representation of the file system change notification.
		/// </summary>
		/// <returns>String the represents the notification.</returns>
		public override string ToString()
		{
			if (this.change == WatcherChangeTypes.Renamed)
			{
				return string.Concat (new string[]
				{
					"Renamed \"",
					this.oldFullPath,
					"\" to \"",
					this.name,
					"\""
				});
			}
			return this.change + " " + this.fullPath;
		}

		private readonly WatcherChangeTypes change;
		private readonly string name;
		private readonly string fullPath;
		private readonly string oldName;
		private readonly string oldFullPath;
	}
}
