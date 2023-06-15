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
	[SharpShell.Attributes.DisplayName("ISI.FileExplorer.Extensions.AssemblyNameBindingRedirect")]
	[SharpShell.Attributes.COMServerAssociation(SharpShell.Attributes.AssociationType.ClassOfExtension, ".dll")]
	[System.Runtime.InteropServices.Guid(ExtensionUuid)]
	public class AssemblyNameBindingRedirect_FileContextMenu : SharpShell.SharpContextMenu.SharpContextMenu
	{
		public const string ExtensionUuid = "3dbe0e37-cccc-4518-81f0-c6f66f21d03a";

		protected Microsoft.Extensions.Logging.ILogger Logger { get; }

		public AssemblyNameBindingRedirect_FileContextMenu()
		{
			ServiceProvider.Initialize();

			Logger = ISI.Extensions.ServiceLocator.Current.GetService<Microsoft.Extensions.Logging.ILogger>();
		}

		protected override bool CanShowMenu()
		{
			var selectedItemPaths = this.GetSelectedItemPaths().ToArray();

			if (selectedItemPaths.Length != 1)
			{
				return false;
			}

			return (selectedItemPaths.First().EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase));
		}

		protected override System.Windows.Forms.ContextMenuStrip CreateMenu()
		{
			var menuStrip = new System.Windows.Forms.ContextMenuStrip();

			try
			{
				var menu = new System.Windows.Forms.ToolStripMenuItem("Get AssemblyName BindingRedirect")
				{
					Name = "ISI.FileExplorer.Extensions.AssemblyNameBindingRedirect_FileContextMenu",
					Image = System.Drawing.Image.FromStream(ISI.FileExplorer.Extensions.T4Resources.Artwork.GetAssemblyNameBindingRedirect_icoStream()),
					Visible = true,
				};

				menu.Click += (sender, args) => { 
					var assemblyFileName = this.GetSelectedItemPaths().First();

					var assemblyName = System.Reflection.AssemblyName.GetAssemblyName(assemblyFileName);

					var name = assemblyName.FullName.Split(new[] { ',' }).First().Trim();
					var assemblyVersion = assemblyName.Version.ToString();
					var publicKeyToken = string.Concat(assemblyName.GetPublicKeyToken().Select(b => b.ToString("X2"))).ToLower();

					var response = new StringBuilder();

					response.AppendLine("<dependentAssembly>");
					response.AppendLine($"\t<assemblyIdentity name=\"{name}\" publicKeyToken=\"{publicKeyToken}\" />");
					response.AppendLine($"\t<bindingRedirect oldVersion=\"0.0.0.0-{assemblyVersion}\" newVersion=\"{assemblyVersion}\" />");
					response.AppendLine("</dependentAssembly>");

					System.Windows.Forms.Clipboard.SetText(response.ToString());
				};

				menuStrip.Items.Add(menu);

			}
			catch (Exception exception)
			{
				Logger.LogError(exception, "AssemblyNameBindingRedirect_FileContextMenu.CreateMenu()");

				throw;
			}

			return menuStrip;
		}
	}
}
