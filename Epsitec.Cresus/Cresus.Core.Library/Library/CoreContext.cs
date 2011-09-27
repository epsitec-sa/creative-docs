﻿//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

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
		
		public static void ParseOptionalSettingsFile(IEnumerable<string> lines)
		{
			foreach (var line in lines)
			{
				int pos1 = line.IndexOf ('(');
				int pos2 = line.IndexOf (')', pos1+1);
				int len  = pos2-pos1-1;

				if ((pos1 > 0) &&
					(len >= 0))
				{
					var methodName = line.Substring (0, pos1).Trim ();
					var parameters = line.Substring (pos1+1, len).Split (',').Select (x => CoreContext.ParseArg (x.Trim ())).ToArray ();
					var methodInfo = typeof (CoreContext).GetMethod (methodName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

					if (methodInfo != null)
					{
						methodInfo.Invoke (null, parameters);
					}
				}
			}
		}

		private static object ParseArg(string arg)
		{
			if ((arg.Length > 1) &&
				(arg[0] == '\"') &&
				(arg[arg.Length-1] == '\"'))
			{
				return arg.Substring (1, arg.Length-2);
			}

			if (arg.IsInteger ())
			{
				return InvariantConverter.ToInt (arg);
			}
			if (arg.IsDecimal ())
			{
				return InvariantConverter.ToDecimal (arg);
			}

			switch (arg)
			{
				case "true":
					return true;
				case "false":
					return false;
				case "null":
					return null;
			}

			throw new System.FormatException (string.Format ("The argument '{0}' could not be parsed", arg));
		}

		
		private static bool						startupCalled;
		private static bool						isInteractive;
		private static bool						isServer;
		
		private static CoreDatabaseType			databaseType;
		private static string					databaseName;
		private static string					databaseHost;
	}
}