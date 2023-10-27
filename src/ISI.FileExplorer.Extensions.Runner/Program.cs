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

			arguments = new ISI.Extensions.CommandLineArguments(ISI.FileExplorer.Extensions.Shell.VisualStudioSolutions.RefreshSolutionsCommandUuid.Formatted(GuidExtensions.GuidFormat.WithHyphens));
			arguments.AddParameter(ISI.FileExplorer.Extensions.Shell.VisualStudioSolutions.ParameterName_SelectedItemPaths, new[] { @"F:\ISI\Internal Projects\ISI.Cake.Addin" });
#endif

			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

			if (string.Equals(arguments.Command, "install", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.WriteLine("install");

				var shellFullName = System.IO.Path.Combine(ISI.Extensions.IO.Path.GetBinDirectory(), "ISI.FileExplorer.Extensions.Shell.dll");

				var registrationType = Environment.Is64BitOperatingSystem ? SharpShell.ServerRegistration.RegistrationType.OS64Bit : SharpShell.ServerRegistration.RegistrationType.OS32Bit;

				var regasm = new SharpShell.Helpers.RegAsm();

				var success = registrationType == SharpShell.ServerRegistration.RegistrationType.OS32Bit ? regasm.Register32(shellFullName, true) : regasm.Register64(shellFullName, true);

				Console.WriteLine(success ? "Installed" : "Failed");
			}
			else if (string.Equals(arguments.Command, "uninstall", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.WriteLine("uninstall");

				var shellFullName = System.IO.Path.Combine(ISI.Extensions.IO.Path.GetBinDirectory(), "ISI.FileExplorer.Extensions.Shell.dll");

				var registrationType = Environment.Is64BitOperatingSystem ? SharpShell.ServerRegistration.RegistrationType.OS64Bit : SharpShell.ServerRegistration.RegistrationType.OS32Bit;

				var regasm = new SharpShell.Helpers.RegAsm();

				var success = registrationType == SharpShell.ServerRegistration.RegistrationType.OS32Bit ? regasm.Unregister32(shellFullName) : regasm.Unregister32(shellFullName);

				Console.WriteLine(success ? "Uninstalled" : "Failed");
			}
			else if (string.Equals(arguments.Command, "versionChecker", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.WriteLine("versionChecker");

				ISI.FileExplorer.Extensions.Runner.VersionChecker.Current.CheckForUpdate(true);
			}
			else
			{
				if (ISI.FileExplorer.Extensions.Runner.VersionChecker.Current.CheckForUpdate())
				{
					ISI.FileExplorer.Extensions.Runner.ServiceProvider.Initialize();

					var commands = ISI.Extensions.TypeLocator.Container.LocalContainer.GetImplementations<ISI.FileExplorer.Extensions.Runner.IExecuteCommand>(ISI.Extensions.ServiceLocator.Current);

					var commandUuid = arguments.Command.ToGuid();

					var command = commands.FirstOrDefault(cmd => cmd.Handles(commandUuid));

					if (command != null)
					{
						Application.EnableVisualStyles();
						Application.SetCompatibleTextRenderingDefault(false);

						var form = command.Execute(arguments);

						if (form != null)
						{
							Application.Run(form);
						}
					}
				}
			}
		}

		private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
		{
			try
			{
				var exception = unhandledExceptionEventArgs.ExceptionObject as Exception ?? new Exception(string.Format("An unhandled exception occurred in this application: {0}", unhandledExceptionEventArgs.ExceptionObject));

				var directory = @"C:\ProgramData\ISI.FileExplorer.Extensions";

				System.IO.Directory.CreateDirectory(directory);

				System.IO.File.WriteAllText(System.IO.Path.Combine(directory, string.Format("DomainError.{0}.{1:N}.txt", DateTime.Now.Formatted(DateTimeExtensions.DateTimeFormat.DateTimeSortable), Guid.NewGuid())), exception.ErrorMessageFormatted());
			}
			catch
			{
				// do not terminate any thread
			}
		}
	}
}