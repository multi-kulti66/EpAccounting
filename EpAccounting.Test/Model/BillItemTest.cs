// ///////////////////////////////////
// File: BillItemTest.cs
// Last Change: 16.03.2017  22:31
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.Model
{
    using System;
    using EpAccounting.Model;
    using FluentAssertions;
    using NUnit.Framework;



    [TestFixture]
    public class BillItemTest
    {
        [Test]
        public void BillItemsEqualIfBothBillItemsHaveSameValues()
        {
            // Arrange
            BillItem billDetail1 = ModelFactory.GetDefaultBillItem();
            BillItem billDetail2 = ModelFactory.GetDefaultBillItem();

            // Act
            bool isEqual = billDetail1.Equals(billDetail2);

            // Assert
            isEqual.Should().BeTrue();
        }

        [Test]
        public void BillItemsUnequalIfBillItemValuesDiffer()
        {
            // Arrange
            BillItem billDetail1 = ModelFactory.GetDefaultBillItem();
            BillItem billDetail2 = ModelFactory.GetDefaultBillItem();
            billDetail2.Discount = 3d;

            // Act
            bool isEqual = billDetail1.Equals(billDetail2);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Test]
        public void BillItemsUnequalIfOtherBillItemIsNull()
        {
            // Arrange
            BillItem billItem = ModelFactory.GetDefaultBillItem();

            // Act
            bool isEqual = billItem.Equals(null);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Test]
        public void GetClonedBillItem()
        {
            // Arrange
            BillItem billItem = ModelFactory.GetDefaultBillItem();

            // Act
            BillItem copiedBillItem = (BillItem)billItem.Clone();

            // Assert
            billItem.Equals(copiedBillItem).Should().BeTrue();
        }

        [Test]
        public void GetHashCodeIfInitialized()
        {
            // Arrange
            BillItem billItem = ModelFactory.GetDefaultBillItem();

            // Act
            Func<int> func = () => billItem.GetHashCode();

            // Assert
            func.Invoke().Should().NotBe(0);
        }
    }
}