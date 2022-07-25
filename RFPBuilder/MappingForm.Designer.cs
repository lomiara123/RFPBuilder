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
            this.buttonMinimize = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Mapping = new MetroFramework.Controls.MetroTabControl();
            this.Modules = new MetroFramework.Controls.MetroTabPage();
            this.ModulesMapGrid = new Zuby.ADGV.AdvancedDataGridView();
            this.Responses = new MetroFramework.Controls.MetroTabPage();
            this.responseDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.ResponsesGrid = new Zuby.ADGV.AdvancedDataGridView();
            this.Position = new MetroFramework.Controls.MetroTabPage();
            this.PositionMapGrid = new Zuby.ADGV.AdvancedDataGridView();
            this.panel1.SuspendLayout();
            this.Mapping.SuspendLayout();
            this.Modules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModulesMapGrid)).BeginInit();
            this.Responses.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResponsesGrid)).BeginInit();
            this.Position.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PositionMapGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(117)))), ((int)(((byte)(233)))));
            this.panel1.Controls.Add(this.buttonMinimize);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1000, 120);
            this.panel1.TabIndex = 0;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            // 
            // buttonMinimize
            // 
            this.buttonMinimize.FlatAppearance.BorderSize = 0;
            this.buttonMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMinimize.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.buttonMinimize.ForeColor = System.Drawing.Color.White;
            this.buttonMinimize.Location = new System.Drawing.Point(915, 0);
            this.buttonMinimize.Name = "buttonMinimize";
            this.buttonMinimize.Size = new System.Drawing.Size(46, 41);
            this.buttonMinimize.TabIndex = 4;
            this.buttonMinimize.Text = "—";
            this.buttonMinimize.UseVisualStyleBackColor = true;
            this.buttonMinimize.Click += new System.EventHandler(this.buttonMinimize_Click);
            // 
            // btnClose
            // 
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(956, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(40, 41);
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
            this.Mapping.Location = new System.Drawing.Point(0, 120);
            this.Mapping.Name = "Mapping";
            this.Mapping.SelectedIndex = 1;
            this.Mapping.Size = new System.Drawing.Size(1000, 520);
            this.Mapping.TabIndex = 0;
            this.Mapping.UseSelectable = true;
            // 
            // Modules
            // 
            this.Modules.Controls.Add(this.ModulesMapGrid);
            this.Modules.HorizontalScrollbarBarColor = true;
            this.Modules.HorizontalScrollbarHighlightOnWheel = false;
            this.Modules.HorizontalScrollbarSize = 10;
            this.Modules.Location = new System.Drawing.Point(4, 38);
            this.Modules.Name = "Modules";
            this.Modules.Size = new System.Drawing.Size(992, 478);
            this.Modules.TabIndex = 0;
            this.Modules.Text = "Modules";
            this.Modules.VerticalScrollbarBarColor = true;
            this.Modules.VerticalScrollbarHighlightOnWheel = false;
            this.Modules.VerticalScrollbarSize = 10;
            // 
            // ModulesMapGrid
            // 
            this.ModulesMapGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ModulesMapGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ModulesMapGrid.BackgroundColor = System.Drawing.Color.White;
            this.ModulesMapGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ModulesMapGrid.FilterAndSortEnabled = true;
            this.ModulesMapGrid.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.ModulesMapGrid.Location = new System.Drawing.Point(3, 3);
            this.ModulesMapGrid.Name = "ModulesMapGrid";
            this.ModulesMapGrid.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ModulesMapGrid.RowHeadersWidth = 51;
            this.ModulesMapGrid.RowTemplate.Height = 24;
            this.ModulesMapGrid.Size = new System.Drawing.Size(981, 467);
            this.ModulesMapGrid.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            this.ModulesMapGrid.TabIndex = 2;
            this.ModulesMapGrid.SortStringChanged += new System.EventHandler<Zuby.ADGV.AdvancedDataGridView.SortEventArgs>(this.ModulesMapGrid_SortStringChanged);
            this.ModulesMapGrid.FilterStringChanged += new System.EventHandler<Zuby.ADGV.AdvancedDataGridView.FilterEventArgs>(this.ModulesMapGrid_FilterStringChanged);
            // 
            // Responses
            // 
            this.Responses.Controls.Add(this.responseDescriptionTextBox);
            this.Responses.Controls.Add(this.ResponsesGrid);
            this.Responses.HorizontalScrollbarBarColor = true;
            this.Responses.HorizontalScrollbarHighlightOnWheel = false;
            this.Responses.HorizontalScrollbarSize = 10;
            this.Responses.Location = new System.Drawing.Point(4, 38);
            this.Responses.Name = "Responses";
            this.Responses.Size = new System.Drawing.Size(992, 478);
            this.Responses.TabIndex = 1;
            this.Responses.Text = "Responses";
            this.Responses.VerticalScrollbarBarColor = true;
            this.Responses.VerticalScrollbarHighlightOnWheel = false;
            this.Responses.VerticalScrollbarSize = 10;
            // 
            // responseDescriptionTextBox
            // 
            this.responseDescriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.responseDescriptionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.responseDescriptionTextBox.Location = new System.Drawing.Point(3, 416);
            this.responseDescriptionTextBox.Multiline = true;
            this.responseDescriptionTextBox.Name = "responseDescriptionTextBox";
            this.responseDescriptionTextBox.ReadOnly = true;
            this.responseDescriptionTextBox.Size = new System.Drawing.Size(981, 44);
            this.responseDescriptionTextBox.TabIndex = 3;
            // 
            // ResponsesGrid
            // 
            this.ResponsesGrid.AllowUserToOrderColumns = true;
            this.ResponsesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResponsesGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ResponsesGrid.BackgroundColor = System.Drawing.Color.White;
            this.ResponsesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ResponsesGrid.FilterAndSortEnabled = true;
            this.ResponsesGrid.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.ResponsesGrid.Location = new System.Drawing.Point(3, 3);
            this.ResponsesGrid.Name = "ResponsesGrid";
            this.ResponsesGrid.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ResponsesGrid.RowHeadersWidth = 51;
            this.ResponsesGrid.RowTemplate.Height = 24;
            this.ResponsesGrid.Size = new System.Drawing.Size(981, 398);
            this.ResponsesGrid.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            this.ResponsesGrid.TabIndex = 2;
            this.ResponsesGrid.SortStringChanged += new System.EventHandler<Zuby.ADGV.AdvancedDataGridView.SortEventArgs>(this.ResponsesGrid_SortStringChanged);
            this.ResponsesGrid.FilterStringChanged += new System.EventHandler<Zuby.ADGV.AdvancedDataGridView.FilterEventArgs>(this.ResponsesGrid_FilterStringChanged);
            this.ResponsesGrid.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.ResponsesGrid_RowEnter);
            this.ResponsesGrid.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.ResponsesGrid_RowValidating);
            // 
            // Position
            // 
            this.Position.Controls.Add(this.PositionMapGrid);
            this.Position.HorizontalScrollbarBarColor = true;
            this.Position.HorizontalScrollbarHighlightOnWheel = false;
            this.Position.HorizontalScrollbarSize = 10;
            this.Position.Location = new System.Drawing.Point(4, 38);
            this.Position.Name = "Position";
            this.Position.Size = new System.Drawing.Size(992, 478);
            this.Position.Style = MetroFramework.MetroColorStyle.Green;
            this.Position.TabIndex = 2;
            this.Position.Text = "Position";
            this.Position.VerticalScrollbarBarColor = true;
            this.Position.VerticalScrollbarHighlightOnWheel = false;
            this.Position.VerticalScrollbarSize = 10;
            // 
            // PositionMapGrid
            // 
            this.PositionMapGrid.AllowUserToOrderColumns = true;
            this.PositionMapGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PositionMapGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.PositionMapGrid.BackgroundColor = System.Drawing.Color.White;
            this.PositionMapGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PositionMapGrid.FilterAndSortEnabled = true;
            this.PositionMapGrid.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.PositionMapGrid.Location = new System.Drawing.Point(8, 3);
            this.PositionMapGrid.Name = "PositionMapGrid";
            this.PositionMapGrid.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PositionMapGrid.RowHeadersWidth = 51;
            this.PositionMapGrid.RowTemplate.Height = 24;
            this.PositionMapGrid.Size = new System.Drawing.Size(976, 467);
            this.PositionMapGrid.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            this.PositionMapGrid.TabIndex = 2;
            this.PositionMapGrid.SortStringChanged += new System.EventHandler<Zuby.ADGV.AdvancedDataGridView.SortEventArgs>(this.PositionMapGrid_SortStringChanged);
            this.PositionMapGrid.FilterStringChanged += new System.EventHandler<Zuby.ADGV.AdvancedDataGridView.FilterEventArgs>(this.PositionMapGrid_FilterStringChanged);
            this.PositionMapGrid.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.PositionMapGrid_RowValidated);
            this.PositionMapGrid.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.PositionMapGrid_RowValidating);
            // 
            // MappingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 640);
            this.ControlBox = false;
            this.Controls.Add(this.Mapping);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MappingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.Mapping.ResumeLayout(false);
            this.Modules.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ModulesMapGrid)).EndInit();
            this.Responses.ResumeLayout(false);
            this.Responses.PerformLayout();
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
        private System.Windows.Forms.Button buttonMinimize;
        private System.Windows.Forms.Button btnClose;
        private Zuby.ADGV.AdvancedDataGridView ModulesMapGrid;
        private Zuby.ADGV.AdvancedDataGridView ResponsesGrid;
        private Zuby.ADGV.AdvancedDataGridView PositionMapGrid;
        private System.Windows.Forms.TextBox responseDescriptionTextBox;
    }
}