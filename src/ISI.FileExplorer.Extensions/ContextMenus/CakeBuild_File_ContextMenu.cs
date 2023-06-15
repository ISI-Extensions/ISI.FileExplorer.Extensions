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
	[SharpShell.Attributes.DisplayName("ISI.FileExplorer.Extensions.CakeBuild")]
	[SharpShell.Attributes.COMServerAssociation(SharpShell.Attributes.AssociationType.ClassOfExtension, ".cake")]
	[System.Runtime.InteropServices.Guid(ExtensionUuid)]
	public class CakeBuild_File_ContextMenu : SharpShell.SharpContextMenu.SharpContextMenu
	{
		public const string ExtensionUuid = "a00e3aa4-ed9c-4486-9c7a-fafc815de0d0";

		protected Microsoft.Extensions.Logging.ILogger Logger { get; }
		protected ISI.Extensions.Cake.CakeApi CakeApi { get; }

		public CakeBuild_File_ContextMenu()
		{
			ServiceProvider.Initialize();

			Logger = ISI.Extensions.ServiceLocator.Current.GetService<Microsoft.Extensions.Logging.ILogger>();
			CakeApi = ISI.Extensions.ServiceLocator.Current.GetService<ISI.Extensions.Cake.CakeApi>();
		}

		protected override bool CanShowMenu()
		{
			var selectedItemPaths = this.GetSelectedItemPaths().ToArray();

			if (selectedItemPaths.Length != 1)
			{
				return false;
			}

			return CakeApi.IsBuildScriptFile(new ()
			{
				BuildScriptFullName = selectedItemPaths.First(),
			})?.IsBuildFile ?? false;
		}

		protected override System.Windows.Forms.ContextMenuStrip CreateMenu()
		{
			var menuStrip = new System.Windows.Forms.ContextMenuStrip();

			try
			{
				var menu = new System.Windows.Forms.ToolStripMenuItem("CakeBuild")
				{
					Name = "ISI.FileExplorer.Extensions.CakeBuild_File_ContextMenu",
					Image = System.Drawing.Image.FromStream(ISI.Extensions.Cake.T4Resources.Artwork.GetCake_16x16_pngStream()),
					Visible = true,
				};

				menuStrip.Items.Add(menu);

				var buildFileName = this.GetSelectedItemPaths().First();

				{
					var menuItem = new System.Windows.Forms.ToolStripMenuItem()
					{
						Name = "ISI.FileExplorer.Extensions.CakeBuild_File_ContextMenu.ExecuteDefaultCakeTarget",
						Text = "Execute Default Cake Target",
						Visible = true,
					};

					menuItem.Click += (sender, args) => { (new ISI.FileExplorer.Extensions.ExecuteCommands.Cake_ExecuteTarget_ExecuteCommand()).Execute(Guid.Parse(ExtensionUuid), buildFileName); };

					menu.DropDownItems.Add(menuItem);
				}

				{
					var activeTargetKeys = CakeApi.GetTargetKeysFromBuildScript(new ISI.Extensions.Cake.DataTransferObjects.CakeApi.GetTargetKeysFromBuildScriptRequest()
					{
						BuildScriptFullName = buildFileName,
					}).Targets;

					if (activeTargetKeys.NullCheckedAny())
					{
						var menuItem = new System.Windows.Forms.ToolStripMenuItem()
						{
							Name = "ISI.FileExplorer.Extensions.CakeBuild_File_ContextMenu.ExecuteTarget",
							Text = "Execute Target",
							Visible = true,
						};

						menu.DropDownItems.Add(menuItem);

						foreach (var activeTargetKey in activeTargetKeys)
						{
							var targetMenuItem = new System.Windows.Forms.ToolStripMenuItem()
							{
								Name = string.Format("ISI.FileExplorer.Extensions.CakeBuild_File_ContextMenu.ExecuteTarget.{0}", activeTargetKey),
								Text = activeTargetKey,
								Visible = true,
							};

							targetMenuItem.Click += (sender, args) =>
							{
								(new ISI.FileExplorer.Extensions.ExecuteCommands.Cake_ExecuteTarget_ExecuteCommand()).Execute(Guid.Parse(ExtensionUuid), buildFileName, activeTargetKey);
							};

							menuItem.DropDownItems.Add(targetMenuItem);
						}
					}
				}
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, "CakeBuild_File_ContextMenu.CreateMenu()");

				throw;
			}

			return menuStrip;
		}
	}
}
