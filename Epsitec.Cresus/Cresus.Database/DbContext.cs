//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database
{
	public sealed class DbContext
	{
		private DbContext()
		{
			this.resourceManager = Resources.DefaultManager;
		}

		public ResourceManager ResourceManager
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
		
		public static DbContext Current
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

		private static object globalExclusion = new object ();
		
		[System.ThreadStatic]
		private static DbContext threadContext;

		private System.Threading.Thread ownerThread;
		private ResourceManager resourceManager;
	}
}
