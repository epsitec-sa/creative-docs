using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class FileManagerTest
	{
		[Test]
		public void CheckDelete()
		{
			System.IO.File.WriteAllText (@"S:\test 1.txt", "Fichier 1\r\n");
			System.IO.File.WriteAllText (@"S:\test 2.txt", "Fichier 2\r\n");
			
			FileManager.DeleteFiles (@"S:\test 1.txt", @"S:\test 2.txt");
		}
	}
}
