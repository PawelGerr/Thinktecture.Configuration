using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.Helpers
{
	public class TestConfiguration<T1>
	{
		public T1 P1 { get; set; }
	}

	public class TestConfiguration<T1, T2> : TestConfiguration<T1>
	{
		public T2 P2 { get; set; }
	}
}