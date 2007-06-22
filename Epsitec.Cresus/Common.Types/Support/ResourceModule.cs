//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Globalization;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceModule</c> class ...
	/// </summary>
	public static class ResourceModule
	{
		public static ResourceModuleInfo Load(string modulePath)
		{
			if (System.IO.Directory.Exists (modulePath))
			{
				System.IO.TextReader textReader = null;
				string           moduleInfoPath = System.IO.Path.Combine (modulePath, ResourceModule.ModuleInfoFileName);

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

							string idAttribute    = root.GetAttribute (ResourceModule.XmlAttributeId);
							string layerAttribute = root.GetAttribute (ResourceModule.XmlAttributeLayer);
							string nameAttribute  = root.GetAttribute (ResourceModule.XmlAttributeName);

							if (string.IsNullOrEmpty (idAttribute))
							{
								throw new System.FormatException (string.Format ("{0} specifies no {1} attribute", ResourceModule.XmlModuleInfo, ResourceModule.XmlAttributeId));
							}
							if (string.IsNullOrEmpty (layerAttribute))
							{
								throw new System.FormatException (string.Format ("{0} specifies no {1} attribute", ResourceModule.XmlModuleInfo, ResourceModule.XmlAttributeLayer));
							}
							if (string.IsNullOrEmpty (nameAttribute))
							{
								throw new System.FormatException (string.Format ("{0} specifies no {1} attribute", ResourceModule.XmlModuleInfo, ResourceModule.XmlAttributeName));
							}

							int.TryParse (idAttribute, NumberStyles.Integer, CultureInfo.InvariantCulture, out moduleId);
							moduleLayer = ResourceModuleInfo.ConvertPrefixToLayer (layerAttribute);
							moduleName  = nameAttribute;

							return new ResourceModuleInfo (moduleName, modulePath, moduleId, moduleLayer);
						}
					}
				}
			}

			return ResourceModuleInfo.Empty;
		}

		public const string ModuleInfoFileName = "module.info";
		
		public const string XmlModuleInfo = "ModuleInfo";
		
		public const string XmlAttributeId    = "id";
		public const string XmlAttributeLayer = "layer";
		public const string XmlAttributeName  = "name";
	}
}
