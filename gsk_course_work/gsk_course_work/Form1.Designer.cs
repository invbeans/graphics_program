namespace gsk_course_work
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.showClrDlg = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.segmentButton = new System.Windows.Forms.Button();
            this.flagButton = new System.Windows.Forms.Button();
            this.triangleButton = new System.Windows.Forms.Button();
            this.splineButton = new System.Windows.Forms.Button();
            this.colorPreview = new System.Windows.Forms.PictureBox();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.figuresContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.переместитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.поворотВокругЗаданногоЦентраНаПроизвольныйУголToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.зеркальноеОтражениеОтносительноЦентраФигурыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.зеркальноеОтражениеОтносительноВертикальнойПрямойToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.удалитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpLabel = new System.Windows.Forms.Label();
            this.angleTrackBar = new System.Windows.Forms.TrackBar();
            this.choseOperands = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.colorPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.figuresContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.angleTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            // 
            // showClrDlg
            // 
            this.showClrDlg.Location = new System.Drawing.Point(121, 12);
            this.showClrDlg.Name = "showClrDlg";
            this.showClrDlg.Size = new System.Drawing.Size(147, 32);
            this.showClrDlg.TabIndex = 1;
            this.showClrDlg.Text = "Выбрать цвет...";
            this.showClrDlg.UseVisualStyleBackColor = true;
            this.showClrDlg.Click += new System.EventHandler(this.ShowClrDlg_Click);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(7, 720);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(111, 34);
            this.clearButton.TabIndex = 3;
            this.clearButton.Text = "Очистить";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // segmentButton
            // 
            this.segmentButton.BackgroundImage = global::gsk_course_work.Properties.Resources.segment_icon;
            this.segmentButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.segmentButton.Location = new System.Drawing.Point(223, 58);
            this.segmentButton.Name = "segmentButton";
            this.segmentButton.Size = new System.Drawing.Size(75, 75);
            this.segmentButton.TabIndex = 8;
            this.segmentButton.UseVisualStyleBackColor = true;
            this.segmentButton.Click += new System.EventHandler(this.button4_Click);
            // 
            // flagButton
            // 
            this.flagButton.BackgroundImage = global::gsk_course_work.Properties.Resources.flag_icon;
            this.flagButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.flagButton.Location = new System.Drawing.Point(151, 58);
            this.flagButton.Name = "flagButton";
            this.flagButton.Size = new System.Drawing.Size(75, 75);
            this.flagButton.TabIndex = 7;
            this.flagButton.UseVisualStyleBackColor = true;
            this.flagButton.Click += new System.EventHandler(this.button3_Click);
            // 
            // triangleButton
            // 
            this.triangleButton.BackgroundImage = global::gsk_course_work.Properties.Resources.triangle_icon;
            this.triangleButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.triangleButton.Location = new System.Drawing.Point(79, 58);
            this.triangleButton.Name = "triangleButton";
            this.triangleButton.Size = new System.Drawing.Size(75, 75);
            this.triangleButton.TabIndex = 6;
            this.triangleButton.UseVisualStyleBackColor = true;
            this.triangleButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // splineButton
            // 
            this.splineButton.BackgroundImage = global::gsk_course_work.Properties.Resources.spline_icon;
            this.splineButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.splineButton.Location = new System.Drawing.Point(7, 58);
            this.splineButton.Name = "splineButton";
            this.splineButton.Size = new System.Drawing.Size(75, 75);
            this.splineButton.TabIndex = 5;
            this.splineButton.UseVisualStyleBackColor = true;
            this.splineButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // colorPreview
            // 
            this.colorPreview.Location = new System.Drawing.Point(12, 12);
            this.colorPreview.Name = "colorPreview";
            this.colorPreview.Size = new System.Drawing.Size(79, 40);
            this.colorPreview.TabIndex = 2;
            this.colorPreview.TabStop = false;
            // 
            // canvas
            // 
            this.canvas.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.canvas.Location = new System.Drawing.Point(304, 0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(1103, 765);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            // 
            // figuresContextMenu
            // 
            this.figuresContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.figuresContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.переместитьToolStripMenuItem,
            this.поворотВокругЗаданногоЦентраНаПроизвольныйУголToolStripMenuItem,
            this.зеркальноеОтражениеОтносительноЦентраФигурыToolStripMenuItem,
            this.зеркальноеОтражениеОтносительноВертикальнойПрямойToolStripMenuItem,
            this.удалитьToolStripMenuItem});
            this.figuresContextMenu.Name = "figuresContextMenu";
            this.figuresContextMenu.Size = new System.Drawing.Size(504, 124);
            // 
            // переместитьToolStripMenuItem
            // 
            this.переместитьToolStripMenuItem.Name = "переместитьToolStripMenuItem";
            this.переместитьToolStripMenuItem.Size = new System.Drawing.Size(503, 24);
            this.переместитьToolStripMenuItem.Text = "Переместить";
            this.переместитьToolStripMenuItem.Click += new System.EventHandler(this.переместитьToolStripMenuItem_Click);
            // 
            // поворотВокругЗаданногоЦентраНаПроизвольныйУголToolStripMenuItem
            // 
            this.поворотВокругЗаданногоЦентраНаПроизвольныйУголToolStripMenuItem.Name = "поворотВокругЗаданногоЦентраНаПроизвольныйУголToolStripMenuItem";
            this.поворотВокругЗаданногоЦентраНаПроизвольныйУголToolStripMenuItem.Size = new System.Drawing.Size(503, 24);
            this.поворотВокругЗаданногоЦентраНаПроизвольныйУголToolStripMenuItem.Text = "Поворот вокруг заданного центра на произвольный угол";
            this.поворотВокругЗаданногоЦентраНаПроизвольныйУголToolStripMenuItem.Click += new System.EventHandler(this.поворотВокругЗаданногоЦентраНаПроизвольныйУголToolStripMenuItem_Click);
            // 
            // зеркальноеОтражениеОтносительноЦентраФигурыToolStripMenuItem
            // 
            this.зеркальноеОтражениеОтносительноЦентраФигурыToolStripMenuItem.Name = "зеркальноеОтражениеОтносительноЦентраФигурыToolStripMenuItem";
            this.зеркальноеОтражениеОтносительноЦентраФигурыToolStripMenuItem.Size = new System.Drawing.Size(503, 24);
            this.зеркальноеОтражениеОтносительноЦентраФигурыToolStripMenuItem.Text = "Зеркальное отражение относительно центра фигуры";
            this.зеркальноеОтражениеОтносительноЦентраФигурыToolStripMenuItem.Click += new System.EventHandler(this.зеркальноеОтражениеОтносительноЦентраФигурыToolStripMenuItem_Click);
            // 
            // зеркальноеОтражениеОтносительноВертикальнойПрямойToolStripMenuItem
            // 
            this.зеркальноеОтражениеОтносительноВертикальнойПрямойToolStripMenuItem.Name = "зеркальноеОтражениеОтносительноВертикальнойПрямойToolStripMenuItem";
            this.зеркальноеОтражениеОтносительноВертикальнойПрямойToolStripMenuItem.Size = new System.Drawing.Size(503, 24);
            this.зеркальноеОтражениеОтносительноВертикальнойПрямойToolStripMenuItem.Text = "Зеркальное отражение относительно вертикальной прямой";
            this.зеркальноеОтражениеОтносительноВертикальнойПрямойToolStripMenuItem.Click += new System.EventHandler(this.зеркальноеОтражениеОтносительноВертикальнойПрямойToolStripMenuItem_Click);
            // 
            // удалитьToolStripMenuItem
            // 
            this.удалитьToolStripMenuItem.Name = "удалитьToolStripMenuItem";
            this.удалитьToolStripMenuItem.Size = new System.Drawing.Size(503, 24);
            this.удалитьToolStripMenuItem.Text = "Удалить";
            this.удалитьToolStripMenuItem.Click += new System.EventHandler(this.удалитьToolStripMenuItem_Click);
            // 
            // helpLabel
            // 
            this.helpLabel.AutoSize = true;
            this.helpLabel.Location = new System.Drawing.Point(9, 183);
            this.helpLabel.Name = "helpLabel";
            this.helpLabel.Size = new System.Drawing.Size(78, 16);
            this.helpLabel.TabIndex = 9;
            this.helpLabel.Text = "Подсказки";
            // 
            // angleTrackBar
            // 
            this.angleTrackBar.Location = new System.Drawing.Point(7, 176);
            this.angleTrackBar.Maximum = 360;
            this.angleTrackBar.Name = "angleTrackBar";
            this.angleTrackBar.Size = new System.Drawing.Size(291, 56);
            this.angleTrackBar.TabIndex = 10;
            this.angleTrackBar.Scroll += new System.EventHandler(this.AngleTrackBar_Scroll);
            // 
            // choseOperands
            // 
            this.choseOperands.Location = new System.Drawing.Point(7, 139);
            this.choseOperands.Name = "choseOperands";
            this.choseOperands.Size = new System.Drawing.Size(206, 31);
            this.choseOperands.TabIndex = 11;
            this.choseOperands.Text = "Выбрать операнды ТМО";
            this.choseOperands.UseVisualStyleBackColor = true;
            this.choseOperands.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1409, 766);
            this.Controls.Add(this.choseOperands);
            this.Controls.Add(this.angleTrackBar);
            this.Controls.Add(this.helpLabel);
            this.Controls.Add(this.segmentButton);
            this.Controls.Add(this.flagButton);
            this.Controls.Add(this.triangleButton);
            this.Controls.Add(this.splineButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.colorPreview);
            this.Controls.Add(this.showClrDlg);
            this.Controls.Add(this.canvas);
            this.Name = "Form1";
            this.Text = "Курсовая работа ГСК";
            ((System.ComponentModel.ISupportInitialize)(this.colorPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.figuresContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.angleTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button showClrDlg;
        private System.Windows.Forms.PictureBox colorPreview;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button splineButton;
        private System.Windows.Forms.Button triangleButton;
        private System.Windows.Forms.Button flagButton;
        private System.Windows.Forms.Button segmentButton;
        private System.Windows.Forms.ContextMenuStrip figuresContextMenu;
        private System.Windows.Forms.ToolStripMenuItem удалитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem переместитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem поворотВокругЗаданногоЦентраНаПроизвольныйУголToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem зеркальноеОтражениеОтносительноЦентраФигурыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem зеркальноеОтражениеОтносительноВертикальнойПрямойToolStripMenuItem;
        private System.Windows.Forms.Label helpLabel;
        private System.Windows.Forms.TrackBar angleTrackBar;
        private System.Windows.Forms.Button choseOperands;
    }
}

