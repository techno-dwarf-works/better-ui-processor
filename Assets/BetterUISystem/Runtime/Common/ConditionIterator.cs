using System;
using System.Collections.Generic;
using UnityEngine;

namespace Better.UISystem.Runtime.Common
{
    [Serializable]
    public class ConditionIterator<TCondition> where TCondition : Condition
    {
        [SerializeReference] private List<TCondition> _conditions;
        public int Count => _conditions.Count;

        public ConditionIterator()
        {
            _conditions = new();
        }

        public bool All(bool state)
        {
            foreach (var condition in _conditions)
            {
                if (condition.Verify() != state)
                {
                    return false;
                }
            }

            return true;
        }

        public bool Any(bool state)
        {
            foreach (var condition in _conditions)
            {
                if (condition.Verify() == state)
                {
                    return true;
                }
            }

            return false;
        }

        public void Add(TCondition condition)
        {
            _conditions.Add(condition);
        }

        public bool Remove(TCondition condition)
        {
            return _conditions.Remove(condition);
        }

        public void Clear()
        {
            _conditions.Clear();
        }
    }
    
    [Serializable]
    public class ConditionIterator : ConditionIterator<Condition>
    {
    }
}