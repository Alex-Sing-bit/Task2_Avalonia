using System;
using System.Collections.ObjectModel;

namespace AvaloniaApplication4.Models
{
    public class File : FileSystemElement
    {
        public long FileSize { get; }

        public override FileSystemElementType ElementType => FileSystemElementType.File;

        public override long Size => FileSize;

        public File(string name, long size, FileSystemElement? parent) : base(name, parent)
        {
            FileSize = size;
        }

        // Метод для создания копии файла
        public override FileSystemElement Clone()
        {
            return new File(Name, FileSize, Parent);
        }

        // Метод для добавления элемента (не поддерживается для файла)
        public override void AddElement(FileSystemElement element)
        {
            throw new InvalidOperationException("Cannot add elements to a file.");
        }

        // Метод для удаления элемента (не поддерживается для файла)
        public override void RemoveElement(FileSystemElement element)
        {
            throw new InvalidOperationException("Cannot remove elements from a file.");
        }

        public override string ToString()
        {
            return $"{Location}.me_file ({Size})";
        }
    }
}