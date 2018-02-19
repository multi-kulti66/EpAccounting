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
            const string expectedTitle = "new Title";
            const int expectedNumber = 32;
            const string expectedName = "Andre";

            // Act
            BindableTestClass testClass = new BindableTestClass
                                          {
                                              Title = expectedTitle,
                                              Number = expectedNumber,
                                              Name = expectedName
                                          };

            // Assert
            testClass.Title.Should().Be(expectedTitle);
            testClass.Number.Should().Be(expectedNumber);
            testClass.Name.Should().Be(expectedName);
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
            const string expectedTitle = "new Title";
            const int expectedNumber = 32;
            const string expectedName = "Andre";

            BindableTestClass testClass = new BindableTestClass(expectedTitle, expectedNumber, expectedName);
            testClass.MonitorEvents<INotifyPropertyChanged>();

            // Act
            testClass.Title = expectedTitle;
            testClass.Number = expectedNumber;
            testClass.Name = expectedName;

            // Assert
            testClass.ShouldNotRaisePropertyChangeFor(x => x.Title);
            testClass.ShouldNotRaisePropertyChangeFor(x => x.Number);
            testClass.ShouldNotRaisePropertyChangeFor(x => x.Name);
        }


        private class BindableTestClass : BindableViewModelBase
        {
            private readonly SubTestClass _subTestClass = new SubTestClass();

            private string _title;
            private int _number;


            public BindableTestClass(string title = "", int number = 0, string name = "")
            {
                this._title = title;
                this._number = number;
                this._subTestClass.Name = name;
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
                get { return this._subTestClass.Name; }
                set { this.SetProperty(() => this._subTestClass.Name = value, () => this._subTestClass.Name == value); }
            }


            private class SubTestClass
            {
                public string Name { get; set; }
            }
        }
    }
}