using System;

namespace EitherImpl
{
    public abstract class Result<TSuccess, TFailure>
    {
        public abstract Result<TResult, TFailure> Try<TResult>(Func<TSuccess, TResult> mapSuccessfullResult);
        public abstract TSuccess Catch(Func<TFailure, TSuccess> mapFailure);

        public abstract Result<TSuccess, TFailure> Try(Action<TSuccess> doIfSuccess);
        public abstract void Catch(Action<TFailure> doIfFailure);
    }

    /// <summary>
    /// Factory class
    /// </summary>
    public static class Result
    {
        private sealed class Success<TSuccess, TFailure> : Result<TSuccess, TFailure>
        {
            private readonly TSuccess value;

            public Success(TSuccess value)
            {
                this.value = value;
            }

            public override Result<TResult, TFailure> Try<TResult>(Func<TSuccess, TResult> mapSuccessfullResult)
            {
                if (mapSuccessfullResult == null)
                    throw new ArgumentNullException(nameof(mapSuccessfullResult));

                return new Success<TResult, TFailure>(mapSuccessfullResult(value));
            }

            public override Result<TSuccess, TFailure> Try(Action<TSuccess> doIfSuccess)
            {
                if (doIfSuccess == null)
                    throw new ArgumentNullException(nameof(doIfSuccess));

                doIfSuccess(value);

                return this;
            }

            public override TSuccess Catch(Func<TFailure, TSuccess> mapFailure)
            {
                return value;
            }

            public override void Catch(Action<TFailure> doIfFailure)
            {
            }
        }

        private sealed class Failure<TSuccess, TFailure> : Result<TSuccess, TFailure>
        {
            private readonly TFailure value;

            public Failure(TFailure value)
            {
                this.value = value;
            }

            public override Result<TResult, TFailure> Try<TResult>(Func<TSuccess, TResult> mapSuccessfullResult)
            {
                return new Failure<TResult, TFailure>(value);
            }

            public override Result<TSuccess, TFailure> Try(Action<TSuccess> doIfSuccess)
            {
                return this;
            }

            public override TSuccess Catch(Func<TFailure, TSuccess> mapFailure)
            {
                if (mapFailure == null)
                    throw new ArgumentNullException(nameof(mapFailure));

                return mapFailure(value);
            }

            public override void Catch(Action<TFailure> doIfFailure)
            {
                if (doIfFailure == null)
                    throw new ArgumentNullException();

                doIfFailure(value);
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

/*
This is free and unencumbered software released into the public domain.
Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.
In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
For more information, please refer to <http://unlicense.org/>
*/
