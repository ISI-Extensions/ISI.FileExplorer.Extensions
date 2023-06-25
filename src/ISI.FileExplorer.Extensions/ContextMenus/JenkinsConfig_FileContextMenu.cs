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

namespace ISI.FileExplorer.Extensions
{
	[System.Runtime.InteropServices.ComVisible(true)]
	[SharpShell.Attributes.DisplayName("ISI.FileExplorer.Extensions.JenkinsConfigFile")]
	[SharpShell.Attributes.COMServerAssociation(SharpShell.Attributes.AssociationType.ClassOfExtension, ISI.Extensions.Jenkins.JenkinsApi.JenkinsConfigFileNameExtension)]
	[System.Runtime.InteropServices.Guid(ExtensionUuid)]
	public class JenkinsConfig_FileContextMenu : SharpShell.SharpContextMenu.SharpContextMenu
	{
		public const string ExtensionUuid = "1de91a16-0454-4278-999e-7b78b8899571";

		protected Microsoft.Extensions.Logging.ILogger Logger { get; }
		protected ISI.Extensions.Jenkins.JenkinsApi JenkinsApi { get; }

		public JenkinsConfig_FileContextMenu()
		{
			ServiceProvider.Initialize();

			Logger = ISI.Extensions.ServiceLocator.Current.GetService<Microsoft.Extensions.Logging.ILogger>();
			JenkinsApi = ISI.Extensions.ServiceLocator.Current.GetService<ISI.Extensions.Jenkins.JenkinsApi>();
		}

		protected override bool CanShowMenu()
		{
			var selectedItemPaths = this.GetSelectedItemPaths().ToArray();
			
			Logger.LogInformation(string.Format("JenkinsConfig_FileContextMenu.CanShowMenu() selectedItemPaths: {0}", string.Join(", ", selectedItemPaths.Select(selectedItemPath => string.Format("\"{0}\"", selectedItemPath)))));

			return selectedItemPaths.Any(selectedItemPath =>  JenkinsApi.IsJenkinsConfigFile(new () { FileName = selectedItemPath })?.IsJenkinsConfigFile ?? false);
		}

		protected override System.Windows.Forms.ContextMenuStrip CreateMenu()
		{
			var menuStrip = new System.Windows.Forms.ContextMenuStrip();

			var menu = new System.Windows.Forms.ToolStripMenuItem("Jenkins")
			{
				Name = "ISI.FileExplorer.Extensions.JenkinsConfig_FileContextMenu",
				Image = System.Drawing.Image.FromStream(ISI.Extensions.Jenkins.T4Resources.Artwork.GetjenkinsConfig_16x16_pngStream()),
			};

			{
				var menuItem = new System.Windows.Forms.ToolStripMenuItem()
				{
					Name = "ISI.FileExplorer.Extensions.JenkinsConfig_FileContextMenu.PushJenkinsConfigToJenkins",
					Text = "Push JenkinsConfig To Jenkins",
				};

				menuItem.Click += (sender, args) =>
				{
					(new ISI.FileExplorer.Extensions.ExecuteCommands.PushJenkinsConfigToJenkins_ExecuteCommand()).Execute(Guid.Parse(ExtensionUuid), this.GetSelectedItemPaths());
				};

				menu.DropDownItems.Add(menuItem);
			}

			{
				var menuItem = new System.Windows.Forms.ToolStripMenuItem()
				{
					Name = "ISI.FileExplorer.Extensions.JenkinsConfig_FileContextMenu.PullJenkinsConfigFromJenkins",
					Text = "Pull JenkinsConfig From Jenkins",
				};

				menuItem.Click += (sender, args) =>
				{
					(new ISI.FileExplorer.Extensions.ExecuteCommands.PullJenkinsConfigFromJenkins_ExecuteCommand()).Execute(Guid.Parse(ExtensionUuid), this.GetSelectedItemPaths());
				};

				menu.DropDownItems.Add(menuItem);
			}

			menuStrip.Items.Add(menu);

			return menuStrip;
		}

	}
}
