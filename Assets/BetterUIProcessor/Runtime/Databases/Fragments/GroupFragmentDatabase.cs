using System;
using System.Threading;
using System.Threading.Tasks;
using Better.Attributes.Runtime.Select;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Databases
{
    [Serializable]
    public class GroupFragmentDatabase : IFragmentDatabase
    {
        [Select]
        [SerializeReference] private IFragmentDatabase[] _databases;

        public GroupFragmentDatabase(IFragmentDatabase[] databases)
        {
            if (databases == null)
            {
                throw new ArgumentNullException(nameof(databases));
            }

            _databases = databases;
        }

        public GroupFragmentDatabase() : this(Array.Empty<IFragmentDatabase>())
        {
        }

        async Task<ProcessResult<IFragment>> IFragmentDatabase.TryCreateFragmentAsync(RectTransform container, Type fragmentType, CancellationToken cancellationToken)
        {
            foreach (var database in _databases)
            {
                var result = await database.TryCreateFragmentAsync(container, fragmentType, cancellationToken);
                if (result.IsSuccessful)
                {
                    return result;
                }
            }

            return ProcessResult<IFragment>.Unsuccessful;
        }
    }
}