using Epsitec.Common.Support.Extensions;


namespace Epsitec.Cresus.Database.Services
{


	public abstract class DbAbstractService
	{

		
		// TODO Comment this class.
		// Marc


		internal DbAbstractService(DbInfrastructure dbInfrastructure)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");

			this.IsTurnedOn = false;
			this.DbInfrastructure = dbInfrastructure;
		}


		internal bool IsTurnedOn
		{
			get;
			private set;
		}


		protected DbInfrastructure DbInfrastructure
		{
			get;
			private set;
		}


		internal virtual void TurnOn()
		{
			this.IsTurnedOn = true;
		}


		protected void CheckIsTurnedOn()
		{
			if (!this.IsTurnedOn)
			{
				throw new System.InvalidOperationException ("This instance is not turned on.");
			}
		}

	
	}


}
