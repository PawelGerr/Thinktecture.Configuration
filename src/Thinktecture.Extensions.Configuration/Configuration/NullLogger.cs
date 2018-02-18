#if NETSTANDARD1_1
using System;
using Microsoft.Extensions.Logging;

namespace Thinktecture.Configuration
{
	internal class NullLogger<T> : ILogger<T>
	{
		public static readonly NullLogger<T> Instance = new NullLogger<T>();

		public IDisposable BeginScope<TState>(TState state)
		{
			return NullDisposable.Instance;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return false;
		}

		private class NullDisposable : IDisposable
		{
			// ReSharper disable once MemberHidesStaticFromOuterClass
			public static readonly NullDisposable Instance = new NullDisposable();

			public void Dispose()
			{
			}
		}
	}
}
#endif
