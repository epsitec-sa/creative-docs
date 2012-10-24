using System.Collections.Generic;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.WebCore.Server.NancyModules;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class EntityValue
	{


		public string Displayed
		{
			get;
			set;
		}


		public string Submitted
		{
			get;
			set;
		}


		public Dictionary<string, object> ToDictionary()
		{
			return new Dictionary<string, object> ()
			{
				{ "displayed", this.Displayed },
				{ "submitted", this.Submitted },
			};
		}


		public static EntityValue Create(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			if (entity == null)
			{
				return new EntityValue ()
				{
					Displayed = Res.Strings.EmptyValue.ToSimpleText (),
					Submitted = Constants.KeyForNullValue,
				};
			}

			return new EntityValue ()
			{
				Displayed = entity.GetCompactSummary ().ToString (),
				Submitted = layoutBuilder.GetEntityId (entity),
			};
		}


	}


}
