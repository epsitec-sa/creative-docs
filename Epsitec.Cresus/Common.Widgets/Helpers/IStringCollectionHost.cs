namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// L'interface IStringCollectionHost permet d'offrir le support pour
	/// la classe StringCollection.
	/// </summary>
	public interface IStringCollectionHost
	{
		void StringCollectionChanged();
		
		StringCollection	Items	{ get; }
	}
}
