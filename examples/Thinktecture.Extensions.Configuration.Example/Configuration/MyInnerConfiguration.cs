using System;

namespace Thinktecture.Extensions.Configuration.Example.Configuration
{
	// ReSharper disable once ClassNeverInstantiated.Global
	// ReSharper disable UnusedAutoPropertyAccessor.Global
	// ReSharper disable MemberCanBePrivate.Global
	public class MyInnerConfiguration : IMyInnerConfiguration
	{
		public string InnerValue { get; set; }
		public MyDependency Dependency { get; }

		public MyInnerConfiguration(MyDependency dependency)
		{
			Dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
		}
	}
}
