using Emgu.CV;

namespace photo_gallery;

public partial class MainScreen : Form
{
    private readonly ImageController _controller;

    // [تعديل #10] ربط كل زر راديو بالنظام اللوني المقابل له
    private Dictionary<RadioButton, ColorSpaceType> _colorSpaceMap = new();

    public MainScreen()
    {
        InitializeComponent();
        _controller = new ImageController();
        ColorNumberSlider.ValueChanged += ColorNumberSlider_ValueChanged;
        UpDownColorsNumber.ValueChanged += UpDownColorsNumber_ValueChanged;

        // [تعديل #11] استدعاء إعداد خريطة الأنظمة اللونية عند بدء النموذج
        SetupColorSpaceMap();

        // [تعديل #12] استدعاء إعداد أحداث الـ sliders والـ checkboxes
        SetupSliderEvents();
    }

    // ---------------------------
    // SETUP
    // ---------------------------

    // [تعديل #13] ربط كل RadioButton في ColorsSystemsGrid بنظامه اللوني
    // والاشتراك في حدث CheckedChanged لكل واحد منهم
    private void SetupColorSpaceMap()
    {
        _colorSpaceMap = new Dictionary<RadioButton, ColorSpaceType>
        {
            { HSVButton,   ColorSpaceType.HSV   },
            { HLSButton,   ColorSpaceType.HLS   },
            { YUVButton,   ColorSpaceType.YUV   },
            { LABButton,   ColorSpaceType.LAB   },
            { CMYKButton,  ColorSpaceType.CMYK  },
            { YCbCrButton, ColorSpaceType.YCbCr },
        };

        foreach (var (btn, _) in _colorSpaceMap)
            btn.CheckedChanged += ColorSpaceButton_CheckedChanged;
    }

    // [تعديل #14] إعداد نطاق الـ sliders وربط أحداث التغيير
    // والـ checkboxes الخاصة بالقنوات
    private void SetupSliderEvents()
    {
        // نطاق الـ slider: 0–100، والقيمة الوسطى 50 تعني scale=1.0 (بدون تغيير)
        foreach (var slider in new[] { RSlider, GSlider, BSlider })
        {
            slider.Minimum = 0;
            slider.Maximum = 100;
            slider.Value   = 50;
        }

        // RSlider.ValueChanged += (_, _) => OnChannelSliderChanged();
        // GSlider.ValueChanged += (_, _) => OnChannelSliderChanged();
        // BSlider.ValueChanged += (_, _) => OnChannelSliderChanged();

        RCheckBox.CheckedChanged += (_, _) => ApplyChannelFilter();
        G.CheckedChanged         += (_, _) => ApplyChannelFilter();
        BCheckBox.CheckedChanged += (_, _) => ApplyChannelFilter();

        RCheckBox.Checked = true;
        G.Checked         = true;
        BCheckBox.Checked = true;
    }

    // ---------------------------
    // TOP-BAR BUTTON HANDLERS (موجودة مسبقاً — لم يتغير منطقها)
    // ---------------------------

    private void LoadButton_Click(object sender, EventArgs e)
    {
        using OpenFileDialog dlg = new OpenFileDialog
        {
            Title  = "اختر صورة",
            Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.tiff;*.tif"
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;

        _controller.LoadImage(dlg.FileName);
        RefreshOriginalImage();
        RefreshModifiedImage();
        UpdateImageInfoPanel();
    }

    private void ResetButton_Click(object sender, EventArgs e)
    {
        _controller.ResetImage();

        // [تعديل #15] إلغاء تحديد أزرار الأنظمة اللونية عند الإعادة
        foreach (var btn in _colorSpaceMap.Keys)
            btn.Checked = false;

        ResetSliders();
        ModifiedImageLabel.Text = "الصورة الأصلية - RGB";
        RefreshModifiedImage();
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        if (!_controller.HasImage) return;

        using SaveFileDialog dlg = new SaveFileDialog
        {
            Title  = "حفظ الصورة",
            Filter = "PNG|*.png|JPEG|*.jpg|BMP|*.bmp"
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;
        _controller.SaveImage(dlg.FileName);
    }

    private void Open3DSpaceButton_Click_1(object sender, EventArgs e)
    {
        // placeholder — ربط نافذة الـ 3D هنا لاحقاً
    }

    // ---------------------------
    // [تعديل #16] حدث اختيار نظام لوني جديد
    // يُطلق عند تحديد أي RadioButton في ColorsSystemsGrid
    // ---------------------------
    private void ColorSpaceButton_CheckedChanged(object sender, EventArgs e)
    {
        if (sender is not RadioButton btn || !btn.Checked) return;
        if (!_controller.HasImage) return;

        if (!_colorSpaceMap.TryGetValue(btn, out var target)) return;

        // [تعديل #17] استدعاء ConvertColorSpace في الـ Controller
        // وتحديث labels القنوات بالأسماء المُرجَعة
        string[] channelNames = _controller.ConvertColorSpace(target);
        UpdateChannelLabels(channelNames);

        ModifiedImageLabel.Text = $"نظام {btn.Text}";
        RefreshModifiedImage();
    }

    // ---------------------------
    // [تعديل #18] حدث تغيير الـ sliders — يُطبّق تحجيم على كل قناة
    // القيمة 50 من الـ slider = scale 1.0 (بدون تغيير)
    // ---------------------------
    // private void OnChannelSliderChanged()
    // {
    //     if (!_controller.HasImage) return;
    //
    //     _controller.ScaleChannel(0, RSlider.Value / 50.0);
    //     _controller.ScaleChannel(1, GSlider.Value / 50.0);
    //     _controller.ScaleChannel(2, BSlider.Value / 50.0);
    //
    //     RefreshModifiedImage();
    // }

    // ---------------------------
    // [تعديل #19] حدث تغيير الـ checkboxes — يُطبّق فلتر القنوات
    // ---------------------------
    private void ApplyChannelFilter()
    {
        if (!_controller.HasImage) return;

        _controller.ApplyChannelFilter(new[]
        {
            RCheckBox.Checked,
            G.Checked,
            BCheckBox.Checked
        });

        RefreshModifiedImage();
    }

    // ---------------------------
    // IMAGE DISPLAY HELPERS
    // ---------------------------

    private void RefreshOriginalImage()
    {
        Mat? original = _controller.OriginalImage;
        if (original == null || original.IsEmpty) return;
        OriginalImageViewer.Image = original.ToBitmap();
    }

    // [تعديل #20] عرض الصورة المعدّلة — CMYK تحتاج تحويل إلى 8-bit قبل العرض
    private void RefreshModifiedImage()
    {
        Mat? edited = _controller.CurrentImage;
        if (edited == null || edited.IsEmpty) return;

        Mat display = edited;
        if (edited.NumberOfChannels == 4 || edited.Depth != Emgu.CV.CvEnum.DepthType.Cv8U)
        {
            display = new Mat();
            edited.ConvertTo(display, Emgu.CV.CvEnum.DepthType.Cv8U, 255.0);
        }

        ModifiedImageViewer.Image = display.ToBitmap();
    }

    // ---------------------------
    // [تعديل #21] تحديث labels الـ sliders بأسماء قنوات النظام الحالي
    // مثال: بعد HSV تصبح "Hue" / "Saturation" / "Value"
    // ---------------------------
    private void UpdateChannelLabels(string[] names)
    {
        if (names.Length >= 1) RColorPer.Text = names[0];
        if (names.Length >= 2) GColorPer.Text = names[1];
        if (names.Length >= 3) BColorPer.Text = names[2];
    }

    // تحديث لوحة معلومات الصورة (موجود مسبقاً في المشروع)
    private void UpdateImageInfoPanel()
    {
        ImageNameValue.Text        = _controller.ImageName;
        ImageSizeValue.Text        = $"{_controller.FileSizeKB:F1} KB";
        ImageDimensionValue.Text   = _controller.ResolutionText;
        ImageExtensionValue.Text   = _controller.Extension;
        ImageChannelValue.Text     = _controller.NumberOfChannels.ToString();
        ImageTypeValue.Text        = _controller.ChannelType;
        ImageDPIValue.Text         = $"{_controller.DpiX:F0} x {_controller.DpiY:F0}";
        ImageAspectRatioValue.Text = _controller.AspectRatio.ToString("F2");
    }

    private void ResetSliders()
    {
        RSlider.Value = 50;
        GSlider.Value = 50;
        BSlider.Value = 50;
    }
}


    private void Open3DSpaceButton_Click(object sender, EventArgs e) {
        var form = new SpacesViewr();
        form.Show();
    }

    private void Open3DSpaceButton_Click_1(object sender, EventArgs e) {
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

    private void ResetButton_Click(object sender, EventArgs e) {
        _controller.ResetImage();
        RefreshImage();
    }


    private void HSVButton_CheckedChanged(object sender, EventArgs e) {
        if (HSVButton.Checked) {
            HSVCompoundsFlow.BringToFront();
        }
    }

    private void RGBButton_CheckedChanged(object sender, EventArgs e) {
        if (RGBButton.Checked) {
            RGBFlow.BringToFront();
        }
    }

    private void CMYKButton_CheckedChanged(object sender, EventArgs e) {
        if (CMYKButton.Checked) {
            CMYKFlow.BringToFront();
        }
    }

    private void YUVButton_CheckedChanged_1(object sender, EventArgs e) {
        if (YUVButton.Checked) {
            YUVFlow.BringToFront();
        }
    }

    private void LABButton_CheckedChanged(object sender, EventArgs e) {
        if (LABButton.Checked) {
            LABFlow.BringToFront();
        }
    }

    private void YCbCrButton_CheckedChanged(object sender, EventArgs e) {
        if (YCbCrButton.Checked) {
            YCbCrFlow.BringToFront();
        }
    }
}