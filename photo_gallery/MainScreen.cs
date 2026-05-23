using PixelLab;

using Emgu.CV;

namespace photo_gallery;

public partial class MainScreen : Form {
    
    private readonly ImageController _controller;
    
    public MainScreen() {
        InitializeComponent();
        _controller = new ImageController();
        ColorNumberSlider.ValueChanged += ColorNumberSlider_ValueChanged;
        UpDownColorsNumber.ValueChanged += UpDownColorsNumber_ValueChanged;
    }

    private void LoadButton_Click(object sender, EventArgs e) {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = @"Images|*.png;*.jpg;*.jpeg";
        if (dialog.ShowDialog() == DialogResult.OK) {
            _controller.LoadImage(dialog.FileName);
            OriginalImageViewer.Image?.Dispose();
            OriginalImageViewer.Image = _controller.OriginalImage.ToBitmap();
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

    
 private void Open3DSpaceButton_Click(object sender, EventArgs e)
    {
        var form = new SpacesViewr();
        form.Show();
    }


    private void ColorNumberSlider_ValueChanged(object sender, EventArgs e)
    {
        UpDownColorsNumber.Value = ColorNumberSlider.Value;
        _controller.QuantizeColors(ColorNumberSlider.Value);
        RefreshImage();
    }

    private void UpDownColorsNumber_ValueChanged(object sender, EventArgs e)
    {
        ColorNumberSlider.Value = (int)UpDownColorsNumber.Value;
        _controller.QuantizeColors((int)UpDownColorsNumber.Value);
        RefreshImage();
    }
}