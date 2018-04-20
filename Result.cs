using System;

namespace EitherImpl
{
    public abstract class Result<TSuccess, TFailure>
    {
        public abstract Result<TSuccess, TNewSuccess> Handle<TNewSuccess>(Func<TFailure, TNewSuccess> toSuccess);
        public abstract Result<TNewFailure, TFailure> Handle<TNewFailure>(Func<TSuccess, TNewFailure> toFailure);
        public abstract TSuccess Reduce(Func<TFailure, TSuccess> mapFailureToSuccess);
        public abstract TFailure Reduce(Func<TSuccess, TFailure> mapSuccessToFailure);
    }

    public static class Result
    {
        private sealed class Success<TSuccess, TFailure> : Result<TSuccess, TFailure>
        {
            private readonly TSuccess value;

            public Success(TSuccess value)
            {
                this.value = value;
            }

            public override Result<TSuccess, TNewSuccess> Handle<TNewSuccess>(Func<TFailure, TNewSuccess> toSuccess)
            {
                if (toSuccess == null)
                    throw new ArgumentNullException(nameof(toSuccess));

                return new Success<TSuccess, TNewSuccess>(value);
            }

            public override Result<TNewFailure, TFailure> Handle<TNewFailure>(Func<TSuccess, TNewFailure> toFailure)
            {
                if (toFailure == null)
                    throw new ArgumentNullException(nameof(toFailure));

                return new Success<TNewFailure, TFailure>(toFailure(value));
            }

            public override TSuccess Reduce(Func<TFailure, TSuccess> mapFailureToSuccess)
            {
                return value;
            }

            public override TFailure Reduce(Func<TSuccess, TFailure> mapSuccessToFailure)
            {
                return mapSuccessToFailure(value);
            }
        }

        private sealed class Failure<TSuccess, TFailure> : Result<TSuccess, TFailure>
        {
            private readonly TFailure value;

            public Failure(TFailure value)
            {
                this.value = value;
            }

            public override Result<TSuccess, TNewSuccess> Handle<TNewSuccess>(Func<TFailure, TNewSuccess> toSuccess)
            {
                if (toSuccess == null)
                    throw new ArgumentNullException(nameof(toSuccess));

                return new Failure<TSuccess, TNewSuccess>(toSuccess(value));
            }

            public override Result<TNewFailure, TFailure> Handle<TNewFailure>(Func<TSuccess, TNewFailure> toFailure)
            {
                if (toFailure == null)
                    throw new ArgumentNullException(nameof(toFailure));

                return new Failure<TNewFailure, TFailure>(value);
            }

            public override TSuccess Reduce(Func<TFailure, TSuccess> mapFailureToSuccess)
            {
                return mapFailureToSuccess(value);
            }

            public override TFailure Reduce(Func<TSuccess, TFailure> mapSuccessToFailure)
            {
                return value;
            }
        }

        public static Result<TSuccess, TFailure> Create<TSuccess, TFailure>(TSuccess value)
        {
            return new Success<TSuccess, TFailure>(value);
        }

        public static Result<TSuccess, TFailure> Create<TSuccess, TFailure>(TFailure value)
        {
            return new Failure<TSuccess, TFailure>(value);
        }
    }
}
