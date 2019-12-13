using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GitWatch.Core
{
    public static class TryHelper
    {
        public static async Task<ITry<T>> TryAsync<T>(this Func<Task> action, Func<T> result)
        {
            try
            {
                await action();
                return new Success<T>(result());
            }
            catch (Exception e)
            {
                return new Failure<T>(e);
            }
        }
    }

    public interface ITry<out T>
    {
        bool IsSuccess { get; }
        bool IsFailure { get; }
        T Get { get; }
        ITry<Exception> Failed { get; }
        ITry<TU> Map<TU>(Func<T, TU> f);
        Task<ITry<TU>> MapAsync<TU>(Func<T, Task<TU>> f);
        ITry<TU> FlatMap<TU>(Func<T, ITry<TU>> f);
        Task<ITry<TU>> FlatMapAsync<TU>(Func<T, Task<ITry<TU>>> f);
        ITry<T> MapFailure(Func<Exception, Exception> f);
        TU Match<TU>(Func<T, TU> success, Func<Exception, TU> failure);
        Task<TU> MatchAsync<TU>(Func<T, Task<TU>> success, Func<Exception, Task<TU>> failure);
    }

    public class Success<T> : ITry<T>
    {
        public Success(T value)
        {
            Get = value;
        }

        public bool IsSuccess => true;
        public bool IsFailure => false;
        public T Get { get; }

        public ITry<Exception> Failed => new Failure<Exception>(new UnsupportedOperationException("Success.Failed"));

        public ITry<TU> Map<TU>(Func<T, TU> f)
        {
            try
            {
                return new Success<TU>(f(Get));
            }
            catch (Exception ex)
            {
                return new Failure<TU>(ex);
            }
        }

        public async Task<ITry<TU>> MapAsync<TU>(Func<T, Task<TU>> f)
        {
            try
            {
                return new Success<TU>(await f(Get));
            }
            catch (Exception ex)
            {
                return new Failure<TU>(ex);
            }
        }

        public ITry<TU> FlatMap<TU>(Func<T, ITry<TU>> f)
        {
            try
            {
                return f(Get);
            }
            catch (Exception ex)
            {
                return new Failure<TU>(ex);
            }
        }

        public async Task<ITry<TU>> FlatMapAsync<TU>(Func<T, Task<ITry<TU>>> f)
        {
            try
            {
                return await f(Get);
            }
            catch (Exception ex)
            {
                return new Failure<TU>(ex);
            }
        }

        public ITry<T> MapFailure(Func<Exception, Exception> f) => this;
        public TU Match<TU>(Func<T, TU> success, Func<Exception, TU> failure) => success(Get);

        public async Task<TU> MatchAsync<TU>(Func<T, Task<TU>> success, Func<Exception, Task<TU>> failure) =>
            await success(Get);
    }

    public class Failure<T> : ITry<T>
    {
        private readonly Exception _exception;

        public Failure(Exception exception)
        {
            _exception = exception;
        }
        public Failure(FlurlHttpException exception)
        {
            _exception = exception;
        }
        public bool IsSuccess => false;
        public bool IsFailure => true;
        public T Get => throw _exception;
        public ITry<Exception> Failed => new Success<Exception>(_exception);
        public ITry<TU> Map<TU>(Func<T, TU> f) => new Failure<TU>(_exception);

        public async Task<ITry<TU>> MapAsync<TU>(Func<T, Task<TU>> f) =>
            await Task.FromResult(new Failure<TU>(_exception));

        public ITry<TU> FlatMap<TU>(Func<T, ITry<TU>> f) => new Failure<TU>(_exception);

        public async Task<ITry<TU>> FlatMapAsync<TU>(Func<T, Task<ITry<TU>>> f) =>
            await Task.FromResult(new Failure<TU>(_exception));

        public ITry<T> MapFailure(Func<Exception, Exception> f) => new Failure<T>(f(_exception));
        public TU Match<TU>(Func<T, TU> success, Func<Exception, TU> failure) => failure(_exception);

        public async Task<TU> MatchAsync<TU>(Func<T, Task<TU>> success, Func<Exception, Task<TU>> failure) =>
            await failure(_exception);
    }

    public static class Try
    {
        public static ITry<T> Enclose<T>(Func<T> f)
        {
            try
            {
                return new Success<T>(f());
            }
            catch (Exception ex)
            {
                return new Failure<T>(ex);
            }
        }

        public static async Task<ITry<T>> EncloseAsync<T>(Func<Task<T>> f)
        {
            try
            {
                return new Success<T>(await f());
            }
            catch (Exception ex)
            {
                return new Failure<T>(ex);
            }
        }

        public static async Task<ITry<T>> EncloseAsyncFlurl<T>(Func<Task<T>> f)
        {
            try
            {
                return new Success<T>(await f());
            }
            catch (FlurlHttpException ex)
            {
                return new Failure<T>(ex);
            }
        }
    }

    public class UnsupportedOperationException : Exception
    {
        public UnsupportedOperationException(string message) : base(message)
        {
        }
    }

    public class NoSuchElementException : Exception
    {
        public NoSuchElementException(string message) : base(message)
        {
        }
    }

    public interface IOption<T>
    {
        bool IsEmpty { get; }
        bool IsDefined { get; }
        T Get { get; }
        T GetOrElse(Func<T> @default);
        IOption<TU> Map<TU>(Func<T, TU> f);
        Task<IOption<TU>> MapAsync<TU>(Func<T, Task<TU>> f);
        TU Match<TU>(Func<T, TU> some, Func<TU> none);
        Task<TU> MatchAsync<TU>(Func<T, Task<TU>> some, Func<Task<TU>> none);
    }

    public class Some<T> : IOption<T>
    {
        public Some(T value)
        {
            Get = value;
        }

        public bool IsEmpty => false;
        public bool IsDefined => true;
        public T Get { get; }

        public T GetOrElse(Func<T> @default) => Get;
        public IOption<TU> Map<TU>(Func<T, TU> f) => new Some<TU>(f(Get));
        public async Task<IOption<TU>> MapAsync<TU>(Func<T, Task<TU>> f) => new Some<TU>(await f(Get));
        public TU Match<TU>(Func<T, TU> some, Func<TU> none) => some(Get);
        public async Task<TU> MatchAsync<TU>(Func<T, Task<TU>> some, Func<Task<TU>> none) => await some(Get);
    }

    public class None<T> : IOption<T>
    {
        public bool IsEmpty => true;
        public bool IsDefined => false;
        public T Get => throw new NoSuchElementException("None.Get");
        public T GetOrElse(Func<T> @default) => @default();
        public IOption<TU> Map<TU>(Func<T, TU> f) => new None<TU>();
        public async Task<IOption<TU>> MapAsync<TU>(Func<T, Task<TU>> f) => await Task.FromResult(new None<TU>());
        public TU Match<TU>(Func<T, TU> some, Func<TU> none) => none();
        public async Task<TU> MatchAsync<TU>(Func<T, Task<TU>> some, Func<Task<TU>> none) => await none();
    }

    public static class OptionHelper
    {
        public static IOption<T> AsOption<T>(this T t) where T : class
        {
            if (t == null)
                return new None<T>();
            return new Some<T>(t);
        }
    }

    public static class QueryableExtensions
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Op
        {
            Equals,
            GreaterThan,
            LessThan,
            GreaterThanOrEqual,
            LessThanOrEqual,
            Contains,
            StartsWith,
            EndsWith
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum Order
        {
            Asc,
            Desc
        }

        public class Filter
        {
            public string PropertyName { get; set; }
            public Op Operation { get; set; }
            public object Value { get; set; }
        }

        public static IQueryable<T> OrderByDynamic<T>(
            this IQueryable<T> query,
            string orderByMember,
            Order direction)
        {
            var queryElementTypeParam = Expression.Parameter(typeof(T));

            var memberAccess = Expression.PropertyOrField(queryElementTypeParam, orderByMember);

            var keySelector = Expression.Lambda(memberAccess, queryElementTypeParam);

            var orderBy = Expression.Call(
                typeof(Queryable),
                direction == Order.Asc ? "OrderBy" : "OrderByDescending",
                new[] { typeof(T), memberAccess.Type },
                query.Expression,
                Expression.Quote(keySelector));

            return query.Provider.CreateQuery<T>(orderBy);
        }

        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperties()
                .Single(pi => pi.Name == propertyName)
                .GetValue(obj, null);
        }
    }
}
