﻿using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{

	
	public class AddressRepository : Repository
	{


		public AddressRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<AddressEntity> GetAddressesByExample(AddressEntity example)
		{
			return this.GetEntitiesByExample<AddressEntity> (example);
		}


		public IEnumerable<AddressEntity> GetAddressesByExample(AddressEntity example, EntityConstrainer constrainer)
		{
			return this.GetEntitiesByExample<AddressEntity> (example, constrainer);
		}


		public IEnumerable<AddressEntity> GetAddressesByExample(AddressEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<AddressEntity> (example, index, count);
		}


		public IEnumerable<AddressEntity> GetAddressesByExample(AddressEntity example, EntityConstrainer constrainer, int index, int count)
		{
			return this.GetEntitiesByExample<AddressEntity> (example, constrainer, index, count);
		}


		public IEnumerable<AddressEntity> GetAllAddresses()
		{
			AddressEntity example = this.CreateAddressExample ();

			return this.GetAddressesByExample (example);
		}


		public IEnumerable<AddressEntity> GetAllAddresses(int index, int count)
		{
			AddressEntity example = this.CreateAddressExample ();

			return this.GetAddressesByExample (example, index, count);
		}


		public IEnumerable<AddressEntity> GetAddressesByStreet(StreetEntity street)
		{
			AddressEntity example = this.CreateAddressExampleByStreet (street);

			return this.GetAddressesByExample (example);
		}


		public IEnumerable<AddressEntity> GetAddressesByStreet(StreetEntity street, int index, int count)
		{
			AddressEntity example = this.CreateAddressExampleByStreet (street);

			return this.GetAddressesByExample (example, index, count);
		}


		public IEnumerable<AddressEntity> GetAddressesByPostBox(PostBoxEntity postBox)
		{
			AddressEntity example = this.CreateAddressExampleByPostBox (postBox);

			return this.GetAddressesByExample (example);
		}


		public IEnumerable<AddressEntity> GetAddressesByPostBox(PostBoxEntity postBox, int index, int count)
		{
			AddressEntity example = this.CreateAddressExampleByPostBox (postBox);

			return this.GetAddressesByExample (example, index, count);
		}


		public IEnumerable<AddressEntity> GetAddressesByLocation(LocationEntity location)
		{
			AddressEntity example = this.CreateAddressExampleByLocation (location);

			return this.GetAddressesByExample (example);
		}


		public IEnumerable<AddressEntity> GetAddressesByLocation(LocationEntity location, int index, int count)
		{
			AddressEntity example = this.CreateAddressExampleByLocation (location);

			return this.GetAddressesByExample (example, index, count);
		}


		public AddressEntity CreateAddressExample()
		{
			return this.CreateExample<AddressEntity> ();
		}


		private AddressEntity CreateAddressExampleByStreet(StreetEntity street)
		{
			AddressEntity example = this.CreateAddressExample ();
			example.Street = street;

			return example;
		}


		private AddressEntity CreateAddressExampleByPostBox(PostBoxEntity postBox)
		{
			AddressEntity example = this.CreateAddressExample ();
			example.PostBox = postBox;

			return example;
		}


		private AddressEntity CreateAddressExampleByLocation(LocationEntity location)
		{
			AddressEntity example = this.CreateAddressExample ();
			example.Location = location;

			return example;
		}


	}


}
