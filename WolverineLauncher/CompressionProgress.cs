using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WolverineLauncher
{
    public partial class CompressionProgress : Form
    {
        public bool isRunning = false;

        private void CompressionProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = isRunning;
        }
        public CompressionProgress()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.Icon;
        }
        private async Task Handle_CompressionProgress(bool compression, List<string> compressionFiles, int compressionFilesCount)
        {
            await Task.Run(() =>
            {
                int compressedFileCount = 0;

                try
                {
                    while (compressedFileCount < progressBar_Compression.Maximum)
                    {
                        if (isRunning == false)
                            throw new InvalidOperationException("Handle_CompressiongProgress can't be running while isRunning == false");

                        compressedFileCount = 0;

                        foreach (string compressionFile in compressionFiles)
                        {
                            FileAttributes attributes = File.GetAttributes(compressionFile);

                            if (compression && (attributes & FileAttributes.Compressed) == FileAttributes.Compressed)
                            {
                                compressedFileCount++;
                            }
                            else if (!compression && (attributes & FileAttributes.Compressed) != FileAttributes.Compressed)
                            {
                                compressedFileCount++;
                            }
                        }

                        progressBar_Compression.Invoke((MethodInvoker)delegate
                        {
                            progressBar_Compression.Value = compressedFileCount;
                        });

                        label_Compression_Progress.Invoke((MethodInvoker)delegate
                        {
                            label_Compression_Progress.Text = $"{compressedFileCount} / {compressionFilesCount}";
                        });

                        Thread.Sleep(1000);
                    }
                }
                catch { }
            });
        }
        public void Stop()
        {
            isRunning = false;

            label_Compression_Progress.Invoke((MethodInvoker)delegate
            {
                label_Compression_Progress.Visible = false;
            });

            this.Invoke((MethodInvoker)delegate
            {
                this.Hide();
            });
        }
        public async void Start(bool compression, List<string> compressionFiles)
        {
            isRunning = true;
            int compressionFilesCount = compressionFiles.Count;

            label_Compression_Title.Invoke((MethodInvoker)delegate
            {
                label_Compression_Title.Text = compression ? "Compression in progress" : "Uncompression in progress";
            });

            progressBar_Compression.Invoke((MethodInvoker)delegate
            {
                progressBar_Compression.Value = 0;
                progressBar_Compression.Maximum = compressionFilesCount;
            });

            label_Compression_Progress.Invoke((MethodInvoker)delegate
            {
                label_Compression_Progress.Text = $"0 / {compressionFilesCount}";
                label_Compression_Progress.Visible = true;
            });

            await Handle_CompressionProgress(compression, compressionFiles, compressionFilesCount);
            Stop();
        }
    }
}
