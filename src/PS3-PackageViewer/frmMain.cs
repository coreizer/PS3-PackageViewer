

namespace PS3_PackageViewer
{
   using System;
   using System.Linq;
   using System.Collections.Generic;
   using System.IO;
   using System.Windows.Forms;
   using System.Diagnostics;
   using System.Threading.Tasks;

   public partial class frmMain : Form
   {
      private readonly PS3PackageReader reader = new PS3PackageReader();
      private PS3PackageReader.Package currentPackage;

      private readonly string[] SIZE_PREFIX = {
            "BK",
            "KB",
            "MB",
            "GB",
            "TB"
        };

      public frmMain()
      {
         InitializeComponent();
         this.SetWindowTitle();
      }

      private async void openToolStripMenuItem_Click(object sender, EventArgs e)
      {
         this.treeViewFolder.Nodes.Clear();
         this.listViewFiles.Items.Clear();
         this.SetWindowTitle();

         try {
            this.SetToolStripStatus("OpenFile...", false);
            using (OpenFileDialog OFD = new OpenFileDialog()) {
               OFD.AddExtension = true;
               OFD.Filter = "Package file (*.pkg)|*.pkg";
               OFD.Multiselect = false;
               OFD.FilterIndex = 1;
               if (OFD.ShowDialog(this) == DialogResult.OK) {
                  this.SetToolStripStatus("Loading...", true);
                  await this.LoadPackage(OFD.FileName);
               }
            }
         }
         catch (Exception ex) {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
         finally {
            this.SetToolStripStatus("Loading completed", false);
         }
      }

      private void treeViewFolder_AfterSelect(object sender, TreeViewEventArgs e)
      {
         this.ReadFolder(this.treeViewFolder.SelectedNode.FullPath);
      }

      private void newToolStripMenuItem_Click(object sender, EventArgs e)
      {
         try {

         }
         catch {

         }
      }

      private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
      {

      }

      private async Task LoadPackage(string path)
      {
         if (string.IsNullOrEmpty(path)) {
            throw new ArgumentNullException(nameof(path));
         }

         this.currentPackage = await Task.Run(async () => await this.reader.ReadAsync(path));
         this.treeViewFolder.Nodes.Add(this.currentPackage.TitleId, this.currentPackage.TitleId);
         this.treeViewFolder.SelectedNode = treeViewFolder.Nodes[0];

         // Add Treeview.
         foreach (PS3PackageReader.File file in this.currentPackage.Files) {
            if (file.Type == PS3PackageReader.FileType.Folder) {
               this.AddFolder(file.Name, file.FolderName);
            }
         }

         this.ReadFolder(treeViewFolder.SelectedNode.FullPath);
         this.SetWindowTitle(this.currentPackage.FileName);
      }

      private void ReadFolder(string path)
      {
         this.listViewFiles.Items.Clear();

         TreeNode[] node = this.treeViewFolder.Nodes.Find(path, true);
         for (int i = 0; i <= node[0].Nodes.Count - 1; i++) {
            this.AddFile(node[0].Nodes[i].Text, -1);
         }

         List<PS3PackageReader.File> files = this.currentPackage.Files.FindAll(x => x.FolderName == path);
         foreach (PS3PackageReader.File file in files) {
            if (file.Type != PS3PackageReader.FileType.Folder) {
               this.AddFile(file.Name, file.Size);
            }
         }
      }

      private void AddFolder(string path, string node = "")
      {
         if (string.IsNullOrEmpty(node)) {
            for (int i = 0; i <= treeViewFolder.Nodes.Count - 1; i++) {
               if (treeViewFolder.Nodes[i].Text == path)
                  return;
            }

            this.treeViewFolder.Nodes.Add(path, path);
            return;
         }

         string[] split = node.Split('\\');
         for (int i = 1; i <= split.Length; i++) {
            try {
               string folder = "";
               for (int x = 0; x <= i - 1; x++) {
                  folder = split[x];
                  if (x > 0) {
                     folder = folder + "\\" + split[x];
                  }
               }

               // 現在のパスを検索して追加する
               TreeNode[] findNode = this.treeViewFolder.Nodes.Find(folder, true);
               folder = path;
               if (i < split.Length)
                  folder = split[i];

               bool existe = false;
               if (findNode.Length > 0) {
                  for (int j = 0; j <= findNode[0].Nodes.Count - 1; j++) {
                     if (findNode[0].Nodes[j].Text == folder)
                        existe = true;
                  }
               }

               if (!existe) {
                  findNode[0].Nodes.Add(findNode[0].FullPath + "\\" + folder, folder);
                  findNode[0].Expand();
               }
            }
            catch {
               MessageBox.Show("Load of 1 item failed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
         }
      }

      private void AddFile(string fileName, long size)
      {
         // remove of folder name.
         if (fileName.Contains("\\")) {
            int index = fileName.IndexOf('\\');
            fileName = fileName.Remove(0, index + 1);
         }

         ListViewItem item = new ListViewItem
         {
            Text = fileName,
            ImageIndex = GetImageIndex(fileName)
         };

         item.SubItems.Add(SizeOf(size));
         item.SubItems.Add("File");
         this.listViewFiles.Items.Add(item);
      }

      private int GetImageIndex(string fileName)
      {
         if (string.IsNullOrEmpty(fileName))
            return 2;

         string extension = Path.GetExtension(fileName);

         switch (extension.ToUpper()) {
            case ".PNG":
               return 3;
         }

         return 0;
      }


      private string SizeOf(long size)
      {
         if (size <= 0) {
            return "0";
         }

         int index = (int)Math.Log(size, 1024);
         decimal length = (decimal)size / ((long)1 << (index * 10));
         return string.Format("{0:n1}{1}", length, SIZE_PREFIX[index]);
      }

      private void SetWindowTitle(string packageName = "")
      {
         if (!string.IsNullOrWhiteSpace(packageName)) {
            this.Text = $"[PS3] Package Viewer - {Application.ProductVersion} | {packageName}";
         }

         this.Text = $"[PS3] Package Viewer - {Application.ProductVersion}";
      }

      private void SetToolStripStatus(string statusText, bool isLoading)
      {
         this.toolStripProgressBar1.Visible = isLoading;
         this.toolStripStatusLabelStatus.Text = $"Status: {statusText}";
      }

      private void openHexReaderToolStripMenuItem_Click(object sender, EventArgs e)
      {
         try {
            OpenFileDialog OFD = new OpenFileDialog();
            if (OFD.ShowDialog() == DialogResult.OK) {
               FileStream stream = new FileStream(OFD.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
               HexReader reader = new HexReader(stream);

               MessageBox.Show(reader.ReadString(4, 0x01));
               MessageBox.Show(reader.ReadString(4));
               MessageBox.Show(reader.ReadString(4));
               MessageBox.Show(reader.ReadString(4, 0x01));
               MessageBox.Show(reader.ReadString(4));
            }

         }
         catch {

         }
      }
   }
}
