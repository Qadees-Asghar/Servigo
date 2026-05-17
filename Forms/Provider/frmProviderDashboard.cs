using SERVIGO.DAL;
using SERVIGO.Helpers;
using SERVIGO.Models;
using SERVIGO.Theme;
using System.Data;

namespace SERVIGO.Forms.Provider
{
    public partial class frmProviderDashboard : Form
    {
        private Panel  _pnlSidebar = null!;
        private Panel  _pnlContent = null!;
        private Label  _lblBadge   = null!;
        private System.Windows.Forms.Timer _notifTimer = null!;
        private Button? _activeNav;

        private Panel _pnlHome     = null!;
        private Panel _pnlServices = null!;
        private Panel _pnlSchedule = null!;
        private Panel _pnlBookings = null!;
        private Panel _pnlNotifs   = null!;

        private int ProviderID => SessionManager.CurrentProviderID ?? 0;

        public frmProviderDashboard()
        {
            if (!SessionManager.IsProvider || ProviderID == 0)
            {
                MessageBox.Show("Provider account not fully configured.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Load += (s, e) => Close();
                return;
            }

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
            Text        = "SERVIGO – Provider Dashboard";
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
                Location  = new Point(22, 60)
            });
            _pnlSidebar.Controls.Add(new Label
            {
                Text      = "Provider  |  " + SessionManager.CurrentUser!.UserID,
                Font      = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(140, 170, 210),
                AutoSize  = true,
                Location  = new Point(22, 82)
            });
            _pnlSidebar.Controls.Add(new Panel
            {
                Location  = new Point(20, 108),
                Size      = new Size(196, 1),
                BackColor = Color.FromArgb(50, 80, 120)
            });

            int y = 124;
            var navItems = new (string Icon, string Label, Action Click)[]
            {
                ("🏠", "Dashboard",         () => ShowPanel(_pnlHome)),
                ("🔧", "My Services",       () => { LoadServices();  ShowPanel(_pnlServices); }),
                ("📅", "My Schedule",       () => { LoadSchedule();  ShowPanel(_pnlSchedule); }),
                ("📋", "Incoming Bookings", () => { LoadBookings();  ShowPanel(_pnlBookings); }),
            };

            foreach (var (icon, label, click) in navItems)
            {
                var btn = MakeNavBtn(icon, label, y);
                var c   = click;
                btn.Click += (s, e) => { SetActiveNav(btn); c(); };
                if (y == 124) { _activeNav = btn; SetActiveNav(btn); }
                _pnlSidebar.Controls.Add(btn);
                y += 50;
            }

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
                btnLogout.Location = new Point(22, _pnlSidebar.Height - 60);
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
            _pnlServices = BuildServicesPanel();
            _pnlSchedule = BuildSchedulePanel();
            _pnlBookings = BuildBookingsPanel();
            _pnlNotifs   = BuildNotificationsPanel();

            foreach (var p in new[] { _pnlHome, _pnlServices, _pnlSchedule, _pnlBookings, _pnlNotifs })
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
                Padding       = new Padding(40),
                BackColor     = Color.Transparent
            };

            flow.Controls.Add(new Label
            {
                Text      = $"Welcome,  {SessionManager.CurrentUser!.FullName.Split(' ')[0]}! 👷",
                Font      = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = AppTheme.Primary,
                AutoSize  = true,
                Margin    = new Padding(0, 0, 0, 8)
            });

            var prov = ProviderDAL.GetProviderByUserID(SessionManager.CurrentUser!.UserID);
            string statusMsg = prov?.IsApproved == true
                ? "✔  Verified Provider  — badge granted by admin."
                : "○  Not yet verified  — you can fully use all features.";
            Color statusColor = prov?.IsApproved == true ? AppTheme.Success : AppTheme.TextMuted;

            flow.Controls.Add(new Label
            {
                Text      = statusMsg,
                Font      = AppTheme.FontBodyBold,
                ForeColor = statusColor,
                AutoSize  = true,
                Margin    = new Padding(0, 0, 0, 32)
            });

            // Quick stat cards
            var cardRow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize      = true,
                BackColor     = Color.Transparent,
                WrapContents  = false
            };

            try
            {
                int completed = prov != null
                    ? Convert.ToInt32(SERVIGO.Helpers.DatabaseHelper.ExecuteScalar(
                        $"SELECT dbo.fn_GetProviderCompletedCount({prov.ProviderID})"))
                    : 0;

                var dt = BookingDAL.GetProviderBookings(ProviderID);
                int pending = dt.AsEnumerable()
                    .Count(r => Convert.ToInt32(r["StatusID"]) == 1);

                var c1 = AppTheme.MakeStatCard("Total Completed", completed.ToString(), AppTheme.Success, 200, 120);
                var c2 = AppTheme.MakeStatCard("Pending Bookings", pending.ToString(), AppTheme.Warning, 200, 120);
                c1.Margin = new Padding(0, 0, 20, 0);
                cardRow.Controls.Add(c1);
                cardRow.Controls.Add(c2);
            }
            catch { }

            flow.Controls.Add(cardRow);

            // Quick links
            var btnAddSlot = AppTheme.MakePrimaryButton("+ Add Time Slot", 220, 46);
            btnAddSlot.Margin = new Padding(0, 32, 0, 12);
            btnAddSlot.Click += (s, e) => { LoadSchedule(); ShowPanel(_pnlSchedule); };

            var btnViewBks = AppTheme.MakeAccentButton("View Bookings", 200, 46);
            btnViewBks.Margin = new Padding(16, 32, 0, 12);
            btnViewBks.Click += (s, e) => { LoadBookings(); ShowPanel(_pnlBookings); };

            var btnRow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize      = true,
                BackColor     = Color.Transparent,
                Margin        = new Padding(0)
            };
            btnRow.Controls.Add(btnAddSlot);
            btnRow.Controls.Add(btnViewBks);
            flow.Controls.Add(btnRow);

            panel.Controls.Add(flow);
            return panel;
        }

        // ──────────────────────────────────────────────────────────────────────
        //  MY SERVICES
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvServices = null!;
        private TextBox _txtSvcName  = null!;
        private TextBox _txtSvcDesc  = null!;
        private TextBox _txtSvcPrice = null!;
        private TextBox _txtSvcDur   = null!;

        private Panel BuildServicesPanel()
        {
            var panel  = new Panel { BackColor = AppTheme.Background };
            var header = MakePanelHeader("My Services", "Add, edit or remove your offered services.");

            // Add service form card
            var card = new Panel
            {
                Height    = 130,
                Dock      = DockStyle.Top,
                BackColor = AppTheme.CardBg,
                Padding   = new Padding(20, 10, 20, 10)
            };

            card.Controls.Add(new Label
            {
                Text = "Add New Service", Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary, AutoSize = true, Location = new Point(20, 10)
            });

            // Field labels row
            var lblName = new Label { Text = "Name", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(20, 38) };
            var lblDesc = new Label { Text = "Description", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(232, 38) };
            var lblPrc  = new Label { Text = "Price (PKR)", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(506, 38) };
            var lblDur  = new Label { Text = "Duration (min)", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(618, 38) };
            card.Controls.Add(lblName); card.Controls.Add(lblDesc); card.Controls.Add(lblPrc); card.Controls.Add(lblDur);

            // Input fields row — below labels
            _txtSvcName  = AppTheme.MakeTextBox(200, 36); _txtSvcName.Location  = new Point(20, 56);
            _txtSvcDesc  = AppTheme.MakeTextBox(260, 36); _txtSvcDesc.Location  = new Point(232, 56);
            _txtSvcPrice = AppTheme.MakeTextBox(100, 36); _txtSvcPrice.Location = new Point(506, 56);
            _txtSvcDur   = AppTheme.MakeTextBox(100, 36); _txtSvcDur.Location   = new Point(618, 56);

            AppTheme.AddPlaceholder(_txtSvcName,  "Service name…");
            AppTheme.AddPlaceholder(_txtSvcDesc,  "Short description…");
            AppTheme.AddPlaceholder(_txtSvcPrice, "Price (PKR)");
            AppTheme.AddPlaceholder(_txtSvcDur,   "Duration (min)");

            var btnAdd = AppTheme.MakeSuccessButton("+ Add Service", 140, 36);
            btnAdd.Location = new Point(730, 56);
            btnAdd.Click   += BtnAddService_Click;

            card.Controls.Add(_txtSvcName);
            card.Controls.Add(_txtSvcDesc);
            card.Controls.Add(_txtSvcPrice);
            card.Controls.Add(_txtSvcDur);
            card.Controls.Add(btnAdd);

            _dgvServices = AppTheme.MakeDataGrid();
            _dgvServices.Dock = DockStyle.Fill;

            var pnlAction = new Panel
            {
                Height = 56, Dock = DockStyle.Bottom,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 8, 16, 8)
            };
            var btnDelete = AppTheme.MakeDangerButton("Deactivate Service", 180, 38);
            btnDelete.Location = new Point(16, 9);
            btnDelete.Click   += (s, e) => DeactivateService();
            pnlAction.Controls.Add(btnDelete);

            panel.Controls.Add(_dgvServices);
            panel.Controls.Add(pnlAction);
            panel.Controls.Add(card);      // Top — below header
            panel.Controls.Add(header);    // Top — last → topmost

            return panel;
        }

        private void LoadServices()
        {
            try
            {
                _dgvServices.DataSource = ServiceDAL.GetByProvider(ProviderID);
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void BtnAddService_Click(object? sender, EventArgs e)
        {
            string name  = AppTheme.GetText(_txtSvcName,  "Service name…");
            string desc  = AppTheme.GetText(_txtSvcDesc,  "Short description…");
            string price = AppTheme.GetText(_txtSvcPrice, "Price (PKR)");
            string dur   = AppTheme.GetText(_txtSvcDur,   "Duration (min)");

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(price) ||
                string.IsNullOrWhiteSpace(dur))
            { ShowInfo("Name, Price, and Duration are required."); return; }

            if (!decimal.TryParse(price, out decimal priceVal) || priceVal <= 0)
            { ShowInfo("Enter a valid price."); return; }
            if (!int.TryParse(dur, out int durVal) || durVal <= 0)
            { ShowInfo("Enter a valid duration in minutes."); return; }

            try
            {
                var svc = new ServiceModel
                {
                    ProviderID      = ProviderID,
                    ServiceName     = name,
                    Description     = desc,
                    Price           = priceVal,
                    DurationMinutes = durVal
                };
                ServiceDAL.CreateService(svc);
                LoadServices();

                // Clear fields
                AppTheme.AddPlaceholder(_txtSvcName,  "Service name…");
                AppTheme.AddPlaceholder(_txtSvcDesc,  "Short description…");
                AppTheme.AddPlaceholder(_txtSvcPrice, "Price (PKR)");
                AppTheme.AddPlaceholder(_txtSvcDur,   "Duration (min)");
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void DeactivateService()
        {
            if (_dgvServices.CurrentRow == null) return;
            int svcID = Convert.ToInt32(_dgvServices.CurrentRow.Cells["ServiceID"].Value);
            if (MessageBox.Show("Deactivate this service? It will be hidden from customers.",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ServiceDAL.DeleteService(svcID);
                LoadServices();
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  MY SCHEDULE (TIME SLOTS)
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvSlots   = null!;
        private DateTimePicker _dtpDate  = null!;
        private DateTimePicker _dtpStart = null!;
        private DateTimePicker _dtpEnd   = null!;

        private Panel BuildSchedulePanel()
        {
            var panel  = new Panel { BackColor = AppTheme.Background };
            var header = MakePanelHeader("My Schedule",
                "Add available time slots. Customers can book within the next 7 days.");

            // Add slot form
            var card = new Panel
            {
                Height = 110, Dock = DockStyle.Top,
                BackColor = AppTheme.CardBg, Padding = new Padding(20, 10, 20, 10)
            };

            card.Controls.Add(new Label
            {
                Text = "Add New Time Slot", Font = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Primary, AutoSize = true, Location = new Point(20, 10)
            });

            // Field labels
            var lblDate  = new Label { Text = "Date", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(20, 34) };
            var lblStart = new Label { Text = "Start Time", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(194, 34) };
            var lblEnd   = new Label { Text = "End Time", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(348, 34) };
            card.Controls.Add(lblDate); card.Controls.Add(lblStart); card.Controls.Add(lblEnd);

            _dtpDate = new DateTimePicker
            {
                Location  = new Point(20, 52),
                Size      = new Size(160, 36),
                Font      = AppTheme.FontInput,
                Format    = DateTimePickerFormat.Short,
                MinDate   = DateTime.Today,
                MaxDate   = DateTime.Today.AddDays(30),
                Value     = DateTime.Today.AddDays(1)
            };

            _dtpStart = new DateTimePicker
            {
                Location  = new Point(194, 52),
                Size      = new Size(140, 36),
                Font      = AppTheme.FontInput,
                Format    = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value     = DateTime.Today.AddHours(9)
            };

            _dtpEnd = new DateTimePicker
            {
                Location  = new Point(348, 52),
                Size      = new Size(140, 36),
                Font      = AppTheme.FontInput,
                Format    = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value     = DateTime.Today.AddHours(10)
            };

            var btnAddSlot = AppTheme.MakeSuccessButton("+ Add Slot", 120, 36);
            btnAddSlot.Location = new Point(502, 52);
            btnAddSlot.Click   += BtnAddSlot_Click;

            card.Controls.Add(_dtpDate);
            card.Controls.Add(_dtpStart);
            card.Controls.Add(_dtpEnd);
            card.Controls.Add(btnAddSlot);

            _dgvSlots = AppTheme.MakeDataGrid();
            _dgvSlots.Dock = DockStyle.Fill;

            var pnlAction = new Panel
            {
                Height = 56, Dock = DockStyle.Bottom,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 8, 16, 8)
            };
            var btnDelete = AppTheme.MakeDangerButton("Delete Slot", 130, 38);
            btnDelete.Location = new Point(16, 9);
            btnDelete.Click   += (s, e) => DeleteSlot();
            var lblNote = new Label
            {
                Text = "  Only available (unbooked) slots can be deleted.",
                Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted,
                AutoSize = true, Location = new Point(156, 18)
            };
            pnlAction.Controls.Add(btnDelete);
            pnlAction.Controls.Add(lblNote);

            panel.Controls.Add(_dgvSlots);
            panel.Controls.Add(pnlAction);
            panel.Controls.Add(card);      // Top — below header
            panel.Controls.Add(header);    // Top — last → topmost

            return panel;
        }

        private void LoadSchedule()
        {
            try
            {
                _dgvSlots.DataSource = BookingDAL.GetSlotsByProvider(ProviderID);
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void BtnAddSlot_Click(object? sender, EventArgs e)
        {
            DateTime date  = _dtpDate.Value.Date;
            TimeSpan start = _dtpStart.Value.TimeOfDay;
            TimeSpan end   = _dtpEnd.Value.TimeOfDay;

            if (start >= end)
            { ShowInfo("End time must be after start time."); return; }

            if (date < DateTime.Today)
            { ShowInfo("Cannot create slots in the past."); return; }

            try
            {
                BookingDAL.CreateSlot(ProviderID, date, start, end);
                LoadSchedule();
                MessageBox.Show("Time slot added successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void DeleteSlot()
        {
            if (_dgvSlots.CurrentRow == null) return;
            int    slotID    = Convert.ToInt32(_dgvSlots.CurrentRow.Cells["SlotID"].Value);
            bool   available = Convert.ToBoolean(_dgvSlots.CurrentRow.Cells["IsAvailable"].Value);
            if (!available) { ShowInfo("This slot is already booked and cannot be deleted."); return; }

            if (MessageBox.Show("Delete this time slot?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                BookingDAL.DeleteSlot(slotID);
                LoadSchedule();
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  INCOMING BOOKINGS
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvBookings = null!;

        private Panel BuildBookingsPanel()
        {
            var panel  = new Panel { BackColor = AppTheme.Background };
            var header = MakePanelHeader("Incoming Bookings",
                "Accept or reject customer booking requests.");

            var toolbar = new Panel
            {
                Height = 60, Dock = DockStyle.Top,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 10, 16, 10)
            };
            var btnRefresh = AppTheme.MakePrimaryButton("Refresh", 110, 38);
            btnRefresh.Location = new Point(16, 11);
            btnRefresh.Click   += (s, e) => LoadBookings();
            toolbar.Controls.Add(btnRefresh);

            _dgvBookings = AppTheme.MakeDataGrid();
            _dgvBookings.Dock = DockStyle.Fill;

            var pnlActions = new Panel
            {
                Height = 56, Dock = DockStyle.Bottom,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 8, 16, 8)
            };

            var btnAccept = AppTheme.MakeSuccessButton("Accept",   110, 38); btnAccept.Location = new Point( 16, 9);
            var btnReject = AppTheme.MakeDangerButton ("Reject",   110, 38); btnReject.Location = new Point(136, 9);
            var btnDone   = AppTheme.MakePrimaryButton("Mark Done",130, 38); btnDone.Location   = new Point(256, 9);

            btnAccept.Click += (s, e) => UpdateBookingStatus(2); // Accepted
            btnReject.Click += (s, e) => UpdateBookingStatus(5); // Rejected
            btnDone.Click   += (s, e) => UpdateBookingStatus(3); // Completed

            pnlActions.Controls.AddRange(new Control[] { btnAccept, btnReject, btnDone });

            panel.Controls.Add(_dgvBookings);
            panel.Controls.Add(pnlActions);
            panel.Controls.Add(toolbar);   // Top — below header
            panel.Controls.Add(header);    // Top — last → topmost

            return panel;
        }

        private void LoadBookings()
        {
            try
            {
                _dgvBookings.DataSource = BookingDAL.GetProviderBookings(ProviderID);
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void UpdateBookingStatus(int newStatus)
        {
            if (_dgvBookings.CurrentRow == null) return;
            int bkID = Convert.ToInt32(_dgvBookings.CurrentRow.Cells["BookingID"].Value);

            string action = newStatus switch
            {
                2 => "accept",
                3 => "mark as Completed",
                5 => "reject",
                _ => "update"
            };

            if (MessageBox.Show($"Confirm: {action} Booking #{bkID}?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    BookingDAL.UpdateStatus(bkID, newStatus, SessionManager.CurrentUser!.UserID);
                    LoadBookings();
                    UpdateBadge();
                }
                catch (Exception ex) { ShowError(ex.Message); }
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  NOTIFICATIONS
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvNotifs = null!;

        private Panel BuildNotificationsPanel()
        {
            var panel  = new Panel { BackColor = AppTheme.Background };
            var header = MakePanelHeader("Notifications", "Booking alerts and updates.");

            var toolbar = new Panel
            {
                Height = 60, Dock = DockStyle.Top,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 10, 16, 10)
            };
            var btnMark = AppTheme.MakePrimaryButton("Mark All Read", 150, 38);
            btnMark.Location = new Point(16, 11);
            btnMark.Click   += (s, e) =>
            {
                NotificationDAL.MarkAllRead(SessionManager.CurrentUser!.UserID);
                LoadNotifications();
                UpdateBadge();
            };
            toolbar.Controls.Add(btnMark);

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
        //  NOTIFICATION BADGE + TIMER
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
            var p = new Panel { Height = 90, BackColor = AppTheme.CardBg, Dock = DockStyle.Top };
            p.Controls.Add(new Label
            {
                Text = title, Font = AppTheme.FontTitle, ForeColor = AppTheme.Primary,
                AutoSize = true, Location = new Point(24, 16)
            });
            p.Controls.Add(new Label
            {
                Text = subtitle, Font = AppTheme.FontBody, ForeColor = AppTheme.TextMuted,
                AutoSize = true, Location = new Point(24, 52)
            });
            p.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 3, BackColor = AppTheme.Success });
            return p;
        }

        private static Label FieldLabel(string text, Point loc)
            => new() { Text = text, Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextDark,
                        AutoSize = true, Location = loc };

        private void ShowError(string msg)
            => MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowInfo(string msg)
            => MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
