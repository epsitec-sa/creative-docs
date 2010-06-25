using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{

	
	public class AbstractPersonRepository : Repository
	{


		public AbstractPersonRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByExample(AbstractPersonEntity example)
		{
			return this.GetGenericPersonsByExample<AbstractPersonEntity> (example);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByExample(AbstractPersonEntity example, EntityConstrainer constrainer)
		{
			return this.GetGenericPersonsByExample<AbstractPersonEntity> (example, constrainer);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByExample(AbstractPersonEntity example, int index, int count)
		{
			return this.GetGenericPersonsByExample<AbstractPersonEntity> (example, index, count);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByExample(AbstractPersonEntity example, EntityConstrainer constrainer, int index, int count)
		{
			return this.GetGenericPersonsByExample<AbstractPersonEntity> (example, constrainer, index, count);
		}


		public IEnumerable<AbstractPersonEntity> GetAllAbstractPersons()
		{
			return this.GetAllGenericPersons<AbstractPersonEntity> ();
		}


		public IEnumerable<AbstractPersonEntity> GetAllAbstractPersons(int index, int count)
		{
			return this.GetAllGenericPersons<AbstractPersonEntity> (index, count);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByPreferredLanguage(LanguageEntity preferredLanguage)
		{
			return this.GetGenericPersonsByPreferredLanguage<AbstractPersonEntity> (preferredLanguage);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByPreferredLanguage(LanguageEntity preferredLanguage, int index, int count)
		{
			return this.GetGenericPersonsByPreferredLanguage<AbstractPersonEntity> (preferredLanguage, index, count);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByContacts(params AbstractContactEntity[] contacts)
		{
			return this.GetGenericPersonsByContacts<AbstractPersonEntity> (contacts);
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersonsByContacts(int index, int count, params AbstractContactEntity[] contacts)
		{
			return this.GetGenericPersonsByContacts<AbstractPersonEntity> (index, count, contacts);
		}


		public AbstractPersonEntity CreateAbstractPersonExample()
		{
			return this.CreateGenericPersonExample<AbstractPersonEntity> ();
		}


		protected IEnumerable<T> GetGenericPersonsByExample<T>(T example) where T : AbstractPersonEntity
		{
			return this.GetEntitiesByExample<T> (example);
		}


		protected IEnumerable<T> GetGenericPersonsByExample<T>(T example, EntityConstrainer constrainer) where T : AbstractPersonEntity
		{
			return this.GetEntitiesByExample<T> (example, constrainer);
		}


		protected IEnumerable<T> GetGenericPersonsByExample<T>(T example, int index, int count) where T : AbstractPersonEntity
		{
			return this.GetEntitiesByExample<T> (example, index, count);
		}


		protected IEnumerable<T> GetGenericPersonsByExample<T>(T example, EntityConstrainer constrainer, int index, int count) where T : AbstractPersonEntity
		{
			return this.GetEntitiesByExample<T> (example, constrainer, index, count);
		}


		protected IEnumerable<T> GetAllGenericPersons<T>() where T : AbstractPersonEntity, new ()
		{
			T example = this.CreateGenericPersonExample<T> ();

			return this.GetGenericPersonsByExample<T> (example);
		}


		protected IEnumerable<T> GetAllGenericPersons<T>(int index, int count) where T : AbstractPersonEntity, new ()
		{
			T example = this.CreateGenericPersonExample<T> ();

			return this.GetGenericPersonsByExample<T> (example, index, count);
		}


		protected IEnumerable<T> GetGenericPersonsByPreferredLanguage<T>(LanguageEntity preferredLanguage) where T : AbstractPersonEntity, new ()
		{
			T example = this.CreateGenericPersonExampleByPreferredLanguage<T> (preferredLanguage);

			return this.GetGenericPersonsByExample<T> (example);
		}


		protected IEnumerable<T> GetGenericPersonsByPreferredLanguage<T>(LanguageEntity preferredLanguage, int index, int count) where T : AbstractPersonEntity, new ()
		{
			T example = this.CreateGenericPersonExampleByPreferredLanguage<T> (preferredLanguage);

			return this.GetGenericPersonsByExample<T> (example, index, count);
		}


		protected IEnumerable<T> GetGenericPersonsByContacts<T>(params AbstractContactEntity[] contacts) where T : AbstractPersonEntity, new ()
		{
			T example = this.CreateGenericPersonExampleByContacts<T> (contacts);

			return this.GetGenericPersonsByExample<T> (example);
		}


		protected IEnumerable<T> GetGenericPersonsByContacts<T>(int index, int count, params AbstractContactEntity[] contacts) where T : AbstractPersonEntity, new ()
		{
			T example = this.CreateGenericPersonExampleByContacts<T> (contacts);

			return this.GetGenericPersonsByExample<T> (example, index, count);
		}


		protected T CreateGenericPersonExample<T>() where T : AbstractPersonEntity, new ()
		{
			return this.CreateExample<T> ();
		}


		protected T CreateGenericPersonExampleByPreferredLanguage<T>(LanguageEntity preferredLanguage) where T : AbstractPersonEntity, new ()
		{
			T example = this.CreateGenericPersonExample<T> ();
			example.PreferredLanguage = preferredLanguage;

			return example;
		}


		protected T CreateGenericPersonExampleByContacts<T>(IEnumerable<AbstractContactEntity> contacts) where T : AbstractPersonEntity, new ()
		{
			T example = this.CreateGenericPersonExample<T> ();

			foreach (AbstractContactEntity contact in contacts)
			{
				example.Contacts.Add (contact);
			}

			return example;
		}


	}


}
