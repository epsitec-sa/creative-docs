//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{
	public class CurrencyRepository : Repository
	{
		public CurrencyRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<CurrencyEntity> GetCurrenciesByExample(CurrencyEntity example)
		{
			return this.GetEntitiesByExample<CurrencyEntity> (example);
		}


		public IEnumerable<CurrencyEntity> GetCurrenciesByRequest(Request request)
		{
			return this.GetEntitiesByRequest<CurrencyEntity> (request);
		}


		public IEnumerable<CurrencyEntity> GetCurrenciesByExample(CurrencyEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<CurrencyEntity> (example, index, count);
		}


		public IEnumerable<CurrencyEntity> GetCurrenciesByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<CurrencyEntity> (request, index, count);
		}


		public IEnumerable<CurrencyEntity> GetAllCurrencies()
		{
			CurrencyEntity example = this.CreateCurrencyExample ();

			return this.GetCurrenciesByExample (example);
		}


		public IEnumerable<CurrencyEntity> GetAllCurrencies(int index, int count)
		{
			CurrencyEntity example = this.CreateCurrencyExample ();

			return this.GetCurrenciesByExample (example, index, count);
		}



		public CurrencyEntity CreateCurrencyExample()
		{
			return this.CreateExample<CurrencyEntity> ();
		}
	}
}
