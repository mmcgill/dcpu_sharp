﻿namespace Com.MattMcGill.Dcpu {
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
         this._stopStartBtn = new System.Windows.Forms.Button();
         this._timeTrackBar = new System.Windows.Forms.TrackBar();
         ((System.ComponentModel.ISupportInitialize)(this._timeTrackBar)).BeginInit();
         this.SuspendLayout();
         // 
         // _terminalPanel
         // 
         this._terminalPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this._terminalPanel.Dcpu = null;
         this._terminalPanel.Location = new System.Drawing.Point(9, 9);
         this._terminalPanel.Margin = new System.Windows.Forms.Padding(0);
         this._terminalPanel.MaximumSize = new System.Drawing.Size(384, 288);
         this._terminalPanel.MinimumSize = new System.Drawing.Size(384, 288);
         this._terminalPanel.Name = "_terminalPanel";
         this._terminalPanel.Size = new System.Drawing.Size(384, 288);
         this._terminalPanel.TabIndex = 0;
         // 
         // _stopStartBtn
         // 
         this._stopStartBtn.Location = new System.Drawing.Point(9, 309);
         this._stopStartBtn.Name = "_stopStartBtn";
         this._stopStartBtn.Size = new System.Drawing.Size(75, 23);
         this._stopStartBtn.TabIndex = 1;
         this._stopStartBtn.Text = "Start";
         this._stopStartBtn.UseVisualStyleBackColor = true;
         this._stopStartBtn.Click += new System.EventHandler(this.HandleStartStopClick);
         // 
         // _timeTrackBar
         // 
         this._timeTrackBar.Location = new System.Drawing.Point(90, 300);
         this._timeTrackBar.Maximum = 20;
         this._timeTrackBar.Name = "_timeTrackBar";
         this._timeTrackBar.Size = new System.Drawing.Size(300, 45);
         this._timeTrackBar.TabIndex = 2;
         this._timeTrackBar.Value = 20;
         this._timeTrackBar.ValueChanged += new System.EventHandler(this.HandleTimeTrackbarValueChanged);
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.AutoSize = true;
         this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.ClientSize = new System.Drawing.Size(402, 345);
         this.Controls.Add(this._timeTrackBar);
         this.Controls.Add(this._stopStartBtn);
         this.Controls.Add(this._terminalPanel);
         this.DoubleBuffered = true;
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
         this.Name = "MainForm";
         this.Text = "DCPU-16";
         ((System.ComponentModel.ISupportInitialize)(this._timeTrackBar)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

        }

        #endregion

        private TerminalPanel _terminalPanel;
        private System.Windows.Forms.Button _stopStartBtn;
        private System.Windows.Forms.TrackBar _timeTrackBar;


    }
}