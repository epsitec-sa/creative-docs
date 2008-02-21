//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Runtime.ConstrainedExecution;
using System.Security.Permissions;

namespace Epsitec.Common.Drawing.Agg
{
	/// <summary>
	/// The <c>SafeRasterizerHandle</c> class implements a wrapper for the
	/// native AGG rasterizer handle.
	/// </summary>
	[SecurityPermission (SecurityAction.InheritanceDemand, UnmanagedCode=true)]
	[SecurityPermission (SecurityAction.Demand, UnmanagedCode=true)]
	sealed class SafeRasterizerHandle : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SafeRasterizerHandle"/> class.
		/// </summary>
		public SafeRasterizerHandle()
			: base (true)
		{
		}

		/// <summary>
		/// Converts the safe handle to an <c>IntPtr</c>.
		/// </summary>
		/// <param name="handle">The safe handle.</param>
		/// <returns>The unwrapped handle.</returns>
		public static implicit operator System.IntPtr(SafeRasterizerHandle handle)
		{
			return handle.handle;
		}

		/// <summary>
		/// Creates the AGG rasterizer on the fly, if needed.
		/// </summary>
		public void Create()
		{
			this.handle = AntiGrain.Rasterizer.New ();
		}

		/// <summary>
		/// Executes the code required to free the handle.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the handle is released successfully; otherwise, in
		/// the event of a catastrophic failure, <c>false</c>. In this case,
		/// it generates a <c>ReleaseHandleFailed</c> Managed Debugging Assistant.
		/// </returns>
		[ReliabilityContract (Consistency.WillNotCorruptState, Cer.MayFail)]
		protected override bool ReleaseHandle()
		{
			AntiGrain.Rasterizer.Delete (this.handle);
			this.handle = System.IntPtr.Zero;
			return true;
		}
	}
}
