using SERVIGO.DAL;
using SERVIGO.Helpers;
using SERVIGO.Theme;
using System.Data;

namespace SERVIGO.Forms.Customer
{
    public partial class frmCustomerDashboard : Form
    {
        private Panel  _pnlSidebar = null!;
        private Panel  _pnlContent = null!;
        private Label  _lblBadge   = null!;
        private System.Windows.Forms.Timer _notifTimer = null!;
        private Button? _activeNav;

        private Panel _pnlHome     = null!;
        private Panel _pnlBrowse   = null!;
        private Panel _pnlBook     = null!;
        private Panel _pnlMyBks    = null!;
        private Panel _pnlReviews  = null!;
        private Panel _pnlFeedback = null!;
        private Panel _pnlNotifs   = null!;

        public frmCustomerDashboard()
        {
            InitializeComponent();
            BuildUI();
            ShowPanel(_pnlHome);
            StartNotifTimer();
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  SHELL
        // ═══════════════════════════════════════════════════════════════════════

        private void BuildUI()
        {
            Text        = "SERVIGO – Customer Dashboard";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 640);
            BackColor   = AppTheme.Background;
            Font        = AppTheme.FontBody;

            var root = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 2,
                RowCount    = 1
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240f));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            _pnlSidebar = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Sidebar };
            _pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Background };

            BuildSidebar();
            BuildAllPanels();

            root.Controls.Add(_pnlSidebar, 0, 0);
            root.Controls.Add(_pnlContent, 1, 0);
            Controls.Add(root);
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  SIDEBAR
        // ═══════════════════════════════════════════════════════════════════════

        private void BuildSidebar()
        {
            _pnlSidebar.Controls.Add(new Label
            {
                Text      = "SERVIGO",
                Font      = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = AppTheme.Accent,
                AutoSize  = true,
                Location  = new Point(20, 24)
            });
            _pnlSidebar.Controls.Add(new Label
            {
                Text      = SessionManager.CurrentUser!.FullName,
                Font      = AppTheme.FontBodyBold,
                ForeColor = Color.White,
                AutoSize  = true,
                Location  = new Point(22, 76)
            });
            _pnlSidebar.Controls.Add(new Label
            {
                Text      = SessionManager.CurrentUser!.UserID,
                Font      = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(140, 170, 210),
                AutoSize  = true,
                Location  = new Point(22, 102)
            });
            _pnlSidebar.Controls.Add(new Panel
            {
                Location  = new Point(20, 138),
                Size      = new Size(196, 1),
                BackColor = Color.FromArgb(50, 80, 120)
            });

            int y = 160;
            var navItems = new (string Icon, string Label, Action Click)[]
            {
                ("🏠", "Home",             () => { ShowPanel(_pnlHome);   }),
                ("🔍", "Browse Services",  () => { LoadBrowse();  ShowPanel(_pnlBrowse); }),
                ("📋", "My Bookings",      () => { LoadMyBookings(); ShowPanel(_pnlMyBks); }),
                ("⭐", "Reviews",          () => { LoadReviews(); ShowPanel(_pnlReviews); }),
                ("📝", "Feedback",         () => { LoadMyFeedback(); ShowPanel(_pnlFeedback); }),
            };

            foreach (var (icon, label, click) in navItems)
            {
                var btn = MakeNavBtn(icon, label, y);
                var c   = click;
                btn.Click += (s, e) => { SetActiveNav(btn); c(); };
                if (y == 126) { _activeNav = btn; SetActiveNav(btn); }
                _pnlSidebar.Controls.Add(btn);
                y += 50;
            }

            // Notification nav
            var btnNotif = MakeNavBtn("🔔", "Notifications", y);
            _lblBadge = new Label
            {
                Size      = new Size(20, 20),
                Location  = new Point(180, y + 14),
                BackColor = AppTheme.Danger,
                ForeColor = Color.White,
                Font      = AppTheme.FontBadge,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible   = false
            };
            btnNotif.Click += (s, e) => { SetActiveNav(btnNotif); LoadNotifications(); ShowPanel(_pnlNotifs); };
            _pnlSidebar.Controls.Add(btnNotif);
            _pnlSidebar.Controls.Add(_lblBadge);

            // Delete Account button
            var btnDeleteAcct = new Button
            {
                Text      = "🗑  Delete Account",
                Size      = new Size(196, 38),
                Location  = new Point(22, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = AppTheme.Danger,
                Font      = AppTheme.FontSmall,
                Cursor    = Cursors.Hand,
                Anchor    = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnDeleteAcct.FlatAppearance.BorderSize = 1;
            btnDeleteAcct.FlatAppearance.BorderColor = AppTheme.Danger;
            btnDeleteAcct.FlatAppearance.MouseOverBackColor = AppTheme.DangerDim;
            btnDeleteAcct.Click += (s, e) => DeleteMyAccount();
            _pnlSidebar.Controls.Add(btnDeleteAcct);

            var btnLogout = new Button
            {
                Text      = "⏻  Logout",
                Size      = new Size(196, 44),
                Location  = new Point(22, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = AppTheme.Danger,
                ForeColor = Color.White,
                Font      = AppTheme.FontBodyBold,
                Cursor    = Cursors.Hand,
                Anchor    = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Logout?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Close();
            };
            _pnlSidebar.Controls.Add(btnLogout);
            _pnlSidebar.Resize += (s, e) =>
            {
                btnLogout.Location     = new Point(22, _pnlSidebar.Height - 60);
                btnDeleteAcct.Location = new Point(22, _pnlSidebar.Height - 106);
            };
        }

        private Button MakeNavBtn(string icon, string label, int y)
        {
            var btn = new Button
            {
                Text      = $"  {icon}   {label}",
                Size      = new Size(236, 46),
                Location  = new Point(2, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(190, 210, 240),
                Font      = AppTheme.FontSidebar,
                Cursor    = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft
            };
            btn.FlatAppearance.BorderSize         = 0;
            btn.FlatAppearance.MouseOverBackColor = AppTheme.SidebarHover;
            return btn;
        }

        private void SetActiveNav(Button btn)
        {
            if (_activeNav != null)
            { _activeNav.BackColor = Color.Transparent; _activeNav.ForeColor = Color.FromArgb(190, 210, 240); }
            btn.BackColor = AppTheme.Primary;
            btn.ForeColor = Color.White;
            _activeNav = btn;
        }

        private void ShowPanel(Panel panel)
        {
            foreach (Control c in _pnlContent.Controls) c.Visible = false;
            panel.Bounds  = new Rectangle(0, 0, _pnlContent.ClientSize.Width, _pnlContent.ClientSize.Height);
            panel.Visible = true;
            panel.BringToFront();
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  BUILD ALL PANELS
        // ═══════════════════════════════════════════════════════════════════════

        private void BuildAllPanels()
        {
            _pnlHome     = BuildHomePanel();
            _pnlBrowse   = BuildBrowsePanel();
            _pnlBook     = BuildBookPanel();
            _pnlMyBks    = BuildMyBookingsPanel();
            _pnlReviews  = BuildReviewsPanel();
            _pnlFeedback = BuildFeedbackPanel();
            _pnlNotifs   = BuildNotificationsPanel();

            foreach (var p in new[] { _pnlHome, _pnlBrowse, _pnlBook, _pnlMyBks,
                                       _pnlReviews, _pnlFeedback, _pnlNotifs })
            {
                p.Visible  = false;
                p.Location = Point.Empty;
                _pnlContent.Controls.Add(p);
            }
            _pnlContent.Resize += (s, e) => FitAllPanels();
            FitAllPanels();
        }

        private void FitAllPanels()
        {
            var sz = _pnlContent.ClientSize;
            foreach (Control c in _pnlContent.Controls)
                c.Bounds = new Rectangle(0, 0, sz.Width, sz.Height);
        }

        // ──────────────────────────────────────────────────────────────────────
        //  HOME
        // ──────────────────────────────────────────────────────────────────────

        private Panel BuildHomePanel()
        {
            var panel = new Panel { BackColor = AppTheme.Background, AutoScroll = true };

            var flow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize      = true,
                WrapContents  = false,
                Dock          = DockStyle.Top,
                Padding       = new Padding(40, 40, 40, 40),
                BackColor     = Color.Transparent
            };

            flow.Controls.Add(new Label
            {
                Text      = $"Hello, {SessionManager.CurrentUser!.FullName.Split(' ')[0]}!  👋",
                Font      = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = AppTheme.Primary,
                AutoSize  = true,
                Margin    = new Padding(0, 0, 0, 16)
            });
            flow.Controls.Add(new Label
            {
                Text      = "What service are you looking for today?",
                Font      = AppTheme.FontSubtitle,
                ForeColor = AppTheme.TextMuted,
                AutoSize  = true,
                Margin    = new Padding(0, 0, 0, 44)
            });

            // Quick action cards
            var services = new[]
            {
                ("⚡", "Electrician"), ("🔧", "Plumber"), ("🚗", "Mechanic"),
                ("🧺", "Laundry"), ("🎨", "Painter"), ("🪚", "Carpenter"),
                ("🧹", "Cleaner"), ("❄️", "AC Repair")
            };

            var cardRow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize      = true,
                WrapContents  = true,
                BackColor     = Color.Transparent
            };

            foreach (var (icon, name) in services)
            {
                var card = new Button
                {
                    Text      = $"{icon}\n{name}",
                    Size      = new Size(130, 110),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = AppTheme.CardBg,
                    ForeColor = AppTheme.TextDark,
                    Font      = AppTheme.FontBodyBold,
                    Cursor    = Cursors.Hand,
                    Margin    = new Padding(0, 0, 16, 16),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                card.FlatAppearance.BorderColor = AppTheme.Border;
                card.FlatAppearance.BorderSize  = 1;
                card.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 242, 252);
                card.Click += (s, e) =>
                {
                    LoadBrowse();
                    ShowPanel(_pnlBrowse);
                };
                cardRow.Controls.Add(card);
            }

            flow.Controls.Add(cardRow);

            var btnBrowse = AppTheme.MakePrimaryButton("Browse All Services →", 260, 48);
            btnBrowse.Margin = new Padding(0, 20, 0, 0);
            btnBrowse.Click += (s, e) => { LoadBrowse(); ShowPanel(_pnlBrowse); };
            flow.Controls.Add(btnBrowse);

            panel.Controls.Add(flow);
            return panel;
        }

        // ──────────────────────────────────────────────────────────────────────
        //  BROWSE SERVICES
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvServices = null!;
        private TextBox      _txtSearch   = null!;
        private ComboBox     _cboCat      = null!;

        private Panel BuildBrowsePanel()
        {
            var panel  = new Panel { BackColor = AppTheme.Background };
            var header = MakePanelHeader("Browse Services",
                "Search by service name or filter by category.");

            var toolbar = new Panel
            {
                Height = 64, Dock = DockStyle.Top,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 10, 16, 10)
            };

            _txtSearch = AppTheme.MakeTextBox(240, 40);
            AppTheme.AddPlaceholder(_txtSearch, "Search services…");
            _txtSearch.Location = new Point(16, 12);

            _cboCat = AppTheme.MakeComboBox(200, 40);
            _cboCat.Location = new Point(268, 12);
            LoadCategoriesToFilter();

            var btnSearch = AppTheme.MakePrimaryButton("Search", 100, 40);
            btnSearch.Location = new Point(480, 12);
            btnSearch.Click   += (s, e) => LoadBrowse();

            toolbar.Controls.Add(_txtSearch);
            toolbar.Controls.Add(_cboCat);
            toolbar.Controls.Add(btnSearch);

            _dgvServices = AppTheme.MakeDataGrid();
            _dgvServices.Dock = DockStyle.Fill;

            var pnlAction = new Panel
            {
                Height = 56, Dock = DockStyle.Bottom,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 8, 16, 8)
            };
            var btnBook = AppTheme.MakeAccentButton("Book Selected Service →", 260, 40);
            btnBook.Location = new Point(16, 8);
            btnBook.Click   += (s, e) => OpenBookingForSelected();
            pnlAction.Controls.Add(btnBook);

            panel.Controls.Add(_dgvServices);
            panel.Controls.Add(pnlAction);
            panel.Controls.Add(toolbar);   // Top — below header
            panel.Controls.Add(header);    // Top — last → topmost

            return panel;
        }

        private void LoadCategoriesToFilter()
        {
            try
            {
                var dt = ProviderDAL.GetCategories();
                var allRow = dt.NewRow();
                allRow["CategoryID"]   = DBNull.Value;
                allRow["CategoryName"] = "All Categories";
                dt.Rows.InsertAt(allRow, 0);
                _cboCat.DisplayMember = "CategoryName";
                _cboCat.ValueMember   = "CategoryID";
                _cboCat.DataSource    = dt;
            }
            catch { }
        }

        private void LoadBrowse()
        {
            try
            {
                string keyword  = AppTheme.GetText(_txtSearch, "Search services…");
                int?   catID    = _cboCat.SelectedValue is DBNull or null ? null
                                  : (int?)Convert.ToInt32(_cboCat.SelectedValue);

                var dt = ServiceDAL.SearchServices(keyword, catID);
                _dgvServices.DataSource = dt;
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void OpenBookingForSelected()
        {
            if (_dgvServices.CurrentRow == null) { ShowInfo("Select a service first."); return; }

            int    svcID      = Convert.ToInt32(_dgvServices.CurrentRow.Cells["ServiceID"].Value);
            int    providerID = Convert.ToInt32(_dgvServices.CurrentRow.Cells["ProviderID"].Value);
            string svcName    = _dgvServices.CurrentRow.Cells["ServiceName"].Value.ToString()!;

            LoadBookPanel(svcID, providerID, svcName);
            ShowPanel(_pnlBook);
        }

        // ──────────────────────────────────────────────────────────────────────
        //  BOOK APPOINTMENT
        // ──────────────────────────────────────────────────────────────────────

        private Label    _lblBookService  = null!;
        private ComboBox _cboSlots        = null!;
        private TextBox  _txtNotes        = null!;
        private int      _bookingProvID;
        private int      _bookingSvcID;

        private Panel BuildBookPanel()
        {
            var panel  = new Panel { BackColor = AppTheme.Background };
            var header = MakePanelHeader("Book Appointment",
                "Select a time slot and confirm your booking.");

            // Content area — Dock.Fill + AutoScroll, sits below header
            var content = new Panel
            {
                Dock       = DockStyle.Fill,
                AutoScroll = true,
                BackColor  = Color.Transparent,
                Padding    = new Padding(32, 24, 32, 24)
            };

            var flow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize      = true,
                WrapContents  = false,
                Dock          = DockStyle.Top,
                BackColor     = Color.Transparent
            };

            _lblBookService = new Label
            {
                Text      = "Service: –",
                Font      = AppTheme.FontSubtitle,
                ForeColor = AppTheme.Primary,
                AutoSize  = true,
                Margin    = new Padding(0, 0, 0, 20)
            };
            flow.Controls.Add(_lblBookService);

            var lblSlotHdr = FieldLabel("Available Time Slots  (next 7 days)", Point.Empty);
            lblSlotHdr.Margin = new Padding(0, 0, 0, 6);
            flow.Controls.Add(lblSlotHdr);

            _cboSlots = AppTheme.MakeComboBox(460, 44);
            _cboSlots.Margin = new Padding(0, 0, 0, 20);
            flow.Controls.Add(_cboSlots);

            var lblNotesHdr = FieldLabel("Notes  (optional)", Point.Empty);
            lblNotesHdr.Margin = new Padding(0, 0, 0, 6);
            flow.Controls.Add(lblNotesHdr);

            _txtNotes = new TextBox
            {
                Size        = new Size(460, 80),
                Multiline   = true,
                Font        = AppTheme.FontInput,
                BackColor   = AppTheme.InputBg,
                ForeColor   = AppTheme.TextLight,
                BorderStyle = BorderStyle.FixedSingle,
                Margin      = new Padding(0, 0, 0, 24)
            };
            AppTheme.AddPlaceholder(_txtNotes, "Any special instructions…");
            flow.Controls.Add(_txtNotes);

            // Buttons row
            var btnRow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize      = true,
                WrapContents  = false,
                BackColor     = Color.Transparent,
                Margin        = new Padding(0)
            };

            var btnConfirm = AppTheme.MakePrimaryButton("✔  Confirm Booking", 220, 48);
            btnConfirm.Margin = new Padding(0, 0, 12, 0);
            btnConfirm.Click += BtnConfirmBooking_Click;
            btnRow.Controls.Add(btnConfirm);

            var btnBack = AppTheme.MakeOutlineButton("← Back to Services", 200, 48);
            btnBack.Click += (s, e) => ShowPanel(_pnlBrowse);
            btnRow.Controls.Add(btnBack);

            flow.Controls.Add(btnRow);

            content.Controls.Add(flow);
            panel.Controls.Add(content);   // Fill
            panel.Controls.Add(header);    // Top — last → topmost

            return panel;
        }

        private void LoadBookPanel(int svcID, int provID, string svcName)
        {
            _bookingSvcID = svcID;
            _bookingProvID = provID;
            _lblBookService.Text = $"Service:  {svcName}";

            try
            {
                var dt = BookingDAL.GetAvailableSlots(provID);
                _cboSlots.Items.Clear();
                _cboSlots.DisplayMember = string.Empty;
                _cboSlots.ValueMember   = string.Empty;

                var items = new List<(string Display, int SlotID)>();
                foreach (DataRow r in dt.Rows)
                {
                    string display = $"{Convert.ToDateTime(r["SlotDate"]):ddd, MMM dd}   " +
                                     $"{(TimeSpan)r["StartTime"]:hh\\:mm} – {(TimeSpan)r["EndTime"]:hh\\:mm}";
                    items.Add((display, Convert.ToInt32(r["SlotID"])));
                }

                _cboSlots.DataSource    = items.Select(x => new { x.Display, x.SlotID }).ToList();
                _cboSlots.DisplayMember = "Display";
                _cboSlots.ValueMember   = "SlotID";

                if (_cboSlots.Items.Count == 0)
                    ShowInfo("No available slots in the next 7 days for this provider.");
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void BtnConfirmBooking_Click(object? sender, EventArgs e)
        {
            if (_cboSlots.SelectedValue == null || _cboSlots.Items.Count == 0)
            { ShowInfo("Please select a time slot."); return; }

            int    slotID = Convert.ToInt32(_cboSlots.SelectedValue);
            string notes  = AppTheme.GetText(_txtNotes, "Any special instructions…");
            string custID = SessionManager.CurrentUser!.UserID;

            try
            {
                int bookingID = BookingDAL.CreateBooking(custID, slotID, _bookingSvcID,
                    string.IsNullOrWhiteSpace(notes) ? null : notes);

                MessageBox.Show(
                    $"Booking confirmed!\nBooking ID: #{bookingID}\n\n" +
                    "Status: Pending  (waiting for provider acceptance)",
                    "SERVIGO – Booking Created",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadMyBookings();
                ShowPanel(_pnlMyBks);
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  MY BOOKINGS
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvMyBks = null!;

        private Panel BuildMyBookingsPanel()
        {
            var panel  = new Panel { BackColor = AppTheme.Background };
            var header = MakePanelHeader("My Bookings",
                "All your bookings and their current status.");

            var toolbar = new Panel
            {
                Height = 60, Dock = DockStyle.Top,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 10, 16, 10)
            };
            var btnRefresh = AppTheme.MakePrimaryButton("Refresh", 110, 38);
            btnRefresh.Location = new Point(16, 11);
            btnRefresh.Click   += (s, e) => LoadMyBookings();
            toolbar.Controls.Add(btnRefresh);

            _dgvMyBks = AppTheme.MakeDataGrid();
            _dgvMyBks.Dock = DockStyle.Fill;

            var pnlAction = new Panel
            {
                Height = 56, Dock = DockStyle.Bottom,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 8, 16, 8)
            };
            var btnCancel = AppTheme.MakeDangerButton("Cancel Booking", 160, 38);
            btnCancel.Location = new Point(16, 9);
            btnCancel.Click   += BtnCancelBooking_Click;

            var btnRate = AppTheme.MakeSuccessButton("⭐ Rate Provider", 160, 38);
            btnRate.Location = new Point(190, 9);
            btnRate.Click   += BtnRate_Click;

            var lblNote = new Label
            {
                Text      = "  Cancel: Pending only.  Rate: Completed only.",
                Font      = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                AutoSize  = true,
                Location  = new Point(364, 18)
            };
            pnlAction.Controls.Add(btnCancel);
            pnlAction.Controls.Add(btnRate);
            pnlAction.Controls.Add(lblNote);

            panel.Controls.Add(_dgvMyBks);
            panel.Controls.Add(pnlAction);
            panel.Controls.Add(toolbar);   // Top — below header
            panel.Controls.Add(header);    // Top — last → topmost

            return panel;
        }

        private void LoadMyBookings()
        {
            try
            {
                var dt = BookingDAL.GetCustomerBookings(SessionManager.CurrentUser!.UserID);
                _dgvMyBks.DataSource = dt;

                // Hide internal columns from display
                if (_dgvMyBks.Columns["ProviderID"] != null)
                    _dgvMyBks.Columns["ProviderID"]!.Visible = false;
                if (_dgvMyBks.Columns["HasRated"] != null)
                    _dgvMyBks.Columns["HasRated"]!.Visible = false;
                if (_dgvMyBks.Columns["StatusID"] != null)
                    _dgvMyBks.Columns["StatusID"]!.Visible = false;
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void BtnCancelBooking_Click(object? sender, EventArgs e)
        {
            if (_dgvMyBks.CurrentRow == null) return;
            int bkID     = Convert.ToInt32(_dgvMyBks.CurrentRow.Cells["BookingID"].Value);
            int statusID = Convert.ToInt32(_dgvMyBks.CurrentRow.Cells["StatusID"].Value);

            if (statusID != 1)
            { ShowInfo("Only Pending bookings can be cancelled."); return; }

            if (MessageBox.Show($"Cancel Booking #{bkID}?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                BookingDAL.UpdateStatus(bkID, 4, SessionManager.CurrentUser!.UserID);
                LoadMyBookings();
            }
        }

        private void BtnRate_Click(object? sender, EventArgs e)
        {
            if (_dgvMyBks.CurrentRow == null) return;

            int    bkID       = Convert.ToInt32(_dgvMyBks.CurrentRow.Cells["BookingID"].Value);
            int    statusID   = Convert.ToInt32(_dgvMyBks.CurrentRow.Cells["StatusID"].Value);
            int    providerID = Convert.ToInt32(_dgvMyBks.CurrentRow.Cells["ProviderID"].Value);
            int    hasRated   = Convert.ToInt32(_dgvMyBks.CurrentRow.Cells["HasRated"].Value);
            string provider   = _dgvMyBks.CurrentRow.Cells["ProviderName"].Value?.ToString() ?? "Provider";

            if (statusID != 3)  // 3 = Completed
            { ShowInfo("You can only rate Completed bookings."); return; }

            if (hasRated == 1)
            { ShowInfo("You have already rated this booking."); return; }

            using var dlg = new frmRating(provider);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    RatingDAL.SubmitRating(bkID, providerID,
                        SessionManager.CurrentUser!.UserID,
                        dlg.SelectedStars, dlg.RatingComment);
                    LoadMyBookings();
                    ShowInfo($"Thank you! You rated {provider}  {new string('★', dlg.SelectedStars)}");
                }
                catch (Exception ex) { ShowError($"Could not submit rating:\n{ex.Message}"); }
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  REVIEWS PANEL
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvUnreviewed = null!;
        private DataGridView _dgvReviewed   = null!;

        private Panel BuildReviewsPanel()
        {
            var panel  = new Panel { BackColor = AppTheme.Background };
            var header = MakePanelHeader("Reviews", "Rate completed bookings and manage your reviews.");

            var content = new Panel
            {
                Dock       = DockStyle.Fill,
                AutoScroll = true,
                BackColor  = Color.Transparent,
                Padding    = new Padding(24, 16, 24, 16)
            };

            var flow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize      = true,
                WrapContents  = false,
                Dock          = DockStyle.Top,
                BackColor     = Color.Transparent
            };

            // Unreviewed section
            var lblPending = AppTheme.MakeLabel("Pending Reviews", AppTheme.FontSubtitle, AppTheme.Gold);
            lblPending.Margin = new Padding(0, 0, 0, 8);
            flow.Controls.Add(lblPending);

            _dgvUnreviewed = AppTheme.MakeDataGrid();
            _dgvUnreviewed.Size = new Size(900, 200);
            _dgvUnreviewed.Margin = new Padding(0, 0, 0, 8);
            flow.Controls.Add(_dgvUnreviewed);

            var btnRate = AppTheme.MakePrimaryButton("⭐ Write Review", 180, 40);
            btnRate.Margin = new Padding(0, 0, 0, 24);
            btnRate.Click += BtnReviewRate_Click;
            flow.Controls.Add(btnRate);

            // Reviewed section
            var lblDone = AppTheme.MakeLabel("Your Reviews", AppTheme.FontSubtitle, AppTheme.Success);
            lblDone.Margin = new Padding(0, 0, 0, 8);
            flow.Controls.Add(lblDone);

            _dgvReviewed = AppTheme.MakeDataGrid();
            _dgvReviewed.Size = new Size(900, 200);
            _dgvReviewed.Margin = new Padding(0, 0, 0, 8);
            flow.Controls.Add(_dgvReviewed);

            var btnEdit = AppTheme.MakeOutlineButton("✏ Edit Review", 160, 40);
            btnEdit.Margin = new Padding(0, 0, 0, 16);
            btnEdit.Click += BtnReviewEdit_Click;
            flow.Controls.Add(btnEdit);

            content.Controls.Add(flow);
            panel.Controls.Add(content);
            panel.Controls.Add(header);

            return panel;
        }

        private void LoadReviews()
        {
            try
            {
                string uid = SessionManager.CurrentUser!.UserID;
                _dgvUnreviewed.DataSource = RatingDAL.GetUnreviewedBookings(uid);
                _dgvReviewed.DataSource   = RatingDAL.GetReviewedBookings(uid);

                if (_dgvUnreviewed.Columns["ProviderID"] != null)
                    _dgvUnreviewed.Columns["ProviderID"]!.Visible = false;
                if (_dgvReviewed.Columns["ProviderID"] != null)
                    _dgvReviewed.Columns["ProviderID"]!.Visible = false;
                if (_dgvReviewed.Columns["BookingID"] != null)
                    _dgvReviewed.Columns["BookingID"]!.Visible = false;
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void BtnReviewRate_Click(object? sender, EventArgs e)
        {
            if (_dgvUnreviewed.CurrentRow == null)
            { ShowInfo("Select a booking to review."); return; }

            int    bkID     = Convert.ToInt32(_dgvUnreviewed.CurrentRow.Cells["BookingID"].Value);
            int    provID   = Convert.ToInt32(_dgvUnreviewed.CurrentRow.Cells["ProviderID"].Value);
            string provider = _dgvUnreviewed.CurrentRow.Cells["ProviderName"].Value?.ToString() ?? "Provider";

            using var dlg = new frmRating(provider);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    RatingDAL.SubmitRating(bkID, provID,
                        SessionManager.CurrentUser!.UserID,
                        dlg.SelectedStars, dlg.RatingComment);
                    LoadReviews();
                    ShowInfo($"Thank you! You rated {provider}  {new string('★', dlg.SelectedStars)}");
                }
                catch (Exception ex) { ShowError(ex.Message); }
            }
        }

        private void BtnReviewEdit_Click(object? sender, EventArgs e)
        {
            if (_dgvReviewed.CurrentRow == null)
            { ShowInfo("Select a review to edit."); return; }

            int    bkID     = Convert.ToInt32(_dgvReviewed.CurrentRow.Cells["BookingID"].Value);
            int    oldStars = Convert.ToInt32(_dgvReviewed.CurrentRow.Cells["Stars"].Value);
            string oldComm  = _dgvReviewed.CurrentRow.Cells["Comment"].Value?.ToString() ?? "";
            string provider = _dgvReviewed.CurrentRow.Cells["ProviderName"].Value?.ToString() ?? "Provider";

            using var dlg = new frmRating(provider, oldStars, oldComm);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    RatingDAL.UpdateRating(bkID, dlg.SelectedStars, dlg.RatingComment);
                    LoadReviews();
                    ShowInfo($"Review updated!  {new string('★', dlg.SelectedStars)}");
                }
                catch (Exception ex) { ShowError(ex.Message); }
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  FEEDBACK & REPORTS PANEL
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvFeedback = null!;

        private Panel BuildFeedbackPanel()
        {
            var panel  = new Panel { BackColor = AppTheme.Background };
            var header = MakePanelHeader("Feedback & Reports",
                "Share feedback, report system issues, or report a provider.");

            var content = new Panel
            {
                Dock       = DockStyle.Fill,
                AutoScroll = true,
                BackColor  = Color.Transparent,
                Padding    = new Padding(24, 16, 24, 16)
            };

            var flow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize      = true,
                WrapContents  = false,
                Dock          = DockStyle.Top,
                BackColor     = Color.Transparent
            };

            // Type selector
            var lblType = AppTheme.MakeLabel("Report Type", AppTheme.FontBodyBold);
            lblType.Margin = new Padding(0, 0, 0, 4);
            flow.Controls.Add(lblType);

            var cboType = AppTheme.MakeComboBox(300, 36);
            cboType.Items.AddRange(new[] { "Feedback", "Report System Issue", "Report a Provider" });
            cboType.SelectedIndex = 0;
            cboType.Margin = new Padding(0, 0, 0, 12);
            flow.Controls.Add(cboType);

            // Subject
            var lblSubj = AppTheme.MakeLabel("Subject", AppTheme.FontBodyBold);
            lblSubj.Margin = new Padding(0, 0, 0, 4);
            flow.Controls.Add(lblSubj);

            var txtSubj = AppTheme.MakeTextBox(500, 36);
            AppTheme.AddPlaceholder(txtSubj, "Brief subject line…");
            txtSubj.Margin = new Padding(0, 0, 0, 12);
            flow.Controls.Add(txtSubj);

            // Description
            var lblDesc = AppTheme.MakeLabel("Description", AppTheme.FontBodyBold);
            lblDesc.Margin = new Padding(0, 0, 0, 4);
            flow.Controls.Add(lblDesc);

            var txtDesc = AppTheme.MakeTextBox(500, 80);
            txtDesc.Multiline  = true;
            txtDesc.ScrollBars = ScrollBars.Vertical;
            AppTheme.AddPlaceholder(txtDesc, "Describe in detail…");
            txtDesc.Margin = new Padding(0, 0, 0, 12);
            flow.Controls.Add(txtDesc);

            // Submit button
            var btnSubmit = AppTheme.MakePrimaryButton("Submit Report", 180, 40);
            btnSubmit.Margin = new Padding(0, 0, 0, 24);
            btnSubmit.Click += (s, e) =>
            {
                string type = cboType.SelectedItem?.ToString() ?? "Feedback";
                string subj = AppTheme.GetText(txtSubj, "Brief subject line…");
                string desc = AppTheme.GetText(txtDesc, "Describe in detail…");

                if (string.IsNullOrWhiteSpace(subj) || string.IsNullOrWhiteSpace(desc))
                { ShowInfo("Please fill in both subject and description."); return; }

                try
                {
                    FeedbackDAL.Submit(SessionManager.CurrentUser!.UserID, type, null, subj, desc);
                    ShowInfo("Your report has been submitted. Admin will review it.");
                    AppTheme.AddPlaceholder(txtSubj, "Brief subject line…");
                    AppTheme.AddPlaceholder(txtDesc, "Describe in detail…");
                    LoadMyFeedback();
                }
                catch (Exception ex) { ShowError(ex.Message); }
            };
            flow.Controls.Add(btnSubmit);

            // History
            var lblHistory = AppTheme.MakeLabel("Your Submissions", AppTheme.FontSubtitle, AppTheme.Info);
            lblHistory.Margin = new Padding(0, 0, 0, 8);
            flow.Controls.Add(lblHistory);

            _dgvFeedback = AppTheme.MakeDataGrid();
            _dgvFeedback.Size   = new Size(900, 200);
            _dgvFeedback.Margin = new Padding(0, 0, 0, 16);
            flow.Controls.Add(_dgvFeedback);

            content.Controls.Add(flow);
            panel.Controls.Add(content);
            panel.Controls.Add(header);

            return panel;
        }

        private void LoadMyFeedback()
        {
            try
            {
                _dgvFeedback.DataSource = FeedbackDAL.GetByUser(SessionManager.CurrentUser!.UserID);
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  NOTIFICATIONS
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvNotifs = null!;

        private Panel BuildNotificationsPanel()
        {
            var panel  = new Panel { BackColor = AppTheme.Background };
            var header = MakePanelHeader("Notifications", "Your booking updates and alerts.");

            var toolbar = new Panel
            {
                Height = 60, Dock = DockStyle.Top,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 10, 16, 10)
            };
            var btnMarkAll = AppTheme.MakePrimaryButton("Mark All Read", 150, 38);
            btnMarkAll.Location = new Point(16, 11);
            btnMarkAll.Click   += (s, e) =>
            {
                NotificationDAL.MarkAllRead(SessionManager.CurrentUser!.UserID);
                LoadNotifications();
                UpdateBadge();
            };
            toolbar.Controls.Add(btnMarkAll);

            _dgvNotifs = AppTheme.MakeDataGrid();
            _dgvNotifs.Dock = DockStyle.Fill;

            panel.Controls.Add(_dgvNotifs);
            panel.Controls.Add(toolbar);   // Top — below header
            panel.Controls.Add(header);    // Top — last → topmost

            return panel;
        }

        private void LoadNotifications()
        {
            try
            {
                var dt = NotificationDAL.GetByUser(SessionManager.CurrentUser!.UserID);
                _dgvNotifs.DataSource = dt;
                NotificationDAL.MarkAllRead(SessionManager.CurrentUser!.UserID);
                UpdateBadge();
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  NOTIFICATION BADGE
        // ═══════════════════════════════════════════════════════════════════════

        private void StartNotifTimer()
        {
            _notifTimer = new System.Windows.Forms.Timer { Interval = 30000 };
            _notifTimer.Tick += (s, e) => UpdateBadge();
            _notifTimer.Start();
            UpdateBadge();
        }

        private void UpdateBadge()
        {
            try
            {
                int count = NotificationDAL.GetUnreadCount(SessionManager.CurrentUser!.UserID);
                _lblBadge.Text    = count > 99 ? "99+" : count.ToString();
                _lblBadge.Visible = count > 0;
            }
            catch { }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _notifTimer?.Stop();
            _notifTimer?.Dispose();
            base.OnFormClosed(e);
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════════════════════

        private static Panel MakePanelHeader(string title, string subtitle)
        {
            var p = new Panel { Height = 110, BackColor = AppTheme.CardBg, Dock = DockStyle.Top };
            p.Controls.Add(new Label
            {
                Text = title, Font = AppTheme.FontTitle, ForeColor = AppTheme.Primary,
                AutoSize = true, Location = new Point(28, 20)
            });
            p.Controls.Add(new Label
            {
                Text = subtitle, Font = AppTheme.FontBody, ForeColor = AppTheme.TextMuted,
                AutoSize = true, Location = new Point(28, 68)
            });
            p.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 3, BackColor = AppTheme.Accent });
            return p;
        }

        private static Label FieldLabel(string text, Point loc)
            => new() { Text = text, Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextDark,
                        AutoSize = true, Location = loc };

        private void DeleteMyAccount()
        {
            var result = MessageBox.Show(
                "Are you sure you want to permanently delete your account?\n\n" +
                "All your bookings and data will be removed.\nThis cannot be undone.",
                "Delete Account",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            // Double confirmation
            var confirm = MessageBox.Show(
                "This is your last chance — are you absolutely sure?",
                "Final Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Stop);

            if (confirm != DialogResult.Yes) return;

            try
            {
                string uid = SessionManager.CurrentUser!.UserID;
                UserDAL.DeleteUser(uid);
                SessionManager.Logout();

                MessageBox.Show("Your account has been deleted.",
                    "Account Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                ShowError($"Could not delete account:\n{ex.Message}");
            }
        }

        private void ShowError(string msg)
            => MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void ShowInfo(string msg)
            => MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
