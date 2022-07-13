

namespace PS3_PackageViewer
{
   using System;
   using System.Windows.Forms;

   internal static class Program
   {
      /// <summary>
      /// アプリケーションのメイン エントリ ポイントです。
      /// </summary>
      [STAThread]
      static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new frmMain());
      }
   }
}
