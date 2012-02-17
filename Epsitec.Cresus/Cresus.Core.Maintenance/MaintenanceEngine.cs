//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			using (var session = new CoreSession ("maintenance session"))
			{
				MaintenanceEngine.ImportCountries (session);
				MaintenanceEngine.ImportSwissLocations (session);
			}
		}


		private static void ImportSwissLocations(CoreSession session)
		{
			//	Use the MAT[CH]zip data to import swiss locations into the database.
			//	http://www.post.ch/fr/post-startseite/post-adress-services-match/post-direct-marketing-datengrundlage.htm
			var languageCodes = new string[] { "1:de", "2:fr", "3:it", "4:rm" };

			var context    = session.GetBusinessContext ();
			var languages  = MaintenanceEngine.GetLanguages (session, languageCodes);
			var repository = session.CoreData.GetRepository<LocationEntity> (context.DataContext);
			var countryCH  = MaintenanceEngine.GetCountry (context, session.CoreData.GetRepository<CountryEntity> (context.DataContext), "CH");

			System.Diagnostics.Debug.WriteLine ("Importing Swiss locations from MAT[CH]zip database");
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
			watch.Start ();

			int count = 0;

			foreach (var zipInfo in SwissPostZip.GetZips ())
			{
				ItemCode onrpCode = ItemCode.Create ("ONRP:" + zipInfo.OnrpCode);
				var location = MaintenanceEngine.GetLocation (context, repository, onrpCode);

				location.Code       = onrpCode.Code;
				location.Name       = FormattedText.FromSimpleText (zipInfo.LongName);
				location.PostalCode = zipInfo.ZipCode.ToString ("####");
				location.Country    = countryCH;
				location.Language1  = languages.ContainsKey (zipInfo.LanguageCode1) ? languages[zipInfo.LanguageCode1] : null;

				count++;
			}

			System.Diagnostics.Debug.WriteLine (string.Format ("Updated/created {0} locations -> {1} ms", count, watch.ElapsedMilliseconds));
			watch.Restart ();
			context.SaveChanges ();
			System.Diagnostics.Debug.WriteLine (string.Format ("Persisted {0} locations -> {1} ms", count, watch.ElapsedMilliseconds));
		}

		public static void ImportCountries(CoreSession session)
		{
			var context = session.GetBusinessContext ();
			var repository = session.CoreData.GetRepository<CountryEntity> (context.DataContext);

			System.Diagnostics.Debug.WriteLine ("Importing countries from geonames.org database");
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
			watch.Start ();

			int count = 0;

			foreach (var alpha2Code in Iso3166.GetAlpha2Codes ())
			{
				var country          = MaintenanceEngine.GetCountry (context, repository, alpha2Code);
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

		private static Dictionary<SwissPostZipLanguageCode, LanguageEntity> GetLanguages(CoreSession session, IEnumerable<string> languageCodes)
		{
			//	The language code is expressed as "1:de" for instance (key "1", ISO-631 language code "de").

			var context    = session.GetBusinessContext ();
			var repository = session.CoreData.GetRepository<LanguageEntity> (context.DataContext);
			var languages  = new Dictionary<SwissPostZipLanguageCode, LanguageEntity> ();

			foreach (var languageCode in languageCodes)
			{
				var iso = languageCode.Substring (2);
				var key = InvariantConverter.ParseInt<SwissPostZipLanguageCode> (languageCode.Substring (0, 1));

				languages[key] = MaintenanceEngine.GetLanguage (context, repository, iso);
			}

			return languages;
		}

		private static LanguageEntity GetLanguage(BusinessContext context, Repository<LanguageEntity> repository, string languageCode)
		{
			var example = repository.CreateExample ();

			example.IsoLanguageCode = languageCode;

			var language = repository.GetByExample (example).FirstOrDefault ();

			if (language.IsNull ())
			{
				language = context.CreateEntity<LanguageEntity> ();
				language.IsoLanguageCode = languageCode;
				language.Name = MaintenanceEngine.GetLanguageName (languageCode);
			}

			return language;
		}

		private static CountryEntity GetCountry(BusinessContext context, Repository<CountryEntity> repository, string alpha2Code)
		{
			var example = repository.CreateExample ();

			example.CountryCode = alpha2Code;

			return repository.GetByExample (example).FirstOrDefault () ?? context.CreateEntity<CountryEntity> ();
		}

		private static LocationEntity GetLocation(BusinessContext context, Repository<LocationEntity> repository, ItemCode code)
		{
			var example = repository.CreateExample ();

			example.Code = code.Code;

			return repository.GetByExample (example).FirstOrDefault () ?? context.CreateEntity<LocationEntity> ();
		}

		private static IEnumerable<string> GetLanguageCodes()
		{
			yield return "fr";
			yield return "de";
			yield return "en";
			yield return "it";
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
	}
}