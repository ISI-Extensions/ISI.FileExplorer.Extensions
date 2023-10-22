#region Copyright & License
/*
Copyright (c) 2018, Integrated Solutions, Inc.
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
using ISI.FileExplorer.Extensions.Shell.Extensions;

namespace ISI.FileExplorer.Extensions.Shell
{
	[System.Runtime.InteropServices.ComVisible(true)]
	[SharpShell.Attributes.DisplayName("ISI.FileExplorer.Extensions.Shell.JenkinsConfigDirectory")]
	[SharpShell.Attributes.COMServerAssociation(SharpShell.Attributes.AssociationType.Directory)]
	[SharpShell.Attributes.COMServerAssociation(SharpShell.Attributes.AssociationType.Class, @"Directory\Background")]
	[System.Runtime.InteropServices.Guid(ExtensionUuid)]
	public class JenkinsConfigDirectoryContextMenu : SharpShell.SharpContextMenu.SharpContextMenu
	{
		public const string ExtensionUuid = "15fe5d93-0219-4c48-96ee-f91eacb54bce";

		protected override bool CanShowMenu() => true;

		protected override System.Windows.Forms.ContextMenuStrip CreateMenu()
		{
			var menuStrip = new System.Windows.Forms.ContextMenuStrip();

			var menu = new System.Windows.Forms.ToolStripMenuItem("Jenkins")
			{
				Name = "ISI.FileExplorer.Extensions.Shell.JenkinsConfigDirectoryContextMenu",
				Image = System.Drawing.Image.FromStream(T4Resources.Artwork.GetjenkinsConfig_16x16_pngStream()),
			};

			{
				var menuItem = new System.Windows.Forms.ToolStripMenuItem()
				{
					Name = "ISI.FileExplorer.Extensions.Shell.JenkinsConfigDirectoryContextMenu.PullJenkinsConfigFromJenkins",
					Text = "Pull From Jenkins",
				};

				menuItem.Click += (sender, args) => PullJenkinsConfigFromJenkinsCommand(this.GetSelectedItemPaths());

				menu.DropDownItems.Add(menuItem);
			}

			menuStrip.Items.Add(menu);

			return menuStrip;
		}

		protected void PullJenkinsConfigFromJenkinsCommand(IEnumerable<string> selectedItemPaths)
		{
			var arguments = new ISI.FileExplorer.Extensions.Shell.CommandLineArguments(ISI.FileExplorer.Extensions.Shell.Jenkins.PullJenkinsConfigFromJenkinsCommandUuid);
			arguments.AddParameter(ISI.FileExplorer.Extensions.Shell.Jenkins.ParameterName_SelectedItemPaths, selectedItemPaths);

			ISI.FileExplorer.Extensions.Shell.Runner.Execute(arguments);
		}
	}
}
