using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class CustomerRepository : Repository
	{


		public CustomerRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<CustomerEntity> GetCustomersByExample(CustomerEntity example)
		{
			return this.GetEntitiesByExample<CustomerEntity> (example);
		}


		public IEnumerable<CustomerEntity> GetCustomersByExample(CustomerEntity example, EntityConstrainer constrainer)
		{
			return this.GetEntitiesByExample<CustomerEntity> (example, constrainer);
		}


		public IEnumerable<CustomerEntity> GetCustomersByExample(CustomerEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<CustomerEntity> (example, index, count);
		}


		public IEnumerable<CustomerEntity> GetCustomersByExample(CustomerEntity example, EntityConstrainer constrainer, int index, int count)
		{
			return this.GetEntitiesByExample<CustomerEntity> (example, constrainer, index, count);
		}


		public IEnumerable<CustomerEntity> GetAllCustomers()
		{
			CustomerEntity example = this.CreateCustomerExample ();

			return this.GetCustomersByExample (example);
		}


		public IEnumerable<CustomerEntity> GetAllCustomers(int index, int count)
		{
			CustomerEntity example = this.CreateCustomerExample ();

			return this.GetCustomersByExample (example, index, count);
		}

		
		public CustomerEntity CreateCustomerExample()
		{
			return this.CreateExample<CustomerEntity> ();
		}


	}


}
