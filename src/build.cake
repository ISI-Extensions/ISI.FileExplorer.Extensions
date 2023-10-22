#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=ISI.Cake.AddIn&loaddependencies=true

//mklink /D Secrets S:\
var settingsFullName = System.IO.Path.Combine(System.Environment.GetEnvironmentVariable("LocalAppData"), "Secrets", "ISI.keyValue");
var settings = GetSettings(settingsFullName);

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solutionFile = File("./ISI.FileExplorer.Extensions.sln");
var solution = ParseSolution(solutionFile);
var rootProjectFile = File("./ISI.FileExplorer.Extensions.Runner/ISI.FileExplorer.Extensions.Runner.csproj");
var rootAssemblyVersionKey = "ISI.FileExplorer.Extensions";
var artifactName = "ISI.FileExplorer.Extensions";
var setupBuildArtifactName = "ISI.FileExplorer.Extensions.Setup";

var buildDateTime = DateTime.UtcNow;
var buildDateTimeStamp = GetDateTimeStamp(buildDateTime);
var buildRevision = GetBuildRevision(buildDateTime);

var assemblyVersions = GetAssemblyVersionFiles(rootAssemblyVersionKey, buildRevision);
var assemblyVersion = assemblyVersions[rootAssemblyVersionKey].AssemblyVersion;

var buildDateTimeStampVersion = new ISI.Extensions.Scm.DateTimeStampVersion(buildDateTimeStamp, assemblyVersions[rootAssemblyVersionKey].AssemblyVersion);

Information("BuildDateTimeStampVersion: {0}", buildDateTimeStampVersion);

var buildArtifactMsiFile = File(string.Format("../Publish/{0}.{1}.msi", artifactName, buildDateTimeStamp));

Task("Clean")
	.Does(() =>
	{
		Information("Cleaning Projects ...");

		foreach(var projectPath in new HashSet<string>(solution.Projects.Select(p => p.Path.GetDirectory().ToString())))
		{
			Information("Cleaning {0}", projectPath);
			CleanDirectories(projectPath + "/**/bin/" + configuration);
			CleanDirectories(projectPath + "/**/obj/" + configuration);
		}
	});

Task("NugetPackageRestore")
	.IsDependentOn("Clean")
	.Does(() =>
	{
		Information("Restoring Nuget Packages ...");
		using(GetNugetLock())
		{
			NuGetRestore(solutionFile);
		}
	});

Task("Build")
	.IsDependentOn("NugetPackageRestore")
	.Does(() => 
	{
		SetAssemblyVersionFiles(assemblyVersions);

		var productWxsFile = File("./ISI.FileExplorer.Extensions.Setup/Package.wxs");
		var productWxsContent = FileReadText(productWxsFile);

		try
		{
			XmlPoke(productWxsFile, "//*[local-name() = 'Package']/@Version", assemblyVersions[rootAssemblyVersionKey].AssemblyVersion);

			var getSignAssemblyCommandResponse = GetSignAssemblyCommand(new ISI.Cake.Addin.CodeSigning.GetSignAssemblyCommandUsingSettingsRequest()
			{
				Settings = settings,
			});

			MSBuild(solutionFile, configurator => configurator
				.SetConfiguration(configuration)
				.WithProperty("Platform", "x64")
				.WithProperty("SignAssemblyCommand", getSignAssemblyCommandResponse.Command)
				.SetVerbosity(Verbosity.Quiet)
				.SetMSBuildPlatform(MSBuildPlatform.x64)
				.SetPlatformTarget(PlatformTarget.MSIL)
				.WithTarget("Rebuild"));
		}
		finally
		{
			FileWriteText(productWxsFile, productWxsContent);

			ResetAssemblyVersionFiles(assemblyVersions);
		}
	});

Task("Sign")
	.IsDependentOn("Build")
	.Does(() =>
	{
		if (configuration.Equals("Release"))
		{
			var files = GetFiles("./ISI.FileExplorer.Extensions.Setup/bin/x64/" + configuration + "/ISI.FileExplorer.Extensions.msi");

			if(files.Any())
			{
				Information("Signing assemblies");
				using(var tempDirectory = GetNewTempDirectory())
				{
					foreach(var file in files)
					{
						var tempFile = File(tempDirectory.FullName + "/" + file.GetFilename());

						if(System.IO.File.Exists(tempFile.Path.FullPath))
						{
							DeleteFile(tempFile);
						}

						CopyFile(file, tempFile);
					}

					var tempFiles = GetFiles(tempDirectory.FullName + "/*");

					SignAssemblies(new ISI.Cake.Addin.CodeSigning.SignAssembliesUsingSettingsRequest()
					{
						AssemblyPaths = tempFiles,
						Settings = settings,
					});

					foreach(var file in files)
					{
						var tempFile = File(tempDirectory.FullName + "/" + file.GetFilename());

						DeleteFile(file);

						CopyFile(tempFile, file);
					}
				}
			}
		}
	});

Task("Package")
	.IsDependentOn("Sign")
	.Does(() =>
	{
		CreateDirectory(buildArtifactMsiFile.Path.GetDirectory().FullPath);

		FileWriteText(File(string.Format("../Publish/{0}.Current.Version.txt", artifactName)), assemblyVersions[rootAssemblyVersionKey].AssemblyVersion);
		
		CopyFile(File("./ISI.FileExplorer.Extensions.Setup/bin/x64/" + configuration + "/ISI.FileExplorer.Extensions.msi"), buildArtifactMsiFile);
		CopyFile(File("./ISI.FileExplorer.Extensions.Setup/bin/x64/" + configuration + "/ISI.FileExplorer.Extensions.msi"), File(string.Format("../Publish/{0}.msi", artifactName)));

		DeleteAgedPackages(new ISI.Cake.Addin.PackageComponents.DeleteAgedPackagesRequest()
		{
			PackagesDirectory = buildArtifactMsiFile.Path.GetDirectory().FullPath,
			PackageName = artifactName,
			PackageNameExtension = "msi",
		});		
	});

Task("Publish")
	.IsDependentOn("Package")
	.Does(() =>
	{
		var buildArtifactsApiKey = GetBuildArtifactsApiKey(new ISI.Cake.Addin.BuildArtifacts.GetBuildArtifactsApiKeyUsingSettingsActiveDirectoryRequest()
		{
			Settings = settings,
		}).BuildArtifactsApiKey;

		UploadBuildArtifact(new ISI.Cake.Addin.BuildArtifacts.UploadBuildArtifactRequest()
		{
			BuildArtifactsApiUri = GetNullableUri(settings.BuildArtifacts.ApiUrl),
			BuildArtifactsApiKey = buildArtifactsApiKey,
			SourceFileName = buildArtifactMsiFile.Path.FullPath,
			BuildArtifactName = artifactName,
			DateTimeStampVersion = buildDateTimeStampVersion,
		});

		SetBuildArtifactEnvironmentDateTimeStampVersion(new ISI.Cake.Addin.BuildArtifacts.SetBuildArtifactEnvironmentDateTimeStampVersionRequest()
		{
			BuildArtifactsApiUri = GetNullableUri(settings.BuildArtifacts.ApiUrl),
			BuildArtifactsApiKey = buildArtifactsApiKey,
			BuildArtifactName = artifactName,
			Environment = "Build",
			DateTimeStampVersion = buildDateTimeStampVersion,
		});
	});
	
Task("Production-Deploy")
	.Does(() => 
	{
		var buildArtifactsApiKey = GetBuildArtifactsApiKey(new ISI.Cake.Addin.BuildArtifacts.GetBuildArtifactsApiKeyUsingSettingsActiveDirectoryRequest()
		{
			Settings = settings,
		}).BuildArtifactsApiKey;

		var dateTimeStampVersion = GetBuildArtifactEnvironmentDateTimeStampVersion(new ISI.Cake.Addin.BuildArtifacts.GetBuildArtifactEnvironmentDateTimeStampVersionRequest()
		{
			BuildArtifactsApiUri = GetNullableUri(settings.BuildArtifacts.ApiUrl),
			BuildArtifactsApiKey = buildArtifactsApiKey,
			BuildArtifactName = artifactName,
			Environment = "Build",
		}).DateTimeStampVersion;

		SetBuildArtifactEnvironmentDateTimeStampVersion(new ISI.Cake.Addin.BuildArtifacts.SetBuildArtifactEnvironmentDateTimeStampVersionRequest()
		{
			BuildArtifactsApiUri = GetNullableUri(settings.BuildArtifacts.ApiUrl),
			BuildArtifactsApiKey = buildArtifactsApiKey,
			BuildArtifactName = artifactName,
			Environment = "Production",
			DateTimeStampVersion = dateTimeStampVersion,
		});

		var getOrCreateBuildArtifactRedirectsResponse = GetOrCreateBuildArtifactRedirects(new ISI.Cake.Addin.BuildArtifacts.GetOrCreateBuildArtifactRedirectsRequest()
		{
			BuildArtifactsApiUri = GetNullableUri(settings.BuildArtifacts.ApiUrl),
			BuildArtifactsApiKey = buildArtifactsApiKey,
				
			BuildArtifactName = artifactName,
			Environment = "Production",
		});

		var artifactDateTimeStampVersionUrl = string.Format("https://www.isi-net.com/file-store/download/{0:D}/{1}.Current.DateTimeStamp.Version.txt", getOrCreateBuildArtifactRedirectsResponse.ArtifactVersionFileStoreUuid, artifactName); 
		var artifactDownloadUrl = string.Format("https://www.isi-net.com/file-store/download/{0:D}/{1}.msi", getOrCreateBuildArtifactRedirectsResponse.ArtifactFileStoreUuid, artifactName); 

		Information(string.Format("curl {0} --output {1}.Current.DateTimeStamp.Version.txt", artifactDateTimeStampVersionUrl, artifactName));
		Information(string.Format("curl {0} --output {1}.msi", artifactDownloadUrl, artifactName));
	});

Task("Default")
	.IsDependentOn("Publish")
	.Does(() => 
	{
		Information("No target provided. Starting default task");
	});

using(GetSolutionLock())
{
	RunTarget(target);
}
