using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace Terminator
{
    public partial class Terminator : Form
    {
        private bool newReplace = false;
        private bool expanded = false;
        private string FileName = "";
        private SqlConnection conn = null;
                
        public Terminator()
        {
            InitializeComponent();
            InitSqlConnection();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 88;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (console.Text != "")
                console.Text += "\r\nwhatup";
            else
                console.Text = "whatup";
        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            string path = txtPath.Text;
            string[] files;
            if (cbSubfolders.Checked)
                files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            else
                files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            int numOfFiles = files.Length;
            int index = path.Length + 1;
            console.Text = path + " \r\n";
            resetFilter();
            scanFiles("init");
        }

        private void resetFilter()
        {
            lbExtension.Items.Clear();
            txtContains.Text = "";
            txtExtention.Text = "";
            cbSubfolders.Checked = true;
        }

        private void loadExtensionCombobox(string [] files)
        {
            foreach ( string file in files )
            {
                int dotIndex = file.IndexOf('.');
                string extension = file.Substring(dotIndex);
            }

        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (txtPath.Text == "")
            {
                MessageBox.Show("Please select a folder!");
                btnBrowse_Click(null, null);
                return;
            }
            if ( newReplace )
            {
                backupFiles();
                newReplace = false;
            }


            if (search.SelectedTab == tabPage1)
                singleLineReplacement();
            else
                multipleLineReplacement();

        }

        void lbFileList_DoubleClick(object sender, System.EventArgs e)
        {
            //var selectedItem = lbFileList.SelectedItem;
            //lbFileList.Items.Remove(selectedItem);
            //console.AppendText(selectedItem.ToString() + "removed. \r\n");
            //console.ScrollToCaret();
        }

        void lbExtension_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string extensions = "";
            foreach (ListItem item in lbExtension.SelectedItems)
            {
                extensions += item.Text + " | ";
                item.Attributes.Add("style", "background-color: lime");
            }
            txtExtention.Text = extensions.Remove(extensions.Length - 3);
        }

        private void btnClipboard_Click(object sender, EventArgs e)
        {
            if (txtPath.Text == "")
            {
                MessageBox.Show("Please select a folder!");
                btnBrowse_Click(null, null);
                return;
            }

            string fileList = "File List: \r\n";
            int fileColumnIndex = Grid.Columns["File Name"].Index;
            foreach (DataGridViewRow row in Grid.Rows)
                fileList += row.Cells[fileColumnIndex].Value.ToString() + "\r\n";

            Clipboard.SetText(fileList);
            MessageBox.Show("File List Copied to Clipboard Successfully!");
        }

        protected string scanFiles(string mode = "list")
        {
            string path = txtPath.Text,
                fileList = "File List: \r\n",
                extension = txtExtention.Text,
                containWord = txtContains.Text,
                excludeWord = txtNameExclude.Text;
            string[] totalFiles;
            List<string> extensions = new List<string>(),
                fileNames = new List<string>();
            IEnumerable<string> files;
            SearchOption searchOption = cbSubfolders.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            totalFiles = Directory.GetFiles(path, "*.*", searchOption);
            Grid.DataSource = null;

            if (extension != "")
            {
                List<string> filesWithExt = new List<string>();
                string[] searchPatterns = extension.Split('|');
                foreach (string sp in searchPatterns)
                {
                    files = Directory.EnumerateFiles(path, "*.*", searchOption).Where(s => Regex.Match(s, @"\..*").Value == sp.Trim());
                    filesWithExt.AddRange(files);
                }
                filesWithExt.Sort();
                files = filesWithExt.Where(name => name.Contains(containWord));
            }
            else
            {
                files = totalFiles.Where(name => name.Contains(containWord));
            }

            if (excludeWord != "")
                files = files.Where(name => !name.Contains(excludeWord));

            int fileCount = 0,
                numOfFiles = totalFiles.Length,
                qualifiedCount = files.Count(),
                index = path.Length + 1;
            console.Text = "Path: " + path + "\r\n";
            if (extension != "")
                console.AppendText("With Extension: " + extension + " \r\n");
            if (containWord != "")
                console.AppendText("Name Contains:  " + containWord + " \r\n");
            if (excludeWord != "")
                console.AppendText("Name Excludes:  " + excludeWord + " \r\n");

            foreach (string file in files)
            {
                string fileName = file.Substring(index);
                fileList += fileName + "\r\n";
                fileNames.Add(fileName);
                
                if (mode == "init")
                {
                    string ext = Regex.Match(fileName, @"\..*").Value;
                    if (ext != "" && !extensions.Contains(ext))
                    {
                        extensions.Add(ext);
                        lbExtension.Items.Add(new ListItem(ext));
                    }
                }
                else if (mode != "clipboard")
                {
                    fileCount++;
                    loadingProgressBar(fileName, fileCount, qualifiedCount);
                }
            }
            populateGrid(fileNames);
            progressInfo.Text = "Filter Successfully Finished!";
            console.AppendText(qualifiedCount + " out of " + numOfFiles + " files found after filter. \r\n");
            return fileList;
        }


        private void loadingProgressBar(string fileName, int currentProcess, int total)
        {
            int percent = currentProcess * 100 / total;
            progressBar1.Value = percent;
            progressInfo.Text = fileName;
            progressInfo.Refresh();
            progressPrecent.Text = percent + "%";
            progressPrecent.Refresh();
        }
            
        protected void singleLineSearch()
        {
            string path = txtPath.Text,
                containWord = txtLineContains.Text;
            int totalFiles = Grid.Rows.Count,
                counter = 0,
                qualifiedCount = 0,
                fileColumnIndex = Grid.Columns["File Name"].Index;
            DataTable dt = new DataTable();
            dt.Columns.Add("Occurence", typeof(int));
            dt.Columns.Add("File Name", typeof(string));
            foreach( DataGridViewRow row in Grid.Rows )
            {
                int occurrence = 0;
                string fileName = row.Cells[fileColumnIndex].Value.ToString(),
                filePath = path + "\\" + fileName;
                foreach (string line in File.ReadAllLines(filePath))
                {
                    if (line.Contains(containWord))
                        occurrence++;
                }
                if (occurrence != 0)
                {
                    dt.Rows.Add(occurrence, fileName);
                    qualifiedCount++;
                }
                counter++;

                loadingProgressBar(fileName, counter, totalFiles);
            }
            Grid.DataSource = dt;
            //Grid.Columns["Occurence"].DisplayIndex = 0;
            Grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            progressInfo.Text = "Search Finished!";
            progressInfo.Refresh();
            console.AppendText("Search files contain: \"" + containWord + "\"\r\n");
            console.AppendText(qualifiedCount + " out of " + totalFiles + " Files Found after search. \r\n");
            console.ScrollToCaret();
        }

        private void populateGrid(List<string> files)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("File Name", typeof(string));
            foreach (string file in files)
                dt.Rows.Add(file);
            Grid.DataSource = dt;
        }

        private void populateGrid(List<string> files, List<int> occurrence)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Occurence", typeof(int));
            dt.Columns.Add("File Name", typeof(string));
            foreach (string file in files)
                dt.Rows.Add(file);
            Grid.DataSource = dt;
        }

        private void btnListFiles_Click(object sender, EventArgs e)
        {
            if (txtPath.Text == "")
            {
                MessageBox.Show("Please select a folder!");
                btnBrowse_Click(null, null);
                return;
            }
            scanFiles("list");
        }

        protected void multipleLineReplacement()
        {
            return;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case ( Keys.Alt | Keys.L ):
                case ( Keys.Alt | Keys.F ):
                    btnListFiles_Click(null, null);
                    return true;
                case ( Keys.Alt | Keys.C ):
                    btnClipboard_Click(null, null);
                    return true;
                case (Keys.Alt | Keys.I):
                    cbSubfolders.Checked = !cbSubfolders.Checked;
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtLineContains.Text == "")
            {
                MessageBox.Show("Please enter search criteria!");
                return;
            }
            if (Grid.Columns.Count == 2)
                scanFiles("list");
            singleLineSearch(); 
        }

        private void lbFileList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void singleLineReplacement()
        {
            if (txtLineContains.Text == "")
            {
                MessageBox.Show("Please enter search criteria!");
                return;
            }

            if (txtReplaceWith.Text == "")
            {
                MessageBox.Show("Please enter replace criteria!");
                return;
            }

        }

        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int fileColumnIndex = Grid.Columns["File Name"].Index;
            string fileName = Grid.Rows[e.RowIndex].Cells[fileColumnIndex].Value.ToString();

            populateGridWithMessage(fileName);
        }

        private void backupFiles()
        {
            if (!Directory.Exists("results"))
                Directory.CreateDirectory("results");
            string[] folderNames = Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\results");
            int newID = folderNames.Length + 1,
            fileColumnIndex = Grid.Columns["File Name"].Index;
            string idPath = "results\\" + newID.ToString() + " - " + Regex.Split(txtPath.Text, @"\\").Last(),
                modifyPath = idPath + "\\Modified",
                backupPath = idPath + "\\Backup";
            Directory.CreateDirectory(modifyPath);
            Directory.CreateDirectory(backupPath);

            foreach ( DataGridViewRow row in Grid.Rows )
            {
                string fileName = row.Cells[fileColumnIndex].Value.ToString(),
                    sourceFile = txtPath.Text + "\\" + fileName,
                    backupFile = backupPath + "\\" + fileName;
                if ( backupPath.Contains("\\") )
                {
                    string[] subfolders = Regex.Split(fileName, @"\\");
                    string currentPath = backupPath;
                    foreach ( string subfolder in subfolders.Take(subfolders.Length -1))
                    {
                        currentPath += "\\" + subfolder;
                        if (!Directory.Exists(currentPath))
                            Directory.CreateDirectory(currentPath);
                    }
                }
                File.Copy(sourceFile, backupFile);
            }

            File.WriteAllLines(idPath + "\\QueryInfo.txt", Regex.Split(console.Text, "\n"));
        }


        private void console_TextChanged(object sender, EventArgs e)
        {
            newReplace = true;
        }

        private void expand_Click(object sender, EventArgs e)
        {
            expanded = !expanded;
            int width = expanded ? 814 : 1145;
            this.ClientSize = new System.Drawing.Size(width, 542);
            btnExpand.Text = expanded ? "<-" : "->";
        }

        private void InitSqlConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "AZDEV1";
            builder["User Id"] = "wwdba";
            builder["Password"] = "sysadmin";
            builder["initial catalog"] = "azzier";
            conn = new SqlConnection(builder.ConnectionString);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void populateGridWithMessage(string fileName)
        {
            FileName = fileName.Replace("\\", "/");
            string sql = "SELECT MsgId, MessageDesc FROM systemmessage WHERE FILENAME = '" + FileName + "'";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            da.Dispose();
            dataGrid.DataSource = dt;
            dataGrid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            if ( expanded == false)
            {
                expanded = true;
                this.ClientSize = new System.Drawing.Size(1145, 542);
                btnExpand.Text = expanded ? "<-" : "->";
            }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string cell = dataGrid.SelectedCells[0].Value.ToString(),
            columnName = dataGrid.SelectedCells[0].OwningColumn.HeaderText;
            if ( columnName == "MessageDesc")
            {
                if (cell != "")
                    MessageBox.Show(cell);
                else
                {
                    int rowIndex = dataGrid.CurrentCell.RowIndex;
                    dataGrid.Rows[rowIndex].Cells[0].Value = "T" + dataGrid.Rows.Count;
                }
            }
        }

        private void Terminator_Load(object sender, EventArgs e)
        {

        }

        private void dataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ( e.ColumnIndex == 1)
                MessageBox.Show("value changed");
            DataGridViewRow row = dataGrid.Rows[e.RowIndex];
            string MsgId = row.Cells[0].Value.ToString(),
                Message = row.Cells[1].Value.ToString(),
            sql = "DELETE FROM SystemMessage WHERE FileName = '" + FileName + "' AND MsgId = '" + MsgId + "'";
            SqlCommand command = new SqlCommand(sql, conn);
            command.ExecuteNonQuery();
            sql = "INSERT INTO SystemMessage (MsgId, MessageDesc, MessageText, FileName, CreatedBy) " +
                "VALUES ('" + MsgId + "', '" + Message + "', '" + Message + "', '" + FileName + "', 'Terminator' )";
            command.CommandText = sql;
            command.ExecuteNonQuery();

            populateGridWithMessage(FileName);

        }

        private void deleteMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

            }
        }

        private void dataGrid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dataGrid.Rows[e.RowIndex].Selected = true;
                //rowIndex = e.RowIndex;
                dataGrid.CurrentCell = dataGrid.Rows[e.RowIndex].Cells[1];
                contextMenuStrip1.Show(dataGrid, e.Location);
                contextMenuStrip1.Show(Cursor.Position);
            }
        }
    }
}
