﻿using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using BFF.DataVirtualizingCollection.DataVirtualizingCollection;

namespace BFF.DataVirtualizingCollection.Test.Integration
{
    internal static class DataVirtualizingCollectionFactory
    {
        internal static IDataVirtualizingCollection<int> CreateCollectionWithIncrementalInteger(
            PageLoadingBehavior pageLoadingBehavior,
            PageRemovalBehavior pageRemovalBehavior,
            FetchersKind fetchersKind,
            IndexAccessBehavior indexAccessBehavior,
            int count,
            int pageSize)
        {
            var pageLoadingBehaviorCollectionBuilder = DataVirtualizingCollectionBuilder<int>.Build(pageSize);
            var pageHoldingBehaviorCollectionBuilder =
                StandardPageHoldingBehaviorCollectionBuilder(pageLoadingBehaviorCollectionBuilder, pageLoadingBehavior);
            var fetchersKindCollectionBuilder =
                StandardFetcherKindCollectionBuilder(
                    pageHoldingBehaviorCollectionBuilder,
                    pageRemovalBehavior,
                    10,
                    1);
            var indexAccessBehaviorCollectionBuilder =
                StandardIndexAccessBehaviorCollectionBuilder(
                    fetchersKindCollectionBuilder,
                    fetchersKind,
                    (offset, pSize) => 
                        Enumerable
                            .Range(offset, pSize)
                            .ToArray(),
                    () => count);

            var dataVirtualizingCollection =
                StandardDataVirtualizingCollection(
                    indexAccessBehaviorCollectionBuilder,
                    indexAccessBehavior,
                    () => -1);
            return dataVirtualizingCollection;
        }
        internal static IDataVirtualizingCollection<int> CreateCollectionWithIncrementalIntegerWhereFetchersIgnorePageSize(
            PageLoadingBehavior pageLoadingBehavior,
            PageRemovalBehavior pageRemovalBehavior,
            FetchersKind fetchersKind,
            IndexAccessBehavior indexAccessBehavior,
            int count,
            int pageSize)
        {
            var pageLoadingBehaviorCollectionBuilder = DataVirtualizingCollectionBuilder<int>.Build(pageSize);
            var pageHoldingBehaviorCollectionBuilder =
                StandardPageHoldingBehaviorCollectionBuilder(pageLoadingBehaviorCollectionBuilder, pageLoadingBehavior);
            var fetchersKindCollectionBuilder =
                StandardFetcherKindCollectionBuilder(
                    pageHoldingBehaviorCollectionBuilder,
                    pageRemovalBehavior,
                    10,
                    1);
            var indexAccessBehaviorCollectionBuilder =
                StandardIndexAccessBehaviorCollectionBuilder(
                    fetchersKindCollectionBuilder,
                    fetchersKind,
                    (offset, pSize) => 
                        Enumerable
                            .Range(offset, pageSize) // <--- This is different! pageSize instead of pSize!
                            .ToArray(),
                    () => count);

            var dataVirtualizingCollection =
                StandardDataVirtualizingCollection(
                    indexAccessBehaviorCollectionBuilder,
                    indexAccessBehavior,
                    () => -1);
            return dataVirtualizingCollection;
        }

        internal static IDataVirtualizingCollection<T> CreateCollectionWithCustomPageFetchingLogic<T>(
            PageLoadingBehavior pageLoadingBehavior,
            PageRemovalBehavior pageRemovalBehavior,
            FetchersKind fetchersKind,
            IndexAccessBehavior indexAccessBehavior,
            int count,
            int pageSize,
            Func<int, int, T[]> pageFetchingLogic,
            T placeholder)
        {
            var pageLoadingBehaviorCollectionBuilder = DataVirtualizingCollectionBuilder<T>.Build(pageSize);
            var pageHoldingBehaviorCollectionBuilder =
                StandardPageHoldingBehaviorCollectionBuilder(pageLoadingBehaviorCollectionBuilder, pageLoadingBehavior);
            var fetchersKindCollectionBuilder =
                StandardFetcherKindCollectionBuilder(
                    pageHoldingBehaviorCollectionBuilder,
                    pageRemovalBehavior,
                    10,
                    1);
            var indexAccessBehaviorCollectionBuilder =
                StandardIndexAccessBehaviorCollectionBuilder(
                    fetchersKindCollectionBuilder,
                    fetchersKind,
                    pageFetchingLogic,
                    () => count);

            var dataVirtualizingCollection =
                StandardDataVirtualizingCollection(
                    indexAccessBehaviorCollectionBuilder,
                    indexAccessBehavior,
                    () => placeholder);
            return dataVirtualizingCollection;
        }

        internal static IDataVirtualizingCollection<T> CreateCollectionWithCustomPageFetchingLogicAndCustomLeastRecentlyUsed<T>(
            PageLoadingBehavior pageLoadingBehavior,
            PageRemovalBehavior pageRemovalBehavior,
            FetchersKind fetchersKind,
            IndexAccessBehavior indexAccessBehavior,
            int count,
            int pageSize,
            Func<int, int, T[]> pageFetchingLogic,
            T placeholder,
            int pageLimit,
            int removalCount)
        {
            var pageLoadingBehaviorCollectionBuilder = DataVirtualizingCollectionBuilder<T>.Build(pageSize);
            var pageHoldingBehaviorCollectionBuilder =
                StandardPageHoldingBehaviorCollectionBuilder(pageLoadingBehaviorCollectionBuilder, pageLoadingBehavior);
            var fetchersKindCollectionBuilder =
                StandardFetcherKindCollectionBuilder(
                    pageHoldingBehaviorCollectionBuilder, 
                    pageRemovalBehavior,
                    pageLimit,
                    removalCount);
            var indexAccessBehaviorCollectionBuilder =
                StandardIndexAccessBehaviorCollectionBuilder(
                    fetchersKindCollectionBuilder,
                    fetchersKind,
                    pageFetchingLogic,
                    () => count);

            var dataVirtualizingCollection =
                StandardDataVirtualizingCollection(
                    indexAccessBehaviorCollectionBuilder,
                    indexAccessBehavior,
                    () => placeholder);
            return dataVirtualizingCollection;
        }

        private static IPageHoldingBehaviorCollectionBuilder<T> StandardPageHoldingBehaviorCollectionBuilder<T>(IPageLoadingBehaviorCollectionBuilder<T> pageLoadingBehaviorCollectionBuilder,
            PageLoadingBehavior pageLoadingBehavior) =>
            pageLoadingBehavior switch
                {
                PageLoadingBehavior.NonPreloading => pageLoadingBehaviorCollectionBuilder.NonPreloading(),
                PageLoadingBehavior.Preloading => pageLoadingBehaviorCollectionBuilder.Preloading(),
                _ => throw new Exception("Test configuration failed!")
                };

        private static IFetchersKindCollectionBuilder<T> StandardFetcherKindCollectionBuilder<T>(IPageHoldingBehaviorCollectionBuilder<T> pageHoldingBehaviorCollectionBuilder,
            PageRemovalBehavior pageRemovalBehavior,
            int pageLimit,
            int removalCount) =>
            pageRemovalBehavior switch
                {
                PageRemovalBehavior.Hoarding => pageHoldingBehaviorCollectionBuilder.Hoarding(),
                PageRemovalBehavior.LeastRecentlyUsed => pageHoldingBehaviorCollectionBuilder.LeastRecentlyUsed(pageLimit, removalCount),
                _ => throw new Exception("Test configuration failed!")
                };

        private static IAsyncOnlyIndexAccessBehaviorCollectionBuilder<T> StandardIndexAccessBehaviorCollectionBuilder<T>(IFetchersKindCollectionBuilder<T> fetchersKindCollectionBuilder,
            FetchersKind fetchersKind,
            Func<int, int, T[]> pageFetcher,
            Func<int> countFetcher) =>
            fetchersKind switch
                {
                FetchersKind.NonTaskBased => fetchersKindCollectionBuilder.NonTaskBasedFetchers(
                    (offset, pSize) =>
                    {
                        Thread.Sleep(25);
                        return pageFetcher(offset, pSize);
                    },
                    () =>
                    {
                        Thread.Sleep(25);
                        return countFetcher();
                    }),
                FetchersKind.TaskBased => fetchersKindCollectionBuilder.TaskBasedFetchers(
                    async (offset, pSize) =>
                    {
                        await Task.Delay(25).ConfigureAwait(false);
                        return pageFetcher(offset, pSize);
                    },
                    async () =>
                    {
                        await Task.Delay(25).ConfigureAwait(false);
                        return countFetcher();
                    }),
                _ => throw new Exception("Test configuration failed!")
                };

        private static IDataVirtualizingCollection<T> StandardDataVirtualizingCollection<T>(IAsyncOnlyIndexAccessBehaviorCollectionBuilder<T> indexAccessBehaviorCollectionBuilder,
            IndexAccessBehavior indexAccessBehavior,
            Func<T> placeholderFactory) =>
            indexAccessBehavior switch
                {
                IndexAccessBehavior.Synchronous => (indexAccessBehaviorCollectionBuilder as IIndexAccessBehaviorCollectionBuilder<T>)?.SyncIndexAccess(new EventLoopScheduler()) ?? throw new Exception("Task-based fetchers and synchronous access is not allowed."),
                IndexAccessBehavior.Asynchronous => indexAccessBehaviorCollectionBuilder.AsyncIndexAccess(
                    (_, __) => placeholderFactory(),
                    new EventLoopScheduler(),
                    new EventLoopScheduler()),
                _ => throw new Exception("Test configuration failed!")
                };
    }
}
