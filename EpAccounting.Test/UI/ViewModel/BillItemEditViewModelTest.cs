// ///////////////////////////////////
// File: BillItemEditViewModelTest.cs
// Last Change: 18.09.2017  20:57
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.ComponentModel;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class BillItemEditViewModelTest
    {
        #region Fields

        private Mock<IRepository> mockRepository;
        private BillItemEditViewModel billItemEditViewModel;

        #endregion



        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.billItemEditViewModel = new BillItemEditViewModel(this.mockRepository.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
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
            this.billItemEditViewModel.SelectedBillItemDetailViewModel = new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem(), this.mockRepository.Object);

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

        [Test]
        public void CalculateSumWhenInclusiveVAT()
        {
            // Arrange
            this.billItemEditViewModel.BillItemDetailViewModels.Add(new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem(), this.mockRepository.Object));

            // Act
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Assert
            this.billItemEditViewModel.NettoSum.Should().BeApproximately(55.56m, 0.05m);
            this.billItemEditViewModel.VatSum.Should().BeApproximately(10.56m, 0.05m);
            this.billItemEditViewModel.BruttoSum.Should().BeApproximately(66.11m, 0.05m);
        }

        [Test]
        public void CalculateSumWhenPlusVAT()
        {
            // Arrange
            this.billItemEditViewModel.BillItemDetailViewModels.Add(new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem(), this.mockRepository.Object));

            // Act
            Bill bill = ModelFactory.GetDefaultBill();
            bill.KindOfVat = KindOfVat.zzgl_MwSt;
            this.billItemEditViewModel.LoadBill(bill);

            // Assert
            this.billItemEditViewModel.NettoSum.Should().BeApproximately(66.11m, 0.05m);
            this.billItemEditViewModel.VatSum.Should().BeApproximately(12.56m, 0.05m);
            this.billItemEditViewModel.BruttoSum.Should().BeApproximately(78.67m, 0.05m);
        }

        [Test]
        public void CalculateSumWhenNoVAT()
        {
            // Arrange
            this.billItemEditViewModel.BillItemDetailViewModels.Add(new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem(), this.mockRepository.Object));

            // Act
            Bill bill = ModelFactory.GetDefaultBill();
            bill.KindOfVat = KindOfVat.without_MwSt;
            this.billItemEditViewModel.LoadBill(bill);

            // Assert
            this.billItemEditViewModel.NettoSum.Should().BeApproximately(66.11m, 0.05m);
            this.billItemEditViewModel.VatSum.Should().Be(0);
            this.billItemEditViewModel.BruttoSum.Should().BeApproximately(66.11m, 0.05m);
        }

        [Test]
        public void ReclaculateNettoVatAndSumWhenUpdateMessageReceived()
        {
            // Arrange
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            this.billItemEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_UpdateSumsForBillItemEditVM));

            // Assert
            this.billItemEditViewModel.ShouldRaisePropertyChangeFor(x => x.NettoSum);
            this.billItemEditViewModel.ShouldRaisePropertyChangeFor(x => x.VatSum);
            this.billItemEditViewModel.ShouldRaisePropertyChangeFor(x => x.BruttoSum);
        }

        [Test]
        public void CanNotChangeVatAfterInitialization()
        {
            // Arrange
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Assert
            this.billItemEditViewModel.CanChangeVAT.Should().BeFalse();
        }

        [Test]
        public void CanChangeVat()
        {
            // Arrange
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            this.billItemEditViewModel.ChangeVATCommand.Execute(null);

            // Assert
            this.billItemEditViewModel.CanChangeVAT.Should().BeTrue();
        }

        [Test]
        public void ChangesVat()
        {
            // Arrange
            double vatPercentage = Settings.Default.VAT;
            this.billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            this.billItemEditViewModel.ChangeVATCommand.Execute(null);
            Settings.Default.VAT = 50;
            this.billItemEditViewModel.ChangeVATCommand.Execute(null);

            // Assert
            Settings.Default.VAT = vatPercentage;
            Settings.Default.Save();
            this.billItemEditViewModel.NettoSum.Should().BeApproximately(44.07M, 0.005M);
            this.billItemEditViewModel.VatSum.Should().BeApproximately(22.04M, 0.005M);
            this.billItemEditViewModel.BruttoSum.Should().BeApproximately(66.11M, 0.005M);
        }

        #endregion
    }
}