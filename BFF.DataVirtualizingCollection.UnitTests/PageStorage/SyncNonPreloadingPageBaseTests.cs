﻿using System;
using System.Reactive.Disposables;
using BFF.DataVirtualizingCollection.PageStorage;
using Xunit;

namespace BFF.DataVirtualizingCollection.Test.PageStorage
{
    public abstract class SyncNonPreloadingPageBaseTestsBase : PageTestsBase
    {
        internal abstract SyncNonPreloadingPageBase<int> PageWithFirstEntry69 { get; }

        internal abstract SyncNonPreloadingPageBase<IDisposable> PageWithDisposable(IDisposable disposable);

        [Fact]
        internal void Dispose_PageHasOneDisposable_DisposesImmediately()
        {
            // Arrange
            var isDisposed = false;
            var disposable = Disposable.Create(() => isDisposed = true);
            var sut = PageWithDisposable(disposable);

            // Act
            sut.Dispose();

            // Assert
            Assert.True(isDisposed);
        }

        [Fact]
        internal void Index_FetchFirstIndex_ReturnsValue()
        {
            // Arrange
            using var sut = PageWithFirstEntry69;

            // Act
            var value = sut[0];

            // Assert
            Assert.Equal(69, value);
        }
    }

    // ReSharper disable once UnusedMember.Global
    public class SyncNonPreloadingNonTaskBasedPageTests : SyncNonPreloadingPageBaseTestsBase
    {
        internal override IPage<int> PageWithPageSizeOne =>
            new SyncNonPreloadingNonTaskBasedPage<int>(0, 1, (offset, pageSize) => new[] { 69 });

        internal override SyncNonPreloadingPageBase<int> PageWithFirstEntry69 =>
            new SyncNonPreloadingNonTaskBasedPage<int>(0, 1, (offset, pageSize) => new[] { 69 });

        internal override SyncNonPreloadingPageBase<IDisposable> PageWithDisposable(IDisposable disposable)
        {
            return new SyncNonPreloadingNonTaskBasedPage<IDisposable>(0, 1, (offset, pageSize) => new[] {disposable});
        }
    }
}
