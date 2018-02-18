using System.ComponentModel;

namespace Thinktecture.Helpers
{
	[TypeConverter(typeof(ClassWithTypeDescriptorTypeConverter))]
	public class ClassWithTypeConverter
	{
		public string Prop { get; set; }
	}
}
