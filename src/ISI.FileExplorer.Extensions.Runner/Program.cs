#region Copyright & License
/*
Copyright (c) 2026, Integrated Solutions, Inc.
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
using System.Threading.Tasks;
using System.Windows.Forms;
using ISI.Extensions.Extensions;
using ISI.Extensions.TypeLocator.Extensions;

namespace ISI.FileExplorer.Extensions.Runner
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Console.WriteLine("ISI.FileExplorer.Extensions.Runner");

			var arguments = new ISI.Extensions.CommandLineArguments(Environment.GetCommandLineArgs(), 1);

#if DEBUG
			//arguments = new ISI.Extensions.CommandLineArguments("versionChecker");

			//arguments = new ISI.Extensions.CommandLineArguments(ISI.FileExplorer.Extensions.Shell.VisualStudioSolutions.RefreshSolutionsCommandUuid.Formatted(GuidExtensions.GuidFormat.WithHyphens));
			//arguments.AddParameter(ISI.FileExplorer.Extensions.Shell.VisualStudioSolutions.ParameterName_SelectedItemPaths, new[] { @"F:\ISI\Internal Projects\ISI.Cake.Addin" });

			//arguments = new ISI.Extensions.CommandLineArguments(ISI.FileExplorer.Extensions.Shell.Cake.ExecuteTargetCommandUuid.Formatted(GuidExtensions.GuidFormat.WithHyphens));
			//arguments.AddParameter(ISI.FileExplorer.Extensions.Shell.Cake.ParameterName_BuildFileName, new[] { @"F:\ISI\Internal Projects\ISI.FileExplorer.Extensions\src\build.cake" });
			//arguments.AddParameter(ISI.FileExplorer.Extensions.Shell.Cake.ParameterName_Target, new[] { @"F:\ISI\Internal Projects\ISI.Cake.Addin" });
#endif

			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

			if (string.Equals(arguments.Command, "install", StringComparison.InvariantCultureIgnoreCase))
			{
				ISI.FileExplorer.Extensions.Shell.Logger.AddToLog("FileExplorer", "Install");

				Console.WriteLine("install");

				var shellFullName = System.IO.Path.Combine(ISI.Extensions.IO.Path.GetBinDirectory(), "ISI.FileExplorer.Extensions.Shell.dll");

				var registrationType = Environment.Is64BitOperatingSystem ? SharpShell.ServerRegistration.RegistrationType.OS64Bit : SharpShell.ServerRegistration.RegistrationType.OS32Bit;

				var regasm = new SharpShell.Helpers.RegAsm();

				var success = registrationType == SharpShell.ServerRegistration.RegistrationType.OS32Bit ? regasm.Register32(shellFullName, true) : regasm.Register64(shellFullName, true);

				Console.WriteLine(success ? "Installed" : "Failed");

				ISI.FileExplorer.Extensions.Shell.Logger.AddToLog("FileExplorer", $"Installed ({(success ? "Installed" : "Failed")})");
			}
			else if (string.Equals(arguments.Command, "uninstall", StringComparison.InvariantCultureIgnoreCase))
			{
				ISI.FileExplorer.Extensions.Shell.Logger.AddToLog("FileExplorer", "Uninstall");

				Console.WriteLine("uninstall");

				var shellFullName = System.IO.Path.Combine(ISI.Extensions.IO.Path.GetBinDirectory(), "ISI.FileExplorer.Extensions.Shell.dll");

				var registrationType = Environment.Is64BitOperatingSystem ? SharpShell.ServerRegistration.RegistrationType.OS64Bit : SharpShell.ServerRegistration.RegistrationType.OS32Bit;

				var regasm = new SharpShell.Helpers.RegAsm();

				var success = registrationType == SharpShell.ServerRegistration.RegistrationType.OS32Bit ? regasm.Unregister32(shellFullName) : regasm.Unregister32(shellFullName);

				Console.WriteLine(success ? "Uninstalled" : "Failed");

				ISI.FileExplorer.Extensions.Shell.Logger.AddToLog("FileExplorer", $"Uninstalled ({(success ? "Installed" : "Failed")})");
			}
			else if (string.Equals(arguments.Command, "versionChecker", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.WriteLine("versionChecker");

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				ISI.FileExplorer.Extensions.Runner.VersionChecker.Current.CheckForUpdate(true);
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				//if (ISI.FileExplorer.Extensions.Runner.VersionChecker.Current.CheckForUpdate())
				//{
				ISI.FileExplorer.Extensions.Runner.ServiceProvider.Initialize();

				var commands = ISI.Extensions.TypeLocator.Container.LocalContainer.GetImplementations<ISI.FileExplorer.Extensions.Runner.IExecuteCommand>(ISI.Extensions.ServiceLocator.Current);

				var commandUuid = arguments.Command.ToGuid();

				var command = commands.FirstOrDefault(cmd => cmd.Handles(commandUuid));

				if (command != null)
				{
					var form = command.Execute(arguments);

					if (form != null)
					{
						Application.Run(form);
					}
				}
				//}
			}
		}

		private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
		{
			try
			{
				var exception = unhandledExceptionEventArgs.ExceptionObject as Exception ?? new Exception($"An unhandled exception occurred in this application: {unhandledExceptionEventArgs.ExceptionObject}");

				var directory = @"C:\ProgramData\ISI.FileExplorer.Extensions";

				System.IO.Directory.CreateDirectory(directory);

				System.IO.File.WriteAllText(System.IO.Path.Combine(directory, $"DomainError.{DateTime.Now.Formatted(DateTimeExtensions.DateTimeFormat.DateTimeSortable)}.{Guid.NewGuid():N}.txt"), exception.ErrorMessageFormatted());
			}
			catch
			{
				// do not terminate any thread
			}
		}
	}
}