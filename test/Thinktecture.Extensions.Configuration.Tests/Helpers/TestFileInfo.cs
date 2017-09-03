using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Thinktecture.Helpers
{
	public class TestFileInfo : IFileInfo
	{
		private readonly byte[] _jsonBytes;

		public bool Exists => true;
		public long Length => _jsonBytes.Length;
		public string PhysicalPath => "configuration.json";
		public string Name => "configuration.json";
		public DateTimeOffset LastModified => new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero);
		public bool IsDirectory => false;

		public TestFileInfo(string json)
		{
			if (json == null)
				throw new ArgumentNullException(nameof(json));

			_jsonBytes = new UTF8Encoding(false, true).GetBytes(json);
		}

		public Stream CreateReadStream()
		{
			var stream = new MemoryStream(_jsonBytes)
			{
				Position = 0
			};
			return stream;
		}
	}
}
