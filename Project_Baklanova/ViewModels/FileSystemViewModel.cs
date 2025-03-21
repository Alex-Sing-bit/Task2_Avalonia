using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Controls;
using AvaloniaApplication4.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace AvaloniaApplication4.ViewModels
{
    // ViewModel для управления файловой системой
    public class FileSystemViewModel : INotifyPropertyChanged
    {
        private Window _parentWindow;

        // Свойство для хранения родительского окна
        public Window ParentWindow
        {
            get => _parentWindow;
            set
            {
                _parentWindow = value;
                OnPropertyChanged(nameof(ParentWindow));
            }
        }

        private FileSystemElement? _selectedElement;
        private Folder? _selectedFolder;

        // Команды для взаимодействия с файловой системой
        public ICommand AddFolderCommand { get; }
        public ICommand AddFileCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand MoveCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand ShowFoldersCommand { get; }

        private string _newElement = string.Empty;

        // Свойство для хранения имени нового элемента (файла или папки)
        public string NewElement
        {
            get => _newElement;
            set
            {
                _newElement = value.Trim();
                OnPropertyChanged();
            }
        }

        // Корневая папка файловой системы
        public Folder Root { get; set; }

        // Выбранный элемент в файловой системе
        public FileSystemElement? SelectedElement
        {
            get => _selectedElement;
            set
            {
                if (_selectedElement != value)
                {
                    _selectedElement = value;
                    OnPropertyChanged();
                }
            }
        }

        // Выбранная папка для перемещения или копирования
        public Folder? SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                if (_selectedFolder != value)
                {
                    _selectedFolder = value;
                    OnPropertyChanged();
                }
            }
        }

        // Коллекция всех папок в файловой системе
        public ObservableCollection<Folder> Folders { get; set; }

        // Конструктор ViewModel
        public FileSystemViewModel(Window parentWindow)
        {
            if (parentWindow == null)
            {
                throw new ArgumentNullException(nameof(parentWindow), "Родительское окно не может быть null.");
            }

            ParentWindow = parentWindow;

            // Инициализация команд
            AddFolderCommand = new RelayCommand(AddFolder);
            AddFileCommand = new RelayCommand(AddFile);
            RemoveCommand = new RelayCommand(Remove);
            MoveCommand = new RelayCommand(MoveElement);
            CopyCommand = new RelayCommand(CopyElement);

            // Создание корневой папки и инициализация коллекций
            Root = new Folder("Root", null);
            SelectedElement = Root;
            Folders = new ObservableCollection<Folder>();

            // Подписываемся на изменения в дереве
            SubscribeToTreeChanges(Root);
            UpdateFolders();
        }

        // Метод для добавления новой папки
        private void AddFolder()
        {
            if (_selectedElement == null || _selectedElement is not Folder selectedFolder)
            {
                return;
            }

            var newFolder = new Folder((!string.IsNullOrEmpty(NewElement)) ? NewElement : "New Folder", selectedFolder);
            selectedFolder.AddElement(newFolder);
            SelectedElement = newFolder;
            NewElement = string.Empty;

            // Подписываемся на изменения в новой папке
            SubscribeToTreeChanges(newFolder);
            UpdateFolders();
        }

        // Метод для добавления нового файла
        private void AddFile()
        {
            if (_selectedElement == null || _selectedElement is not Folder selectedFolder)
            {
                return;
            }

            var newFile = new File((!string.IsNullOrEmpty(NewElement)) ? NewElement : "New File", 4, selectedFolder);
            selectedFolder.AddElement(newFile);
            SelectedElement = newFile;
            NewElement = string.Empty;
        }

        // Метод для удаления выбранного элемента
        private void Remove()
        {
            if (_selectedElement == null || _selectedElement.Parent is not Folder parent)
            {
                return;
            }

            parent.RemoveElement(_selectedElement);
            SelectedElement = parent;

            UpdateFolders();
        }

        // Метод для перемещения выбранного элемента в другую папку
        private async void MoveElement()
        {
            if (SelectedElement == null || SelectedFolder == null || SelectedElement == SelectedFolder)
            {
                return;
            }

            if (SelectedElement.Parent is not Folder parentFolder)
            {
                return;
            }

            if (SelectedFolder.Children.Any(e => e.Name == SelectedElement.Name))
            {
                return;
            }

            FileSystemElement.Move(SelectedElement, SelectedFolder);
            UpdateFolders();
        }

        // Метод для копирования выбранного элемента в другую папку
        private async void CopyElement()
        {
            FileSystemElement parent = SelectedFolder;
            if (SelectedElement == null || SelectedElement == SelectedFolder)
            {
                return;
            }

            if (parent == null)
            {
                parent = SelectedElement.Parent == null ? Root : SelectedElement.Parent;
            }

            FileSystemElement.Copy(SelectedElement, parent);
            UpdateFolders();
        }

        // Метод для подписки на изменения в дереве папок
        private void SubscribeToTreeChanges(Folder folder)
        {
            folder.Children.CollectionChanged += (sender, e) =>
            {
                UpdateFolders();
            };

            foreach (var element in folder.Children)
            {
                if (element is Folder subFolder)
                {
                    SubscribeToTreeChanges(subFolder);
                }
            }
        }

        // Метод для обновления списка всех папок
        private void UpdateFolders()
        {
            Folders.Clear();
            var allFolders = GetAllFolders(Root);
            foreach (var folder in allFolders)
            {
                Folders.Add(folder);
            }
        }

        // Метод для получения всех папок в файловой системе
        private ObservableCollection<Folder> GetAllFolders(Folder folder)
        {
            var stack = new Stack<Folder>();
            stack.Push(folder);

            var folders = new List<Folder>();

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                folders.Add(current);

                foreach (var element in current.Children)
                {
                    if (element is Folder subFolder)
                    {
                        stack.Push(subFolder);
                    }
                }
            }

            return new ObservableCollection<Folder>(folders);
        }

        // Событие для уведомления об изменении свойств
        public event PropertyChangedEventHandler? PropertyChanged;

        // Метод для вызова события PropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}