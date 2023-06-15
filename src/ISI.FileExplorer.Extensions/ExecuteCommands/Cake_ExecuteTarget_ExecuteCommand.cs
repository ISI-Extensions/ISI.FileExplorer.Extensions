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
using ISI.FileExplorer.Extensions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ISI.FileExplorer.Extensions.ExecuteCommands
{
	[ExecuteCommand]
	public class Cake_ExecuteTarget_ExecuteCommand : IExecuteCommand
	{
		public const string Command = "Cake_Command";

		public const string ParameterName_BuildFileName = "BuildFileName";
		public const string ParameterName_Target = "Target";

		protected Microsoft.Extensions.Logging.ILogger Logger { get; }
		protected ISI.Extensions.Cake.CakeApi CakeApi { get; }

		public Cake_ExecuteTarget_ExecuteCommand()
		{
			ServiceProvider.Initialize();

			Logger = ISI.Extensions.ServiceLocator.Current.GetService<Microsoft.Extensions.Logging.ILogger>();
			CakeApi = ISI.Extensions.ServiceLocator.Current.GetService<ISI.Extensions.Cake.CakeApi>();
		}

		public bool Handles(ISI.Extensions.CommandLineArguments arguments)
		{
			return string.Equals(arguments.Command, Command, StringComparison.InvariantCultureIgnoreCase);
		}

		public void Execute(Guid extensionUuid, string buildFileName, string target = null)
		{
			VersionChecker_ExecuteCommand.Current.CheckForUpdate();

			var arguments = new ISI.Extensions.CommandLineArguments(Command);
			arguments.AddParameter(ParameterName_BuildFileName, buildFileName);
			arguments.AddParameter(ParameterName_Target, target ?? string.Empty);

			Program.ExecuteAsync(extensionUuid, arguments);
		}

		public System.Windows.Forms.Form Execute(ISI.Extensions.CommandLineArguments arguments)
		{
			if (arguments.TryGetParameterValue(ParameterName_BuildFileName, out var buildFileName) && arguments.TryGetParameterValue(ParameterName_Target, out var target))
			{
				System.Threading.Tasks.Task.Run(() =>
				{
					CakeApi.ExecuteBuildTarget(new ISI.Extensions.Cake.DataTransferObjects.CakeApi.ExecuteBuildTargetRequest()
					{
						BuildScriptFullName = buildFileName,
						Target = target,
						UseShell = true,
					});
				});
			}

			return null;
		}
	}
}