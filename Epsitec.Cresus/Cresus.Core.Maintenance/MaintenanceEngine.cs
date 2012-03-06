//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Repositories;
using Epsitec.Cresus.WebCore.Server.CoreServer;

using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Data;
using Epsitec.Common.IO;

namespace Epsitec.Cresus.Core.Maintenance
{
	public sealed class MaintenanceEngine
	{
		public MaintenanceEngine()
		{
			this.Refresh ();
		}


		private void Refresh()
		{
			using (this.session = new CoreSession ("maintenance session"))
			{
				this.context    = this.session.GetBusinessContext ();
				this.data       = this.session.CoreData;

				this.cantonRepository   = this.context.GetRepository<StateProvinceCountyEntity> ();
				this.locationRepository = this.context.GetRepository<LocationEntity> ();
				this.countryRepository  = this.context.GetRepository<CountryEntity> ();
				this.languageRepository = this.context.GetRepository<LanguageEntity> ();
				
				this.countryCH  = this.GetCountry ("CH");
				
				this.ImportCountries ();
				this.ImportSwissCantons ();
				this.ImportSwissLocations ();
			}
		}


		private void ImportSwissLocations()
		{
			//	Use the MAT[CH]zip data to import swiss locations into the database.
			//	http://www.post.ch/fr/post-startseite/post-adress-services-match/post-direct-marketing-datengrundlage.htm
			var languageCodes = new string[] { "1:de", "2:fr", "3:it", "4:rm" };

			var languages  = this.GetLanguages (languageCodes);

			System.Diagnostics.Debug.WriteLine ("Importing Swiss locations from MAT[CH]zip database");
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
			watch.Start ();

			int count = 0;

			foreach (var zipInfo in SwissPostZipRepository.Current.FindAll ())
			{
				ItemCode onrpCode = ItemCode.Create ("ONRP:" + zipInfo.OnrpCode);
				var location = this.GetLocation (onrpCode);

				location.Code       = onrpCode.Code;
				location.Name       = FormattedText.FromSimpleText (zipInfo.LongName);
				location.PostalCode = zipInfo.ZipCode.ToString ("####");
				location.Country    = this.countryCH;
				location.Language1  = languages.ContainsKey (zipInfo.LanguageCode1) ? languages[zipInfo.LanguageCode1] : null;

				count++;
			}

			System.Diagnostics.Debug.WriteLine (string.Format ("Updated/created {0} locations -> {1} ms", count, watch.ElapsedMilliseconds));
			watch.Restart ();
			context.SaveChanges ();
			System.Diagnostics.Debug.WriteLine (string.Format ("Persisted {0} locations -> {1} ms", count, watch.ElapsedMilliseconds));
		}

		private void ImportSwissCantons()
		{
			var cantonTable = new StringLineTable (MaintenanceEngine.GetCantonTableLines ());

			string[] ids = cantonTable.Header.Split ('\t');

			foreach (var row in cantonTable.Rows)
			{
				string[] cols = row.Split ('\t');

				if (cols.Length == ids.Length)
				{
					string id = cols[0];

					StateProvinceCountyEntity canton = this.GetCanton (id);

					var multilingualName = new MultilingualText ();

					for (int i = 1; i < cols.Length; i++)
					{
						string languageCode = ids[i];
						multilingualName.SetText (languageCode, FormattedText.FromSimpleText (cols[i]));
					}

					canton.Name = multilingualName.GetGlobalText ();
				}
			}
		}

		public void ImportCountries()
		{
			System.Diagnostics.Debug.WriteLine ("Importing countries from geonames.org database");
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
			watch.Start ();

			int count = 0;

			foreach (var alpha2Code in Iso3166.GetAlpha2Codes ())
			{
				var country          = this.GetCountry (alpha2Code);
				var multilingualName = new MultilingualText ();

				foreach (var languageCode in MaintenanceEngine.GetLanguageCodes ())
				{
					multilingualName.SetText (languageCode, FormattedText.FromSimpleText (Iso3166.GetCountryInformation (alpha2Code, languageCode).Name));
				}

				country.CountryCode = alpha2Code;
				country.Name        = multilingualName.GetGlobalText ();
				country.IsPreferred = MaintenanceEngine.IsPreferredCountry (alpha2Code);

				count++;
			}

			System.Diagnostics.Debug.WriteLine (string.Format ("Updated/created {0} countries -> {1} ms", count, watch.ElapsedMilliseconds));
			watch.Restart ();
			context.SaveChanges ();
			System.Diagnostics.Debug.WriteLine (string.Format ("Persisted {0} countries -> {1} ms", count, watch.ElapsedMilliseconds));
		}

		private Dictionary<SwissPostLanguageCode, LanguageEntity> GetLanguages(IEnumerable<string> languageCodes)
		{
			//	The language code is expressed as "1:de" for instance (key "1", ISO-631 language code "de").

			var context    = this.session.GetBusinessContext ();
			var languages  = new Dictionary<SwissPostLanguageCode, LanguageEntity> ();

			foreach (var languageCode in languageCodes)
			{
				var iso = languageCode.Substring (2);
				var key = InvariantConverter.ParseInt<SwissPostLanguageCode> (languageCode.Substring (0, 1));

				languages[key] = this.GetLanguage (iso);
			}

			return languages;
		}

		private LanguageEntity GetLanguage(string languageCode)
		{
			var example = this.languageRepository.CreateExample ();

			example.IsoLanguageCode = languageCode;

			var language = this.languageRepository.GetByExample (example).FirstOrDefault ();

			if (language.IsNull ())
			{
				language = context.CreateEntity<LanguageEntity> ();
				language.IsoLanguageCode = languageCode;
				language.Name = MaintenanceEngine.GetLanguageName (languageCode);
			}

			return language;
		}

		private CountryEntity GetCountry(string alpha2Code)
		{
			var example = this.countryRepository.CreateExample ();

			example.CountryCode = alpha2Code;

			return this.countryRepository.GetByExample (example).FirstOrDefault ()
				?? this.context.CreateEntity<CountryEntity> ();
		}

		private LocationEntity GetLocation(ItemCode code)
		{
			var example = this.locationRepository.CreateExample ();

			example.Code = code.Code;

			return this.locationRepository.GetByExample (example).FirstOrDefault ()
				?? this.context.CreateEntity<LocationEntity> ();
		}

		private StateProvinceCountyEntity GetCanton(string code)
		{
			var example = this.cantonRepository.CreateExample ();

			example.RegionCode = code;
			example.Country    = this.countryCH;

			var result = this.cantonRepository.GetByExample (example).FirstOrDefault ();

			if (result == null)
			{
				result = this.context.CreateEntity<StateProvinceCountyEntity> ();
				result.Country = this.countryCH;
				result.RegionCode =	code;
			}
			
			return result;
		}



		private static IEnumerable<string> GetLanguageCodes()
		{
			yield return "fr";
			yield return "de";
			yield return "en";
			yield return "it";
		}

		private static IEnumerable<string> GetCantonTableLines()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			var resource = "Epsitec.Cresus.Core.Maintenance.DataFiles.CantonTable.txt";

			using (System.IO.Stream stream = assembly.GetManifestResourceStream (resource))
			{
				return Epsitec.Common.IO.StringLineExtractor.GetLines (stream);
			}
		}
		
		private static bool IsPreferredCountry(string alpha2Code)
		{
			switch (alpha2Code.ToLowerInvariant ())
			{
				case "fr":
				case "ch":
				case "de":
				case "it":
				case "li":
				case "at":
					return true;
				default:
					return false;
			}
		}

		private static FormattedText GetLanguageName(string code)
		{
			switch (code)
			{
				case "fr":
					return TextFormatter.FormatText ("français");
				case "de":
					return TextFormatter.FormatText ("allemand");
				case "it":
					return TextFormatter.FormatText ("italien");
				case "rm":
					return TextFormatter.FormatText ("romanche");

				default:
					return FormattedText.Empty;
			}
		}

		private CoreSession session;
		private BusinessContext context;
		private CoreData data;
		private Repository<StateProvinceCountyEntity> cantonRepository;
		private Repository<CountryEntity> countryRepository;
		private Repository<LocationEntity> locationRepository;
		private Repository<LanguageEntity> languageRepository;

		private CountryEntity countryCH;
	}
}