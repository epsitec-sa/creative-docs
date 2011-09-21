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

namespace Epsitec.Cresus.Core.Maintenance
{
	public sealed class Engine
	{
		public void Refresh()
		{
			var session = CoreServer.Instance.CreateSession ();

			Engine.ImportCountries (session);

			CoreServer.Instance.DeleteSession (session);
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
		
		private static IEnumerable<string> GetLanguageCodes()
		{
			yield return "fr";
			yield return "de";
			yield return "en";
			yield return "it";
		}
	}
}
