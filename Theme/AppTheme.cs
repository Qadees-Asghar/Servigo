using System.Drawing;
using System.Windows.Forms;

namespace SERVIGO.Theme
{
    public static class AppTheme
    {
        // ── Dark Palette (matches HTML demo) ─────────────────────────────────
        public static readonly Color BgDark      = Color.FromArgb( 13,  15,  20);
        public static readonly Color Surface      = Color.FromArgb( 22,  26,  35);
        public static readonly Color Surface2     = Color.FromArgb( 30,  35,  50);
        public static readonly Color Border       = Color.FromArgb( 42,  48,  72);
        public static readonly Color Accent       = Color.FromArgb(249, 115,  22);
        public static readonly Color AccentHover  = Color.FromArgb(251, 146,  60);
        public static readonly Color AccentDim    = Color.FromArgb( 50,  25,   8);
        public static readonly Color Gold         = Color.FromArgb(245, 197,  66);
        public static readonly Color TextLight    = Color.FromArgb(232, 236, 244);
        public static readonly Color TextMuted    = Color.FromArgb(122, 133, 163);
        public static readonly Color Success      = Color.FromArgb( 34, 197,  94);
        public static readonly Color SuccessDim   = Color.FromArgb( 10,  50,  25);
        public static readonly Color Danger       = Color.FromArgb(239,  68,  68);
        public static readonly Color DangerDim    = Color.FromArgb( 60,  15,  15);
        public static readonly Color Info         = Color.FromArgb( 96, 165, 250);
        public static readonly Color InfoDim      = Color.FromArgb( 15,  35,  65);
        public static readonly Color GoldDim      = Color.FromArgb( 55,  45,  10);
        public static readonly Color Warning      = Color.FromArgb(255, 193,  7);

        // ── Legacy aliases (kept for compilation) ────────────────────────────
        public static readonly Color Primary      = Accent;
        public static readonly Color PrimaryLight = AccentHover;
        public static readonly Color Sidebar      = Surface;
        public static readonly Color SidebarHover = Surface2;
        public static readonly Color Background   = BgDark;
        public static readonly Color CardBg       = Surface;
        public static readonly Color TextDark     = TextLight;
        public static readonly Color InputBg      = Surface2;
        public static readonly Color Placeholder  = TextMuted;

        // ── Fonts ─────────────────────────────────────────────────────────────
        public static readonly Font FontHero     = new Font("Segoe UI", 32, FontStyle.Bold);
        public static readonly Font FontTitle    = new Font("Segoe UI", 18, FontStyle.Bold);
        public static readonly Font FontSubtitle = new Font("Segoe UI", 13, FontStyle.Bold);
        public static readonly Font FontBody     = new Font("Segoe UI", 10, FontStyle.Regular);
        public static readonly Font FontBodyBold = new Font("Segoe UI", 10, FontStyle.Bold);
        public static readonly Font FontSmall    = new Font("Segoe UI",  8, FontStyle.Regular);
        public static readonly Font FontButton   = new Font("Segoe UI", 10, FontStyle.Bold);
        public static readonly Font FontInput    = new Font("Segoe UI", 10, FontStyle.Regular);
        public static readonly Font FontSidebar  = new Font("Segoe UI", 10, FontStyle.Regular);
        public static readonly Font FontBadge    = new Font("Segoe UI",  7, FontStyle.Bold);
        public static readonly Font FontStat     = new Font("Segoe UI", 26, FontStyle.Bold);
        public static readonly Font FontGrid     = new Font("Segoe UI",  9, FontStyle.Regular);
        public static readonly Font FontGridHead = new Font("Segoe UI",  9, FontStyle.Bold);

        // ── Button Factory ────────────────────────────────────────────────────

        public static Button MakePrimaryButton(string text, int width = 200, int height = 40)
        {
            var b = new Button
            {
                Text      = text,
                Size      = new Size(width, height),
                BackColor = Accent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = FontButton,
                Cursor    = Cursors.Hand
            };
            b.FlatAppearance.BorderSize          = 0;
            b.FlatAppearance.MouseOverBackColor  = AccentHover;
            b.FlatAppearance.MouseDownBackColor  = Color.FromArgb(200, 90, 10);
            return b;
        }

        public static Button MakeAccentButton(string text, int width = 200, int height = 40)
            => MakePrimaryButton(text, width, height);

        public static Button MakeOutlineButton(string text, int width = 140, int height = 36)
        {
            var b = new Button
            {
                Text      = text,
                Size      = new Size(width, height),
                BackColor = Color.Transparent,
                ForeColor = TextMuted,
                FlatStyle = FlatStyle.Flat,
                Font      = FontButton,
                Cursor    = Cursors.Hand
            };
            b.FlatAppearance.BorderSize  = 1;
            b.FlatAppearance.BorderColor = Border;
            b.FlatAppearance.MouseOverBackColor = Surface2;
            return b;
        }

        public static Button MakeDangerButton(string text, int width = 130, int height = 36)
        {
            var b = new Button
            {
                Text      = text,
                Size      = new Size(width, height),
                BackColor = Danger,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = FontButton,
                Cursor    = Cursors.Hand
            };
            b.FlatAppearance.BorderSize         = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 50, 50);
            return b;
        }

        public static Button MakeSuccessButton(string text, int width = 130, int height = 36)
        {
            var b = new Button
            {
                Text      = text,
                Size      = new Size(width, height),
                BackColor = Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = FontButton,
                Cursor    = Cursors.Hand
            };
            b.FlatAppearance.BorderSize         = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(20, 160, 70);
            return b;
        }

        // ── Input Factory ─────────────────────────────────────────────────────

        public static TextBox MakeTextBox(int width = 300, int height = 40, bool password = false)
        {
            return new TextBox
            {
                Size                  = new Size(width, height),
                Font                  = FontInput,
                BackColor             = Surface2,
                ForeColor             = TextLight,
                BorderStyle           = BorderStyle.FixedSingle,
                UseSystemPasswordChar = password
            };
        }

        public static ComboBox MakeComboBox(int width = 300, int height = 40)
        {
            return new ComboBox
            {
                Size          = new Size(width, height),
                Font          = FontInput,
                BackColor     = Surface2,
                ForeColor     = TextLight,
                FlatStyle     = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
        }

        public static Label MakeLabel(string text, Font? font = null, Color? color = null)
        {
            return new Label
            {
                Text      = text,
                Font      = font  ?? FontBody,
                ForeColor = color ?? TextLight,
                AutoSize  = true
            };
        }

        public static Panel MakeCard(int width, int height)
            => new Panel { Size = new Size(width, height), BackColor = Surface };

        // ── Stat Card ─────────────────────────────────────────────────────────

        public static Panel MakeStatCard(string title, string value, Color accent, int width = 230, int height = 150)
        {
            var card = new Panel
            {
                Size      = new Size(width, height),
                BackColor = Surface
            };

            var topBar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 4,
                BackColor = accent
            };

            var lblVal = new Label
            {
                Text      = value,
                Font      = FontStat,
                ForeColor = accent,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location  = new Point(20, 20),
                Size      = new Size(width - 32, 68),
                BackColor = Color.Transparent
            };

            var lblTitle = new Label
            {
                Text      = title,
                Font      = FontBody,
                ForeColor = TextMuted,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location  = new Point(20, 94),
                Size      = new Size(width - 32, 28),
                BackColor = Color.Transparent
            };

            card.Controls.Add(topBar);
            card.Controls.Add(lblVal);
            card.Controls.Add(lblTitle);
            return card;
        }

        // ── DataGridView ──────────────────────────────────────────────────────

        public static DataGridView MakeDataGrid()
        {
            var dgv = new DataGridView
            {
                BackgroundColor           = Surface,
                BorderStyle               = BorderStyle.None,
                CellBorderStyle           = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle  = DataGridViewHeaderBorderStyle.None,
                GridColor                 = Border,
                RowHeadersVisible         = false,
                AllowUserToAddRows        = false,
                AllowUserToDeleteRows     = false,
                ReadOnly                  = true,
                SelectionMode             = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode       = DataGridViewAutoSizeColumnsMode.Fill,
                Font                      = FontGrid,
                RowTemplate               = { Height = 40 },
                EnableHeadersVisualStyles = false
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor  = Surface2;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor  = TextMuted;
            dgv.ColumnHeadersDefaultCellStyle.Font       = FontGridHead;
            dgv.ColumnHeadersDefaultCellStyle.Padding    = new Padding(8, 0, 0, 0);
            dgv.ColumnHeadersHeight                       = 42;
            dgv.DefaultCellStyle.BackColor                = Surface;
            dgv.DefaultCellStyle.ForeColor                = TextLight;
            dgv.DefaultCellStyle.SelectionBackColor       = Color.FromArgb(80, 50, 15);
            dgv.DefaultCellStyle.SelectionForeColor       = TextLight;
            dgv.DefaultCellStyle.Padding                  = new Padding(6, 0, 0, 0);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Surface2;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = TextLight;
            return dgv;
        }

        // ── Placeholder ───────────────────────────────────────────────────────

        public static void AddPlaceholder(TextBox tb, string placeholder)
        {
            tb.Text      = placeholder;
            tb.ForeColor = TextMuted;

            tb.Enter += (s, e) =>
            {
                if (tb.Text == placeholder && tb.ForeColor == TextMuted)
                { tb.Text = string.Empty; tb.ForeColor = TextLight; }
            };
            tb.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                { tb.Text = placeholder; tb.ForeColor = TextMuted; }
            };
        }

        public static bool IsPlaceholder(TextBox tb, string placeholder)
            => tb.Text == placeholder && tb.ForeColor == TextMuted;

        public static string GetText(TextBox tb, string placeholder)
            => IsPlaceholder(tb, placeholder) ? string.Empty : tb.Text.Trim();
    }
}
