//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>CoreContext</c> class provides basic information about the execution context.
	/// It is a global, per process context; it cannot contain thread or session specific
	/// informations.
	/// </summary>
	public static class CoreContext
	{
		public static bool						IsInteractive
		{
			get
			{
				return CoreContext.isInteractive;
			}
		}

		public static bool						IsServer
		{
			get
			{
				return CoreContext.isServer;
			}
		}

		public static CoreDatabaseType			DatabaseType
		{
			get
			{
				return CoreContext.databaseType;
			}
			set
			{
				if (CoreContext.databaseType == CoreDatabaseType.None)
				{
					CoreContext.databaseType = value;
				}
				else
				{
					throw new System.InvalidOperationException ("Cannot set database type twice");
				}
			}
		}

		public static string					DatabaseName
		{
			get
			{
				return CoreContext.databaseName;
			}
		}

		public static string					DatabaseHost
		{
			get
			{
				return CoreContext.databaseHost;
			}
		}


		public static void StartAsInteractive()
		{
			CoreContext.startupCalled.WhenTrueThrow<System.InvalidOperationException> ("Start already called");

			CoreContext.startupCalled = true;
			CoreContext.isInteractive = true;
		}

		public static void StartAsMaintenance()
		{
			CoreContext.StartAsInteractive ();
		}
		
		public static void StartAsServer()
		{
			CoreContext.startupCalled.WhenTrueThrow<System.InvalidOperationException> ("Start already called");

			CoreContext.startupCalled = true;
			CoreContext.isServer      = true;
		}

		public static void DefineDatabase(string name, string host)
		{
			CoreContext.databaseName = name;
			CoreContext.databaseHost = host;
		}

		
		private static bool						startupCalled;
		private static bool						isInteractive;
		private static bool						isServer;
		
		private static CoreDatabaseType			databaseType;
		private static string					databaseName;
		private static string					databaseHost;
	}
}