using System;
using System.Threading;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Data;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Interfaces
{
    public interface IFragmentDatabase
    {
        public Task<ProcessResult<IFragment>> TryCreateFragmentAsync(RectTransform container, Type fragmentType, CancellationToken cancellationToken);
    }
}