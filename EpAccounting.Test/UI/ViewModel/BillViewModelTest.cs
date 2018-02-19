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
        private Mock<IRepository> _mockRepository;
        private Mock<IDialogService> _mockDialogService;
        private BillViewModel _billViewModel;


        [SetUp]
        public void Init()
        {
            this._mockRepository = new Mock<IRepository>();
            this._mockDialogService = new Mock<IDialogService>();
            this._billViewModel = new BillViewModel(Resources.Workspace_Title_Bills, Resources.img_bills, this._mockRepository.Object, this._mockDialogService.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            this._mockRepository = null;
            this._mockDialogService = null;
            this._billViewModel = null;
        }


        [Test]
        public void BillEditViewModelInitializedAfterCreation()
        {
            // Assert
            this._billViewModel.BillEditViewModel.Should().NotBeNull();
        }

        [Test]
        public void BillWorkspaceViewModelInitializedAfterCreation()
        {
            // Assert
            this._billViewModel.BillWorkspaceViewModel.Should().NotBeNull();
        }

        [Test]
        public void ChangeWorkspaceToBillSearchViewModelOnNotificationMessageThatBillSearchViewModelShouldBeLoaded()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_LoadBillSearchViewModelMessageForBillVM));

            // Assert
            this._billViewModel.BillWorkspaceViewModel.Should().BeOfType<BillSearchViewModel>();
        }

        [Test]
        public void ChangeWorkspaceToBillItemEditViewModelOnNotificationMessageThatBillItemEditViewModelShouldBeLoaded()
        {
            // Act
            Messenger.Default.Send(new NotificationMessage<Bill>(ModelFactory.GetDefaultBill(), Resources.Message_LoadBillItemEditViewModelForBillVM));

            // Assert
            this._billViewModel.BillWorkspaceViewModel.Should().BeOfType<BillItemEditViewModel>();
        }

        [Test]
        public void ChangeWorkspaceToBillSearchViewModelOnNotificationMessageThatBillWasDeleted()
        {
            // Arrange
            this._mockRepository.Setup(x => x.GetByCriteria(It.IsAny<ICriterion>(), It.IsAny<Expression<Func<Bill, Client>>>(), It.IsAny<ICriterion>(), 1))
                .Returns(new List<Bill>());

            // Act
            Messenger.Default.Send(new NotificationMessage(Resources.Message_ResetBillItemEditVMAndChangeToSearchWorkspaceForBillVM));

            // Assert
            this._billViewModel.BillWorkspaceViewModel.Should().BeOfType<BillSearchViewModel>();
        }
    }
}