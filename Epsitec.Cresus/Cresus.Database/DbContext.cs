//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbContext</c> class defines a per-thread context which gives
	/// access to the resource manager, etc.
	/// </summary>
	public sealed class DbContext
	{
		private DbContext()
		{
			this.resourceManager = Resources.DefaultManager;
		}

		/// <summary>
		/// Gets or sets the resource manager associated with the context.
		/// </summary>
		/// <value>The resource manager.</value>
		public ResourceManager					ResourceManager
		{
			get
			{
				this.VerifyCallingThread ();
				return this.resourceManager;
			}
			set
			{
				this.VerifyCallingThread ();
				this.resourceManager = value;
			}
		}

		/// <summary>
		/// Gets the context owner thread.
		/// </summary>
		/// <value>The context owner thread.</value>
		public System.Threading.Thread			OwnerThread
		{
			get
			{
				return this.ownerThread;
			}
		}

		/// <summary>
		/// Gets the context associated with the current thread.
		/// </summary>
		/// <value>The current context.</value>
		public static DbContext					Current
		{
			get
			{
				if (DbContext.threadContext == null)
				{
					lock (DbContext.globalExclusion)
					{
						if (DbContext.threadContext == null)
						{
							DbContext.threadContext = new DbContext ();
							DbContext.threadContext.ownerThread = System.Threading.Thread.CurrentThread;
						}
					}
				}

				return DbContext.threadContext;
			}
		}

		[System.Diagnostics.Conditional ("DEBUG")]
		private void VerifyCallingThread()
		{
			if (this.ownerThread != System.Threading.Thread.CurrentThread)
			{
				throw new System.InvalidOperationException ("Code called by wrong thread");
			}
		}

		private static object					globalExclusion = new object ();
		
		[System.ThreadStatic]
		private static DbContext				threadContext;

		private System.Threading.Thread			ownerThread;
		private ResourceManager					resourceManager;
	}
}
