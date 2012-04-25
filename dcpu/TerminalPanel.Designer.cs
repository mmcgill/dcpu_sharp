namespace Com.MattMcGill.Dcpu {
    partial class TerminalPanel {
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
            this.SuspendLayout();
            // 
            // TerminalPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.MaximumSize = new System.Drawing.Size(384, 288);
            this.MinimumSize = new System.Drawing.Size(384, 288);
            this.Name = "TerminalPanel";
            this.Size = new System.Drawing.Size(384, 288);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DoPaint);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HandleKeyPress);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
