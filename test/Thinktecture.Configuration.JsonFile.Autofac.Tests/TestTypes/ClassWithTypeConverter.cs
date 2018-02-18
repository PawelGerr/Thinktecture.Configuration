using System.ComponentModel;

namespace Thinktecture.TestTypes
{
	[TypeConverter(typeof(ClassWithTypeDescriptorTypeConverter))]
	public class ClassWithTypeConverter
	{
		public string Prop { get; set; }
	}
}
