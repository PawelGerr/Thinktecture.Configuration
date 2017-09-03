using System;
using System.IO;
using System.Linq;
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
	public class JsonFileConfigurationLoader : IConfigurationLoader<JToken, JToken>
	{
		private readonly IFile _file;
		private readonly string[] _filePaths;
		private readonly IJsonTokenConverter _tokenConverter;
		private readonly Func<JsonSerializerSettings> _jsonSerializerSettingsProvider;
		private readonly Func<JToken[], IJsonTokenConverter, IConfigurationProvider<JToken, JToken>> _jsonConfigurationProviderFactory;
		private readonly IEncoding _encoding;

		/// <summary>
		/// Creates new instance of <see cref="JsonFileConfigurationLoader"/>.
		/// </summary>
		/// <param name="file">Enables file system access.</param>
		/// <param name="tokenConverter">Json token converter.</param>
		/// <param name="filePaths">Json file paths. The first file is considered the main file, the others act as overrides.</param>
		/// <param name="encoding">Encoding to be used for reading json file.</param>
		/// <param name="jsonSerializerSettingsProvider">Provides <see cref="JsonSerializerSettings"/> for deserialization of file content to <see cref="JToken"/>.</param>
		/// <param name="jsonConfigurationProviderFactory">A factory for creation of <see cref="IConfigurationProvider{TRawDataIn,TRawDataOut}"/>.</param>
		public JsonFileConfigurationLoader(IFile file, IJsonTokenConverter tokenConverter, string[] filePaths, IEncoding encoding = null, Func<JsonSerializerSettings> jsonSerializerSettingsProvider = null, Func<JToken[], IJsonTokenConverter, IConfigurationProvider<JToken, JToken>> jsonConfigurationProviderFactory = null)
		{
			if (filePaths == null)
				throw new ArgumentNullException(nameof(filePaths));
			if (filePaths.Any(path => String.IsNullOrWhiteSpace(path)))
				throw new ArgumentException("At least one of the configuration file path is empty.", nameof(filePaths));

			_file = file ?? throw new ArgumentNullException(nameof(file));
			_filePaths = filePaths;
			_tokenConverter = tokenConverter ?? throw new ArgumentNullException(nameof(tokenConverter));
			_jsonSerializerSettingsProvider = jsonSerializerSettingsProvider;
			_encoding = encoding ?? new UTF8Encoding(false, true).ToInterface();
			_jsonConfigurationProviderFactory = jsonConfigurationProviderFactory ?? ((tokens, converter) => new JsonFileConfigurationProvider(tokens, converter));
		}

		/// <inheritdoc />
		public IConfigurationProvider<JToken, JToken> Load()
		{
			var tokens = GetTokens();
			return _jsonConfigurationProviderFactory(tokens, _tokenConverter);
		}

		private JToken[] GetTokens()
		{
			var serializer = (_jsonSerializerSettingsProvider == null) ? JsonSerializer.CreateDefault() : JsonSerializer.CreateDefault(_jsonSerializerSettingsProvider());
			var tokens = new JToken[_filePaths.Length];

			for (var i = 0; i < _filePaths.Length; i++)
			{
				using (var stream = _file.Open(_filePaths[i], FileMode.Open, FileAccess.Read, FileShare.Read))
				using (var textReader = new StreamReaderAdapter(stream, _encoding, true))
				using (var jsonReader = new JsonTextReader(textReader.ToImplementation()))
				{
					tokens[i] = serializer.Deserialize<JToken>(jsonReader);
				}
			}

			return tokens;
		}
	}
}
