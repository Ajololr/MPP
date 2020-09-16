using System;

namespace ClassLibrary1
{
    public interface IPlugin
    {
        Object Generate();
        Type GetGeneratorType();
    }
}