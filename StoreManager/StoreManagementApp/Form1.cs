using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreManagementApp
{
    public partial class Form1 : Form
    {
        private bool m_dataRefreshing = false;
        private int m_selectedRow = 0;
        private CSettings m_settings;

        public Form1()
        {
            InitializeComponent();
            m_settings = new CSettings("config.ini");
            if (System.IO.File.Exists("config.ini"))
            {
                if (m_settings.LoadSettings())
                {
                    foreach (KeyValuePair<String, String> attribute in m_settings.GetSection("[Default]"))
                    {
                        switch (attribute.Key.ToLower())
                        {
                            case "address":
                                Console.WriteLine(" -- ADDRESS = {0}", attribute.Value);
                                tbStoreAddress.Text = attribute.Value;
                                break;
                            case "uuid":
                                Console.WriteLine(" -- UUID = {0}", attribute.Value);
                                tbUUID.Text = attribute.Value;
                                break;
                        }
                    }
                }
            }
            dataGridViewServer.Rows.Clear();
            populateServerView();
            dataGridViewHistory.Rows.Clear();
        }

        private WebClient newWebClient()
        {
            WebClient client = new WebClient();
            client.Proxy = null;
            client.UseDefaultCredentials = false;

            return client;
        }

        private void btnRefreshServerView_Click(object sender, EventArgs e)
        {
            populateServerView();
        }

        private void populateServerView()
        {
            btnRefreshServerView.Enabled = false;
            using (WebClient client = newWebClient())
            {
                try
                {
                    client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(httpClient_DownloadServerComplete);
                    tbPostTestResponse.Text = "";
                    if (tbStoreAddress.Text.Equals(""))
                        return;
                    client.DownloadStringAsync(new System.Uri("http://" + tbStoreAddress.Text.ToString() + "/000Admin/server.txt"));
                }
                catch (Exception ex)
                {
                    tbPostTestResponse.Text = ex.Message;
                }
            }
        }

        private void httpClient_DownloadServerComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                tbPostTestResponse.Text += e.Result;
                createServerItems(e.Result.ToString());
                if (!btnRefreshServerView.Enabled)
                    btnRefreshServerView.Enabled = true;
            }
            catch (Exception ex)
            {
                tbPostTestResponse.Text += ex.Message;
                if (ex.InnerException != null)
                    tbPostTestResponse.Text += Environment.NewLine + ex.InnerException.Message;
            }
        }

        private void createServerItems(String serverTxt)
        {
            m_dataRefreshing = true;
            dataGridViewServer.Rows.Clear();
            char[] eol = { '\n', '\r' };
            String[] items = serverTxt.Split(eol);
            foreach(String item in items)
            {
                if (item.Equals(""))
                    continue;
                String[] entry = @item.Split(',');
                dataGridViewServer.Rows.Add(entry);
            }
            m_dataRefreshing = false;
        }

        private void btnRefreshHistoryView_Click(object sender, EventArgs e)
        {
            populateHistoryView();
        }

        private void populateHistoryView()
        {
            btnRefreshHistoryView.Enabled = false;
            using (WebClient client = newWebClient())
            {
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(httpClient_DownloadHistoryComplete);
                try
                {
                    tbPostTestResponse.Text = "";
                    if (tbStoreAddress.Text.Equals(""))
                        return;
                    client.DownloadStringAsync(new System.Uri("http://" + tbStoreAddress.Text.ToString() + "/000Admin/history.txt"));
                }
                catch (Exception ex)
                {
                    tbPostTestResponse.Text = ex.Message;
                }
            }
        }

        private void httpClient_DownloadHistoryComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                tbPostTestResponse.Text += e.Result;
                createHistoryItems(e.Result.ToString());
                if (!btnRefreshHistoryView.Enabled)
                    btnRefreshHistoryView.Enabled = true;
            }
            catch (Exception ex)
            {
                tbPostTestResponse.Text += ex.Message;
                if (ex.InnerException != null)
                    tbPostTestResponse.Text += Environment.NewLine + ex.InnerException.Message;
            }
        }

        private void createHistoryItems(String historyTxt)
        {
            m_dataRefreshing = true;
            dataGridViewHistory.Rows.Clear();
            char[] eol = { '\n', '\r' };
            String[] items = historyTxt.Split(eol);
            foreach (String item in items)
            {
                if (item.Equals(""))
                    continue;
                String[] entry = @item.Split(',');
                if(entry.Length == 3)
                {
                    String[] shortEntry = {entry[0], entry[1], "", "", "", entry[2], "", ""};
                    dataGridViewHistory.Rows.Add(shortEntry);
                }
                else
                    dataGridViewHistory.Rows.Add(entry);
            }
            m_dataRefreshing = false;
        }

        private void dataGridViewServer_SelectionChanged(object sender, EventArgs e)
        {
            if (!m_dataRefreshing)
            {
                m_selectedRow = 0;
                DataGridView view = (DataGridView)sender;
                DataGridViewSelectedRowCollection selectedRows = view.SelectedRows;
                if (selectedRows.Count != 1)
                {
                    DataGridViewSelectedCellCollection selectedCells = view.SelectedCells;
                    if (selectedCells.Count > 0)
                    {
                        m_selectedRow = selectedCells[0].RowIndex;
                        if (!btnDelete.Enabled)
                            btnDelete.Enabled = true;
                    }
                }
                else if (selectedRows.Count == 1)
                {
                    m_selectedRow = view.SelectedRows[0].Index;
                    if (!btnDelete.Enabled)
                        btnDelete.Enabled = true;
                }
                else
                    btnDelete.Enabled = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            btnDelete.Enabled = false;
            DataGridViewRow row = dataGridViewServer.Rows[m_selectedRow];
            DataGridViewCell cell = row.Cells[0];
            String product = @cell.Value.ToString();
            product = product.Replace("\\", "");
            product = product.Replace("\"", "");
            using (WebClient client = newWebClient())
            {
                client.UploadValuesCompleted += new UploadValuesCompletedEventHandler(httpClient_PostComplete);
                try
                {
                    tbPostTestResponse.Text = "";
                    NameValueCollection command = new NameValueCollection()
                    {
                        { "id", tbUUID.Text },
                        { "del", product },
                    };
                    client.UploadValuesAsync(new System.Uri("http://" + tbStoreAddress.Text.ToString()), "POST", command);
                }
                catch (Exception ex)
                {
                    tbPostTestResponse.Text = ex.Message;
                }
            }
        }

        private void httpClient_PostComplete(object sender, UploadValuesCompletedEventArgs e)
        {
            string result = "";
            try
            {
                result = System.Text.Encoding.UTF8.GetString(e.Result);
            }
            catch(Exception ex)
            {
                result = ex.Message;
            }
            tbPostTestResponse.Text += result;
            populateServerView();
            populateHistoryView();
        }

        private void btnFetchJobCount_Click(object sender, EventArgs e)
        {
            fetchJobCount();
        }

        private void fetchJobCount()
        {
            btnFetchJobCount.Enabled = false;
            using (WebClient client = newWebClient())
            {
                client.UploadValuesCompleted += new UploadValuesCompletedEventHandler(httpClient_DownloadJobCountComplete);
                try
                {
                    tbPostTestResponse.Text = "";
                    if (tbStoreAddress.Text.Equals(""))
                        return;
                    NameValueCollection command = new NameValueCollection()
                    {
                        { "count", "current" },
                    };
                    client.UploadValuesAsync(new System.Uri("http://" + tbStoreAddress.Text.ToString()), "POST", command);
                }
                catch (Exception ex)
                {
                    tbPostTestResponse.Text = ex.Message;
                }
            }
        }

        private void httpClient_DownloadJobCountComplete(object sender, UploadValuesCompletedEventArgs e)
        {
            string result = System.Text.Encoding.UTF8.GetString(e.Result);
            tbPostTestResponse.Text += result;
            int dataStart = result.IndexOf("<p>") + 3;
            int dataEnd = result.IndexOf("</p>");
            result = result.Substring(dataStart, (dataEnd - dataStart));
            lblPendingJobCount.Text = result;
            if (!btnFetchJobCount.Enabled)
                btnFetchJobCount.Enabled = true;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    populateServerView();
                    break;
                case 1:
                    populateHistoryView();
                    fetchJobCount();
                    break;
            }
            resize();
        }

        private void tPageVersion_Resize(object sender, EventArgs e)
        {
            dataGridViewServer.SuspendLayout();

            label4.Location = new Point(3, 3);
            dataGridViewServer.Location = new Point(3, 19);
            int tabWidth = tPageVersion.Size.Width;
            int tabHeight = tPageVersion.Size.Height;

            btnDelete.Location = new Point((tabWidth - btnDelete.Width - 3), (tabHeight - btnDelete.Height - 3));
            btnRefreshServerView.Location = new Point(3, (tabHeight - btnRefreshServerView.Height - 3));

            int gridWidth = tabWidth - 6;
            int gridHeight = (tabHeight - dataGridViewServer.Location.Y) - ((tabHeight - btnRefreshServerView.Location.Y) + 3);
            dataGridViewServer.Size = new Size(gridWidth, gridHeight);

            dataGridViewServer.ResumeLayout();
        }

        private void tabControl1_Resize(object sender, EventArgs e)
        {
            resize();
        }

        private void resize()
        {
            // tPageVersion
            dataGridViewServer.SuspendLayout();

            label4.Location = new Point(3, 3);
            dataGridViewServer.Location = new Point(3, 19);
            int tabWidth = tPageVersion.Size.Width;
            int tabHeight = tPageVersion.Size.Height;

            btnDelete.Location = new Point((tabWidth - btnDelete.Width - 3), (tabHeight - btnDelete.Height - 3));
            btnRefreshServerView.Location = new Point(3, (tabHeight - btnRefreshServerView.Height - 3));

            int gridWidth = tabWidth - 6;
            int gridHeight = (tabHeight - dataGridViewServer.Location.Y) - ((tabHeight - btnRefreshServerView.Location.Y) + 3);
            dataGridViewServer.Size = new Size(gridWidth, gridHeight);

            dataGridViewServer.ResumeLayout();

            // tPageStore
            dataGridViewHistory.SuspendLayout();

            label1.Location = new Point(3, 3);
            tbStoreAddress.Location = new Point(3, 23);
            label2.Location = new Point(3, 46);
            btnFetchJobCount.Location = new Point(276, 37);
            lblPendingJobCount.Location = new Point(364, 42);

            tabHeight = tPageStore.Height;
            tabWidth = tPageStore.Width;
            gridWidth = tabWidth - 6;

            int responseTop = tabHeight - tbPostTestResponse.Size.Height - 3;
            tbPostTestResponse.Location = new Point(3, responseTop);
            tbPostTestResponse.Size = new Size(gridWidth, 94);

            btnRefreshHistoryView.Location = new Point(3, (tbPostTestResponse.Location.Y - btnRefreshHistoryView.Height - 3));

            dataGridViewHistory.Location = new Point(3, 65);
            gridHeight = (tabHeight - dataGridViewHistory.Location.Y) - ((tabHeight - btnRefreshHistoryView.Location.Y) + 3);
            dataGridViewHistory.Size = new Size(gridWidth, gridHeight);

            dataGridViewHistory.ResumeLayout();
        }

        private void dataGridViewHistory_MouseEnter(object sender, EventArgs e)
        {
            dataGridViewHistory.Focus();
        }

        private void dataGridViewHistory_MouseLeave(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void tbPostTestResponse_MouseEnter(object sender, EventArgs e)
        {
            tbPostTestResponse.Focus();
            tbPostTestResponse.Select(0, 0);
        }

        private void tbPostTestResponse_MouseLeave(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void dataGridViewServer_MouseEnter(object sender, EventArgs e)
        {
            dataGridViewServer.Focus();
        }

        private void dataGridViewServer_MouseLeave(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            resize();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                m_settings.Set("[Default]", "ADDRESS", tbStoreAddress.Text);
                if (tbUUID.Text != "")
                    m_settings.Set("[Default]", "UUID", tbUUID.Text);
                m_settings.SaveSettings();
            }
            catch(Exception ex)
            {
                Console.WriteLine("{0} : Exception : {1}", DateTime.Now.ToString(), ex.Message);
            }
            Application.Exit();
        }

        private void dataGridViewServer_MouseUp(object sender, MouseEventArgs e)
        {
            var ht = dataGridViewServer.HitTest(e.X, e.Y);
            if(ht.Type == DataGridViewHitTestType.None)
            {
                dataGridViewServer.Rows[m_selectedRow].Selected = false;
                m_selectedRow = -1;
                btnDelete.Enabled = false;
            }
        }

        private void dataGridViewServer_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView view = (DataGridView)sender;
            if (e.RowIndex < (view.Rows.Count - 1))
            {
                m_selectedRow = e.RowIndex;
                view.Rows[m_selectedRow].Selected = true;
            }
            else
            {
                m_selectedRow = -1;
                btnDelete.Enabled = false;
            }
        }

        private void btnGenerateUUID_Click(object sender, EventArgs e)
        {
            if(!tbUUID.Text.Equals(""))
            {
                if (MessageBox.Show("This Manager already has a UUID - if you change it you will have to have your symbol store administrator register your new UUID", "Confirm Change", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                    return;
            }
            Guid uuid = Guid.NewGuid();
            m_settings.Set("[Default]", "UUID", uuid.ToString());
            m_settings.SaveSettings();
            tbUUID.Text = uuid.ToString().ToUpper();
        }
    }
}
