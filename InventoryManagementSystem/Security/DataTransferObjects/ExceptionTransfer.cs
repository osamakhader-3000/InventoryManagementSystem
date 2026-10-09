using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace SecurityApplicationPresentation.DataTransferObjects
{
    public class ExceptionTransfer
    {
        public string? HelpLink { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? Source { get; set; }

        public string? StackTrace { get; set; }
    }
}
