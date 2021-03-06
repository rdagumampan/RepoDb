﻿using NUnit.Framework;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateDeleteTest
    {
        private class TestWithoutMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateDelete(queryBuilder, null);
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestWithoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithoutMappingsAndWithExpressionsClass : DataEntity
        {
        }

        [Test]
        public void TestWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithoutMappingsAndWithExpressionsClass>();
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateDelete(queryBuilder, queryGroup);
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestWithoutMappingsAndWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestWithMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithMappingsClass>();
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateDelete(queryBuilder, queryGroup);
            var expected = $"" +
                $"DELETE " +
                $"FROM [ClassName] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
