//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.Support
{
	using System.Globalization;
	using System.Collections;
	
	/// <summary>
	/// La classe Resources permet de gérer les ressources de l'application.
	/// </summary>
	public class Resources
	{
		private Resources()
		{
		}
		
		static Resources()
		{
			Resources.providers     = new IResourceProvider[0];
			Resources.provider_hash = new System.Collections.Hashtable ();
			Resources.culture       = CultureInfo.CurrentCulture;
			
			Resources.Initialise ();
		}
		
		
		protected static void Initialise()
		{
			System.Collections.ArrayList providers = new System.Collections.ArrayList ();
			System.Reflection.Assembly   assembly  = System.Reflection.Assembly.LoadWithPartialName ("Support.Implementation");
			
			System.Type[] types_in_assembly = assembly.GetTypes ();
			
			foreach (System.Type type in types_in_assembly)
			{
				if (type.IsClass)
				{
					if (type.GetInterface ("IResourceProvider") != null)
					{
						IResourceProvider provider = System.Activator.CreateInstance (type) as IResourceProvider;
						
						if (provider != null)
						{
							System.Diagnostics.Debug.Assert (Resources.provider_hash.Contains (provider.Prefix) == false);
							
							providers.Add (provider);
							Resources.provider_hash[provider.Prefix] = provider;
							
							provider.SelectLocale (Resources.culture);
						}
					}
				}
			}
			
			Resources.providers = new IResourceProvider[providers.Count];
			providers.CopyTo (Resources.providers);
		}
		
		
		public static CultureInfo				Culture
		{
			get
			{
				return Resources.culture;
			}
			set
			{
				if (Resources.culture != value)
				{
					Resources.SelectLocale (value);
				}
			}
		}
		
		public static int						ProviderCount
		{
			get { return Resources.providers.Length; }
		}
		
		public static string[]					ProviderPrefixes
		{
			get
			{
				string[] prefixes = new string[Resources.providers.Length];
				
				for (int i = 0; i < Resources.providers.Length; i++)
				{
					prefixes[i] = Resources.providers[i].Prefix;
				}
				
				return prefixes;
			}
		}
		
		public static bool ValidateId(string id)
		{
			IResourceProvider provider = Resources.FindProvider (id, out id);
			
			if (provider != null)
			{
				return provider.ValidateId (id);
			}
			
			return false;
		}
		
		public static bool Contains(string id)
		{
			IResourceProvider provider = Resources.FindProvider (id, out id);
			
			if (provider != null)
			{
				return provider.Contains (id);
			}
			
			return false;
		}

		
		
		public static void DebugDumpProviders()
		{
			for (int i = 0; i < Resources.providers.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Prefix '{0}' implemented by class {1}", Resources.providers[i].Prefix, Resources.providers[i].GetType ().Name));
			}
		}
		
		
		protected static void SelectLocale(CultureInfo culture)
		{
			Resources.culture = culture;
			
			for (int i = 0; i < Resources.providers.Length; i++)
			{
				Resources.providers[i].SelectLocale (culture);
			}
		}
		
		protected static IResourceProvider FindProvider(string full_id, out string local_id)
		{
			if (full_id != null)
			{
				int pos = full_id.IndexOf (":");
			
				if (pos > 0)
				{
					string prefix;
					
					prefix   = full_id.Substring (0, pos);
					local_id = full_id.Substring (pos+1);
					
					IResourceProvider provider = Resources.provider_hash[prefix] as IResourceProvider;
					
					return provider;
				}
			}
			
			local_id = null;
			return null;
		}
		
		
		
		protected static CultureInfo			culture;
		protected static IResourceProvider[]	providers;
		protected static Hashtable				provider_hash;
	}
}
