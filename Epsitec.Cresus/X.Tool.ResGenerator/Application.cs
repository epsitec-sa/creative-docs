//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Tool.ResGenerator
{
	class Application
	{
		[System.STAThread]
		static void Main(string[] args)
		{
			if (args.Length == 2)
			{
				Application.Process (args[0], args[1]);
				System.Console.Out.WriteLine ("Done.");
			}
			else if (System.IO.File.Exists ("build.ini"))
			{
				Application.ProcessBuildFile ("build.ini");
				
				System.Console.Out.WriteLine ("Done.");
			}
			else
			{
				System.Console.Out.WriteLine ("Invalid command line and no 'build.ini' script found.");
			}
		}

		/// <summary>
		/// Processes the build file. Skips comments and empty lines and executes
		/// individual generation directives.
		/// </summary>
		/// <param name="buildFileName">Name of the build file.</param>
		static void ProcessBuildFile(string buildFileName)
		{
			string[] args;
			
			using (System.IO.StreamReader reader = new System.IO.StreamReader (buildFileName, System.Text.Encoding.UTF8))
			{
				for (int count = 0; ; count++)
				{
					string line = reader.ReadLine ();

					if (line == null)
					{
						break;
					}

					line = line.Trim ();

					if ((line.Length == 0) ||
						(line.StartsWith ("#")) ||
						(line.StartsWith ("//")))
					{
						continue;
					}

					args = line.Split ('=');

					if (args.Length == 2)
					{
						Application.Process (args[0], args[1]);
					}
					else
					{
						System.Console.Error.WriteLine ("Error: {0}:{1}: syntax error.", buildFileName, count);
					}
				}
			}
		}

		/// <summary>
		/// Processes the specified command line. There are two arguments: the default
		/// namespace and the application directory.
		/// </summary>
		/// <param name="defaultNamespace">The default namespace.</param>
		/// <param name="moduleDirectory">The module directory.</param>
		static void Process(string defaultNamespace, string moduleDirectory)
		{
			string moduleName = "x";

			defaultNamespace = defaultNamespace.Trim ();
			moduleDirectory  = moduleDirectory.Trim ();
			
			//	The directory name may contain the short application module name;
			//	this is encoded as "directory|module".
			
			if (moduleDirectory.IndexOf ("|") > 0)
			{
				string[] split = moduleDirectory.Split ('|');
				
				moduleDirectory = split[0];
				moduleName      = split[1];
			}
			
			Application.GenerateResFile (moduleDirectory, defaultNamespace, moduleName);
			
			System.Console.Out.WriteLine ("Generated 'Res.cs' for {0}, application '{1}'", moduleDirectory, moduleName);
		}

		/// <summary>
		/// Generates the <c>"Res.cs"</c> file for a given module.
		/// </summary>
		/// <param name="moduleDirectory">The module directory.</param>
		/// <param name="defaultNamespace">The default namespace.</param>
		/// <param name="moduleName">Name of the module.</param>
		static void GenerateResFile(string moduleDirectory, string defaultNamespace, string moduleName)
		{
			ResourceManager manager = new ResourceManager (moduleDirectory);
			
			manager.DefineDefaultModuleName (moduleName);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			Generator generator = new Generator (buffer);
			
			buffer.Append ("//\tAutomatically generated by ResGenerator, on ");
			buffer.Append (System.DateTime.Now.ToString ("G", System.Threading.Thread.CurrentThread.CurrentCulture));
			buffer.Append ("\n");
			buffer.Append ("//\tDo not edit manually.\n\n");
			
			generator.BeginBlock ("namespace", defaultNamespace);
			generator.BeginBlock ("public sealed class", "Res");
			
			foreach (string bundleId in manager.GetBundleIds ("*", "*", ResourceLevel.Default))
			{
				ResourceBundle bundle = manager.GetBundle (bundleId, ResourceLevel.Default);

				if (bundle == null)
				{
					System.Console.Error.WriteLine ("Bundle {0} could not be loaded.", bundleId);
					continue;
				}

				string bundleType = bundle.Type;

				switch (bundleType)
				{
					case "Strings":
						Application.GenerateStrings (buffer, generator, bundleId, bundle);
						break;
				}
			}
			
			buffer.Append (generator.Tabs);
			buffer.Append ("\n");
			generator.BeginBlock ("public static void", "Initialise(System.Type type, string name)");
			buffer.Append (generator.Tabs);
			buffer.Append (@"_manager = new Epsitec.Common.Support.ResourceManager (type);");
			buffer.Append ("\n");
			buffer.Append (generator.Tabs);
			buffer.Append (@"_manager.DefineDefaultModuleName (name);");
			buffer.Append ("\n");
			generator.EndBlock ();
			buffer.Append (generator.Tabs);
			buffer.Append ("\n");
			generator.BeginBlock ("public static Epsitec.Common.Support.ResourceManager", "Manager");
			buffer.Append (generator.Tabs);
			buffer.Append (@"get { return _manager; }");
			buffer.Append ("\n");
			generator.EndBlock ();
			buffer.Append (generator.Tabs);
			buffer.Append ("\n");
			buffer.Append (generator.Tabs);
			buffer.Append ("private static Epsitec.Common.Support.ResourceManager _manager = Epsitec.Common.Support.Resources.DefaultManager;");
			buffer.Append ("\n");
			
			generator.EndBlock ();
			generator.EndBlock ();
			
			using (System.IO.StreamWriter file = new System.IO.StreamWriter (System.IO.Path.Combine (moduleDirectory, "Res.cs"), false, System.Text.Encoding.UTF8))
			{
				file.Write (buffer.ToString ());
			}
		}

		static void GenerateStrings(System.Text.StringBuilder buffer, Generator generator, string bundleId, ResourceBundle bundle)
		{
			string prefix   = "";
			bool addNewline = false;

			generator.BeginBlock ("public sealed class", bundleId);

			string[] fields   = bundle.FieldNames;
			string[] sortKeys = new string[fields.Length];

			for (int j = 0; j < fields.Length; j++)
			{
				int pos = fields[j].LastIndexOf ('.');

				if (pos < 0)
				{
					sortKeys[j] = fields[j];
				}
				else
				{
					sortKeys[j] = string.Concat (fields[j].Substring (0, pos), "!", fields[j].Substring (pos+1));
				}
			}

			System.Array.Sort (sortKeys, fields);

			for (int j = 0; j < fields.Length; j++)
			{
				string field = fields[j];

				while ((prefix != "") && (field.StartsWith (prefix + ".") == false))
				{
					//	Remonte d'un niveau dans la hi�rarchie des classes.

					string[] args = prefix.Split ('.');
					string last = args[args.Length-1];

					generator.EndBlock ();

					prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length - last.Length - 1));
					addNewline = true;
				}

				string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length + 1);

				if (addNewline)
				{
					buffer.Append (generator.Tabs);
					buffer.Append ("\n");
					addNewline = false;
				}

				//	Cr�e les classes manquantes, si besoin :

				while (delta.IndexOf ('.') > -1)
				{
					string[] args = delta.Split ('.');
					string elem = args[0];

					generator.BeginBlock ("public sealed class", elem);

					if (prefix.Length == 0)
					{
						prefix = elem;
					}
					else
					{
						prefix = string.Concat (prefix, ".", elem);
					}

					delta = field.Substring (prefix.Length + 1);
				}

				//	Cr�e l'accesseur pour le champ actuel :

				buffer.Append (generator.Tabs);

				Support.Druid druid = bundle[field].Druid;

				buffer.Append ("public static string ");
				buffer.Append (delta);
				buffer.Append (@" { get { return GetText (""");
				buffer.Append (bundleId);
				buffer.Append (@"""");

				if (druid.Type == Support.DruidType.ModuleRelative)
				{
					buffer.Append (@", """);
					buffer.Append (druid.ToFieldName ());
					buffer.Append (@"""");
				}
				else
				{
					string[] elems = field.Split ('.');

					for (int k = 0; k < elems.Length; k++)
					{
						buffer.Append (@", """);
						buffer.Append (elems[k]);
						buffer.Append (@"""");
					}
				}

				buffer.Append ("); } }\n");
			}

			//	Referme les classes ouvertes :

			if (prefix.Length > 0)
			{
				string[] args = prefix.Split ('.');

				for (int j = 0; j < args.Length; j++)
				{
					generator.EndBlock ();
				}
			}
			buffer.Append (generator.Tabs);
			buffer.Append ("\n");

			generator.BeginBlock ("public static string", "GetString(params string[] path)");

			buffer.Append (generator.Tabs);
			buffer.Append (@"string field = string.Join (""."", path);");
			buffer.Append ("\n");
			buffer.Append (generator.Tabs);
			buffer.Append (@"return _bundle[field].AsString;");
			buffer.Append ("\n");

			generator.EndBlock ();

			buffer.Append (generator.Tabs);
			buffer.Append ("\n");

			buffer.Append (generator.Tabs);
			buffer.Append ("#region Internal Support Code\n");

			generator.BeginBlock ("private static string", "GetText(string bundle, params string[] path)");

			buffer.Append (generator.Tabs);
			buffer.Append (@"string field = string.Join (""."", path);");
			buffer.Append ("\n");
			buffer.Append (generator.Tabs);
			buffer.Append (@"return _bundle[field].AsString;");
			buffer.Append ("\n");

			generator.EndBlock ();

			buffer.Append (generator.Tabs);
			buffer.Append (@"private static Epsitec.Common.Support.ResourceBundle _bundle = _manager.GetBundle (""");
			buffer.Append (bundleId);
			buffer.Append (@""");");
			buffer.Append ("\n");

			buffer.Append (generator.Tabs);
			buffer.Append ("#endregion\n");

			generator.EndBlock ();
		}
	}
}
