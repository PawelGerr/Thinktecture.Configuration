using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Creates and populates types using <see cref="IConfiguration"/>.
	/// </summary>
	public interface IMicrosoftConfigurationConverter
	{
		/// <summary>
		/// Creates a new instance of type <typeparamref name="T"/> and populates it with values from <paramref name="configuration"/>.
		/// </summary>
		/// <typeparam name="T">Type an instance to create of.</typeparam>
		/// <param name="configuration">Provides values to populate an instance of type <typeparamref name="T"/>.</param>
		/// <returns>A new instance of type <typeparamref name="T"/>.</returns>
		T Convert<T>(IConfiguration configuration);

		/// <summary>
		/// Creates a new instance of provided <paramref name="type"/> and populates it with values from <paramref name="configuration"/>.
		/// </summary>
		/// <param name="configuration">Provides values to populate an instance of type <paramref name="type"/>.</param>
		/// <param name="type">Type an instance to create of.</param>
		/// <returns>A new instance of provided <paramref name="type"/>.</returns>
		object Convert(IConfiguration configuration, Type type);
	}
}