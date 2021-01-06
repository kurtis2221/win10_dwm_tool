
namespace win10_dwm_tool
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.bt_dwm_on = new System.Windows.Forms.Button();
            this.bt_dwm_off = new System.Windows.Forms.Button();
            this.bt_run = new System.Windows.Forms.Button();
            this.tb_run = new System.Windows.Forms.TextBox();
            this.tmr_restore = new System.Windows.Forms.Timer(this.components);
            this.tb_console = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // bt_dwm_on
            // 
            this.bt_dwm_on.Location = new System.Drawing.Point(12, 12);
            this.bt_dwm_on.Name = "bt_dwm_on";
            this.bt_dwm_on.Size = new System.Drawing.Size(260, 24);
            this.bt_dwm_on.TabIndex = 0;
            this.bt_dwm_on.Text = "Enable DWM";
            this.bt_dwm_on.UseVisualStyleBackColor = true;
            this.bt_dwm_on.Click += new System.EventHandler(this.bt_dwm_on_Click);
            // 
            // bt_dwm_off
            // 
            this.bt_dwm_off.Location = new System.Drawing.Point(12, 42);
            this.bt_dwm_off.Name = "bt_dwm_off";
            this.bt_dwm_off.Size = new System.Drawing.Size(260, 24);
            this.bt_dwm_off.TabIndex = 1;
            this.bt_dwm_off.Text = "Disable DWM";
            this.bt_dwm_off.UseVisualStyleBackColor = true;
            this.bt_dwm_off.Click += new System.EventHandler(this.bt_dwm_off_Click);
            // 
            // bt_run
            // 
            this.bt_run.Location = new System.Drawing.Point(12, 102);
            this.bt_run.Name = "bt_run";
            this.bt_run.Size = new System.Drawing.Size(56, 24);
            this.bt_run.TabIndex = 3;
            this.bt_run.Text = "Run";
            this.bt_run.UseVisualStyleBackColor = true;
            this.bt_run.Click += new System.EventHandler(this.bt_run_Click);
            // 
            // tb_run
            // 
            this.tb_run.Location = new System.Drawing.Point(74, 105);
            this.tb_run.Name = "tb_run";
            this.tb_run.Size = new System.Drawing.Size(198, 20);
            this.tb_run.TabIndex = 4;
            // 
            // tmr_restore
            // 
            this.tmr_restore.Interval = 5000;
            this.tmr_restore.Tick += new System.EventHandler(this.tmr_restore_Tick);
            // 
            // tb_console
            // 
            this.tb_console.BackColor = System.Drawing.Color.White;
            this.tb_console.Location = new System.Drawing.Point(278, 12);
            this.tb_console.Multiline = true;
            this.tb_console.Name = "tb_console";
            this.tb_console.ReadOnly = true;
            this.tb_console.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_console.Size = new System.Drawing.Size(294, 113);
            this.tb_console.TabIndex = 5;
            this.tb_console.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 141);
            this.Controls.Add(this.tb_console);
            this.Controls.Add(this.tb_run);
            this.Controls.Add(this.bt_run);
            this.Controls.Add(this.bt_dwm_off);
            this.Controls.Add(this.bt_dwm_on);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Windows 10 DWM Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bt_dwm_on;
        private System.Windows.Forms.Button bt_dwm_off;
        private System.Windows.Forms.Button bt_run;
        private System.Windows.Forms.TextBox tb_run;
        private System.Windows.Forms.Timer tmr_restore;
        private System.Windows.Forms.TextBox tb_console;
    }
}

