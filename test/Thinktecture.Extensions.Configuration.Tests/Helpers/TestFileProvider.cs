using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Thinktecture.Helpers
{
	public class TestFileProvider : IFileProvider
	{
		private readonly string _json;

		public TestFileProvider(object obj)
			: this(JsonConvert.SerializeObject(obj))
		{
		}

		public TestFileProvider(string json)
		{
			_json = json ?? throw new ArgumentNullException(nameof(json));
		}

		public IFileInfo GetFileInfo(string subpath)
		{
			return new TestFileInfo(_json);
		}

		public IDirectoryContents GetDirectoryContents(string subpath)
		{
			throw new NotImplementedException();
		}

		public IChangeToken Watch(string filter)
		{
			throw new NotImplementedException();
		}
	}
}
