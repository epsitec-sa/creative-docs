namespace Epsitec.Common.Widgets.Adorner
{
	/// <summary>
	/// La classe Adorner.Factory donne accès à l'interface IAdorner actuellement
	/// active.
	/// </summary>
	public class Factory
	{
		private Factory()
		{
			//	On ne peut pas instancier Factory !
		}
		
		static Factory()
		{
			Factory.adorner = new Default ();
		}
		
		public static IAdorner			Adorner
		{
			get { return Factory.adorner; }
		}
		
		
		private static IAdorner			adorner;
	}
}
