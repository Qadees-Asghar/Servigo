using SERVIGO.DAL;
using SERVIGO.Helpers;
using SERVIGO.Models;
using SERVIGO.Theme;
using System.Data;

namespace SERVIGO.Forms.Admin
{
    public partial class frmAdminDashboard : Form
    {
        // Layout
        private Panel          _pnlSidebar  = null!;
        private Panel          _pnlContent  = null!;
        private Label          _lblBadge    = null!;
        private System.Windows.Forms.Timer _notifTimer = null!;

        // Content panels
        private Panel _pnlDashboard   = null!;
        private Panel _pnlUsers       = null!;
        private Panel _pnlProviders   = null!;
        private Panel _pnlBookings    = null!;
        private Panel _pnlLogs        = null!;
        private Panel _pnlAnalytics   = null!;

        // Active sidebar button tracking
        private Button? _activeNav;

        public frmAdminDashboard()
        {
            InitializeComponent();
            BuildUI();
            ShowPanel(_pnlDashboard);
            StartNotifTimer();
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  SHELL LAYOUT
        // ═══════════════════════════════════════════════════════════════════════

        private void BuildUI()
        {
            Text          = "SERVIGO – Admin Panel";
            WindowState   = FormWindowState.Maximized;
            MinimumSize   = new Size(1100, 640);
            BackColor     = AppTheme.Background;
            Font          = AppTheme.FontBody;

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
            // App title
            var lblApp = new Label
            {
                Text      = "SERVIGO",
                Font      = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = AppTheme.Accent,
                AutoSize  = true,
                Location  = new Point(20, 24)
            };
            var lblRole = new Label
            {
                Text      = "Admin Panel",
                Font      = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(130, 160, 200),
                AutoSize  = true,
                Location  = new Point(22, 62)
            };

            // User info chip
            var lblUser = new Label
            {
                Text      = SessionManager.CurrentUser?.FullName ?? "Admin",
                Font      = AppTheme.FontBodyBold,
                ForeColor = Color.White,
                AutoSize  = false,
                Size      = new Size(196, 30),
                Location  = new Point(22, 92),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var divider = new Panel
            {
                Location  = new Point(20, 134),
                Size      = new Size(196, 1),
                BackColor = Color.FromArgb(50, 80, 120)
            };

            _pnlSidebar.Controls.Add(lblApp);
            _pnlSidebar.Controls.Add(lblRole);
            _pnlSidebar.Controls.Add(lblUser);
            _pnlSidebar.Controls.Add(divider);

            // Nav buttons
            int y = 156;
            var navItems = new (string Icon, string Label, Action Click)[]
            {
                ("⊞", "Dashboard",         () => { LoadDashboardStats(); ShowPanel(_pnlDashboard); }),
                ("👥", "Manage Users",      () => { LoadUsers();          ShowPanel(_pnlUsers); }),
                ("🔧", "Manage Providers",  () => { LoadProviders();      ShowPanel(_pnlProviders); }),
                ("📋", "All Bookings",      () => { LoadBookings();       ShowPanel(_pnlBookings); }),
                ("📊", "Analytics",         () => { LoadAnalytics();      ShowPanel(_pnlAnalytics); }),
                ("📜", "Audit Logs",        () => { LoadLogs();           ShowPanel(_pnlLogs); }),
            };

            foreach (var (icon, label, click) in navItems)
            {
                var btn = MakeNavButton(icon, label, y);
                var c   = click; // capture
                btn.Click += (s, e) =>
                {
                    SetActiveNav(btn);
                    c();
                };
                if (y == 140) { _activeNav = btn; SetActiveNav(btn); }
                _pnlSidebar.Controls.Add(btn);
                y += 50;
            }

            // Notification badge
            var btnNotif = MakeNavButton("🔔", "Notifications", y);
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
            btnNotif.Click += (s, e) => ShowNotifications();
            _pnlSidebar.Controls.Add(btnNotif);
            _pnlSidebar.Controls.Add(_lblBadge);
            y += 50;

            // Logout at bottom
            var btnLogout = new Button
            {
                Text      = "⏻  Logout",
                Size      = new Size(196, 44),
                Location  = new Point(22, _pnlSidebar.Height - 60),
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
                if (MessageBox.Show("Logout from SERVIGO?", "Confirm Logout",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Close();
            };
            _pnlSidebar.Controls.Add(btnLogout);
        }

        private Button MakeNavButton(string icon, string label, int y)
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
            btn.FlatAppearance.BorderSize          = 0;
            btn.FlatAppearance.MouseOverBackColor  = AppTheme.SidebarHover;
            btn.FlatAppearance.MouseDownBackColor  = AppTheme.Primary;
            return btn;
        }

        private void SetActiveNav(Button btn)
        {
            if (_activeNav != null)
            {
                _activeNav.BackColor = Color.Transparent;
                _activeNav.ForeColor = Color.FromArgb(190, 210, 240);
            }
            btn.BackColor = AppTheme.Primary;
            btn.ForeColor = Color.White;
            _activeNav = btn;
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  PANEL SWITCHING
        // ═══════════════════════════════════════════════════════════════════════

        private void ShowPanel(Panel panel)
        {
            foreach (Control c in _pnlContent.Controls) c.Visible = false;
            panel.Bounds  = new Rectangle(0, 0, _pnlContent.ClientSize.Width, _pnlContent.ClientSize.Height);
            panel.Visible = true;
            panel.BringToFront();
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  BUILD ALL CONTENT PANELS
        // ═══════════════════════════════════════════════════════════════════════

        private void BuildAllPanels()
        {
            _pnlDashboard = BuildDashboardPanel();
            _pnlUsers     = BuildUsersPanel();
            _pnlProviders = BuildProvidersPanel();
            _pnlBookings  = BuildBookingsPanel();
            _pnlAnalytics = BuildAnalyticsPanel();
            _pnlLogs      = BuildLogsPanel();

            foreach (var p in new[] { _pnlDashboard, _pnlUsers, _pnlProviders,
                                       _pnlBookings,  _pnlAnalytics, _pnlLogs })
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
        //  DASHBOARD PANEL
        // ──────────────────────────────────────────────────────────────────────

        private Label _lblStatCustomers  = null!;
        private Label _lblStatProviders  = null!;
        private Label _lblStatPending    = null!;
        private Label _lblStatBookings   = null!;
        private Label _lblStatCompleted  = null!;

        private Panel BuildDashboardPanel()
        {
            var panel = new Panel { BackColor = AppTheme.Background };

            var pnlHead = MakePanelHeader("Dashboard Overview",
                "System statistics and activity at a glance.");
            pnlHead.Dock = DockStyle.Top;

            // Content area — Dock.Fill so it occupies space BELOW the header
            var content = new Panel
            {
                Dock       = DockStyle.Fill,
                AutoScroll = true,
                BackColor  = Color.Transparent,
                Padding    = new Padding(24, 24, 24, 24)
            };

            // Stat cards row
            var flow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize      = true,
                Dock          = DockStyle.Top,
                BackColor     = Color.Transparent,
                WrapContents  = true,
                Padding       = new Padding(0)
            };

            var cardData = new[]
            {
                ("Total Customers",  "0", AppTheme.Info),
                ("Total Providers",  "0", AppTheme.Success),
                ("Pending Approvals","0", AppTheme.Gold),
                ("Total Bookings",   "0", AppTheme.Primary),
                ("Completed",        "0", AppTheme.Accent),
            };

            Label[] statLabels = new Label[5];
            for (int i = 0; i < cardData.Length; i++)
            {
                var (title, val, color) = cardData[i];
                var card = AppTheme.MakeStatCard(title, val, color, 220, 130);
                card.Margin = new Padding(0, 0, 24, 24);

                // Grab inner value label for updates
                statLabels[i] = (Label)card.Controls[1];
                flow.Controls.Add(card);
            }

            _lblStatCustomers = statLabels[0];
            _lblStatProviders = statLabels[1];
            _lblStatPending   = statLabels[2];
            _lblStatBookings  = statLabels[3];
            _lblStatCompleted = statLabels[4];

            content.Controls.Add(flow);
            panel.Controls.Add(content);    // Fill — occupies remaining space
            panel.Controls.Add(pnlHead);   // Top — last → topmost
            return panel;
        }

        private void LoadDashboardStats()
        {
            try
            {
                var dt = BookingDAL.GetDashboardStats();
                if (dt.Rows.Count == 0) return;
                var r = dt.Rows[0];
                _lblStatCustomers.Text = r["TotalCustomers"].ToString()!;
                _lblStatProviders.Text = r["TotalProviders"].ToString()!;
                _lblStatPending.Text   = r["PendingApprovals"].ToString()!;
                _lblStatBookings.Text  = r["TotalBookings"].ToString()!;
                _lblStatCompleted.Text = r["CompletedBookings"].ToString()!;
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  USERS PANEL
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvUsers = null!;

        private Panel BuildUsersPanel()
        {
            var panel = new Panel { BackColor = AppTheme.Background };

            var pnlHead = MakePanelHeader("Manage Users", "View, activate/deactivate, or remove customers.");
            pnlHead.Dock = DockStyle.Top;

            // Toolbar
            var toolbar = new Panel
            {
                Height    = 60,
                Dock      = DockStyle.Top,
                BackColor = AppTheme.CardBg,
                Padding   = new Padding(16, 10, 16, 10)
            };

            var txtSearch = AppTheme.MakeTextBox(260, 38);
            AppTheme.AddPlaceholder(txtSearch, "Search by name / email…");
            txtSearch.Location = new Point(16, 11);

            var btnSearch = AppTheme.MakePrimaryButton("Search", 100, 38);
            btnSearch.Location = new Point(286, 11);

            var btnRefresh = AppTheme.MakeOutlineButton("Refresh", 100, 38);
            btnRefresh.Location = new Point(396, 11);

            toolbar.Controls.Add(txtSearch);
            toolbar.Controls.Add(btnSearch);
            toolbar.Controls.Add(btnRefresh);

            // Grid
            _dgvUsers = AppTheme.MakeDataGrid();
            _dgvUsers.Dock = DockStyle.Fill;

            // Action buttons below grid
            var pnlActions = new Panel
            {
                Height    = 56,
                Dock      = DockStyle.Bottom,
                BackColor = AppTheme.CardBg,
                Padding   = new Padding(16, 8, 16, 8)
            };

            var btnToggle = AppTheme.MakeOutlineButton("Toggle Active", 160, 38);
            btnToggle.Location = new Point(16, 9);
            btnToggle.Click   += (s, e) => ToggleUserActive(_dgvUsers);

            var btnDelete = AppTheme.MakeDangerButton("Delete User", 130, 38);
            btnDelete.Location = new Point(186, 9);
            btnDelete.Click   += (s, e) => DeleteUser(_dgvUsers);

            pnlActions.Controls.Add(btnToggle);
            pnlActions.Controls.Add(btnDelete);

            btnSearch.Click  += (s, e) => LoadUsers(AppTheme.GetText(txtSearch, "Search by name / email…"));
            btnRefresh.Click += (s, e) => LoadUsers();

            // Add in reverse dock order: Fill/Bottom first, then Top controls bottom-to-top
            panel.Controls.Add(_dgvUsers);
            panel.Controls.Add(pnlActions);
            panel.Controls.Add(toolbar);   // Top — appears below header
            panel.Controls.Add(pnlHead);   // Top — added last → docked topmost

            return panel;
        }

        private void LoadUsers(string search = "")
        {
            try
            {
                var dt = UserDAL.GetAllCustomers();
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var filtered = dt.AsEnumerable()
                        .Where(r => r["FullName"].ToString()!.Contains(search, StringComparison.OrdinalIgnoreCase)
                                 || r["Email"].ToString()!.Contains(search, StringComparison.OrdinalIgnoreCase));
                    dt = filtered.Any() ? filtered.CopyToDataTable() : dt.Clone();
                }
                _dgvUsers.DataSource = dt;
                StyleGridColumns(_dgvUsers, new[] { "UserID", "FullName", "Email", "Phone",
                                                     "IsActive", "TotalBookings", "CreatedAt" });
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void ToggleUserActive(DataGridView dgv)
        {
            if (dgv.CurrentRow == null) return;
            string uid    = dgv.CurrentRow.Cells["UserID"].Value.ToString()!;
            bool   active = Convert.ToBoolean(dgv.CurrentRow.Cells["IsActive"].Value);
            if (MessageBox.Show($"Set user {uid} active = {!active}?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UserDAL.SetActiveStatus(uid, !active);
                LoadUsers();
            }
        }

        private void DeleteUser(DataGridView dgv)
        {
            if (dgv.CurrentRow == null) return;
            string uid = dgv.CurrentRow.Cells["UserID"].Value.ToString()!;
            if (MessageBox.Show($"Permanently delete user {uid}?\nThis cannot be undone.", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                UserDAL.DeleteUser(uid);
                LoadUsers();
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  PROVIDERS PANEL
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvProviders = null!;

        private Panel BuildProvidersPanel()
        {
            var panel = new Panel { BackColor = AppTheme.Background };
            var head  = MakePanelHeader("Manage Providers", "Grant or remove the Verified badge. Providers can work without it.");
            head.Dock = DockStyle.Top;

            var toolbar = new Panel
            {
                Height    = 60, Dock = DockStyle.Top,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 10, 16, 10)
            };
            var btnRefresh = AppTheme.MakeOutlineButton("Refresh", 100, 38);
            btnRefresh.Location = new Point(16, 11);
            btnRefresh.Click   += (s, e) => LoadProviders();

            var lblInfo = new Label
            {
                Text      = "  Grant or remove the Verified badge for any provider.",
                Font      = AppTheme.FontSmall,
                ForeColor = AppTheme.Gold,
                AutoSize  = true,
                Location  = new Point(130, 22)
            };
            toolbar.Controls.Add(btnRefresh);
            toolbar.Controls.Add(lblInfo);

            _dgvProviders = AppTheme.MakeDataGrid();
            _dgvProviders.Dock = DockStyle.Fill;

            var pnlActions = new Panel
            {
                Height = 56, Dock = DockStyle.Bottom,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 8, 16, 8)
            };

            var btnApprove = AppTheme.MakeSuccessButton("✔ Verify", 120, 38);
            btnApprove.Location = new Point(16, 9);
            btnApprove.Click   += (s, e) => SetProviderApproval(true);

            var btnReject = AppTheme.MakeOutlineButton("✖ Unverify", 130, 38);
            btnReject.Location = new Point(146, 9);
            btnReject.Click   += (s, e) => SetProviderApproval(false);

            var btnDelete = AppTheme.MakeDangerButton("Delete Provider", 160, 38);
            btnDelete.Location = new Point(266, 9);
            btnDelete.Click   += (s, e) => DeleteProvider();

            pnlActions.Controls.AddRange(new Control[] { btnApprove, btnReject, btnDelete });

            panel.Controls.Add(_dgvProviders);
            panel.Controls.Add(pnlActions);
            panel.Controls.Add(toolbar);   // Top — below header
            panel.Controls.Add(head);      // Top — last → topmost

            return panel;
        }

        private void LoadProviders()
        {
            try
            {
                var dt = UserDAL.GetAllProviders();
                _dgvProviders.DataSource = dt;
                StyleGridColumns(_dgvProviders, new[] {
                    "UserID","FullName","Email","Phone","CategoryName",
                    "IsApproved","AverageRating","IsActive","CreatedAt" });
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void SetProviderApproval(bool approve)
        {
            if (_dgvProviders.CurrentRow == null) return;
            int provID = Convert.ToInt32(_dgvProviders.CurrentRow.Cells["ProviderID"].Value);
            ProviderDAL.SetApproval(provID, approve);
            LoadProviders();
        }

        private void DeleteProvider()
        {
            if (_dgvProviders.CurrentRow == null) return;
            int provID = Convert.ToInt32(_dgvProviders.CurrentRow.Cells["ProviderID"].Value);
            if (MessageBox.Show("Delete this provider?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                ProviderDAL.DeleteProvider(provID);
                LoadProviders();
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  BOOKINGS PANEL
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvBookings = null!;

        private Panel BuildBookingsPanel()
        {
            var panel = new Panel { BackColor = AppTheme.Background };
            var head  = MakePanelHeader("All Bookings", "Complete booking history across all users.");
            head.Dock = DockStyle.Top;

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

            panel.Controls.Add(_dgvBookings);
            panel.Controls.Add(toolbar);   // Top — below header
            panel.Controls.Add(head);      // Top — last → topmost

            return panel;
        }

        private void LoadBookings()
        {
            try
            {
                _dgvBookings.DataSource = BookingDAL.GetAllBookings();
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  ANALYTICS PANEL
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvSummary  = null!;
        private DataGridView _dgvProvStats = null!;

        private Panel BuildAnalyticsPanel()
        {
            var panel = new Panel { BackColor = AppTheme.Background };
            var head  = MakePanelHeader("Analytics & Reports", "GROUP BY booking summaries and provider performance.");
            head.Dock = DockStyle.Top;

            // Content area — Dock.Fill + AutoScroll so it sits below header
            var content = new Panel
            {
                Dock       = DockStyle.Fill,
                AutoScroll = true,
                BackColor  = Color.Transparent,
                Padding    = new Padding(24, 20, 24, 20)
            };

            var flow = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize      = true,
                WrapContents  = false,
                Dock          = DockStyle.Top,
                BackColor     = Color.Transparent
            };

            var lblS1 = AppTheme.MakeLabel("Bookings by Status", AppTheme.FontSubtitle, AppTheme.Primary);
            lblS1.Margin = new Padding(0, 0, 0, 8);
            flow.Controls.Add(lblS1);

            _dgvSummary = AppTheme.MakeDataGrid();
            _dgvSummary.Size   = new Size(500, 200);
            _dgvSummary.Margin = new Padding(0, 0, 0, 24);
            flow.Controls.Add(_dgvSummary);

            var lblS2 = AppTheme.MakeLabel("Provider Performance", AppTheme.FontSubtitle, AppTheme.Primary);
            lblS2.Margin = new Padding(0, 0, 0, 8);
            flow.Controls.Add(lblS2);

            _dgvProvStats = AppTheme.MakeDataGrid();
            _dgvProvStats.Size   = new Size(900, 300);
            _dgvProvStats.Margin = new Padding(0, 0, 0, 20);
            flow.Controls.Add(_dgvProvStats);

            content.Controls.Add(flow);
            panel.Controls.Add(content);   // Fill
            panel.Controls.Add(head);      // Top — last → topmost

            return panel;
        }

        private void LoadAnalytics()
        {
            try
            {
                _dgvSummary.DataSource   = BookingDAL.GetBookingSummary();
                _dgvProvStats.DataSource = BookingDAL.GetProviderStats();
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  AUDIT LOGS PANEL
        // ──────────────────────────────────────────────────────────────────────

        private DataGridView _dgvLogs = null!;

        private Panel BuildLogsPanel()
        {
            var panel = new Panel { BackColor = AppTheme.Background };
            var head  = MakePanelHeader("Audit Logs", "Automatic trigger-generated activity log.");
            head.Dock = DockStyle.Top;

            var toolbar = new Panel
            {
                Height = 60, Dock = DockStyle.Top,
                BackColor = AppTheme.CardBg, Padding = new Padding(16, 10, 16, 10)
            };
            var btnRefresh = AppTheme.MakePrimaryButton("Refresh", 110, 38);
            btnRefresh.Location = new Point(16, 11);
            btnRefresh.Click   += (s, e) => LoadLogs();
            toolbar.Controls.Add(btnRefresh);

            _dgvLogs = AppTheme.MakeDataGrid();
            _dgvLogs.Dock = DockStyle.Fill;

            panel.Controls.Add(_dgvLogs);
            panel.Controls.Add(toolbar);   // Top — below header
            panel.Controls.Add(head);      // Top — last → topmost

            return panel;
        }

        private void LoadLogs()
        {
            try
            {
                _dgvLogs.DataSource = NotificationDAL.GetAuditLogs();
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  NOTIFICATIONS
        // ═══════════════════════════════════════════════════════════════════════

        private void ShowNotifications()
        {
            try
            {
                string uid = SessionManager.CurrentUser!.UserID;
                var dt     = NotificationDAL.GetByUser(uid);
                NotificationDAL.MarkAllRead(uid);
                UpdateBadge();

                var frm = new Form
                {
                    Text          = "Notifications",
                    Size          = new Size(560, 500),
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor     = AppTheme.Background,
                    Font          = AppTheme.FontBody
                };
                var dgv = AppTheme.MakeDataGrid();
                dgv.Dock       = DockStyle.Fill;
                dgv.DataSource = dt;
                frm.Controls.Add(dgv);
                frm.ShowDialog(this);
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

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
            catch { /* ignore timer errors */ }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _notifTimer?.Stop();
            _notifTimer?.Dispose();
            base.OnFormClosed(e);
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  UI HELPERS
        // ═══════════════════════════════════════════════════════════════════════

        private static Panel MakePanelHeader(string title, string subtitle)
        {
            var p = new Panel
            {
                Height    = 110,
                BackColor = AppTheme.CardBg,
                Padding   = new Padding(28, 20, 28, 0)
            };
            var lbl = new Label
            {
                Text      = title,
                Font      = AppTheme.FontTitle,
                ForeColor = AppTheme.Primary,
                AutoSize  = true,
                Location  = new Point(28, 22)
            };
            var sub = new Label
            {
                Text      = subtitle,
                Font      = AppTheme.FontBody,
                ForeColor = AppTheme.TextMuted,
                AutoSize  = true,
                Location  = new Point(28, 62)
            };
            var bar = new Panel
            {
                Dock = DockStyle.Bottom, Height = 3, BackColor = AppTheme.Primary
            };
            p.Controls.Add(lbl);
            p.Controls.Add(sub);
            p.Controls.Add(bar);
            return p;
        }

        private static void StyleGridColumns(DataGridView dgv, string[] keep)
        {
            foreach (DataGridViewColumn col in dgv.Columns)
                col.Visible = keep.Contains(col.Name);
        }

        private void ShowError(string msg)
            => MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
