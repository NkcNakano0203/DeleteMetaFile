using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace DeleteMetaFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            string sourceFolderPass = PathTextBox.Text;
            // 実行したいコマンド
            string powerShellCommand = $"cd {sourceFolderPass} ; Get-ChildItem *.meta -Recurse | Remove-Item";

            OpenAndStart(powerShellCommand);
        }

        /// <summary>
        /// PowerShellの実行
        /// </summary>
        /// <param name="command">PowerShellコマンド</param>
        private void OpenAndStart(string command)
        {
            Process cmd = new();
            cmd.StartInfo.FileName = "PowerShell.exe";
            //PowerShellのウィンドウを出さずに実行。
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = command;
            cmd.Start();
        }

        private void FolderDrop(object sender, DragEventArgs e)
        {
            // ドロップされたアイテムがフォルダか判別
            bool isFolder = IsDroppedItemFolder(e.Data);

            e.Effects = isFolder switch
            {
                true => DragDropEffects.Copy,
                false => DragDropEffects.None,
            };
            // フォルダ以外は受け付けない
            if (!isFolder) return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            // ドロップされたフォルダのパスを保持
            PathTextBox.Text = files[0];
        }

        private bool IsDroppedItemFolder(IDataObject data)
        {
            // ドロップされたアイテムがフォルダか
            return !data.GetDataPresent(DataFormats.FileDrop)
            || ((string[])data.GetData(DataFormats.FileDrop)).Length == 1
            || Directory.Exists(((string[])data.GetData(DataFormats.FileDrop))[0]);
        }

        private void OpenFolderButtonClick(object sender, RoutedEventArgs e)
        {
            using (var cofd = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください",
                IsFolderPicker = true
            })
            {
                if (cofd.ShowDialog() != CommonFileDialogResult.Ok) return;

                // 選択されたフォルダ名を保持
                PathTextBox.Text = cofd.FileName;
            }
        }
    }
}