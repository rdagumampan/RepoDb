﻿using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateBatchQueryTest
    {

        private class TestWithoutMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [Test]
        public void TestWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithoutMappingsClass>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [TestWithoutMappingsClass] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithExpressionsClass : DataEntity
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [Test]
        public void TestWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithExpressionsClass>();
            var where = QueryGroup.Parse(new
            {
                Field1 = "Test"
            });
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, where, 0, 10, orderBy);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [TestWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithMultipleOrderedColumnsAndWithoutAttributesClass : DataEntity
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [Test]
        public void TestWithMultipleOrderedColumnsAndWithoutAttributes()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithMultipleOrderedColumnsAndWithoutAttributesClass>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Descending,
                Field2 = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] DESC, [Field2] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [TestWithMultipleOrderedColumnsAndWithoutAttributesClass] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] DESC, [Field2] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithClassMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [Test]
        public void TestWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithClassMappingClass>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [ClassName] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithFieldMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            [Map("Field3")]
            public int Field2 { get; set; }
        }

        [Test]
        public void TestWithFieldMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithFieldMappingsClass>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field3] " +
                $"FROM [ClassName] " +
                $") " +
                $"SELECT [Field1], [Field3] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithFieldMappingsAndWithIgnoredBatchQueryCommandClass : DataEntity
        {
            public int Field1 { get; set; }
            [Attributes.Ignore(Command.BatchQuery)]
            public int Field2 { get; set; }
            public int Field3 { get; set; }
        }

        [Test]
        public void TestWithFieldMappingsAndWithIgnoredBatchQueryCommand()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithFieldMappingsAndWithIgnoredBatchQueryCommandClass>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field3] " +
                $"FROM [ClassName] " +
                $") " +
                $"SELECT [Field1], [Field3] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithFieldMappingsAndWithIgnoredQueryCommandClass : DataEntity
        {
            public int Field1 { get; set; }
            [Attributes.Ignore(Command.Query)]
            public int Field2 { get; set; }
            public int Field3 { get; set; }
        }

        [Test]
        public void TestWithFieldMappingsAndWithIgnoredQueryCommand()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithFieldMappingsAndWithIgnoredQueryCommandClass>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field3] " +
                $"FROM [ClassName] " +
                $") " +
                $"SELECT [Field1], [Field3] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithFieldMappingsAndWithIgnoredBathQueryAndQueryCommandClass : DataEntity
        {
            public int Field1 { get; set; }
            [Attributes.Ignore(Command.Query)]
            public int Field2 { get; set; }
            public int Field3 { get; set; }
            [Attributes.Ignore(Command.BatchQuery)]
            public int Field4 { get; set; }
        }

        [Test]
        public void TestWithFieldMappingsAndWithIgnoredBathQueryAndQueryCommand()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithFieldMappingsAndWithIgnoredBathQueryAndQueryCommandClass>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field3] " +
                $"FROM [ClassName] " +
                $") " +
                $"SELECT [Field1], [Field3] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionIfThereAreNoQueryableFieldsClass : DataEntity
        {
        }

        [Test]
        public void ThrowExceptionIfThereAreNoQueryableFields()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfThereAreNoQueryableFieldsClass>();
            var queryGroup = (QueryGroup)null;

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateBatchQuery(queryBuilder, queryGroup, 0, 10, null));
        }

        private class ThrowExceptionIfAllFieldsWereIgnoredClass : DataEntity
        {
            [Attributes.Ignore(Command.Query)]
            public int Field1 { get; set; }
            [Attributes.Ignore(Command.Query)]
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Query)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfAllFieldsWereIgnored()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfAllFieldsWereIgnoredClass>();
            var queryGroup = (QueryGroup)null;

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateBatchQuery(queryBuilder, queryGroup, 0, 10, null));
        }
    }
}
