namespace MiniOrmAot.Ado.Impl;

public static class AsyncEnumerable {
    public static IAsyncEnumerable<TValue> Empty<TValue>() => EmptyAsyncIterator<TValue>.Instance;

    internal sealed class EmptyAsyncIterator<TValue> : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue> {
        public static readonly EmptyAsyncIterator<TValue> Instance = new EmptyAsyncIterator<TValue>();

        public TValue Current => default!;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);

        public IAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested(); // NB: [LDM-2018-11-28] Equivalent to async iterator behavior.

            return this;
        }

        public ValueTask DisposeAsync() => default;
    }
}