using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using EnvDTE80;

namespace Epsitec.Cresus.Strings
{
	public class DTE2Provider
	{
		private static class Interop
		{
			[DllImport ("ole32.dll")]
			public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);
			[DllImport ("ole32.dll")]
			public static extern void GetRunningObjectTable(int reserved, out IRunningObjectTable prot);
		}
		/// <summary>
		/// Enumerates the running visual studio IDEs
		/// </summary>
		public static IEnumerable<DTE2> EnumDTE2s()
		{
			IRunningObjectTable rot;
			Interop.GetRunningObjectTable (0, out rot);
			IEnumMoniker enumMoniker;
			rot.EnumRunning (out enumMoniker);
			enumMoniker.Reset ();
			IntPtr fetched = IntPtr.Zero;
			IMoniker[] moniker = new IMoniker[1];
			while (enumMoniker.Next (1, moniker, fetched) == 0)
			{
				IBindCtx bindCtx;
				Interop.CreateBindCtx (0, out bindCtx);
				string displayName;
				moniker[0].GetDisplayName (bindCtx, null, out displayName);
				// add all VisualStudio ROT entries to list
				if (displayName.StartsWith ("!VisualStudio"))
				{
					object comObject;
					rot.GetObject (moniker[0], out comObject);
					yield return (DTE2)comObject;
				}
			}
		}

		public static DTE2 GetDTE2(int processId)
		{
			string progId = "!VisualStudio.DTE.11.0:" + processId.ToString ();

			IBindCtx bindCtx = null;
			IRunningObjectTable rot = null;
			IEnumMoniker enumMoniker = null;

			try
			{
				Marshal.ThrowExceptionForHR (Interop.CreateBindCtx (reserved: 0, ppbc: out bindCtx));
				bindCtx.GetRunningObjectTable (out rot);
				rot.EnumRunning (out enumMoniker);

				IMoniker[] moniker = new IMoniker[1];
				IntPtr fetched = IntPtr.Zero;
				while (enumMoniker.Next (1, moniker, fetched) == 0)
				{
					IMoniker runningObjectMoniker = moniker[0];
					string name = null;
					try
					{
						if (runningObjectMoniker != null)
						{
							runningObjectMoniker.GetDisplayName (bindCtx, null, out name);
						}
					}
					catch (UnauthorizedAccessException)
					{
						// Do nothing, there is something in the ROT that we do not have access to.
					}

					if (!string.IsNullOrEmpty (name) && string.Equals (name, progId, StringComparison.Ordinal))
					{
						object runningObject = null;
						Marshal.ThrowExceptionForHR (rot.GetObject (runningObjectMoniker, out runningObject));
						return (DTE2) runningObject;
					}
				}
			}
			finally
			{
				if (enumMoniker != null)
				{
					Marshal.ReleaseComObject (enumMoniker);
				}

				if (rot != null)
				{
					Marshal.ReleaseComObject (rot);
				}

				if (bindCtx != null)
				{
					Marshal.ReleaseComObject (bindCtx);
				}
			}
			return null;
		}
		
		
		public static IEnumerable<DTE2> EnumDTE2s(string solutionFilePath)
		{
			return DTE2Provider.EnumDTE2s ().Where(dte2 => dte2.Solution != null && string.Compare (dte2.Solution.FullName, solutionFilePath, true) == 0);
		}

		//private DTE2 GetAnyDTE2()
		//{
		//	return (DTE2)Marshal.GetActiveObject ("VisualStudio.DTE.11.0");
		//}

	}
}
