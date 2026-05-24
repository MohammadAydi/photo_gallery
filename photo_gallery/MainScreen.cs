using Emgu.CV;
using PixelLab;

namespace photo_gallery;

public partial class MainScreen : Form {
    private readonly ImageController _controller;

    private Dictionary<RadioButton, ColorSpaceType> _colorSpaceMap = new();

    // -----------------------------------------------------------------------
    // Channel index constants
    // -----------------------------------------------------------------------

    // RGB
    private const int CH_R = 0, CH_G = 1, CH_B = 2;

    // HSV  (H=0, S=1, V=2)
    private const int CH_H = 0, CH_S = 1, CH_V = 2;

    // YUV  (Y=0, U=1, V=2)
    private const int CH_YUVY = 0, CH_U = 1, CH_YUVV = 2;

    // LAB  (L=0, A=1, B=2)
    private const int CH_L = 0, CH_A = 1, CH_LABB = 2;

    // YCbCr (Y=0, Cb=1, Cr=2)
    private const int CH_YCBCRY = 0, CH_CB = 1, CH_CR = 2;

    // CMYK  (C=0, M=1, Y=2, K=3)
    private const int CH_C = 0, CH_M = 1, CH_CY = 2, CH_K = 3;


    public MainScreen() {
        InitializeComponent();
        _controller = new ImageController();

        ColorNumberSlider.ValueChanged += ColorNumberSlider_ValueChanged;
        UpDownColorsNumber.ValueChanged += UpDownColorsNumber_ValueChanged;

        SetupColorSpaceMap();
        SetupChannelControls();
        OriginalImageViewer.AllowDrop = true;
    }


    private void SetupColorSpaceMap() {
        _colorSpaceMap = new Dictionary<RadioButton, ColorSpaceType> {
            { HSVButton, ColorSpaceType.HSV },
            { RGBButton, ColorSpaceType.RGB },
            { YUVButton, ColorSpaceType.YUV },
            { LABButton, ColorSpaceType.LAB },
            { CMYKButton, ColorSpaceType.CMYK },
            { YCbCrButton, ColorSpaceType.YCbCr },
        };

        foreach (var (btn, _) in _colorSpaceMap)
            btn.CheckedChanged += ColorSpaceButton_CheckedChanged;
    }

    private void SetupChannelControls() {
        // --- RGB ---
        WireChannel(RedCheckbox, RedSlider, () => CH_B);
        WireChannel(GreenCheckbox, GreenSlider, () => CH_G);
        WireChannel(BlueCheckbox, BlueSlider, () => CH_R);

        // --- HSV ---
        WireChannel(HueCheck, HueSlider, () => CH_H);
        WireChannel(SaturationCheck, SaturationSlider, () => CH_S);
        WireChannel(HSVValueCheck, HSVValueSlider, () => CH_V);

        // --- YUV ---
        WireChannel(YYUVLumaCheckbox, YYUVLumaSlider, () => CH_YUVY);
        WireChannel(BlueProjectionCheckbox, BlueProjectionSlider, () => CH_U);
        WireChannel(RedPorjectionCheckbox, RedPorjectionSlider, () => CH_YUVV);

        // --- LAB ---
        WireChannel(LightnessCheck, LightnessSlider, () => CH_L);
        WireChannel(GreenAxisCheck, GreenAxisSlider, () => CH_A);
        WireChannel(BlueAxisCheck, BlueAxisSlider, () => CH_LABB);

        // --- YCbCr ---
        WireChannel(YLumaCheck, YLumaSlider, () => CH_YCBCRY);
        WireChannel(BlueDiffCheck, BlueDiffSlider, () => CH_CB);
        WireChannel(RedDiffCheck, RedDiffSlider, () => CH_CR);

        // --- CMYK ---
        WireChannel(CyanCheckbox, CyanSlider, () => CH_C);
        WireChannel(MagentaCheckbox, MagentaSlider, () => CH_M);
        WireChannel(YellowCheckbox, YellowSlider, () => CH_CY);
        WireChannel(KeyCheckbox, KeySlider, () => CH_K);
    }

    private void WireChannel(CheckBox checkbox, TrackBar slider, Func<int> getIndex) {
        // Initialise slider to 100 %
        slider.Minimum = 0;
        slider.Maximum = 400;
        slider.Value = 100;

        checkbox.Checked = true;

        checkbox.CheckedChanged += (_, _) => {
            if (!_controller.HasImage) return;
            _controller.SetChannelEnabled(getIndex(), checkbox.Checked);
            RefreshModifiedImage();
        };

        slider.ValueChanged += (_, _) => {
            if (!_controller.HasImage) return;
            _controller.SetChannelScale(getIndex(), slider.Value);
            RefreshModifiedImage();
        };
    }

    // -----------------------------------------------------------------------
    // LOAD / RESET / SAVE BUTTONS
    // -----------------------------------------------------------------------

    private void LoadButton_Click(object sender, EventArgs e) {
        using OpenFileDialog dlg = new OpenFileDialog {
            Title = "اختر صورة",
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.tiff;*.tif"
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;

        _controller.LoadImage(dlg.FileName);
        ResetAllSliders();
        RefreshOriginalImage();
        RefreshModifiedImage();
        UpdateImageInfoPanel();
    }

    private void ResetButton_Click(object sender, EventArgs e) {
        _controller.ResetImage();

        foreach (var btn in _colorSpaceMap.Keys)
            btn.Checked = false;

        ResetAllSliders();
        ModifiedImageLabel.Text = "الصورة الأصلية - RGB";
        RefreshModifiedImage();
    }

    private void SaveButton_Click(object sender, EventArgs e) {
        if (!_controller.HasImage) return;

        using SaveFileDialog dlg = new SaveFileDialog {
            Title = "حفظ الصورة",
            Filter = "PNG|*.png|JPEG|*.jpg|BMP|*.bmp"
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;
        _controller.SaveImage(dlg.FileName);
    }

    // -----------------------------------------------------------------------
    // COLOR SPACE SELECTION
    // -----------------------------------------------------------------------

    private void ColorSpaceButton_CheckedChanged(object sender, EventArgs e) {
        if (sender is not RadioButton btn || !btn.Checked) return;
        if (!_controller.HasImage) return;
        if (!_colorSpaceMap.TryGetValue(btn, out var target)) return;

        ResetAllSliders();

        _controller.ConvertColorSpace(target);

        ModifiedImageLabel.Text = $"نظام {btn.Text}";
        RefreshModifiedImage();
    }

    // -----------------------------------------------------------------------
    // IMAGE DISPLAY
    // -----------------------------------------------------------------------

    private void RefreshOriginalImage() {
        Mat? original = _controller.OriginalImage;
        if (original == null || original.IsEmpty) return;
        OriginalImageViewer.Image = original.ToBitmap();
    }

    private void RefreshModifiedImage() {
        Mat? edited = _controller.CurrentImage;
        if (edited == null || edited.IsEmpty) return;

        Mat display = edited;
        if (edited.NumberOfChannels == 4 ||
            edited.Depth != Emgu.CV.CvEnum.DepthType.Cv8U) {
            display = new Mat();
            edited.ConvertTo(display, Emgu.CV.CvEnum.DepthType.Cv8U, 255.0);
        }

        ModifiedImageViewer.Image = display.ToBitmap();
    }

    // -----------------------------------------------------------------------
    // INFO PANEL
    // -----------------------------------------------------------------------

    private void UpdateImageInfoPanel() {
        ImageNameValue.Text = _controller.ImageName;
        ImageSizeValue.Text = $"{_controller.FileSizeKB:F1} KB";
        ImageDimensionValue.Text = _controller.ResolutionText;
        ImageExtensionValue.Text = _controller.Extension;
        ImageChannelValue.Text = _controller.NumberOfChannels.ToString();
        ImageTypeValue.Text = _controller.ChannelType;
        ImageDPIValue.Text = $"{_controller.DpiX:F0} x {_controller.DpiY:F0}";
        ImageAspectRatioValue.Text = _controller.AspectRatio.ToString("F2");
    }

    // -----------------------------------------------------------------------
    // 3-D VIEWER
    // -----------------------------------------------------------------------

    private void Open3DSpaceButton_Click_1(object sender, EventArgs e) {
        var form = new SpacesViewr();
        form.Show();
    }

    // -----------------------------------------------------------------------
    // COLOR QUANTIZATION
    // -----------------------------------------------------------------------

    private void ColorNumberSlider_ValueChanged(object sender, EventArgs e) {
        UpDownColorsNumber.Value = ColorNumberSlider.Value;
        _controller.QuantizeColors(ColorNumberSlider.Value);
        RefreshModifiedImage();
    }

    private void UpDownColorsNumber_ValueChanged(object sender, EventArgs e) {
        ColorNumberSlider.Value = (int)UpDownColorsNumber.Value;
        _controller.QuantizeColors((int)UpDownColorsNumber.Value);
        RefreshModifiedImage();
    }

    // -----------------------------------------------------------------------
    // COMPOUND PANEL VISIBILITY
    // -----------------------------------------------------------------------

    private void HSVButton_CheckedChanged(object sender, EventArgs e) {
        if (HSVButton.Checked) HSVCompoundsFlow.BringToFront();
    }

    private void RGBButton_CheckedChanged(object sender, EventArgs e) {
        if (RGBButton.Checked) RGBFlow.BringToFront();
    }

    private void CMYKButton_CheckedChanged(object sender, EventArgs e) {
        if (CMYKButton.Checked) CMYKFlow.BringToFront();
    }

    private void YUVButton_CheckedChanged_1(object sender, EventArgs e) {
        if (YUVButton.Checked) YUVFlow.BringToFront();
    }

    private void LABButton_CheckedChanged(object sender, EventArgs e) {
        if (LABButton.Checked) LABFlow.BringToFront();
    }

    private void YCbCrButton_CheckedChanged(object sender, EventArgs e) {
        if (YCbCrButton.Checked) YCbCrFlow.BringToFront();
    }

    // -----------------------------------------------------------------------
    // HELPERS
    // -----------------------------------------------------------------------
    private void ResetAllSliders() {
        TrackBar[] sliders = [
            RedSlider, GreenSlider, BlueSlider,
            HueSlider, SaturationSlider, HSVValueSlider,
            YYUVLumaSlider, BlueProjectionSlider, RedPorjectionSlider,
            LightnessSlider, GreenAxisSlider, BlueAxisSlider,
            YLumaSlider, BlueDiffSlider, RedDiffSlider,
            CyanSlider, MagentaSlider, YellowSlider, KeySlider,
        ];

        CheckBox[] checkboxes = [
            RedCheckbox, GreenCheckbox, BlueCheckbox,
            HueCheck, SaturationCheck, HSVValueCheck,
            YYUVLumaCheckbox, BlueProjectionCheckbox, RedPorjectionCheckbox,
            LightnessCheck, GreenAxisCheck, BlueAxisCheck,
            YLumaCheck, BlueDiffCheck, RedDiffCheck,
            CyanCheckbox, MagentaCheckbox, YellowCheckbox, KeyCheckbox,
        ];

        foreach (var s in sliders) s.Value = 100;
        foreach (var c in checkboxes) c.Checked = true;
    }
    

    private void OriginalImageViewer_DragEnter(object sender, DragEventArgs e) {
        if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void OriginalImageViewer_DragDrop(object sender, DragEventArgs e) {
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

        // Allow only one file
        if (files.Length != 1) {
            MessageBox.Show("Please drop only one image.");
            return;
        }

        string filePath = files[0];

        string[] validExtensions = {
            ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif"
        };

        string extension = Path.GetExtension(filePath).ToLower();

        if (!validExtensions.Contains(extension)) {
            MessageBox.Show("Please drop a valid image file.");
            return;
        }

        _controller.LoadImage(filePath);

        ResetAllSliders();
        RefreshOriginalImage();
        RefreshModifiedImage();
        UpdateImageInfoPanel();
    }
}