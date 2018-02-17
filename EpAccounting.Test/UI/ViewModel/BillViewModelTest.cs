// ///////////////////////////////////
// File: BillViewModelTest.cs
// Last Change: 23.08.2017  19:51
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NHibernate.Criterion;
    using NUnit.Framework;



    [TestFixture]
    public class BillViewModelTest
    {
        private Mock<IRepository> mockRepository;
        private Mock<IDialogService> mockDialogService;
        private BillViewModel billViewModel;


        [SetUp]
        public void Init()
        {
            this.mockRepository = new Mock<IRepository>();
            this.mockDialogService = new Mock<IDialogService>();
            this.billViewModel = new BillViewModel(Resources.Workspace_Title_Bills, Resources.img_bills, this.mockRepository.Object, this.mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this.mockRepository = null;
            this.mockDialogService = null;
            this.billViewModel = null;
        }


        [Test]
        public void BillEditViewModelInitializedAfterCreation()
        {
            // Assert
            this.billViewModel.BillEditViewModel.Should().NotBeNull();
        }

        [Test]
        public void BillWorkspaceViewModelInitializedAfterCreation()
        {
            // Assert
            this.billViewModel.BillWorkspaceViewModel.Should().NotBeNull();
        }

        [Test]
        public void ChangeWorkspaceToBillSearchViewModelOnNotificationMessageThatBillSearchViewModelShouldBeLoaded()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_LoadBillSearchViewModelMessageForBillVM));

            // Assert
            this.billViewModel.BillWorkspaceViewModel.Should().BeOfType<BillSearchViewModel>();
        }

        [Test]
        public void ChangeWorkspaceToBillItemEditViewModelOnNotificationMessageThatBillItemEditViewModelShouldBeLoaded()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage<Bill>(ModelFactory.GetDefaultBill(), Resources.Message_LoadBillItemEditViewModelForBillVM));

            // Assert
            this.billViewModel.BillWorkspaceViewModel.Should().BeOfType<BillItemEditViewModel>();
        }

        [Test]
        public void ChangeWorkspaceToBillSearchViewModelOnNotificationMessageThatBillWasDeleted()
        {
            // Arrange
            this.mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill>());

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ResetBillItemEditVMAndChangeToSearchWorkspaceForBillVM));

            // Assert
            this.billViewModel.BillWorkspaceViewModel.Should().BeOfType<BillSearchViewModel>();
        }
    }
}