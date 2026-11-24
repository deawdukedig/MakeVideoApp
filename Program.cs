using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageAudioToVideo
{
    public class MainForm : Form
    {
        private TextBox txtImage, txtAudio, txtOutput;
        private Button btnImage, btnAudio, btnOutput, btnConvert, btnClear;
        private Label lblStatus, lblPreviewHint;
        private PictureBox picPreview;
        private bool isCustomOutput = false;

        private readonly string[] imageExts = { ".jpg", ".jpeg", ".png", ".bmp" };
        private readonly string[] audioExts = { ".mp3", ".wav", ".m4a", ".aac" };

        public MainForm()
        {
            this.Text = "MP3 + Image to Full HD Video (FFmpeg)";
            this.Size = new Size(1060, 440);
            this.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AllowDrop = true;

            this.DragEnter += MainForm_DragEnter;
            this.DragDrop += MainForm_DragDrop;

            // 1. เลือกไฟล์ภาพ
            Label lblImg = new Label() { Text = "ไฟล์ภาพ:", Location = new Point(25, 32), AutoSize = true };
            txtImage = new TextBox() { Location = new Point(145, 28), Width = 380, ReadOnly = true, AllowDrop = true };
            txtImage.DragEnter += MainForm_DragEnter;
            txtImage.DragDrop += MainForm_DragDrop;

            btnImage = new Button() { Text = "เลือกภาพ...", Location = new Point(540, 25), Width = 140, Height = 40 };
            btnImage.Click += (s, e) => {
                using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp" })
                    if (ofd.ShowDialog() == DialogResult.OK) SetImageFile(ofd.FileName);
            };

            // 2. เลือกไฟล์เสียง
            Label lblAud = new Label() { Text = "ไฟล์เสียง:", Location = new Point(25, 92), AutoSize = true };
            txtAudio = new TextBox() { Location = new Point(145, 88), Width = 380, ReadOnly = true, AllowDrop = true };
            txtAudio.DragEnter += MainForm_DragEnter;
            txtAudio.DragDrop += MainForm_DragDrop;

            btnAudio = new Button() { Text = "เลือกเสียง...", Location = new Point(540, 85), Width = 140, Height = 40 };
            btnAudio.Click += (s, e) => {
                using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Audio Files|*.mp3;*.wav;*.m4a;*.aac" })
                    if (ofd.ShowDialog() == DialogResult.OK) SetAudioFile(ofd.FileName);
            };

            // 3. เลือกไฟล์ปลายทาง
            Label lblOut = new Label() { Text = "บันทึกที่:", Location = new Point(25, 152), AutoSize = true };
            txtOutput = new TextBox() { Location = new Point(145, 148), Width = 380 };
            btnOutput = new Button() { Text = "ปลายทาง...", Location = new Point(540, 145), Width = 140, Height = 40 };
            btnOutput.Click += (s, e) => {
                string defaultName = string.IsNullOrWhiteSpace(txtOutput.Text) ? "vdo.mp4" : Path.GetFileName(txtOutput.Text);
                using (SaveFileDialog sfd = new SaveFileDialog { Filter = "MP4 Video|*.mp4", FileName = defaultName })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        txtOutput.Text = sfd.FileName;
                        isCustomOutput = true;
                    }
                }
            };

            // 4. ปุ่ม Convert และปุ่ม Clear
            btnConvert = new Button()
            {
                Text = "Convert เป็นวิดีโอ Full HD",
                Location = new Point(145, 215),
                Width = 380,
                Height = 55,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold)
            };
            btnConvert.Click += BtnConvert_Click;

            btnClear = new Button()
            {
                Text = "ล้างค่า",
                Location = new Point(540, 215),
                Width = 140,
                Height = 55,
                Font = new Font("Segoe UI", 14F, FontStyle.Regular)
            };
            btnClear.Click += BtnClear_Click;

            // 5. สถานะการทำงาน
            lblStatus = new Label()
            {
                Text = "สถานะ: พร้อมทำงาน (ลากไฟล์ภาพและเสียงมาวางได้ทันที)",
                Location = new Point(25, 310),
                AutoSize = true,
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold)
            };

            // กรอบแสดงภาพตัวอย่าง (Thumbnail Preview 16:9)
            Label lblPreviewTitle = new Label() { Text = "ตัวอย่างภาพ (16:9):", Location = new Point(705, 25), AutoSize = true };

            picPreview = new PictureBox()
            {
                Location = new Point(705, 62),
                Size = new Size(320, 180),
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                AllowDrop = true
            };
            picPreview.DragEnter += MainForm_DragEnter;
            picPreview.DragDrop += MainForm_DragDrop;

            lblPreviewHint = new Label()
            {
                Text = "ยังไม่ได้เลือกภาพ\n(ลากภาพมาวางที่นี่ได้)",
                ForeColor = Color.LightGray,
                BackColor = Color.Black,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(310, 60),
                Location = new Point(710, 122)
            };
            lblPreviewHint.Click += (s, e) => btnImage.PerformClick();

            this.Controls.AddRange(new Control[] {
                lblImg, txtImage, btnImage,
                lblAud, txtAudio, btnAudio,
                lblOut, txtOutput, btnOutput,
                btnConvert, btnClear, lblStatus,
                lblPreviewTitle, picPreview, lblPreviewHint
            });
            lblPreviewHint.BringToFront();
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length == 0) return;

            foreach (string file in files)
            {
                string ext = Path.GetExtension(file).ToLowerInvariant();
                if (imageExts.Contains(ext))
                {
                    SetImageFile(file);
                }
                else if (audioExts.Contains(ext))
                {
                    SetAudioFile(file);
                }
            }
        }

        private void SetImageFile(string filePath)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    Image img = Image.FromStream(ms);
                    if (picPreview.Image != null)
                    {
                        picPreview.Image.Dispose();
                    }
                    picPreview.Image = new Bitmap(img);
                }

                lblPreviewHint.Visible = false;
                txtImage.Text = filePath;

                if (!isCustomOutput && string.IsNullOrWhiteSpace(txtAudio.Text))
                {
                    txtOutput.Text = Path.ChangeExtension(filePath, ".mp4");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ไม่สามารถโหลดภาพตัวอย่างได้: " + ex.Message, "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetAudioFile(string filePath)
        {
            txtAudio.Text = filePath;
            if (!isCustomOutput)
            {
                txtOutput.Text = Path.ChangeExtension(filePath, ".mp4");
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtImage.Clear();
            txtAudio.Clear();
            txtOutput.Clear();
            isCustomOutput = false;

            if (picPreview.Image != null)
            {
                picPreview.Image.Dispose();
                picPreview.Image = null;
            }
            lblPreviewHint.Visible = true;

            lblStatus.Text = "สถานะ: ล้างค่าเรียบร้อย พร้อมเริ่มงานใหม่";
            lblStatus.ForeColor = Color.DimGray;
        }

        private async void BtnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtImage.Text) || string.IsNullOrWhiteSpace(txtAudio.Text) || string.IsNullOrWhiteSpace(txtOutput.Text))
            {
                MessageBox.Show("กรุณาเลือกไฟล์ภาพ ไฟล์เสียง และที่เก็บไฟล์ปลายทางให้ครบถ้วน", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string localFfmpeg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
            string ffmpegPath = File.Exists(localFfmpeg) ? localFfmpeg : "ffmpeg";

            string args = string.Format("-y -loop 1 -framerate 1 -i \"{0}\" -i \"{1}\" -c:v libx264 -tune stillimage -vf \"scale=1920:1080:force_original_aspect_ratio=decrease,pad=1920:1080:(ow-iw)/2:(oh-ih)/2:black\" -c:a aac -b:a 192k -pix_fmt yuv420p -shortest \"{2}\"",
                txtImage.Text, txtAudio.Text, txtOutput.Text);

            btnConvert.Enabled = false;
            btnClear.Enabled = false;
            lblStatus.Text = "สถานะ: กำลังแปลงไฟล์ กรุณารอสักครู่...";
            lblStatus.ForeColor = Color.Blue;

            try
            {
                int exitCode = await Task.Run(() => RunFFmpeg(ffmpegPath, args));
                if (exitCode == 0)
                {
                    lblStatus.Text = "สถานะ: แปลงไฟล์สำเร็จเรียบร้อย!";
                    lblStatus.ForeColor = Color.Green;
                    MessageBox.Show("สร้างวิดีโอสำเร็จ!\n\nบันทึกที่: " + txtOutput.Text, "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (File.Exists(txtOutput.Text))
                    {
                        Process.Start("explorer.exe", $"/select,\"{txtOutput.Text}\"");
                    }
                }
                else
                {
                    lblStatus.Text = "สถานะ: เกิดข้อผิดพลาดในการแปลงไฟล์";
                    lblStatus.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "สถานะ: ไม่พบไฟล์ ffmpeg.exe";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message + "\n\nกรุณานำไฟล์ ffmpeg.exe มาวางไว้ในโฟลเดอร์เดียวกับโปรแกรม", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConvert.Enabled = true;
                btnClear.Enabled = true;
            }
        }

        private int RunFFmpeg(string ffmpegPath, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                process.WaitForExit();
                return process.ExitCode;
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
