namespace VsRemovePublishButton {
    partial class OptionPage {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.chkPublishButton = new System.Windows.Forms.CheckBox();
            this.btnToggleVisibility = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkPublishButton
            // 
            this.chkPublishButton.AutoSize = true;
            this.chkPublishButton.Location = new System.Drawing.Point(3, 3);
            this.chkPublishButton.Name = "chkPublishButton";
            this.chkPublishButton.Size = new System.Drawing.Size(184, 17);
            this.chkPublishButton.TabIndex = 0;
            this.chkPublishButton.Text = "&Hide the publish button by default";
            this.chkPublishButton.UseVisualStyleBackColor = true;
            this.chkPublishButton.CheckedChanged += new System.EventHandler(this.chkPublishButton_OnCheckedChanged);
            // 
            // btnToggleVisibility
            // 
            this.btnToggleVisibility.Location = new System.Drawing.Point(3, 26);
            this.btnToggleVisibility.Name = "btnToggleVisibility";
            this.btnToggleVisibility.Size = new System.Drawing.Size(109, 23);
            this.btnToggleVisibility.TabIndex = 1;
            this.btnToggleVisibility.Text = "&Toggle visibility";
            this.btnToggleVisibility.UseVisualStyleBackColor = true;
            this.btnToggleVisibility.Click += new System.EventHandler(this.btnToggleVisibility_OnClick);
            // 
            // OptionPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnToggleVisibility);
            this.Controls.Add(this.chkPublishButton);
            this.Name = "OptionPage";
            this.Size = new System.Drawing.Size(339, 68);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkPublishButton;
        private System.Windows.Forms.Button btnToggleVisibility;
    }
}
