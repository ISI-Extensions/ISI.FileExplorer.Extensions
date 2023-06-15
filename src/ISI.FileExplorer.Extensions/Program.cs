#region Copyright & License
/*
Copyright (c) 2023, Integrated Solutions, Inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

		* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
		* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
		* Neither the name of the Integrated Solutions, Inc. nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISI.Extensions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ISI.FileExplorer.Extensions
{
	internal class Program
	{
		private static Microsoft.Extensions.Logging.ILogger _logger = null;
		protected static Microsoft.Extensions.Logging.ILogger Logger => _logger ?? ISI.Extensions.ServiceLocator.Current.GetService<Microsoft.Extensions.Logging.ILogger>();

		public static void ExecuteAsync(Guid extensionUuid, ISI.Extensions.CommandLineArguments arguments)
		{
			ServiceProvider.Initialize();

			Logger.LogInformation("looking for ISI.FileExplorer.Extensions.Runner.exe");

			var execFileName = string.Empty;

			try
			{
				using (var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(string.Format("CLSID\\{0:B}\\InprocServer32", extensionUuid), false))
				{
					var value = key?.GetValue("CodeBase");
					if (value != null)
					{
						execFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(value as string), "ISI.FileExplorer.Extensions.Runner.exe").Substring(6);
						//execFileName = value as string;
					}

					Logger.LogInformation(string.Format("ISI.FileExplorer.Extensions.Runner.exe found @ \"{0}\" via registry", execFileName));
				}
			}
			catch (Exception exception)
			{
				Logger.LogInformation(exception.ErrorMessageFormatted());
			}

			try
			{
				if (string.IsNullOrEmpty(execFileName))
				{
					execFileName = Type.GetType("ISI.FileExplorer.Extensions.Runner.T4Resources, ISI.FileExplorer.Extensions.Runner").Assembly.Location;

					Logger.LogInformation(string.Format("ISI.FileExplorer.Extensions.Runner.exe found @ \"{0}\" via Runner type", execFileName));
				}
			}
			catch (Exception exception)
			{
				Logger.LogInformation(exception.ErrorMessageFormatted());
			}

			if (string.IsNullOrEmpty(execFileName))
			{
				Logger.LogInformation("ISI.FileExplorer.Extensions.Runner.exe not found");
			}

			try
			{
				if (string.IsNullOrEmpty(execFileName))
				{
					execFileName = typeof(Program).Assembly.Location;

					execFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(execFileName), "ISI.FileExplorer.Extensions.Runner.exe");

					Logger.LogInformation(string.Format("ISI.FileExplorer.Extensions.Runner.exe found @ \"{0}\" via type", execFileName));
				}
			}
			catch (Exception exception)
			{
				Logger.LogInformation(exception.ErrorMessageFormatted());
			}

			if (string.IsNullOrEmpty(execFileName))
			{
				Logger.LogInformation("ISI.FileExplorer.Extensions.Runner.exe not found");
			}

			var processStartInfo = new System.Diagnostics.ProcessStartInfo(execFileName)
			{
				Arguments = arguments.ToArguments(),
				//CreateNoWindow = true,
				UseShellExecute = false,
				WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
			};

			System.Diagnostics.Process.Start(processStartInfo);
		}
	}
}
