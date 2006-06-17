using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class CommandTest
	{
		[SetUp]
		public void Initialise()
		{
			Epsitec.Common.Document.Engine.Initialise ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");

			Resources.DefaultManager.ActiveCulture = Resources.FindSpecificCultureInfo ("fr");
		}
		
		[Test]
		public void CheckApplicationCommands()
		{
			Assert.AreEqual ("Couper", ApplicationCommands.Cut.LongCaption);
			Assert.AreEqual ("Copier", ApplicationCommands.Copy.LongCaption);
			Assert.AreEqual ("Coller", ApplicationCommands.Paste.LongCaption);
			Assert.AreEqual ("Supprimer", ApplicationCommands.Delete.LongCaption);
			Assert.AreEqual ("Sélectionner tout", ApplicationCommands.SelectAll.LongCaption);
		}

		[Test]
		public void CheckCommandFromDruid()
		{
			Command command = Command.Get (Druid.Parse ("[0005]"));

			Assert.IsNotNull (command);
			Assert.AreEqual ("[0005]", command.Name);

			Assert.AreEqual ("Sélectionner tout", command.LongCaption);
			Assert.AreEqual (ApplicationCommands.SelectAll, command);
		}
	}
}