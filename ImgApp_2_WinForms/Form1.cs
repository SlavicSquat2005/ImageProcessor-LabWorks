using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ImgApp_2_WinForms
{
    public partial class Form1 : Form
    {
        private Bitmap image1 = null;
        private Bitmap image2 = null;

        // Хранилище выбранной операции
        private string selectedOperation = null;
        private Action selectedOperationAction = null;

        // Хранилище параметров маски
        private string maskShape = "Круг";
        private int maskWidth = 100;
        private int maskHeight = 100;

        // Хранилище имени файла
        private string defaultFileName = "out.jpg";

        // Хранилище канала
        private bool[] selectedChannels = new bool[] { true, true, true }; // R, G, B

        public Form1()
        {
            InitializeComponent();

            // Инициализация пустых изображений и каналов
            image1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = image1;

            image2 = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            pictureBox2.Image = image2;

            radioCircle.Checked = true;
            nudMaskWidth.Value = 100;
            nudMaskHeight.Value = 100;

            radioRGB.Checked = true;
            chkR.Checked = true;
            chkG.Checked = true;
            chkB.Checked = true;
            UpdateChannelsFromRadio();
        }

        private void bOpen1_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (image1 != null)
                {
                    pictureBox1.Image = null;
                    image1.Dispose();
                }

                image1 = new Bitmap(openFileDialog.FileName);
                pictureBox1.Image = image1;
            }
        }

        private void bOpen2_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (image2 != null)
                {
                    pictureBox2.Image = null;
                    image2.Dispose();
                }

                image2 = new Bitmap(openFileDialog.FileName);
                pictureBox2.Image = image2;
            }
        }

        private void ChannelsChanged(object sender, EventArgs e)
        {
            UpdateChannelsFromRadio();
        }

        private void ChannelCheckChanged(object sender, EventArgs e)
        {
            radioRGB.Checked = false;
            radioRG.Checked = false;
            radioRB.Checked = false;
            radioGB.Checked = false;

            selectedChannels[0] = chkR.Checked;
            selectedChannels[1] = chkG.Checked;
            selectedChannels[2] = chkB.Checked;
        }

        private void UpdateChannelsFromRadio()
        {
            if (radioRGB.Checked)
            {
                chkR.Checked = chkG.Checked = chkB.Checked = true;
            }
            else if (radioRG.Checked)
            {
                chkR.Checked = chkG.Checked = true;
                chkB.Checked = false;
            }
            else if (radioRB.Checked)
            {
                chkR.Checked = chkB.Checked = true;
                chkG.Checked = false;
            }
            else if (radioGB.Checked)
            {
                chkG.Checked = chkB.Checked = true;
                chkR.Checked = false;
            }

            selectedChannels[0] = chkR.Checked;
            selectedChannels[1] = chkG.Checked;
            selectedChannels[2] = chkB.Checked;
        }

        // Масштабирование изображения
        private Bitmap ResizeImage(Bitmap source, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(source, 0, 0, width, height);
            }
            return result;
        }

        // Применение выбранных каналов к цвету
        private Color ApplyChannels(Color color)
        {
            int r = selectedChannels[0] ? color.R : 0;
            int g = selectedChannels[1] ? color.G : 0;
            int b = selectedChannels[2] ? color.B : 0;
            return Color.FromArgb(r, g, b);
        }

        // Подготовка изображений
        private bool PrepareImagesForOperation(out Bitmap img1, out Bitmap img2, out int width, out int height)
        {
            img1 = null;
            img2 = null;
            width = 0;
            height = 0;

            if (image1 == null || image2 == null ||
                image1.Width == 0 || image1.Height == 0 ||
                image2.Width == 0 || image2.Height == 0)
            {
                MessageBox.Show("Сначала загрузите оба изображения!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            width = Math.Max(image1.Width, image2.Width);
            height = Math.Max(image1.Height, image2.Height);

            img1 = ResizeImage(image1, width, height);
            img2 = ResizeImage(image2, width, height);

            return true;
        }

        private void MaskShapeChanged(object sender, EventArgs e)
        {
            if (radioCircle.Checked)
                maskShape = "Круг";
            else if (radioSquare.Checked)
                maskShape = "Квадрат";
            else if (radioRectangle.Checked)
                maskShape = "Прямоугольник";

            if (maskShape == "Квадрат")
            {
                nudMaskHeight.Value = nudMaskWidth.Value;
                nudMaskHeight.Enabled = false;
            }
            else
            {
                nudMaskHeight.Enabled = true;
            }
        }

        // Список операций
        private void btnSumSelected(object sender, EventArgs e)
        {
            selectedOperation = "Суммирование";
            defaultFileName = "sum.jpg";
            selectedOperationAction = PerformSum;
            groupBoxMaskSettings.Visible = false;
            UpdateSelectedOperationDisplay();
        }

        private void btnAverageSelected(object sender, EventArgs e)
        {
            selectedOperation = "Среднее арифметическое";
            defaultFileName = "average.jpg";
            selectedOperationAction = PerformAverage;
            groupBoxMaskSettings.Visible = false;
            UpdateSelectedOperationDisplay();
        }

        private void btnMaxSelected(object sender, EventArgs e)
        {
            selectedOperation = "Попиксельный максимум";
            defaultFileName = "max.jpg";
            selectedOperationAction = PerformMax;
            groupBoxMaskSettings.Visible = false;
            UpdateSelectedOperationDisplay();
        }

        private void btnMinSelected(object sender, EventArgs e)
        {
            selectedOperation = "Попиксельный минимум";
            defaultFileName = "min.jpg";
            selectedOperationAction = PerformMin;
            groupBoxMaskSettings.Visible = false;
            UpdateSelectedOperationDisplay();
        }

        private void btnProductSelected(object sender, EventArgs e)
        {
            selectedOperation = "Произведение";
            defaultFileName = "product.jpg";
            selectedOperationAction = PerformProduct;
            groupBoxMaskSettings.Visible = false;
            UpdateSelectedOperationDisplay();
        }

        private void btnMaskSelected(object sender, EventArgs e)
        {
            selectedOperation = "Наложение маски";
            defaultFileName = "masked.jpg";
            selectedOperationAction = PerformMask;
            groupBoxMaskSettings.Visible = true;
            UpdateSelectedOperationDisplay();
        }

        private void UpdateSelectedOperationDisplay()
        {
            if (!string.IsNullOrEmpty(selectedOperation))
            {
                lblSelectedOperation.Text = $"{selectedOperation} -> {defaultFileName}";
                lblSelectedOperation.ForeColor = System.Drawing.Color.Black;
                btnStart.Enabled = true;
            }
            else
            {
                lblSelectedOperation.Text = "Операция не выбрана";
                lblSelectedOperation.ForeColor = System.Drawing.Color.Gray;
                btnStart.Enabled = false;
            }
        }

        private string ShowSaveFileDialog(string defaultName)
        {
            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|BMP Image|*.bmp|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = defaultName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return null;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            maskWidth = (int)nudMaskWidth.Value;
            maskHeight = (int)nudMaskHeight.Value;

            selectedChannels[0] = chkR.Checked;
            selectedChannels[1] = chkG.Checked;
            selectedChannels[2] = chkB.Checked;

            if (selectedOperationAction != null)
            {
                selectedOperationAction.Invoke();
            }
        }

        // Линия прогресса
        private void UpdateProgress(int current, int total, string operation)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action<int, int, string>(UpdateProgress), current, total, operation);
                return;
            }

            int percentage = (int)((double)current / total * 100);
            progressBar1.Value = percentage;
            progressLabel.Text = $"{operation}: {percentage}% завершено";

            progressBar1.Refresh();
            progressLabel.Refresh();
            Application.DoEvents();
        }

        private void ShowProgress(string operation)
        {
            progressBar1.Value = 0;
            progressBar1.Visible = true;
            progressLabel.Visible = true;
            progressLabel.Text = $"{operation}: 0% завершено";
            Application.DoEvents();
        }

        private void HideProgress()
        {
            progressBar1.Visible = false;
            progressLabel.Visible = false;
            Application.DoEvents();
        }

        private bool SaveResultWithDialog(Bitmap result, string defaultName)
        {
            string filePath = ShowSaveFileDialog(defaultName);
            if (filePath == null) return false;

            try
            {
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Jpeg;
                string ext = Path.GetExtension(filePath).ToLower();

                switch (ext)
                {
                    case ".png": format = System.Drawing.Imaging.ImageFormat.Png; break;
                    case ".bmp": format = System.Drawing.Imaging.ImageFormat.Bmp; break;
                    case ".gif": format = System.Drawing.Imaging.ImageFormat.Gif; break;
                }

                result.Save(filePath, format);
                MessageBox.Show($"Результат сохранен как: {filePath}", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Операция суммирования
        private void PerformSum()
        {
            if (!PrepareImagesForOperation(out Bitmap img1, out Bitmap img2, out int width, out int height))
                return;

            Bitmap result = null;
            try
            {
                ShowProgress("Суммирование");
                result = new Bitmap(width, height);
                int totalPixels = height * width;
                int processedPixels = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color color1 = img1.GetPixel(x, y);
                        Color color2 = img2.GetPixel(x, y);

                        color1 = ApplyChannels(color1);
                        color2 = ApplyChannels(color2);

                        int r = Math.Min(255, color1.R + color2.R);
                        int g = Math.Min(255, color1.G + color2.G);
                        int b = Math.Min(255, color1.B + color2.B);

                        result.SetPixel(x, y, Color.FromArgb(r, g, b));
                        processedPixels++;

                        if (processedPixels % 1000 == 0)
                            UpdateProgress(processedPixels, totalPixels, "Суммирование");
                    }
                }

                UpdateProgress(totalPixels, totalPixels, "Суммирование");
                SaveResultWithDialog(result, defaultFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                img1?.Dispose();
                img2?.Dispose();
                result?.Dispose();
                HideProgress();
            }
        }

        // Операция усреднения
        private void PerformAverage()
        {
            if (!PrepareImagesForOperation(out Bitmap img1, out Bitmap img2, out int width, out int height))
                return;

            Bitmap result = null;
            try
            {
                ShowProgress("Усреднение");
                result = new Bitmap(width, height);
                int totalPixels = height * width;
                int processedPixels = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color color1 = img1.GetPixel(x, y);
                        Color color2 = img2.GetPixel(x, y);

                        color1 = ApplyChannels(color1);
                        color2 = ApplyChannels(color2);

                        int r = (color1.R + color2.R) / 2;
                        int g = (color1.G + color2.G) / 2;
                        int b = (color1.B + color2.B) / 2;

                        result.SetPixel(x, y, Color.FromArgb(r, g, b));
                        processedPixels++;

                        if (processedPixels % 1000 == 0)
                            UpdateProgress(processedPixels, totalPixels, "Усреднение");
                    }
                }

                UpdateProgress(totalPixels, totalPixels, "Усреднение");
                SaveResultWithDialog(result, defaultFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                img1?.Dispose();
                img2?.Dispose();
                result?.Dispose();
                HideProgress();
            }
        }

        // Операция максимума
        private void PerformMax()
        {
            if (!PrepareImagesForOperation(out Bitmap img1, out Bitmap img2, out int width, out int height))
                return;

            Bitmap result = null;
            try
            {
                ShowProgress("Поиск максимума");
                result = new Bitmap(width, height);
                int totalPixels = height * width;
                int processedPixels = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color color1 = img1.GetPixel(x, y);
                        Color color2 = img2.GetPixel(x, y);

                        color1 = ApplyChannels(color1);
                        color2 = ApplyChannels(color2);

                        int r = Math.Max(color1.R, color2.R);
                        int g = Math.Max(color1.G, color2.G);
                        int b = Math.Max(color1.B, color2.B);

                        result.SetPixel(x, y, Color.FromArgb(r, g, b));
                        processedPixels++;

                        if (processedPixels % 1000 == 0)
                            UpdateProgress(processedPixels, totalPixels, "Поиск максимума");
                    }
                }

                UpdateProgress(totalPixels, totalPixels, "Поиск максимума");
                SaveResultWithDialog(result, defaultFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                img1?.Dispose();
                img2?.Dispose();
                result?.Dispose();
                HideProgress();
            }
        }

        // Операция минимума
        private void PerformMin()
        {
            if (!PrepareImagesForOperation(out Bitmap img1, out Bitmap img2, out int width, out int height))
                return;

            Bitmap result = null;
            try
            {
                ShowProgress("Поиск минимума");
                result = new Bitmap(width, height);
                int totalPixels = height * width;
                int processedPixels = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color color1 = img1.GetPixel(x, y);
                        Color color2 = img2.GetPixel(x, y);

                        color1 = ApplyChannels(color1);
                        color2 = ApplyChannels(color2);

                        int r = Math.Min(color1.R, color2.R);
                        int g = Math.Min(color1.G, color2.G);
                        int b = Math.Min(color1.B, color2.B);

                        result.SetPixel(x, y, Color.FromArgb(r, g, b));
                        processedPixels++;

                        if (processedPixels % 1000 == 0)
                            UpdateProgress(processedPixels, totalPixels, "Поиск минимума");
                    }
                }

                UpdateProgress(totalPixels, totalPixels, "Поиск минимума");
                SaveResultWithDialog(result, defaultFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                img1?.Dispose();
                img2?.Dispose();
                result?.Dispose();
                HideProgress();
            }
        }

        // Операция произведения
        private void PerformProduct()
        {
            if (!PrepareImagesForOperation(out Bitmap img1, out Bitmap img2, out int width, out int height))
                return;

            Bitmap result = null;
            try
            {
                ShowProgress("Произведение");
                result = new Bitmap(width, height);
                int totalPixels = height * width;
                int processedPixels = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color color1 = img1.GetPixel(x, y);
                        Color color2 = img2.GetPixel(x, y);

                        color1 = ApplyChannels(color1);
                        color2 = ApplyChannels(color2);

                        int r = (int)(color1.R * (color2.R / 255.0));
                        int g = (int)(color1.G * (color2.G / 255.0));
                        int b = (int)(color1.B * (color2.B / 255.0));

                        r = Math.Min(255, Math.Max(0, r));
                        g = Math.Min(255, Math.Max(0, g));
                        b = Math.Min(255, Math.Max(0, b));

                        result.SetPixel(x, y, Color.FromArgb(r, g, b));
                        processedPixels++;

                        if (processedPixels % 1000 == 0)
                            UpdateProgress(processedPixels, totalPixels, "Произведение");
                    }
                }

                UpdateProgress(totalPixels, totalPixels, "Произведение");
                SaveResultWithDialog(result, defaultFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                img1?.Dispose();
                img2?.Dispose();
                result?.Dispose();
                HideProgress();
            }
        }

        // Наложение маски
        private void PerformMask()
        {
            if (image1 == null || image1.Width == 0 || image1.Height == 0)
            {
                MessageBox.Show("Сначала загрузите изображение!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Bitmap mask = null;
            Bitmap result = null;

            try
            {
                ShowProgress("Наложение маски");

                int width = image1.Width;
                int height = image1.Height;

                mask = new Bitmap(width, height);
                result = new Bitmap(width, height);

                int centerX = width / 2;
                int centerY = height / 2;

                int mWidth = Math.Min(maskWidth, width);
                int mHeight = Math.Min(maskHeight, height);

                // Создание маски
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bool inMask = false;

                        switch (maskShape)
                        {
                            case "Круг":
                                double radius = Math.Min(mWidth, mHeight) / 2.0;
                                double dx = x - centerX;
                                double dy = y - centerY;
                                inMask = (dx * dx + dy * dy) <= (radius * radius);
                                break;

                            case "Квадрат":
                                inMask = (Math.Abs(x - centerX) <= mWidth / 2) &&
                                         (Math.Abs(y - centerY) <= mWidth / 2);
                                break;

                            case "Прямоугольник":
                                inMask = (Math.Abs(x - centerX) <= mWidth / 2) &&
                                         (Math.Abs(y - centerY) <= mHeight / 2);
                                break;
                        }

                        mask.SetPixel(x, y, inMask ? Color.White : Color.Black);
                    }
                }

                // Применение маски с учетом выбранных каналов
                int totalPixels = height * width;
                int processedPixels = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color originalColor = image1.GetPixel(x, y);
                        originalColor = ApplyChannels(originalColor);
                        Color maskColor = mask.GetPixel(x, y);

                        int r = maskColor.R == 255 ? originalColor.R : 0;
                        int g = maskColor.G == 255 ? originalColor.G : 0;
                        int b = maskColor.B == 255 ? originalColor.B : 0;

                        result.SetPixel(x, y, Color.FromArgb(r, g, b));
                        processedPixels++;

                        if (processedPixels % 1000 == 0)
                            UpdateProgress(processedPixels, totalPixels, "Наложение маски");
                    }
                }

                UpdateProgress(totalPixels, totalPixels, "Наложение маски");

                if (SaveResultWithDialog(result, defaultFileName))
                {
                    DialogResult saveMask = MessageBox.Show(
                        "Сохранить также изображение маски?",
                        "Сохранение маски",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (saveMask == DialogResult.Yes)
                    {
                        string maskPath = ShowSaveFileDialog("mask.jpg");
                        if (maskPath != null)
                        {
                            string ext = Path.GetExtension(maskPath).ToLower();
                            System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Jpeg;

                            switch (ext)
                            {
                                case ".png": format = System.Drawing.Imaging.ImageFormat.Png; break;
                                case ".bmp": format = System.Drawing.Imaging.ImageFormat.Bmp; break;
                            }

                            mask.Save(maskPath, format);
                            MessageBox.Show($"Маска сохранена как: {maskPath}", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                mask?.Dispose();
                result?.Dispose();
                HideProgress();
            }
        }

        // Обработчик изменения размера окна
        private void Form1_Resize(object sender, EventArgs e)
        {
            int margin = 65;
            int buttonMargin = 4;
            int spacing = 30;
            int progressBarHeight = 23;
            int progressBarMargin = 15;
            int bottomPanelHeight = 70;
            int bottomPanelMargin = 10;

            int availableWidth = this.ClientSize.Width - (2 * margin) - spacing;
            int pictureWidth = availableWidth / 2;
            int pictureHeight = this.ClientSize.Height - 300;

            pictureBox1.Size = new Size(pictureWidth, pictureHeight);
            pictureBox2.Size = new Size(pictureWidth, pictureHeight);

            pictureBox1.Location = new Point(margin, 38);
            pictureBox2.Location = new Point(margin + pictureWidth + spacing, 38);

            bOpen1.Location = new Point(margin, pictureBox1.Bottom + buttonMargin);
            bOpen1.Size = new Size(pictureWidth, bOpen1.Height);

            bOpen2.Location = new Point(margin + pictureWidth + spacing, pictureBox2.Bottom + buttonMargin);
            bOpen2.Size = new Size(pictureWidth, bOpen2.Height);

            progressBar1.Location = new Point(margin, bOpen1.Bottom + progressBarMargin);
            progressBar1.Size = new Size(this.ClientSize.Width - (2 * margin), progressBarHeight);
            progressLabel.Location = new Point(progressBar1.Right - 150, progressBar1.Top - 20);

            int bottomY = progressBar1.Bottom + bottomPanelMargin;

            groupBoxSelectedOperation.Location = new Point(margin, bottomY);
            groupBoxSelectedOperation.Size = new Size(400, bottomPanelHeight);

            groupBoxChannels.Location = new Point(margin + 420, bottomY);
            groupBoxChannels.Size = new Size(280, bottomPanelHeight + 20);

            groupBoxMaskSettings.Location = new Point(margin + 720, bottomY);
            groupBoxMaskSettings.Size = new Size(290, bottomPanelHeight + 30);

            btnStart.Location = new Point(groupBoxMaskSettings.Right + 10, bottomY + 10);
            btnStart.Size = new Size(138, 60);
        }
    }
}