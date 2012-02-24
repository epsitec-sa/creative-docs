using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor
{


	internal sealed class PanelFieldAccessorCache
	{


		public PanelFieldAccessorCache()
		{
			this.lambdaToPanelFieldAccessor = new Dictionary<string, AbstractPanelFieldAccessor> ();
			this.idToPanelFieldAccessor = new Dictionary<string, AbstractPanelFieldAccessor> ();
		}


		public AbstractPanelFieldAccessor Get(LambdaExpression lambda)
		{
			AbstractPanelFieldAccessor panelFieldAccessor;

			var lambdaKey = PanelFieldAccessorCache.GetLambdaKey (lambda);

			var exists = this.lambdaToPanelFieldAccessor.TryGetValue (lambdaKey, out panelFieldAccessor);

			if (!exists)
			{
				var id = InvariantConverter.ToString (this.lambdaToPanelFieldAccessor.Count);

				panelFieldAccessor = AbstractPanelFieldAccessor.Create (lambda, id);

				this.lambdaToPanelFieldAccessor[lambdaKey] = panelFieldAccessor;
				this.idToPanelFieldAccessor[id] = panelFieldAccessor;
			}

			return panelFieldAccessor;
		}


		public AbstractPanelFieldAccessor Get(string id)
		{
			AbstractPanelFieldAccessor panelFieldAccessor;

			this.idToPanelFieldAccessor.TryGetValue (id, out panelFieldAccessor);

			return panelFieldAccessor;
		}


		private static string GetLambdaKey(LambdaExpression lambda)
		{
			var part1 = lambda.ToString ();
			var part2 = lambda.ReturnType.FullName;
			var part3 = lambda.Parameters[0].Type.FullName;

			return part1 + part2 + part3;
		}


		private readonly Dictionary<string, AbstractPanelFieldAccessor> lambdaToPanelFieldAccessor;


		private readonly Dictionary<string, AbstractPanelFieldAccessor> idToPanelFieldAccessor;


	}


}
