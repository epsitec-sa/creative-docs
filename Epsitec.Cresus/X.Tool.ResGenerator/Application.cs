//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Tool.ResGenerator
{
	class Application
	{
		[System.STAThread] static void Main(string[] args)
		{
			if (args.Length == 2)
			{
				Application.Process (args);
				System.Console.Out.WriteLine ("Done.");
			}
			else if (System.IO.File.Exists ("build.ini"))
			{
				string save_dir = System.IO.Directory.GetCurrentDirectory ();
				
				using (System.IO.StreamReader reader = new System.IO.StreamReader ("build.ini", System.Text.Encoding.UTF8))
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
							Application.Process (args);
						}
						else
						{
							System.Console.Error.WriteLine ("Error: build.ini:{0}: syntax error.", count);
						}
					}
				}
				
				System.Console.Out.WriteLine ("Done.");
			}
			else
			{
				System.Console.Out.WriteLine ("Invalid command line and no 'build.ini' script found.");
			}
		}
		
		public static void Process(string[] args)
		{
			string default_namespace = args[0].Trim ();
			string app_directory     = args[1].Trim ();
			string app_name          = "x";
			
			if (app_directory.IndexOf ("|") > 0)
			{
				string[] split = app_directory.Split ('|');
				
				app_directory = split[0];
				app_name      = split[1];
			}
			
			string   save_dir = System.IO.Directory.GetCurrentDirectory ();
			string   root_dir = System.IO.Path.Combine (app_directory, "resources");
			
//			System.IO.Directory.SetCurrentDirectory (app_directory);
			Application.GenerateResFile (app_directory, default_namespace, app_name);
//			System.IO.Directory.SetCurrentDirectory (save_dir);
			
			System.Console.Out.WriteLine ("Generated 'Res.cs' for {0}, application '{1}'", app_directory, app_name);
		}
		
		public static void GenerateResFile(string app_directory, string default_namespace, string app_name)
		{
			ResourceManager manager = new ResourceManager (app_directory);
			
			manager.SetupApplication (app_name);
			
			string[] names = manager.GetBundleIds ("*", "String", ResourceLevel.Default);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			Generator generator = new Generator (buffer);
			
			buffer.Append ("//\tAutomatically generated by ResGenerator, on ");
			buffer.Append (System.DateTime.Now.ToString ("G", System.Threading.Thread.CurrentThread.CurrentCulture));
			buffer.Append ("\n");
			buffer.Append ("//\tDo not edit manually.\n\n");
			
			generator.BeginBlock ("namespace", default_namespace);
			generator.BeginBlock ("public sealed class", "Res");
			
			for (int i = 0; i < names.Length; i++)
			{
				string file_name = names[i];
				string prefix    = "";
				bool add_newline = false;
				
				generator.BeginBlock ("public sealed class", file_name);
				
				ResourceBundle bundle = manager.GetBundle (file_name, ResourceLevel.Default);
				
				string[] fields    = bundle.FieldNames;
				string[] sort_keys = new string[fields.Length];
				
				for (int j = 0; j < fields.Length; j++)
				{
					int pos = fields[j].LastIndexOf ('.');
					
					if (pos < 0)
					{
						sort_keys[j] = fields[j];
					}
					else
					{
						sort_keys[j] = string.Concat (fields[j].Substring (0, pos), "!", fields[j].Substring (pos+1));
					}
				}
				
				System.Array.Sort (sort_keys, fields);
				
				for (int j = 0; j < fields.Length; j++)
				{
					string field = fields[j];
					
					while (field.StartsWith (prefix) == false)
					{
						//	Remonte d'un niveau dans la hi�rarchie des classes.
						
						string[] args = prefix.Split ('.');
						string   last = args[args.Length-1];
						
						generator.EndBlock ();
						
						prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length - last.Length - 1));
						add_newline = true;
					}
					
					string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length + 1);
					
					if (add_newline)
					{
						buffer.Append (generator.Tabs);
						buffer.Append ("\n");
						add_newline = false;
					}
					
					//	Cr�e les classes manquantes, si besoin :
					
					while (delta.IndexOf ('.') > -1)
					{
						string[] args = delta.Split ('.');
						string   elem = args[0];
						
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
					buffer.Append ("public static string ");
					buffer.Append (delta);
					buffer.Append (@" { get { return GetText (""");
					buffer.Append (file_name);
					buffer.Append (@"""");
					
					string[] elems = field.Split ('.');
					
					for (int k = 0; k < elems.Length; k++)
					{
						buffer.Append (@", """);
						buffer.Append (elems[k]);
						buffer.Append (@"""");
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
				buffer.Append (file_name);
				buffer.Append (@""");");
				buffer.Append ("\n");
				
				buffer.Append (generator.Tabs);
				buffer.Append ("#endregion\n");
				
				generator.EndBlock ();
			}
			
			buffer.Append (generator.Tabs);
			buffer.Append ("\n");
			generator.BeginBlock ("public static void", "Initialise(System.Type type, string name)");
			buffer.Append (generator.Tabs);
			buffer.Append (@"_manager = new Epsitec.Common.Support.ResourceManager (type);");
			buffer.Append ("\n");
			buffer.Append (generator.Tabs);
			buffer.Append (@"_manager.SetupApplication (name);");
			buffer.Append ("\n");
			generator.EndBlock ();
			buffer.Append (generator.Tabs);
			buffer.Append ("\n");
			buffer.Append (generator.Tabs);
			buffer.Append ("private static Epsitec.Common.Support.ResourceManager _manager = Epsitec.Common.Support.Resources.DefaultManager;");
			buffer.Append ("\n");
			
			generator.EndBlock ();
			generator.EndBlock ();
			
			using (System.IO.StreamWriter file = new System.IO.StreamWriter (System.IO.Path.Combine (app_directory, "Res.cs"), false, System.Text.Encoding.UTF8))
			{
				file.Write (buffer.ToString ());
			}
		}
		
		
		public class Generator
		{
			public Generator(System.Text.StringBuilder buffer)
			{
				this.buffer    = buffer;
				this.tab_count = 0;
			}
			
			
			
			public void BeginBlock(string prefix, string name)
			{
				if (char.IsLower (name[0]))
				{
					name = char.ToUpper (name[0]) + name.Substring (1);
				}
				
				this.buffer.Append (this.Tabs);
				this.buffer.Append (prefix);
				this.buffer.Append (" ");
				this.buffer.Append (name);
				this.buffer.Append ("\n");
				
				this.buffer.Append (this.Tabs);
				this.buffer.Append ("{\n");
				
				this.tab_count++;
			}
			
			public void EndBlock()
			{
				this.tab_count--;
				
				this.buffer.Append (this.Tabs);
				this.buffer.Append ("}\n");
			}
			
			
			public string						Tabs
			{
				get
				{
					return new string ('\t', this.tab_count);
				}
			}
			
			
			private System.Text.StringBuilder	buffer;
			private int							tab_count;
		}
	}
}
