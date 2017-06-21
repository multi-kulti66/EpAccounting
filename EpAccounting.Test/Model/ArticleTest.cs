// ///////////////////////////////////
// File: ArticleTest.cs
// Last Change: 13.03.2017  20:45
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Test.Model
{
    using System;
    using EpAccounting.Model;
    using FluentAssertions;
    using NUnit.Framework;



    [TestFixture]
    internal class ArticleTest
    {
        [Test]
        public void ArticlesEqualIfBothArticlesHaveSameValues()
        {
            // Arrange
            Article article1 = ModelFactory.GetDefaultArticle();
            Article article2 = ModelFactory.GetDefaultArticle();

            // Act
            bool isEqual = article1.Equals(article2);

            // Assert
            isEqual.Should().BeTrue();
        }

        [Test]
        public void ArticlesUnequalIfArticleValuesDiffer()
        {
            // Arrange
            Article article1 = ModelFactory.GetDefaultArticle();
            Article article2 = ModelFactory.GetDefaultArticle();
            article2.Description = "test description";

            // Act
            bool isEqual = article1.Equals(article2);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Test]
        public void ArticlesUnequalIfOtherArticleIsNull()
        {
            // Arrange
            Article article = ModelFactory.GetDefaultArticle();

            // Act
            bool isEqual = article.Equals(null);

            // Assert
            isEqual.Should().BeFalse();
        }

        [Test]
        public void GetHashCodeIfInitialized()
        {
            // Arrange
            Article article = ModelFactory.GetDefaultArticle();

            // Act
            Func<int> func = () => article.GetHashCode();

            // Assert
            func.Invoke().Should().NotBe(0);
        }
    }
}