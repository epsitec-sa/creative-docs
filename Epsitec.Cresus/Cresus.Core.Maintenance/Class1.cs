
using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Server;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Common.Types;

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

			foreach (var country in Iso3166.GetCountries ("fr"))
			{
				var example = repository.CreateExample ();

				example.CountryCode = country.IsoAlpha2;

				var result = repository.GetByExample (example).FirstOrDefault ();

				if (result == null)
				{
					result = context.CreateEntity<CountryEntity> ();
				}

				MultilingualText multilingualName = new MultilingualText ();

				multilingualName.SetText ("fr", country.Name);
				multilingualName.SetText ("de", Iso3166.GetCountryInformation (country.IsoAlpha2, "de").Name);
				multilingualName.SetText ("en", Iso3166.GetCountryInformation (country.IsoAlpha2, "en").Name);
				multilingualName.SetText ("it", Iso3166.GetCountryInformation (country.IsoAlpha2, "it").Name);

				result.CountryCode = country.IsoAlpha2;
				result.Name = multilingualName.GetGlobalText ();
			}

			context.SaveChanges ();
		}
	}
}
