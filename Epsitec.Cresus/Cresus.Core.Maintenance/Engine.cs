//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Repositories;
using Epsitec.Cresus.Core.Server;

using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Data;

namespace Epsitec.Cresus.Core.Maintenance
{
	public sealed class Engine
	{
		public Engine()
		{
			this.Refresh ();
		}

		public void Refresh()
		{
			var session = CoreServer.Instance.CreateSession ();

			Engine.ImportSwissLocalities (session);
			Engine.ImportCountries (session);

			CoreServer.Instance.DeleteSession (session);
		}

		private static void ImportSwissLocalities(CoreSession session)
		{
			var context = session.GetBusinessContext ();
			var repository = session.CoreData.GetRepository<LocationEntity> (context.DataContext);
			var countryCH = Engine.GetCountry (context, session.CoreData.GetRepository<CountryEntity> (context.DataContext), "CH");

			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
			watch.Start ();

			int count = 0;

			foreach (var locality in SwissPostZip.GetZips ())
			{
				ItemCode onrpCode = ItemCode.Create ("ONRP:" + locality.OnrpCode);
				var location = Engine.GetLocation (context, repository, onrpCode);

				location.Code       = onrpCode.Code;
				location.Name       = locality.LongName;
				location.PostalCode = locality.ZipCode;
				location.Country    = countryCH;

				count++;
			}

			watch.Stop ();
			
			System.Diagnostics.Debug.WriteLine (string.Format ("{0} locations -> {1} ms", count, watch.ElapsedMilliseconds));

			context.SaveChanges ();
		}

		public static void ImportCountries(CoreSession session)
		{
			var context = session.GetBusinessContext ();
			var repository = session.CoreData.GetRepository<CountryEntity> (context.DataContext);

			foreach (var alpha2Code in Iso3166.GetAlpha2Codes ())
			{
				var country          = Engine.GetCountry (context, repository, alpha2Code);
				var multilingualName = new MultilingualText ();

				foreach (var languageCode in Engine.GetLanguageCodes ())
				{
					multilingualName.SetText (languageCode, Iso3166.GetCountryInformation (alpha2Code, languageCode).Name);
				}

				country.CountryCode = alpha2Code;
				country.Name        = multilingualName.GetGlobalText ();
			}

			context.SaveChanges ();
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
	}
}
