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
 
namespace ISI.FileExplorer.Extensions
{
	partial class NewVersionForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewVersionForm));
			this.tlpForm = new System.Windows.Forms.TableLayoutPanel();
			this.lblCaption = new System.Windows.Forms.Label();
			this.btnDownloadNewVersion = new System.Windows.Forms.Button();
			this.btnSnooze = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.tlpForm.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tlpForm.ColumnCount = 4;
			this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
			this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
			this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
			this.tlpForm.Controls.Add(this.lblCaption, 0, 0);
			this.tlpForm.Controls.Add(this.btnDownloadNewVersion, 2, 1);
			this.tlpForm.Controls.Add(this.btnSnooze, 3, 1);
			this.tlpForm.Controls.Add(this.btnClose, 3, 1);
			this.tlpForm.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpForm.Location = new System.Drawing.Point(0, 0);
			this.tlpForm.Name = "tlpForm";
			this.tlpForm.RowCount = 2;
			this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
			this.tlpForm.Size = new System.Drawing.Size(406, 68);
			this.tlpForm.TabIndex = 0;
			// 
			// label1
			// 
			this.lblCaption.AutoSize = true;
			this.tlpForm.SetColumnSpan(this.lblCaption, 3);
			this.lblCaption.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblCaption.Location = new System.Drawing.Point(3, 0);
			this.lblCaption.Name = "lblCaption";
			this.lblCaption.Size = new System.Drawing.Size(315, 37);
			this.lblCaption.TabIndex = 0;
			this.lblCaption.Text = "New version of ISI.FileExplorer.Extensions available";
			this.lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnDownloadNewVersion
			// 
			this.btnDownloadNewVersion.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnDownloadNewVersion.Location = new System.Drawing.Point(239, 40);
			this.btnDownloadNewVersion.Name = "btnDownloadNewVersion";
			this.btnDownloadNewVersion.Size = new System.Drawing.Size(79, 25);
			this.btnDownloadNewVersion.TabIndex = 2;
			this.btnDownloadNewVersion.Text = "Download";
			this.btnDownloadNewVersion.UseVisualStyleBackColor = true;
			// 
			// btnSnooze
			// 
			this.btnSnooze.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnSnooze.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnSnooze.Location = new System.Drawing.Point(324, 40);
			this.btnSnooze.Name = "btnSnooze";
			this.btnSnooze.Size = new System.Drawing.Size(79, 25);
			this.btnSnooze.TabIndex = 3;
			this.btnSnooze.Text = "Snooze";
			this.btnSnooze.UseVisualStyleBackColor = true;
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnClose.Location = new System.Drawing.Point(324, 40);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(79, 25);
			this.btnClose.TabIndex = 3;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// NewVersionForm
			// 
			this.AcceptButton = this.btnDownloadNewVersion;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnSnooze;
			this.ClientSize = new System.Drawing.Size(406, 68);
			this.ControlBox = false;
			this.Controls.Add(this.tlpForm);
			this.Name = "NewVersionForm";
			this.Text = "New Version Checker";
			this.tlpForm.ResumeLayout(false);
			this.tlpForm.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tlpForm;
		private System.Windows.Forms.Label lblCaption;
		private System.Windows.Forms.Button btnDownloadNewVersion;
		private System.Windows.Forms.Button btnSnooze;
		private System.Windows.Forms.Button btnClose;
	}
}