//	Copyright © 2003-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>Resources</c> class provides access to the resources through the
	/// <see cref="ResourceManager"/>.
	/// </summary>
	public static class Resources
	{
		static Resources()
		{
			string[] names = { "fr-CH", "de-CH", "it-CH", "en-GB", "es-ES", "pt-PT" };

			Resources.factory = new ResourceProviderFactory ();
			Resources.manager = new ResourceManager (typeof (ResourceManager));
			Resources.cultures = names.Select (code => CultureInfo.GetCultureInfoByIetfLanguageTag (code)).ToList ();
			
			Types.ResourceBinding.RebindCallback = Resources.Rebinder;
		}

		/// <summary>
		/// Gets the default resource manager.
		/// </summary>
		/// <value>The default resource manager.</value>
		public static ResourceManager			DefaultManager
		{
			get
			{
				return Resources.manager;
			}
		}

		/// <summary>
		/// Gets the well known cultures.
		/// </summary>
		/// <value>The well known cultures.</value>
		public static IEnumerable<CultureInfo>	WellKnownCultures
		{
			get
			{
				return Resources.cultures;
			}
		}

		/// <summary>
		/// Gets the resource provider factory.
		/// </summary>
		/// <value>The resource provider factory.</value>
		internal static ResourceProviderFactory Factory
		{
			get
			{
				return Resources.factory;
			}
		}

		
		public static string ExtractPrefix(string id)
		{
			if (id != null)
			{
				int pos = id.IndexOf (Resources.PrefixSeparator);
			
				if (pos > 0)
				{
					return id.Substring (0, pos);
				}
			}
			
			return null;
		}
		
		public static string ExtractSuffix(string id)
		{
			//	L'extraction d'un suffixe n'a de sens que si l'on utilise GetBundleIds
			//	avec level = ResourceLevel.All; sinon, en principe, on n'a jamais besoin
			//	de se soucier des suffixes.
			
			if (id != null)
			{
				int pos = id.LastIndexOf ('.') + 1;
				int len = id.Length;
				
				if (pos > 0)
				{
					if ((pos + 2 == len) ||
						(pos + 3 == len))
					{
						return id.Substring (pos);
					}
				}
			}
			
			return null;
		}
		
		public static string StripSuffix(string id)
		{
			string suffix = Resources.ExtractSuffix (id);
			
			if (string.IsNullOrEmpty (suffix))
			{
				return id;
			}
			else
			{
				return id.Substring (0, id.Length - suffix.Length - 1);
			}
		}
		
		public static string ExtractName(string id)
		{
			if (id != null)
			{
				int pos = id.IndexOf (Resources.PrefixSeparator);
			
				if (pos > 0)
				{
					return id.Substring (pos+1);
				}
				else
				{
					return id;
				}
			}
			
			return null;
		}

		public static string JoinFullPrefix(string prefix, string module)
		{
			if (string.IsNullOrEmpty (module))
			{
				return prefix;
			}
			else
			{
				return string.Concat (prefix, Resources.ModuleSeparator, module);
			}
		}

		public static void SplitFullPrefix(string fullPrefix, out string prefix, out string module)
		{
			if (fullPrefix == null)
			{
				prefix = null;
				module = null;
			}
			else
			{
				int pos = fullPrefix.IndexOf (Resources.ModuleSeparator);

				if (pos >= 0)
				{
					prefix = fullPrefix.Substring (0, pos);
					module = fullPrefix.Substring (pos+1);
				}
				else
				{
					prefix = fullPrefix;
					module = null;
				}
			}
		}

		public static string JoinFullId(string prefix, string localId)
		{
			if (string.IsNullOrEmpty (prefix))
			{
				return localId;
			}
			else
			{
				return string.Concat (prefix, Resources.PrefixSeparator, localId);
			}
		}

		public static void SplitFullId(string fullId, out string prefix, out string localId)
		{
			int pos = fullId.IndexOf (Resources.PrefixSeparator);

			if (pos >= 0)
			{
				prefix  = fullId.Substring (0, pos);
				localId = fullId.Substring (pos+1);
			}
			else
			{
				prefix  = "";
				localId = fullId;
			}
		}

		public static bool SplitFieldId(string id, out string bundle, out string field)
		{
			id = Resources.ResolveStringsIdReference (id);

			return Resources.SplitFieldIdWithoutIdResolution (id, out bundle, out field);
		}

		public static bool SplitFieldIdWithoutIdResolution(string id, out string bundle, out string field)
		{
			int pos = id.IndexOf (Resources.FieldSeparator);

			bundle = id;
			field  = null;

			if (pos >= 0)
			{
				bundle = id.Substring (0, pos);
				field  = id.Substring (pos+1);

				return true;
			}

			return false;
		}

		public static string JoinFieldId(string bundle, string field)
		{
			if ((bundle == null) ||
				(bundle.IndexOf (Resources.FieldSeparator) != -1) ||
				(field == null) ||
				(field.IndexOf (Resources.FieldSeparator) != -1))
			{
				throw new ResourceException ("Invalid target specified.");
			}

			return string.Concat (bundle, Resources.FieldSeparator, field);
		}

		public static string ResolveStringsIdReference(string fullId)
		{
			string prefix;
			string localId;

			Resources.SplitFullId (fullId, out prefix, out localId);
			Resources.ResolveStringsIdReference (ref prefix, ref localId);

			return Resources.JoinFullId (prefix, localId);
		}

		public static string ResolveCaptionsIdReference(string prefix, Druid druid)
		{
			return string.Concat (
				prefix,
				Resources.ModuleSeparatorText,
				druid.Module.ToString (CultureInfo.InvariantCulture),
				Resources.PrefixSeparatorText,
				Resources.CaptionsBundleName);
		}

		public static void ResolveStringsIdReference(ref string prefix, ref string localId)
		{
			if (Druid.IsValidResourceId (localId))
			{
				//	The local ID is not a standard resource bundle identifier; it
				//	looks like a DRUID : "[mmDLLDLLDmLDm]"

				Druid druid = Druid.Parse (localId);

				if (prefix.Contains ("/"))
				{
					//	OK, prefix already contains module id.
				}
				else
				{
					prefix  = string.Format (CultureInfo.InvariantCulture, "{0}/{1}", prefix, druid.Module);
				}
				
				localId = Resources.JoinFieldId (Resources.StringsBundleName, druid.ToFieldName ());
			}
		}

		public static void ResolveBundleIdReference(ref string prefix, ref string localId)
		{
			if (Druid.IsValidBundleId (localId))
			{
				//	The local ID is not a standard resource bundle identifier; it
				//	looks like a DRUID : "_mmDLLDLLDmLDm"

				Druid druid = Druid.Parse (localId);

				if (prefix.Contains ("/"))
				{
					//	OK, prefix already contains module id.
				}
				else
				{
					prefix  = string.Format (CultureInfo.InvariantCulture, "{0}/{1}", prefix, druid.Module);
				}
				
				localId = druid.ToBundleId ();
			}
		}

		public static bool IsFieldId(string id)
		{
			return id.IndexOf (Resources.FieldSeparator) >= 0;
		}
		
		public static CultureInfo FindCultureInfo(string twoLetterCode)
		{
			if (string.IsNullOrEmpty (twoLetterCode))
			{
				return null;
			}
			if (twoLetterCode == Resources.DefaultTwoLetterISOLanguageName)
			{
				return null;
			}

			twoLetterCode = twoLetterCode.ToLowerInvariant ();
			CultureInfo[] cultures = CultureInfo.GetCultures (System.Globalization.CultureTypes.NeutralCultures);
			
			for (int i = 0; i < cultures.Length; i++)
			{
				if (cultures[i].TwoLetterISOLanguageName == twoLetterCode)
				{
					return cultures[i];
				}
			}
			
			return null;
		}
		
		public static CultureInfo FindSpecificCultureInfo(string twoLetterCode)
		{
			if (string.IsNullOrEmpty (twoLetterCode))
			{
				return null;
			}

			CultureInfo info;

			lock (Resources.cultures)
			{
				info = Resources.cultures.Find (x => x.TwoLetterISOLanguageName == twoLetterCode);

				if (info == null)
				{
					info = Resources.InternalFindSpecificCultureInfo (twoLetterCode);

					if (info != null)
					{
						Resources.cultures.Add (info);
					}
				}
			}
				
			return info;
		}

		private static CultureInfo InternalFindSpecificCultureInfo(string twoLetterCode)
		{
			//	FindSpecificCultureInfo retourne une culture propre à un pays, avec
			//	une préférence pour la Suisse ou les USA.
			
			if (string.IsNullOrEmpty (twoLetterCode))
			{
				return null;
			}

			CultureInfo[] cultures = CultureInfo.GetCultures (System.Globalization.CultureTypes.SpecificCultures);
			twoLetterCode = twoLetterCode.ToLowerInvariant ();
			
			List<CultureInfo> found = new List<CultureInfo> ();
			
			for (int i = 0; i < cultures.Length; i++)
			{
				CultureInfo item = cultures[i];
				string      name = item.Name;
				
				if ((item.TwoLetterISOLanguageName == twoLetterCode) &&
					(name.Length == 5) &&
					(name[2] == '-'))
				{
					if (((name[3] == 'C') && (name[4] == 'H')) ||
						((name[3] == 'U') && (name[4] == 'S')))
					{
						return item;
					}
					
					found.Add (item);
				}
			}

			if (found.Count > 0)
			{
				foreach (CultureInfo item in found)
				{
					string name = item.Name.ToLower ();

					if ((name[3] == name[0]) &&
						(name[4] == name[1]))
					{
						return item;
					}
				}

				return found[0];
			}
			
			return null;
		}

		public static void OverrideDefaultTwoLetterISOLanguageName(string value)
		{
			if ((value == null) ||
				(value.Length == 2))
			{
				Resources.twoLetterISOLanguageNameOverride = value;
			}
			else
			{
				throw new System.ArgumentException ();
			}
		}

		public static string GetDefaultTwoLetterISOLanguageName()
		{
			if (string.IsNullOrEmpty (Resources.twoLetterISOLanguageNameOverride))
			{
				return null;
			}
			else
			{
				return Resources.twoLetterISOLanguageNameOverride;
			}
		}
		
		public static string GetTwoLetterISOLanguageName(CultureInfo culture)
		{
			string iso = culture.TwoLetterISOLanguageName;

			if (iso == "fr")
			{
				return Resources.twoLetterISOLanguageNameOverride ?? iso;
			}
			else
			{
				return iso;
			}
		}
		
		public static bool EqualCultures(ResourceLevel levelA, CultureInfo cultureA, ResourceLevel levelB, CultureInfo cultureB)
		{
			if (levelA != levelB)
			{
				return false;
			}
			if (levelA == ResourceLevel.Default)
			{
				return true;
			}
			
			return Resources.EqualCultures (cultureA, cultureB);
		}
		
		public static bool EqualCultures(CultureInfo cultureA, CultureInfo cultureB)
		{
			if (cultureA == cultureB)
			{
				return true;
			}
			
			if ((cultureA == null) ||
				(cultureB == null))
			{
				return false;
			}
			
			return cultureA.TwoLetterISOLanguageName == cultureB.TwoLetterISOLanguageName;
		}

		#region Internal and Private Methods

		internal static string CreateBundleKey(string prefix, ResourceModuleId module, string resourceId, ResourceLevel level, CultureInfo culture, string suffix)
		{
			System.Diagnostics.Debug.Assert (prefix != null);
			System.Diagnostics.Debug.Assert (prefix.Length > 0);
			System.Diagnostics.Debug.Assert (prefix.IndexOf (Resources.PrefixSeparator) < 0);
			System.Diagnostics.Debug.Assert (prefix.IndexOf (Resources.ModuleSeparator) < 0);
			System.Diagnostics.Debug.Assert (resourceId.Length > 0);
			System.Diagnostics.Debug.Assert (resourceId.IndexOf (Resources.FieldSeparator) < 0);

			int moduleId = module.Id;
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			if (!string.IsNullOrEmpty (prefix))
			{
				buffer.Append (prefix);
			}
			if (moduleId >= 0)
			{
				buffer.Append (Resources.ModuleSeparator);
				buffer.AppendFormat (CultureInfo.InvariantCulture, "{0}", moduleId);
			}

			buffer.Append (Resources.PrefixSeparator);
			buffer.Append (resourceId);
			buffer.Append ("~");
			buffer.Append (((int) level).ToString (System.Globalization.CultureInfo.InvariantCulture));
			buffer.Append ("~");
			buffer.Append (level == ResourceLevel.Default ? Resources.DefaultTwoLetterISOLanguageName : culture.TwoLetterISOLanguageName);

			if (!string.IsNullOrEmpty (suffix))
			{
				buffer.Append ("~");
				buffer.Append (suffix);
			}

			return buffer.ToString ();
		}
		
		internal static string CreateCaptionKey(Druid druid, ResourceLevel level, CultureInfo culture)
		{
			return string.Concat (
				druid.ToString (),
				"~",
				level.ToString (),
				"~",
				level == ResourceLevel.Default ? Resources.DefaultTwoLetterISOLanguageName : culture.TwoLetterISOLanguageName);
		}

		private static void Rebinder(object resourceManager, Types.ResourceBinding binding)
		{
			ResourceManager that = resourceManager as ResourceManager;

			if (that == null)
			{
				throw new System.ArgumentNullException ("resourceManager", "No resource manager specified");
			}
			if (binding == null)
			{
				throw new System.ArgumentNullException ("binding", "No binding specified");
			}

			if (that.SetResourceBinding (binding) == false)
			{
				throw new ResourceException (string.Format ("Cannot bind to ressource '{0}'", binding.ResourceId));
			}
		}

		#endregion

		public const string						StringsBundleName  = "Strings";
		public const string						CaptionsBundleName = "Captions";
		
		public const string						StringTypeName  = "String";
		public const string						CaptionTypeName = "Caption";
		public const string						PanelTypeName   = "Panel";
		public const string						FormTypeName    = "Form";
		
		public static readonly string			DefaultTwoLetterISOLanguageName = "00";

		public static readonly char				PrefixSeparator = ':';
		public static readonly char				ModuleSeparator = '/';

		public static readonly char				FieldIdPrefix  = '$';
		public static readonly char				FieldSeparator = '#';
		
		public static readonly string			PrefixSeparatorText = ":";
		public static readonly string			ModuleSeparatorText = "/";
		public static readonly string			FieldSeparatorText  = "#";
		
		public static readonly int				MaxRecursionCount = 50;

		private readonly static ResourceProviderFactory factory;

		private readonly static List<CultureInfo> cultures;
		private readonly static ResourceManager manager;
		private static string					twoLetterISOLanguageNameOverride;
	}
}
