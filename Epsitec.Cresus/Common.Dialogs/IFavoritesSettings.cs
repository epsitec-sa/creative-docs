namespace Epsitec.Common.Dialogs
{
	public interface IFavoritesSettings
	{
		bool FavoritesBig
		{
			get;
			set;
		}

		System.Collections.ArrayList FavoritesList
		{
			get;
		}
	}
}
