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
using ISI.FileExplorer.Extensions.Shell.Extensions;

namespace ISI.FileExplorer.Extensions.Shell
{
	[System.Runtime.InteropServices.ComVisible(true)]
	[SharpShell.Attributes.DisplayName("ISI.FileExplorer.Extensions.Shell.AntBuildFile")]
	[SharpShell.Attributes.COMServerAssociation(SharpShell.Attributes.AssociationType.ClassOfExtension, ISI.FileExplorer.Extensions.Shell.Ant.AntFileNameExtension)]
	[SharpShell.Attributes.COMServerAssociation(SharpShell.Attributes.AssociationType.ClassOfExtension, ".build.xml")]
	[System.Runtime.InteropServices.Guid(ExtensionUuid)]
	public class AntBuildFileContextMenu : SharpShell.SharpContextMenu.SharpContextMenu
	{
		public const string ExtensionUuid = "295cc8b0-0e0e-4e0a-8df7-47adf2a7413f";

		protected override bool CanShowMenu()
		{
			var selectedItemPaths = this.GetSelectedItemPaths().ToArray();

			ISI.FileExplorer.Extensions.Shell.Logger.AddToLog("AntBuildFile", "GetSelectedItemPaths()", selectedItemPaths);

			if (selectedItemPaths.Length != 1)
			{
				return false;
			}

			var isBuildFile = ISI.FileExplorer.Extensions.Shell.Ant.IsBuildScriptFile(selectedItemPaths.First());

			ISI.FileExplorer.Extensions.Shell.Logger.AddToLog("AntBuildFile", "IsBuildFile", string.Format("isBuildFile = {0}", (isBuildFile ? "true" : "false")));

			return isBuildFile;
		}

		protected override System.Windows.Forms.ContextMenuStrip CreateMenu()
		{
			var menuStrip = new System.Windows.Forms.ContextMenuStrip();

			try
			{
				var menu = new System.Windows.Forms.ToolStripMenuItem("AntBuild")
				{
					Name = "ISI.FileExplorer.Extensions.Shell.AntBuildFileContextMenu",
					Image = System.Drawing.Image.FromStream(T4Resources.Artwork.GetAnt_16x16_pngStream()),
					Visible = true,
				};

				menuStrip.Items.Add(menu);

				var buildFileName = this.GetSelectedItemPaths().First();

				{
					var menuItem = new System.Windows.Forms.ToolStripMenuItem()
					{
						Name = "ISI.FileExplorer.Extensions.Shell.AntBuildFileContextMenu.ExecuteDefaultAntTarget",
						Text = "Execute Default Ant Target",
						Visible = true,
					};

					menuItem.Click += (sender, args) => ExecuteTargetCommand(buildFileName);

					menu.DropDownItems.Add(menuItem);
				}

				{
					var activeTargetKeys = ISI.FileExplorer.Extensions.Shell.Ant.GetTargetKeysFromBuildScript(buildFileName);
					if (activeTargetKeys.Any())
					{
						var menuItem = new System.Windows.Forms.ToolStripMenuItem()
						{
							Name = "ISI.FileExplorer.Extensions.Shell.AntBuildFileContextMenu.ExecuteTarget",
							Text = "Execute Target",
							Visible = true,
						};

						menu.DropDownItems.Add(menuItem);

						foreach (var activeTargetKey in activeTargetKeys)
						{
							var targetMenuItem = new System.Windows.Forms.ToolStripMenuItem()
							{
								Name = string.Format("ISI.FileExplorer.Extensions.Shell.AntBuildFileContextMenu.ExecuteTarget.{0}", activeTargetKey),
								Text = activeTargetKey,
								Visible = true,
							};

							targetMenuItem.Click += (sender, args) => ExecuteTargetCommand(buildFileName, activeTargetKey);

							menuItem.DropDownItems.Add(targetMenuItem);
						}
					}
				}
			}
			catch (Exception exception)
			{
				ISI.FileExplorer.Extensions.Shell.Logger.AddToLog("AntBuildFile", "CreateMenu()", exception.ErrorMessageFormatted());

				throw;
			}

			return menuStrip;
		}

		protected void ExecuteTargetCommand(string buildFileName, string activeTargetKey = null)
		{
			var arguments = new ISI.FileExplorer.Extensions.Shell.CommandLineArguments(ISI.FileExplorer.Extensions.Shell.Ant.ExecuteTargetCommandUuid);
			arguments.AddParameter(ISI.FileExplorer.Extensions.Shell.Ant.ParameterName_BuildFileName, buildFileName);
			if (!string.IsNullOrWhiteSpace(activeTargetKey))
			{
				arguments.AddParameter(ISI.FileExplorer.Extensions.Shell.Ant.ParameterName_Target, activeTargetKey);
			}

			ISI.FileExplorer.Extensions.Shell.Runner.Execute(arguments);
		}
	}
}
