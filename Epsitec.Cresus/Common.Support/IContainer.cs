namespace Epsitec.Common.Support
{
	public interface IContainer
	{
		ComponentCollection Components { get; }
		
		void NotifyComponentInsertion(ComponentCollection collection, IComponent component);
		void NotifyComponentRemoval(ComponentCollection collection, IComponent component);
	}
}
