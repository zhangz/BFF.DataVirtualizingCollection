using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using Xunit;

namespace BFF.DataVirtualizingCollection.IntegrationTests.DataVirtualizingCollectionDataAccess
{
    public class SyncDataAccessTests
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<object[]> Combinations =>
            Enum.GetValues(typeof(PageLoadingBehavior)).OfType<PageLoadingBehavior>()
                .Cartesian(
                    Enum.GetValues(typeof(PageRemovalBehavior)).OfType<PageRemovalBehavior>(), 
                    Enum.GetValues(typeof(FetchersKind)).OfType<FetchersKind>().Except(new [] { FetchersKind.TaskBased }),
                    (first, second, third) =>
                        new object[] {first, second, third, IndexAccessBehavior.Synchronous});

        [Theory]
        [MemberData(nameof(Combinations))]
        public void BuildingCollectionWith6969Elements_FirstEntry_0(
            PageLoadingBehavior pageLoadingBehavior,
            PageRemovalBehavior pageRemovalBehavior,
            FetchersKind fetchersKind,
            IndexAccessBehavior indexAccessBehavior)
        {
            // Arrange
            using var collection = DataVirtualizingCollectionFactory.CreateCollectionWithIncrementalInteger(
                pageLoadingBehavior,
                pageRemovalBehavior,
                fetchersKind,
                indexAccessBehavior,
                6969,
                100);

            // Act + Assert
            Assert.Equal(0, collection[0]);
        }

        [Theory]
        [MemberData(nameof(Combinations))]
        public void BuildingCollectionWith6969Elements_70thEntry_69(
            PageLoadingBehavior pageLoadingBehavior,
            PageRemovalBehavior pageRemovalBehavior,
            FetchersKind fetchersKind,
            IndexAccessBehavior indexAccessBehavior)
        {
            // Arrange
            using var collection = DataVirtualizingCollectionFactory.CreateCollectionWithIncrementalInteger(
                pageLoadingBehavior,
                pageRemovalBehavior,
                fetchersKind,
                indexAccessBehavior,
                6969,
                100);

            // Act + Assert
            Assert.Equal(69, collection[69]);
        }

        [Theory]
        [MemberData(nameof(Combinations))]
        public void BuildingCollectionWith6969Elements_124thEntry_123(
            PageLoadingBehavior pageLoadingBehavior,
            PageRemovalBehavior pageRemovalBehavior,
            FetchersKind fetchersKind,
            IndexAccessBehavior indexAccessBehavior)
        {
            // Arrange
            using var collection = DataVirtualizingCollectionFactory.CreateCollectionWithIncrementalInteger(
                pageLoadingBehavior,
                pageRemovalBehavior,
                fetchersKind,
                indexAccessBehavior,
                6969,
                100);

            // Act + Assert
            Assert.Equal(123, collection[123]);
        }

        [Theory]
        [MemberData(nameof(Combinations))]
        public void BuildingCollectionWith6969Elements_6001thEntry_6000(
            PageLoadingBehavior pageLoadingBehavior,
            PageRemovalBehavior pageRemovalBehavior,
            FetchersKind fetchersKind,
            IndexAccessBehavior indexAccessBehavior)
        {
            // Arrange
            using var collection = DataVirtualizingCollectionFactory.CreateCollectionWithIncrementalInteger(
                pageLoadingBehavior,
                pageRemovalBehavior,
                fetchersKind,
                indexAccessBehavior,
                6969,
                100);

            // Act + Assert
            Assert.Equal(6000, collection[6000]);
        }

        [Theory]
        [MemberData(nameof(Combinations))]
        public void BuildingCollectionWith6969Elements_6969thEntry_6968(
            PageLoadingBehavior pageLoadingBehavior,
            PageRemovalBehavior pageRemovalBehavior,
            FetchersKind fetchersKind,
            IndexAccessBehavior indexAccessBehavior)
        {
            // Arrange
            using var collection = DataVirtualizingCollectionFactory.CreateCollectionWithIncrementalInteger(
                pageLoadingBehavior,
                pageRemovalBehavior,
                fetchersKind,
                indexAccessBehavior,
                6969,
                100);

            // Act + Assert
            Assert.Equal(6968, collection[6968]);
        }

        [Theory]
        [MemberData(nameof(Combinations))]
        public void BuildingCollectionWith6969Elements_6970thEntry_ThrowsIndexOutOfRangeException(
            PageLoadingBehavior pageLoadingBehavior,
            PageRemovalBehavior pageRemovalBehavior,
            FetchersKind fetchersKind,
            IndexAccessBehavior indexAccessBehavior)
        {
            // Arrange
            using var collection = DataVirtualizingCollectionFactory.CreateCollectionWithIncrementalInteger(
                pageLoadingBehavior,
                pageRemovalBehavior,
                fetchersKind,
                indexAccessBehavior,
                6969,
                100);

            // Act + Assert
            Assert.Throws<IndexOutOfRangeException>(() => collection[6969]);
        }

        [Theory]
        [MemberData(nameof(Combinations))]
        public void BuildingCollectionWith6969Elements_MinusFirstEntry_ThrowsIndexOutOfRangeException(
            PageLoadingBehavior pageLoadingBehavior,
            PageRemovalBehavior pageRemovalBehavior,
            FetchersKind fetchersKind,
            IndexAccessBehavior indexAccessBehavior)
        {
            // Arrange
            using var collection = DataVirtualizingCollectionFactory.CreateCollectionWithIncrementalInteger(
                pageLoadingBehavior,
                pageRemovalBehavior,
                fetchersKind,
                indexAccessBehavior,
                6969,
                100);

            // Act + Assert
            Assert.Throws<IndexOutOfRangeException>(() => collection[-1]);
        }

        [Theory]
        [MemberData(nameof(Combinations))]
        public void BuildingCollectionWherePageFetcherIgnoresGivenPageSize23_70thEntry_69(
            PageLoadingBehavior pageLoadingBehavior,
            PageRemovalBehavior pageRemovalBehavior,
            FetchersKind fetchersKind,
            IndexAccessBehavior indexAccessBehavior)
        {
            // Arrange
            using var collection = DataVirtualizingCollectionFactory.CreateCollectionWithIncrementalIntegerWhereFetchersIgnorePageSize(
                pageLoadingBehavior,
                pageRemovalBehavior,
                fetchersKind,
                indexAccessBehavior,
                6969,
                23);

            // Act + Assert
            Assert.Equal(69, collection[69]);
        }
    }
}
