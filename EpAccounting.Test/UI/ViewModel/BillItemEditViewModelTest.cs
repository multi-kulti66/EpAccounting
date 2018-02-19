// ///////////////////////////////////
// File: BillItemEditViewModelTest.cs
// Last Change: 26.10.2017  22:05
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class BillItemEditViewModelTest
    {
        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private Mock<IWordService> _mockWordService;
        private BillItemEditViewModel _billItemEditViewModel;


        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._mockWordService = new Mock<IWordService>();
            this._billItemEditViewModel = new BillItemEditViewModel(this._mockRepository.Object, this._mockDialogService.Object, this._mockWordService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._mockDialogService = null;
            this._mockWordService = null;
            this._billItemEditViewModel = null;
            GC.Collect();
        }


        [Test]
        public void DerivesFromBillWorkspaceViewModel()
        {
            typeof(BillItemEditViewModel).Should().BeDerivedFrom<BillWorkspaceViewModel>();
        }

        [Test]
        public void InitializedAllCommands()
        {
            // Assert
            this._billItemEditViewModel.Commands.Count.Should().Be(4);
            this._billItemEditViewModel.WordCommands.Count.Should().Be(2);
        }

        [Test]
        public void LoadBillItemDetailViewModelsViaPassedBill()
        {
            // Arrange
            Bill bill = ModelFactory.GetDefaultBill();

            // Act
            this._billItemEditViewModel.LoadBill(bill);

            // Assert
            this._billItemEditViewModel.BillItemDetailViewModels.Should().HaveCount(1);
            this._billItemEditViewModel.BillItemDetailViewModels[0].Description.Should().Be(ModelFactory.DefaultBillItemDescription);
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
            this._billItemEditViewModel.LoadBill(bill);

            // Assert
            this._billItemEditViewModel.BillItemDetailViewModels.Should().HaveCount(2);
            this._billItemEditViewModel.BillItemDetailViewModels[0].Position.Should().Be(1);
            this._billItemEditViewModel.BillItemDetailViewModels[1].Position.Should().Be(2);
        }

        [Test]
        public void EnableEditingOnEnableEditingMessage()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));

            // Assert
            this._billItemEditViewModel.IsEditingEnabled.Should().BeTrue();
        }

        [Test]
        public void DisableEditingOnDisableEditingMessage()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            Messenger.Default.Send(new NotificationMessage<bool>(false, Resources.Message_EnableStateForBillItemEditVM));

            // Assert
            this._billItemEditViewModel.IsEditingEnabled.Should().BeFalse();
        }

        [Test]
        public void ClearsCurrentlyLoadedBillAndBillItems()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            this._billItemEditViewModel.Clear();

            // Assert
            this._billItemEditViewModel.BillItemDetailViewModels.Should().HaveCount(0);
        }

        [Test]
        public void AddsNewBillItem()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));

            // Act
            this._billItemEditViewModel.AddItemCommand.RelayCommand.Execute(null);

            // Assert
            this._billItemEditViewModel.BillItemDetailViewModels.Count.Should().Be(2);
        }

        [Test]
        public void CanNotAddNewBillItemWhenEditingNotEnabled()
        {
            // Act
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Assert
            this._billItemEditViewModel.AddItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void DeletesBillItem()
        {
            // Act
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            this._billItemEditViewModel.SelectedBillItemDetailViewModel = this._billItemEditViewModel.BillItemDetailViewModels[0];
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            this._billItemEditViewModel.DeleteItemCommand.RelayCommand.Execute(null);

            // Assert
            this._billItemEditViewModel.BillItemDetailViewModels.Count.Should().Be(0);
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
            this._billItemEditViewModel.LoadBill(bill);
            this._billItemEditViewModel.SelectedBillItemDetailViewModel = this._billItemEditViewModel.BillItemDetailViewModels[2];
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            this._billItemEditViewModel.DeleteItemCommand.RelayCommand.Execute(null);

            // Assert
            for (int i = 0; i < this._billItemEditViewModel.BillItemDetailViewModels.Count; i++)
            {
                int currentPosition = i + 1;
                this._billItemEditViewModel.BillItemDetailViewModels[i].Position.Should().Be(currentPosition);
            }
        }

        [Test]
        public void CanNotDeleteBillItemWhenEditingNotEnabled()
        {
            // Act
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Assert
            this._billItemEditViewModel.DeleteItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void CanNotDeleteBillItemWhenNoItemIsSelected()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));

            // Assert
            this._billItemEditViewModel.DeleteItemCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void RaisesPropertyChangedWhenSelectedBillItemChanges()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            this._billItemEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._billItemEditViewModel.SelectedBillItemDetailViewModel = new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem(), this._mockRepository.Object);

            // Assert
            this._billItemEditViewModel.SelectedBillItemDetailViewModel.Should().NotBeNull();
            this._billItemEditViewModel.ShouldRaisePropertyChangeFor(x => x.SelectedBillItemDetailViewModel);
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
            this._billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            this._billItemEditViewModel.SelectedBillItemDetailViewModel = this._billItemEditViewModel.BillItemDetailViewModels[2];
            this._billItemEditViewModel.MoveItemUpCommand.RelayCommand.Execute(null);

            // Assert
            this._billItemEditViewModel.BillItemDetailViewModels[1].Position.Should().Be(2);
            this._billItemEditViewModel.BillItemDetailViewModels[1].Amount.Should().Be(3);
            this._billItemEditViewModel.BillItemDetailViewModels[2].Position.Should().Be(3);
            this._billItemEditViewModel.BillItemDetailViewModels[2].Amount.Should().Be(2);
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
            this._billItemEditViewModel.LoadBill(bill);
            this._billItemEditViewModel.SelectedBillItemDetailViewModel = this._billItemEditViewModel.BillItemDetailViewModels[2];

            // Assert
            this._billItemEditViewModel.MoveItemUpCommand.RelayCommand.CanExecute(null).Should().BeFalse();
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
            this._billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));

            // Assert
            this._billItemEditViewModel.MoveItemUpCommand.RelayCommand.CanExecute(null).Should().BeFalse();
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
            this._billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            this._billItemEditViewModel.SelectedBillItemDetailViewModel = this._billItemEditViewModel.BillItemDetailViewModels[0];

            // Assert
            this._billItemEditViewModel.MoveItemUpCommand.RelayCommand.CanExecute(null).Should().BeFalse();
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
            this._billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            this._billItemEditViewModel.SelectedBillItemDetailViewModel = this._billItemEditViewModel.BillItemDetailViewModels[2];
            this._billItemEditViewModel.MoveItemDownCommand.RelayCommand.Execute(null);

            // Assert
            this._billItemEditViewModel.BillItemDetailViewModels[2].Position.Should().Be(3);
            this._billItemEditViewModel.BillItemDetailViewModels[2].Amount.Should().Be(4);
            this._billItemEditViewModel.BillItemDetailViewModels[3].Position.Should().Be(4);
            this._billItemEditViewModel.BillItemDetailViewModels[3].Amount.Should().Be(3);
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
            this._billItemEditViewModel.LoadBill(bill);
            this._billItemEditViewModel.SelectedBillItemDetailViewModel = this._billItemEditViewModel.BillItemDetailViewModels[2];

            // Assert
            this._billItemEditViewModel.MoveItemDownCommand.RelayCommand.CanExecute(null).Should().BeFalse();
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
            this._billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));

            // Assert
            this._billItemEditViewModel.MoveItemDownCommand.RelayCommand.CanExecute(null).Should().BeFalse();
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
            this._billItemEditViewModel.LoadBill(bill);
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));
            this._billItemEditViewModel.SelectedBillItemDetailViewModel = this._billItemEditViewModel.BillItemDetailViewModels[4];

            // Assert
            this._billItemEditViewModel.MoveItemDownCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void ClearBillWhenContainingClientWasDeleted()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Message_RemoveClientForBillItemEditVM));

            // Assert
            this._billItemEditViewModel.BillItemDetailViewModels.Count.Should().Be(0);
        }

        [Test]
        public void CalculateSumWhenInclusiveVat()
        {
            // Arrange
            this._billItemEditViewModel.BillItemDetailViewModels.Add(new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem(), this._mockRepository.Object));

            // Act
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Assert
            this._billItemEditViewModel.NettoSum.Should().BeApproximately(55.56m, 0.05m);
            this._billItemEditViewModel.VatSum.Should().BeApproximately(10.56m, 0.05m);
            this._billItemEditViewModel.BruttoSum.Should().BeApproximately(66.11m, 0.05m);
        }

        [Test]
        public void CalculateSumWhenPlusVat()
        {
            // Arrange
            this._billItemEditViewModel.BillItemDetailViewModels.Add(new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem(), this._mockRepository.Object));

            // Act
            Bill bill = ModelFactory.GetDefaultBill();
            bill.KindOfVat = KindOfVat.ZzglMwSt;
            this._billItemEditViewModel.LoadBill(bill);

            // Assert
            this._billItemEditViewModel.NettoSum.Should().BeApproximately(66.11m, 0.05m);
            this._billItemEditViewModel.VatSum.Should().BeApproximately(12.56m, 0.05m);
            this._billItemEditViewModel.BruttoSum.Should().BeApproximately(78.67m, 0.05m);
        }

        [Test]
        public void CalculateSumWhenNoVat()
        {
            // Arrange
            this._billItemEditViewModel.BillItemDetailViewModels.Add(new BillItemDetailViewModel(ModelFactory.GetDefaultBillItem(), this._mockRepository.Object));

            // Act
            Bill bill = ModelFactory.GetDefaultBill();
            bill.KindOfVat = KindOfVat.WithoutMwSt;
            this._billItemEditViewModel.LoadBill(bill);

            // Assert
            this._billItemEditViewModel.NettoSum.Should().BeApproximately(66.11m, 0.05m);
            this._billItemEditViewModel.VatSum.Should().Be(0);
            this._billItemEditViewModel.BruttoSum.Should().BeApproximately(66.11m, 0.05m);
        }

        [Test]
        public void ReclaculateNettoVatAndSumWhenUpdateMessageReceivedForInklVat()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            this._billItemEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_OnVatChangeRecalculatePricesForBillItemEditVM));

            // Assert
            this._billItemEditViewModel.ShouldRaisePropertyChangeFor(x => x.NettoSum);
            this._billItemEditViewModel.ShouldRaisePropertyChangeFor(x => x.VatSum);
            this._billItemEditViewModel.ShouldRaisePropertyChangeFor(x => x.BruttoSum);
            this._billItemEditViewModel.BillItemDetailViewModels[0].Price.Should().Be(ModelFactory.DefaultBillItemPrice * (100 + (decimal)ModelFactory.DefaultBillVatPercentage) / 100);
        }

        [Test]
        public void ReclaculateNettoVatAndSumWhenUpdateMessageReceivedForZzglVat()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            this._billItemEditViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            this._billItemEditViewModel.CurrentBillDetailViewModel.KindOfVat = KindOfVat.ZzglMwSt;

            // Assert
            this._billItemEditViewModel.ShouldRaisePropertyChangeFor(x => x.NettoSum);
            this._billItemEditViewModel.ShouldRaisePropertyChangeFor(x => x.VatSum);
            this._billItemEditViewModel.ShouldRaisePropertyChangeFor(x => x.BruttoSum);
            this._billItemEditViewModel.BillItemDetailViewModels[0].Price.Should().Be(ModelFactory.DefaultBillItemPrice * 100 / (100 + (decimal)ModelFactory.DefaultBillVatPercentage));
        }

        [Test]
        public void CanNotChangeVatAfterInitialization()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Assert
            this._billItemEditViewModel.CanChangeVat.Should().BeFalse();
        }

        [Test]
        public void CanChangeVat()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            this._billItemEditViewModel.ChangeVatCommand.Execute(null);

            // Assert
            this._billItemEditViewModel.CanChangeVat.Should().BeTrue();
        }

        [Test]
        public void ChangesVat()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            this._billItemEditViewModel.ChangeVatCommand.Execute(null);
            this._billItemEditViewModel.CurrentBillDetailViewModel.VatPercentage = 50;
            this._billItemEditViewModel.ChangeVatCommand.Execute(null);

            // Assert
            this._billItemEditViewModel.NettoSum.Should().BeApproximately(44.07M, 0.005M);
            this._billItemEditViewModel.VatSum.Should().BeApproximately(22.04M, 0.005M);
            this._billItemEditViewModel.BruttoSum.Should().BeApproximately(66.11M, 0.005M);
        }

        [Test]
        public void DoesNotCalculateSumsWhenNoBillWasLoaded()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_OnVatChangeRecalculatePricesForBillItemEditVM));

            // Assert
            this._billItemEditViewModel.CurrentBillDetailViewModel.Should().BeNull();
        }

        [Test]
        public void CanCreateBillWhenInLoadedMode()
        {
            // Act
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Assert
            this._billItemEditViewModel.CreateDocumentCommand.RelayCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public void CanNotCreateBillWhenInEditMode()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Message_EnableStateForBillItemEditVM));

            // Assert
            this._billItemEditViewModel.CreateDocumentCommand.RelayCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void IsCreatingBillShouldBeFalse()
        {
            // Assert
            this._billItemEditViewModel.IsCreatingBill.Should().BeFalse();
        }

        [Test]
        public void IsCreatingBillShouldBeTrue()
        {
            // Arrange
            this._mockWordService.Setup(x => x.CreateWordBill(It.IsAny<BillItemEditViewModel>(), true)).Callback(() => Thread.Sleep(100));
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            this._billItemEditViewModel.CreateDocumentCommand.RelayCommand.Execute(null);

            // Assert
            this._billItemEditViewModel.IsCreatingBill.Should().BeTrue();
            Thread.Sleep(200);
        }

        [Test]
        public void CreatesWordDocument()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            this._billItemEditViewModel.CreateDocumentCommand.RelayCommand.Execute(null);
            Thread.Sleep(100);

            // Assert
            this._mockWordService.Verify(x => x.CreateWordBill(It.IsAny<BillItemEditViewModel>(), true), Times.Once);
        }

        [Test]
        public void ShowMessageWhenWordDocumentWasCreated()
        {
            // Arrange
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            this._billItemEditViewModel.CreateDocumentCommand.RelayCommand.Execute(null);
            Thread.Sleep(100);

            // Assert
            this._mockDialogService.Verify(x => x.ShowMessage(Resources.Dialog_Title_Bill_Created, Resources.Dialog_Message_Bill_Created), Times.Once);
        }

        [Test]
        public void ShowMessageWhenWordDocumentCouldNotBeCreated()
        {
            // Arrange
            this._mockWordService.Setup(x => x.CreateWordBill(It.IsAny<BillItemEditViewModel>(), true)).Throws<Exception>();
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());

            // Act
            this._billItemEditViewModel.CreateDocumentCommand.RelayCommand.Execute(null);
            Thread.Sleep(100);

            // Assert
            this._mockDialogService.Verify(x => x.ShowMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PrintsDocument()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(new Bill() { Printed = true });
            this._mockWordService.Setup(x => x.PrintDocument()).Returns(true);
            this._mockWordService.Setup(x => x.CreateWordBill(this._billItemEditViewModel, false));
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            this._billItemEditViewModel.CurrentBillDetailViewModel.Printed = null;

            // Act
            this._billItemEditViewModel.PrintDocumentCommand.RelayCommand.Execute(null);
            Thread.Sleep(100);

            // Assert
            this._billItemEditViewModel.CurrentBillDetailViewModel.Printed.Should().BeTrue();
            this._mockWordService.Verify(x => x.PrintDocument(), Times.Once);
            this._mockWordService.Verify(x => x.CloseDocument(), Times.Once);
            this._mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Bill>()), Times.Once);
        }

        [Test]
        public void ShowMessageWhenBillCouldNotBeUpdatedAfterPrinting()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(new Bill() { Printed = true });
            this._mockRepository.Setup(x => x.SaveOrUpdate(It.IsAny<Bill>())).Throws<Exception>();
            this._mockWordService.Setup(x => x.PrintDocument()).Returns(true);
            this._mockWordService.Setup(x => x.CreateWordBill(this._billItemEditViewModel, false));
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            this._billItemEditViewModel.CurrentBillDetailViewModel.Printed = null;

            // Act
            this._billItemEditViewModel.PrintDocumentCommand.RelayCommand.Execute(null);
            Thread.Sleep(100);

            // Assert
            this._billItemEditViewModel.CurrentBillDetailViewModel.Printed.Should().BeTrue();
            this._mockWordService.Verify(x => x.PrintDocument(), Times.Once);
            this._mockWordService.Verify(x => x.CloseDocument(), Times.Once);
            this._mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Bill>()), Times.Once);
            this._mockDialogService.Verify(x => x.ShowExceptionMessage(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PrintDocumentWasCancelled()
        {
            // Arrange
            this._mockWordService.Setup(x => x.PrintDocument()).Returns(false);
            this._mockWordService.Setup(x => x.CreateWordBill(this._billItemEditViewModel, false));
            this._billItemEditViewModel.LoadBill(ModelFactory.GetDefaultBill());
            this._billItemEditViewModel.CurrentBillDetailViewModel.Printed = null;

            // Act
            this._billItemEditViewModel.PrintDocumentCommand.RelayCommand.Execute(null);
            Thread.Sleep(100);

            // Assert
            this._billItemEditViewModel.CurrentBillDetailViewModel.Printed.Should().BeNull();
            this._mockWordService.Verify(x => x.PrintDocument(), Times.Once);
            this._mockWordService.Verify(x => x.CloseDocument(), Times.Once);
            this._mockRepository.Verify(x => x.SaveOrUpdate(It.IsAny<Bill>()), Times.Never);
        }
    }
}