using NUnit.Framework;

namespace Epsitec.Cresus.DataLayer
{
	[TestFixture]
	public class GeneralTest
	{
		[Test] public void CheckNewDataStore()
		{
			System.Data.DataSet data_set = new System.Data.DataSet ();
			
			//	TODO: remplir le data set
			
			DataStore ds = new DataStore (data_set);
			
			//	TODO: attacher une DbTable...
		}
	}
}
