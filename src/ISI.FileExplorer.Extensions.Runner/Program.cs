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
			var arguments = new ISI.Extensions.CommandLineArguments(Environment.GetCommandLineArgs(), 1);

			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

			if (string.Equals(arguments.Command, "versionChecker", StringComparison.InvariantCultureIgnoreCase))
			{
				ISI.FileExplorer.Extensions.ExecuteCommands.VersionChecker_ExecuteCommand.Current.CheckForUpdate(true);
			}
			else
			{
				ISI.FileExplorer.Extensions.ServiceProvider.Initialize();

				var commands = ISI.Extensions.TypeLocator.Container.LocalContainer.GetImplementations<IExecuteCommand>(ISI.Extensions.ServiceLocator.Current);

				var command = commands.FirstOrDefault(cmd => cmd.Handles(arguments));

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