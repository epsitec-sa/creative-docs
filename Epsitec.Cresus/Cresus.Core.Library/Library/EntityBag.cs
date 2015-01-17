//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public static class EntityBag
	{
		public static string GetId(AbstractEntity entity)
		{
			var context = DataContext.GetDataContext (entity);
			var key = context.GetNormalizedEntityKey (entity);

			if (key == null)
			{
				return "";
			}
			else
			{
				return key.Value.ToString ().Replace ('/', '-');
			}
		}
		
		public static void Process<T>(T entity, System.Action<T> action)
			where T : AbstractEntity
		{
			if (entity == null)
			{
				return;
			}

			using (EntityBag.Loading ())
			{
				action (entity);

				EntityBag.Remove (entity);
			}
		}

		public static void Add(AbstractEntity entity, string title)
		{
			EntityBag.Add (entity, title, entity.GetSummary ());
		}

		public static void Add(AbstractEntity entity, string title, FormattedText summary)
		{
			var manager = EntityBagManager.GetCurrentEntityBagManager ();
			var loginName = EntityBag.GetLoginName ();
			var id = EntityBag.GetId (entity);

			manager.AddToBag (loginName, title, summary, id, When.Now);
		}

		public static void Remove(AbstractEntity entity)
		{
			var id = EntityBag.GetId (entity);

			EntityBag.Remove (id);
		}

		public static void Remove(string id)
		{
			var manager = EntityBagManager.GetCurrentEntityBagManager ();
			var loginName = EntityBag.GetLoginName ();

			manager.RemoveFromBag (loginName, id, When.Now);
		}

		public static IEnumerable<AbstractEntity> GetEntities(DataContext context)
		{
			var manager   = EntityBagManager.GetCurrentEntityBagManager ();
			var loginName = EntityBag.GetLoginName ();
			
			var entitiesId = manager.GetUserBagEntitiesId (loginName).ToList ();

			return entitiesId.Select (x => context.GetPersistedEntity (x));
		}

		public static System.IDisposable Loading()
		{
			var manager = EntityBagManager.GetCurrentEntityBagManager ();
			var loginName = EntityBag.GetLoginName ();
			
			manager.SetLoading (loginName, state: true);

			return new StopLoading (manager, loginName);
		}

		public static string GetLoginName()
		{
			var user = UserManager.Current.AuthenticatedUser;
			return user == null ? "" : user.LoginName;
		}


		private class StopLoading : System.IDisposable
		{
			public StopLoading(EntityBagManager manager, string loginName)
			{
				this.manager = manager;
				this.loginName = loginName;
			}


			#region IDisposable Members

			public void Dispose()
			{
				this.manager.SetLoading (this.loginName, state: false);
			}

			#endregion


			private readonly EntityBagManager	manager;
			private readonly string				loginName;
		}
	}
}

