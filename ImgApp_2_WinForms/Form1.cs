using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgApp_2_WinForms
{
    public partial class Form1 : Form
    {
        private Bitmap image1 = null;
        private Bitmap image2 = null;

        private string selectedOperation = null;
        private Action selectedOperationAction = null;

        private string maskShape = "Круг";
        private int maskWidth = 100;
        private int maskHeight = 100;

        private string defaultFileName = "out.jpg";
        private bool[] selectedChannels = new bool[] { true, true, true };

        private CurveEditorPanel curveEditor;
        private PictureBox histogramBox;
        private ComboBox targetImageSelector;
        private ComboBox interpolationTypeSelector;
        private Bitmap image1Copy = null;
        private Bitmap image2Copy = null;
        private bool isApplyingCurve = false;
        private bool isInitializing = true;

        // Поля для бинаризации
        private string selectedBinarizationMethod = null;
        private int? selectedBinarizationImage = null;
        private const int WINDOW_SIZE = 15;
        private const double K_NIBLACK = -0.2;
        private const double K_SAUVOLA = 0.5;
        private const double K_BRADLEY = 0.15;

        private Bitmap binarizationBackup1 = null;
        private Bitmap binarizationBackup2 = null;

        // Поля для фильтрации (единое окно)
        private string selectedFilterMethod = null;
        private string currentFilterType = null; // "linear", "median", "gaussian"

        private string selectedFreqFilterType = null;
        private int freqDisplayMode = 0; // 0 = окрестность, 1 = Фурье-образ

        public Form1()
        {
            InitializeComponent();

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

            InitializeAdditionalControls();

            // Подписываем обработчики для фильтрации
            this.comboFilterImage.SelectedIndexChanged += ComboFilterImage_SelectedIndexChanged;
            this.btnFilterApply.Click += BtnFilterApply_Click;
            this.btnFilterSave.Click += BtnFilterSave_Click;
            this.btnFilterCancel.Click += BtnFilterCancel_Click;

            isInitializing = false;

            UpdateHistogramForSelectedImage();
            ApplyCurveToSelectedImage();
        }

        private void InitializeAdditionalControls()
        {
            Panel leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 450,
                Padding = new Padding(10)
            };

            curveEditor = new CurveEditorPanel
            {
                Size = new Size(400, 400),
                Location = new Point(10, 10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            curveEditor.CurveChanged += CurveEditor_CurveChanged;
            leftPanel.Controls.Add(curveEditor);

            histogramBox = new PictureBox
            {
                Size = new Size(400, 150),
                Location = new Point(10, 420),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            leftPanel.Controls.Add(histogramBox);

            Panel selectorPanel = new Panel
            {
                Size = new Size(400, 35),
                Location = new Point(10, 580)
            };

            targetImageSelector = new ComboBox
            {
                Size = new Size(140, 30),
                Location = new Point(0, 0),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            targetImageSelector.Items.AddRange(new string[] { "Изображение 1", "Изображение 2" });
            targetImageSelector.SelectedIndex = 0;
            targetImageSelector.SelectedIndexChanged += TargetImageSelector_SelectedIndexChanged;
            selectorPanel.Controls.Add(targetImageSelector);

            interpolationTypeSelector = new ComboBox
            {
                Size = new Size(140, 30),
                Location = new Point(150, 0),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            interpolationTypeSelector.Items.AddRange(new string[] { "Линейная", "B-сплайн" });
            interpolationTypeSelector.SelectedIndex = 0;
            interpolationTypeSelector.SelectedIndexChanged += InterpolationTypeSelector_SelectedIndexChanged;
            selectorPanel.Controls.Add(interpolationTypeSelector);

            leftPanel.Controls.Add(selectorPanel);
            this.Controls.Add(leftPanel);

            int shift = leftPanel.Width;

            pictureBox1.Location = new Point(pictureBox1.Location.X + shift, pictureBox1.Location.Y);
            pictureBox2.Location = new Point(pictureBox2.Location.X + shift, pictureBox2.Location.Y);
            bOpen1.Location = new Point(bOpen1.Location.X + shift, bOpen1.Location.Y);
            bOpen2.Location = new Point(bOpen2.Location.X + shift, bOpen2.Location.Y);

            Button resetButton1 = new Button
            {
                Text = "✖",
                Size = new Size(30, 30),
                Location = new Point(bOpen1.Right - 30, bOpen1.Top),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGray,
                ForeColor = Color.Red,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold),
                Tag = 1,
                Cursor = Cursors.Hand
            };
            resetButton1.Click += ResetButton_Click;
            this.Controls.Add(resetButton1);

            Button resetButton2 = new Button
            {
                Text = "✖",
                Size = new Size(30, 30),
                Location = new Point(bOpen2.Right - 30, bOpen2.Top),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGray,
                ForeColor = Color.Red,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold),
                Tag = 2,
                Cursor = Cursors.Hand
            };
            resetButton2.Click += ResetButton_Click;
            this.Controls.Add(resetButton2);

            progressBar1.Location = new Point(progressBar1.Location.X + shift, progressBar1.Location.Y);
            progressLabel.Location = new Point(progressLabel.Location.X + shift, progressLabel.Location.Y);
            groupBoxSelectedOperation.Location = new Point(groupBoxSelectedOperation.Location.X + shift, groupBoxSelectedOperation.Location.Y);
            groupBoxChannels.Location = new Point(groupBoxChannels.Location.X + shift, groupBoxChannels.Location.Y);
            groupBoxMaskSettings.Location = new Point(groupBoxMaskSettings.Location.X + shift, groupBoxMaskSettings.Location.Y);

            btnStart.Location = new Point(groupBoxSelectedOperation.Location.X + 420, groupBoxSelectedOperation.Location.Y + groupBoxSelectedOperation.Height + 10);
            btnStart.Size = new Size(138, 40);
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Tag.ToString() == "1")
            {
                if (image1Copy != null) image1Copy.Dispose();
                image1Copy = null;
                image1?.Dispose();
                image1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image = image1;
            }
            else
            {
                if (image2Copy != null) image2Copy.Dispose();
                image2Copy = null;
                image2?.Dispose();
                image2 = new Bitmap(pictureBox2.Width, pictureBox2.Height);
                pictureBox2.Image = image2;
            }

            UpdateHistogramForSelectedImage();
            UpdateApplyButtonState();

            if (selectedBinarizationImage.HasValue)
            {
                var img = selectedBinarizationImage == 1 ? image1 : image2;
                if (img == null || img.Width == 0 || img.Height == 0)
                {
                    comboBinarizationImage.SelectedIndex = -1;
                    selectedBinarizationImage = null;
                }
            }
        }

        private void InterpolationTypeSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isBSpline = interpolationTypeSelector.SelectedIndex == 1;
            curveEditor.SetInterpolationType(isBSpline);
        }

        private void CurveEditor_CurveChanged(object sender, EventArgs e)
        {
            if (!isInitializing && !isApplyingCurve)
            {
                ApplyCurveToSelectedImage();
            }
        }

        private void TargetImageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                UpdateHistogramForSelectedImage();
            }
        }

        private void ApplyCurveToSelectedImage()
        {
            if (isInitializing) return;

            isApplyingCurve = true;
            try
            {
                Bitmap targetImage = null;

                if (targetImageSelector != null && targetImageSelector.SelectedIndex == 0)
                    targetImage = image1;
                else
                    targetImage = image2;

                if (targetImage == null || targetImage.Width == 0 || targetImage.Height == 0)
                {
                    return;
                }

                byte[] table = curveEditor.GetLookupTable();

                ImageData data = LoadImageData(targetImage);
                byte[] resultData = new byte[data.Data.Length];
                Array.Copy(data.Data, resultData, data.Data.Length);

                int stride = data.Stride;
                int height = data.Height;
                int width = data.Width;

                Parallel.For(0, height, y =>
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int offset = rowOffset + x * 4;
                        if (selectedChannels[0]) resultData[offset] = table[data.Data[offset]];
                        if (selectedChannels[1]) resultData[offset + 1] = table[data.Data[offset + 1]];
                        if (selectedChannels[2]) resultData[offset + 2] = table[data.Data[offset + 2]];
                    }
                });

                Bitmap resultBitmap = CreateEmptyBitmap(width, height);
                SaveImageData(resultBitmap, new ImageData { Data = resultData, Stride = stride, Width = width, Height = height });

                if (targetImageSelector != null && targetImageSelector.SelectedIndex == 0)
                {
                    if (image1Copy != null) image1Copy.Dispose();
                    image1Copy = resultBitmap;
                    pictureBox1.Image = image1Copy;
                }
                else
                {
                    if (image2Copy != null) image2Copy.Dispose();
                    image2Copy = resultBitmap;
                    pictureBox2.Image = image2Copy;
                }

                data.Dispose();
                UpdateHistogramForSelectedImage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при применении кривой: {ex.Message}");
            }
            finally
            {
                isApplyingCurve = false;
            }
        }

        private void UpdateHistogramForSelectedImage()
        {
            if (isInitializing) return;

            Bitmap targetImage = null;

            if (targetImageSelector != null && targetImageSelector.SelectedIndex == 0)
                targetImage = (image1Copy ?? image1);
            else
                targetImage = (image2Copy ?? image2);

            if (targetImage == null || targetImage.Width == 0) return;

            ImageData data = LoadImageData(targetImage);
            int[] histogram = new int[256];
            int stride = data.Stride;
            int height = data.Height;
            int width = data.Width;

            Parallel.For(0, height, y =>
            {
                int rowOffset = y * stride;
                for (int x = 0; x < width; x++)
                {
                    int offset = rowOffset + x * 4;
                    int brightness = (data.Data[offset] + data.Data[offset + 1] + data.Data[offset + 2]) / 3;
                    System.Threading.Interlocked.Increment(ref histogram[brightness]);
                }
            });

            data.Dispose();

            int histWidth = 256;
            int histHeight = 150;
            Bitmap histBitmap = new Bitmap(histWidth, histHeight);
            using (Graphics g = Graphics.FromImage(histBitmap))
            {
                g.Clear(Color.White);
                int maxVal = histogram.Max();
                if (maxVal > 0)
                {
                    for (int i = 0; i < 256; i++)
                    {
                        int heightBar = (int)((double)histogram[i] / maxVal * histHeight);
                        g.DrawLine(Pens.Black, i, histHeight, i, histHeight - heightBar);
                    }
                }
                g.DrawRectangle(Pens.Gray, 0, 0, histWidth - 1, histHeight - 1);
            }

            if (histogramBox != null && histogramBox.Image != null)
                histogramBox.Image.Dispose();
            if (histogramBox != null)
                histogramBox.Image = histBitmap;
        }

        private ImageData LoadImageData(Bitmap bitmap)
        {
            if (bitmap == null) return null;

            var imageData = new ImageData
            {
                Width = bitmap.Width,
                Height = bitmap.Height
            };

            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            imageData.Stride = bmpData.Stride;
            int byteCount = imageData.Stride * bitmap.Height;
            imageData.Data = new byte[byteCount];

            Marshal.Copy(bmpData.Scan0, imageData.Data, 0, byteCount);
            bitmap.UnlockBits(bmpData);

            return imageData;
        }

        private void SaveImageData(Bitmap bitmap, ImageData imageData)
        {
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            Marshal.Copy(imageData.Data, 0, bmpData.Scan0, imageData.Data.Length);
            bitmap.UnlockBits(bmpData);
        }

        private Bitmap CreateEmptyBitmap(int width, int height)
        {
            return new Bitmap(width, height, PixelFormat.Format32bppArgb);
        }

        private ImageData CreateEmptyImageData(int width, int height)
        {
            int stride = width * 4;
            int byteCount = stride * height;

            return new ImageData
            {
                Width = width,
                Height = height,
                Stride = stride,
                Data = new byte[byteCount]
            };
        }

        private bool PrepareImagesForOperation(out ImageData data1, out ImageData data2, out int width, out int height)
        {
            data1 = null;
            data2 = null;
            width = 0;
            height = 0;

            var img1 = image1Copy ?? image1;
            var img2 = image2Copy ?? image2;

            if (img1 == null || img2 == null ||
                img1.Width == 0 || img1.Height == 0 ||
                img2.Width == 0 || img2.Height == 0)
            {
                MessageBox.Show("Сначала загрузите оба изображения!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            width = Math.Max(img1.Width, img2.Width);
            height = Math.Max(img1.Height, img2.Height);

            var img1Data = LoadImageData(img1);
            var img2Data = LoadImageData(img2);

            if (img1Data.Width != width || img1Data.Height != height)
                data1 = ResizeImageData(img1Data, width, height);
            else
                data1 = img1Data;

            if (img2Data.Width != width || img2Data.Height != height)
                data2 = ResizeImageData(img2Data, width, height);
            else
                data2 = img2Data;

            if (data1 != img1Data) img1Data?.Dispose();
            if (data2 != img2Data) img2Data?.Dispose();

            return true;
        }

        private ImageData ResizeImageData(ImageData source, int newWidth, int newHeight)
        {
            using (var tempBitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb))
            {
                BitmapData bmpData = tempBitmap.LockBits(
                    new Rectangle(0, 0, source.Width, source.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);
                Marshal.Copy(source.Data, 0, bmpData.Scan0, source.Data.Length);
                tempBitmap.UnlockBits(bmpData);

                using (var resizedBitmap = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb))
                using (Graphics g = Graphics.FromImage(resizedBitmap))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(tempBitmap, 0, 0, newWidth, newHeight);
                    return LoadImageData(resizedBitmap);
                }
            }
        }

        private void PerformOperation(string operationName, Func<byte, byte, byte> pixelOperation)
        {
            if (!PrepareImagesForOperation(out ImageData img1, out ImageData img2, out int width, out int height))
                return;

            ImageData result = null;
            try
            {
                ShowProgress(operationName);
                result = CreateEmptyImageData(width, height);

                int totalPixels = width * height;
                int processedPixels = 0;

                int stride = result.Stride;
                byte[] data1 = img1.Data;
                byte[] data2 = img2.Data;
                byte[] dataResult = result.Data;

                for (int i = 3; i < dataResult.Length; i += 4)
                {
                    dataResult[i] = 255;
                }

                for (int y = 0; y < height; y++)
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int offset = rowOffset + (x * 4);

                        dataResult[offset] = pixelOperation(data1[offset], data2[offset]);
                        dataResult[offset + 1] = pixelOperation(data1[offset + 1], data2[offset + 1]);
                        dataResult[offset + 2] = pixelOperation(data1[offset + 2], data2[offset + 2]);

                        processedPixels++;

                        if (processedPixels % 5000 == 0)
                            UpdateProgress(processedPixels, totalPixels, operationName);
                    }
                }

                for (int i = 0; i < dataResult.Length; i += 4)
                {
                    if (!selectedChannels[2]) dataResult[i + 2] = 0;
                    if (!selectedChannels[1]) dataResult[i + 1] = 0;
                    if (!selectedChannels[0]) dataResult[i] = 0;
                }

                UpdateProgress(totalPixels, totalPixels, operationName);

                using (Bitmap resultBitmap = CreateEmptyBitmap(width, height))
                {
                    SaveImageData(resultBitmap, result);
                    SaveResultWithDialog(resultBitmap, defaultFileName);
                }
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

        private void PerformSum() => PerformOperation("Суммирование", (a, b) => (byte)Math.Min(255, a + b));
        private void PerformAverage() => PerformOperation("Усреднение", (a, b) => (byte)((a + b) / 2));
        private void PerformMax() => PerformOperation("Поиск максимума", (a, b) => (byte)Math.Max(a, b));
        private void PerformMin() => PerformOperation("Поиск минимума", (a, b) => (byte)Math.Min(a, b));
        private void PerformProduct() => PerformOperation("Произведение", (a, b) => (byte)((a * b) / 255));

        private void PerformMask()
        {
            var img = image1Copy ?? image1;

            if (img == null || img.Width == 0 || img.Height == 0)
            {
                MessageBox.Show("Сначала загрузите изображение!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ImageData imgData = null;
            ImageData result = null;
            ImageData maskData = null;

            try
            {
                ShowProgress("Наложение маски");

                int width = img.Width;
                int height = img.Height;

                imgData = LoadImageData(img);
                result = CreateEmptyImageData(width, height);
                maskData = CreateEmptyImageData(width, height);

                int centerX = width / 2;
                int centerY = height / 2;
                int mWidth = Math.Min(maskWidth, width);
                int mHeight = Math.Min(maskHeight, height);

                int stride = maskData.Stride;
                byte[] maskBytes = maskData.Data;
                byte[] imgBytes = imgData.Data;
                byte[] resultBytes = result.Data;

                for (int y = 0; y < height; y++)
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int offset = rowOffset + (x * 4);
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

                        byte maskValue = (byte)(inMask ? 255 : 0);
                        maskBytes[offset] = maskValue;
                        maskBytes[offset + 1] = maskValue;
                        maskBytes[offset + 2] = maskValue;
                        maskBytes[offset + 3] = 255;
                    }
                }

                int totalPixels = width * height;
                int processedPixels = 0;

                for (int y = 0; y < height; y++)
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int offset = rowOffset + (x * 4);
                        bool inMask = maskBytes[offset] == 255;

                        resultBytes[offset] = inMask ? imgBytes[offset] : (byte)0;
                        resultBytes[offset + 1] = inMask ? imgBytes[offset + 1] : (byte)0;
                        resultBytes[offset + 2] = inMask ? imgBytes[offset + 2] : (byte)0;
                        resultBytes[offset + 3] = 255;

                        processedPixels++;
                        if (processedPixels % 5000 == 0)
                            UpdateProgress(processedPixels, totalPixels, "Наложение маски");
                    }
                }

                for (int i = 0; i < resultBytes.Length; i += 4)
                {
                    if (!selectedChannels[2]) resultBytes[i + 2] = 0;
                    if (!selectedChannels[1]) resultBytes[i + 1] = 0;
                    if (!selectedChannels[0]) resultBytes[i] = 0;
                }

                UpdateProgress(totalPixels, totalPixels, "Наложение маски");

                using (Bitmap resultBitmap = CreateEmptyBitmap(width, height))
                {
                    SaveImageData(resultBitmap, result);
                    if (SaveResultWithDialog(resultBitmap, defaultFileName))
                    {
                        DialogResult saveMask = MessageBox.Show(
                            "Сохранить также изображение маски?",
                            "Сохранение маски",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (saveMask == DialogResult.Yes)
                        {
                            using (Bitmap maskBitmap = CreateEmptyBitmap(width, height))
                            {
                                SaveImageData(maskBitmap, maskData);
                                string maskPath = ShowSaveFileDialog("mask.jpg");
                                if (maskPath != null)
                                    maskBitmap.Save(maskPath, GetImageFormat(maskPath));
                            }
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
                imgData?.Dispose();
                result?.Dispose();
                maskData?.Dispose();
                HideProgress();
            }
        }

        private ImageFormat GetImageFormat(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return ext switch
            {
                ".png" => ImageFormat.Png,
                ".bmp" => ImageFormat.Bmp,
                ".gif" => ImageFormat.Gif,
                _ => ImageFormat.Jpeg
            };
        }

        private void UpdateProgress(int current, int total, string operation)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action<int, int, string>(UpdateProgress), current, total, operation);
                return;
            }

            int percentage = total == 0 ? 0 : (int)((double)current / total * 100);
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

        private string ShowSaveFileDialog(string defaultName)
        {
            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|BMP Image|*.bmp|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = defaultName;

            return saveFileDialog.ShowDialog() == DialogResult.OK ? saveFileDialog.FileName : null;
        }

        private bool SaveResultWithDialog(Bitmap result, string defaultName)
        {
            string filePath = ShowSaveFileDialog(defaultName);
            if (filePath == null) return false;

            try
            {
                result.Save(filePath, GetImageFormat(filePath));
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

                if (image1Copy != null) image1Copy.Dispose();
                image1Copy = null;
                UpdateHistogramForSelectedImage();
                ApplyCurveToSelectedImage();
                UpdateApplyButtonState();
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

                if (image2Copy != null) image2Copy.Dispose();
                image2Copy = null;
                UpdateHistogramForSelectedImage();
                ApplyCurveToSelectedImage();
                UpdateApplyButtonState();
            }
        }

        private void ChannelsChanged(object sender, EventArgs e) => UpdateChannelsFromRadio();

        private void ChannelCheckChanged(object sender, EventArgs e)
        {
            radioRGB.Checked = false;
            radioRG.Checked = false;
            radioRB.Checked = false;
            radioGB.Checked = false;

            selectedChannels[0] = chkR.Checked;
            selectedChannels[1] = chkG.Checked;
            selectedChannels[2] = chkB.Checked;

            if (!isInitializing)
                ApplyCurveToSelectedImage();
        }

        private void UpdateChannelsFromRadio()
        {
            if (radioRGB.Checked)
                chkR.Checked = chkG.Checked = chkB.Checked = true;
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

            if (!isInitializing)
                ApplyCurveToSelectedImage();
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

        private void btnSumSelected(object sender, EventArgs e)
        {
            if (binarizationBackup1 != null) { binarizationBackup1.Dispose(); binarizationBackup1 = null; }
            if (binarizationBackup2 != null) { binarizationBackup2.Dispose(); binarizationBackup2 = null; }

            groupBoxBinarizationImage.Visible = false;
            groupBoxFilterCommon.Visible = false;
            selectedBinarizationMethod = null;
            selectedBinarizationImage = null;
            selectedFilterMethod = null;
            currentFilterType = null;
            selectedFreqFilterType = null;
            comboBinarizationImage.SelectedIndex = -1;
            lblSelectedBinarizationMethod.Text = "Метод не выбран";
            btnApplyToView.Enabled = false;
            btnApplyBinarization.Enabled = false;
            selectedOperation = "Суммирование";
            defaultFileName = "sum.jpg";
            selectedOperationAction = PerformSum;
            groupBoxMaskSettings.Visible = false;

            lblFreqModeNeighborhood.Visible = false;
            panelFreqSwitch.Visible = false;
            panelFreqKnobLeft.Visible = false;
            panelFreqKnobRight.Visible = false;
            lblFreqModeFourier.Visible = false;

            UpdateSelectedOperationDisplay();
        }

        private void btnAverageSelected(object sender, EventArgs e)
        {
            if (binarizationBackup1 != null) { binarizationBackup1.Dispose(); binarizationBackup1 = null; }
            if (binarizationBackup2 != null) { binarizationBackup2.Dispose(); binarizationBackup2 = null; }

            groupBoxBinarizationImage.Visible = false;
            groupBoxFilterCommon.Visible = false;
            selectedBinarizationMethod = null;
            selectedBinarizationImage = null;
            selectedFilterMethod = null;
            currentFilterType = null;
            selectedFreqFilterType = null;
            comboBinarizationImage.SelectedIndex = -1;
            lblSelectedBinarizationMethod.Text = "Метод не выбран";
            btnApplyToView.Enabled = false;
            btnApplyBinarization.Enabled = false;
            selectedOperation = "Среднее арифметическое";
            defaultFileName = "average.jpg";
            selectedOperationAction = PerformAverage;
            groupBoxMaskSettings.Visible = false;

            lblFreqModeNeighborhood.Visible = false;
            panelFreqSwitch.Visible = false;
            panelFreqKnobLeft.Visible = false;
            panelFreqKnobRight.Visible = false;
            lblFreqModeFourier.Visible = false;

            UpdateSelectedOperationDisplay();
        }

        private void btnMaxSelected(object sender, EventArgs e)
        {
            if (binarizationBackup1 != null) { binarizationBackup1.Dispose(); binarizationBackup1 = null; }
            if (binarizationBackup2 != null) { binarizationBackup2.Dispose(); binarizationBackup2 = null; }

            groupBoxBinarizationImage.Visible = false;
            groupBoxFilterCommon.Visible = false;
            selectedBinarizationMethod = null;
            selectedBinarizationImage = null;
            selectedFilterMethod = null;
            currentFilterType = null;
            selectedFreqFilterType = null;
            comboBinarizationImage.SelectedIndex = -1;
            lblSelectedBinarizationMethod.Text = "Метод не выбран";
            btnApplyToView.Enabled = false;
            btnApplyBinarization.Enabled = false;
            selectedOperation = "Попиксельный максимум";
            defaultFileName = "max.jpg";
            selectedOperationAction = PerformMax;
            groupBoxMaskSettings.Visible = false;

            lblFreqModeNeighborhood.Visible = false;
            panelFreqSwitch.Visible = false;
            panelFreqKnobLeft.Visible = false;
            panelFreqKnobRight.Visible = false;
            lblFreqModeFourier.Visible = false;

            UpdateSelectedOperationDisplay();
        }

        private void btnMinSelected(object sender, EventArgs e)
        {
            if (binarizationBackup1 != null) { binarizationBackup1.Dispose(); binarizationBackup1 = null; }
            if (binarizationBackup2 != null) { binarizationBackup2.Dispose(); binarizationBackup2 = null; }

            groupBoxBinarizationImage.Visible = false;
            groupBoxFilterCommon.Visible = false;
            selectedBinarizationMethod = null;
            selectedBinarizationImage = null;
            selectedFilterMethod = null;
            currentFilterType = null;
            selectedFreqFilterType = null;
            comboBinarizationImage.SelectedIndex = -1;
            lblSelectedBinarizationMethod.Text = "Метод не выбран";
            btnApplyToView.Enabled = false;
            btnApplyBinarization.Enabled = false;
            selectedOperation = "Попиксельный минимум";
            defaultFileName = "min.jpg";
            selectedOperationAction = PerformMin;
            groupBoxMaskSettings.Visible = false;

            lblFreqModeNeighborhood.Visible = false;
            panelFreqSwitch.Visible = false;
            panelFreqKnobLeft.Visible = false;
            panelFreqKnobRight.Visible = false;
            lblFreqModeFourier.Visible = false;

            UpdateSelectedOperationDisplay();
        }

        private void btnProductSelected(object sender, EventArgs e)
        {
            if (binarizationBackup1 != null) { binarizationBackup1.Dispose(); binarizationBackup1 = null; }
            if (binarizationBackup2 != null) { binarizationBackup2.Dispose(); binarizationBackup2 = null; }

            groupBoxBinarizationImage.Visible = false;
            groupBoxFilterCommon.Visible = false;
            selectedBinarizationMethod = null;
            selectedBinarizationImage = null;
            selectedFilterMethod = null;
            currentFilterType = null;
            selectedFreqFilterType = null;
            comboBinarizationImage.SelectedIndex = -1;
            lblSelectedBinarizationMethod.Text = "Метод не выбран";
            btnApplyToView.Enabled = false;
            btnApplyBinarization.Enabled = false;
            selectedOperation = "Произведение";
            defaultFileName = "product.jpg";
            selectedOperationAction = PerformProduct;
            groupBoxMaskSettings.Visible = false;

            lblFreqModeNeighborhood.Visible = false;
            panelFreqSwitch.Visible = false;
            panelFreqKnobLeft.Visible = false;
            panelFreqKnobRight.Visible = false;
            lblFreqModeFourier.Visible = false;

            UpdateSelectedOperationDisplay();
        }

        private void btnMaskSelected(object sender, EventArgs e)
        {
            if (binarizationBackup1 != null) { binarizationBackup1.Dispose(); binarizationBackup1 = null; }
            if (binarizationBackup2 != null) { binarizationBackup2.Dispose(); binarizationBackup2 = null; }

            groupBoxBinarizationImage.Visible = false;
            groupBoxFilterCommon.Visible = false;
            selectedBinarizationMethod = null;
            selectedBinarizationImage = null;
            selectedFilterMethod = null;
            currentFilterType = null;
            selectedFreqFilterType = null;
            comboBinarizationImage.SelectedIndex = -1;
            lblSelectedBinarizationMethod.Text = "Метод не выбран";
            btnApplyToView.Enabled = false;
            btnApplyBinarization.Enabled = false;
            selectedOperation = "Наложение маски";
            defaultFileName = "masked.jpg";
            selectedOperationAction = PerformMask;
            groupBoxMaskSettings.Visible = true;

            lblFreqModeNeighborhood.Visible = false;
            panelFreqSwitch.Visible = false;
            panelFreqKnobLeft.Visible = false;
            panelFreqKnobRight.Visible = false;
            lblFreqModeFourier.Visible = false;

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

        private void btnStart_Click(object sender, EventArgs e)
        {
            maskWidth = (int)nudMaskWidth.Value;
            maskHeight = (int)nudMaskHeight.Value;

            selectedChannels[0] = chkR.Checked;
            selectedChannels[1] = chkG.Checked;
            selectedChannels[2] = chkB.Checked;

            selectedOperationAction?.Invoke();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int margin = 65;
            int buttonMargin = 4;
            int spacing = 30;
            int progressBarHeight = 23;
            int progressBarMargin = 15;
            int bottomPanelHeight = 70;
            int bottomPanelMargin = 10;

            int leftPanelShift = 450;

            int availableWidth = ClientSize.Width - (2 * margin) - spacing - leftPanelShift;
            int pictureWidth = availableWidth / 2;
            int pictureHeight = ClientSize.Height - 350;

            pictureBox1.Size = new Size(pictureWidth, pictureHeight);
            pictureBox2.Size = new Size(pictureWidth, pictureHeight);

            pictureBox1.Location = new Point(margin + leftPanelShift, 38);
            pictureBox2.Location = new Point(margin + pictureWidth + spacing + leftPanelShift, 38);

            bOpen1.Location = new Point(margin + leftPanelShift, pictureBox1.Bottom + buttonMargin);
            bOpen1.Size = new Size(pictureWidth, bOpen1.Height);

            bOpen2.Location = new Point(margin + pictureWidth + spacing + leftPanelShift, pictureBox2.Bottom + buttonMargin);
            bOpen2.Size = new Size(pictureWidth, bOpen2.Height);

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn && btn.Text == "✖")
                {
                    if (btn.Tag.ToString() == "1")
                        btn.Location = new Point(bOpen1.Right - 30, bOpen1.Top);
                    else if (btn.Tag.ToString() == "2")
                        btn.Location = new Point(bOpen2.Right - 30, bOpen2.Top);
                }
            }

            progressBar1.Location = new Point(margin + leftPanelShift, bOpen1.Bottom + progressBarMargin);
            progressBar1.Size = new Size(ClientSize.Width - (2 * margin) - leftPanelShift, progressBarHeight);
            progressLabel.Location = new Point(progressBar1.Right - 150, progressBar1.Top - 20);

            int bottomY = progressBar1.Bottom + bottomPanelMargin;

            int panelSpacing = 15;

            groupBoxSelectedOperation.Location = new Point(margin + leftPanelShift, bottomY);
            groupBoxSelectedOperation.Size = new Size(350, bottomPanelHeight);

            groupBoxChannels.Location = new Point(groupBoxSelectedOperation.Right + panelSpacing, bottomY);
            groupBoxChannels.Size = new Size(230, bottomPanelHeight);

            groupBoxMaskSettings.Location = new Point(groupBoxChannels.Right + panelSpacing, bottomY);
            groupBoxMaskSettings.Size = new Size(220, 70);

            btnStart.Location = new Point(margin + leftPanelShift, bottomY + bottomPanelHeight + 10);
            btnStart.Size = new Size(138, 40);

            groupBoxBinarizationImage.Location = new Point(btnStart.Right + 20, btnStart.Top);
            groupBoxBinarizationImage.Size = new Size(450, 50);

            groupBoxFilterCommon.Location = new Point(btnStart.Right + 20, btnStart.Top);
            groupBoxFilterCommon.Size = new Size(800, 50);

            // Переключатель Окрестность / Фурье-образ
            lblFreqModeNeighborhood.Location = new Point(btnStart.Right + 30, btnStart.Top + 55);
            panelFreqSwitch.Location = new Point(lblFreqModeNeighborhood.Right + 5, btnStart.Top + 56);
            panelFreqKnobLeft.Location = new Point(1, 4);
            panelFreqKnobRight.Location = new Point(panelFreqSwitch.Width - 9, 4);
            lblFreqModeFourier.Location = new Point(panelFreqSwitch.Right + 5, btnStart.Top + 55);
        }

        private void pictureBox2_Click(object sender, EventArgs e) { }

        // МЕТОДЫ БИНАРИЗАЦИИ
        private Bitmap PerformBinarization(Bitmap sourceImage, string method)
        {
            if (sourceImage == null || sourceImage.Width == 0 || sourceImage.Height == 0)
                return null;

            Bitmap result = null;

            switch (method)
            {
                case "Метод Гаврилова":
                    result = BinarizeGavrilov(sourceImage);
                    break;
                case "Метод Отсу":
                    result = BinarizeOtsu(sourceImage);
                    break;
                case "Метод Ниблека":
                    result = BinarizeNiblack(sourceImage);
                    break;
                case "Метод Сауволы":
                    result = BinarizeSauvola(sourceImage);
                    break;
                case "Метод Вульфа":
                    result = BinarizeWolf(sourceImage);
                    break;
                case "Метод Брэдли-Рота":
                    result = BinarizeBradleyRoth(sourceImage);
                    break;
            }

            return result;
        }

        private void BinarizationMethodSelected(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                selectedBinarizationMethod = menuItem.Text;
                groupBoxFilterCommon.Visible = false;
                groupBoxBinarizationImage.Visible = true;
                selectedFreqFilterType = null;

                lblFreqModeNeighborhood.Visible = false;
                panelFreqSwitch.Visible = false;
                panelFreqKnobLeft.Visible = false;
                panelFreqKnobRight.Visible = false;
                lblFreqModeFourier.Visible = false;

                lblSelectedBinarizationMethod.Text = selectedBinarizationMethod;
                lblSelectedBinarizationMethod.ForeColor = System.Drawing.Color.Black;
                UpdateApplyButtonState();
            }
        }

        private void BinarizationImageSelectionChanged(object sender, EventArgs e)
        {
            if (comboBinarizationImage.SelectedIndex == 0)
                selectedBinarizationImage = 1;
            else if (comboBinarizationImage.SelectedIndex == 1)
                selectedBinarizationImage = 2;
            else
                selectedBinarizationImage = null;

            UpdateApplyButtonState();
        }

        private void UpdateApplyButtonState()
        {
            bool methodSelected = !string.IsNullOrEmpty(selectedBinarizationMethod);
            bool imageSelected = selectedBinarizationImage.HasValue;
            bool imageValid = false;

            if (imageSelected)
            {
                var img = selectedBinarizationImage == 1 ?
                    (image1Copy ?? image1) : (image2Copy ?? image2);
                imageValid = img != null && img.Width > 0 && img.Height > 0;
            }

            btnApplyToView.Enabled = methodSelected && imageValid;
            btnApplyBinarization.Enabled = methodSelected && imageValid;

            foreach (var btn in new[] { btnApplyToView, btnApplyBinarization })
            {
                if (!btn.Enabled)
                {
                    btn.BackColor = System.Drawing.Color.LightGray;
                }
                else
                {
                    btn.BackColor = System.Drawing.SystemColors.Control;
                }
            }
        }

        private void BtnApplyToView_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedBinarizationMethod) || !selectedBinarizationImage.HasValue)
                return;

            Bitmap sourceImage = selectedBinarizationImage == 1 ?
                (image1Copy ?? image1) : (image2Copy ?? image2);

            if (sourceImage == null || sourceImage.Width == 0 || sourceImage.Height == 0)
            {
                MessageBox.Show("Выбранное изображение не загружено!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ShowProgress($"Бинаризация ({selectedBinarizationMethod})");

                if (selectedBinarizationImage == 1)
                {
                    if (binarizationBackup1 != null) binarizationBackup1.Dispose();
                    binarizationBackup1 = image1Copy != null ? new Bitmap(image1Copy) : new Bitmap(image1);
                }
                else
                {
                    if (binarizationBackup2 != null) binarizationBackup2.Dispose();
                    binarizationBackup2 = image2Copy != null ? new Bitmap(image2Copy) : new Bitmap(image2);
                }

                Bitmap result = PerformBinarization(sourceImage, selectedBinarizationMethod);

                HideProgress();

                if (result != null)
                {
                    if (selectedBinarizationImage == 1)
                    {
                        if (image1Copy != null) image1Copy.Dispose();
                        image1Copy = result;
                        pictureBox1.Image = image1Copy;
                    }
                    else
                    {
                        if (image2Copy != null) image2Copy.Dispose();
                        image2Copy = result;
                        pictureBox2.Image = image2Copy;
                    }

                    UpdateHistogramForSelectedImage();
                }
            }
            catch (Exception ex)
            {
                HideProgress();
                MessageBox.Show($"Ошибка при бинаризации: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnApplyBinarization_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedBinarizationMethod) || !selectedBinarizationImage.HasValue)
                return;

            Bitmap sourceImage = selectedBinarizationImage == 1 ?
                (image1Copy ?? image1) : (image2Copy ?? image2);

            if (sourceImage == null || sourceImage.Width == 0 || sourceImage.Height == 0)
            {
                MessageBox.Show("Выбранное изображение не загружено!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ShowProgress($"Бинаризация ({selectedBinarizationMethod})");

                Bitmap result = PerformBinarization(sourceImage, selectedBinarizationMethod);

                HideProgress();

                if (result != null)
                {
                    string methodShortName = selectedBinarizationMethod
                        .Replace("Метод ", "")
                        .Replace(" ", "_")
                        .Replace("-", "_")
                        .ToLower();
                    SaveResultWithDialog(result, $"binarized_{methodShortName}.jpg");
                    result.Dispose();
                }
            }
            catch (Exception ex)
            {
                HideProgress();
                MessageBox.Show($"Ошибка при бинаризации: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelBinarization_Click(object sender, EventArgs e)
        {
            if (binarizationBackup1 != null && selectedBinarizationImage == 1)
            {
                if (image1Copy != null) image1Copy.Dispose();
                image1Copy = new Bitmap(binarizationBackup1);
                pictureBox1.Image = image1Copy;
                binarizationBackup1.Dispose();
                binarizationBackup1 = null;
                UpdateHistogramForSelectedImage();
            }

            if (binarizationBackup2 != null && selectedBinarizationImage == 2)
            {
                if (image2Copy != null) image2Copy.Dispose();
                image2Copy = new Bitmap(binarizationBackup2);
                pictureBox2.Image = image2Copy;
                binarizationBackup2.Dispose();
                binarizationBackup2 = null;
                UpdateHistogramForSelectedImage();
            }

            groupBoxBinarizationImage.Visible = false;

            selectedBinarizationMethod = null;
            selectedBinarizationImage = null;
            comboBinarizationImage.SelectedIndex = -1;
            lblSelectedBinarizationMethod.Text = "Метод не выбран";
            lblSelectedBinarizationMethod.ForeColor = System.Drawing.Color.Gray;

            btnApplyToView.Enabled = false;
            btnApplyBinarization.Enabled = false;
            btnApplyToView.BackColor = System.Drawing.Color.LightGray;
            btnApplyBinarization.BackColor = System.Drawing.Color.LightGray;
        }

        private byte[] ConvertToGrayscale(ImageData data)
        {
            byte[] gray = new byte[data.Width * data.Height];
            int stride = data.Stride;

            Parallel.For(0, data.Height, y =>
            {
                int rowOffset = y * stride;
                int grayOffset = y * data.Width;
                for (int x = 0; x < data.Width; x++)
                {
                    int offset = rowOffset + x * 4;
                    byte r = data.Data[offset + 2];
                    byte g = data.Data[offset + 1];
                    byte b = data.Data[offset];
                    gray[grayOffset + x] = (byte)(0.2125 * r + 0.7154 * g + 0.0721 * b);
                }
            });

            return gray;
        }

        private Bitmap CreateBinaryBitmap(byte[] binary, int width, int height)
        {
            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = result.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            int stride = bmpData.Stride;
            byte[] resultData = new byte[stride * height];

            Parallel.For(0, height, y =>
            {
                int rowOffset = y * stride;
                int binOffset = y * width;
                for (int x = 0; x < width; x++)
                {
                    int offset = rowOffset + x * 4;
                    byte value = binary[binOffset + x];
                    resultData[offset] = value;
                    resultData[offset + 1] = value;
                    resultData[offset + 2] = value;
                    resultData[offset + 3] = 255;
                }
            });

            Marshal.Copy(resultData, 0, bmpData.Scan0, resultData.Length);
            result.UnlockBits(bmpData);

            return result;
        }

        private Bitmap BinarizeGavrilov(Bitmap image)
        {
            ImageData data = LoadImageData(image);
            byte[] gray = ConvertToGrayscale(data);

            long sum = 0;
            for (int i = 0; i < gray.Length; i++)
                sum += gray[i];

            byte threshold = (byte)(sum / gray.Length);

            byte[] binary = new byte[gray.Length];
            Parallel.For(0, gray.Length, i =>
            {
                binary[i] = gray[i] > threshold ? (byte)255 : (byte)0;
            });

            data.Dispose();
            return CreateBinaryBitmap(binary, data.Width, data.Height);
        }

        private Bitmap BinarizeOtsu(Bitmap image)
        {
            ImageData data = LoadImageData(image);
            byte[] gray = ConvertToGrayscale(data);

            int[] histogram = new int[256];
            for (int i = 0; i < gray.Length; i++)
                histogram[gray[i]]++;

            double[] normHist = new double[256];
            double totalPixels = gray.Length;
            for (int i = 0; i < 256; i++)
                normHist[i] = histogram[i] / totalPixels;

            double globalMean = 0;
            for (int i = 0; i < 256; i++)
                globalMean += i * normHist[i];

            double maxVariance = 0;
            byte threshold = 128;
            double omega1 = 0;
            double mu1 = 0;

            for (int t = 0; t < 256; t++)
            {
                omega1 += normHist[t];
                if (omega1 == 0) continue;

                double omega2 = 1 - omega1;
                if (omega2 == 0) break;

                mu1 += t * normHist[t];
                double mu2 = (globalMean - mu1) / omega2;

                double variance = omega1 * omega2 * Math.Pow(mu1 / omega1 - mu2, 2);

                if (variance > maxVariance)
                {
                    maxVariance = variance;
                    threshold = (byte)t;
                }
            }

            byte[] binary = new byte[gray.Length];
            Parallel.For(0, gray.Length, i =>
            {
                binary[i] = gray[i] > threshold ? (byte)255 : (byte)0;
            });

            data.Dispose();
            return CreateBinaryBitmap(binary, data.Width, data.Height);
        }

        private Bitmap BinarizeNiblack(Bitmap image)
        {
            ImageData data = LoadImageData(image);
            byte[] gray = ConvertToGrayscale(data);
            int width = data.Width;
            int height = data.Height;
            int halfWindow = WINDOW_SIZE / 2;

            byte[] binary = new byte[gray.Length];

            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int count = 0;
                    long sum = 0;
                    long sumSq = 0;

                    for (int wy = -halfWindow; wy <= halfWindow; wy++)
                    {
                        int ny = y + wy;
                        if (ny < 0 || ny >= height) continue;

                        for (int wx = -halfWindow; wx <= halfWindow; wx++)
                        {
                            int nx = x + wx;
                            if (nx < 0 || nx >= width) continue;

                            byte val = gray[ny * width + nx];
                            sum += val;
                            sumSq += val * val;
                            count++;
                        }
                    }

                    double mean = (double)sum / count;
                    double variance = (double)sumSq / count - mean * mean;
                    double stdDev = Math.Sqrt(variance);

                    double threshold = mean + K_NIBLACK * stdDev;

                    int idx = y * width + x;
                    binary[idx] = gray[idx] > threshold ? (byte)255 : (byte)0;
                }
            });

            data.Dispose();
            return CreateBinaryBitmap(binary, width, height);
        }

        private Bitmap BinarizeSauvola(Bitmap image)
        {
            ImageData data = LoadImageData(image);
            byte[] gray = ConvertToGrayscale(data);
            int width = data.Width;
            int height = data.Height;
            int halfWindow = WINDOW_SIZE / 2;
            const double R = 128.0;

            byte[] binary = new byte[gray.Length];

            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int count = 0;
                    long sum = 0;
                    long sumSq = 0;

                    for (int wy = -halfWindow; wy <= halfWindow; wy++)
                    {
                        int ny = y + wy;
                        if (ny < 0 || ny >= height) continue;

                        for (int wx = -halfWindow; wx <= halfWindow; wx++)
                        {
                            int nx = x + wx;
                            if (nx < 0 || nx >= width) continue;

                            byte val = gray[ny * width + nx];
                            sum += val;
                            sumSq += val * val;
                            count++;
                        }
                    }

                    double mean = (double)sum / count;
                    double variance = (double)sumSq / count - mean * mean;
                    double stdDev = Math.Sqrt(variance);

                    double threshold = mean * (1 + K_SAUVOLA * (stdDev / R - 1));

                    int idx = y * width + x;
                    binary[idx] = gray[idx] > threshold ? (byte)255 : (byte)0;
                }
            });

            data.Dispose();
            return CreateBinaryBitmap(binary, width, height);
        }

        private Bitmap BinarizeWolf(Bitmap image)
        {
            ImageData data = LoadImageData(image);
            byte[] gray = ConvertToGrayscale(data);
            int width = data.Width;
            int height = data.Height;
            int halfWindow = WINDOW_SIZE / 2;
            const double a = 0.5;

            byte minVal = 255;
            for (int i = 0; i < gray.Length; i++)
                if (gray[i] < minVal) minVal = gray[i];

            double[] means = new double[gray.Length];
            double[] stdDevs = new double[gray.Length];
            double maxStdDev = 0;

            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int count = 0;
                    long sum = 0;
                    long sumSq = 0;

                    for (int wy = -halfWindow; wy <= halfWindow; wy++)
                    {
                        int ny = y + wy;
                        if (ny < 0 || ny >= height) continue;

                        for (int wx = -halfWindow; wx <= halfWindow; wx++)
                        {
                            int nx = x + wx;
                            if (nx < 0 || nx >= width) continue;

                            byte val = gray[ny * width + nx];
                            sum += val;
                            sumSq += val * val;
                            count++;
                        }
                    }

                    double mean = (double)sum / count;
                    double variance = (double)sumSq / count - mean * mean;
                    double stdDev = Math.Sqrt(variance);

                    int idx = y * width + x;
                    means[idx] = mean;
                    stdDevs[idx] = stdDev;

                    lock (this)
                    {
                        if (stdDev > maxStdDev) maxStdDev = stdDev;
                    }
                }
            });

            byte[] binary = new byte[gray.Length];
            double R = maxStdDev > 0 ? maxStdDev : 128.0;

            Parallel.For(0, gray.Length, i =>
            {
                double threshold = (1 - a) * means[i] + a * minVal +
                                  a * (stdDevs[i] / R) * (means[i] - minVal);
                binary[i] = gray[i] > threshold ? (byte)255 : (byte)0;
            });

            data.Dispose();
            return CreateBinaryBitmap(binary, width, height);
        }

        private Bitmap BinarizeBradleyRoth(Bitmap image)
        {
            ImageData data = LoadImageData(image);
            byte[] gray = ConvertToGrayscale(data);
            int width = data.Width;
            int height = data.Height;
            int windowSize = WINDOW_SIZE;
            int halfWindow = windowSize / 2;

            long[] integral = new long[width * height];

            for (int y = 0; y < height; y++)
            {
                long rowSum = 0;
                for (int x = 0; x < width; x++)
                {
                    rowSum += gray[y * width + x];
                    int idx = y * width + x;

                    if (y == 0)
                        integral[idx] = rowSum;
                    else
                        integral[idx] = integral[(y - 1) * width + x] + rowSum;
                }
            }

            byte[] binary = new byte[gray.Length];

            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int x1 = Math.Max(0, x - halfWindow);
                    int y1 = Math.Max(0, y - halfWindow);
                    int x2 = Math.Min(width - 1, x + halfWindow);
                    int y2 = Math.Min(height - 1, y + halfWindow);

                    long sum = integral[y2 * width + x2];

                    if (x1 > 0)
                        sum -= integral[y2 * width + (x1 - 1)];
                    if (y1 > 0)
                        sum -= integral[(y1 - 1) * width + x2];
                    if (x1 > 0 && y1 > 0)
                        sum += integral[(y1 - 1) * width + (x1 - 1)];

                    int count = (x2 - x1 + 1) * (y2 - y1 + 1);
                    double mean = (double)sum / count;

                    int idx = y * width + x;
                    double threshold = mean * (1 - K_BRADLEY);

                    binary[idx] = gray[idx] > threshold ? (byte)255 : (byte)0;
                }
            });

            data.Dispose();
            return CreateBinaryBitmap(binary, width, height);
        }

        // МЕТОДЫ ФИЛЬТРАЦИИ (ЕДИНОЕ ОКНО)
        private void FilterTypeSelected(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null) return;

            groupBoxBinarizationImage.Visible = false;

            selectedBinarizationMethod = null;
            selectedBinarizationImage = null;
            selectedFreqFilterType = null;
            comboBinarizationImage.SelectedIndex = -1;
            lblSelectedBinarizationMethod.Text = "Метод не выбран";
            btnApplyToView.Enabled = false;
            btnApplyBinarization.Enabled = false;

            groupBoxFilterCommon.Visible = true;

            lblFreqModeNeighborhood.Visible = false;
            panelFreqSwitch.Visible = false;
            panelFreqKnobLeft.Visible = false;
            panelFreqKnobRight.Visible = false;
            lblFreqModeFourier.Visible = false;

            if (menuItem == linearFilterToolStripMenuItem)
            {
                currentFilterType = "linear";
                selectedFilterMethod = "Линейная фильтрация";
                lblFilterMethod.Text = "Линейная фильтрация";
                lblFilterMethod.ForeColor = System.Drawing.Color.Black;
                groupBoxFilterCommon.Text = "Изображение для фильтрации (Линейная)";

                lblFilterParam1.Text = "Ширина:";
                lblFilterParam2.Text = "Высота:";
                lblFilterParam1.Visible = true;
                nudFilterParam1.Visible = true;
                lblFilterParam2.Visible = true;
                nudFilterParam2.Visible = true;
                nudFilterParam1.Minimum = 1;
                nudFilterParam1.Maximum = 15;
                nudFilterParam1.Value = 3;
                nudFilterParam2.Minimum = 1;
                nudFilterParam2.Maximum = 15;
                nudFilterParam2.Value = 3;
                nudFilterParam1.DecimalPlaces = 0;
            }
            else if (menuItem == medianFilterToolStripMenuItem)
            {
                currentFilterType = "median";
                selectedFilterMethod = "Медианная фильтрация";
                lblFilterMethod.Text = "Медианная фильтрация";
                lblFilterMethod.ForeColor = System.Drawing.Color.Black;
                groupBoxFilterCommon.Text = "Изображение для фильтрации (Медианная)";

                lblFilterParam1.Text = "Окно:";
                lblFilterParam2.Visible = false;
                nudFilterParam2.Visible = false;
                lblFilterParam1.Visible = true;
                nudFilterParam1.Visible = true;
                nudFilterParam1.Minimum = 3;
                nudFilterParam1.Maximum = 15;
                nudFilterParam1.Value = 3;
                nudFilterParam1.DecimalPlaces = 0;
            }
            else if (menuItem == gaussianFilterToolStripMenuItem)
            {
                currentFilterType = "gaussian";
                selectedFilterMethod = "Размытие по Гауссу";
                lblFilterMethod.Text = "Размытие по Гауссу";
                lblFilterMethod.ForeColor = System.Drawing.Color.Black;
                groupBoxFilterCommon.Text = "Изображение для фильтрации (Гаусс)";

                lblFilterParam1.Text = "Сигма, σ:";
                lblFilterParam2.Visible = false;
                nudFilterParam2.Visible = false;
                lblFilterParam1.Visible = true;
                nudFilterParam1.Visible = true;
                nudFilterParam1.Minimum = 1;
                nudFilterParam1.Maximum = 50;
                nudFilterParam1.Value = 3;
                nudFilterParam1.DecimalPlaces = 1;
            }

            btnFilterApply.Enabled = true;
            btnFilterSave.Enabled = true;
            btnFilterApply.BackColor = System.Drawing.SystemColors.Control;
            btnFilterSave.BackColor = System.Drawing.SystemColors.Control;

            if (comboFilterImage.SelectedIndex == -1)
                comboFilterImage.SelectedIndex = 0;
        }

        private void ComboFilterImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFilterButtonState();
        }

        private void UpdateFilterButtonState()
        {
            bool methodSelected = !string.IsNullOrEmpty(selectedFilterMethod);
            bool imageSelected = comboFilterImage.SelectedIndex != -1;
            bool imageValid = false;

            if (imageSelected)
            {
                var img = comboFilterImage.SelectedIndex == 0 ?
                    (image1Copy ?? image1) : (image2Copy ?? image2);
                imageValid = img != null && img.Width > 0 && img.Height > 0;
            }

            btnFilterApply.Enabled = methodSelected && imageValid;
            btnFilterSave.Enabled = methodSelected && imageValid;

            foreach (var btn in new[] { btnFilterApply, btnFilterSave })
            {
                if (!btn.Enabled)
                {
                    btn.BackColor = System.Drawing.Color.LightGray;
                }
                else
                {
                    btn.BackColor = System.Drawing.SystemColors.Control;
                }
            }
        }

        private void BtnFilterApply_Click(object sender, EventArgs e)
        {
            // Проверка, активна ли частотная фильтрация
            if (!string.IsNullOrEmpty(selectedFreqFilterType))
            {
                Application.DoEvents(); // Предотвращает зависание интерфейса
                ShowFreqVisualization();
                Application.DoEvents();
                ApplyFreqFilter();
                return;
            }

            if (string.IsNullOrEmpty(currentFilterType) || comboFilterImage.SelectedIndex == -1)
                return;

            var img = comboFilterImage.SelectedIndex == 0 ?
                (image1Copy ?? image1) : (image2Copy ?? image2);

            if (img == null || img.Width == 0)
            {
                MessageBox.Show("Изображение не загружено!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ShowProgress($"Фильтрация ({selectedFilterMethod})");
                Bitmap result = null;

                if (currentFilterType == "linear")
                {
                    int kw = (int)nudFilterParam1.Value;
                    int kh = (int)nudFilterParam2.Value;
                    if (kw % 2 == 0) kw++; if (kh % 2 == 0) kh++;
                    double[,] kernel = new double[kh, kw];
                    double val = 1.0 / (kw * kh);
                    for (int y = 0; y < kh; y++)
                        for (int x = 0; x < kw; x++)
                            kernel[y, x] = val;
                    result = SpatialFilter.LinearFilter(img, kernel);
                }
                else if (currentFilterType == "median")
                {
                    int ws = (int)nudFilterParam1.Value;
                    if (ws % 2 == 0) ws++;
                    result = SpatialFilter.MedianFilter(img, ws);
                }
                else if (currentFilterType == "gaussian")
                {
                    result = SpatialFilter.GaussianBlur(img, (double)nudFilterParam1.Value);
                }

                HideProgress();
                if (result != null) ApplyToImage(result, comboFilterImage.SelectedIndex);
            }
            catch (Exception ex)
            {
                HideProgress();
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnFilterSave_Click(object sender, EventArgs e)
        {
            // Частотная фильтрация — только сохранить
            if (!string.IsNullOrEmpty(selectedFreqFilterType))
            {
                if (comboFilterImage.SelectedIndex == -1) return;
                var img = comboFilterImage.SelectedIndex == 0 ? (image1Copy ?? image1) : (image2Copy ?? image2);
                if (img == null || img.Width == 0) return;

                string fType = "";
                double p1 = (double)nudFilterParam1.Value;
                double p2 = nudFilterParam2.Visible ? (double)nudFilterParam2.Value : 5.0;
                switch (selectedFreqFilterType)
                {
                    case "Низкочастотный фильтр": fType = "lowpass"; break;
                    case "Высокочастотный фильтр": fType = "highpass"; break;
                    case "Режекторный фильтр": fType = "bandreject"; break;
                    case "Полосовой фильтр": fType = "bandpass"; break;
                    case "Узкополосный режекторный": fType = "notchreject"; break;
                    case "Узкополосный полосовой": fType = "notchpass"; break;
                }

                try
                {
                    ShowProgress("Сохранение...");
                    Bitmap result = FourierTransform.FreqFilterImage(img, fType, p1, p2);
                    HideProgress();
                    if (result != null)
                    {
                        SaveResultWithDialog(result, $"freq_{fType}.jpg");
                        result.Dispose();
                    }
                }
                catch (Exception ex) { HideProgress(); MessageBox.Show($"Ошибка: {ex.Message}"); }
                return;
            }

            // Обычная фильтрация
            if (string.IsNullOrEmpty(currentFilterType) || comboFilterImage.SelectedIndex == -1) return;
            var src = comboFilterImage.SelectedIndex == 0 ? (image1Copy ?? image1) : (image2Copy ?? image2);
            if (src == null || src.Width == 0) return;

            try
            {
                ShowProgress("Сохранение...");
                Bitmap result = null;
                if (currentFilterType == "linear")
                {
                    int kw = (int)nudFilterParam1.Value, kh = (int)nudFilterParam2.Value;
                    if (kw % 2 == 0) kw++; if (kh % 2 == 0) kh++;
                    double[,] k = new double[kh, kw]; double v = 1.0 / (kw * kh);
                    for (int y = 0; y < kh; y++) for (int x = 0; x < kw; x++) k[y, x] = v;
                    result = SpatialFilter.LinearFilter(src, k);
                }
                else if (currentFilterType == "median")
                {
                    int ws = (int)nudFilterParam1.Value; if (ws % 2 == 0) ws++;
                    result = SpatialFilter.MedianFilter(src, ws);
                }
                else if (currentFilterType == "gaussian")
                    result = SpatialFilter.GaussianBlur(src, (double)nudFilterParam1.Value);

                HideProgress();
                if (result != null) { SaveResultWithDialog(result, $"filtered_{currentFilterType}.jpg"); result.Dispose(); }
            }
            catch (Exception ex) { HideProgress(); MessageBox.Show($"Ошибка: {ex.Message}"); }
        }

        private void BtnFilterCancel_Click(object sender, EventArgs e)
        {
            groupBoxFilterCommon.Visible = false;

            selectedFilterMethod = null;
            currentFilterType = null;
            lblFilterMethod.Text = "Метод не выбран";
            lblFilterMethod.ForeColor = System.Drawing.Color.Gray;

            comboFilterImage.SelectedIndex = -1;

            btnFilterApply.Enabled = false;
            btnFilterSave.Enabled = false;
            btnFilterApply.BackColor = System.Drawing.Color.LightGray;
            btnFilterSave.BackColor = System.Drawing.Color.LightGray;

            lblFilterParam1.Visible = false;
            nudFilterParam1.Visible = false;
            lblFilterParam2.Visible = false;
            nudFilterParam2.Visible = false;
        }

        private void ApplyFreqFilter()
        {
            if (string.IsNullOrEmpty(selectedFreqFilterType) || comboFilterImage.SelectedIndex == -1)
                return;

            var img = comboFilterImage.SelectedIndex == 0 ?
                (image1Copy ?? image1) : (image2Copy ?? image2);

            if (img == null || img.Width == 0)
            {
                MessageBox.Show("Изображение не загружено!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Определяем тип фильтра и параметры
            string fType = "";
            double p1 = (double)nudFilterParam1.Value;
            double p2 = nudFilterParam2.Visible ? (double)nudFilterParam2.Value : 5.0;

            switch (selectedFreqFilterType)
            {
                case "Низкочастотный фильтр": fType = "lowpass"; break;
                case "Высокочастотный фильтр": fType = "highpass"; break;
                case "Режекторный фильтр": fType = "bandreject"; break;
                case "Полосовой фильтр": fType = "bandpass"; break;
                case "Узкополосный режекторный": fType = "notchreject"; break;
                case "Узкополосный полосовой": fType = "notchpass"; break;
            }

            try
            {
                // Применяем фильтр
                ShowProgress($"Частотная фильтрация ({selectedFreqFilterType})...");
                Bitmap result = FourierTransform.FreqFilterImage(img, fType, p1, p2);
                HideProgress();

                if (result != null)
                {
                    ApplyToImage(result, comboFilterImage.SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                HideProgress();
                MessageBox.Show($"Ошибка частотной фильтрации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyToImage(Bitmap result, int index)
        {
            if (index == 0)
            {
                if (image1Copy != null) image1Copy.Dispose();
                image1Copy = result;
                pictureBox1.Image = image1Copy;
            }
            else
            {
                if (image2Copy != null) image2Copy.Dispose();
                image2Copy = result;
                pictureBox2.Image = image2Copy;
            }
            UpdateHistogramForSelectedImage();
        }

        private void ShowFreqVisualization()
        {
            if (string.IsNullOrEmpty(selectedFreqFilterType) || comboFilterImage.SelectedIndex == -1)
                return;

            var img = comboFilterImage.SelectedIndex == 0 ?
                (image1Copy ?? image1) : (image2Copy ?? image2);

            if (img == null || img.Width == 0) return;

            string fType = "";
            double p1 = (double)nudFilterParam1.Value;
            double p2 = nudFilterParam2.Visible ? (double)nudFilterParam2.Value : 5.0;

            switch (selectedFreqFilterType)
            {
                case "Низкочастотный фильтр": fType = "lowpass"; break;
                case "Высокочастотный фильтр": fType = "highpass"; break;
                case "Режекторный фильтр": fType = "bandreject"; break;
                case "Полосовой фильтр": fType = "bandpass"; break;
                case "Узкополосный режекторный": fType = "notchreject"; break;
                case "Узкополосный полосовой": fType = "notchpass"; break;
            }

            Bitmap visBmp = null;

            if (freqDisplayMode == 0)
            {
                // Окрестность (маска фильтра)
                visBmp = FourierTransform.VisualizeFreqFilter(img, fType, p1, p2);
            }
            else
            {
                // Фурье-образ
                visBmp = FourierTransform.VisualizeFullFourier(img);
            }

            if (visBmp != null)
            {
                if (comboFilterImage.SelectedIndex == 0)
                {
                    if (image2Copy != null) image2Copy.Dispose();
                    image2Copy = visBmp;
                    pictureBox2.Image = image2Copy;
                }
                else
                {
                    if (image1Copy != null) image1Copy.Dispose();
                    image1Copy = visBmp;
                    pictureBox1.Image = image1Copy;
                }
            }
        }

        private class ImageData : IDisposable
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public byte[] Data { get; set; }
            public int Stride { get; set; }

            public void Dispose()
            {
                Data = null;
            }
        }

        private void FreqFilterTypeSelected(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null) return;

            groupBoxBinarizationImage.Visible = false;
            selectedBinarizationMethod = null; selectedFilterMethod = null; currentFilterType = null;

            groupBoxFilterCommon.Visible = true;
            groupBoxFilterCommon.Text = "Изображение для частотной фильтрации";
            selectedFreqFilterType = menuItem.Text;
            lblFilterMethod.Text = menuItem.Text;
            lblFilterMethod.ForeColor = System.Drawing.Color.Black;

            // Сначала скрываем всё, потом настраиваем
            lblFilterParam1.Visible = false; nudFilterParam1.Visible = false;
            lblFilterParam2.Visible = false; nudFilterParam2.Visible = false;

            switch (menuItem.Text)
            {
                case "Низкочастотный фильтр":
                case "Высокочастотный фильтр":
                    lblFilterParam1.Text = "Радиус:";
                    lblFilterParam1.Visible = true;
                    nudFilterParam1.Visible = true;
                    // Сначала меняем максимум, потом значение
                    nudFilterParam1.Minimum = 1;
                    nudFilterParam1.Maximum = 100;
                    nudFilterParam1.Value = 30;
                    nudFilterParam1.DecimalPlaces = 0;
                    break;

                case "Режекторный фильтр":
                case "Полосовой фильтр":
                    lblFilterParam1.Text = "R1:";
                    lblFilterParam2.Text = "R2:";
                    lblFilterParam1.Visible = true;
                    nudFilterParam1.Visible = true;
                    lblFilterParam2.Visible = true;
                    nudFilterParam2.Visible = true;
                    // Сначала устанавливаем диапазоны
                    nudFilterParam1.Minimum = 1;
                    nudFilterParam1.Maximum = 100;
                    nudFilterParam2.Minimum = 1;
                    nudFilterParam2.Maximum = 100;
                    // Потом значения (меньше максимума!)
                    nudFilterParam1.Value = 15;
                    nudFilterParam2.Value = 60;
                    nudFilterParam1.DecimalPlaces = 0;
                    nudFilterParam2.DecimalPlaces = 0;
                    break;

                case "Узкополосный режекторный":
                case "Узкополосный полосовой":
                    lblFilterParam1.Text = "Центр:";
                    lblFilterParam2.Text = "Ширина:";
                    lblFilterParam1.Visible = true;
                    nudFilterParam1.Visible = true;
                    lblFilterParam2.Visible = true;
                    nudFilterParam2.Visible = true;
                    // Сначала диапазоны
                    nudFilterParam1.Minimum = 1;
                    nudFilterParam1.Maximum = 100;
                    nudFilterParam2.Minimum = 1;
                    nudFilterParam2.Maximum = 100;
                    // Потом значения
                    nudFilterParam1.Value = 40;
                    nudFilterParam2.Value = 5;
                    nudFilterParam1.DecimalPlaces = 0;
                    nudFilterParam2.DecimalPlaces = 0;
                    break;
            }

            btnFilterApply.Enabled = true;
            btnFilterSave.Enabled = true;
            btnFilterApply.BackColor = System.Drawing.SystemColors.Control;
            btnFilterSave.BackColor = System.Drawing.SystemColors.Control;

            if (comboFilterImage.SelectedIndex == -1)
                comboFilterImage.SelectedIndex = 0;

            // Показываем переключатель Окрестность/Фурье-образ
            lblFreqModeNeighborhood.Visible = true;
            panelFreqSwitch.Visible = true;
            panelFreqKnobLeft.Visible = true;
            panelFreqKnobRight.Visible = true;
            lblFreqModeFourier.Visible = true;
            SetFreqMode(0);
        }

        private void LblFreqMode_Click(object sender, EventArgs e)
        {
            var label = sender as Label;
            if (label == lblFreqModeNeighborhood)
                SetFreqMode(0);
            else if (label == lblFreqModeFourier)
                SetFreqMode(1);
        }

        private void PanelFreqSwitch_Click(object sender, EventArgs e)
        {
            // Переключаем на противоположный режим
            SetFreqMode(freqDisplayMode == 0 ? 1 : 0);
        }

        private void SetFreqMode(int mode)
        {
            freqDisplayMode = mode;

            if (mode == 0)
            {
                lblFreqModeNeighborhood.ForeColor = System.Drawing.Color.DarkBlue;
                lblFreqModeFourier.ForeColor = System.Drawing.Color.Gray;
                // Показываем левый круг, скрываем правый
                panelFreqKnobLeft.Visible = true;
                panelFreqKnobRight.Visible = false;
            }
            else
            {
                lblFreqModeNeighborhood.ForeColor = System.Drawing.Color.Gray;
                lblFreqModeFourier.ForeColor = System.Drawing.Color.DarkBlue;
                // Скрываем левый круг, показываем правый
                panelFreqKnobLeft.Visible = false;
                panelFreqKnobRight.Visible = true;
            }
        }
    }

    // Класс для ДПФ и частотной фильтрации
    public static class FourierTransform
    {
        public struct Complex
        {
            public double Real;
            public double Imag;

            public Complex(double r, double i) { Real = r; Imag = i; }

            public static Complex operator +(Complex a, Complex b) => new Complex(a.Real + b.Real, a.Imag + b.Imag);
            public static Complex operator -(Complex a, Complex b) => new Complex(a.Real - b.Real, a.Imag - b.Imag);
            public static Complex operator *(Complex a, Complex b) => new Complex(
                a.Real * b.Real - a.Imag * b.Imag,
                a.Real * b.Imag + a.Imag * b.Real);
            public static Complex operator *(Complex a, double s) => new Complex(a.Real * s, a.Imag * s);
            public static Complex operator /(Complex a, double s) => new Complex(a.Real / s, a.Imag / s);

            public double Magnitude => Math.Sqrt(Real * Real + Imag * Imag);
            public Complex Conjugate => new Complex(Real, -Imag);
        }

        // Прямое 1D ДПФ
        private static Complex[] DFT1D(Complex[] input, bool inverse)
        {
            int N = input.Length;
            Complex[] output = new Complex[N];
            double sign = inverse ? 1.0 : -1.0;

            for (int u = 0; u < N; u++)
            {
                Complex sum = new Complex(0, 0);
                for (int k = 0; k < N; k++)
                {
                    double angle = sign * 2.0 * Math.PI * u * k / N;
                    Complex w = new Complex(Math.Cos(angle), Math.Sin(angle));
                    sum += input[k] * w;
                }
                if (!inverse) sum = sum / N;
                output[u] = sum;
            }
            return output;
        }

        // Прямое 2D ДПФ (по строкам, затем по столбцам)
        private static Complex[,] DFT2D(double[,] input)
        {
            int H = input.GetLength(0);
            int W = input.GetLength(1);

            // Центрирование: умножаем на (-1)^(x+y)
            double[,] centered = new double[H, W];
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    centered[y, x] = input[y, x] * ((x + y) % 2 == 0 ? 1.0 : -1.0);

            Complex[,] temp = new Complex[H, W];

            // ДПФ по строкам
            for (int y = 0; y < H; y++)
            {
                Complex[] row = new Complex[W];
                for (int x = 0; x < W; x++)
                    row[x] = new Complex(centered[y, x], 0);
                Complex[] dftRow = DFT1D(row, false);
                for (int x = 0; x < W; x++)
                    temp[y, x] = dftRow[x];
            }

            // ДПФ по столбцам
            Complex[,] result = new Complex[H, W];
            for (int x = 0; x < W; x++)
            {
                Complex[] col = new Complex[H];
                for (int y = 0; y < H; y++)
                    col[y] = temp[y, x];
                Complex[] dftCol = DFT1D(col, false);
                for (int y = 0; y < H; y++)
                    result[y, x] = dftCol[y];
            }

            return result;
        }

        // Обратное 2D ДПФ
        private static double[,] IDFT2D(Complex[,] dft)
        {
            int H = dft.GetLength(0);
            int W = dft.GetLength(1);
            Complex[,] temp = new Complex[H, W];

            // Обратное ДПФ по столбцам
            for (int x = 0; x < W; x++)
            {
                Complex[] col = new Complex[H];
                for (int y = 0; y < H; y++)
                    col[y] = dft[y, x];
                Complex[] idftCol = DFT1D(col, true);
                for (int y = 0; y < H; y++)
                    temp[y, x] = idftCol[y];
            }

            // Обратное ДПФ по строкам
            double[,] result = new double[H, W];
            for (int y = 0; y < H; y++)
            {
                Complex[] row = new Complex[W];
                for (int x = 0; x < W; x++)
                    row[x] = temp[y, x];
                Complex[] idftRow = DFT1D(row, true);
                for (int x = 0; x < W; x++)
                    result[y, x] = idftRow[x].Real * ((x + y) % 2 == 0 ? 1.0 : -1.0);
            }

            return result;
        }

        // Визуализация Фурье-образа
        public static Bitmap VisualizeFourier(Complex[,] dft)
        {
            int H = dft.GetLength(0);
            int W = dft.GetLength(1);

            // Вычисляем логарифмическую яркость
            double maxLog = 0;
            double[,] logMag = new double[H, W];
            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    double mag = dft[y, x].Magnitude;
                    logMag[y, x] = Math.Log(mag + 1.0);
                    if (logMag[y, x] > maxLog) maxLog = logMag[y, x];
                }
            }

            Bitmap result = new Bitmap(W, H, PixelFormat.Format32bppArgb);
            BitmapData bd = result.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte[] data = new byte[bd.Stride * H];

            double scale = maxLog > 0 ? 255.0 / maxLog : 1.0;

            for (int y = 0; y < H; y++)
            {
                int rowOff = y * bd.Stride;
                for (int x = 0; x < W; x++)
                {
                    byte val = (byte)(logMag[y, x] * scale);
                    int off = rowOff + x * 4;
                    data[off] = val;
                    data[off + 1] = val;
                    data[off + 2] = val;
                    data[off + 3] = 255;
                }
            }

            Marshal.Copy(data, 0, bd.Scan0, data.Length);
            result.UnlockBits(bd);
            return result;
        }

        // Визуализация маски фильтра (чёрно-белая)
        public static Bitmap VisualizeMask(bool[,] mask)
        {
            int H = mask.GetLength(0);
            int W = mask.GetLength(1);

            Bitmap result = new Bitmap(W, H, PixelFormat.Format32bppArgb);
            BitmapData bd = result.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte[] data = new byte[bd.Stride * H];

            for (int y = 0; y < H; y++)
            {
                int rowOff = y * bd.Stride;
                for (int x = 0; x < W; x++)
                {
                    byte val = mask[y, x] ? (byte)255 : (byte)0;
                    int off = rowOff + x * 4;
                    data[off] = val;
                    data[off + 1] = val;
                    data[off + 2] = val;
                    data[off + 3] = 255;
                }
            }

            Marshal.Copy(data, 0, bd.Scan0, data.Length);
            result.UnlockBits(bd);
            return result;
        }

        // Создание маски фильтра
        private static bool[,] CreateMask(int H, int W, string filterType, double param1, double param2)
        {
            bool[,] mask = new bool[H, W];
            int cx = W / 2, cy = H / 2;
            double maxR = Math.Sqrt(cx * cx + cy * cy);

            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    double dx = x - cx;
                    double dy = y - cy;
                    double dist = Math.Sqrt(dx * dx + dy * dy);
                    double normalizedR = dist / maxR * 100.0; // Нормализуем к 0..100

                    switch (filterType)
                    {
                        case "lowpass":
                            mask[y, x] = normalizedR <= param1;
                            break;
                        case "highpass":
                            mask[y, x] = normalizedR >= param1;
                            break;
                        case "bandreject":
                            mask[y, x] = normalizedR <= param1 || normalizedR >= param2;
                            break;
                        case "bandpass":
                            mask[y, x] = normalizedR >= param1 && normalizedR <= param2;
                            break;
                        case "notchreject":
                            // Узкополосный режекторный: обнуляем кольцо вокруг param1
                            double bandWidth = param2;
                            mask[y, x] = Math.Abs(normalizedR - param1) > bandWidth;
                            break;
                        case "notchpass":
                            // Узкополосный полосовой: оставляем только кольцо вокруг param1
                            double bw = param2;
                            mask[y, x] = Math.Abs(normalizedR - param1) <= bw;
                            break;
                        default:
                            mask[y, x] = true;
                            break;
                    }
                }
            }
            return mask;
        }

        // Применение фильтра к Фурье-образу
        public static Complex[,] ApplyFilter(Complex[,] dft, bool[,] mask)
        {
            int H = dft.GetLength(0);
            int W = dft.GetLength(1);
            Complex[,] result = new Complex[H, W];

            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    result[y, x] = mask[y, x] ? dft[y, x] : new Complex(0, 0);

            return result;
        }

        // Полный пайплайн: канал -> Фурье -> фильтр -> обратное Фурье (с нормализацией)
        public static Bitmap FreqFilterChannel(byte[] channel, int W, int H, string filterType, double p1, double p2)
        {
            // Преобразуем в double[,]
            double[,] input = new double[H, W];
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    input[y, x] = channel[y * W + x];

            // Прямое ДПФ
            Complex[,] dft = DFT2D(input);

            // Создаём маску
            bool[,] mask = CreateMask(H, W, filterType, p1, p2);

            // Применяем фильтр
            Complex[,] filtered = ApplyFilter(dft, mask);

            // Обратное ДПФ
            double[,] output = IDFT2D(filtered);

            // НОРМАЛИЗАЦИЯ: растягиваем значения на весь диапазон 0-255
            double minVal = double.MaxValue, maxVal = double.MinValue;
            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    if (output[y, x] < minVal) minVal = output[y, x];
                    if (output[y, x] > maxVal) maxVal = output[y, x];
                }
            }

            byte[] result = new byte[W * H];
            double range = maxVal - minVal;
            if (range > 0)
            {
                double scale = 255.0 / range;
                for (int y = 0; y < H; y++)
                    for (int x = 0; x < W; x++)
                        result[y * W + x] = (byte)((output[y, x] - minVal) * scale);
            }
            else
            {
                // Если все значения одинаковые — просто копируем
                for (int y = 0; y < H; y++)
                    for (int x = 0; x < W; x++)
                        result[y * W + x] = (byte)Math.Max(0, Math.Min(255, output[y, x]));
            }

            return CreateChannelBitmap(result, W, H);
        }

        // Полный пайплайн для цветного изображения (с автоматическим уменьшением)
        public static Bitmap FreqFilterImage(Bitmap image, string filterType, double p1, double p2)
        {
            if (image == null) return null;

            // Уменьшаем изображение до максимум 256×256 для скорости
            int origW = image.Width, origH = image.Height;
            int maxSize = 128; // Максимальный размер для БЫСТРОЙ обработки
            Bitmap resized = null;
            bool needsResize = origW > maxSize || origH > maxSize;

            if (needsResize)
            {
                double scale = Math.Min((double)maxSize / origW, (double)maxSize / origH);
                int newW = (int)(origW * scale);
                int newH = (int)(origH * scale);
                resized = new Bitmap(image, newW, newH);
            }
            else
            {
                resized = new Bitmap(image);
            }

            int W = resized.Width, H = resized.Height;
            BitmapData bd = resized.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] src = new byte[bd.Stride * H];
            Marshal.Copy(bd.Scan0, src, 0, src.Length);
            resized.UnlockBits(bd);

            byte[] r = new byte[W * H], g = new byte[W * H], b = new byte[W * H];
            for (int y = 0; y < H; y++)
            {
                int rowOff = y * bd.Stride;
                for (int x = 0; x < W; x++)
                {
                    int off = rowOff + x * 4;
                    int idx = y * W + x;
                    b[idx] = src[off];
                    g[idx] = src[off + 1];
                    r[idx] = src[off + 2];
                }
            }

            Bitmap rBmp = FreqFilterChannel(r, W, H, filterType, p1, p2);
            Bitmap gBmp = FreqFilterChannel(g, W, H, filterType, p1, p2);
            Bitmap bBmp = FreqFilterChannel(b, W, H, filterType, p1, p2);

            Bitmap resultSmall = new Bitmap(W, H, PixelFormat.Format32bppArgb);
            BitmapData rd = resultSmall.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte[] dst = new byte[rd.Stride * H];
            byte[] rData = ExtractChannelData(rBmp);
            byte[] gData = ExtractChannelData(gBmp);
            byte[] bData = ExtractChannelData(bBmp);

            for (int y = 0; y < H; y++)
            {
                int rowOff = y * rd.Stride;
                for (int x = 0; x < W; x++)
                {
                    int idx = y * W + x;
                    int off = rowOff + x * 4;
                    dst[off] = bData[idx];
                    dst[off + 1] = gData[idx];
                    dst[off + 2] = rData[idx];
                    dst[off + 3] = 255;
                }
            }
            Marshal.Copy(dst, 0, rd.Scan0, dst.Length);
            resultSmall.UnlockBits(rd);

            rBmp.Dispose(); gBmp.Dispose(); bBmp.Dispose();

            // Восстанавливаем исходный размер
            Bitmap finalResult;
            if (needsResize)
            {
                finalResult = new Bitmap(image.Width, image.Height);
                using (Graphics gfx = Graphics.FromImage(finalResult))
                {
                    gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gfx.DrawImage(resultSmall, 0, 0, image.Width, image.Height);
                }
                resultSmall.Dispose();
            }
            else
            {
                finalResult = resultSmall;
            }

            resized.Dispose();
            return finalResult;
        }

        private static byte[] ExtractChannelData(Bitmap bmp)
        {
            int W = bmp.Width, H = bmp.Height;
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] src = new byte[bd.Stride * H];
            Marshal.Copy(bd.Scan0, src, 0, src.Length);
            bmp.UnlockBits(bd);

            byte[] ch = new byte[W * H];
            for (int y = 0; y < H; y++)
            {
                int rowOff = y * bd.Stride;
                for (int x = 0; x < W; x++)
                    ch[y * W + x] = src[rowOff + x * 4]; // Берём синий канал (все одинаковые)
            }
            return ch;
        }

        private static Bitmap CreateChannelBitmap(byte[] ch, int W, int H)
        {
            Bitmap bmp = new Bitmap(W, H, PixelFormat.Format32bppArgb);
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte[] data = new byte[bd.Stride * H];
            for (int y = 0; y < H; y++)
            {
                int rowOff = y * bd.Stride;
                for (int x = 0; x < W; x++)
                {
                    int off = rowOff + x * 4;
                    byte val = ch[y * W + x];
                    data[off] = val;
                    data[off + 1] = val;
                    data[off + 2] = val;
                    data[off + 3] = 255;
                }
            }
            Marshal.Copy(data, 0, bd.Scan0, data.Length);
            bmp.UnlockBits(bd);
            return bmp;
        }

        // Визуализация Фурье-образа + маски
        public static Bitmap VisualizeFreqFilter(Bitmap image, string filterType, double p1, double p2)
        {
            if (image == null) return null;
            int W = image.Width, H = image.Height;
            // Создаём маску мгновенно (без ДПФ!)
            bool[,] mask = CreateMask(H, W, filterType, p1, p2);
            return VisualizeMask(mask);
        }

        // Визуализация полного Фурье-образа (с уменьшением для скорости)
        public static Bitmap VisualizeFullFourier(Bitmap image)
        {
            if (image == null) return null;

            // Уменьшаем до максимум 128×128 для БЫСТРОЙ визуализации
            int origW = image.Width, origH = image.Height;
            int maxSize = 128;
            Bitmap resized = null;

            if (origW > maxSize || origH > maxSize)
            {
                double scale = Math.Min((double)maxSize / origW, (double)maxSize / origH);
                int newW = (int)(origW * scale);
                int newH = (int)(origH * scale);
                resized = new Bitmap(image, newW, newH);
            }
            else
            {
                resized = new Bitmap(image);
            }

            int W = resized.Width, H = resized.Height;
            BitmapData bd = resized.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] src = new byte[bd.Stride * H];
            Marshal.Copy(bd.Scan0, src, 0, src.Length);
            resized.UnlockBits(bd);

            double[,] gray = new double[H, W];
            for (int y = 0; y < H; y++)
            {
                int rowOff = y * bd.Stride;
                for (int x = 0; x < W; x++)
                {
                    int off = rowOff + x * 4;
                    gray[y, x] = 0.2125 * src[off + 2] + 0.7154 * src[off + 1] + 0.0721 * src[off];
                }
            }

            Complex[,] dft = DFT2D(gray);
            Bitmap result = VisualizeFourier(dft);

            // Растягиваем обратно до исходного размера для отображения
            if (origW > maxSize || origH > maxSize)
            {
                Bitmap stretched = new Bitmap(origW, origH);
                using (Graphics g = Graphics.FromImage(stretched))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(result, 0, 0, origW, origH);
                }
                result.Dispose();
                resized.Dispose();
                return stretched;
            }

            resized.Dispose();
            return result;
        }
    }

    // Класс для пространственной фильтрации
    public static class SpatialFilter
    {
        private class FilterImageData
        {
            public int Width;
            public int Height;
            public byte[] Data;
            public int Stride;
        }

        private static byte QuickSelect(byte[] arr, int left, int right, int k)
        {
            while (left < right)
            {
                int mid = left + (right - left) / 2;
                if (arr[mid] < arr[left]) (arr[left], arr[mid]) = (arr[mid], arr[left]);
                if (arr[right] < arr[left]) (arr[left], arr[right]) = (arr[right], arr[left]);
                if (arr[right] < arr[mid]) (arr[mid], arr[right]) = (arr[right], arr[mid]);

                byte pivot = arr[mid];
                (arr[mid], arr[right]) = (arr[right], arr[mid]);

                int i = left;
                for (int j = left; j < right; j++)
                {
                    if (arr[j] <= pivot)
                    {
                        (arr[i], arr[j]) = (arr[j], arr[i]);
                        i++;
                    }
                }
                (arr[i], arr[right]) = (arr[right], arr[i]);

                if (k == i) return arr[k];
                if (k < i) right = i - 1;
                else left = i + 1;
            }
            return arr[left];
        }

        public static Bitmap LinearFilter(Bitmap image, double[,] kernel)
        {
            if (image == null || kernel == null) return null;

            FilterImageData src = LockImage(image);
            int w = src.Width, h = src.Height;
            int kH = kernel.GetLength(0), kW = kernel.GetLength(1);
            int kCX = kW / 2, kCY = kH / 2;

            double[] kFlat = new double[kH * kW];
            for (int ky = 0; ky < kH; ky++)
                for (int kx = 0; kx < kW; kx++)
                    kFlat[ky * kW + kx] = kernel[ky, kx];

            byte[] output = new byte[src.Data.Length];

            for (int i = 3; i < output.Length; i += 4)
                output[i] = 255;

            Parallel.For(0, h, y =>
            {
                int rowOff = y * src.Stride;
                for (int x = 0; x < w; x++)
                {
                    double sumB = 0, sumG = 0, sumR = 0;
                    int outOff = rowOff + x * 4;

                    for (int ky = 0; ky < kH; ky++)
                    {
                        int py = MirrorCoord(y + ky - kCY, h);
                        int srcRow = py * src.Stride;
                        int kRow = ky * kW;

                        for (int kx = 0; kx < kW; kx++)
                        {
                            int px = MirrorCoord(x + kx - kCX, w);
                            int srcOff = srcRow + px * 4;
                            double wt = kFlat[kRow + kx];
                            sumB += src.Data[srcOff] * wt;
                            sumG += src.Data[srcOff + 1] * wt;
                            sumR += src.Data[srcOff + 2] * wt;
                        }
                    }

                    output[outOff] = Clamp(sumB);
                    output[outOff + 1] = Clamp(sumG);
                    output[outOff + 2] = Clamp(sumR);
                }
            });

            UnlockImage(image, src);
            return CreateBitmap(w, h, output);
        }

        public static Bitmap MedianFilter(Bitmap image, int winSize)
        {
            if (image == null) return null;
            if (winSize % 2 == 0) winSize++;

            FilterImageData src = LockImage(image);
            int w = src.Width, h = src.Height;
            int half = winSize / 2;
            int maxPixels = winSize * winSize;

            byte[] output = new byte[src.Data.Length];
            for (int i = 3; i < output.Length; i += 4)
                output[i] = 255;

            Parallel.For(0, h, y =>
            {
                byte[] rVals = new byte[maxPixels];
                byte[] gVals = new byte[maxPixels];
                byte[] bVals = new byte[maxPixels];

                int rowOff = y * src.Stride;

                for (int x = 0; x < w; x++)
                {
                    int count = 0;

                    for (int wy = -half; wy <= half; wy++)
                    {
                        int py = MirrorCoord(y + wy, h);
                        int srcRow = py * src.Stride;

                        for (int wx = -half; wx <= half; wx++)
                        {
                            int px = MirrorCoord(x + wx, w);
                            int srcOff = srcRow + px * 4;
                            bVals[count] = src.Data[srcOff];
                            gVals[count] = src.Data[srcOff + 1];
                            rVals[count] = src.Data[srcOff + 2];
                            count++;
                        }
                    }

                    int medIdx = count / 2;
                    int outOff = rowOff + x * 4;
                    output[outOff] = QuickSelect(bVals, 0, count - 1, medIdx);
                    output[outOff + 1] = QuickSelect(gVals, 0, count - 1, medIdx);
                    output[outOff + 2] = QuickSelect(rVals, 0, count - 1, medIdx);
                }
            });

            UnlockImage(image, src);
            return CreateBitmap(w, h, output);
        }

        private static double[] CalculateGaussianKernel1D(double sigma, out int kernelSize)
        {
            int r = (int)Math.Ceiling(3 * sigma);
            int size = 2 * r + 1;
            kernelSize = size;

            double[] kernel = new double[size];
            double twoSigmaSq = 2.0 * sigma * sigma;
            double norm = 1.0 / (Math.Sqrt(2 * Math.PI) * sigma);
            double sum = 0;

            for (int i = 0; i < size; i++)
            {
                int x = i - r;
                double val = norm * Math.Exp(-(x * x) / twoSigmaSq);
                kernel[i] = val;
                sum += val;
            }

            double invSum = 1.0 / sum;
            for (int i = 0; i < size; i++)
                kernel[i] *= invSum;

            return kernel;
        }

        private static Bitmap LinearFilterSeparable(Bitmap image, double[] kernel1D)
        {
            if (image == null || kernel1D == null) return null;

            FilterImageData src = LockImage(image);
            int w = src.Width, h = src.Height;
            int kSize = kernel1D.Length;
            int kCenter = kSize / 2;

            byte[] temp = new byte[src.Data.Length];
            for (int i = 3; i < temp.Length; i += 4)
                temp[i] = 255;

            Parallel.For(0, h, y =>
            {
                int rowOff = y * src.Stride;
                for (int x = 0; x < w; x++)
                {
                    double sumB = 0, sumG = 0, sumR = 0;
                    for (int kx = 0; kx < kSize; kx++)
                    {
                        int px = MirrorCoord(x + kx - kCenter, w);
                        int srcOff = rowOff + px * 4;
                        double wt = kernel1D[kx];
                        sumB += src.Data[srcOff] * wt;
                        sumG += src.Data[srcOff + 1] * wt;
                        sumR += src.Data[srcOff + 2] * wt;
                    }
                    int outOff = rowOff + x * 4;
                    temp[outOff] = Clamp(sumB);
                    temp[outOff + 1] = Clamp(sumG);
                    temp[outOff + 2] = Clamp(sumR);
                }
            });

            byte[] output = new byte[src.Data.Length];
            for (int i = 3; i < output.Length; i += 4)
                output[i] = 255;

            Parallel.For(0, h, y =>
            {
                int rowOff = y * src.Stride;
                for (int x = 0; x < w; x++)
                {
                    double sumB = 0, sumG = 0, sumR = 0;
                    for (int ky = 0; ky < kSize; ky++)
                    {
                        int py = MirrorCoord(y + ky - kCenter, h);
                        int srcOff = py * src.Stride + x * 4;
                        double wt = kernel1D[ky];
                        sumB += temp[srcOff] * wt;
                        sumG += temp[srcOff + 1] * wt;
                        sumR += temp[srcOff + 2] * wt;
                    }
                    int outOff = rowOff + x * 4;
                    output[outOff] = Clamp(sumB);
                    output[outOff + 1] = Clamp(sumG);
                    output[outOff + 2] = Clamp(sumR);
                }
            });

            UnlockImage(image, src);
            return CreateBitmap(w, h, output);
        }

        public static Bitmap GaussianBlur(Bitmap image, double sigma)
        {
            double[] kernel1D = CalculateGaussianKernel1D(sigma, out int kSize);
            return LinearFilterSeparable(image, kernel1D);
        }

        private static int MirrorCoord(int coord, int max)
        {
            if (coord < 0) return -coord;
            if (coord >= max) return 2 * max - coord - 2;
            return coord;
        }

        private static byte Clamp(double val)
        {
            return val < 0 ? (byte)0 : val > 255 ? (byte)255 : (byte)val;
        }

        private static FilterImageData LockImage(Bitmap bmp)
        {
            var data = new FilterImageData { Width = bmp.Width, Height = bmp.Height };
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            data.Stride = bd.Stride;
            data.Data = new byte[bd.Stride * bmp.Height];
            Marshal.Copy(bd.Scan0, data.Data, 0, data.Data.Length);
            bmp.UnlockBits(bd);
            return data;
        }

        private static void UnlockImage(Bitmap bmp, FilterImageData data) { }

        private static Bitmap CreateBitmap(int w, int h, byte[] data)
        {
            Bitmap res = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            BitmapData bd = res.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(data, 0, bd.Scan0, data.Length);
            res.UnlockBits(bd);
            return res;
        }
    }

    // Интерфейсы и классы для кривой
    public interface IInterpolation
    {
        double f(double _x);
        void calc(List<MyPoint> points);
    }

    public class MyPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public MyPoint(double x, double y) { X = x; Y = y; }
    }

    public class LinearInterpolation : IInterpolation
    {
        private double[] x = new double[0];
        private double[] y = new double[0];
        private int lastIndex = 0;

        public void calc(List<MyPoint> points)
        {
            x = points.Select(p => p.X).ToArray();
            y = points.Select(p => p.Y).ToArray();
            lastIndex = 0;
        }

        public double f(double _x)
        {
            int N = x.Length;
            if (N < 2) return _x;

            if (_x <= x[0]) return y[0];
            if (_x >= x[N - 1]) return y[N - 1];

            int i = lastIndex;
            if (i < 0 || i >= N - 1 || _x < x[i] || _x > x[i + 1])
            {
                i = Array.BinarySearch(x, _x);
                if (i < 0) i = ~i - 1;
                if (i < 0) i = 0;
                if (i >= N - 1) i = N - 2;
                lastIndex = i;
            }

            double t = (_x - x[i]) / (x[i + 1] - x[i]);
            return y[i] * (1 - t) + y[i + 1] * t;
        }
    }

    public class BSplineInterpolation : IInterpolation
    {
        private List<MyPoint> controlPoints;
        private double[] knots;
        private int degree = 3;
        private double[] x = new double[0];
        private double[] y = new double[0];
        private double[] cachedValues = new double[256];
        private bool cacheValid = false;

        public void calc(List<MyPoint> points)
        {
            controlPoints = points;
            if (controlPoints.Count < 2) return;

            x = controlPoints.Select(p => p.X).ToArray();
            y = controlPoints.Select(p => p.Y).ToArray();

            BuildKnots();
            cacheValid = false;
        }

        private void BuildKnots()
        {
            int n = controlPoints.Count;
            int m = n + degree + 1;
            knots = new double[m];

            for (int i = 0; i <= degree; i++)
                knots[i] = 0;
            for (int i = n; i < m; i++)
                knots[i] = 1;

            if (n > degree + 1)
            {
                int internalKnots = n - degree - 1;
                for (int i = degree + 1; i < n; i++)
                    knots[i] = (double)(i - degree) / (internalKnots + 1);
            }
        }

        private double BasisFunction(int i, int k, double t)
        {
            if (k == 0)
            {
                if (i == knots.Length - 1)
                    return (t >= knots[i] && t <= knots[i + 1]) ? 1.0 : 0.0;
                return (t >= knots[i] && t < knots[i + 1]) ? 1.0 : 0.0;
            }

            double denom1 = knots[i + k] - knots[i];
            double denom2 = knots[i + k + 1] - knots[i + 1];

            double term1 = 0.0, term2 = 0.0;

            if (denom1 > 1e-10)
                term1 = ((t - knots[i]) / denom1) * BasisFunction(i, k - 1, t);

            if (denom2 > 1e-10)
                term2 = ((knots[i + k + 1] - t) / denom2) * BasisFunction(i + 1, k - 1, t);

            return term1 + term2;
        }

        public double f(double _x)
        {
            int n = controlPoints.Count;
            if (n < 2) return _x;

            if (!cacheValid)
            {
                BuildCache();
            }

            int index = (int)(_x * 255);
            if (index < 0) index = 0;
            if (index > 255) index = 255;
            return cachedValues[index];
        }

        private void BuildCache()
        {
            int n = controlPoints.Count;
            if (n < 2) return;

            for (int sample = 0; sample <= 255; sample++)
            {
                double t = sample / 255.0;

                if (t < knots[degree] + 1e-6) t = knots[degree] + 1e-6;
                if (t > knots[n] - 1e-6) t = knots[n] - 1e-6;

                double result = 0;
                double sumWeights = 0;

                for (int i = 0; i < n; i++)
                {
                    double basis = BasisFunction(i, degree, t);
                    result += basis * y[i];
                    sumWeights += basis;
                }

                if (sumWeights > 1e-10)
                    result /= sumWeights;

                cachedValues[sample] = result;
            }
            cacheValid = true;
        }
    }

    public class CurveEditorPanel : Panel
    {
        private List<MyPoint> points = new List<MyPoint>();
        private IInterpolation interpolation = new LinearInterpolation();
        private int? selectedPointIndex = null;
        private Pen axisPen = new Pen(Color.LightGray, 1);
        private Pen curvePen = new Pen(Color.Blue, 2);
        private Brush pointBrush = Brushes.Red;
        private int pointRadius = 6;
        private Button resetCurveButton;
        private bool isDragging = false;
        private Point lastMousePos;
        private byte[] lookupTable = new byte[256];
        private bool needUpdateTable = true;
        private bool isBSplineMode = false;

        public event EventHandler CurveChanged;

        public CurveEditorPanel()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.MouseDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseUp += OnMouseUp;
            this.Paint += OnPaint;
            this.Resize += (s, e) => { this.Invalidate(); };

            resetCurveButton = new Button
            {
                Text = "Сбросить кривую",
                Size = new Size(100, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGray,
                Cursor = Cursors.Hand
            };
            resetCurveButton.Click += ResetCurveButton_Click;
            this.Controls.Add(resetCurveButton);

            this.Resize += (s, e) =>
            {
                resetCurveButton.Location = new Point(this.ClientSize.Width - 110, this.ClientSize.Height - 35);
            };

            InitializeLinearCurve();
        }

        private void InitializeLinearCurve()
        {
            points.Clear();
            points.Add(new MyPoint(0, 0));
            points.Add(new MyPoint(1, 1));
            UpdateInterpolation();
        }

        private void InitializeBSplineCurve()
        {
            points.Clear();
            points.Add(new MyPoint(0, 0));
            points.Add(new MyPoint(0, 0.3333));
            points.Add(new MyPoint(1, 0.6667));
            points.Add(new MyPoint(1, 1));
            UpdateInterpolation();
        }

        private void ResetCurveButton_Click(object sender, EventArgs e)
        {
            if (isBSplineMode)
                InitializeBSplineCurve();
            else
                InitializeLinearCurve();
        }

        public void SetInterpolationType(bool isBSpline)
        {
            isBSplineMode = isBSpline;

            if (isBSpline)
            {
                interpolation = new BSplineInterpolation();
                InitializeBSplineCurve();
            }
            else
            {
                interpolation = new LinearInterpolation();
                InitializeLinearCurve();
            }
        }

        public byte[] GetLookupTable()
        {
            if (needUpdateTable)
            {
                for (int i = 0; i < 256; i++)
                {
                    double normalizedInput = i / 255.0;
                    double normalizedOutput = interpolation.f(normalizedInput);
                    normalizedOutput = Math.Max(0, Math.Min(1, normalizedOutput));
                    lookupTable[i] = (byte)(normalizedOutput * 255);
                }
                needUpdateTable = false;
            }
            return lookupTable;
        }

        private void UpdateInterpolation()
        {
            if (points.Count < 2) return;

            var sortedPoints = points.OrderBy(p => p.X).ToList();
            interpolation.calc(sortedPoints);
            needUpdateTable = true;
            CurveChanged?.Invoke(this, EventArgs.Empty);
            this.Invalidate();
        }

        private PointF ToPanelCoordinates(MyPoint p)
        {
            float x = (float)(p.X * this.ClientSize.Width);
            float y = (float)((1 - p.Y) * this.ClientSize.Height);
            return new PointF(x, y);
        }

        private MyPoint ToCurveCoordinates(Point panelPoint)
        {
            double x = (double)panelPoint.X / this.ClientSize.Width;
            double y = 1 - (double)panelPoint.Y / this.ClientSize.Height;
            x = Math.Max(0, Math.Min(1, x));
            y = Math.Max(0, Math.Min(1, y));
            return new MyPoint(x, y);
        }

        private int FindNearestPoint(Point mousePos)
        {
            for (int i = 0; i < points.Count; i++)
            {
                PointF pt = ToPanelCoordinates(points[i]);
                if (Math.Abs(pt.X - mousePos.X) <= pointRadius &&
                    Math.Abs(pt.Y - mousePos.Y) <= pointRadius)
                {
                    return i;
                }
            }
            return -1;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (resetCurveButton.Bounds.Contains(e.Location))
                return;

            if (e.Button == MouseButtons.Left)
            {
                int hitIndex = FindNearestPoint(e.Location);

                if (hitIndex >= 0)
                {
                    selectedPointIndex = hitIndex;
                    isDragging = true;
                    lastMousePos = e.Location;
                }
                else
                {
                    MyPoint newPoint = ToCurveCoordinates(e.Location);

                    int insertIndex = 0;
                    while (insertIndex < points.Count && points[insertIndex].X < newPoint.X)
                        insertIndex++;

                    points.Insert(insertIndex, newPoint);
                    selectedPointIndex = insertIndex;
                    isDragging = true;
                    lastMousePos = e.Location;
                    UpdateInterpolation();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                int hitIndex = FindNearestPoint(e.Location);
                if (hitIndex >= 0 && hitIndex > 0 && hitIndex < points.Count - 1)
                {
                    points.RemoveAt(hitIndex);
                    selectedPointIndex = null;
                    UpdateInterpolation();
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && selectedPointIndex.HasValue && e.Button == MouseButtons.Left)
            {
                MyPoint newPoint = ToCurveCoordinates(e.Location);
                MyPoint current = points[selectedPointIndex.Value];

                if (selectedPointIndex.Value > 0)
                    newPoint.X = Math.Max(newPoint.X, points[selectedPointIndex.Value - 1].X + 0.001);
                if (selectedPointIndex.Value < points.Count - 1)
                    newPoint.X = Math.Min(newPoint.X, points[selectedPointIndex.Value + 1].X - 0.001);

                newPoint.Y = Math.Max(0, Math.Min(1, newPoint.Y));

                if (Math.Abs(newPoint.X - current.X) > 0.0001 || Math.Abs(newPoint.Y - current.Y) > 0.0001)
                {
                    points[selectedPointIndex.Value] = newPoint;
                    UpdateInterpolation();
                }

                lastMousePos = e.Location;
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            selectedPointIndex = null;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;

            if (w <= 0 || h <= 0) return;

            g.Clear(Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            for (int i = 0; i <= 4; i++)
            {
                float x = i * w / 4f;
                float y = i * h / 4f;
                g.DrawLine(axisPen, x, 0, x, h);
                g.DrawLine(axisPen, 0, y, w, y);
            }

            if (points.Count >= 2)
            {
                using (Pen curvePenSmooth = new Pen(Color.Blue, 2.5f))
                {
                    int steps = Math.Max(300, w);
                    PointF[] curvePoints = new PointF[steps + 1];

                    for (int i = 0; i <= steps; i++)
                    {
                        double normX = i / (double)steps;
                        double normY = interpolation.f(normX);
                        curvePoints[i] = new PointF(
                            (float)(normX * w),
                            (float)((1 - normY) * h));
                    }
                    g.DrawLines(curvePenSmooth, curvePoints);
                }
            }

            foreach (var p in points)
            {
                PointF pt = ToPanelCoordinates(p);
                g.FillEllipse(pointBrush, pt.X - pointRadius, pt.Y - pointRadius, pointRadius * 2, pointRadius * 2);
                g.DrawEllipse(Pens.DarkRed, pt.X - pointRadius, pt.Y - pointRadius, pointRadius * 2, pointRadius * 2);
            }

            try
            {
                if (h > 25 && w > 40)
                {
                    using (Font font = new Font("Arial", 8))
                    using (Brush textBrush = Brushes.Gray)
                    {
                        int bottomY = h - 12;
                        if (bottomY > 0)
                        {
                            g.DrawString("0", font, textBrush, 2, bottomY);
                            g.DrawString("1", font, textBrush, w - 12, bottomY);
                            g.DrawString("1", font, textBrush, 2, 2);
                        }
                    }
                }
            }
            catch { }
        }
    }
}