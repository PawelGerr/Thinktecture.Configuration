using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thinktecture.IO;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Loads configuration from a json file.
	/// </summary>
	public class JsonFileConfigurationLoader : IConfigurationLoader<JToken>
	{
		private readonly IFile _file;
		private readonly string _filePath;
		private readonly IJsonTokenConverter _tokenConverter;

		/// <summary>
		/// Creates new instance of <see cref="JsonFileConfigurationLoader"/>.
		/// </summary>
		/// <param name="file">Enables file system access.</param>
		/// <param name="filePath">Json file path.</param>
		/// <param name="tokenConverter">Json token converter.</param>
		public JsonFileConfigurationLoader(IFile file, string filePath, IJsonTokenConverter tokenConverter)
		{
			if (file == null)
				throw new ArgumentNullException(nameof(file));
			if (filePath == null)
				throw new ArgumentNullException(nameof(filePath));

			_file = file;
			_filePath = filePath;
			_tokenConverter = tokenConverter;
		}

		/// <inheritdoc />
		public IConfigurationProvider<JToken> Load()
		{
			var json = _file.ReadAllText(_filePath, Encoding.UTF8);
			var config = JsonConvert.DeserializeObject<JToken>(json);

			return new JsonFileConfigurationProvider(config, _tokenConverter);
		}
	}
}