// ///////////////////////////////////
// File: BillTest.cs
// Last Change: 13.03.2017  20:44
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.Model
{
    using System;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
    using FluentAssertions;
    using NUnit.Framework;



    [TestFixture]
    public class BillTest
    {
        [Test]
        public void CanAddBillItemToBill()
        {
            // Arrange
            Bill bill = new Bill();
            BillItem billItem = new BillItem();

            // Act
            bill.AddBillItem(billItem);

            // Assert
            bill.BillItems.Should().NotBeEmpty();
        }

        [Test]
        public void ThrowExceptionWhenNullBillItemShouldBeAddedToBill()
        {
            // Arrange
            Bill bill = new Bill();

            // Act
            Action action = () => bill.AddBillItem(null);

            // Assert
            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void BillsEqualIfBothBillsHaveSameValues()
        {
            // Arrange
            Bill bill1 = ModelFactory.GetDefaultBill();
            Bill bill2 = ModelFactory.GetDefaultBill();

            // Act
            bool isEqual = bill1.Equals(bill2);

            // Assert
            isEqual.Should().BeTrue();
        }

        [Test]
        public void BillsEqualIfBothBillsHaveSameValuesAndSameBillItems()
        {
            // Arrange
            Bill bill1 = ModelFactory.GetDefaultBill();
            Bill bill2 = ModelFactory.GetDefaultBill();

            BillItem billDetail1 = ModelFactory.GetDefaultBillItem();
            BillItem billDetail2 = ModelFactory.GetDefaultBillItem();

            bill1.AddBillItem(billDetail1);
            bill2.AddBillItem(billDetail2);

            // Act
            bool isEqual = bill1.Equals(bill2);

            // Assert
            isEqual.Should().BeTrue();
        }

        [Test]
        public void BillsUnequalIfBillValuesDiffer()
        {
            // Arrange
            Bill bill1 = ModelFactory.GetDefaultBill();
            Bill bill2 = ModelFactory.GetDefaultBill();
            bill2.KindOfBill = KindOfBill.Gutschrift;

            // Act
            bool isEqual = bill1.Equals(bill2);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Test]
        public void BillsUnequalIfBillItemsHaveDifferentQuantity()
        {
            // Arrange
            Bill bill1 = ModelFactory.GetDefaultBill();
            Bill bill2 = ModelFactory.GetDefaultBill();
            BillItem billItem = ModelFactory.GetDefaultBillItem();

            bill1.AddBillItem(billItem);

            // Act
            bool isEqual = bill1.Equals(bill2);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Test]
        public void BillsUnequalIfBillItemsDiffer()
        {
            // Arrange
            Bill bill1 = ModelFactory.GetDefaultBill();
            Bill bill2 = ModelFactory.GetDefaultBill();

            BillItem billDetail1 = ModelFactory.GetDefaultBillItem();
            BillItem billDetail2 = ModelFactory.GetDefaultBillItem();
            billDetail2.ArticleNumber = 5;

            bill1.AddBillItem(billDetail1);
            bill2.AddBillItem(billDetail2);

            // Act
            bool isEqual = bill1.Equals(bill2);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Test]
        public void BillsUnequalIfOtherBillIsNull()
        {
            // Arrange
            Bill bill = ModelFactory.GetDefaultBill();

            // Act
            bool isEqual = bill.Equals(null);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Test]
        public void GetClonedBill()
        {
            // Arrange
            Bill bill = ModelFactory.GetDefaultBill();
            BillItem billItem = ModelFactory.GetDefaultBillItem();
            bill.AddBillItem(billItem);

            // Act
            Bill copiedBill = (Bill)bill.Clone();

            // Assert
            bill.Equals(copiedBill).Should().BeTrue();
        }

        [Test]
        public void GetHashCodeIfInitialized()
        {
            // Arrange
            Bill bill = ModelFactory.GetDefaultBill();

            // Act
            Func<int> func = () => bill.GetHashCode();

            // Assert
            func.Invoke().Should().NotBe(0);
        }
    }
}