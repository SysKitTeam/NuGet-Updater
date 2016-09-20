using System;
using Microsoft.Win32;


namespace Acceleratio.Nuget.Updater
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label1 = new System.Windows.Forms.Label();
            this.rootPathTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.solutionsList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.repositoryTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.latestCheckBox = new System.Windows.Forms.CheckBox();
            this.updateButton = new System.Windows.Forms.Button();
            this.versionsDropDown = new System.Windows.Forms.ComboBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.uploadCheckBox = new System.Windows.Forms.CheckBox();
            this.deleteOldCheckBox = new System.Windows.Forms.CheckBox();
            this.versionLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.sourceControlComboBox = new System.Windows.Forms.ComboBox();
            this.packagesComboBoxList = new CheckComboBoxTest.CheckedComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Root directory:";
            // 
            // rootPathTextBox
            // 
            this.rootPathTextBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.rootPathTextBox.Location = new System.Drawing.Point(108, 41);
            this.rootPathTextBox.Name = "rootPathTextBox";
            this.rootPathTextBox.ReadOnly = true;
            this.rootPathTextBox.Size = new System.Drawing.Size(536, 23);
            this.rootPathTextBox.TabIndex = 1;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(650, 13);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(245, 51);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse for solution directory";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // solutionsList
            // 
            this.solutionsList.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.solutionsList.FormattingEnabled = true;
            this.solutionsList.ItemHeight = 15;
            this.solutionsList.Location = new System.Drawing.Point(15, 220);
            this.solutionsList.Name = "solutionsList";
            this.solutionsList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.solutionsList.Size = new System.Drawing.Size(629, 124);
            this.solutionsList.TabIndex = 4;
            this.solutionsList.SelectedIndexChanged += new System.EventHandler(this.solutionsList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "NuGet repository:";
            // 
            // repositoryTextBox
            // 
            this.repositoryTextBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.repositoryTextBox.Location = new System.Drawing.Point(108, 13);
            this.repositoryTextBox.Name = "repositoryTextBox";
            this.repositoryTextBox.Size = new System.Drawing.Size(447, 23);
            this.repositoryTextBox.TabIndex = 6;
            this.repositoryTextBox.TextChanged += new System.EventHandler(this.repositoryTextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Package(s):";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 204);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Solutions:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 356);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Status:";
            // 
            // statusTextBox
            // 
            this.statusTextBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.statusTextBox.Location = new System.Drawing.Point(15, 372);
            this.statusTextBox.Multiline = true;
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ReadOnly = true;
            this.statusTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.statusTextBox.Size = new System.Drawing.Size(629, 124);
            this.statusTextBox.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Package version:";
            // 
            // latestCheckBox
            // 
            this.latestCheckBox.AutoSize = true;
            this.latestCheckBox.Checked = true;
            this.latestCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.latestCheckBox.Enabled = false;
            this.latestCheckBox.Location = new System.Drawing.Point(650, 102);
            this.latestCheckBox.Name = "latestCheckBox";
            this.latestCheckBox.Size = new System.Drawing.Size(55, 17);
            this.latestCheckBox.TabIndex = 17;
            this.latestCheckBox.Text = "Latest";
            this.latestCheckBox.UseVisualStyleBackColor = true;
            this.latestCheckBox.CheckedChanged += new System.EventHandler(this.latestCheckBox_CheckedChanged);
            // 
            // updateButton
            // 
            this.updateButton.Enabled = false;
            this.updateButton.Location = new System.Drawing.Point(650, 220);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(245, 52);
            this.updateButton.TabIndex = 18;
            this.updateButton.Text = "Update selected solutions";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // versionsDropDown
            // 
            this.versionsDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.versionsDropDown.Enabled = false;
            this.versionsDropDown.FormattingEnabled = true;
            this.versionsDropDown.Location = new System.Drawing.Point(108, 100);
            this.versionsDropDown.Name = "versionsDropDown";
            this.versionsDropDown.Size = new System.Drawing.Size(536, 21);
            this.versionsDropDown.TabIndex = 19;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(561, 13);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(83, 23);
            this.connectButton.TabIndex = 21;
            this.connectButton.Text = "Load";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 163);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Advanced (TFS):";
            this.label7.Visible = false;
            // 
            // uploadCheckBox
            // 
            this.uploadCheckBox.AutoSize = true;
            this.uploadCheckBox.Checked = true;
            this.uploadCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uploadCheckBox.Location = new System.Drawing.Point(108, 162);
            this.uploadCheckBox.Name = "uploadCheckBox";
            this.uploadCheckBox.Size = new System.Drawing.Size(257, 17);
            this.uploadCheckBox.TabIndex = 23;
            this.uploadCheckBox.Text = "Upload files to packages folder on source control";
            this.uploadCheckBox.UseVisualStyleBackColor = true;
            this.uploadCheckBox.Visible = false;
            // 
            // deleteOldCheckBox
            // 
            this.deleteOldCheckBox.AutoSize = true;
            this.deleteOldCheckBox.Checked = true;
            this.deleteOldCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.deleteOldCheckBox.Location = new System.Drawing.Point(108, 185);
            this.deleteOldCheckBox.Name = "deleteOldCheckBox";
            this.deleteOldCheckBox.Size = new System.Drawing.Size(405, 17);
            this.deleteOldCheckBox.TabIndex = 24;
            this.deleteOldCheckBox.Text = "Delete older files from packages folder on source control (this could take a whil" +
    "e)";
            this.deleteOldCheckBox.UseVisualStyleBackColor = true;
            this.deleteOldCheckBox.Visible = false;
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(873, 483);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(22, 13);
            this.versionLabel.TabIndex = 25;
            this.versionLabel.Text = "1.2";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 130);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 26;
            this.label8.Text = "Source control:";
            // 
            // sourceControlComboBox
            // 
            this.sourceControlComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sourceControlComboBox.FormattingEnabled = true;
            this.sourceControlComboBox.Items.AddRange(new object[] {
            "Git",
            "Team Foundation Server"});
            this.sourceControlComboBox.Location = new System.Drawing.Point(108, 127);
            this.sourceControlComboBox.Name = "sourceControlComboBox";
            this.sourceControlComboBox.Size = new System.Drawing.Size(536, 21);
            this.sourceControlComboBox.TabIndex = 27;
            this.sourceControlComboBox.SelectedIndexChanged += new System.EventHandler(this.sourceControlComboBox_SelectedIndexChanged);
            // 
            // packagesComboBoxList
            // 
            this.packagesComboBoxList.CheckOnClick = true;
            this.packagesComboBoxList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.packagesComboBoxList.DropDownHeight = 1;
            this.packagesComboBoxList.Enabled = false;
            this.packagesComboBoxList.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.packagesComboBoxList.FormattingEnabled = true;
            this.packagesComboBoxList.IntegralHeight = false;
            this.packagesComboBoxList.Location = new System.Drawing.Point(108, 70);
            this.packagesComboBoxList.Name = "packagesComboBoxList";
            this.packagesComboBoxList.Size = new System.Drawing.Size(536, 24);
            this.packagesComboBoxList.TabIndex = 20;
            this.packagesComboBoxList.ValueSeparator = "; ";
            this.packagesComboBoxList.DropDownClosed += new System.EventHandler(this.packagesComboBoxList_DropDownClosed);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 511);
            this.Controls.Add(this.sourceControlComboBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.deleteOldCheckBox);
            this.Controls.Add(this.uploadCheckBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.packagesComboBoxList);
            this.Controls.Add(this.versionsDropDown);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.latestCheckBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.statusTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.repositoryTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.solutionsList);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.rootPathTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Acceleratio.Nuget.Updater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox rootPathTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.ListBox solutionsList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox repositoryTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox statusTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox latestCheckBox;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.ComboBox versionsDropDown;
        private CheckComboBoxTest.CheckedComboBox packagesComboBoxList;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox uploadCheckBox;
        private System.Windows.Forms.CheckBox deleteOldCheckBox;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox sourceControlComboBox;
    }
}

