namespace RFPBuilder
{
    partial class MappingForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.Mapping = new MetroFramework.Controls.MetroTabControl();
            this.Modules = new MetroFramework.Controls.MetroTabPage();
            this.Responses = new MetroFramework.Controls.MetroTabPage();
            this.Position = new MetroFramework.Controls.MetroTabPage();
            this.panel1.SuspendLayout();
            this.Mapping.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(117)))), ((int)(((byte)(233)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(760, 80);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(26, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 36);
            this.label1.TabIndex = 2;
            this.label1.Text = "Mapping";
            // 
            // Mapping
            // 
            this.Mapping.Controls.Add(this.Modules);
            this.Mapping.Controls.Add(this.Responses);
            this.Mapping.Controls.Add(this.Position);
            this.Mapping.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Mapping.Location = new System.Drawing.Point(0, 80);
            this.Mapping.Name = "Mapping";
            this.Mapping.SelectedIndex = 0;
            this.Mapping.Size = new System.Drawing.Size(760, 359);
            this.Mapping.TabIndex = 1;
            this.Mapping.UseSelectable = true;
            // 
            // Modules
            // 
            this.Modules.HorizontalScrollbarBarColor = true;
            this.Modules.HorizontalScrollbarHighlightOnWheel = false;
            this.Modules.HorizontalScrollbarSize = 10;
            this.Modules.Location = new System.Drawing.Point(4, 38);
            this.Modules.Name = "Modules";
            this.Modules.Size = new System.Drawing.Size(752, 317);
            this.Modules.TabIndex = 0;
            this.Modules.Text = "Modules";
            this.Modules.VerticalScrollbarBarColor = true;
            this.Modules.VerticalScrollbarHighlightOnWheel = false;
            this.Modules.VerticalScrollbarSize = 10;
            // 
            // Responses
            // 
            this.Responses.HorizontalScrollbarBarColor = true;
            this.Responses.HorizontalScrollbarHighlightOnWheel = false;
            this.Responses.HorizontalScrollbarSize = 10;
            this.Responses.Location = new System.Drawing.Point(4, 38);
            this.Responses.Name = "Responses";
            this.Responses.Size = new System.Drawing.Size(752, 317);
            this.Responses.TabIndex = 1;
            this.Responses.Text = "Responses";
            this.Responses.VerticalScrollbarBarColor = true;
            this.Responses.VerticalScrollbarHighlightOnWheel = false;
            this.Responses.VerticalScrollbarSize = 10;
            // 
            // Position
            // 
            this.Position.HorizontalScrollbarBarColor = true;
            this.Position.HorizontalScrollbarHighlightOnWheel = false;
            this.Position.HorizontalScrollbarSize = 10;
            this.Position.Location = new System.Drawing.Point(4, 38);
            this.Position.Name = "Position";
            this.Position.Size = new System.Drawing.Size(752, 317);
            this.Position.Style = MetroFramework.MetroColorStyle.Green;
            this.Position.TabIndex = 2;
            this.Position.Text = "Position";
            this.Position.VerticalScrollbarBarColor = true;
            this.Position.VerticalScrollbarHighlightOnWheel = false;
            this.Position.VerticalScrollbarSize = 10;
            // 
            // MappingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 439);
            this.Controls.Add(this.Mapping);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MappingForm";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.Mapping.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private MetroFramework.Controls.MetroTabControl Mapping;
        private MetroFramework.Controls.MetroTabPage Modules;
        private MetroFramework.Controls.MetroTabPage Responses;
        private MetroFramework.Controls.MetroTabPage Position;
    }
}