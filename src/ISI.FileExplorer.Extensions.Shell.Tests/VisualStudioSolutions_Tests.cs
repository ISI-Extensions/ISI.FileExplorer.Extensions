using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ISI.FileExplorer.Extensions.Shell.Tests
{
	[TestFixture]
	public class VisualStudioSolutions_Tests
	{
		[Test]
		public void VisualStudioSolutions_Test()
		{
			var directory = @"F:\ISI\Clients\West River Systems\wrs.state.timeseries.service";

			var found = ISI.FileExplorer.Extensions.Shell.IO.CheckForExistence(directory, "*.sln*", ISI.FileExplorer.Extensions.Shell.VisualStudioSolutions.DefaultExcludePathFilters, ISI.FileExplorer.Extensions.Shell.VisualStudioSolutions.MaxCheckDirectoryDepth, IsSolutionFileName);
		}

		public static bool IsSolutionFileName(string fullName)
		{
			var fileNameExtension = System.IO.Path.GetExtension(fullName);

			return (string.Equals(fileNameExtension, ".sln", StringComparison.InvariantCultureIgnoreCase) || string.Equals(fileNameExtension, ".slnx", StringComparison.InvariantCultureIgnoreCase));
		}
	}
}
