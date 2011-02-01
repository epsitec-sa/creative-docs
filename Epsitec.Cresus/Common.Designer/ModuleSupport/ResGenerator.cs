//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGeneration;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Designer.ModuleSupport
{
	public static class ResGenerator
	{
		public static CodeFormatter GenerateResFile(ResourceManager resourceManager, ResourceModuleInfo moduleInfo)
		{
			string sourceNamespace = moduleInfo.SourceNamespace;
			string moduleName = moduleInfo.FullId.Name;
			ResourceTextMode textMode = moduleInfo.TextMode;

			return ResGenerator.GenerateResFile (resourceManager, sourceNamespace, moduleName, textMode);
		}
		
		/// <summary>
		/// Generates the <c>"Res.cs"</c> file for a given module.
		/// </summary>
		public static CodeFormatter GenerateResFile(ResourceManager manager, string defaultNamespace, string moduleName, ResourceTextMode textMode)
		{
			CodeFormatter formatter = new CodeFormatter ();
			ResGenerator.commandsGenerated = false;

			System.Diagnostics.Debug.Assert (manager.DefaultModuleId >= 0);

			CodeHelper.EmitHeader (formatter);

			formatter.WriteBeginNamespace (defaultNamespace);
			formatter.WriteBeginClass (new CodeAttributes (CodeAccessibility.Static), "Res");
			
			string[] bundleIds = manager.GetBundleIds ("*", "*", ResourceLevel.Default);

			bool addLine = false;

			foreach (string bundleId in bundleIds)
			{
				ResourceBundle bundle = manager.GetBundle (bundleId, ResourceLevel.Default);

				if (bundle == null)
				{
					System.Console.Error.WriteLine ("Bundle {0} could not be loaded.", bundleId);
					continue;
				}

				string bundleType = bundle.Type;

				if (addLine)
				{
					formatter.WriteCodeLine ();
				}

				formatter.WriteCodeLine ("//\tCode mapping for '", bundleType, "' resources");
				
				switch (bundleType)
				{
					case Resources.StringTypeName:
						ResGenerator.GenerateStrings (manager, formatter, defaultNamespace, textMode, bundleId, bundle);
						addLine = true;
						break;
					
					case Resources.CaptionTypeName:
						ResGenerator.GenerateCommandsCaptionsAndTypes (manager, formatter, defaultNamespace, bundleId, bundle);
						addLine = true;
						break;
				}
			}

			formatter.WriteCodeLine ();

			formatter.WriteBeginMethod (CodeHelper.StaticClassConstructorAttributes, "Res()");
			formatter.WriteCodeLine (@"Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));");
			formatter.WriteCodeLine (@"Res._manager.DefineDefaultModuleName (""", moduleName, @""");");
			if (ResGenerator.commandsGenerated)
			{
				formatter.WriteCodeLine (@"Commands._Initialize ();");
			}
			formatter.WriteEndMethod ();
			formatter.WriteCodeLine ();

			formatter.WriteBeginMethod (CodeHelper.PublicStaticMethodAttributes, "void Initialize()");
			formatter.WriteEndMethod ();
			formatter.WriteCodeLine ();

			formatter.WriteBeginProperty (CodeHelper.PublicStaticPropertyAttributes, "global::Epsitec.Common.Support.ResourceManager Manager");
			formatter.WriteBeginGetter (CodeAttributes.Default);
			formatter.WriteCodeLine ("return Res._manager;");
			formatter.WriteEndGetter ();
			formatter.WriteEndProperty ();
			formatter.WriteCode ();
			
			formatter.WriteBeginProperty (CodeHelper.PublicStaticPropertyAttributes, "int ModuleId");
			formatter.WriteBeginGetter (CodeAttributes.Default);
			formatter.WriteCodeLine ("return Res._moduleId;");
			formatter.WriteEndGetter ();
			formatter.WriteEndProperty ();
			formatter.WriteCode ();
			
			formatter.WriteField (CodeHelper.PrivateStaticReadOnlyFieldAttributes,
				"global::Epsitec.Common.Support.ResourceManager _manager;");

			formatter.WriteField (CodeHelper.PrivateConstFieldAttributes,
				"int _moduleId = ",
				string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", manager.DefaultModuleId),
				";");

			formatter.WriteEndClass ();
			formatter.WriteEndNamespace ();
			formatter.Flush ();

			return formatter;
		}

		static void GenerateCommandsCaptionsAndTypes(ResourceManager manager, CodeFormatter formatter, string defaultNamespace, string bundleId, ResourceBundle bundle)
		{
			List<string> cmdFields = new List<string> ();
			List<string> capFields = new List<string> ();
			List<string> typFields = new List<string> ();
			List<string> valFields = new List<string> ();
			List<string> fldFields = new List<string> ();

			foreach (string field in bundle.FieldNames)
			{
				if (field.StartsWith ("Cmd."))
				{
					cmdFields.Add (field);
				}
				else if (field.StartsWith ("Cap."))
				{
					capFields.Add (field);
				}
				else if (field.StartsWith ("Typ."))
				{
					typFields.Add (field);
				}
				else if (field.StartsWith ("Val."))
				{
					valFields.Add (field);
				}
				else if (field.StartsWith ("Fld."))
				{
					fldFields.Add (field);
				}
				else
				{
					System.Console.Error.WriteLine ("Field {0} not supported in {1}.", field, bundleId);
				}
			}

			if (cmdFields.Count > 0)
			{
				formatter.WriteCodeLine ();
				ResGenerator.GenerateCommands (manager, formatter, defaultNamespace, bundleId, bundle, cmdFields);

				formatter.WriteCodeLine ();
				ResGenerator.GenerateCommandIds (manager, formatter, defaultNamespace, bundleId, bundle, cmdFields);
			}

			if (capFields.Count > 0)
			{
				formatter.WriteCodeLine ();
				ResGenerator.GenerateCaptions (manager, formatter, defaultNamespace, bundleId, bundle, capFields);
				
				formatter.WriteCodeLine ();
				ResGenerator.GenerateCaptionIds (manager, formatter, defaultNamespace, bundleId, bundle, capFields);
			}

			if (typFields.Count > 0)
			{
				formatter.WriteCodeLine ();
				ResGenerator.GenerateTypes (manager, formatter, defaultNamespace, bundleId, bundle, typFields);
			}

			if (valFields.Count > 0)
			{
				formatter.WriteCodeLine ();
				ResGenerator.GenerateValues (manager, formatter, defaultNamespace, bundleId, bundle, valFields);
			}

			if (fldFields.Count > 0)
			{
				formatter.WriteCodeLine ();
				ResGenerator.GenerateFields (manager, formatter, defaultNamespace, bundleId, bundle, fldFields);
			}
		}

		static void GenerateCommands(ResourceManager manager, CodeFormatter formatter, string defaultNamespace, string bundleId, ResourceBundle bundle, List<string> cmdFields)
		{
			ResGenerator.commandsGenerated = true;
			string prefix   = "";
			bool addNewline = false;

			formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, "Commands");

			string[] fields   = new string[cmdFields.Count];
			string[] sortKeys = new string[cmdFields.Count];

			for (int i = 0; i < fields.Length; i++)
			{
				fields[i] = cmdFields[i];

				int pos = fields[i].LastIndexOf ('.');
				if (pos < 0)
				{
					sortKeys[i] = fields[i];
				}
				else
				{
					sortKeys[i] = string.Concat (fields[i].Substring (0, pos), "!", fields[i].Substring (pos+1));
				}
			}

			System.Array.Sort (sortKeys, fields);

			List<string> classes = new List<string> ();

			for (int i = 0; i < fields.Length; i++)
			{
				string field = fields[i].Substring (4);

				while (prefix != "" && !field.StartsWith (prefix + "."))
				{
					//	Remonte d'un niveau dans la hiérarchie des classes.
					string[] args = prefix.Split ('.');
					string last = args[args.Length-1];

					formatter.WriteEndClass ();

					prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length-last.Length-1));
					addNewline = true;
				}

				string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length+1);

				if (addNewline)
				{
					formatter.WriteCodeLine ();
					addNewline = false;
				}

				//	Crée les classes manquantes, si besoin :
				while (delta.IndexOf ('.') > -1)
				{
					string[] args = delta.Split ('.');
					string elem = args[0];

					formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, elem);
					
					System.Text.StringBuilder fullClass = new System.Text.StringBuilder ();
					args = field.Split ('.');

					for (int j = 0; j < args.Length-1; j++)
					{
						if (j > 0)
						{
							fullClass.Append (".");
						}

						fullClass.Append (args[j]);
					}

					if (!classes.Contains (fullClass.ToString ()))
					{
						classes.Add (fullClass.ToString ());
						System.Diagnostics.Debug.WriteLine (fullClass.ToString ());
					}

					formatter.WriteBeginMethod (CodeHelper.InternalStaticMethodAttributes, "void _Initialize()");
					formatter.WriteEndMethod ();
					formatter.WriteCodeLine ();

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

				//	Crée l'accesseur pour le champ actuel :

				Support.Druid localDruid = bundle[fields[i]].Id;
				Druid moduleDruid = new Druid (localDruid, bundle.Module.Id);

				formatter.WriteCodeLine ("//\tdesigner:cap/", moduleDruid.ToString ().Trim ('[', ']'));

				formatter.WriteField (CodeHelper.PublicStaticReadOnlyFieldAttributes,
					@"global::Epsitec.Common.Widgets.Command ",
					delta,
					@" = global::Epsitec.Common.Widgets.Command.Get (new global::Epsitec.Common.Support.Druid (_moduleId, ",
					localDruid.DeveloperAndPatchLevel.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ",
					localDruid.Local.ToString (System.Globalization.CultureInfo.InvariantCulture), "));");
			}

			//	Referme les classes ouvertes :
			if (prefix.Length > 0)
			{
				string[] args = prefix.Split ('.');

				for (int j=0; j<args.Length; j++)
				{
					formatter.WriteEndClass ();
				}
			}
			
			formatter.WriteCodeLine ();
			formatter.WriteBeginMethod (CodeHelper.InternalStaticMethodAttributes, "void _Initialize()");
			
			for (int i = 0; i < classes.Count; i++)
			{
				formatter.WriteCodeLine (classes[i], ".", "_Initialize ();");
			}

			formatter.WriteEndMethod ();
			formatter.WriteEndClass ();
		}

		static void GenerateCommandIds(ResourceManager manager, CodeFormatter formatter, string defaultNamespace, string bundleId, ResourceBundle bundle, List<string> cmdFields)
		{
			ResGenerator.GenerateGenericCaptions (manager, formatter, defaultNamespace, bundleId, bundle, cmdFields,
				"CommandIds",
				(delta, localDruid) =>
				{
					Druid moduleDruid = new Druid (localDruid, bundle.Module.Id);
					formatter.WriteField (CodeHelper.PublicConstFieldAttributes, "long ", delta, @" = 0x", moduleDruid.ToLong ().ToString ("X"), "L;");
				});
		}

		static void GenerateCaptions(ResourceManager manager, CodeFormatter formatter, string defaultNamespace, string bundleId, ResourceBundle bundle, List<string> capFields)
		{
			ResGenerator.GenerateGenericCaptions (manager, formatter, defaultNamespace, bundleId, bundle, capFields,
				"Captions",
				(delta, localDruid) =>
				{
					formatter.WriteBeginProperty (CodeHelper.PublicStaticPropertyAttributes, string.Concat (@"global::Epsitec.Common.Types.Caption ", delta));
					formatter.WriteBeginGetter (CodeAttributes.Default);
					formatter.WriteCodeLine ("return global::", defaultNamespace, ".Res.", "_manager", ".GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, ",
						localDruid.DeveloperAndPatchLevel.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ",
						localDruid.Local.ToString (System.Globalization.CultureInfo.InvariantCulture), "));");
					formatter.WriteEndGetter ();
					formatter.WriteEndProperty ();
				});
		}
		
		static void GenerateCaptionIds(ResourceManager manager, CodeFormatter formatter, string defaultNamespace, string bundleId, ResourceBundle bundle, List<string> capFields)
		{
			ResGenerator.GenerateGenericCaptions (manager, formatter, defaultNamespace, bundleId, bundle, capFields,
				"CaptionIds",
				(delta, localDruid) =>
				{
					formatter.WriteField (CodeHelper.PublicStaticReadOnlyFieldAttributes,
						@"global::Epsitec.Common.Support.Druid ",
						delta,
						@" = new global::Epsitec.Common.Support.Druid (_moduleId, ",
						localDruid.DeveloperAndPatchLevel.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ",
						localDruid.Local.ToString (System.Globalization.CultureInfo.InvariantCulture), ");");
				});
		}


		static void GenerateGenericCaptions(ResourceManager manager, CodeFormatter formatter, string defaultNamespace, string bundleId, ResourceBundle bundle, List<string> capFields, string className,
			System.Action<string, Druid> fieldWriter)
		{
			string prefix   = "";
			bool addNewline = false;

			formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, className);

			string[] fields   = new string[capFields.Count];
			string[] sortKeys = new string[capFields.Count];

			for (int i = 0; i < fields.Length; i++)
			{
				fields[i] = capFields[i];

				int pos = fields[i].LastIndexOf ('.');
				if (pos < 0)
				{
					sortKeys[i] = fields[i];
				}
				else
				{
					sortKeys[i] = string.Concat (fields[i].Substring (0, pos), "!", fields[i].Substring (pos+1));
				}
			}

			System.Array.Sort (sortKeys, fields);

			for (int i = 0; i < fields.Length; i++)
			{
				string field = fields[i].Substring (4);

				while (prefix != "" && !field.StartsWith (prefix + "."))
				{
					//	Remonte d'un niveau dans la hiérarchie des classes.
					string[] args = prefix.Split ('.');
					string last = args[args.Length-1];

					formatter.WriteEndClass ();

					prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length-last.Length-1));
					addNewline = true;
				}

				string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length+1);

				if (addNewline)
				{
					formatter.WriteCodeLine ();
					addNewline = false;
				}

				//	Crée les classes manquantes, si besoin :
				while (delta.IndexOf ('.') > -1)
				{
					string[] args = delta.Split ('.');
					string elem = args[0];

					formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, elem);

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

				//	Crée l'accesseur pour le champ actuel :

				Support.Druid localDruid = bundle[fields[i]].Id;
				Druid moduleDruid = new Druid (localDruid, bundle.Module.Id);

				formatter.WriteCodeLine ("//\tdesigner:cap/", moduleDruid.ToString ().Trim ('[', ']'));

				fieldWriter (delta, localDruid);
			}

			//	Referme les classes ouvertes :
			if (prefix.Length > 0)
			{
				string[] args = prefix.Split ('.');

				for (int j=0; j<args.Length; j++)
				{
					formatter.WriteEndClass ();
				}
			}

			formatter.WriteCodeLine ();
			formatter.WriteEndClass ();
		}




		static void GenerateValues(ResourceManager manager, CodeFormatter formatter, string defaultNamespace, string bundleId, ResourceBundle bundle, List<string> valFields)
		{
#if false
			string prefix   = "";
			bool addNewline = false;

			generator.BeginBlock ("public static class", "Values");

			string[] fields   = new string[valFields.Count];
			string[] sortKeys = new string[valFields.Count];

			for (int i = 0; i < fields.Length; i++)
			{
				fields[i] = valFields[i];

				int pos = fields[i].LastIndexOf ('.');
				if (pos < 0)
				{
					sortKeys[i] = fields[i];
				}
				else
				{
					sortKeys[i] = string.Concat (fields[i].Substring (0, pos), "!", fields[i].Substring (pos+1));
				}
			}

			System.Array.Sort (sortKeys, fields);

			for (int i = 0; i < fields.Length; i++)
			{
				string field = fields[i].Substring (4);

				while (prefix != "" && !field.StartsWith (prefix + "."))
				{
					//	Remonte d'un niveau dans la hiérarchie des classes.
					string[] args = prefix.Split ('.');
					string last = args[args.Length-1];

					generator.EndBlock ();

					prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length-last.Length-1));
					addNewline = true;
				}

				string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length+1);

				if (addNewline)
				{
					buffer.Append (generator.Tabs);
					buffer.Append ("\n");
					addNewline = false;
				}

				//	Crée les classes manquantes, si besoin :
				while (delta.IndexOf ('.') > -1)
				{
					string[] args = delta.Split ('.');
					string elem = args[0];

					generator.BeginBlock ("public static class", elem);

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

				//	Crée l'accesseur pour le champ actuel :

				Support.Druid localDruid = bundle[fields[i]].Id;
				Druid moduleDruid = new Druid (localDruid, bundle.Module.Id);

				buffer.Append (string.Concat (generator.Tabs, "//\tdesigner:cap/", moduleDruid.ToString ().Trim ('[', ']'), "\n"));
				buffer.Append (generator.Tabs);

				buffer.Append ("public static global::Epsitec.Common.Types.Caption ");
				buffer.Append (delta);
				buffer.Append (@" { get { return Res._manager.GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, ");
				buffer.Append (localDruid.DeveloperAndPatchLevel.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (", ");
				buffer.Append (localDruid.Local.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (")); } }\n");
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

			generator.EndBlock ();
#endif
		}

		static void GenerateFields(ResourceManager manager, CodeFormatter formatter, string defaultNamespace, string bundleId, ResourceBundle bundle, List<string> fldFields)
		{
#if false
			string prefix   = "";
			bool addNewline = false;

			generator.BeginBlock ("public static class", "Fields");

			string[] fields   = new string[fldFields.Count];
			string[] sortKeys = new string[fldFields.Count];

			for (int i = 0; i < fields.Length; i++)
			{
				fields[i] = fldFields[i];

				int pos = fields[i].LastIndexOf ('.');
				if (pos < 0)
				{
					sortKeys[i] = fields[i];
				}
				else
				{
					sortKeys[i] = string.Concat (fields[i].Substring (0, pos), "!", fields[i].Substring (pos+1));
				}
			}

			System.Array.Sort (sortKeys, fields);

			for (int i = 0; i < fields.Length; i++)
			{
				string field = fields[i].Substring (4);

				while (prefix != "" && !field.StartsWith (prefix + "."))
				{
					//	Remonte d'un niveau dans la hiérarchie des classes.
					string[] args = prefix.Split ('.');
					string last = args[args.Length-1];

					generator.EndBlock ();

					prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length-last.Length-1));
					addNewline = true;
				}

				string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length+1);

				if (addNewline)
				{
					buffer.Append (generator.Tabs);
					buffer.Append ("\n");
					addNewline = false;
				}

				//	Crée les classes manquantes, si besoin :
				while (delta.IndexOf ('.') > -1)
				{
					string[] args = delta.Split ('.');
					string elem = args[0];

					generator.BeginBlock ("public static class", elem);

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

				//	Crée l'accesseur pour le champ actuel :

				Support.Druid localDruid = bundle[fields[i]].Id;
				Druid moduleDruid = new Druid (localDruid, bundle.Module.Id);

				buffer.Append (string.Concat (generator.Tabs, "//\tdesigner:cap/", moduleDruid.ToString ().Trim ('[', ']'), "\n"));
				buffer.Append (generator.Tabs);

				buffer.Append ("public static readonly global::Epsitec.Common.Support.Druid ");
				buffer.Append (delta);
				buffer.Append (@" = new global::Epsitec.Common.Support.Druid (_moduleId, ");
				buffer.Append (localDruid.DeveloperAndPatchLevel.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (", ");
				buffer.Append (localDruid.Local.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (");\n");
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

			generator.EndBlock ();
#endif
		}

		static void GenerateTypes(ResourceManager manager, CodeFormatter formatter, string defaultNamespace, string bundleId, ResourceBundle bundle, List<string> typFields)
		{
#if false
			string prefix   = "";
			bool addNewline = false;

			generator.BeginBlock ("public static class", "Types");

			string[] fields   = new string[typFields.Count];
			string[] sortKeys = new string[typFields.Count];

			for (int i = 0; i < fields.Length; i++)
			{
				fields[i] = typFields[i];

				int pos = fields[i].LastIndexOf ('.');
				if (pos < 0)
				{
					sortKeys[i] = fields[i];
				}
				else
				{
					sortKeys[i] = string.Concat (fields[i].Substring (0, pos), "!", fields[i].Substring (pos+1));
				}
			}

			System.Array.Sort (sortKeys, fields);

			string[] wellKnownPrefixes = new string[] { "CollectionType.", "StructuredType." };

			for (int i = 0; i < fields.Length; i++)
			{
				string field = fields[i].Substring (4);

				foreach (string wellKnownPrefix in wellKnownPrefixes)
				{
					if (field.StartsWith (wellKnownPrefix))
					{
						field = field.Substring (wellKnownPrefix.Length);
						break;
					}
				}

				while (prefix != "" && !field.StartsWith (prefix + "."))
				{
					//	Remonte d'un niveau dans la hiérarchie des classes.
					string[] args = prefix.Split ('.');
					string last = args[args.Length-1];

					generator.EndBlock ();

					prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length-last.Length-1));
					addNewline = true;
				}

				string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length+1);

				if (addNewline)
				{
					buffer.Append (generator.Tabs);
					buffer.Append ("\n");
					addNewline = false;
				}

				//	Crée les classes manquantes, si besoin :
				while (delta.IndexOf ('.') > -1)
				{
					string[] args = delta.Split ('.');
					string elem = args[0];

					generator.BeginBlock ("public static class", elem);

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

				//	Crée l'accesseur pour le champ actuel :
				Support.ResourceBundle.Field f = bundle[fields[i]];
				Support.Druid localDruid = f.Id;

				string s = f.AsString;

				if (string.IsNullOrEmpty (s))
				{
					continue;
				}

				Types.Caption caption = new Types.Caption ();
				caption.DeserializeFromString (s, manager);

				Types.AbstractType type = Types.TypeRosetta.CreateTypeObject (caption);

				if (type == null)
				{
					continue;
				}

				//	Cherche le nom du type complet, par exemple "Epsitec.Common.Types.StringType"
				string typeName = type.ToString ();

				Druid moduleDruid = new Druid (localDruid, bundle.Module.Id);

				buffer.Append (string.Concat (generator.Tabs, "//\tdesigner:cap/", moduleDruid.ToString ().Trim ('[', ']'), "\n"));
				buffer.Append (generator.Tabs);

				buffer.Append ("public static readonly global::");
				buffer.Append (typeName);
				buffer.Append (" ");
				buffer.Append (delta);
				buffer.Append (" = (global::");
				buffer.Append (typeName);
				buffer.Append (") global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (");
				buffer.Append ("new global::Epsitec.Common.Support.Druid (_moduleId, ");
				buffer.Append (localDruid.DeveloperAndPatchLevel.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append (", ");
				buffer.Append (localDruid.Local.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append ("));\n");
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

			generator.EndBlock ();
#endif
		}

		static void GenerateStrings(ResourceManager manager, CodeFormatter formatter, string defaultNamespace, ResourceTextMode textMode, string bundleId, ResourceBundle bundle)
		{
			formatter.WriteCodeLine ();

			string prefix   = "";
			bool addNewline = false;

			formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, bundleId);
			
			string[] fields   = bundle.FieldNames;
			string[] sortKeys = new string[fields.Length];

			for (int i = 0; i < fields.Length; i++)
			{
				int pos = fields[i].LastIndexOf ('.');

				if (pos < 0)
				{
					sortKeys[i] = fields[i];
				}
				else
				{
					sortKeys[i] = string.Concat (fields[i].Substring (0, pos), "!", fields[i].Substring (pos+1));
				}
			}

			System.Array.Sort (sortKeys, fields);

			for (int i = 0; i < fields.Length; i++)
			{
				string field = fields[i];

				while ((prefix != "") && (field.StartsWith (prefix + ".") == false))
				{
					//	Remonte d'un niveau dans la hiérarchie des classes.

					string[] args = prefix.Split ('.');
					string last = args[args.Length-1];

					formatter.WriteEndClass ();

					prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length - last.Length - 1));
					addNewline = true;
				}

				string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length + 1);

				if (addNewline)
				{
					formatter.WriteCodeLine ();
					addNewline = false;
				}

				//	Crée les classes manquantes, si besoin :

				while (delta.IndexOf ('.') > -1)
				{
					string[] args = delta.Split ('.');
					string elem = args[0];

					formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, elem);

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

				//	Crée l'accesseur pour le champ actuel :

				Druid localDruid = bundle[field].Id;
				Druid moduleDruid = new Druid (localDruid, bundle.Module.Id);

				System.Text.StringBuilder id = new System.Text.StringBuilder ();

				if (localDruid.Type == Support.DruidType.ModuleRelative)
				{
					id.Append (@"global::Epsitec.Common.Support.Druid.FromFieldId (");
					id.Append (localDruid.ToFieldId ());
					id.Append (@")");
				}
				else
				{
					id.Append (@"""");
					id.Append (bundleId);
					id.Append (@"""");

					string[] elems = field.Split ('.');

					for (int k = 0; k < elems.Length; k++)
					{
						id.Append (@", """);
						id.Append (elems[k]);
						id.Append (@"""");
					}
				}
				
				formatter.WriteCodeLine ("//\tdesigner:str/", moduleDruid.ToString ().Trim ('[', ']'));

				switch (textMode)
				{
					case ResourceTextMode.String:
						formatter.WriteBeginProperty (CodeHelper.PublicStaticPropertyAttributes, string.Concat ("global::System.String ", delta));
						formatter.WriteBeginGetter (CodeAttributes.Default);
						formatter.WriteCodeLine ("return global::", defaultNamespace, ".Res.", bundleId, ".GetString (", id.ToString (), ");");
						break;

					case ResourceTextMode.FormattedText:
						formatter.WriteBeginProperty (CodeHelper.PublicStaticPropertyAttributes, string.Concat ("global::Epsitec.Common.Types.FormattedText ", delta));
						formatter.WriteBeginGetter (CodeAttributes.Default);
						formatter.WriteCodeLine ("return global::", defaultNamespace, ".Res.", bundleId, ".GetText (", id.ToString (), ");");
						break;

					default:
						throw new System.NotImplementedException ();
				}
								
				formatter.WriteEndGetter ();
				formatter.WriteEndProperty ();
			}

			//	Referme les classes ouvertes :

			if (prefix.Length > 0)
			{
				string[] args = prefix.Split ('.');

				for (int j = 0; j < args.Length; j++)
				{
					formatter.WriteEndClass ();
				}
			}

			formatter.WriteCodeLine ();

			formatter.WriteBeginMethod (CodeHelper.PublicStaticMethodAttributes, "global::Epsitec.Common.Types.FormattedText GetText(params string[] path)");
			formatter.WriteCodeLine (@"string field = string.Join (""."", path);");
			formatter.WriteCodeLine (@"return new global::Epsitec.Common.Types.FormattedText (_stringsBundle[field].AsString);");
			formatter.WriteEndMethod ();

			formatter.WriteCodeLine ();

			formatter.WriteBeginMethod (CodeHelper.PublicStaticMethodAttributes, "global::System.String GetString(params string[] path)");
			formatter.WriteCodeLine (@"string field = string.Join (""."", path);");
			formatter.WriteCodeLine (@"return _stringsBundle[field].AsString;");
			formatter.WriteEndMethod ();
			
			formatter.WriteCodeLine ();

			formatter.WriteCodeLine ("#region Internal Support Code");
			formatter.WriteCodeLine ();
			formatter.WriteBeginMethod (CodeHelper.PrivateStaticMethodAttributes, "global::Epsitec.Common.Types.FormattedText GetText(string bundle, params string[] path)");
			formatter.WriteCodeLine ("return new global::Epsitec.Common.Types.FormattedText (global::", defaultNamespace, ".Res.", bundleId, ".GetString (", "bundle, path", "));");
			formatter.WriteEndMethod ();
			formatter.WriteCodeLine ();
			formatter.WriteBeginMethod (CodeHelper.PrivateStaticMethodAttributes, "global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)");
			formatter.WriteCodeLine ("return new global::Epsitec.Common.Types.FormattedText (global::", defaultNamespace, ".Res.", bundleId, ".GetString (", "druid", "));");
			formatter.WriteEndMethod ();
			formatter.WriteCodeLine ();
			formatter.WriteBeginMethod (CodeHelper.PrivateStaticMethodAttributes, "global::System.String GetString(string bundle, params string[] path)");
			formatter.WriteCodeLine (@"string field = string.Join (""."", path);");
			formatter.WriteCodeLine (@"return _stringsBundle[field].AsString;");
			formatter.WriteEndMethod ();
			formatter.WriteCodeLine ();
			formatter.WriteBeginMethod (CodeHelper.PrivateStaticMethodAttributes, "global::System.String GetString(global::Epsitec.Common.Support.Druid druid)");
			formatter.WriteCodeLine (@"return _stringsBundle[druid].AsString;");
			formatter.WriteEndMethod ();
			formatter.WriteCodeLine ();
			
			formatter.WriteField (CodeHelper.PrivateStaticReadOnlyFieldAttributes,
				@"global::Epsitec.Common.Support.ResourceBundle _stringsBundle = Res._manager.GetBundle (""",
				bundleId, @""");");
			formatter.WriteCodeLine ();
			formatter.WriteCodeLine ("#endregion");
			
			formatter.WriteEndClass ();
		}

		private static bool commandsGenerated;
	}
}
