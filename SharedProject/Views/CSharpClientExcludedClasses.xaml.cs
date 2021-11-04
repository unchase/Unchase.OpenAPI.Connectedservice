//-----------------------------------------------------------------------
// <copyright file="CSharpClientExcludedClasses.cs" company="Unchase">
//     Copyright (c) Nikolay Chebotov (Unchase). All rights reserved.
// </copyright>
// <license>https://github.com/unchase/Unchase.OpenAPI.Connectedservice/blob/master/LICENSE.md</license>
// <author>Nickolay Chebotov (Unchase), spiritkola@hotmail.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Unchase.OpenAPI.ConnectedService.Views
{
    /// <summary>
    /// Логика взаимодействия для CSharpClientExcludedClasses.xaml
    /// </summary>
    public partial class CSharpClientExcludedClasses : Window
    {
        public ObservableCollection<Class> Classes { get; set; }

        public class Class
        {
            public string Name { get; set; }

            public bool Excluded { get; set; }
        }

        public CSharpClientExcludedClasses(IEnumerable<string> classNames)
        {
            InitializeComponent();
            Classes = new ObservableCollection<Class>();
            foreach (var className in classNames)
            {
                Classes.Add(new Class
                {
                    Name = className,
                    Excluded = false
                });
            }
            ExcludedClassesListBox.ItemsSource = Classes;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
