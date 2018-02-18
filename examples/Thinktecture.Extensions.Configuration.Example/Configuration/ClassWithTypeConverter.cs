using System.ComponentModel;

namespace Thinktecture.Extensions.Configuration.Example.Configuration
{
	[TypeConverter(typeof(ClassWithTypeDescriptorTypeConverter))]
	public class ClassWithTypeConverter
	{
		public string Prop { get; set; }
	}
}
