using System;

namespace GaReporter
{
    public interface IRequestView
    {
        string Title { get; set; }
        string FileName { get; set; }
    }
}
