namespace Thinktecture.TestTypes
{
	public class ConfigurationWithNonDefaultCtor
		: IConfigurationWithNonDefaultCtor
	{
		public SimpleDependency SimpleDependency { get; }

		public ConfigurationWithNonDefaultCtor(SimpleDependency simpleDependency)
		{
			SimpleDependency = simpleDependency;
		}
	}
}
