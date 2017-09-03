using System;
using System.Globalization;
using Thinktecture.Configuration;

namespace Thinktecture.Helpers
{
	public class TestInstanceCreator : InstanceCreator
	{
		public TestInstanceCreator(CultureInfo culture)
			: base(culture)
		{
		}

		public override IConversionResult Create(Type type)
		{
			throw new NotImplementedException();
		}
	}
}
