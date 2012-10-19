namespace Epsitec.Cresus.Core.Business.UserManagement
{
	public sealed class UserScope
	{
		public UserScope(string id, string name)
		{
			this.id = id;
			this.name = name;
		}


		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}


		private readonly string id;
		private readonly string name;
	}
}
