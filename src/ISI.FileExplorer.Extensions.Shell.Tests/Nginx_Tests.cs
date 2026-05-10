using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ISI.FileExplorer.Extensions.Shell.Tests
{
	[TestFixture]
	public class Nginx_Tests
	{
		[Test]
		public void Nginx_Test()
		{
			var selectedItemPaths = new[]
			{
				@"F:\ISI\Internal Projects\ISI.Chocolately.Deploy\nginx\chocolately.isi-net.com.nginxConfig"
			};

			var arguments = new ISI.FileExplorer.Extensions.Shell.CommandLineArguments(ISI.FileExplorer.Extensions.Shell.Nginx.ProcessNginxConfigsCommandUuid);
			arguments.AddParameter(ISI.FileExplorer.Extensions.Shell.Nginx.ParameterName_SelectedItemPaths, selectedItemPaths);

			ISI.FileExplorer.Extensions.Shell.Runner.Execute(arguments);
		}
	}
}
