using System;
using Entin.StaticData.Sheet;

namespace Entin.StaticData.Attributes
{
    public interface IAttributeValidator
    {
        void Validate<TSheet>(StaticData staticData, Action<string> onError) where TSheet : BaseSheet;
    }
}