// ///////////////////////////////////
// File: BillItemEditViewModelTest.cs
// Last Change: 21.06.2017  15:25
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
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
            BillItemEditViewModel billItemEditViewModel = this.GetBillItemEditViewModel();
            Bill bill = ModelFactory.GetDefaultBill();

            // Act
            billItemEditViewModel.LoadBill(bill);

            // Assert
            billItemEditViewModel.BillItemDetailViewModels.Should().HaveCount(1);
            billItemEditViewModel.BillItemDetailViewModels[0].Description.Should().Be(ModelFactory.DefaultBillItemDescription);
        }

        [Test]
        public void EnableEditingOnEnableEditingMessage()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetBillItemEditViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateForBillItemEditing));

            // Assert
            billItemEditViewModel.IsEditingEnabled.Should().BeTrue();
        }

        [Test]
        public void DisableEditingOnDisableEditingMessage()
        {
            // Arrange
            BillItemEditViewModel billItemEditViewModel = this.GetBillItemEditViewModel();

            // Act
            Messenger.Default.Send(new NotificationMessage<bool>(true, Resources.Messenger_Message_EnableStateForBillItemEditing));
            Messenger.Default.Send(new NotificationMessage<bool>(false, Resources.Messenger_Message_EnableStateForBillItemEditing));

            // Assert
            billItemEditViewModel.IsEditingEnabled.Should().BeFalse();
        }

        #endregion



        private BillItemEditViewModel GetBillItemEditViewModel()
        {
            return new BillItemEditViewModel();
        }
    }
}