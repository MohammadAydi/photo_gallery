namespace PixelLab;

public partial class ColorValuesPanel : UserControl
{
    private static readonly Color BgDark   = Color.FromArgb(50, 50, 50);
    private static readonly Color Accent   = Color.FromArgb(85, 79, 179);
    private static readonly Color TextLight = Color.FromArgb(230, 230, 230);
    private static readonly Color TextDim  = Color.FromArgb(140, 140, 140);
    private static readonly Font  MonoFont  = new("Consolas", 9f);
    private static readonly Font  LabelFont = new("Segoe UI", 8f, FontStyle.Bold);

    private Panel  _colorPreview;
    private Label  _currentSpaceLabel;
    private Label  _currentValueLabel;

  
    private readonly Dictionary<string, Label> _spaceLabels = new();

    public ColorValuesPanel()
    {
        BackColor = BgDark;
        Padding   = new Padding(8);
        BuildLayout();
    }

    private void BuildLayout()
    {
        _colorPreview = new Panel
        {
            Size        = new Size(40, 40),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor   = Color.FromArgb(80, 80, 80)
        };

        _currentSpaceLabel = new Label
        {
            AutoSize  = true,
            ForeColor = Accent,
            Font      = LabelFont,
            Text      = "—"
        };

        _currentValueLabel = new Label
        {
            AutoSize  = true,
            ForeColor = TextLight,
            Font      = MonoFont,
            Text      = "—"
        };

        var divider = new Panel
        {
            Height    = 1,
            Width     = 300,
            BackColor = Color.FromArgb(80, 80, 80),
            Margin    = new Padding(0, 6, 0, 6)
        };

        var allValuesFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            AutoSize      = true,
            WrapContents  = false,
            BackColor     = BgDark
        };

        
        foreach (string key in new[] { "RGB","CMY","HSV","HSL" })
        {
            var lbl = new Label
            {
                AutoSize  = true,
                Font      = MonoFont,
                ForeColor = TextDim,
                BackColor = Color.Transparent,
                Margin    = new Padding(0, 2, 0, 2),
                Text      = $"{key,-7} —"
            };
            _spaceLabels[key] = lbl;
            allValuesFlow.Controls.Add(lbl);
        }

        var currentTextFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            AutoSize      = true,
            WrapContents  = false,
            BackColor     = BgDark,
            Padding       = new Padding(6, 0, 0, 0)
        };
        currentTextFlow.Controls.Add(_currentSpaceLabel);
        currentTextFlow.Controls.Add(_currentValueLabel);

        var topFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize      = true,
            WrapContents  = false,
            BackColor     = BgDark,
            Margin        = new Padding(0, 0, 0, 6)
        };
        topFlow.Controls.Add(_colorPreview);
        topFlow.Controls.Add(currentTextFlow);

        var root = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            Dock          = DockStyle.Fill,
            WrapContents  = false,
            BackColor     = BgDark
        };
        root.Controls.Add(topFlow);
        root.Controls.Add(divider);
        root.Controls.Add(allValuesFlow);

        Controls.Add(root);
    }

    public void UpdateValues(string currentSpace, float[] currentChannels,
        string[] channelNames, Color rgbColor,
        Dictionary<string, (string[] names, float[] values)> allSpaces)
    {
        _colorPreview.BackColor    = rgbColor;
        _currentSpaceLabel.Text    = currentSpace;
        _currentValueLabel.Text    = FormatChannels(channelNames, currentChannels);

       
        foreach (var kv in allSpaces)
        {
            if (!_spaceLabels.TryGetValue(kv.Key, out var lbl)) continue;
            bool isCurrent = kv.Key == currentSpace;
            lbl.ForeColor  = isCurrent ? Accent : TextDim;
            lbl.Text       = $"{kv.Key,-7} {FormatChannels(kv.Value.names, kv.Value.values)}";
        }
    }

    private string FormatChannels(string[] names, float[] values)
    {
        var parts = new List<string>();
        for (int i = 0; i < names.Length; i++)
        {
            string name = names[i];
            float  val  = values[i];

            string formatted = name switch
            {
                "H"                => $"{val:0}°",
                "R" or "G" or "B"  => $"{val:0}",
                "L" when val > 2f  => $"{val:0.0}",
                "V"        => $"{val:+0.000;-0.000}",
                _                  => $"{val:0.000}"
            };

            parts.Add($"{name}:{formatted}");
        }
        return string.Join("  ", parts);
    }
}