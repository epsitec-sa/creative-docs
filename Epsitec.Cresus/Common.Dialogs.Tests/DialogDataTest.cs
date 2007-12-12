using NUnit.Framework;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using Demo.Demo5juin.Entities;

namespace Epsitec.Common.Dialogs
{
	[TestFixture]
	public class DialogDataTest
	{
		[Test]
		public void Check01DialogModeIsolated()
		{
			EntityContext context = EntityContext.Current;
			AbstractEntity entity = context.CreateEmptyEntity<PrixEntity> ();
			DialogData data = new DialogData (entity, DialogDataMode.Isolated);

			PrixEntity prix = data.Data as PrixEntity;
		}
		
		[Test]
		public void Check02DialogModeRealTime()
		{
			EntityContext context = EntityContext.Current;
			AbstractEntity entity = context.CreateEmptyEntity<PrixEntity> ();
			DialogData data = new DialogData (entity, DialogDataMode.RealTime);

			PrixEntity prix = data.Data as PrixEntity;
		}
	}
}
