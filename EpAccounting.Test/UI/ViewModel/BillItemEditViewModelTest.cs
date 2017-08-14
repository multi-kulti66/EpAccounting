// ///////////////////////////////////
// File: BillItemEditViewModelTest.cs
// Last Change: 05.07.2017  20:32
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System.ComponentModel;
    using EpAccounting.Model;
    using EpAccounting.Test.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using NUnit.Framework;



    [TestFixture]
    public class BillItemEditViewModelTest
    {
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
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            Bill bill = ModelFactory.GetDefaultBill();

            // Act
            billItemEditViewModel.LoadBill(bill);

            // Assert
            billItemEditViewModel.BillItemDetailViewModels.Should().HaveCount(1);
            billItemEditViewModel.BillItemDetailViewModels[0].Description.Should().Be(ModelFactory.DefaultBillItemDescription);
        }

        [Test]
        public void LoadBillItemDetailViewModelsSortedByPositionViaPassedBill()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            Bill bill = ModelFactory.GetDefaultBill();
            bill.BillItems.Clear();

            BillItem billItem1 = new BillItem() {Position = 2};
            BillItem billItem2 = new BillItem() {Position = 1};

            bill.AddBillItem(billItem1);
            bill.AddBillItem(billItem2);

            // Act
            billItemEditViewModel.LoadBill(bill);

            // Assert
            billItemEditViewModel.BillItemDetailViewModels.Should().HaveCount(2);
            billItemEditViewModel.BillItemDetailViewModels[0].Position.Should().Be(1);
            billItemEditViewModel.BillItemDetailViewModels[1].Position.Should().Be(2);
        }

        [Test]
        public void EnableEditingOnEnableEditingMessage()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));

            // Assert
            billItemEditViewModel.IsEditingEnabled.Should().BeTrue();
        }

        [Test]
        public void DisableEditingOnDisableEditingMessage()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));
            Messenger.Default.Send(new NotificationMessage<bool>(false, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));

            // Assert
            billItemEditViewModel.IsEditingEnabled.Should().BeFalse();
        }

        [Test]
        public void ClearsCurrentlyLoadedBillAndBillItems()
        {
            // Arrange
            BillItemEditViewModel billItemDetailViewModel = this.GetDefaultViewModel();
            billItemDetailViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            billItemDetailViewModel.Clear();

            // Assert
            billItemDetailViewModel.BillItemDetailViewModels.Should().HaveCount(0);
        }

        [Test]
        public void AddsNewBillItem()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));

            // Act
            billItemEditViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Assert
            billItemEditViewModel.BillItemDetailViewModels.Count.Should().Be(2);
        }

        [Test]
        public void CanNotAddNewBillItemWhenEditingNotEnabled()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();

            // Act
            billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Assert
            billItemEditViewModel.AddItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void DeletesBillItem()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();

            // Act
            billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            billItemEditViewModel.SelectedBillItemDetailViewModel = billItemEditViewModel.BillItemDetailViewModels[0];
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));
            billItemEditViewModel.DeleteItemCommand.RelayCommand.Execute(null);

            // Assert
            billItemEditViewModel.SelectedBillItemDetailViewModel.Should().BeNull();
            billItemEditViewModel.BillItemDetailViewModels.Count.Should().Be(0);
        }

        [Test]
        public void UpdatesPositionsWhenBillItemWasDeleted()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            billItemEditViewModel.LoadBill(bill);
            billItemEditViewModel.SelectedBillItemDetailViewModel = billItemEditViewModel.BillItemDetailViewModels[2];
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));
            billItemEditViewModel.DeleteItemCommand.RelayCommand.Execute(null);

            // Assert
            for (int i = 0; i < billItemEditViewModel.BillItemDetailViewModels.Count; i++)
            {
                int currentPosition = i + 1;
                billItemEditViewModel.BillItemDetailViewModels[i].Position.Should().Be(currentPosition);
            }
        }

        [Test]
        public void CanNotDeleteBillItemWhenEditingNotEnabled()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();

            // Act
            billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Assert
            billItemEditViewModel.DeleteItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotDeleteBillItemWhenNoItemIsSelected()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));

            // Assert
            billItemEditViewModel.DeleteItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void RaisesPropertyChangedWhenSelectedBillItemChanges()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            billItemEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            billItemEditViewModel.SelectedBillItemDetailViewModel = new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem());

            // Assert
            billItemEditViewModel.SelectedBillItemDetailViewModel.Should().NotBeNull();
            billItemEditViewModel.ShouldRaisePropertyChangeFor(x => x.SelectedBillItemDetailViewModel);
        }

        [Test]
        public void MovesItemUp()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));
            billItemEditViewModel.SelectedBillItemDetailViewModel = billItemEditViewModel.BillItemDetailViewModels[2];
            billItemEditViewModel.MoveItemUpCommand.RelayCommand.Execute(null);

            // Assert
            billItemEditViewModel.BillItemDetailViewModels[1].Position.Should().Be(2);
            billItemEditViewModel.BillItemDetailViewModels[1].Amount.Should().Be(3);
            billItemEditViewModel.BillItemDetailViewModels[2].Position.Should().Be(3);
            billItemEditViewModel.BillItemDetailViewModels[2].Amount.Should().Be(2);
        }

        [Test]
        public void CanNotMoveItemUpWhenEditingNotEnabled()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            billItemEditViewModel.LoadBill(bill);
            billItemEditViewModel.SelectedBillItemDetailViewModel = billItemEditViewModel.BillItemDetailViewModels[2];
            

            // Assert
            billItemEditViewModel.MoveItemUpCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotMoveItemUpWhenNoItemIsSelected()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));

            // Assert
            billItemEditViewModel.MoveItemUpCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotMoveItemUpWhenOnFirstPosition()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));
            billItemEditViewModel.SelectedBillItemDetailViewModel = billItemEditViewModel.BillItemDetailViewModels[0];

            // Assert
            billItemEditViewModel.MoveItemUpCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void MovesItemDown()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));
            billItemEditViewModel.SelectedBillItemDetailViewModel = billItemEditViewModel.BillItemDetailViewModels[2];
            billItemEditViewModel.MoveItemDownCommand.RelayCommand.Execute(null);

            // Assert
            billItemEditViewModel.BillItemDetailViewModels[2].Position.Should().Be(3);
            billItemEditViewModel.BillItemDetailViewModels[2].Amount.Should().Be(4);
            billItemEditViewModel.BillItemDetailViewModels[3].Position.Should().Be(4);
            billItemEditViewModel.BillItemDetailViewModels[3].Amount.Should().Be(3);
        }

        [Test]
        public void CanNotMoveItemDownWhenEditingNotEnabled()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            billItemEditViewModel.LoadBill(bill);
            billItemEditViewModel.SelectedBillItemDetailViewModel = billItemEditViewModel.BillItemDetailViewModels[2];


            // Assert
            billItemEditViewModel.MoveItemDownCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotMoveItemDownWhenNoItemIsSelected()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));

            // Assert
            billItemEditViewModel.MoveItemDownCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotMoveItemDownWhenOnLastPosition()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetDefaultViewModel();
            Bill bill = new Bill();

            for (int i = 0; i < 5; i++)
            {
                BillItem billItem = ModelFactory.GetDefaultBillItem();
                billItem.Position = i + 1;
                billItem.Amount = i + 1;
                bill.AddBillItem(billItem);
            }

            // Act
            billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateMessageForBillItemEditVM));
            billItemEditViewModel.SelectedBillItemDetailViewModel = billItemEditViewModel.BillItemDetailViewModels[4];

            // Assert
            billItemEditViewModel.MoveItemDownCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        #endregion



        private BillItemEditViewModel GetDefaultViewModel()
        {
            return new BillItemEditViewModel();
        }
    }
}