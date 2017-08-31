// ///////////////////////////////////
// File: BillItemEditViewModelTest.cs
// Last Change: 22.08.2017  20:59
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.ComponentModel;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using NUnit.Framework;



    [TestFixture]
    public class BillItemEditViewModelTest
    {
        #region Fields

        private BillItemEditViewModel billItemEditViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.billItemEditViewModel = new BillItemEditViewModel();
        }

        [TearDown]
        public void Cleanup()
        {
            this.billItemEditViewModel = null;
            GC.Collect();
        }

        #endregion



        #region Test Methods

        [Test]
        public void DerivesFromBillWorkspaceViewModel()
        {
            typeof(BillItemEditViewModel).Should().BeDerivedFrom<BillWorkspaceViewModel>();
        }

        [Test]
        public void LoadBillItemDetailViewModelsViaPassedBill()
        {
            // Arrange
            Bill bill = ModelFactory.GetDefaultBill();

            // Act
            this.billItemEditViewModel.LoadBill(bill);

            // Assert
            this.billItemEditViewModel.BillItemDetailViewModels.Should().HaveCount(1);
            this.billItemEditViewModel.BillItemDetailViewModels[0].Description.Should().Be(ModelFactory.DefaultBillItemDescription);
        }

        [Test]
        public void LoadBillItemDetailViewModelsSortedByPositionViaPassedBill()
        {
            // Arrange
            Bill bill = ModelFactory.GetDefaultBill();
            bill.BillItems.Clear();

            BillItem billItem1 = new BillItem() { Position = 2 };
            BillItem billItem2 = new BillItem() { Position = 1 };

            bill.AddBillItem(billItem1);
            bill.AddBillItem(billItem2);

            // Act
            this.billItemEditViewModel.LoadBill(bill);

            // Assert
            this.billItemEditViewModel.BillItemDetailViewModels.Should().HaveCount(2);
            this.billItemEditViewModel.BillItemDetailViewModels[0].Position.Should().Be(1);
            this.billItemEditViewModel.BillItemDetailViewModels[1].Position.Should().Be(2);
        }

        [Test]
        public void EnableEditingOnEnableEditingMessage()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));

            // Assert
            this.billItemEditViewModel.IsEditingEnabled.Should().BeTrue();
        }

        [Test]
        public void DisableEditingOnDisableEditingMessage()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            Messenger.Default.Send(new NotificationMessage<bool>(false, Resources.Message_EnableStateForBillItemEditVM));

            // Assert
            this.billItemEditViewModel.IsEditingEnabled.Should().BeFalse();
        }

        [Test]
        public void ClearsCurrentlyLoadedBillAndBillItems()
        {
            // Arrange
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            this.billItemEditViewModel.Clear();

            // Assert
            this.billItemEditViewModel.BillItemDetailViewModels.Should().HaveCount(0);
        }

        [Test]
        public void AddsNewBillItem()
        {
            // Arrange
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));

            // Act
            this.billItemEditViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Assert
            this.billItemEditViewModel.BillItemDetailViewModels.Count.Should().Be(2);
        }

        [Test]
        public void CanNotAddNewBillItemWhenEditingNotEnabled()
        {
            // Act
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Assert
            this.billItemEditViewModel.AddItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void DeletesBillItem()
        {
            // Act
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            this.billItemEditViewModel.SelectedBillItemDetailViewModel = this.billItemEditViewModel.BillItemDetailViewModels[0];
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            this.billItemEditViewModel.DeleteItemCommand.RelayCommand.Execute(null);

            // Assert
            this.billItemEditViewModel.SelectedBillItemDetailViewModel.Should().BeNull();
            this.billItemEditViewModel.BillItemDetailViewModels.Count.Should().Be(0);
        }

        [Test]
        public void UpdatesPositionsWhenBillItemWasDeleted()
        {
            // Arrange
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            this.billItemEditViewModel.LoadBill(bill);
            this.billItemEditViewModel.SelectedBillItemDetailViewModel = this.billItemEditViewModel.BillItemDetailViewModels[2];
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            this.billItemEditViewModel.DeleteItemCommand.RelayCommand.Execute(null);

            // Assert
            for (int i = 0; i < this.billItemEditViewModel.BillItemDetailViewModels.Count; i++)
            {
                int currentPosition = i + 1;
                this.billItemEditViewModel.BillItemDetailViewModels[i].Position.Should().Be(currentPosition);
            }
        }

        [Test]
        public void CanNotDeleteBillItemWhenEditingNotEnabled()
        {
            // Act
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Assert
            this.billItemEditViewModel.DeleteItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotDeleteBillItemWhenNoItemIsSelected()
        {
            // Arrange
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));

            // Assert
            this.billItemEditViewModel.DeleteItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void RaisesPropertyChangedWhenSelectedBillItemChanges()
        {
            // Arrange
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            this.billItemEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this.billItemEditViewModel.SelectedBillItemDetailViewModel = new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem());

            // Assert
            this.billItemEditViewModel.SelectedBillItemDetailViewModel.Should().NotBeNull();
            this.billItemEditViewModel.ShouldRaisePropertyChangeFor(x => x.SelectedBillItemDetailViewModel);
        }

        [Test]
        public void MovesItemUp()
        {
            // Arrange
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            this.billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            this.billItemEditViewModel.SelectedBillItemDetailViewModel = this.billItemEditViewModel.BillItemDetailViewModels[2];
            this.billItemEditViewModel.MoveItemUpCommand.RelayCommand.Execute(null);

            // Assert
            this.billItemEditViewModel.BillItemDetailViewModels[1].Position.Should().Be(2);
            this.billItemEditViewModel.BillItemDetailViewModels[1].Amount.Should().Be(3);
            this.billItemEditViewModel.BillItemDetailViewModels[2].Position.Should().Be(3);
            this.billItemEditViewModel.BillItemDetailViewModels[2].Amount.Should().Be(2);
        }

        [Test]
        public void CanNotMoveItemUpWhenEditingNotEnabled()
        {
            // Arrange
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            this.billItemEditViewModel.LoadBill(bill);
            this.billItemEditViewModel.SelectedBillItemDetailViewModel = this.billItemEditViewModel.BillItemDetailViewModels[2];

            // Assert
            this.billItemEditViewModel.MoveItemUpCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotMoveItemUpWhenNoItemIsSelected()
        {
            // Arrange
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            this.billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));

            // Assert
            this.billItemEditViewModel.MoveItemUpCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotMoveItemUpWhenOnFirstPosition()
        {
            // Arrange
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            this.billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            this.billItemEditViewModel.SelectedBillItemDetailViewModel = this.billItemEditViewModel.BillItemDetailViewModels[0];

            // Assert
            this.billItemEditViewModel.MoveItemUpCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void MovesItemDown()
        {
            // Arrange
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            this.billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            this.billItemEditViewModel.SelectedBillItemDetailViewModel = this.billItemEditViewModel.BillItemDetailViewModels[2];
            this.billItemEditViewModel.MoveItemDownCommand.RelayCommand.Execute(null);

            // Assert
            this.billItemEditViewModel.BillItemDetailViewModels[2].Position.Should().Be(3);
            this.billItemEditViewModel.BillItemDetailViewModels[2].Amount.Should().Be(4);
            this.billItemEditViewModel.BillItemDetailViewModels[3].Position.Should().Be(4);
            this.billItemEditViewModel.BillItemDetailViewModels[3].Amount.Should().Be(3);
        }

        [Test]
        public void CanNotMoveItemDownWhenEditingNotEnabled()
        {
            // Arrange
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            this.billItemEditViewModel.LoadBill(bill);
            this.billItemEditViewModel.SelectedBillItemDetailViewModel = this.billItemEditViewModel.BillItemDetailViewModels[2];

            // Assert
            this.billItemEditViewModel.MoveItemDownCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotMoveItemDownWhenNoItemIsSelected()
        {
            // Arrange
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            this.billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));

            // Assert
            this.billItemEditViewModel.MoveItemDownCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotMoveItemDownWhenOnLastPosition()
        {
            // Arrange
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            this.billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            this.billItemEditViewModel.SelectedBillItemDetailViewModel = this.billItemEditViewModel.BillItemDetailViewModels[4];

            // Assert
            this.billItemEditViewModel.MoveItemDownCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void ClearBillWhenContainingClientWasDeleted()
        {
            // Arrange
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_RemoveClientForBillItemEditVM));

            // Assert
            this.billItemEditViewModel.BillItemDetailViewModels.Count.Should().Be(0);
        }

        #endregion
    }
}