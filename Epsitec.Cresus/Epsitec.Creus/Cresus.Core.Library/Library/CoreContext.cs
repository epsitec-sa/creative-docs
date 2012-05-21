//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

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


		public static T CreateApplication<T>()
			where T : CoreApp
		{
			if (CoreContext.applicationType != null)
			{
				return System.Activator.CreateInstance (CoreContext.applicationType) as T;
			}
			else
			{
				return null;
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

		public static void DefineApplicationClass(string assemblyName, string typeName)
		{
			var assembly = System.Reflection.Assembly.LoadFrom (assemblyName);
			CoreContext.applicationType = assembly.GetType (typeName);
		}			

		/// <summary>
		/// Parses the optional settings file. Every line in the file can specify a
		/// static method of <see cref="CoreContext"/> to execute. Currently, arguments
		/// can only be constants of type <c>string</c>, <c>int</c>, <c>decimal</c> and
		/// <c>bool</c>.
		/// </summary>
		/// <param name="lines">The source lines from the settings file.</param>
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

					if (methodInfo == null)
					{
						throw new System.FormatException (string.Format ("Cannot resolve method '{0}'", methodName));
					}

					methodInfo.Invoke (null, parameters);
				}
			}
		}

		/// <summary>
		/// Reads the core context settings file (if any). The file has the same path as the
		/// currently executing assembly, with <c>.crconfig</c> appended to it.
		/// </summary>
		/// <returns>The lines of text found in the settings file.</returns>
		public static IEnumerable<string> ReadCoreContextSettingsFile()
		{
			var file = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			var dir  = System.IO.Path.GetDirectoryName (file);
			var name = System.IO.Path.GetFileNameWithoutExtension (file);
			var path = System.IO.Path.Combine (dir, name + ".crconfig");

			if (System.IO.File.Exists (path))
			{
				return System.IO.File.ReadLines (path, System.Text.Encoding.Default);
			}
			else
			{
				return EmptyEnumerable<string>.Instance;
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

		private static System.Type				applicationType;
	}
}