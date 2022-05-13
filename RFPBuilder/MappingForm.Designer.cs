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
            this.button2 = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Mapping = new MetroFramework.Controls.MetroTabControl();
            this.Modules = new MetroFramework.Controls.MetroTabPage();
            this.ModulesMapGrid = new System.Windows.Forms.DataGridView();
            this.moduleMapDataGridView = new System.Windows.Forms.DataGridView();
            this.Responses = new MetroFramework.Controls.MetroTabPage();
            this.ResponsesGrid = new System.Windows.Forms.DataGridView();
            this.Position = new MetroFramework.Controls.MetroTabPage();
            this.PositionMapGrid = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.Mapping.SuspendLayout();
            this.Modules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModulesMapGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.moduleMapDataGridView)).BeginInit();
            this.Responses.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResponsesGrid)).BeginInit();
            this.Position.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PositionMapGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(117)))), ((int)(((byte)(233)))));
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(786, 100);
            this.panel1.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Right;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 28.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(661, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(63, 100);
            this.button2.TabIndex = 4;
            this.button2.Text = "-";
            this.button2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(724, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(62, 100);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
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
            this.Mapping.Location = new System.Drawing.Point(0, 100);
            this.Mapping.Name = "Mapping";
            this.Mapping.SelectedIndex = 2;
            this.Mapping.Size = new System.Drawing.Size(786, 345);
            this.Mapping.TabIndex = 0;
            this.Mapping.UseSelectable = true;
            // 
            // Modules
            // 
            this.Modules.Controls.Add(this.ModulesMapGrid);
            this.Modules.Controls.Add(this.moduleMapDataGridView);
            this.Modules.HorizontalScrollbarBarColor = true;
            this.Modules.HorizontalScrollbarHighlightOnWheel = false;
            this.Modules.HorizontalScrollbarSize = 10;
            this.Modules.Location = new System.Drawing.Point(4, 38);
            this.Modules.Name = "Modules";
            this.Modules.Size = new System.Drawing.Size(778, 303);
            this.Modules.TabIndex = 0;
            this.Modules.Text = "Modules";
            this.Modules.VerticalScrollbarBarColor = true;
            this.Modules.VerticalScrollbarHighlightOnWheel = false;
            this.Modules.VerticalScrollbarSize = 10;
            // 
            // ModulesMapGrid
            // 
            this.ModulesMapGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ModulesMapGrid.BackgroundColor = System.Drawing.Color.White;
            this.ModulesMapGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ModulesMapGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModulesMapGrid.Location = new System.Drawing.Point(0, 0);
            this.ModulesMapGrid.Name = "ModulesMapGrid";
            this.ModulesMapGrid.RowHeadersWidth = 51;
            this.ModulesMapGrid.RowTemplate.Height = 24;
            this.ModulesMapGrid.Size = new System.Drawing.Size(778, 303);
            this.ModulesMapGrid.TabIndex = 3;
            // 
            // moduleMapDataGridView
            // 
            this.moduleMapDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.moduleMapDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.moduleMapDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.moduleMapDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.moduleMapDataGridView.Location = new System.Drawing.Point(0, 0);
            this.moduleMapDataGridView.Name = "moduleMapDataGridView";
            this.moduleMapDataGridView.RowHeadersWidth = 51;
            this.moduleMapDataGridView.RowTemplate.Height = 24;
            this.moduleMapDataGridView.Size = new System.Drawing.Size(778, 303);
            this.moduleMapDataGridView.TabIndex = 2;
            // 
            // Responses
            // 
            this.Responses.Controls.Add(this.ResponsesGrid);
            this.Responses.HorizontalScrollbarBarColor = true;
            this.Responses.HorizontalScrollbarHighlightOnWheel = false;
            this.Responses.HorizontalScrollbarSize = 10;
            this.Responses.Location = new System.Drawing.Point(4, 38);
            this.Responses.Name = "Responses";
            this.Responses.Size = new System.Drawing.Size(778, 303);
            this.Responses.TabIndex = 1;
            this.Responses.Text = "Responses";
            this.Responses.VerticalScrollbarBarColor = true;
            this.Responses.VerticalScrollbarHighlightOnWheel = false;
            this.Responses.VerticalScrollbarSize = 10;
            // 
            // ResponsesGrid
            // 
            this.ResponsesGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ResponsesGrid.BackgroundColor = System.Drawing.Color.White;
            this.ResponsesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ResponsesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResponsesGrid.Location = new System.Drawing.Point(0, 0);
            this.ResponsesGrid.Name = "ResponsesGrid";
            this.ResponsesGrid.RowHeadersWidth = 51;
            this.ResponsesGrid.RowTemplate.Height = 24;
            this.ResponsesGrid.Size = new System.Drawing.Size(778, 303);
            this.ResponsesGrid.TabIndex = 2;
            // 
            // Position
            // 
            this.Position.Controls.Add(this.PositionMapGrid);
            this.Position.HorizontalScrollbarBarColor = true;
            this.Position.HorizontalScrollbarHighlightOnWheel = false;
            this.Position.HorizontalScrollbarSize = 10;
            this.Position.Location = new System.Drawing.Point(4, 38);
            this.Position.Name = "Position";
            this.Position.Size = new System.Drawing.Size(778, 303);
            this.Position.Style = MetroFramework.MetroColorStyle.Green;
            this.Position.TabIndex = 2;
            this.Position.Text = "Position";
            this.Position.VerticalScrollbarBarColor = true;
            this.Position.VerticalScrollbarHighlightOnWheel = false;
            this.Position.VerticalScrollbarSize = 10;
            // 
            // PositionMapGrid
            // 
            this.PositionMapGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.PositionMapGrid.BackgroundColor = System.Drawing.Color.White;
            this.PositionMapGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PositionMapGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PositionMapGrid.Location = new System.Drawing.Point(0, 0);
            this.PositionMapGrid.Name = "PositionMapGrid";
            this.PositionMapGrid.RowHeadersWidth = 51;
            this.PositionMapGrid.RowTemplate.Height = 24;
            this.PositionMapGrid.Size = new System.Drawing.Size(778, 303);
            this.PositionMapGrid.TabIndex = 2;
            // 
            // MappingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 445);
            this.ControlBox = false;
            this.Controls.Add(this.Mapping);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MappingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.Mapping.ResumeLayout(false);
            this.Modules.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ModulesMapGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.moduleMapDataGridView)).EndInit();
            this.Responses.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ResponsesGrid)).EndInit();
            this.Position.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PositionMapGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private MetroFramework.Controls.MetroTabControl Mapping;
        private MetroFramework.Controls.MetroTabPage Modules;
        private MetroFramework.Controls.MetroTabPage Responses;
        private MetroFramework.Controls.MetroTabPage Position;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView moduleMapDataGridView;
        private System.Windows.Forms.DataGridView ModulesMapGrid;
        private System.Windows.Forms.DataGridView ResponsesGrid;
        private System.Windows.Forms.DataGridView PositionMapGrid;
    }
}