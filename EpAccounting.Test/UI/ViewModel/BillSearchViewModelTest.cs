// ///////////////////////////////////
// File: BillSearchViewModelTest.cs
// Last Change: 09.06.2017  20:20
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.UI.ViewModel
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Test.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.ViewModel;
    using FluentAssertions;
    using GalaSoft.MvvmLight.Messaging;
    using Moq;
    using NHibernate.Criterion;
    using NUnit.Framework;



    [TestFixture]
    public class BillSearchViewModelTest
    {
        #region Test Methods

        [Test]
        public void DerivesFromBindableViewModelBase()
        {
            // Assert
            typeof(BillSearchViewModel).Should().BeDerivedFrom<BindableViewModelBase>();
        }

        [Test]
        public void GetEmptyListAfterCreation()
        {
            // Arrange
            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel();

            // Assert
            billSearchViewModel.FoundBills.Should().HaveCount(0);
        }

        [Test]
        public void AddSingleBillToListThatMatchesCriterion()
        {
            // Arrange
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetByCriteria<Bill>(It.IsAny<ICriterion>()))
                          .Returns(new List<Bill> { ModelFactory.GetDefaultBill() });

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);

            // Act
            Messenger.Default.Send(new NotificationMessage<ICriterion>(null, Resources.Messenger_Message_BillSearchCriteria));

            // Assert
            billSearchViewModel.FoundBills.Should().HaveCount(1);
            billSearchViewModel.FoundBills[0].Date.Should().Be(ModelFactory.DefaultBillDate);
            billSearchViewModel.FoundBills[0].KindOfBill.Should().Be(ModelFactory.DefaultBillKindOfBill);
        }

        [Test]
        public void SelectedBillReturnsNullAfterCreation()
        {
            // Act
            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel();

            // Assert
            billSearchViewModel.SelectedBillDetailViewModel.Should().BeNull();
        }

        [Test]
        public void RaisePropertyChangedWhenSelectedBillChanges()
        {
            // Arrange
            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel();
            billSearchViewModel.MonitorEvents<INotifyPropertyChanged>();

            // Act
            billSearchViewModel.SelectedBillDetailViewModel = new BillDetailViewModel(new Bill());

            // Assert
            billSearchViewModel.ShouldRaisePropertyChangeFor(x => x.SelectedBillDetailViewModel);
        }

        [Test]
        public void UpdatesBillWhenUpdateBillNotificationReceived()
        {
            // Arrange
            const string ExpectedDate = "01.01.2015";
            Bill expectedBill = new Bill { BillId = 1, Date = ExpectedDate, KindOfVat = ModelFactory.DefaultBillKindOfVat };
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(expectedBill);

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);
            billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { BillId = 1, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat }));
            billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { BillId = 2, Date = ModelFactory.DefaultBillDate, KindOfVat = ModelFactory.DefaultBillKindOfVat }));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Messenger_Message_UpdateBillValues));

            // Assert
            billSearchViewModel.FoundBills.Should().HaveCount(2);
            billSearchViewModel.FoundBills[0].Date.Should().Be(ExpectedDate);
            billSearchViewModel.FoundBills[1].Date.Should().Be(ModelFactory.DefaultBillDate);
        }

        [Test]
        public void UpdatesBillWhenUpdateClientNotificationReceived()
        {
            // Arrange
            const string ExpectedFirstName = "Matthias";
            Bill expectedBill = ModelFactory.GetDefaultBill();
            expectedBill.Client.FirstName = ExpectedFirstName;
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(expectedBill);

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);
            billSearchViewModel.FoundBills.Add(new BillDetailViewModel(new Bill { BillId = 1, Date = "01.01.2017", KindOfVat = "Gutschein", Client = new Client()}));
            billSearchViewModel.FoundBills.Add(new BillDetailViewModel( ModelFactory.GetDefaultBill()));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(0, Resources.Messenger_Message_UpdateClientValues));

            // Assert
            billSearchViewModel.FoundBills.Should().HaveCount(2);
            billSearchViewModel.FoundBills[1].FirstName.Should().Be(ExpectedFirstName);
        }

        [Test]
        public void RemovesBillWhenUpdateNotificationReceived()
        {
            // Arrange
            const int Id = 1;
            const string Date = "01.01.2015";
            const string KindOfBill = "Gutschrift";

            Bill expectedBill = new Bill { BillId = Id, Date = Date, KindOfBill = KindOfBill };
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetById<Bill>(It.IsAny<int>())).Returns(expectedBill);

            BillSearchViewModel billSearchViewModel = this.GetDefaultViewModel(mockRepository);
            billSearchViewModel.FoundBills.Add(new BillDetailViewModel(expectedBill));

            // Act
            Messenger.Default.Send(new NotificationMessage<int>(1, Resources.Messenger_Message_RemoveBill));

            // Assert
            billSearchViewModel.FoundBills.Should().HaveCount(0);
        }

        #endregion



        private BillSearchViewModel GetDefaultViewModel()
        {
            Mock<IRepository> mockRepository = new Mock<IRepository>();
            return new BillSearchViewModel(mockRepository.Object);
        }

        private BillSearchViewModel GetDefaultViewModel(Mock<IRepository> repository)
        {
            return new BillSearchViewModel(repository.Object);
        }
    }
}