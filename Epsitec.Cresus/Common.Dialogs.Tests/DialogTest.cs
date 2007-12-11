using NUnit.Framework;

using Epsitec.Common.UI;

namespace Epsitec.Common.Dialogs
{
	[TestFixture] public class DialogTest
	{
		[SetUp] public void SetUp()
		{
			Epsitec.Common.Document.Engine.Initialize ();
			
			Support.Resources.DefaultManager.DefineDefaultModuleName ("DialogTest");
		}
	}
}
