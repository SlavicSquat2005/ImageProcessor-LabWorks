namespace ImgApp_2_WinForms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button bOpen1;
        private System.Windows.Forms.Button bOpen2;

        // Элементы меню:
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem operationsToolStripMenuItem;

        // Пункты меню операций:
        private System.Windows.Forms.ToolStripMenuItem sumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem averageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem productToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maskToolStripMenuItem;

        // Пункт меню для бинаризации
        private System.Windows.Forms.ToolStripMenuItem binarizationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gavrilovToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem otsuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem niblackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sauvolaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wolfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bradleyRothToolStripMenuItem;

        // Элементы для выбора изображения для бинаризации
        private System.Windows.Forms.GroupBox groupBoxBinarizationImage;
        private System.Windows.Forms.ComboBox comboBinarizationImage;
        private System.Windows.Forms.Label lblSelectedBinarizationMethod;
        private System.Windows.Forms.Button btnApplyToView;
        private System.Windows.Forms.Button btnApplyBinarization;
        private System.Windows.Forms.Button btnCancelBinarization;

        // Элементы прогресса:
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label progressLabel;

        // Элементы для выбора операции:
        private System.Windows.Forms.GroupBox groupBoxSelectedOperation;
        private System.Windows.Forms.Label lblSelectedOperation;
        private System.Windows.Forms.Button btnStart;

        // Элементы для маски:
        private System.Windows.Forms.GroupBox groupBoxMaskSettings;
        private System.Windows.Forms.RadioButton radioCircle;
        private System.Windows.Forms.RadioButton radioSquare;
        private System.Windows.Forms.RadioButton radioRectangle;
        private System.Windows.Forms.Label lblMaskWidth;
        private System.Windows.Forms.Label lblMaskHeight;
        private System.Windows.Forms.NumericUpDown nudMaskWidth;
        private System.Windows.Forms.NumericUpDown nudMaskHeight;

        // для выбора цветовых каналов
        private System.Windows.Forms.GroupBox groupBoxChannels;
        private System.Windows.Forms.CheckBox chkR;
        private System.Windows.Forms.CheckBox chkG;
        private System.Windows.Forms.CheckBox chkB;
        private System.Windows.Forms.RadioButton radioRGB;
        private System.Windows.Forms.RadioButton radioRG;
        private System.Windows.Forms.RadioButton radioRB;
        private System.Windows.Forms.RadioButton radioGB;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.bOpen1 = new System.Windows.Forms.Button();
            this.bOpen2 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.operationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.averageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.productToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.binarizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gavrilovToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otsuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.niblackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sauvolaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wolfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bradleyRothToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.groupBoxSelectedOperation = new System.Windows.Forms.GroupBox();
            this.lblSelectedOperation = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.groupBoxMaskSettings = new System.Windows.Forms.GroupBox();
            this.radioCircle = new System.Windows.Forms.RadioButton();
            this.radioSquare = new System.Windows.Forms.RadioButton();
            this.radioRectangle = new System.Windows.Forms.RadioButton();
            this.lblMaskWidth = new System.Windows.Forms.Label();
            this.lblMaskHeight = new System.Windows.Forms.Label();
            this.nudMaskWidth = new System.Windows.Forms.NumericUpDown();
            this.nudMaskHeight = new System.Windows.Forms.NumericUpDown();
            this.groupBoxChannels = new System.Windows.Forms.GroupBox();
            this.radioRGB = new System.Windows.Forms.RadioButton();
            this.radioRG = new System.Windows.Forms.RadioButton();
            this.radioRB = new System.Windows.Forms.RadioButton();
            this.radioGB = new System.Windows.Forms.RadioButton();
            this.chkR = new System.Windows.Forms.CheckBox();
            this.chkG = new System.Windows.Forms.CheckBox();
            this.chkB = new System.Windows.Forms.CheckBox();
            this.groupBoxBinarizationImage = new System.Windows.Forms.GroupBox();
            this.comboBinarizationImage = new System.Windows.Forms.ComboBox();
            this.lblSelectedBinarizationMethod = new System.Windows.Forms.Label();
            this.btnApplyBinarization = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBoxSelectedOperation.SuspendLayout();
            this.groupBoxMaskSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaskWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaskHeight)).BeginInit();
            this.groupBoxChannels.SuspendLayout();
            this.groupBoxBinarizationImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(65, 38);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(470, 320);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(603, 38);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(470, 320);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // bOpen1
            // 
            this.bOpen1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bOpen1.Location = new System.Drawing.Point(65, 367);
            this.bOpen1.Margin = new System.Windows.Forms.Padding(4);
            this.bOpen1.Name = "bOpen1";
            this.bOpen1.Size = new System.Drawing.Size(470, 37);
            this.bOpen1.TabIndex = 2;
            this.bOpen1.Text = "Загрузить изображение 1";
            this.bOpen1.UseVisualStyleBackColor = true;
            this.bOpen1.Click += new System.EventHandler(this.bOpen1_Click);
            // 
            // bOpen2
            // 
            this.bOpen2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bOpen2.Location = new System.Drawing.Point(603, 367);
            this.bOpen2.Margin = new System.Windows.Forms.Padding(4);
            this.bOpen2.Name = "bOpen2";
            this.bOpen2.Size = new System.Drawing.Size(470, 37);
            this.bOpen2.TabIndex = 3;
            this.bOpen2.Text = "Загрузить изображение 2";
            this.bOpen2.UseVisualStyleBackColor = true;
            this.bOpen2.Click += new System.EventHandler(this.bOpen2_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.operationsToolStripMenuItem,
            this.binarizationToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1147, 28);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // operationsToolStripMenuItem
            // 
            this.operationsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sumToolStripMenuItem,
            this.averageToolStripMenuItem,
            this.maxToolStripMenuItem,
            this.minToolStripMenuItem,
            this.productToolStripMenuItem,
            this.maskToolStripMenuItem});
            this.operationsToolStripMenuItem.Name = "operationsToolStripMenuItem";
            this.operationsToolStripMenuItem.Size = new System.Drawing.Size(95, 24);
            this.operationsToolStripMenuItem.Text = "Операции";
            // 
            // sumToolStripMenuItem
            // 
            this.sumToolStripMenuItem.Name = "sumToolStripMenuItem";
            this.sumToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.sumToolStripMenuItem.Text = "Суммировать";
            this.sumToolStripMenuItem.Click += new System.EventHandler(this.btnSumSelected);
            // 
            // averageToolStripMenuItem
            // 
            this.averageToolStripMenuItem.Name = "averageToolStripMenuItem";
            this.averageToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.averageToolStripMenuItem.Text = "Среднее арифметическое";
            this.averageToolStripMenuItem.Click += new System.EventHandler(this.btnAverageSelected);
            // 
            // maxToolStripMenuItem
            // 
            this.maxToolStripMenuItem.Name = "maxToolStripMenuItem";
            this.maxToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.maxToolStripMenuItem.Text = "Попиксельный максимум";
            this.maxToolStripMenuItem.Click += new System.EventHandler(this.btnMaxSelected);
            // 
            // minToolStripMenuItem
            // 
            this.minToolStripMenuItem.Name = "minToolStripMenuItem";
            this.minToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.minToolStripMenuItem.Text = "Попиксельный минимум";
            this.minToolStripMenuItem.Click += new System.EventHandler(this.btnMinSelected);
            // 
            // productToolStripMenuItem
            // 
            this.productToolStripMenuItem.Name = "productToolStripMenuItem";
            this.productToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.productToolStripMenuItem.Text = "Произведение";
            this.productToolStripMenuItem.Click += new System.EventHandler(this.btnProductSelected);
            // 
            // maskToolStripMenuItem
            // 
            this.maskToolStripMenuItem.Name = "maskToolStripMenuItem";
            this.maskToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.maskToolStripMenuItem.Text = "Наложить маску";
            this.maskToolStripMenuItem.Click += new System.EventHandler(this.btnMaskSelected);
            // 
            // binarizationToolStripMenuItem
            // 
            this.binarizationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gavrilovToolStripMenuItem,
            this.otsuToolStripMenuItem,
            this.niblackToolStripMenuItem,
            this.sauvolaToolStripMenuItem,
            this.wolfToolStripMenuItem,
            this.bradleyRothToolStripMenuItem});
            this.binarizationToolStripMenuItem.Name = "binarizationToolStripMenuItem";
            this.binarizationToolStripMenuItem.Size = new System.Drawing.Size(117, 24);
            this.binarizationToolStripMenuItem.Text = "Бинаризация";
            // 
            // gavrilovToolStripMenuItem
            // 
            this.gavrilovToolStripMenuItem.Name = "gavrilovToolStripMenuItem";
            this.gavrilovToolStripMenuItem.Size = new System.Drawing.Size(227, 26);
            this.gavrilovToolStripMenuItem.Text = "Метод Гаврилова";
            this.gavrilovToolStripMenuItem.Click += new System.EventHandler(this.BinarizationMethodSelected);
            // 
            // otsuToolStripMenuItem
            // 
            this.otsuToolStripMenuItem.Name = "otsuToolStripMenuItem";
            this.otsuToolStripMenuItem.Size = new System.Drawing.Size(227, 26);
            this.otsuToolStripMenuItem.Text = "Метод Отсу";
            this.otsuToolStripMenuItem.Click += new System.EventHandler(this.BinarizationMethodSelected);
            // 
            // niblackToolStripMenuItem
            // 
            this.niblackToolStripMenuItem.Name = "niblackToolStripMenuItem";
            this.niblackToolStripMenuItem.Size = new System.Drawing.Size(227, 26);
            this.niblackToolStripMenuItem.Text = "Метод Ниблека";
            this.niblackToolStripMenuItem.Click += new System.EventHandler(this.BinarizationMethodSelected);
            // 
            // sauvolaToolStripMenuItem
            // 
            this.sauvolaToolStripMenuItem.Name = "sauvolaToolStripMenuItem";
            this.sauvolaToolStripMenuItem.Size = new System.Drawing.Size(227, 26);
            this.sauvolaToolStripMenuItem.Text = "Метод Сауволы";
            this.sauvolaToolStripMenuItem.Click += new System.EventHandler(this.BinarizationMethodSelected);
            // 
            // wolfToolStripMenuItem
            // 
            this.wolfToolStripMenuItem.Name = "wolfToolStripMenuItem";
            this.wolfToolStripMenuItem.Size = new System.Drawing.Size(227, 26);
            this.wolfToolStripMenuItem.Text = "Метод Вульфа";
            this.wolfToolStripMenuItem.Click += new System.EventHandler(this.BinarizationMethodSelected);
            // 
            // bradleyRothToolStripMenuItem
            // 
            this.bradleyRothToolStripMenuItem.Name = "bradleyRothToolStripMenuItem";
            this.bradleyRothToolStripMenuItem.Size = new System.Drawing.Size(227, 26);
            this.bradleyRothToolStripMenuItem.Text = "Метод Брэдли-Рота";
            this.bradleyRothToolStripMenuItem.Click += new System.EventHandler(this.BinarizationMethodSelected);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(65, 415);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1008, 23);
            this.progressBar1.TabIndex = 5;
            this.progressBar1.Visible = false;
            // 
            // progressLabel
            // 
            this.progressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(971, 390);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(102, 16);
            this.progressLabel.TabIndex = 6;
            this.progressLabel.Text = "0% завершено";
            this.progressLabel.Visible = false;
            // 
            // groupBoxSelectedOperation
            // 
            this.groupBoxSelectedOperation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSelectedOperation.Controls.Add(this.lblSelectedOperation);
            this.groupBoxSelectedOperation.Location = new System.Drawing.Point(65, 455);
            this.groupBoxSelectedOperation.Name = "groupBoxSelectedOperation";
            this.groupBoxSelectedOperation.Size = new System.Drawing.Size(400, 70);
            this.groupBoxSelectedOperation.TabIndex = 7;
            this.groupBoxSelectedOperation.TabStop = false;
            this.groupBoxSelectedOperation.Text = "Выбранная операция";
            // 
            // lblSelectedOperation
            // 
            this.lblSelectedOperation.AutoSize = true;
            this.lblSelectedOperation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSelectedOperation.ForeColor = System.Drawing.Color.Gray;
            this.lblSelectedOperation.Location = new System.Drawing.Point(15, 30);
            this.lblSelectedOperation.Name = "lblSelectedOperation";
            this.lblSelectedOperation.Size = new System.Drawing.Size(213, 20);
            this.lblSelectedOperation.TabIndex = 0;
            this.lblSelectedOperation.Text = "Операция не выбрана";
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(65, 531);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(138, 50);
            this.btnStart.TabIndex = 8;
            this.btnStart.Text = "Начать";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // groupBoxMaskSettings
            // 
            this.groupBoxMaskSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMaskSettings.Controls.Add(this.radioCircle);
            this.groupBoxMaskSettings.Controls.Add(this.radioSquare);
            this.groupBoxMaskSettings.Controls.Add(this.radioRectangle);
            this.groupBoxMaskSettings.Controls.Add(this.lblMaskWidth);
            this.groupBoxMaskSettings.Controls.Add(this.lblMaskHeight);
            this.groupBoxMaskSettings.Controls.Add(this.nudMaskWidth);
            this.groupBoxMaskSettings.Controls.Add(this.nudMaskHeight);
            this.groupBoxMaskSettings.Location = new System.Drawing.Point(775, 455);
            this.groupBoxMaskSettings.Name = "groupBoxMaskSettings";
            this.groupBoxMaskSettings.Size = new System.Drawing.Size(240, 70);
            this.groupBoxMaskSettings.TabIndex = 9;
            this.groupBoxMaskSettings.TabStop = false;
            this.groupBoxMaskSettings.Text = "Настройки маски";
            this.groupBoxMaskSettings.Visible = false;
            // 
            // radioCircle
            // 
            this.radioCircle.AutoSize = true;
            this.radioCircle.Location = new System.Drawing.Point(6, 15);
            this.radioCircle.Name = "radioCircle";
            this.radioCircle.Size = new System.Drawing.Size(52, 17);
            this.radioCircle.TabIndex = 0;
            this.radioCircle.TabStop = true;
            this.radioCircle.Text = "Круг";
            this.radioCircle.UseVisualStyleBackColor = true;
            this.radioCircle.CheckedChanged += new System.EventHandler(this.MaskShapeChanged);
            // 
            // radioSquare
            // 
            this.radioSquare.AutoSize = true;
            this.radioSquare.Location = new System.Drawing.Point(60, 15);
            this.radioSquare.Name = "radioSquare";
            this.radioSquare.Size = new System.Drawing.Size(64, 17);
            this.radioSquare.TabIndex = 1;
            this.radioSquare.TabStop = true;
            this.radioSquare.Text = "Квадрат";
            this.radioSquare.UseVisualStyleBackColor = true;
            this.radioSquare.CheckedChanged += new System.EventHandler(this.MaskShapeChanged);
            // 
            // radioRectangle
            // 
            this.radioRectangle.AutoSize = true;
            this.radioRectangle.Location = new System.Drawing.Point(126, 15);
            this.radioRectangle.Name = "radioRectangle";
            this.radioRectangle.Size = new System.Drawing.Size(90, 17);
            this.radioRectangle.TabIndex = 2;
            this.radioRectangle.TabStop = true;
            this.radioRectangle.Text = "Прямоуг.";
            this.radioRectangle.UseVisualStyleBackColor = true;
            this.radioRectangle.CheckedChanged += new System.EventHandler(this.MaskShapeChanged);
            // 
            // lblMaskWidth
            // 
            this.lblMaskWidth.AutoSize = true;
            this.lblMaskWidth.Location = new System.Drawing.Point(6, 40);
            this.lblMaskWidth.Name = "lblMaskWidth";
            this.lblMaskWidth.Size = new System.Drawing.Size(20, 13);
            this.lblMaskWidth.TabIndex = 3;
            this.lblMaskWidth.Text = "Ш:";
            // 
            // lblMaskHeight
            // 
            this.lblMaskHeight.AutoSize = true;
            this.lblMaskHeight.Location = new System.Drawing.Point(120, 40);
            this.lblMaskHeight.Name = "lblMaskHeight";
            this.lblMaskHeight.Size = new System.Drawing.Size(18, 13);
            this.lblMaskHeight.TabIndex = 4;
            this.lblMaskHeight.Text = "В:";
            // 
            // nudMaskWidth
            // 
            this.nudMaskWidth.Location = new System.Drawing.Point(28, 38);
            this.nudMaskWidth.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudMaskWidth.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudMaskWidth.Name = "nudMaskWidth";
            this.nudMaskWidth.Size = new System.Drawing.Size(80, 20);
            this.nudMaskWidth.TabIndex = 5;
            this.nudMaskWidth.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // nudMaskHeight
            // 
            this.nudMaskHeight.Location = new System.Drawing.Point(138, 38);
            this.nudMaskHeight.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudMaskHeight.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudMaskHeight.Name = "nudMaskHeight";
            this.nudMaskHeight.Size = new System.Drawing.Size(80, 20);
            this.nudMaskHeight.TabIndex = 6;
            this.nudMaskHeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // groupBoxChannels
            // 
            this.groupBoxChannels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxChannels.Controls.Add(this.radioRGB);
            this.groupBoxChannels.Controls.Add(this.radioRG);
            this.groupBoxChannels.Controls.Add(this.radioRB);
            this.groupBoxChannels.Controls.Add(this.radioGB);
            this.groupBoxChannels.Controls.Add(this.chkR);
            this.groupBoxChannels.Controls.Add(this.chkG);
            this.groupBoxChannels.Controls.Add(this.chkB);
            this.groupBoxChannels.Location = new System.Drawing.Point(480, 455);
            this.groupBoxChannels.Name = "groupBoxChannels";
            this.groupBoxChannels.Size = new System.Drawing.Size(280, 90);
            this.groupBoxChannels.TabIndex = 10;
            this.groupBoxChannels.TabStop = false;
            this.groupBoxChannels.Text = "Цветовые каналы";
            // 
            // radioRGB
            // 
            this.radioRGB.AutoSize = true;
            this.radioRGB.Location = new System.Drawing.Point(10, 20);
            this.radioRGB.Name = "radioRGB";
            this.radioRGB.Size = new System.Drawing.Size(57, 20);
            this.radioRGB.TabIndex = 0;
            this.radioRGB.TabStop = true;
            this.radioRGB.Text = "RGB";
            this.radioRGB.UseVisualStyleBackColor = true;
            this.radioRGB.CheckedChanged += new System.EventHandler(this.ChannelsChanged);
            // 
            // radioRG
            // 
            this.radioRG.AutoSize = true;
            this.radioRG.Location = new System.Drawing.Point(70, 20);
            this.radioRG.Name = "radioRG";
            this.radioRG.Size = new System.Drawing.Size(48, 20);
            this.radioRG.TabIndex = 1;
            this.radioRG.TabStop = true;
            this.radioRG.Text = "RG";
            this.radioRG.UseVisualStyleBackColor = true;
            this.radioRG.CheckedChanged += new System.EventHandler(this.ChannelsChanged);
            // 
            // radioRB
            // 
            this.radioRB.AutoSize = true;
            this.radioRB.Location = new System.Drawing.Point(125, 20);
            this.radioRB.Name = "radioRB";
            this.radioRB.Size = new System.Drawing.Size(47, 20);
            this.radioRB.TabIndex = 2;
            this.radioRB.TabStop = true;
            this.radioRB.Text = "RB";
            this.radioRB.UseVisualStyleBackColor = true;
            this.radioRB.CheckedChanged += new System.EventHandler(this.ChannelsChanged);
            // 
            // radioGB
            // 
            this.radioGB.AutoSize = true;
            this.radioGB.Location = new System.Drawing.Point(180, 20);
            this.radioGB.Name = "radioGB";
            this.radioGB.Size = new System.Drawing.Size(47, 20);
            this.radioGB.TabIndex = 3;
            this.radioGB.TabStop = true;
            this.radioGB.Text = "GB";
            this.radioGB.UseVisualStyleBackColor = true;
            this.radioGB.CheckedChanged += new System.EventHandler(this.ChannelsChanged);
            // 
            // chkR
            // 
            this.chkR.AutoSize = true;
            this.chkR.Location = new System.Drawing.Point(10, 50);
            this.chkR.Name = "chkR";
            this.chkR.Size = new System.Drawing.Size(39, 20);
            this.chkR.TabIndex = 4;
            this.chkR.Text = "R";
            this.chkR.UseVisualStyleBackColor = true;
            this.chkR.CheckedChanged += new System.EventHandler(this.ChannelCheckChanged);
            // 
            // chkG
            // 
            this.chkG.AutoSize = true;
            this.chkG.Location = new System.Drawing.Point(70, 50);
            this.chkG.Name = "chkG";
            this.chkG.Size = new System.Drawing.Size(39, 20);
            this.chkG.TabIndex = 5;
            this.chkG.Text = "G";
            this.chkG.UseVisualStyleBackColor = true;
            this.chkG.CheckedChanged += new System.EventHandler(this.ChannelCheckChanged);
            // 
            // chkB
            // 
            this.chkB.AutoSize = true;
            this.chkB.Location = new System.Drawing.Point(130, 50);
            this.chkB.Name = "chkB";
            this.chkB.Size = new System.Drawing.Size(38, 20);
            this.chkB.TabIndex = 6;
            this.chkB.Text = "B";
            this.chkB.UseVisualStyleBackColor = true;
            this.chkB.CheckedChanged += new System.EventHandler(this.ChannelCheckChanged);
            // 
            // groupBoxBinarizationImage
            // 
            this.groupBoxBinarizationImage = new System.Windows.Forms.GroupBox();
            this.comboBinarizationImage = new System.Windows.Forms.ComboBox();
            this.lblSelectedBinarizationMethod = new System.Windows.Forms.Label();
            this.btnApplyBinarization = new System.Windows.Forms.Button();
            this.btnApplyToView = new System.Windows.Forms.Button();
            this.btnCancelBinarization = new System.Windows.Forms.Button();
            this.groupBoxBinarizationImage.SuspendLayout();

            // groupBoxBinarizationImage
            // 
            this.groupBoxBinarizationImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxBinarizationImage.Controls.Add(this.comboBinarizationImage);
            this.groupBoxBinarizationImage.Controls.Add(this.lblSelectedBinarizationMethod);
            this.groupBoxBinarizationImage.Controls.Add(this.btnApplyToView);
            this.groupBoxBinarizationImage.Controls.Add(this.btnApplyBinarization);
            this.groupBoxBinarizationImage.Controls.Add(this.btnCancelBinarization);
            this.groupBoxBinarizationImage.Location = new System.Drawing.Point(220, 531);
            this.groupBoxBinarizationImage.Name = "groupBoxBinarizationImage";
            this.groupBoxBinarizationImage.Size = new System.Drawing.Size(520, 50);
            this.groupBoxBinarizationImage.TabIndex = 11;
            this.groupBoxBinarizationImage.TabStop = false;
            this.groupBoxBinarizationImage.Text = "Изображение для бинаризации";
            this.groupBoxBinarizationImage.Visible = false;

            // 
            // comboBinarizationImage
            // 
            this.comboBinarizationImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBinarizationImage.FormattingEnabled = true;
            this.comboBinarizationImage.Items.AddRange(new object[] {
                "Изображение 1",
                "Изображение 2"});
            this.comboBinarizationImage.Location = new System.Drawing.Point(10, 20);
            this.comboBinarizationImage.Name = "comboBinarizationImage";
            this.comboBinarizationImage.Size = new System.Drawing.Size(130, 24);
            this.comboBinarizationImage.TabIndex = 0;
            this.comboBinarizationImage.SelectedIndexChanged += new System.EventHandler(this.BinarizationImageSelectionChanged);

            // 
            // lblSelectedBinarizationMethod
            // 
            this.lblSelectedBinarizationMethod.AutoSize = true;
            this.lblSelectedBinarizationMethod.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSelectedBinarizationMethod.ForeColor = System.Drawing.Color.Gray;
            this.lblSelectedBinarizationMethod.Location = new System.Drawing.Point(150, 23);
            this.lblSelectedBinarizationMethod.Name = "lblSelectedBinarizationMethod";
            this.lblSelectedBinarizationMethod.Size = new System.Drawing.Size(60, 17);
            this.lblSelectedBinarizationMethod.TabIndex = 1;
            this.lblSelectedBinarizationMethod.Text = "Метод не выбран";

            // 
            // btnApplyToView
            // 
            this.btnApplyToView.Enabled = false;
            this.btnApplyToView.Location = new System.Drawing.Point(270, 17);
            this.btnApplyToView.Name = "btnApplyToView";
            this.btnApplyToView.Size = new System.Drawing.Size(85, 25);
            this.btnApplyToView.TabIndex = 2;
            this.btnApplyToView.Text = "Применить";
            this.btnApplyToView.UseVisualStyleBackColor = true;
            this.btnApplyToView.Click += new System.EventHandler(this.BtnApplyToView_Click);

            // 
            // btnApplyBinarization
            // 
            this.btnApplyBinarization.Enabled = false;
            this.btnApplyBinarization.Location = new System.Drawing.Point(360, 17);
            this.btnApplyBinarization.Name = "btnApplyBinarization";
            this.btnApplyBinarization.Size = new System.Drawing.Size(85, 25);
            this.btnApplyBinarization.TabIndex = 3;
            this.btnApplyBinarization.Text = "Сохранить";
            this.btnApplyBinarization.UseVisualStyleBackColor = true;
            this.btnApplyBinarization.Click += new System.EventHandler(this.BtnApplyBinarization_Click);

            // 
            // btnCancelBinarization
            // 
            this.btnCancelBinarization.Enabled = true;
            this.btnCancelBinarization.Location = new System.Drawing.Point(450, 17);
            this.btnCancelBinarization.Name = "btnCancelBinarization";
            this.btnCancelBinarization.Size = new System.Drawing.Size(60, 25);
            this.btnCancelBinarization.TabIndex = 4;
            this.btnCancelBinarization.Text = "Отмена";
            this.btnCancelBinarization.UseVisualStyleBackColor = true;
            this.btnCancelBinarization.Click += new System.EventHandler(this.BtnCancelBinarization_Click);

            this.groupBoxBinarizationImage.ResumeLayout(false);
            this.groupBoxBinarizationImage.PerformLayout();

            // Добавляем контрол на форму
            this.Controls.Add(this.groupBoxBinarizationImage);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1147, 590);
            this.Controls.Add(this.groupBoxBinarizationImage);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.groupBoxChannels);
            this.Controls.Add(this.groupBoxMaskSettings);
            this.Controls.Add(this.groupBoxSelectedOperation);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.bOpen2);
            this.Controls.Add(this.bOpen1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "Form1";
            this.Text = "Работа с изображениями";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBoxSelectedOperation.ResumeLayout(false);
            this.groupBoxSelectedOperation.PerformLayout();
            this.groupBoxMaskSettings.ResumeLayout(false);
            this.groupBoxMaskSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaskWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaskHeight)).EndInit();
            this.groupBoxChannels.ResumeLayout(false);
            this.groupBoxChannels.PerformLayout();
            this.groupBoxBinarizationImage.ResumeLayout(false);
            this.groupBoxBinarizationImage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}