namespace PS3_PackageViewer
{
   partial class frmMain
   {
      /// <summary>
      /// 必要なデザイナー変数です。
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// 使用中のリソースをすべてクリーンアップします。
      /// </summary>
      /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null)) {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows フォーム デザイナーで生成されたコード

      /// <summary>
      /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
      /// コード エディターで変更しないでください。
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
         this.statusStrip1 = new System.Windows.Forms.StatusStrip();
         this.menuStrip1 = new System.Windows.Forms.MenuStrip();
         this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
         this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
         this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
         this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.listViewFiles = new System.Windows.Forms.ListView();
         this.treeViewFolder = new System.Windows.Forms.TreeView();
         this.splitContainer1 = new System.Windows.Forms.SplitContainer();
         this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.imageListIcons = new System.Windows.Forms.ImageList(this.components);
         this.contextMenuStripFile = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
         this.extractorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.menuStrip1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
         this.splitContainer1.Panel1.SuspendLayout();
         this.splitContainer1.Panel2.SuspendLayout();
         this.splitContainer1.SuspendLayout();
         this.contextMenuStripFile.SuspendLayout();
         this.SuspendLayout();
         // 
         // statusStrip1
         // 
         this.statusStrip1.Location = new System.Drawing.Point(0, 428);
         this.statusStrip1.Name = "statusStrip1";
         this.statusStrip1.Size = new System.Drawing.Size(800, 22);
         this.statusStrip1.TabIndex = 1;
         this.statusStrip1.Text = "statusStrip1";
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(800, 24);
         this.menuStrip1.TabIndex = 2;
         this.menuStrip1.Text = "menuStrip1";
         // 
         // fileToolStripMenuItem
         // 
         this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
         this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
         this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
         this.fileToolStripMenuItem.Text = "File";
         // 
         // toolsToolStripMenuItem
         // 
         this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractorToolStripMenuItem});
         this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
         this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
         this.toolsToolStripMenuItem.Text = "Tools";
         // 
         // helpToolStripMenuItem
         // 
         this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
         this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
         this.helpToolStripMenuItem.Text = "Help";
         // 
         // newToolStripMenuItem
         // 
         this.newToolStripMenuItem.Enabled = false;
         this.newToolStripMenuItem.Name = "newToolStripMenuItem";
         this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
         this.newToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
         this.newToolStripMenuItem.Text = "New";
         this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
         // 
         // openToolStripMenuItem
         // 
         this.openToolStripMenuItem.Name = "openToolStripMenuItem";
         this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
         this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
         this.openToolStripMenuItem.Text = "Open";
         this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
         // 
         // toolStripSeparator1
         // 
         this.toolStripSeparator1.Name = "toolStripSeparator1";
         this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
         // 
         // closeToolStripMenuItem
         // 
         this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
         this.closeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
         this.closeToolStripMenuItem.Text = "Close";
         // 
         // toolStripSeparator2
         // 
         this.toolStripSeparator2.Name = "toolStripSeparator2";
         this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
         // 
         // saveToolStripMenuItem
         // 
         this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
         this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
         this.saveToolStripMenuItem.Text = "Save";
         // 
         // saveAsToolStripMenuItem
         // 
         this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
         this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
         this.saveAsToolStripMenuItem.Text = "Save as...";
         // 
         // toolStripSeparator3
         // 
         this.toolStripSeparator3.Name = "toolStripSeparator3";
         this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
         // 
         // exitToolStripMenuItem
         // 
         this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
         this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
         this.exitToolStripMenuItem.Text = "Exit";
         // 
         // listViewFiles
         // 
         this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderSize,
            this.columnHeaderType});
         this.listViewFiles.ContextMenuStrip = this.contextMenuStripFile;
         this.listViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
         this.listViewFiles.FullRowSelect = true;
         this.listViewFiles.HideSelection = false;
         this.listViewFiles.LargeImageList = this.imageListIcons;
         this.listViewFiles.Location = new System.Drawing.Point(0, 0);
         this.listViewFiles.MultiSelect = false;
         this.listViewFiles.Name = "listViewFiles";
         this.listViewFiles.Size = new System.Drawing.Size(530, 404);
         this.listViewFiles.SmallImageList = this.imageListIcons;
         this.listViewFiles.TabIndex = 3;
         this.listViewFiles.UseCompatibleStateImageBehavior = false;
         this.listViewFiles.View = System.Windows.Forms.View.Details;
         // 
         // treeViewFolder
         // 
         this.treeViewFolder.Dock = System.Windows.Forms.DockStyle.Fill;
         this.treeViewFolder.ImageIndex = 2;
         this.treeViewFolder.ImageList = this.imageListIcons;
         this.treeViewFolder.Location = new System.Drawing.Point(0, 0);
         this.treeViewFolder.Name = "treeViewFolder";
         this.treeViewFolder.SelectedImageIndex = 2;
         this.treeViewFolder.Size = new System.Drawing.Size(266, 404);
         this.treeViewFolder.TabIndex = 4;
         this.treeViewFolder.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewFolder_AfterSelect);
         // 
         // splitContainer1
         // 
         this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer1.Location = new System.Drawing.Point(0, 24);
         this.splitContainer1.Name = "splitContainer1";
         // 
         // splitContainer1.Panel1
         // 
         this.splitContainer1.Panel1.Controls.Add(this.treeViewFolder);
         // 
         // splitContainer1.Panel2
         // 
         this.splitContainer1.Panel2.Controls.Add(this.listViewFiles);
         this.splitContainer1.Size = new System.Drawing.Size(800, 404);
         this.splitContainer1.SplitterDistance = 266;
         this.splitContainer1.TabIndex = 5;
         // 
         // columnHeaderName
         // 
         this.columnHeaderName.Text = "FileName";
         this.columnHeaderName.Width = 306;
         // 
         // columnHeaderSize
         // 
         this.columnHeaderSize.Text = "Size";
         this.columnHeaderSize.Width = 120;
         // 
         // columnHeaderType
         // 
         this.columnHeaderType.Text = "Type";
         this.columnHeaderType.Width = 94;
         // 
         // imageListIcons
         // 
         this.imageListIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListIcons.ImageStream")));
         this.imageListIcons.TransparentColor = System.Drawing.Color.Transparent;
         this.imageListIcons.Images.SetKeyName(0, "data.ico");
         this.imageListIcons.Images.SetKeyName(1, "error.ico");
         this.imageListIcons.Images.SetKeyName(2, "folder.ico");
         this.imageListIcons.Images.SetKeyName(3, "images.ico");
         // 
         // contextMenuStripFile
         // 
         this.contextMenuStripFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem1,
            this.exportToolStripMenuItem});
         this.contextMenuStripFile.Name = "contextMenuStripFile";
         this.contextMenuStripFile.Size = new System.Drawing.Size(109, 48);
         // 
         // exportToolStripMenuItem
         // 
         this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
         this.exportToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
         this.exportToolStripMenuItem.Text = "Export";
         // 
         // openToolStripMenuItem1
         // 
         this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
         this.openToolStripMenuItem1.Size = new System.Drawing.Size(108, 22);
         this.openToolStripMenuItem1.Text = "Open";
         // 
         // extractorToolStripMenuItem
         // 
         this.extractorToolStripMenuItem.Name = "extractorToolStripMenuItem";
         this.extractorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
         this.extractorToolStripMenuItem.Text = "Extractor";
         // 
         // frmMain
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(800, 450);
         this.Controls.Add(this.splitContainer1);
         this.Controls.Add(this.statusStrip1);
         this.Controls.Add(this.menuStrip1);
         this.MainMenuStrip = this.menuStrip1;
         this.Name = "frmMain";
         this.Text = "...";
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         this.splitContainer1.Panel1.ResumeLayout(false);
         this.splitContainer1.Panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
         this.splitContainer1.ResumeLayout(false);
         this.contextMenuStripFile.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.StatusStrip statusStrip1;
      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
      private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
      private System.Windows.Forms.ListView listViewFiles;
      private System.Windows.Forms.ColumnHeader columnHeaderName;
      private System.Windows.Forms.ColumnHeader columnHeaderSize;
      private System.Windows.Forms.ColumnHeader columnHeaderType;
      private System.Windows.Forms.TreeView treeViewFolder;
      private System.Windows.Forms.SplitContainer splitContainer1;
      private System.Windows.Forms.ImageList imageListIcons;
      private System.Windows.Forms.ToolStripMenuItem extractorToolStripMenuItem;
      private System.Windows.Forms.ContextMenuStrip contextMenuStripFile;
      private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
   }
}

