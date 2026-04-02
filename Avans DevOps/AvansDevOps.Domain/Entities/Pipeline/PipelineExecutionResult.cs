using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_DevOps.AvansDevOps.Domain.Entities.Pipeline
{
    public class PipelineExecutionResult
    {
        public bool Succeeded { get; }
        public string? ErrorMessage { get; }

        private PipelineExecutionResult(bool succeeded, string? errorMessage)
        {
            Succeeded = succeeded;
            ErrorMessage = errorMessage;
        }

        public static PipelineExecutionResult Success()
        {
            return new PipelineExecutionResult(true, null);
        }

        public static PipelineExecutionResult Failure(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("Error message cannot be empty.", nameof(errorMessage));

            return new PipelineExecutionResult(false, errorMessage);
        }
    }
}
