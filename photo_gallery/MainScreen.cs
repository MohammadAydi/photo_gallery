using PixelLab;
using Emgu.CV;

namespace photo_gallery;

public partial class MainScreen : Form {
    private readonly ImageController _controller;

    public MainScreen() {
        InitializeComponent();
        _controller = new ImageController();
    }

    private void LoadButton_Click(object sender, EventArgs e) {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = @"Images|*.png;*.jpg;*.jpeg";
        if (dialog.ShowDialog() == DialogResult.OK) {
            _controller.LoadImage(dialog.FileName);
            // ---------------------------
            // DISPLAY IMAGE INFORMATION
            // ---------------------------
            ImageNameValue.Text = _controller.ImageName;
            ImageSizeValue.Text = $@"{_controller.FileSizeMB:F2} MB";
            ImageTypeValue.Text = _controller.ResolutionText;
            ImageExtensionValue.Text = _controller.Extension;
            ImageChannelValue.Text = _controller.NumberOfChannels.ToString();
            ImageTypeValue.Text = _controller.ChannelType;
            ImageDPIValue.Text = $@"{_controller.DpiX:F0} x {_controller.DpiY:F0}";
            ImageAspectRatioValue.Text = _controller.AspectRatio.ToString("F2");

            // ---------------------------
            // DISPLAY IMAGE
            // ---------------------------
            OriginalImageViewer.Image?.Dispose();
            OriginalImageViewer.Image = _controller.OriginalImage!.ToBitmap();

            RefreshImage();
        }
    }

    private void SaveButton_Click(object sender, EventArgs e) {
        if (!_controller.HasImage)
            return;
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Filter = @"Images|*.png;*.jpg;*.jpeg";
        if (dialog.ShowDialog() == DialogResult.OK) {
            _controller.SaveImage(dialog.FileName);
        }
    }


    private void RefreshImage() {
        if (_controller.CurrentImage == null) {
            return;
        }

        ModifiedImageViewer.Image?.Dispose();
        ModifiedImageViewer.Image = _controller.CurrentImage.ToBitmap();
    }


    private void Open3DSpaceButton_Click(object sender, EventArgs e) {
        var form = new SpacesViewr();
        form.Show();
    }

    private void Open3DSpaceButton_Click_1(object sender, EventArgs e) {
        var form = new SpacesViewr();
        form.Show();
    }

    private void ResetButton_Click(object sender, EventArgs e) {
        _controller.ResetImage();
        RefreshImage();
    }
}