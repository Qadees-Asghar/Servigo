using SERVIGO.Theme;

namespace SERVIGO.Forms.Customer
{
    public class frmRating : Form
    {
        public int    SelectedStars { get; private set; } = 0;
        public string RatingComment { get; private set; } = string.Empty;

        private readonly Button[] _starBtns = new Button[5];
        private readonly TextBox  _txtComment;
        private readonly Label    _lblHint;

        public frmRating(string providerName, int existingStars = 0, string existingComment = "")
        {
            bool isEdit = existingStars > 0;

            Text            = isEdit ? "Edit Your Review" : "Rate Provider";
            Size            = new Size(460, 330);
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = AppTheme.Surface;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;

            var lblTitle = new Label
            {
                Text      = isEdit ? $"Edit review for {providerName}" : $"Rate  {providerName}",
                Font      = AppTheme.FontSubtitle,
                ForeColor = AppTheme.TextLight,
                AutoSize  = true,
                Location  = new Point(24, 20)
            };

            _lblHint = new Label
            {
                Text      = "Tap a star to rate",
                Font      = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                AutoSize  = true,
                Location  = new Point(24, 52)
            };

            for (int i = 0; i < 5; i++)
            {
                int stars = i + 1;
                _starBtns[i] = new Button
                {
                    Text      = "★",
                    Size      = new Size(52, 52),
                    Location  = new Point(24 + i * 58, 76),
                    Font      = new Font("Segoe UI", 22, FontStyle.Regular),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ForeColor = AppTheme.TextMuted,
                    Cursor    = Cursors.Hand,
                    TabStop   = false
                };
                _starBtns[i].FlatAppearance.BorderSize = 0;
                _starBtns[i].Click += (s, e) => SetStars(stars);
                Controls.Add(_starBtns[i]);
            }

            var lblComment = new Label
            {
                Text      = "Comment (optional)",
                Font      = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                AutoSize  = true,
                Location  = new Point(24, 142)
            };

            _txtComment = AppTheme.MakeTextBox(408, 60);
            _txtComment.Location   = new Point(24, 162);
            _txtComment.Multiline  = true;
            _txtComment.ScrollBars = ScrollBars.Vertical;

            if (!string.IsNullOrWhiteSpace(existingComment))
            {
                _txtComment.Text      = existingComment;
                _txtComment.ForeColor = AppTheme.TextLight;
            }
            else
            {
                AppTheme.AddPlaceholder(_txtComment, "Share your experience…");
            }

            var btnSubmit = AppTheme.MakePrimaryButton(isEdit ? "Update Review" : "Submit Rating", 180, 40);
            btnSubmit.Location = new Point(24, 240);
            btnSubmit.Click   += BtnSubmit_Click;

            var btnCancel = AppTheme.MakeOutlineButton("Cancel", 100, 40);
            btnCancel.Location = new Point(216, 240);
            btnCancel.Click   += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[]
            {
                lblTitle, _lblHint, lblComment, _txtComment, btnSubmit, btnCancel
            });

            // Pre-fill stars if editing
            if (existingStars > 0)
                SetStars(existingStars);
        }

        private void SetStars(int stars)
        {
            SelectedStars = stars;
            _lblHint.Text = stars switch
            {
                1 => "★  Poor",
                2 => "★★  Fair",
                3 => "★★★  Good",
                4 => "★★★★  Very Good",
                5 => "★★★★★  Excellent!",
                _ => "Tap a star to rate"
            };
            _lblHint.ForeColor = AppTheme.Gold;

            for (int i = 0; i < 5; i++)
                _starBtns[i].ForeColor = i < stars ? AppTheme.Gold : AppTheme.TextMuted;
        }

        private void BtnSubmit_Click(object? sender, EventArgs e)
        {
            if (SelectedStars == 0)
            {
                MessageBox.Show("Please select a star rating before submitting.",
                    "Rating Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            RatingComment = AppTheme.GetText(_txtComment, "Share your experience…");
            DialogResult  = DialogResult.OK;
            Close();
        }
    }
}
