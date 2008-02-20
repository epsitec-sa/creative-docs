//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>FileOperationMode</c> class is used to customize the file operations
	/// supported by the <see cref="FileManager"/> class.
	/// </summary>
	public sealed class FileOperationMode
	{
		public FileOperationMode()
		{
		}

		public FileOperationMode(Platform.IFileOperationWindow ownerWindow)
			: this ()
		{
			this.ownerWindow = ownerWindow;
		}

		public Platform.IFileOperationWindow	OwnerWindow
		{
			get
			{
				return this.ownerWindow;
			}
			set
			{
				this.ownerWindow = value;
			}
		}

		public bool								Silent
		{
			get
			{
				return this.silent;
			}
			set
			{
				this.silent = value;
			}
		}

		public bool								AutoRenameOnCollision
		{
			get
			{
				return this.autoRenameOnCollision;
			}
			set
			{
				this.autoRenameOnCollision = value;
			}
		}

		public bool								AutoConfirmation
		{
			get
			{
				return this.autoConfirmation;
			}
			set
			{
				this.autoConfirmation = value;
			}
		}

		public bool								AutoCreateDirectory
		{
			get
			{
				return this.autoCreateDirectory;
			}
			set
			{
				this.autoCreateDirectory = value;
			}
		}

		private bool							silent;
		private bool							autoRenameOnCollision;
		private bool							autoConfirmation;
		private bool							autoCreateDirectory;
		private Platform.IFileOperationWindow	ownerWindow;
	}
}
