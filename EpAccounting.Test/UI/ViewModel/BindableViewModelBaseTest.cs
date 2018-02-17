// ///////////////////////////////////
// File: BindableViewModelBaseTest.cs
// Last Change: 18.09.2017  20:35
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System.ComponentModel;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight;
    using NUnit.Framework;



    [TestFixture]
    public class BindableViewModelBaseTest
    {
        [Test]
        public void DerivesFromViewModelBase()
        {
            // Arrange
            BindableTestClass testClass = new BindableTestClass();

            // Assert
            testClass.GetType().Should().BeDerivedFrom<ViewModelBase>();
        }

        [Test]
        public void ChangesPropertyValue()
        {
            // Arrange
            const string ExpectedTitle = "new Title";
            const int ExpectedNumber = 32;
            const string ExpectedName = "Andre";

            // Act
            BindableTestClass testClass = new BindableTestClass
                                          {
                                              Title = ExpectedTitle,
                                              Number = ExpectedNumber,
                                              Name = ExpectedName
                                          };

            // Assert
            testClass.Title.Should().Be(ExpectedTitle);
            testClass.Number.Should().Be(ExpectedNumber);
            testClass.Name.Should().Be(ExpectedName);
        }

        [Test]
        public void RaisePropertyChangedWhenPropertyValueChanges()
        {
            // Arrange
            BindableTestClass testClass = new BindableTestClass();
            testClass.MonitorEvents<INotifyPropertyChanged>();

            // Act
            testClass.Title = "Hello";
            testClass.Number = 10;
            testClass.Name = "Michael";

            // Assert
            testClass.ShouldRaisePropertyChangeFor(x => x.Title);
            testClass.ShouldRaisePropertyChangeFor(x => x.Number);
            testClass.ShouldRaisePropertyChangeFor(x => x.Name);
        }

        [Test]
        public void DoNotRaisePropertyChangedWhenPropertyValuesGetSameValue()
        {
            // Arrange
            const string ExpectedTitle = "new Title";
            const int ExpectedNumber = 32;
            const string ExpectedName = "Andre";

            BindableTestClass testClass = new BindableTestClass(ExpectedTitle, ExpectedNumber, ExpectedName);
            testClass.MonitorEvents<INotifyPropertyChanged>();

            // Act
            testClass.Title = ExpectedTitle;
            testClass.Number = ExpectedNumber;
            testClass.Name = ExpectedName;

            // Assert
            testClass.ShouldNotRaisePropertyChangeFor(x => x.Title);
            testClass.ShouldNotRaisePropertyChangeFor(x => x.Number);
            testClass.ShouldNotRaisePropertyChangeFor(x => x.Name);
        }


        private class BindableTestClass : BindableViewModelBase
        {
            private readonly SubTestClass subTestClass = new SubTestClass();

            private string _title;
            private int _number;


            public BindableTestClass(string title = "", int number = 0, string name = "")
            {
                this._title = title;
                this._number = number;
                this.subTestClass.Name = name;
            }


            public string Title
            {
                get { return this._title; }
                set { this.SetProperty(ref this._title, value); }
            }

            public int Number
            {
                get { return this._number; }
                set { this.SetProperty(ref this._number, value); }
            }

            public string Name
            {
                get { return this.subTestClass.Name; }
                set { this.SetProperty(() => this.subTestClass.Name = value, () => this.subTestClass.Name == value); }
            }


            private class SubTestClass
            {
                public string Name { get; set; }
            }
        }
    }
}