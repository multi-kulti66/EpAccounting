// ///////////////////////////////////
// File: BillViewModelTest.cs
// Last Change: 02.06.2017  21:02
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Test.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NUnit.Framework;



    [TestFixture]
    public class BillViewModelTest
    {
        [Test]
        public void AllSubViewModelsInitializedAfterCreation()
        {
            // Act
            BillViewModel billViewModel = this.GetBillViewModel();

            // Assert
            billViewModel.BillEditViewModel.Should().NotBeNull();
            billViewModel.BillWorkspaceViewModel.Should().NotBeNull();
        }

        [Test]
        public void ChangeWorkspaceToBillSearchViewModelOnNotificationMessage()
        {
            // Arrange
            BillViewModel billViewModel = this.GetBillViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Messenger_Message_LoadBillSearchViewModel));

            // Assert
            billViewModel.BillWorkspaceViewModel.Should().BeOfType<BillSearchViewModel>();
        }

        [Test]
        public void ChangeWorkspaceToBillItemEditViewModelOnNotificationMessage()
        {
            // Arrange
            BillViewModel billViewModel = this.GetBillViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage<Bill>(ModelFactory.GetDefaultBill(), Resources.Messenger_Message_LoadBillItemEditViewModel));

            // Assert
            billViewModel.BillWorkspaceViewModel.Should().BeOfType<BillItemEditViewModel>();
        }

        private BillViewModel GetBillViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            Mock<IDialogService> mockDialogService = new Mock<IDialogService>();

            return new BillViewModel("Rechnungen", Resources.img_bills, mockRepository.Object, mockDialogService.Object);
        }
    }
}