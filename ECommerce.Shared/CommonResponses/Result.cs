using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.CommonResponses
{
    public class Result
    {
        private readonly bool hasErrors;
        public bool IsFailure => hasErrors;
        public bool IsSuccess => !IsFailure;
        public IEnumerable<Error> Errors { get; private set; } = [];

        protected Result()
        {
            hasErrors = false;
        }

        protected Result(Error error)
        {
            // Errors = new List<Error> { error };
            Errors = [error];
            hasErrors = true;
        }

        protected Result(IEnumerable<Error> errors)
        {
            Errors = errors.ToList();
            hasErrors = true;
        }

        public static Result Ok() => new Result();

        public static Result Fail(Error error) => new Result(error);

        public static Result Fail(IEnumerable<Error> errors) => new Result(errors);
    }

    public class Result<TValue> : Result
    {
        private readonly TValue _data;

        public TValue Data =>
            IsSuccess
                ? _data
                : throw new InvalidOperationException("Cannot reach data of failure response");

        private Result(TValue data)
        {
            _data = data;
        }

        private Result(Error error)
            : base(error)
        {
            _data = default!;
        }

        private Result(IEnumerable<Error> errors)
            : base(errors)
        {
            _data = default!;
        }

        public static Result<TValue> Ok(TValue data) => new Result<TValue>(data);

        public static new Result<TValue> Fail(Error error) => new Result<TValue>(error);

        public static new Result<TValue> Fail(IEnumerable<Error> errors) =>
            new Result<TValue>(errors);
    }
}
