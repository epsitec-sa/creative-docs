using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.Core.Server.UserInterface
{


	internal sealed class PanelFieldAccessorCache
	{


		public PanelFieldAccessorCache()
		{
			this.keyToPanelFieldAccessor = new Dictionary<string, PanelFieldAccessor> ();
			this.idToPanelFieldAccessor = new Dictionary<int, PanelFieldAccessor> ();
		}


		public PanelFieldAccessor Get(LambdaExpression lambda)
		{
			PanelFieldAccessor accessor;

			var key = PanelFieldAccessorCache.GetKey (lambda);

			var exists = this.keyToPanelFieldAccessor.TryGetValue (key, out accessor);

			if (!exists)
			{
				int id = this.keyToPanelFieldAccessor.Count;

				accessor = new PanelFieldAccessor (lambda, id);

				this.keyToPanelFieldAccessor[key] = accessor;
				this.idToPanelFieldAccessor[id] = accessor;
			}

			return accessor;
		}


		public PanelFieldAccessor Get(int id)
		{
			PanelFieldAccessor accessor;

			this.idToPanelFieldAccessor.TryGetValue (id, out accessor);

			return accessor;
		}


		private static string GetKey(LambdaExpression lambda)
		{
			var part1 = lambda.ToString ();
			var part2 = lambda.ReturnType.FullName;
			var part3 = lambda.Parameters[0].Type.FullName;

			return part1 + part2 + part3;
		}


		private readonly Dictionary<string, PanelFieldAccessor> keyToPanelFieldAccessor;


		private readonly Dictionary<int, PanelFieldAccessor> idToPanelFieldAccessor;


	}


}
