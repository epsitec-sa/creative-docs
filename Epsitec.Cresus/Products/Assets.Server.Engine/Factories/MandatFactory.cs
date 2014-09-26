//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.Engine
{
	public class MandatFactory
	{
		public string						Name;
		public bool							IsDefault;
		public System.Action<DataAccessor, string, System.DateTime, bool> Create;

		public static IEnumerable<MandatFactory> Factories
		{
			get
			{
				yield return new MandatFactory
				{
					Name = "Vide",
					Create = delegate (DataAccessor accessor, string name, System.DateTime startDate, bool withSamples)
					{
						using (var factory = new EmptyMandatFactory (accessor))
						{
							factory.Create (name, startDate, withSamples);
						}
					},
				};

				yield return new MandatFactory
				{
					Name = "Pour collectivité publique (MCH2)",
					Create = delegate (DataAccessor accessor, string name, System.DateTime startDate, bool withSamples)
					{
						using (var factory = new MCH2MandatFactory (accessor))
						{
							factory.Create (name, startDate, withSamples);
						}
					},
				};

				yield return new MandatFactory
				{
					Name = "Pour test (MCH2)",
					IsDefault = true,
					Create = delegate (DataAccessor accessor, string name, System.DateTime startDate, bool withSamples)
					{
						using (var factory = new TestMandatFactory (accessor))
						{
							factory.Create (name, startDate, withSamples);
						}
					},
				};

				yield return new MandatFactory
				{
					Name = "Pour entreprise",
					Create = delegate (DataAccessor accessor, string name, System.DateTime startDate, bool withSamples)
					{
						using (var factory = new CompanyMandatFactory (accessor))
						{
							factory.Create (name, startDate, withSamples);
						}
					},
				};
			}
		}
	}
}
