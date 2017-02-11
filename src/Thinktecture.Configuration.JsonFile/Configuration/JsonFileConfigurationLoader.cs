using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;
using Thinktecture.Text;

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
		private readonly Func<JsonSerializerSettings> _jsonSerializerProvider;
		private readonly Func<JToken, IJsonTokenConverter, IConfigurationProvider<JToken>> _jsonConfigurationProviderFactory;
		private readonly IEncoding _encoding;

		/// <summary>
		/// Creates new instance of <see cref="JsonFileConfigurationLoader"/>.
		/// </summary>
		/// <param name="file">Enables file system access.</param>
		/// <param name="filePath">Json file path.</param>
		/// <param name="tokenConverter">Json token converter.</param>
		/// <param name="encoding">Encoding to be used for reading json file.</param>
		/// <param name="jsonSerializerProvider">Provides <see cref="JsonSerializerSettings"/> for deserialization of file content to <see cref="JToken"/>.</param>
		/// <param name="jsonConfigurationProviderFactory">A factory for creation of <see cref="IConfigurationProvider{JToken}"/>.</param>
		public JsonFileConfigurationLoader(IFile file, string filePath, IJsonTokenConverter tokenConverter, IEncoding encoding = null,
			Func<JsonSerializerSettings> jsonSerializerProvider = null,
			Func<JToken, IJsonTokenConverter, IConfigurationProvider<JToken>> jsonConfigurationProviderFactory = null)
		{
			if (file == null)
				throw new ArgumentNullException(nameof(file));
			if (filePath == null)
				throw new ArgumentNullException(nameof(filePath));
			if (tokenConverter == null)
				throw new ArgumentNullException(nameof(tokenConverter));

			_file = file;
			_filePath = filePath;
			_tokenConverter = tokenConverter;
			_jsonSerializerProvider = jsonSerializerProvider;
			_encoding = encoding ?? new UTF8Encoding(false, true).ToInterface();
			_jsonConfigurationProviderFactory = jsonConfigurationProviderFactory ?? ((token, converter) => new JsonFileConfigurationProvider(token, converter));
		}

		/// <inheritdoc />
		public IConfigurationProvider<JToken> Load()
		{
			var serializer = (_jsonSerializerProvider == null) ? JsonSerializer.CreateDefault() : JsonSerializer.CreateDefault(_jsonSerializerProvider());

			using (var stream = _file.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var textReader = new StreamReaderAdapter(stream, _encoding, true))
			using (var jsonReader = new JsonTextReader(textReader.ToImplementation()))
			{
				var token = serializer.Deserialize<JToken>(jsonReader);
				return _jsonConfigurationProviderFactory(token, _tokenConverter);
			}
		}
	}
}