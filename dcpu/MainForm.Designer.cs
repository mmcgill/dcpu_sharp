namespace Com.MattMcGill.Dcpu {
    partial class MainForm {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this._terminalPanel = new Com.MattMcGill.Dcpu.TerminalPanel();
            this.SuspendLayout();
            // 
            // _terminalPanel
            // 
            this._terminalPanel.Dcpu = null;
            this._terminalPanel.Location = new System.Drawing.Point(0, 0);
            this._terminalPanel.Margin = new System.Windows.Forms.Padding(0);
            this._terminalPanel.MaximumSize = new System.Drawing.Size(384, 288);
            this._terminalPanel.MinimumSize = new System.Drawing.Size(384, 288);
            this._terminalPanel.Name = "_terminalPanel";
            this._terminalPanel.Size = new System.Drawing.Size(384, 288);
            this._terminalPanel.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(402, 294);
            this.Controls.Add(this._terminalPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MainForm";
            this.Text = "DCPU-16";
            this.ResumeLayout(false);

        }

        #endregion

        private TerminalPanel _terminalPanel;


    }
}