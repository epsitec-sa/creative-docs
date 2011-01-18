//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Platform.Win32
{
	using ProcessWindowStyle = System.Diagnostics.ProcessWindowStyle;
	
	internal sealed class ShellShortcut : System.IDisposable
	{
		public ShellShortcut(string path)
		{
			IPersistFile pf;

			this.link = (IShellLink) new ShellLink ();
			this.path = path;

			if (System.IO.File.Exists (this.path))
			{
				try
				{
					pf = (IPersistFile) this.link;
					pf.Load (this.path, 0);
					//this.link.Resolve (System.IntPtr.Zero, (ShellApi.SLR_FLAGS) (0x00010000*100) | ShellApi.SLR_FLAGS.SLR_NO_UI | ShellApi.SLR_FLAGS.SLR_NOSEARCH | ShellApi.SLR_FLAGS.SLR_NOUPDATE);
				}
				catch
				{
					this.Dispose ();
				}
			}
		}

		~ShellShortcut()
		{
			this.Dispose (false);
		}

		public string							Arguments
		{
			get
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder (Win32Const.INFOTIPSIZE);
				this.link.GetArguments (buffer, buffer.Capacity);
				return buffer.ToString ();
			}
			set
			{
				this.link.SetArguments (value);
			}
		}

		public string							Description
		{
			get
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder (Win32Const.INFOTIPSIZE);
				this.link.GetDescription (buffer, buffer.Capacity);
				return buffer.ToString ();
			}
			set
			{
				this.link.SetDescription (value);
			}
		}

		public string							WorkingDirectory
		{
			get
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder (Win32Const.MAX_PATH);
				this.link.GetWorkingDirectory (buffer, buffer.Capacity);
				return buffer.ToString ();
			}
			set
			{
				this.link.SetWorkingDirectory (value);
			}
		}

		public string							TargetPath
		{
			get
			{
				ShellApi.WIN32_FIND_DATAW fd = new ShellApi.WIN32_FIND_DATAW ();
				System.Text.StringBuilder buffer = new System.Text.StringBuilder (Win32Const.MAX_PATH);

				this.link.GetPath (buffer, buffer.Capacity, out fd, ShellApi.SLGP_FLAGS.SLGP_UNCPRIORITY);
				return buffer.ToString ();
			}
			set
			{
				this.link.SetPath (value);
			}
		}

		public PidlHandle						TargetPidl
		{
			get
			{
				if (this.link != null)
				{
					System.IntPtr pidl;
					this.link.GetIDList (out pidl);

					return PidlHandle.Inherit (pidl);
				}
				else
				{
					return null;
				}
			}
			set
			{
				this.link.SetIDList (value.Pidl);
			}
		}
		
		public string							IconPath
		{
			get
			{
				int iconIndex;
				System.Text.StringBuilder buffer = new System.Text.StringBuilder (Win32Const.MAX_PATH);
				this.link.GetIconLocation (buffer, buffer.Capacity, out iconIndex);
				return buffer.ToString ();
			}
			set
			{
				this.link.SetIconLocation (value, this.IconIndex);
			}
		}

		public int								IconIndex
		{
			get
			{
				int iconIndex;
				System.Text.StringBuilder buffer = new System.Text.StringBuilder (Win32Const.MAX_PATH);
				this.link.GetIconLocation (buffer, buffer.Capacity, out iconIndex);
				return iconIndex;
			}
			set
			{
				this.link.SetIconLocation (this.IconPath, value);
			}
		}

		public System.Drawing.Icon				Icon
		{
			get
			{
				int iconIndex;
				System.Text.StringBuilder buffer = new System.Text.StringBuilder (Win32Const.MAX_PATH);
				System.IntPtr hIcon;
				System.IntPtr hInst;
				System.Drawing.Icon icon;
				System.Drawing.Icon iconCopy;


				this.link.GetIconLocation (buffer, buffer.Capacity, out iconIndex);

				string location = buffer.ToString ();
				
				hInst = System.Runtime.InteropServices.Marshal.GetHINSTANCE (typeof (ShellShortcut).Module);
				hIcon = ShellApi.ExtractIcon (hInst, location, iconIndex);
				
				if (hIcon == System.IntPtr.Zero)
				{
					return null;
				}

				// Return a cloned Icon, because we have to free the original ourselves.
				icon = System.Drawing.Icon.FromHandle (hIcon);
				iconCopy = (System.Drawing.Icon) icon.Clone ();
				icon.Dispose ();
				ShellApi.DestroyIcon (hIcon);
				return iconCopy;
			}
		}

		public ProcessWindowStyle				WindowStyle
		{
			get
			{
				int windowStyle;
				this.link.GetShowCmd (out windowStyle);

				switch (windowStyle)
				{
					case Win32Const.SW_SHOWMINIMIZED:	return ProcessWindowStyle.Minimized;
					case Win32Const.SW_SHOWMINNOACTIVE:	return ProcessWindowStyle.Minimized;
					case Win32Const.SW_SHOWMAXIMIZED:	return ProcessWindowStyle.Maximized;
					default:							return ProcessWindowStyle.Normal;
				}
			}
			set
			{
				int windowStyle;

				switch (value)
				{
					case ProcessWindowStyle.Normal:		windowStyle = Win32Const.SW_SHOWNORMAL;			break;
					case ProcessWindowStyle.Minimized:	windowStyle = Win32Const.SW_SHOWMINNOACTIVE;	break;
					case ProcessWindowStyle.Maximized:	windowStyle = Win32Const.SW_SHOWMAXIMIZED;		break;

					default:
						throw new System.ArgumentException (string.Format ("Unsupported ProcessWindowStyle.{0}", value));
				}

				this.link.SetShowCmd (windowStyle);

			}
		}

		public System.Windows.Forms.Keys		Hotkey
		{
			get
			{
				ushort hotkey;
				
				this.link.GetHotkey (out hotkey);

				//	Convert from IShellLink 16-bit format ([MM][VK]) to the 32-bit Keys
				//	enumeration format (00[MM]00[VK]), where [MM] encodes the modifiers
				//	and [VK] maps to the virtual key code.

				int modifiers  = (hotkey & 0xff00) >> 8;
				int virtualKey = (hotkey & 0x00ff) >> 0;
				
				return (System.Windows.Forms.Keys) (modifiers << 16 + virtualKey);
			}
			set
			{
				int modifiers  = ((int)value & 0x00ff0000) >> 16;
				int virtualKey = ((int)value & 0x000000ff) >> 0;

				if (modifiers == 0)
				{
					throw new System.ArgumentException ("Shortcut Hotkey must include a modifier");
				}

				this.link.SetHotkey ((ushort) (modifiers << 8 + virtualKey));
			}
		}

		public void Save()
		{
			IPersistFile pf = (IPersistFile) this.link;
			pf.Save (this.path, true);
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		private void Dispose(bool disposing)
		{
			if (this.link != null)
			{
				System.Runtime.InteropServices.Marshal.ReleaseComObject (this.link);
				this.link = null;
			}
		}



		private IShellLink						link;
		private string							path;
	}
}
