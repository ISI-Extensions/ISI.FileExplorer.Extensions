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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ISI.FileExplorer.Extensions.Runner
{
	public partial class NewVersionForm : Form
	{
		public NewVersionForm(bool forceCheck, bool newVersionFound)
		{
			InitializeComponent();

			ISI.Extensions.WinForms.ThemeHelper.SyncTheme(this);

			this.Icon = new Icon(ISI.FileExplorer.Extensions.Runner.T4Resources.Artwork.GetLantern_icoStream());
			this.ControlBox = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ShowIcon = true;

			this.btnDownloadNewVersion.Click += btnDownloadNewVersion_Click;
			this.btnDownloadNewVersion.Visible = newVersionFound;

			this.btnSnooze.Click += btnSnooze_Click;
			this.btnSnooze.Visible = !forceCheck;

			this.btnClose.Click += btnClose_Click;
			this.btnClose.Visible = forceCheck;

			this.lblCaption.Text = (newVersionFound ? "New version of ISI.FileExplorer.Extensions available" : "Current Version already installed");
		}

		private void btnDownloadNewVersion_Click(object sender, EventArgs e)
		{
			// cleanup 
			if (this.Modal)
			{
				this.DialogResult = System.Windows.Forms.DialogResult.Yes;
			}
			else
			{
				this.Close();
			}
		}

		private void btnSnooze_Click(object sender, EventArgs e)
		{
			// cleanup 
			if (this.Modal)
			{
				this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			}
			else
			{
				this.Close();
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			// cleanup 
			if (this.Modal)
			{
				this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			}
			else
			{
				this.Close();
			}
		}
	}
}
