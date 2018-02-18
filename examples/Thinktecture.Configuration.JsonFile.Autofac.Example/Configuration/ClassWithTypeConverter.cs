using System.ComponentModel;

namespace Thinktecture.Configuration.JsonFile.Autofac.Example.Configuration
{
	[TypeConverter(typeof(ClassWithTypeDescriptorTypeConverter))]
	public class ClassWithTypeConverter
	{
		public string Prop { get; set; }
	}
}