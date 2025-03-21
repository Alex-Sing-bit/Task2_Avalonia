using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvaloniaApplication4.Models
{
    public class Folder : FileSystemElement
    {
        public override ObservableCollection<FileSystemElement> Children { get; } = new ObservableCollection<FileSystemElement>();

        public override FileSystemElementType ElementType => FileSystemElementType.Folder;

        public override long Size => CalculateSize();

        public Folder(string name, FileSystemElement? parent) : base(name, parent) { }

        // Метод для вычисления размера папки
        private long CalculateSize()
        {
            long totalSize = 0;
            // Суммируем размеры всех дочерних элементов
            foreach (var element in Children)
            {
                totalSize += element.Size;
            }
            return totalSize;
        }

        // Метод для создания копии папки (включая все дочерние элементы)
        public override FileSystemElement Clone()
        {
            var clone = new Folder(Name, Parent);
            // Копируем все дочерние элементы
            foreach (var element in Children)
            {
                clone.AddElement(element.Clone());
            }
            return clone;
        }

        // Метод для добавления дочернего элемента в папку
        public override void AddElement(FileSystemElement element)
        {
            Children.Add(element);
            // Устанавливаем текущую папку как родительскую для добавленного элемента
            element.Parent = this;
        }

        // Метод для удаления дочернего элемента из папки
        public override void RemoveElement(FileSystemElement element)
        {
            Children.Remove(element);
        }

        // Метод для строкового представления папки (путь и размер)
        public override string ToString()
        {
            return $"{Location}({Size})";
        }
    }
}