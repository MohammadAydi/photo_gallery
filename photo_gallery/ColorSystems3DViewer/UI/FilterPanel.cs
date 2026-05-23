namespace PixelLab;

public partial class FilterPanel : UserControl
{
    private static readonly Color BgDark    = Color.FromArgb(50, 50, 50);
    private static readonly Color Accent    = Color.FromArgb(85, 79, 179);
    private static readonly Color TextLight = Color.FromArgb(230, 230, 230);

    private readonly List<(TrackBar min, TrackBar max)> _clipTracks  = new();
    private readonly List<TrackBar>                     _pointTracks = new();

    private ColorFilter  _filter;
    private Panel        _clipPanel;
    private Panel        _pointPanel;
    private TrackBar     _peelTrack;
    private CheckBox     _subtractCheck;
    private RadioButton  _rbFreeClip;
    private RadioButton  _rbPickPoint;
    private Button       _btnShowInSpace;

    public event Action FilterChanged;

    public FilterPanel()
    {
        InitializeComponent();
        AutoScroll = true;
    }

    // ── Public entry point ────────────────────────────────────────

    public void LoadFilter(ColorFilter filter)
    {
        _filter = filter;
        _clipTracks.Clear();
        _pointTracks.Clear();
        Controls.Clear();

        BuildModeToggle();
        BuildClipPanel();
        BuildPointPanel();

        Controls.Add(_clipPanel);
        Controls.Add(_pointPanel);

        SetMode(true);
    }

    // ── Mode toggle ───────────────────────────────────────────────

    private void BuildModeToggle()
    {
        _rbFreeClip = new RadioButton
        {
            Text     = "Free Clip",
            Location = new Point(8, 6),
            AutoSize = true,
            Checked  = true,
            ForeColor = TextLight,
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9f)
        };

        _rbPickPoint = new RadioButton
        {
            Text      = "Pick Point",
            Location  = new Point(110, 6),
            AutoSize  = true,
            ForeColor = TextLight,
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9f)
        };

        _rbFreeClip.CheckedChanged  += (_, _) => { if (_rbFreeClip.Checked)  SetMode(true);  };
        _rbPickPoint.CheckedChanged += (_, _) => { if (_rbPickPoint.Checked) SetMode(false); };

        Controls.Add(_rbFreeClip);
        Controls.Add(_rbPickPoint);
    }

    // ── Clip panel ────────────────────────────────────────────────

    private void BuildClipPanel()
    {
        _clipPanel = new Panel
        {
            Location     = new Point(0, 30),
            BackColor    = BgDark,
            AutoSize     = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        AddSectionHeader(_clipPanel, "— CLIP RANGES —");

        int y = 24;

        for (int i = 0; i < _filter.ChannelNames.Length; i++)
        {
            int idx = i;

            var lbl = MakeLabel(_filter.ChannelNames[i], new Point(6, y + 8));

            var minTrack = MakeTrackBar(0, new Point(30, y));
            var maxTrack = MakeTrackBar(100, new Point(170, y));

            var minBox = MakeValueBox(minTrack, v => $"{v}%");
            var maxBox = MakeValueBox(maxTrack, v => $"{v}%");

            minBox.Location = new Point(minTrack.Right + 2, y + 4);
            maxBox.Location = new Point(maxTrack.Right + 2, y + 4);

            minTrack.ValueChanged += (_, _) =>
            {
                _filter.Ranges[idx] = (minTrack.Value / 100f, _filter.Ranges[idx].Max);
                FilterChanged?.Invoke();
            };
            maxTrack.ValueChanged += (_, _) =>
            {
                _filter.Ranges[idx] = (_filter.Ranges[idx].Min, maxTrack.Value / 100f);
                FilterChanged?.Invoke();
            };

            _clipPanel.Controls.AddRange([lbl, minTrack, minBox, maxTrack, maxBox]);
            _clipTracks.Add((minTrack, maxTrack));

            y += 44;
        }

        // divider
        _clipPanel.Controls.Add(MakeDivider(y));
        y += 12;

        // peel
        var peelLbl = MakeLabel("Peel", new Point(6, y + 8));
        _peelTrack = MakeTrackBar(100, new Point(50, y));
        _peelTrack.Width = 200;

        var peelBox = MakeValueBox(_peelTrack, v => $"{v}%");
        peelBox.Location = new Point(_peelTrack.Right + 2, y + 4);

        _peelTrack.ValueChanged += (_, _) =>
        {
            _filter.Peel = _peelTrack.Value / 100f;
            FilterChanged?.Invoke();
        };

        y += 44;

        _subtractCheck = new CheckBox
        {
            Text      = "Subtract inner",
            Location  = new Point(6, y),
            AutoSize  = true,
            ForeColor = TextLight,
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9f)
        };
        _subtractCheck.CheckedChanged += (_, _) =>
        {
            _filter.SubtractInner = _subtractCheck.Checked;
            FilterChanged?.Invoke();
        };

        _clipPanel.Controls.AddRange([peelLbl, _peelTrack, peelBox, _subtractCheck]);
    }

    // ── Point panel ───────────────────────────────────────────────

    private void BuildPointPanel()
    {
        _pointPanel = new Panel
        {
            Location     = new Point(0, 30),
            BackColor    = BgDark,
            AutoSize     = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        AddSectionHeader(_pointPanel, "— PICK POINT —");

        int y = 24;

        for (int i = 0; i < _filter.ChannelNames.Length; i++)
        {
            int idx = i;

            var lbl   = MakeLabel(_filter.ChannelNames[i], new Point(6, y + 8));
            var track = MakeTrackBar(50, new Point(30, y));
            track.Width = 220;

            var valBox = MakeValueBox(track, v => $"{v}%");
            valBox.Location = new Point(track.Right + 2, y + 4);

            track.ValueChanged += (_, _) =>
            {
                if (_filter.PointChannels == null) return;
                _filter.PointChannels[idx] = track.Value / 100f;
                FilterChanged?.Invoke();
            };

            _pointPanel.Controls.AddRange([lbl, track, valBox]);
            _pointTracks.Add(track);

            y += 44;
        }

        _pointPanel.Controls.Add(MakeDivider(y));
        y += 12;

        _btnShowInSpace = new Button
        {
            Text      = "Show in Space",
            Location  = new Point(6, y),
            Width     = 200,
            Height    = 36,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(31, 31, 31),
            ForeColor = Accent,
            Font      = new Font("Segoe UI", 9f, FontStyle.Bold)
        };
        _btnShowInSpace.FlatAppearance.BorderColor = Accent;
        _btnShowInSpace.FlatAppearance.BorderSize  = 2;

        _btnShowInSpace.Click += (_, _) =>
        {
            _filter.PeelToPoint();
            if (_peelTrack != null)
                _peelTrack.Value = (int)(_filter.Peel * 100);
            FilterChanged?.Invoke();
        };

        _pointPanel.Controls.Add(_btnShowInSpace);
    }

    // ── Mode switch ───────────────────────────────────────────────

    private void SetMode(bool freeClip)
    {
        if (freeClip)
        {
            _filter.PointChannels = null;
            _clipPanel.Visible    = true;
            _pointPanel.Visible   = false;
        }
        else
        {
            _filter.PointChannels = Enumerable
                .Repeat(0.5f, _filter.ChannelNames.Length).ToArray();
            _clipPanel.Visible  = false;
            _pointPanel.Visible = true;
        }

        FilterChanged?.Invoke();
    }

    // ── Helpers ───────────────────────────────────────────────────

    private static Label MakeLabel(string text, Point location) => new()
    {
        Text      = text,
        Location  = location,
        AutoSize  = true,
        ForeColor = Color.FromArgb(230, 230, 230),
        BackColor = Color.Transparent,
        Font      = new Font("Segoe UI", 9f)
    };

    private static TrackBar MakeTrackBar(int value, Point location) => new()
    {
        Minimum       = 0,
        Maximum       = 100,
        Value         = value,
        Location      = location,
        Width         = 130,
        Height        = 30,
        TickFrequency = 10,
        BackColor     = Color.FromArgb(50, 50, 50)
    };

    private TextBox MakeValueBox(TrackBar track, Func<int, string> format)
    {
        var box = new TextBox
        {
            Width       = 44,
            Height      = 22,
            BackColor   = Color.FromArgb(31, 31, 31),
            ForeColor   = Color.FromArgb(230, 230, 230),
            BorderStyle = BorderStyle.FixedSingle,
            Font        = new Font("Consolas", 8f),
            ReadOnly    = true,
            Text        = format(track.Value)
        };
        track.ValueChanged += (_, _) => box.Text = format(track.Value);
        return box;
    }

    private static void AddSectionHeader(Panel panel, string text)
    {
        panel.Controls.Add(new Label
        {
            Text      = text,
            Location  = new Point(6, 4),
            AutoSize  = true,
            ForeColor = Color.FromArgb(85, 79, 179),
            BackColor = Color.Transparent,
            Font      = new Font("Segoe UI", 8f, FontStyle.Bold)
        });
    }

    private static Panel MakeDivider(int y) => new()
    {
        Location  = new Point(0, y + 4),
        Height    = 1,
        Width     = 320,
        BackColor = Color.FromArgb(80, 80, 80)
    };
}