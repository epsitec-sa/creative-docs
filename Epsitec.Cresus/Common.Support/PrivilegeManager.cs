//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>PrivilegeManager</c> class gives access to privilege related
	/// informations.
	/// </summary>
	public sealed class PrivilegeManager
	{
		private PrivilegeManager()
		{
			this.shieldIcons = new Dictionary<IconSize, Image> ();
		}


		/// <summary>
		/// Gets the current privilege manager; this manager is thread specific.
		/// </summary>
		/// <value>The current privilege manager.</value>
		public static PrivilegeManager Current
		{
			get
			{
				if (PrivilegeManager.manager == null)
				{
					PrivilegeManager.manager = new PrivilegeManager ();
				}

				return PrivilegeManager.manager;
			}
		}

		
		/// <summary>
		/// Gets a value indicating whether the user is an administrator.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the user is an administrator; otherwise, <c>false</c>.
		/// </value>
		public bool IsUserAnAdministrator
		{
			get
			{
				return Platform.UserAccountApi.IsUserAnAdministrator;
			}
		}

		/// <summary>
		/// Gets the shield icon if the user running as an unprivileged user.
		/// </summary>
		/// <param name="iconSize">Size of the icon.</param>
		/// <returns>The shield icon or <c>null</c>.</returns>
		public Image GetShieldIcon(IconSize iconSize)
		{
			if (Platform.UserAccountApi.IsUserAnAdministrator)
			{
				return null;
			}

			Image bitmap;

			if (this.shieldIcons.TryGetValue (iconSize, out bitmap))
			{
				return bitmap;
			}

			switch (iconSize)
			{
				case IconSize.Default:
				case IconSize.Normal:
					bitmap = Bitmap.FromNativeIcon (Platform.StockIcons.ShieldIcon);
					break;

				case IconSize.Small:
					bitmap = Bitmap.FromNativeIcon (Platform.StockIcons.SmallShieldIcon);
					break;
			}

			this.shieldIcons[iconSize] = bitmap;

			return bitmap;
		}


		public bool LaunchElevated(System.IntPtr applicationWindowHandle, string arguments)
		{
			System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo ();
			
			startInfo.UseShellExecute  = true;
			startInfo.WorkingDirectory = Globals.Directories.InitialDirectory;
			startInfo.FileName         = Globals.ExecutablePath;
			startInfo.Arguments        = arguments;
			startInfo.Verb             = "runas";

			if (applicationWindowHandle != null)
			{
				startInfo.ErrorDialog             = true;
				startInfo.ErrorDialogParentHandle = applicationWindowHandle;
			}

			try
			{
				System.Diagnostics.Process process = System.Diagnostics.Process.Start (startInfo);
				process.WaitForExit ();
			}
			catch (System.Exception)
			{
				return false;
			}

			return true;
		}


		[System.ThreadStatic]
		private static PrivilegeManager manager;


		private readonly Dictionary<IconSize, Image> shieldIcons;
	}
}
