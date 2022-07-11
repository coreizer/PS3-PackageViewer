using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PS3_PackageViewer
{
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

      private void SetWindowTitle(string packageName = "")
      {
         if (!string.IsNullOrWhiteSpace(packageName)) {
            this.Text = $"[PS3] Package Viewer - {Application.ProductVersion} | {packageName}";
         }

         this.Text = $"[PS3] Package Viewer - {Application.ProductVersion}";
      }

      private void openToolStripMenuItem_Click(object sender, EventArgs e)
      {
         this.treeViewFolder.Nodes.Clear();
         this.listViewFiles.Items.Clear();
         this.SetWindowTitle();

         try {
            using (OpenFileDialog OFD = new OpenFileDialog()) {
               OFD.AddExtension = true;
               OFD.Filter = "Package file (*.pkg)|*.pkg";
               OFD.Multiselect = false;
               OFD.FilterIndex = 1;
               if (OFD.ShowDialog(this) == DialogResult.OK) {
                  this.LoadPackage(OFD.FileName);
               }
            }
         }
         catch (Exception ex) {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void treeViewFolder_AfterSelect(object sender, TreeViewEventArgs e)
      {
         this.ReadFolder(this.treeViewFolder.SelectedNode.FullPath);
      }

      private void LoadPackage(string filePath)
      {
         if (string.IsNullOrEmpty(filePath)) {
            throw new ArgumentNullException(nameof(filePath));
         }


         this.currentPackage = this.reader.Read(filePath);
         this.treeViewFolder.Nodes.Add(this.currentPackage.ContentId, this.currentPackage.ContentId);
         this.treeViewFolder.SelectedNode = treeViewFolder.Nodes[0];

         // Add Treeview.
         foreach (PS3PackageReader.File file in this.currentPackage.Files) {
            if (file.Type == 0x04) {
               AddFolder(file.Name, file.FolderName);
            }
         }

         ReadFolder(treeViewFolder.SelectedNode.FullPath);
         SetWindowTitle(this.currentPackage.FileName);
      }

      private void ReadFolder(string folderName)
      {
         this.listViewFiles.Items.Clear();

         try {
            TreeNode[] node = this.treeViewFolder.Nodes.Find(folderName, true);
            for (int i = 0; i <= node[0].Nodes.Count - 1; i++)
               AddFile(node[0].Nodes[i].Text, -1);

            List<PS3PackageReader.File> files = this.currentPackage.Files.FindAll(x => x.FolderName == folderName);
            foreach (PS3PackageReader.File file in files) {
               if (file.Type != 0x04) {
                  AddFile(file.Name, file.Size);
               }
            }
         }
         catch {
         }
      }

      private void AddFolder(string name, string node = "")
      {
         if (string.IsNullOrEmpty(node)) {
            for (int i = 0; i <= treeViewFolder.Nodes.Count - 1; i++) {
               if (treeViewFolder.Nodes[i].Text == name)
                  return;
            }

            treeViewFolder.Nodes.Add(name, name);
         }
         else {
            string[] split = node.Split('\\');
            for (int i = 1; i <= split.Length; i++) {
               try {
                  string path = "";
                  for (int j = 0; j <= i - 1; j++) {
                     path = split[j];
                     if (j > 0)
                        path = path + "\\" + split[j];
                  }

                  // 現在のパスを検索して追加する。
                  TreeNode[] findNode = treeViewFolder.Nodes.Find(path, true);
                  path = name;
                  if (i < split.Length)
                     path = split[i];

                  bool existe = false;
                  if (findNode.Length > 0) {
                     for (int j = 0; j <= findNode[0].Nodes.Count - 1; j++) {
                        if (findNode[0].Nodes[j].Text == path)
                           existe = true;
                     }
                  }

                  if (!existe) {
                     findNode[0].Nodes.Add(findNode[0].FullPath + "\\" + path, path);
                     findNode[0].Expand();
                  }
               }
               catch {
                  MessageBox.Show("Load of 1 item failed.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
               }
            }
         }
      }

      private void AddFile(string name, long size)
      {
         // remove of folder name.
         if (name.Contains("\\")) {
            int index = name.IndexOf('\\');
            name = name.Remove(0, index + 1);
         }

         string extension = Path.GetExtension(name);
         ListViewItem item = new ListViewItem();
         item.Text = name;
         item.ImageIndex = GetImageIndex(extension);
         item.SubItems.Add(
             SizeOf(size)
         );
         listViewFiles.Items.Add(item);
      }

      private int GetImageIndex(string extension)
      {
         // 0 for 'File' icon
         // 1 for 'Close Folder' icon
         // 2 for 'Open Folder' icon
         // 3 for 'Image' icon
         if (string.IsNullOrEmpty(extension))
            return 2;

         switch (extension.ToUpper()) {
            case ".PNG":
               return 3;
         }

         return 0;
      }


      private string SizeOf(long size)
      {
         if (size <= 0) // TODO: overflow fix. 
            return "0";

         int index = (int)Math.Log(size, 1024);
         decimal received = (decimal)size / ((long)1 << (index * 10));
         return string.Format("{0:n1}{1}", received, SIZE_PREFIX[index]);
      }

      private void newToolStripMenuItem_Click(object sender, EventArgs e)
      {
         try {

         }
         catch {

         }
      }
   }
}
