//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;

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

		public static bool						EnableSnapshotService
		{
			get
			{
				return CoreContext.enableSnapshotService;
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

		public static bool						UseEmbeddedDatabaseClient
		{
			get
			{
				return CoreContext.useEmbeddedDatabaseClient;
			}
		}

		public static bool						DisableUserManagerReload
		{
			get
			{
				return CoreContext.disableUserManagerReload;
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
			CoreContext.disableUserManagerReload = true;
		}


		public static void DefineDatabase(string name, string host)
		{
			CoreContext.databaseName = name;
			CoreContext.databaseHost = host;
		}

		public static void EnableEmbeddedDatabaseClient(bool enabled)
		{
			CoreContext.useEmbeddedDatabaseClient = enabled;
		}

		public static void DefineApplicationClass(string assemblyName, string typeName)
		{
			var assembly = System.Reflection.Assembly.LoadFrom (assemblyName);
			CoreContext.applicationType = assembly.GetType (typeName);
		}

		public static void DefineMetadata(string className, string xmlSource)
		{
			var types = from type in TypeEnumerator.Instance.GetAllTypes ()
						where type.IsClass && type.Name == className && type.IsSubclassOf (typeof (CoreMetadata))
						select type;

			var match = types.FirstOrDefault ();

			if (match == null)
			{
				throw new System.ArgumentException (string.Format ("The class name {0} cannot be resolved", className));
			}

			var xml   = XElement.Parse (xmlSource, LoadOptions.None);
			var args  = new object[1] { xml };
			var flags = BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public;
			
			//	At this very early stage in the boot process, we cannot instanciate the meta
			//	class, so we will have to defer the restore until the application object calls
			//	CoreContext back through method ExecutePendingSetupFunctions.
			
			CoreContext.EnqueueSetupCode (() => CoreContext.AddMetadata (match.InvokeMember ("Restore", flags, null, null, args) as CoreMetadata));
		}

		public static void DefineTypeSubstitution(string baseClassName, string implClassName)
		{
			CoreContext.typeSubstitutions[baseClassName] = implClassName;
		}

		public static void DefineEnableSnapshotService(bool enableSnapshotService)
		{
			CoreContext.enableSnapshotService = enableSnapshotService;
		}
		
		private static void AddMetadata(CoreMetadata metadata)
		{
			if (metadata == null)
			{
				return;
			}

			var type = metadata.GetType ();
			var meta = metadata;

			if (CoreContext.metadata.TryGetValue (type, out meta))
			{
				meta.Add (metadata);
			}
			else
			{
				CoreContext.metadata.Add (type, metadata);
			}
			
#if false
			var ef1 = new Metadata.EntityFilter (Druid.Parse ("[GVAV5]"));
			var ef2 = new Metadata.EntityFilter (Druid.Parse ("[GVAV5]"));

			var filter = new Metadata.Filter ()
			{
				CombineMode = Metadata.FilterCombineMode.Or,
				Description = "Essai",
				Name = "Test",
			};

			var ex1 = new Metadata.ColumnFilterComparisonExpression ()
			{
				Comparison = Metadata.ColumnFilterComparisonCode.Equal,
				Constant = Metadata.ColumnFilterConstant.From ("X")
			};

			var ex2 = new Metadata.ColumnFilterRangeExpression ()
			{
				LowerBoundComparison = Metadata.ColumnFilterComparisonCode.GreaterThanOrEqual,
				LowerBound = Metadata.ColumnFilterConstant.From ("A"),
				UpperBoundComparison = Metadata.ColumnFilterComparisonCode.LessThanOrEqual,
				UpperBound = Metadata.ColumnFilterConstant.From ("Z")
			};

			ef1.Columns.Add (new Metadata.ColumnRef<Metadata.EntityColumnFilter> ("[GVA06].[GVA3].[FVA33]", new Metadata.EntityColumnFilter (ex1)));
			ef2.Columns.Add (new Metadata.ColumnRef<Metadata.EntityColumnFilter> ("[GVA06].[GVAK7].[FVAE2].[FVA71]", new Metadata.EntityColumnFilter (ex2)));

			filter.Nodes.Add (new Metadata.FilterNode (ef1, Metadata.FilterIncludeMode.Inclusive, Metadata.FilterActiveMode.Enabled));
			filter.Nodes.Add (new Metadata.FilterNode (ef2, Metadata.FilterIncludeMode.Exclusive, Metadata.FilterActiveMode.Enforced));

			var xml = filter.Save ("filter");
			var copy = Metadata.Filter.Restore (xml);

			var @param = System.Linq.Expressions.Expression.Parameter (Epsitec.Common.Support.EntityEngine.EntityInfo.GetType (ef1.EntityId), "customer");
			var compoundExpression = filter.GetExpression (@param);
			var compoundLambda = System.Linq.Expressions.Expression.Lambda (compoundExpression, @param);
#endif
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
			XmlExtractor xmlExtractor = null;
			string       xmlSource;
			
			var stack = new List<string> ();
			var dict  = new Dictionary<string, string> ();

			foreach (var current in lines)
			{
				string line = current;

				if ((line.StartsWith ("//")) ||
					(line.StartsWith ("#")))
				{
					continue;
				}

				if (line.Contains ('@'))
				{
					line = CoreContext.SubstituteArguments (line, dict);
				}

				int pos = line.IndexOf ('=');

				if (pos > 0)
				{
					var key   = line.Substring (0, pos).Trim ();
					var value = line.Substring (pos+1).Trim ();

					if (key.All (c => char.IsLetterOrDigit (c)))
					{
						dict[key] = value;
						continue;
					}
				}

			extractXml:

				if (xmlExtractor != null)
				{
					xmlExtractor.AppendLine (line);

					if (xmlExtractor.Finished)
					{
						line = xmlExtractor.ExcessText;
						
						xmlSource    = xmlExtractor.ToString ();
						xmlExtractor = null;
						
						stack.Insert (0, xmlSource);
					}
					else
					{
						continue;
					}
				}

				line = line.TrimStart (' ', '\t');

				if (line.StartsWith ("<"))
				{
					//	Found some inline XML: parse it and store it for future reference

					xmlExtractor = new XmlExtractor ();
					
					goto extractXml;
				}

				int pos1 = line.IndexOf ('(');
				int pos2 = line.IndexOf (')', pos1+1);
				int len  = pos2-pos1-1;

				if ((pos1 > 0) &&
					(len >= 0))
				{
					var methodName = line.Substring (0, pos1).Trim ();
					var parameters = line.Substring (pos1+1, len).Split (',').Select (x => CoreContext.ParseArg (x.Trim (), stack)).ToArray ();
					var methodInfo = typeof (CoreContext).GetMethod (methodName, BindingFlags.Static | BindingFlags.Public);

					if (methodInfo == null)
					{
						throw new System.FormatException (string.Format ("Cannot resolve method '{0}'", methodName));
					}

					methodInfo.Invoke (null, parameters);
				}
			}
		}

		private static string SubstituteArguments(string line, IDictionary<string, string> dict)
		{
			var buffer = new System.Text.StringBuilder ();
			var key    = new System.Text.StringBuilder ();

			bool isKey = false;

			foreach (char c in line)
			{
				if (isKey)
				{
					if ((c == '@') &&
						(key.Length == 0))
					{
						buffer.Append ('@');
						isKey = false;
						continue;
					}

					if (char.IsLetterOrDigit (c))
					{
						key.Append (c);
						continue;
					}
					
					buffer.Append (dict[key.ToString ()]);
					key.Clear ();
					isKey = false;
				}

				if (c == '@')
				{
					isKey = true;
				}
				else
				{
					buffer.Append (c);
				}
			}

			if (key.Length > 0)
			{
				buffer.Append (dict[key.ToString ()]);
			}

			return buffer.ToString ();
		}

		public static void EnqueueSetupCode(System.Action action)
		{
			CoreContext.pendingSetupCode.Enqueue (action);
		}

		public static void ExecutePendingSetupFunctions()
		{
			while (CoreContext.pendingSetupCode.Count > 0)
			{
				var action = CoreContext.pendingSetupCode.Dequeue ();
				action ();
			}
		}

		/// <summary>
		/// Reads the core context settings file (if any). The file has the same path as the
		/// currently executing assembly, with <c>.crconfig</c> appended to it.
		/// </summary>
		/// <returns>The lines of text found in the settings file.</returns>
		public static IEnumerable<string> ReadCoreContextSettingsFile()
		{
			var file = Assembly.GetExecutingAssembly ().Location;
			var dir  = System.IO.Path.GetDirectoryName (file);
			var name = System.IO.Path.GetFileNameWithoutExtension (file);
			var path = System.IO.Path.Combine (dir, name + ".crconfig");

			if (System.IO.File.Exists (path))
			{
				return System.IO.File.ReadLines (path, System.Text.Encoding.Default);
			}
			else
			{
				return Enumerable.Empty<string> ();
			}
		}


		/// <summary>
		/// Gets the metadata of the specified type.
		/// </summary>
		/// <typeparam name="T">The type of metadata which is requested.</typeparam>
		/// <returns>The metadata of type <paramref name="T"/>.</returns>
		/// <exception cref="System.ArgumentException">If the metadata cannot be found.</exception>
		public static T GetMetadata<T>()
			where T : CoreMetadata
		{
			CoreMetadata metadata;

			if (CoreContext.metadata.TryGetValue (typeof (T), out metadata))
			{
				return metadata as T;
			}

			throw new System.ArgumentException ("Cannot find metadata of type " + typeof (T).FullName);
		}

		public static bool ContainsMetadata<T>()
			where T : CoreMetadata
		{
			CoreMetadata metadata;

			if (CoreContext.metadata.TryGetValue (typeof (T), out metadata))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static T New<T>(params object[] args)
			where T : class
		{
			var type = CoreContext.ResolveType (typeof (T));

			var binding = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			return System.Activator.CreateInstance (type, binding, null, args, null) as T;
		}

		public static System.Type ResolveType(System.Type baseType)
		{
			string baseName = baseType.Name;
			string implName;

			if (CoreContext.typeSubstitutions.TryGetValue (baseName, out implName))
			{
				var implTypes = from type in TypeEnumerator.Instance.GetAllTypes ()
								where type.IsClass && (type.Name == implName || type.FullName == implName) && type.IsSubclassOf (baseType)
								select type;

				var implType = implTypes.FirstOrDefault ();

				if (implType == null)
				{
					throw new System.ArgumentException (string.Format ("The implementation class name {0} cannot be resolved", implName));
				}

				return implType;
			}
			else
			{
				return baseType;
			}
		}

		/// <summary>
		/// Parses the argument and derives the proper type. This recognizes <c>true</c>, <c>false</c>
		/// and <c>null</c>, integers and decimals, <c>"strings"</c> and <c>$0</c> indexes into the
		/// XML stack.
		/// </summary>
		/// <param name="arg">The argument.</param>
		/// <param name="stack">The XML stack.</param>
		/// <returns>The value of the argument.</returns>
		private static object ParseArg(string arg, IList<string> stack)
		{
			if (arg[0] == '$')
			{
				int index = InvariantConverter.ToInt (arg.Substring (1));
				return stack[index];
			}

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



		static CoreContext()
		{
			CoreContext.metadata = new Dictionary<System.Type, CoreMetadata> ();
			CoreContext.pendingSetupCode = new Queue<System.Action> ();
			CoreContext.typeSubstitutions = new Dictionary<string, string> ();
		}

		
		private static bool						startupCalled;
		private static bool						isInteractive;
		private static bool						isServer;
		private static bool						enableSnapshotService;
		private static bool						disableUserManagerReload;
		
		private static CoreDatabaseType			databaseType;
		private static string					databaseName;
		private static string					databaseHost;
		private static bool						useEmbeddedDatabaseClient;

		private static System.Type				applicationType;
		
		private static readonly Dictionary<System.Type, CoreMetadata> metadata;
		private static readonly Queue<System.Action> pendingSetupCode;
		private static readonly Dictionary<string, string> typeSubstitutions;
	}
}