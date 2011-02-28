//	Copyright © 2008-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGeneration;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Designer.ModuleSupport
{
	public class ResGenerator
	{
		public static CodeFormatter GenerateResFile(ResourceManager resourceManager, ResourceModuleInfo moduleInfo)
		{
			string sourceNamespace = moduleInfo.SourceNamespace;
			string moduleName = moduleInfo.FullId.Name;
			ResourceTextMode textMode = moduleInfo.TextMode;

			var generator = new ResGenerator (resourceManager, sourceNamespace, moduleName, textMode);

			return generator.GenerateResFile ();
		}

		private ResGenerator(ResourceManager manager, string defaultNamespace, string moduleName, ResourceTextMode textMode)
		{
			this.manager = manager;
			this.defaultNamespace = defaultNamespace;
			this.moduleName = moduleName;
			this.textMode = textMode;
			this.formatter = new CodeFormatter ();
			this.classes = new List<string> ();
		}
		
		/// <summary>
		/// Generates the <c>"Res.cs"</c> file for a given module.
		/// </summary>
		private CodeFormatter GenerateResFile()
		{
			this.commandsGenerated = false;

			System.Diagnostics.Debug.Assert (this.manager.DefaultModuleId >= 0);

			CodeHelper.EmitHeader (this.formatter);

			this.formatter.WriteBeginNamespace (this.defaultNamespace);
			this.formatter.WriteBeginClass (new CodeAttributes (CodeAccessibility.Static), "Res");
			
			string[] bundleIds = this.manager.GetBundleIds ("*", "*", ResourceLevel.Default);

			bool addLine = false;

			foreach (string bundleId in bundleIds)
			{
				ResourceBundle bundle = this.manager.GetBundle (bundleId, ResourceLevel.Default);

				if (bundle == null)
				{
					System.Console.Error.WriteLine ("Bundle {0} could not be loaded.", bundleId);
					continue;
				}

				string bundleType = bundle.Type;

				if (addLine)
				{
					this.formatter.WriteCodeLine ();
				}

				this.formatter.WriteCodeLine ("//\tCode mapping for '", bundleType, "' resources");
				
				switch (bundleType)
				{
					case Resources.StringTypeName:
						this.GenerateStrings (bundleId, bundle);
						addLine = true;
						break;
					
					case Resources.CaptionTypeName:
						this.GenerateCommandsCaptionsAndTypes (bundleId, bundle);
						addLine = true;
						break;
				}
			}

			this.formatter.WriteCodeLine ();

			this.formatter.WriteBeginMethod (CodeHelper.StaticClassConstructorAttributes, "Res()");
			this.formatter.WriteCodeLine (@"Res._manager = new global::Epsitec.Common.Support.ResourceManager (typeof (Res));");
			this.formatter.WriteCodeLine (@"Res._manager.DefineDefaultModuleName (""", moduleName, @""");");
			
			if (this.classes.Count > 0)
			{
				foreach (var className in this.classes)
				{
					this.formatter.WriteCodeLine (className, ".", "_Initialize ();");
				}
			}
			
			this.formatter.WriteEndMethod ();
			this.formatter.WriteCodeLine ();

			this.formatter.WriteBeginMethod (CodeHelper.PublicStaticMethodAttributes, "void Initialize()");

			//	TODO: emit code to initialize nested classes

			this.formatter.WriteEndMethod ();
			this.formatter.WriteCodeLine ();

			this.formatter.WriteBeginProperty (CodeHelper.PublicStaticPropertyAttributes, "global::Epsitec.Common.Support.ResourceManager Manager");
			this.formatter.WriteBeginGetter (CodeAttributes.Default);
			this.formatter.WriteCodeLine ("return Res._manager;");
			this.formatter.WriteEndGetter ();
			this.formatter.WriteEndProperty ();
			this.formatter.WriteCode ();

			this.formatter.WriteBeginProperty (CodeHelper.PublicStaticPropertyAttributes, "int ModuleId");
			this.formatter.WriteBeginGetter (CodeAttributes.Default);
			this.formatter.WriteCodeLine ("return Res._moduleId;");
			this.formatter.WriteEndGetter ();
			this.formatter.WriteEndProperty ();
			this.formatter.WriteCode ();

			this.formatter.WriteField (CodeHelper.PrivateStaticReadOnlyFieldAttributes,
				"global::Epsitec.Common.Support.ResourceManager _manager;");

			this.formatter.WriteField (CodeHelper.PrivateConstFieldAttributes,
				"int _moduleId = ",
				string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", this.manager.DefaultModuleId),
				";");

			this.formatter.WriteEndClass ();
			this.formatter.WriteEndNamespace ();
			this.formatter.Flush ();

			return this.formatter;
		}

		private void GenerateCommandsCaptionsAndTypes(string bundleId, ResourceBundle bundle)
		{
			List<string> cmdFields = new List<string> ();
			List<string> capFields = new List<string> ();
			List<string> typFields = new List<string> ();
			List<string> valFields = new List<string> ();
			List<string> fldFields = new List<string> ();

			foreach (string field in bundle.FieldNames.Distinct ())
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
				this.formatter.WriteCodeLine ();
				this.GenerateCommands (bundle, cmdFields);

				this.formatter.WriteCodeLine ();
				this.GenerateCommandIds (bundle, cmdFields);
			}

			if (capFields.Count > 0)
			{
				this.formatter.WriteCodeLine ();
				this.GenerateCaptions (bundle, capFields);

				this.formatter.WriteCodeLine ();
				this.GenerateCaptionIds (bundle, capFields);
			}

			if (typFields.Count > 0)
			{
				this.formatter.WriteCodeLine ();
				this.GenerateTypes (bundle, typFields);
			}

			if (valFields.Count > 0)
			{
				this.formatter.WriteCodeLine ();
				this.GenerateValues (bundle, valFields);
			}

			if (fldFields.Count > 0)
			{
				this.formatter.WriteCodeLine ();
				this.GenerateFields (bundle, fldFields);
			}
		}

		private void GenerateInitializer(string prefix, string field = null)
		{
			var fullClass = new System.Text.StringBuilder ();
			
			fullClass.Append (prefix);

			if (string.IsNullOrEmpty (field) == false)
			{
				var args = field.Split ('.');

				for (int j = 0; j < args.Length-1; j++)
				{
					fullClass.Append (".");
					fullClass.Append (args[j]);
				}
			}

			var className = fullClass.ToString ();

			if (!classes.Contains (className))
			{
				classes.Add (className);
			}

			this.formatter.WriteBeginMethod (CodeHelper.InternalStaticMethodAttributes, "void _Initialize()");
			this.formatter.WriteEndMethod ();
			this.formatter.WriteCodeLine ();
		}
		
		private void GenerateCommands(ResourceBundle bundle, List<string> cmdFields)
		{
			this.commandsGenerated = true;
			string prefix   = "";
			bool addNewline = false;

			this.formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, "Commands");
			this.GenerateInitializer ("Commands");

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

			for (int i = 0; i < fields.Length; i++)
			{
				string field = fields[i].Substring (4);

				while (prefix != "" && !field.StartsWith (prefix + "."))
				{
					//	Remonte d'un niveau dans la hiérarchie des classes.
					string[] args = prefix.Split ('.');
					string last = args[args.Length-1];

					this.formatter.WriteEndClass ();

					prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length-last.Length-1));
					addNewline = true;
				}

				string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length+1);

				if (addNewline)
				{
					this.formatter.WriteCodeLine ();
					addNewline = false;
				}

				//	Crée les classes manquantes, si besoin :
				while (delta.IndexOf ('.') > -1)
				{
					string[] args = delta.Split ('.');
					string elem = args[0];

					this.formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, elem);

					this.GenerateInitializer ("Commands", field);

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

				this.formatter.WriteCodeLine ("//\tdesigner:cap/", moduleDruid.ToString ().Trim ('[', ']'));

				this.formatter.WriteField (CodeHelper.PublicStaticReadOnlyFieldAttributes,
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
					this.formatter.WriteEndClass ();
				}
			}

			this.formatter.WriteEndClass ();
		}

		private void GenerateCommandIds(ResourceBundle bundle, List<string> cmdFields)
		{
			this.GenerateGenericCaptions (bundle, cmdFields, "CommandIds", (delta, localDruid) =>
{
	Druid moduleDruid = new Druid (localDruid, bundle.Module.Id);
	this.formatter.WriteField (CodeHelper.PublicConstFieldAttributes, "long ", delta, @" = 0x", moduleDruid.ToLong ().ToString ("X"), "L;");
});
		}

		private void GenerateCaptions(ResourceBundle bundle, List<string> capFields)
		{
			this.GenerateGenericCaptions (bundle, capFields, "Captions", (delta, localDruid) =>
{
	this.formatter.WriteBeginProperty (CodeHelper.PublicStaticPropertyAttributes, string.Concat (@"global::Epsitec.Common.Types.Caption ", delta));
	this.formatter.WriteBeginGetter (CodeAttributes.Default);
	this.formatter.WriteCodeLine ("return global::", this.defaultNamespace, ".Res.", "_manager", ".GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, ", localDruid.DeveloperAndPatchLevel.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ", localDruid.Local.ToString (System.Globalization.CultureInfo.InvariantCulture), "));");
	this.formatter.WriteEndGetter ();
	this.formatter.WriteEndProperty ();
});
		}

		private void GenerateCaptionIds(ResourceBundle bundle, List<string> capFields)
		{
			this.GenerateGenericCaptions (bundle, capFields, "CaptionIds", (delta, localDruid) =>
{
	this.formatter.WriteField (CodeHelper.PublicStaticReadOnlyFieldAttributes, @"global::Epsitec.Common.Support.Druid ", delta, @" = new global::Epsitec.Common.Support.Druid (_moduleId, ", localDruid.DeveloperAndPatchLevel.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ", localDruid.Local.ToString (System.Globalization.CultureInfo.InvariantCulture), ");");
});
		}

		private void GenerateGenericCaptions(ResourceBundle bundle, List<string> capFields, string className, System.Action<string, Druid> fieldWriter)
		{
			string prefix   = "";
			bool addNewline = false;

			this.formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, className);
			this.GenerateInitializer (className);

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

					this.formatter.WriteEndClass ();

					prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length-last.Length-1));
					addNewline = true;
				}

				string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length+1);

				if (addNewline)
				{
					this.formatter.WriteCodeLine ();
					addNewline = false;
				}

				//	Crée les classes manquantes, si besoin :
				while (delta.IndexOf ('.') > -1)
				{
					string[] args = delta.Split ('.');
					string elem = args[0];

					this.formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, elem);

					this.GenerateInitializer (className, field);

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

				this.formatter.WriteCodeLine ("//\tdesigner:cap/", moduleDruid.ToString ().Trim ('[', ']'));

				fieldWriter (delta, localDruid);
			}

			//	Referme les classes ouvertes :
			if (prefix.Length > 0)
			{
				string[] args = prefix.Split ('.');

				for (int j=0; j<args.Length; j++)
				{
					this.formatter.WriteEndClass ();
				}
			}

			this.formatter.WriteCodeLine ();
			this.formatter.WriteEndClass ();
		}

		private void GenerateValues(ResourceBundle bundle, List<string> valFields)
		{
			this.GenerateGenericCaptions (bundle, valFields, "Values", (delta, localDruid) =>
{
	this.formatter.WriteBeginProperty (CodeHelper.PublicStaticPropertyAttributes, string.Concat (@"global::Epsitec.Common.Types.Caption ", delta));
	this.formatter.WriteBeginGetter (CodeAttributes.Default);
	this.formatter.WriteCodeLine ("return global::", this.defaultNamespace, ".Res.", "_manager", ".GetCaption (new global::Epsitec.Common.Support.Druid (_moduleId, ", localDruid.DeveloperAndPatchLevel.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ", localDruid.Local.ToString (System.Globalization.CultureInfo.InvariantCulture), "));");
	this.formatter.WriteEndGetter ();
	this.formatter.WriteEndProperty ();
});
		}


		private void GenerateFields(ResourceBundle bundle, List<string> fldFields)
		{
			string prefix   = "";
			bool addNewline = false;

			this.formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, "Fields");

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

					this.formatter.WriteEndClass ();

					prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length-last.Length-1));
					addNewline = true;
				}

				string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length+1);

				if (addNewline)
				{
					this.formatter.WriteCodeLine ();
					addNewline = false;
				}

				//	Crée les classes manquantes, si besoin :
				while (delta.IndexOf ('.') > -1)
				{
					string[] args = delta.Split ('.');
					string elem = args[0];

					this.formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, elem);

					this.GenerateInitializer ("Fields", field);

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

				this.formatter.WriteCodeLine ("//\tdesigner:cap/", moduleDruid.ToString ().Trim ('[', ']'));

				this.formatter.WriteField (CodeHelper.PublicStaticReadOnlyFieldAttributes,
					@"global::Epsitec.Common.Support.Druid ",
					delta,
					@" = new global::Epsitec.Common.Support.Druid (_moduleId, ",
					localDruid.DeveloperAndPatchLevel.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ",
					localDruid.Local.ToString (System.Globalization.CultureInfo.InvariantCulture), ");");
			}

			//	Referme les classes ouvertes :
			if (prefix.Length > 0)
			{
				string[] args = prefix.Split ('.');

				for (int j = 0; j < args.Length; j++)
				{
					this.formatter.WriteEndClass ();
				}
			}

			this.formatter.WriteEndClass ();
		}

		private void GenerateTypes(ResourceBundle bundle, List<string> typFields)
		{
			string prefix   = "";
			bool addNewline = false;

			this.formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, "Types");
			this.GenerateInitializer ("Types");

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

					this.formatter.WriteEndClass ();

					prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length-last.Length-1));
					addNewline = true;
				}

				string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length+1);

				if (addNewline)
				{
					this.formatter.WriteCodeLine ();
					addNewline = false;
				}

				//	Crée les classes manquantes, si besoin :
				while (delta.IndexOf ('.') > -1)
				{
					string[] args = delta.Split ('.');
					string elem = args[0];

					this.formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, elem);

					this.GenerateInitializer ("Types", field);

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


				this.formatter.WriteCodeLine ("//\tdesigner:cap/", moduleDruid.ToString ().Trim ('[', ']'));

				this.formatter.WriteField (CodeHelper.PublicStaticReadOnlyFieldAttributes,
					typeName, @" ", delta,
					@" = (global::", typeName, @") global::Epsitec.Common.Types.TypeRosetta.CreateTypeObject (",
					@"new global::Epsitec.Common.Support.Druid (_moduleId, ",
					localDruid.DeveloperAndPatchLevel.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ",
					localDruid.Local.ToString (System.Globalization.CultureInfo.InvariantCulture), "));");
			}

			//	Referme les classes ouvertes :
			if (prefix.Length > 0)
			{
				string[] args = prefix.Split ('.');

				for (int j = 0; j < args.Length; j++)
				{
					this.formatter.WriteEndClass ();
				}
			}

			this.formatter.WriteEndClass ();
		}

		private void GenerateStrings(string bundleId, ResourceBundle bundle)
		{
			this.formatter.WriteCodeLine ();

			string prefix   = "";
			bool addNewline = false;

			this.formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, bundleId);
			
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

					this.formatter.WriteEndClass ();

					prefix = prefix.Substring (0, System.Math.Max (0, prefix.Length - last.Length - 1));
					addNewline = true;
				}

				string delta = prefix.Length == 0 ? field : field.Substring (prefix.Length + 1);

				if (addNewline)
				{
					this.formatter.WriteCodeLine ();
					addNewline = false;
				}

				//	Crée les classes manquantes, si besoin :

				while (delta.IndexOf ('.') > -1)
				{
					string[] args = delta.Split ('.');
					string elem = args[0];

					this.formatter.WriteBeginClass (CodeHelper.PublicStaticClassAttributes, elem);

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

				this.formatter.WriteCodeLine ("//\tdesigner:str/", moduleDruid.ToString ().Trim ('[', ']'));

				switch (this.textMode)
				{
					case ResourceTextMode.String:
						this.formatter.WriteBeginProperty (CodeHelper.PublicStaticPropertyAttributes, string.Concat ("global::System.String ", delta));
						this.formatter.WriteBeginGetter (CodeAttributes.Default);
						this.formatter.WriteCodeLine ("return global::", this.defaultNamespace, ".Res.", bundleId, ".GetString (", id.ToString (), ");");
						break;

					case ResourceTextMode.FormattedText:
						this.formatter.WriteBeginProperty (CodeHelper.PublicStaticPropertyAttributes, string.Concat ("global::Epsitec.Common.Types.FormattedText ", delta));
						this.formatter.WriteBeginGetter (CodeAttributes.Default);
						this.formatter.WriteCodeLine ("return global::", this.defaultNamespace, ".Res.", bundleId, ".GetText (", id.ToString (), ");");
						break;

					default:
						throw new System.NotImplementedException ();
				}

				this.formatter.WriteEndGetter ();
				this.formatter.WriteEndProperty ();
			}

			//	Referme les classes ouvertes :

			if (prefix.Length > 0)
			{
				string[] args = prefix.Split ('.');

				for (int j = 0; j < args.Length; j++)
				{
					this.formatter.WriteEndClass ();
				}
			}

			this.formatter.WriteCodeLine ();

			this.formatter.WriteBeginMethod (CodeHelper.PublicStaticMethodAttributes, "global::Epsitec.Common.Types.FormattedText GetText(params string[] path)");
			this.formatter.WriteCodeLine (@"string field = string.Join (""."", path);");
			this.formatter.WriteCodeLine (@"return new global::Epsitec.Common.Types.FormattedText (_stringsBundle[field].AsString);");
			this.formatter.WriteEndMethod ();

			this.formatter.WriteCodeLine ();

			this.formatter.WriteBeginMethod (CodeHelper.PublicStaticMethodAttributes, "global::System.String GetString(params string[] path)");
			this.formatter.WriteCodeLine (@"string field = string.Join (""."", path);");
			this.formatter.WriteCodeLine (@"return _stringsBundle[field].AsString;");
			this.formatter.WriteEndMethod ();

			this.formatter.WriteCodeLine ();

			this.formatter.WriteCodeLine ("#region Internal Support Code");
			this.formatter.WriteCodeLine ();
			this.formatter.WriteBeginMethod (CodeHelper.PrivateStaticMethodAttributes, "global::Epsitec.Common.Types.FormattedText GetText(string bundle, params string[] path)");
			this.formatter.WriteCodeLine ("return new global::Epsitec.Common.Types.FormattedText (global::", this.defaultNamespace, ".Res.", bundleId, ".GetString (", "bundle, path", "));");
			this.formatter.WriteEndMethod ();
			this.formatter.WriteCodeLine ();
			this.formatter.WriteBeginMethod (CodeHelper.PrivateStaticMethodAttributes, "global::Epsitec.Common.Types.FormattedText GetText(global::Epsitec.Common.Support.Druid druid)");
			this.formatter.WriteCodeLine ("return new global::Epsitec.Common.Types.FormattedText (global::", this.defaultNamespace, ".Res.", bundleId, ".GetString (", "druid", "));");
			this.formatter.WriteEndMethod ();
			this.formatter.WriteCodeLine ();
			this.formatter.WriteBeginMethod (CodeHelper.PrivateStaticMethodAttributes, "global::System.String GetString(string bundle, params string[] path)");
			this.formatter.WriteCodeLine (@"string field = string.Join (""."", path);");
			this.formatter.WriteCodeLine (@"return _stringsBundle[field].AsString;");
			this.formatter.WriteEndMethod ();
			this.formatter.WriteCodeLine ();
			this.formatter.WriteBeginMethod (CodeHelper.PrivateStaticMethodAttributes, "global::System.String GetString(global::Epsitec.Common.Support.Druid druid)");
			this.formatter.WriteCodeLine (@"return _stringsBundle[druid].AsString;");
			this.formatter.WriteEndMethod ();
			this.formatter.WriteCodeLine ();

			this.formatter.WriteField (CodeHelper.PrivateStaticReadOnlyFieldAttributes,
				@"global::Epsitec.Common.Support.ResourceBundle _stringsBundle = Res._manager.GetBundle (""",
				bundleId, @""");");
			this.formatter.WriteCodeLine ();
			this.formatter.WriteCodeLine ("#endregion");

			this.formatter.WriteEndClass ();
		}

		private bool commandsGenerated;

		private readonly ResourceManager manager;
		private readonly string defaultNamespace;
		private readonly string moduleName;
		private readonly ResourceTextMode textMode;
		private readonly CodeFormatter formatter;
		private readonly List<string> classes;
	}
}
