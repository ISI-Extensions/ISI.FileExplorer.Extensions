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

namespace ISI.FileExplorer.Extensions.Runner
{
	public class VersionChecker
	{
		private const string VersionUrl = @"https://www.isi-net.com/file-store/download/4a1700b7-2dc9-42d1-989d-76357ff7615b/ISI.FileExplorer.Extensions.Current.Version.txt";
		private const string InstallerUrl = @"https://www.isi-net.com/file-store/download/6cee4d35-449c-409a-8b46-c0c3ed066323/ISI.FileExplorer.Extensions.msi";

		private static VersionChecker _current = null;
		public static VersionChecker Current => _current ??= new VersionChecker();

		protected Microsoft.Extensions.Logging.ILogger Logger { get; }

		protected FileExplorerSettings FileExplorerSettings { get; }

		public VersionChecker()
		{
			ServiceProvider.Initialize();

			Logger = ISI.Extensions.ServiceLocator.Current.GetService<Microsoft.Extensions.Logging.ILogger>();

			FileExplorerSettings = new FileExplorerSettings();
		}

		private DateTime? LastCheckedDateTimeUtc = null;

		public bool CheckForUpdate(bool forceCheck = false)
		{
			var updateStarted = false;
#if DEBUG
			return true;
#endif

			LastCheckedDateTimeUtc = FileExplorerSettings.GetLastCheckedDateTimeUtc() ?? DateTime.MinValue;

			if (forceCheck || (DateTime.UtcNow - LastCheckedDateTimeUtc.Value > TimeSpan.FromDays(1)))
			{
				using (var statusForm = new ISI.FileExplorer.Extensions.Runner.StatusForm("Checking", () =>
				{
					var currentVersion = ISI.Extensions.SystemInformation.GetAssemblyVersion(this.GetType().Assembly);
					var newVersion = string.Empty;

					try
					{
						var webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(VersionUrl);

						webRequest.Method = System.Net.WebRequestMethods.Http.Get;

						using (var response = webRequest.GetResponse())
						{
							using (var responseStream = response.GetResponseStream())
							{
								using (var streamReader = new System.IO.StreamReader(responseStream))
								{
									newVersion = (new ISI.Extensions.Scm.DateTimeStampVersion(streamReader.ReadToEnd())).Version.ToString();
								}
							}
						}

						var newVersionFound = !string.Equals(currentVersion, newVersion, StringComparison.InvariantCultureIgnoreCase);

						if (forceCheck || newVersionFound)
						{
							using (var newVersionForm = new ISI.FileExplorer.Extensions.Runner.NewVersionForm(forceCheck, newVersionFound, currentVersion, newVersion))
							{
								if (newVersionForm.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
								{
									System.Diagnostics.Process.Start(InstallerUrl);

									updateStarted = true;
								}
							}
						}
					}
					catch (Exception exception)
					{

					}

					LastCheckedDateTimeUtc = DateTime.UtcNow;

					FileExplorerSettings.SetLastCheckedDateTimeUtc(LastCheckedDateTimeUtc);
				}))
				{
					statusForm.Text = "Checking for updates";
					statusForm.ShowDialog();
				}
			}

			return !updateStarted;
		}
	}
}
