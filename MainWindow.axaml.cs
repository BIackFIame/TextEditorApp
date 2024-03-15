using Avalonia.Controls;
using Avalonia.Interactivity;
using System.IO;
using Avalonia.Markup.Xaml;
using System.Collections;
using Avalonia.LogicalTree;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System.Linq;

namespace TextEditorAp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            OpenNewDocument();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var tabControl = this.FindControl<TabControl>("DocumentsTab");
            if (tabControl?.SelectedItem is TabItem selectedTab && selectedTab.Content is TextBox textBox)
            {
                if (selectedTab.Tag is string filePath && !string.IsNullOrEmpty(filePath))
                {
                    await File.WriteAllTextAsync(filePath, textBox.Text);
                }
                else
                {
                    var result = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                    {
                        DefaultExtension = "txt",
                        FileTypeChoices = new[] { new FilePickerFileType("Text Files") { Patterns = new[] { "*.txt" } } }
                    });
                    if (result != null)
                    {
                        await File.WriteAllTextAsync(result.Path.LocalPath, textBox.Text);
                        selectedTab.Header = Path.GetFileName(result.Path.LocalPath);
                        selectedTab.Tag = result.Path.LocalPath;
                    }
                }
            }
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                FileTypeFilter = new[] { new FilePickerFileType("Text Files") { Patterns = new[] { "*.txt" } } }
            });

            if (result != null && result.Count > 0)
            {
                var selectedFile = result[0];
                var filePath = selectedFile.Path.LocalPath;
                if (!IsFileAlreadyOpen(filePath))
                {
                    var text = await File.ReadAllTextAsync(filePath);
                    OpenNewDocument(text, filePath);
                }
            }
        }

        private bool IsFileAlreadyOpen(string filePath)
        {
            var tabControl = this.FindControl<TabControl>("DocumentsTab");
            if (tabControl != null)
            {
                foreach (var tabItem in from TabItem tabItem in tabControl.Items
                                        where tabItem.Tag is string openFilePath && openFilePath == filePath
                                        select tabItem)
                {
                    tabControl.SelectedItem = tabItem;
                    return true;
                }
            }
            return false;
        }

        private void OpenNewDocument(string text = "", string? filePath = null)
        {
            var textBox = new TextBox { AcceptsReturn = true, AcceptsTab = true, Text = text };
            var header = string.IsNullOrEmpty(filePath) ? "New Document" : Path.GetFileName(filePath!);
            var tabItem = new TabItem { Header = header, Content = textBox, Tag = filePath };
            var tabControl = this.FindControl<TabControl>("DocumentsTab");
            if (tabControl != null)
            {
                tabControl.Items.Add(tabItem);
                tabControl.SelectedItem = tabItem;
            }
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.TemplatedParent is TabItem tabItem)
            {
                var tabControl = this.FindControl<TabControl>("DocumentsTab");
                if (tabControl != null)
                {
                    int index = tabControl.Items.IndexOf(tabItem);
                    if (index >= 0)
                    {
                        tabControl.Items.RemoveAt(index);
                    }
                }
            }
        }
    }
}