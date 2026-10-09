using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityDomain.Models
{
    public class Result<TError> where TError : struct, Enum
    {
        public bool IsSuccess { get; }
        public TError? Error { get; }
        public string? Message { get; }

        protected Result(bool isSuccess, TError? error, string? message)
        {
            IsSuccess = isSuccess;
            Error = error;
            Message = message;
        }

        public static Result<TError> Success() => new(true, null, null);

        public static Result<TError> Failure(TError error, string? message = null) => new(false, error, message);
    }


    public sealed class Result<TValue, TError> : Result<TError> where TError : struct, Enum
    {
        public TValue? Value { get; }

        private Result(bool isSuccess, TValue? value, TError? error, string? message): base(isSuccess, error, message) => Value = value;

        public static Result<TValue, TError> Success(TValue value) => new(true, value, null, null);

        public static new Result<TValue, TError> Failure(TError error, string? message = null) => new(false, default, error, message);

    }

}
