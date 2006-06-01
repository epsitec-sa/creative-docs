//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe Resources permet de gérer les ressources de l'application.
	/// </summary>
	public static class Resources
	{
		static Resources()
		{
			string[] names = { "fr", "de", "it", "en", "es" };

			Resources.factory = new ResourceProviderFactory ();
			
			Resources.InternalInitialise ();
			Resources.InternalDefineCultures (names);
			
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
			id = Resources.ResolveDruidReference (id);

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

		public static string ResolveDruidReference(string fullId)
		{
			string prefix;
			string localId;

			Resources.SplitFullId (fullId, out prefix, out localId);
			Resources.ResolveDruidReference (ref prefix, ref localId);

			return Resources.JoinFullId (prefix, localId);
		}

		public static void ResolveDruidReference(ref string prefix, ref string localId)
		{
			if (Druid.IsValidResourceId (localId))
			{
				//	The local ID is not a standard resource bundle identifier; it
				//	looks like a DRUID : "[mmDLLDLLDmLDm]"

				Druid druid = Druid.Parse (localId);

				prefix  = string.Format (CultureInfo.InvariantCulture, "{0}/{1}", prefix, druid.Module);
				localId = Resources.JoinFieldId (Resources.DruidBundleName, druid.ToFieldName ());
			}
		}
		
		
		public static CultureInfo FindCultureInfo(string twoLetterCode)
		{
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
			//	FindSpecificCultureInfo retourne une culture propre à un pays, avec
			//	une préférence pour la Suisse ou les USA.
			
			CultureInfo[] cultures = CultureInfo.GetCultures (System.Globalization.CultureTypes.SpecificCultures);
			twoLetterCode = twoLetterCode.ToLowerInvariant ();
			
			CultureInfo found = null;
			
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
					
					if (found == null)
					{
						found = item;
					}
				}
			}
			
			return found;
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

		internal static string CreateBundleKey(string prefix, int moduleId, string resource_id, ResourceLevel level, CultureInfo culture)
		{
			System.Diagnostics.Debug.Assert (prefix != null);
			System.Diagnostics.Debug.Assert (prefix.Length > 0);
			System.Diagnostics.Debug.Assert (prefix.IndexOf (Resources.PrefixSeparator) < 0);
			System.Diagnostics.Debug.Assert (resource_id.Length > 0);
			System.Diagnostics.Debug.Assert (resource_id.IndexOf (Resources.FieldSeparator) < 0);

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
			buffer.Append (resource_id);
			buffer.Append ("~");
			buffer.Append ((int) level);
			buffer.Append ("~");
			buffer.Append (culture.TwoLetterISOLanguageName);

			return buffer.ToString ();
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

		private static void InternalInitialise()
		{
			Resources.manager = new ResourceManager (typeof (ResourceManager));
		}
		
		private static void InternalDefineCultures(string[] names)
		{
			int n = names.Length;
			
			Resources.cultures = new CultureInfo[n];
			
			for (int i = 0; i < n; i++)
			{
				Resources.cultures[i] = Resources.FindCultureInfo (names[i]);
			}
		}

		#endregion

		public static readonly string			DruidBundleName = "DruidData";

		public static readonly char				PrefixSeparator = ':';
		public static readonly char				ModuleSeparator = '/';

		public static readonly char				FieldIdPrefix = '$';
		public static readonly char				FieldSeparator = '#';
		public static readonly int				MaxRecursion = 50;
		
		private static ResourceProviderFactory	factory;

		private static CultureInfo[]			cultures;
		private static ResourceManager			manager;
	}
}
