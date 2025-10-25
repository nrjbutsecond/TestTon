using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper
{
    public class ConflictException :Exception
    {
        public ConflictException(string message) : base(message) { }

    }
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message)
            : base(message)
        {
        }

        public UnauthorizedException()
            : base("Unauthorized access")
        {
        }
    }
    public class BadRequestException : Exception
    {
        public BadRequestException(string message)
            : base(message)
        {
        }
    }

    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }

    public class BusinessRuleException : Exception
    {
        public string RuleName { get; }

        public BusinessRuleException(string ruleName, string message)
            : base(message)
        {
            RuleName = ruleName;
        }
    }

    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {
        }

        public ForbiddenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }





}
