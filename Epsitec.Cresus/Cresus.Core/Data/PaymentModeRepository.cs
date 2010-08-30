//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{
	public class PaymentModeRepository : Repository<PaymentModeEntity>
	{
		public PaymentModeRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<PaymentModeEntity> GetPaymentModesByExample(PaymentModeEntity example)
		{
			return this.GetEntitiesByExample<PaymentModeEntity> (example);
		}


		public IEnumerable<PaymentModeEntity> GetPaymentModesByRequest(Request request)
		{
			return this.GetEntitiesByRequest<PaymentModeEntity> (request);
		}


		public IEnumerable<PaymentModeEntity> GetPaymentModesByExample(PaymentModeEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<PaymentModeEntity> (example, index, count);
		}


		public IEnumerable<PaymentModeEntity> GetPaymentModesByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<PaymentModeEntity> (request, index, count);
		}


		public IEnumerable<PaymentModeEntity> GetAllPaymentModes()
		{
			PaymentModeEntity example = this.CreatePaymentModeExample ();

			return this.GetPaymentModesByExample (example);
		}


		public IEnumerable<PaymentModeEntity> GetAllPaymentModes(int index, int count)
		{
			PaymentModeEntity example = this.CreatePaymentModeExample ();

			return this.GetPaymentModesByExample (example, index, count);
		}



		public PaymentModeEntity CreatePaymentModeExample()
		{
			return this.CreateExample<PaymentModeEntity> ();
		}
	}
}
