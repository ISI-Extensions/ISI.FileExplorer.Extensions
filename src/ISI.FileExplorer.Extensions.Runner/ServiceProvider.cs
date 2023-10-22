﻿#region Copyright & License
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
using ISI.Extensions.ConfigurationHelper.Extensions;
using ISI.Extensions.DependencyInjection.Extensions;
using ISI.Extensions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ISI.FileExplorer.Extensions.Runner
{
	public class ServiceProvider
	{
		private static bool _isInitialized = false;

		public static void Initialize()
		{
			if (!_isInitialized)
			{
				var configurationBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
				var configuration = configurationBuilder.Build();

				var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection()
					.AddOptions()
					.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(configuration);

				services.AddAllConfigurations(configuration)

					//.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory, Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory>()
					.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory, Microsoft.Extensions.Logging.LoggerFactory>()
					.AddLogging(builder => builder
							.AddConsole()
						//.AddFilter(level => level >= Microsoft.Extensions.Logging.LogLevel.Information)
					)
					//.AddSingleton<Microsoft.Extensions.Logging.ILogger>(_ => new ISI.Extensions.ConsoleLogger())
					.AddSingleton<Microsoft.Extensions.Logging.ILogger>(_ => new ISI.Extensions.FileLogger(ISI.FileExplorer.Extensions.Shell.Logger.GetLogDirectory(), ISI.FileExplorer.Extensions.Shell.Logger.GetLogFileName()))

					.AddSingleton<ISI.Extensions.DateTimeStamper.IDateTimeStamper, ISI.Extensions.DateTimeStamper.LocalMachineDateTimeStamper>()

					.AddSingleton<ISI.Extensions.JsonSerialization.IJsonSerializer, ISI.Extensions.JsonSerialization.Newtonsoft.NewtonsoftJsonSerializer>()
					.AddSingleton<ISI.Extensions.Serialization.ISerialization, ISI.Extensions.Serialization.Serialization>()

					.AddSingleton<ISI.Extensions.StatusTrackers.FileStatusTrackerFactory>()
					.AddSingleton<ISI.Extensions.JsonSerialization.Newtonsoft.NewtonsoftJsonSerializer>()
					.AddSingleton<ISI.Extensions.Jenkins.JenkinsApi>()
					.AddSingleton<ISI.Extensions.Svn.SvnApi>()
					.AddSingleton<ISI.Extensions.Git.GitApi>()
					.AddSingleton<ISI.Extensions.Scm.SourceControlClientApi>()
					.AddSingleton<ISI.Extensions.Scm.BuildScriptApi>()
					.AddSingleton<ISI.Extensions.Nuget.NugetApi>()
					.AddSingleton<ISI.Extensions.VisualStudio.SolutionApi>()

					.AddConfigurationRegistrations(configuration)
					.ProcessServiceRegistrars()
					;

				var serviceProvider = services.BuildServiceProvider<ISI.Extensions.DependencyInjection.Iunq.ServiceProviderBuilder>(configuration);

				var serializationConfiguration = serviceProvider.GetService<ISI.Extensions.Serialization.Configuration>();
				serializationConfiguration.DefaultDataContractSerializerType = typeof(ISI.Extensions.JsonSerialization.Newtonsoft.NewtonsoftJsonSerializer).AssemblyQualifiedNameWithoutVersion();
				serializationConfiguration.DefaultSerializerType = typeof(ISI.Extensions.JsonSerialization.Newtonsoft.NewtonsoftJsonSerializer).AssemblyQualifiedNameWithoutVersion();

				serviceProvider.SetServiceLocator();

				_isInitialized = true;
			}
		}
	}
}
