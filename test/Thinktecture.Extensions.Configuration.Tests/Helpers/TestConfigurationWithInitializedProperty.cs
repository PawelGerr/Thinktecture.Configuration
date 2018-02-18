namespace Thinktecture.Helpers
{
	public class TestConfigurationWithInitializedProperty<T1>
		where T1 : new()
	{
		public T1 P1 { get; set; } = new T1();
	}
}
