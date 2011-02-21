//	Copyright © 2007-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Globalization;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceModule</c> class is used to manipulate module related
	/// information (see <see cref="ResourceModuleId"/>).
	/// </summary>
	public static class ResourceModule
	{
		/// <summary>
		/// Loads the module information for a specified module path. The information
		/// is loaded from a manifest file (<see cref="ResourceModule.ManifestFileName"/>).
		/// </summary>
		/// <param name="modulePath">The module path.</param>
		/// <returns>The module information or <c>null</c>.</returns>
		public static ResourceModuleInfo LoadManifest(string modulePath)
		{
			if (System.IO.Directory.Exists (modulePath))
			{
				System.IO.TextReader textReader = null;
				string           moduleInfoPath = System.IO.Path.Combine (modulePath, ResourceModule.ManifestFileName);

				if (System.IO.File.Exists (moduleInfoPath))
				{
					try
					{
						textReader = System.IO.File.OpenText (moduleInfoPath);
					}
					catch (System.IO.FileNotFoundException)
					{
					}
					catch (System.IO.PathTooLongException)
					{
					}
				}

				if (textReader != null)
				{
					using (textReader)
					{
						System.Xml.XmlDocument xml = new System.Xml.XmlDocument ();
						xml.Load (textReader);

						System.Xml.XmlElement root = xml.DocumentElement;

						if (root.Name == ResourceModule.XmlModuleInfo)
						{
							int                 moduleId    = -1;
							ResourceModuleLayer moduleLayer = ResourceModuleLayer.Undefined;
							string              moduleName  = null;
							ResourceTextMode	textMode	= ResourceTextMode.String;

							//	The module.info file contains a root node <ModuleInfo>
							//	which defines following attributes :
							//	- id, the numeric identifier for the module
							//	- name, the textual identifier for the module
							//	- layer, the code for the resource module layer
							//	Optionally :
							//  - textMode, the mode used for the textual resources (string / formatted text).
							//	- namespace, the namespace used when generating associated code

							string idAttribute         = root.GetAttribute (ResourceModule.XmlAttributeId);
							string layerAttribute      = root.GetAttribute (ResourceModule.XmlAttributeLayer);
							string nameAttribute       = root.GetAttribute (ResourceModule.XmlAttributeName);
							string namespaceAttribute  = root.GetAttribute (ResourceModule.XmlAttributeNamespace);
							string textModeAttribute   = root.GetAttribute (ResourceModule.XmlAttributeTextMode);
							string assembliesAttribute = root.GetAttribute (ResourceModule.XmlAttributeAssemblies);

							if (string.IsNullOrEmpty (idAttribute))
							{
								throw new System.FormatException (string.Format ("{0} specifies no {1} attribute in module {2}", ResourceModule.XmlModuleInfo, ResourceModule.XmlAttributeId, modulePath));
							}
							if (string.IsNullOrEmpty (layerAttribute))
							{
								throw new System.FormatException (string.Format ("{0} specifies no {1} attribute in module {2}", ResourceModule.XmlModuleInfo, ResourceModule.XmlAttributeLayer, modulePath));
							}
							if (string.IsNullOrEmpty (nameAttribute))
							{
								throw new System.FormatException (string.Format ("{0} specifies no {1} attribute in module {2}", ResourceModule.XmlModuleInfo, ResourceModule.XmlAttributeName, modulePath));
							}

							int.TryParse (idAttribute, NumberStyles.Integer, CultureInfo.InvariantCulture, out moduleId);
							moduleLayer = ResourceModuleId.ConvertPrefixToLayer (layerAttribute);
							moduleName  = nameAttribute;
							textMode = InvariantConverter.ToEnum<ResourceTextMode> (textModeAttribute, ResourceTextMode.String);
							
							ResourceModuleInfo info = new ResourceModuleInfo ();

							System.Xml.XmlNodeList nodes = root.GetElementsByTagName (ResourceModule.XmlReferenceModulePath);
							System.Xml.XmlElement  node  = nodes.Count == 1 ? (nodes[0] as System.Xml.XmlElement) : null;

							if (node != null)
							{
								if (!string.IsNullOrEmpty (node.InnerText))
								{
									info.ReferenceModulePath = node.InnerText;
								}
							}
							else if (nodes.Count > 1)
							{
								throw new System.FormatException (string.Format ("{0} specifies more than 1 {1} element in module {2}", ResourceModule.XmlModuleInfo, ResourceModule.XmlReferenceModulePath, modulePath));
							}

							nodes = root.GetElementsByTagName (ResourceModule.XmlPatchDepth);
							node  = nodes.Count == 1 ? (nodes[0] as System.Xml.XmlElement) : null;

							if (node != null)
							{
								if (!string.IsNullOrEmpty (node.InnerText))
								{
									info.PatchDepth = InvariantConverter.ParseInt (node.InnerText);
								}
							}
							else if (nodes.Count > 1)
							{
								throw new System.FormatException (string.Format ("{0} specifies more than 1 {1} element in module {2}", ResourceModule.XmlModuleInfo, ResourceModule.XmlPatchDepth, modulePath));
							}

							nodes = root.GetElementsByTagName (ResourceModule.XmlVersions);
							node  = nodes.Count == 1 ? (nodes[0] as System.Xml.XmlElement) : null;

							if (node != null)
							{
								foreach (System.Xml.XmlElement versionNode in node.ChildNodes)
								{
									int developerId = InvariantConverter.ToInt (versionNode.GetAttribute (ResourceModule.XmlAttributeId));
									int buildNumber = InvariantConverter.ToInt (versionNode.GetAttribute (ResourceModule.XmlAttributeBuild));
									System.DateTime buildDate = InvariantConverter.ToDateTime (versionNode.GetAttribute (ResourceModule.XmlAttributeDate));

									info.Versions.Add (new ResourceModuleVersion (developerId, buildNumber, buildDate));
								}
							}
							else if (nodes.Count > 1)
							{
								throw new System.FormatException (string.Format ("{0} specifies more than 1 {1} element in module {2}", ResourceModule.XmlModuleInfo, ResourceModule.XmlVersions, modulePath));
							}

							
							info.FullId = new ResourceModuleId (moduleName, modulePath, moduleId, moduleLayer);
							info.SourceNamespace = namespaceAttribute ?? "";
							info.TextMode = textMode;
							info.Assemblies = string.IsNullOrEmpty (assembliesAttribute) ? null : assembliesAttribute;
							info.Freeze ();

							return info;
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Saves the module definition identity into the associated manifest
		/// file (<see cref="ResourceModule.ManifestFileName"/>).
		/// </summary>
		/// <param name="info">The module identity.</param>
		public static void SaveManifest(ResourceModuleInfo info)
		{
			ResourceModule.SaveManifest (info, null);
		}

		/// <summary>
		/// Saves the module definition identity into the associated manifest
		/// file (<see cref="ResourceModule.ManifestFileName"/>).
		/// </summary>
		/// <param name="info">The module identity.</param>
		/// <param name="comment">The module comment (or <c>null</c>).</param>
		public static void SaveManifest(ResourceModuleInfo info, string comment)
		{
			string modulePath     = info.FullId.Path;
			string moduleInfoPath = System.IO.Path.Combine (modulePath, ResourceModule.ManifestFileName);

			System.IO.Directory.CreateDirectory (modulePath);
			
			System.Xml.XmlDocument xml = ResourceModule.CreateXmlManifest (info, comment);
			
			xml.Save (moduleInfoPath);
		}

		public static System.Xml.XmlDocument CreateXmlManifest(ResourceModuleInfo info, string comment)
		{
			System.Xml.XmlDocument xml = new System.Xml.XmlDocument ();
			System.Xml.XmlElement root = xml.CreateElement (ResourceModule.XmlModuleInfo);

			root.SetAttribute (ResourceModule.XmlAttributeId, InvariantConverter.ToString (info.FullId.Id));
			root.SetAttribute (ResourceModule.XmlAttributeName, info.FullId.Name);
			root.SetAttribute (ResourceModule.XmlAttributeLayer, ResourceModuleId.ConvertLayerToPrefix (info.FullId.Layer));
			root.SetAttribute (ResourceModule.XmlAttributeTextMode, System.Enum.GetName (typeof (ResourceTextMode), info.TextMode));
			root.SetAttribute (ResourceModule.XmlAttributeAssemblies, info.Assemblies ?? "");

			if (!string.IsNullOrEmpty (info.SourceNamespace))
			{
				root.SetAttribute (ResourceModule.XmlAttributeNamespace, info.SourceNamespace);
			}

			if (info.HasVersions)
			{
				System.Xml.XmlElement versionsNode = xml.CreateElement (ResourceModule.XmlVersions);
				
				foreach (ResourceModuleVersion version in info.Versions)
				{
					System.Xml.XmlElement versionNode = xml.CreateElement (ResourceModule.XmlVersion);

					versionNode.SetAttribute (ResourceModule.XmlAttributeId, InvariantConverter.ToString (version.DeveloperId));
					versionNode.SetAttribute (ResourceModule.XmlAttributeBuild, InvariantConverter.ToString (version.BuildNumber));
					versionNode.SetAttribute (ResourceModule.XmlAttributeDate, InvariantConverter.ToString (version.BuildDate));

					versionsNode.AppendChild (versionNode);
				}

				root.AppendChild (versionsNode);
			}
			
			xml.InsertBefore (xml.CreateXmlDeclaration ("1.0", "utf-8", null), null);
			
			if (!string.IsNullOrEmpty (comment))
			{
				xml.AppendChild (xml.CreateComment (comment));
			}
			
			xml.AppendChild (root);

			if (info.IsPatchModule)
			{
				System.Xml.XmlElement node = xml.CreateElement (ResourceModule.XmlReferenceModulePath);

				node.InnerText = info.ReferenceModulePath;
				root.AppendChild (node);
			}

			if (info.PatchDepth > 0)
			{
				System.Xml.XmlElement node = xml.CreateElement (ResourceModule.XmlPatchDepth);

				node.InnerText = info.PatchDepth.ToString (CultureInfo.InvariantCulture);
				root.AppendChild (node);
			}

			return xml;
		}

		/// <summary>
		/// Gets the version fingerprint for the specified module.
		/// </summary>
		/// <param name="info">The module information.</param>
		/// <returns>The fingerprint or <c>""</c>.</returns>
		public static string GetVersionFingerprint(ResourceModuleInfo info)
		{
			if ((info == null) ||
				(info.HasVersions == false))
			{
				return "";
			}

			List<ResourceModuleVersion> versions = new List<ResourceModuleVersion> (info.Versions);
			versions.Sort (ResourceModuleVersion.Comparer);

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (ResourceModuleVersion version in versions)
			{
				if (buffer.Length > 0)
				{
					buffer.Append ("-");
				}

				buffer.Append (InvariantConverter.ToString (version.DeveloperId));
				buffer.Append ("/");
				buffer.Append (InvariantConverter.ToString (version.BuildNumber));
			}

			return buffer.ToString ();
		}

		/// <summary>
		/// Finds all possible module paths given a root path. This will walk
		/// through the full tree, attempting to locate directories with a
		/// manifest file.
		/// </summary>
		/// <param name="rootPath">The root path.</param>
		/// <returns>The enumeration of plausible module paths.</returns>
		public static IEnumerable<string> FindModulePaths(string rootPath)
		{
			if (System.IO.Directory.Exists (rootPath))
			{
				string[] paths = null;
				
				try
				{
					paths = System.IO.Directory.GetDirectories (rootPath);
				}
				catch
				{
					paths = new	string[0];
				}

				foreach (string path in paths)
				{
					string name  = System.IO.Path.GetFileName (path);
					string probe = System.IO.Path.Combine (path, ResourceModule.ManifestFileName);
					bool found = false;

					if (name.StartsWith ("."))
					{
						continue;
					}

					try
					{
						found = System.IO.File.Exists (probe);
					}
					catch (System.IO.IOException)
					{
						//	Ignore file related exceptions; we don't want to stop if
						//	we try to analyze some protected directory: just skip it.
					}

					if (found)
					{
						yield return path;
					}

					foreach (string child in ResourceModule.FindModulePaths (path))
					{
						yield return child;
					}
				}
			}
		}
		
		public const string ManifestFileName = "module.info";

		#region Internal constants

		internal const string XmlModuleInfo          = "ModuleInfo";
		internal const string XmlVersion			 = "Version";
		internal const string XmlVersions			 = "Versions";
		internal const string XmlReferenceModulePath = "ReferenceModulePath";
		internal const string XmlPatchDepth			 = "PatchDepth";

		internal const string XmlAttributeBuild		= "build";
		internal const string XmlAttributeDate		= "date";
		internal const string XmlAttributeId		= "id";
		internal const string XmlAttributeLayer		= "layer";
		internal const string XmlAttributeName		= "name";
		internal const string XmlAttributeNamespace = "namespace";
		internal const string XmlAttributeTextMode	= "textMode";
		internal const string XmlAttributeAssemblies= "assemblies";

		#endregion
	}
}
