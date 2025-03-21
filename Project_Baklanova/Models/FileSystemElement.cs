using System.Collections.Generic;
using System.Linq;

namespace AvaloniaApplication4.Models
{
    // Абстрактный класс, представляющий элемент файловой системы (файл или папку)
    public abstract class FileSystemElement
    {
        public string Name { get; set; }

        // Родительский элемент (папка, в которой находится текущий элемент)
        public FileSystemElement? Parent { get; set; }

        public abstract FileSystemElementType ElementType { get; }

        public abstract long Size { get; }

        // Свойство для получения полного пути элемента в файловой системе
        public string Location => Parent == null ? Name : $"{Parent.Location}/{Name}";

        // Виртуальное свойство для получения дочерних элементов
        public virtual IEnumerable<FileSystemElement> Children => Enumerable.Empty<FileSystemElement>();

        protected FileSystemElement(string name, FileSystemElement? parent)
        {
            Name = name;
            Parent = parent;
        }

        // Статический метод для перемещения элемента в другую папку
        public static void Move(FileSystemElement element, FileSystemElement newParent)
        {
            if (element.Parent != null)
            {
                element.Parent.RemoveElement(element);
            }
            newParent.AddElement(element);
        }

        // Статический метод для копирования элемента в другую папку
        public static void Copy(FileSystemElement element, FileSystemElement newParent)
        {
            var copy = element.Clone();
            copy.Name += " (copy)";
            newParent.AddElement(copy);
        }

        // Абстрактный метод для создания копии элемента
        public abstract FileSystemElement Clone();

        // Абстрактный метод для добавления дочернего элемента
        public abstract void AddElement(FileSystemElement element);

        // Абстрактный метод для удаления дочернего элемента
        public abstract void RemoveElement(FileSystemElement element);
    }
}