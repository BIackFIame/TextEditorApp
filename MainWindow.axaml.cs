using Avalonia.Controls;
using Avalonia.Interactivity;
using System.IO;
using System;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Collections;
using Avalonia.LogicalTree;

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
            if (tabControl.SelectedItem is TabItem selectedTab && selectedTab.Content is TextBox textBox)
            {
                if (selectedTab.Tag is string filePath && !string.IsNullOrEmpty(filePath))
                {
                    // Если у нас уже есть путь к файлу, просто перезаписываем его
                    await File.WriteAllTextAsync(filePath, textBox.Text);
                }
                else
                {
                    // Иначе отображаем диалог сохранения файла
                    var dialog = new SaveFileDialog();
                    var result = await dialog.ShowAsync(this);
                    if (result != null)
                    {
                        await File.WriteAllTextAsync(result, textBox.Text);
                        selectedTab.Header = Path.GetFileName(result); // Обновляем название вкладки
                        selectedTab.Tag = result; // Сохраняем путь к файлу в Tag
                    }
                }
            }
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false
            };
            var result = await dialog.ShowAsync(this);
            if (result != null && result.Length > 0)
            {
                var text = await File.ReadAllTextAsync(result[0]);
                OpenNewDocument(text, result[0]);
            }
        }

        private void OpenNewDocument(string text = "", string filePath = null)
        {
            var textBox = new TextBox
            {
                AcceptsReturn = true,
                AcceptsTab = true,
                Text = text
            };

            var header = string.IsNullOrEmpty(filePath) ? "New Document" : Path.GetFileName(filePath);

            var tabItem = new TabItem
            {
                Header = header,
                Content = textBox,
                Tag = filePath // Сохраняем путь к файлу в Tag
            };

            var tabControl = this.FindControl<TabControl>("DocumentsTab");
            tabControl.Items.Add(tabItem);
            tabControl.SelectedItem = tabItem;
        }

private void CloseTab_Click(object sender, RoutedEventArgs e)
{
    // sender это кнопка закрытия вкладки
    if (sender is Button button)
    {
        // Пытаемся найти TabItem, которому принадлежит кнопка
        var tabItem = button.GetLogicalParent() as TabItem;
        if (tabItem != null)
        {
            // Получаем доступ к TabControl
            var tabControl = this.FindControl<TabControl>("DocumentsTab");
            if (tabControl != null && tabControl.Items is IList items)
            {
                items.Remove(tabItem);
            }
        }
    }
}


    }
}
