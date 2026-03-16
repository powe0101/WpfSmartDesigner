using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WpfSmartDesigner.Engine
{
    /// <summary>
    /// Service for capturing screenshots of Visual Studio windows.
    /// </summary>
    public class ScreenCaptureService
    {
        public string? LastCapturePath { get; private set; }

        /// <summary>
        /// Captures the Visual Studio window and saves it to disk.
        /// </summary>
        /// <returns>True if capture was successful, false otherwise.</returns>
        public bool CaptureVisualStudioWindow()
        {
            try
            {
                // Find Visual Studio process
                var vsProcess = Process.GetProcessesByName("devenv").FirstOrDefault();
                if (vsProcess == null)
                {
                    System.Diagnostics.Debug.WriteLine("Visual Studio process not found");
                    return false;
                }

                var windowHandle = vsProcess.MainWindowHandle;
                if (windowHandle == IntPtr.Zero)
                {
                    System.Diagnostics.Debug.WriteLine("Visual Studio window handle is invalid");
                    return false;
                }

                // Get window rectangle
                if (!GetWindowRect(windowHandle, out RECT rect))
                {
                    System.Diagnostics.Debug.WriteLine("Failed to get window rectangle");
                    return false;
                }

                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                // Capture the window
                using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                using var graphics = Graphics.FromImage(bitmap);

                var hdcBitmap = graphics.GetHdc();
                try
                {
                    PrintWindow(windowHandle, hdcBitmap, 0);
                }
                finally
                {
                    graphics.ReleaseHdc(hdcBitmap);
                }

                // Save to disk
                var capturesDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "WpfSmartDesigner",
                    "Captures");

                Directory.CreateDirectory(capturesDir);

                var fileName = $"capture_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                var filePath = Path.Combine(capturesDir, fileName);

                bitmap.Save(filePath, ImageFormat.Png);

                LastCapturePath = filePath;
                System.Diagnostics.Debug.WriteLine($"Screenshot saved to: {filePath}");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Screen capture error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Captures a specific region of the screen.
        /// </summary>
        public bool CaptureRegion(int x, int y, int width, int height, string outputPath)
        {
            try
            {
                using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                using var graphics = Graphics.FromImage(bitmap);

                graphics.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

                bitmap.Save(outputPath, ImageFormat.Png);

                LastCapturePath = outputPath;
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Region capture error: {ex.Message}");
                return false;
            }
        }

        #region Win32 API

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        #endregion
    }
}
